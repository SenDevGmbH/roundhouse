namespace roundhouse.databases.advantage.orm
{
    using System.Globalization;
    using infrastructure;

    static class MappingHelper
    {
        internal static string GetTableName(string tableName)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}_{1}", ApplicationParameters.CurrentMappings.roundhouse_schema_name, tableName);
        }
    }
}