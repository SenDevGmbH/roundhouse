namespace roundhouse.databases.advantage
{
    using connections;
    using infrastructure.app;
    using infrastructure.extensions;
    using infrastructure.logging;
    using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Text;

    public class AdvantageDatabase : AdoNetDatabase
    {
        public AdvantageDatabase()
        {
            // Retry upto 5 times with exponential backoff before giving up
            retry_policy = new RetryPolicy(
                new TransientErrorDetectionStrategy(),
                5,
                minBackoff: TimeSpan.FromSeconds(5),
                maxBackoff: TimeSpan.FromMinutes(2),
                deltaBackoff: TimeSpan.FromSeconds(5));

            retry_policy.Retrying += (sender, args) => log_command_retrying(args);
        }

        private string connect_options = "Integrated Security=SSPI;";

        public override string sql_statement_separator_regex_pattern
        {
            get
            {
                return null;
            }
        }

        public override void initialize_connections(ConfigurationPropertyHolder configuration_property_holder)
        {


            set_provider();


            var factory = DbProviderFactories.GetFactory(provider);
            if (!string.IsNullOrEmpty(connection_string))
            {
                var csb = factory.CreateConnectionStringBuilder();
                csb.ConnectionString = connection_string;
                server_name = csb["Data Source"]?.ToString();
            }

            configuration_property_holder.ConnectionString = connection_string;

        }

        public override void set_provider()
        {
            provider = "Advantage.Data.Provider";
        }


        protected override AdoNetConnection GetAdoNetConnection(string conn_string)
        {
            var connection_retry_policy = new RetryPolicy<TransientErrorDetectionStrategy>(
                5, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
            connection_retry_policy.Retrying += (sender, args) => log_connection_retrying(args);

            // Command retry policy is only used when ReliableSqlConnection.ExecuteCommand helper methods are explicitly invoked.
            // This is not our case, as those method are not used.
            var command_retry_policy = RetryPolicy.NoRetry;

            var connection = new ReliableSqlConnection(conn_string, connection_retry_policy, command_retry_policy);

            connection_specific_setup(connection);
            return new AdoNetConnection(connection);
        }

        protected override void connection_specific_setup(IDbConnection connection)
        {
            ((ReliableSqlConnection)connection).Current.InfoMessage += (sender, e) => Log.bound_to(this).log_a_debug_event_containing("  [SQL PRINT]: {0}{1}", Environment.NewLine, e.Message);
        }

        public override void run_database_specific_tasks()
        {
        }


        public override string create_database_script() => string.Empty;

        public override string set_recovery_mode_script(bool simple) => string.Empty;

        public override string restore_database_script(string restore_from_path, string custom_restore_options)
        {
            throw new NotSupportedException();
        }

        public override string delete_database_script()
        {
            return string.Format(
                @"USE master
                        DECLARE @azure_engine INT = 5
                        IF EXISTS(SELECT * FROM sys.databases WHERE [name] = '{0}' AND source_database_id is NULL) AND ISNULL(SERVERPROPERTY('EngineEdition'), 0) <> @azure_engine
                        BEGIN
                            ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE
                        END

                        IF EXISTS(SELECT * FROM sys.databases WHERE [name] = '{0}') 
                        BEGIN
                            IF ISNULL(SERVERPROPERTY('EngineEdition'), 0) <> @azure_engine
                            BEGIN
                                EXEC sp_executesql N'EXEC msdb.dbo.sp_delete_database_backuphistory @database_name = ''{0}'''
                            END

                            DROP DATABASE [{0}] 
                        END",
                database_name);
        }

        /// <summary>
        /// Low level hit to query the database for a restore
        /// </summary>
        private DataTable execute_datatable(string sql_to_run)
        {
            DataSet result = new DataSet();

            using (IDbCommand command = setup_database_command(sql_to_run, ConnectionType.Default, null))
            {
                using (IDataReader data_reader = command.ExecuteReader())
                {
                    DataTable data_table = new DataTable();
                    data_table.Load(data_reader);
                    data_reader.Close();
                    data_reader.Dispose();

                    result.Tables.Add(data_table);
                }
                command.Dispose();
            }

            return result.Tables.Count == 0 ? null : result.Tables[0];
        }

        private void log_connection_retrying(RetryingEventArgs args)
        {
            Log.bound_to(this).log_a_warning_event_containing(
                "Failure opening connection, trying again (current retry count:{0}){1}{2}",
                args.CurrentRetryCount,
                Environment.NewLine,
                args.LastException.to_string());
        }

        private void log_command_retrying(RetryingEventArgs args)
        {
            Log.bound_to(this).log_a_warning_event_containing(
                "Failure executing command, trying again (current retry count:{0}){1}{2}",
                args.CurrentRetryCount,
                Environment.NewLine,
                args.LastException.to_string());
        }
    }
}