namespace roundhouse.infrastructure.app.builders
{
    using databases;
    using filesystem;
    using loaders;
    using System;
    using System.Globalization;
    using System.Reflection;
    using System.Security.Principal;

    public static class DatabaseBuilder
    {
        private static string merge_assembly_name = "rh";

        public static Database build(FileSystemAccess file_system, ConfigurationPropertyHolder configuration_property_holder)
        {
            Database database_to_migrate;

            if (Assembly.GetExecutingAssembly().Location.Contains("roundhouse.dll"))
            {
                merge_assembly_name = "roundhouse";
            }

            string database_type = configuration_property_holder.DatabaseType;
            string typeNameWithoutAssembly = database_type.Substring(0, database_type.IndexOf(','));
            database_to_migrate =
                DefaultInstanceCreator.create_object_from_string_type<Database>(typeNameWithoutAssembly + ", " + merge_assembly_name) ??
                DefaultInstanceCreator.create_object_from_string_type<Database>(configuration_property_holder.DatabaseType);

            if(database_to_migrate == null)
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                    "A type could not be created from the object you passed. \"{0}\" resolves to null.", database_type));


            if (restore_from_file_ends_with_LiteSpeed_extension(file_system, configuration_property_holder.RestoreFromPath))
            {
                database_to_migrate = new SqlServerLiteSpeedDatabase(database_to_migrate);
            }
            database_to_migrate.configuration = configuration_property_holder;
            database_to_migrate.server_name = configuration_property_holder.ServerName ?? string.Empty;
            database_to_migrate.database_name = configuration_property_holder.DatabaseName ?? string.Empty;
            database_to_migrate.connection_string = configuration_property_holder.ConnectionString;
            database_to_migrate.admin_connection_string = configuration_property_holder.ConnectionStringAdmin;
            database_to_migrate.roundhouse_schema_name = configuration_property_holder.SchemaName;
            database_to_migrate.version_table_name = configuration_property_holder.VersionTableName;
            database_to_migrate.scripts_run_table_name = configuration_property_holder.ScriptsRunTableName;
            database_to_migrate.scripts_run_errors_table_name = configuration_property_holder.ScriptsRunErrorsTableName;
            database_to_migrate.user_name = get_identity_of_person_running_roundhouse();
            database_to_migrate.command_timeout = configuration_property_holder.CommandTimeout;
            database_to_migrate.admin_command_timeout = configuration_property_holder.CommandTimeoutAdmin;
            database_to_migrate.restore_timeout = configuration_property_holder.RestoreTimeout;

            return database_to_migrate;
        }

        private static string get_identity_of_person_running_roundhouse()
        {
            string identity_of_runner = string.Empty;
            WindowsIdentity windows_identity = WindowsIdentity.GetCurrent();
            if (windows_identity != null)
            {
                identity_of_runner = windows_identity.Name;
            }

            return identity_of_runner;
        }

        private static bool restore_from_file_ends_with_LiteSpeed_extension(FileSystemAccess file_system, string restore_path)
        {
            if (string.IsNullOrEmpty(restore_path)) return false;

            return file_system.get_file_name_without_extension_from(restore_path).ToLower().EndsWith("ls");
        }
    }
}