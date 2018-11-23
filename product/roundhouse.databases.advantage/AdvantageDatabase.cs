namespace roundhouse.databases.advantage
{
    using infrastructure.app;
    using infrastructure.extensions;
    using infrastructure.logging;
    using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
    using System;
    using System.Data;
    using System.Data.Common;

    public class AdvantageDatabase : AdoNetDatabase
    {


        public override string sql_statement_separator_regex_pattern => null;

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
        
        public override void run_database_specific_tasks()
        {
        }


        public override bool create_database_if_it_doesnt_exist(string custom_create_database_script)
        {
            return true;
        }
        public override string create_database_script() => string.Empty;

        public override string set_recovery_mode_script(bool simple) => string.Empty;

        public override string restore_database_script(string restore_from_path, string custom_restore_options)
        {
            throw new NotSupportedException();
        }

    }
}