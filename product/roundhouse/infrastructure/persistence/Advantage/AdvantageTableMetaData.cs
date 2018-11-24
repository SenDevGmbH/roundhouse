using NHibernate.Dialect.Schema;
using System;
using System.Data;
using System.Linq;

namespace roundhouse.infrastructure.persistence
{
    public class AdvantageTableMetaData : AbstractTableMetadata
    {
        public AdvantageTableMetaData(DataRow rs, IDataBaseSchema meta, bool extras) : base(rs, meta, extras) { }

        protected override IColumnMetadata GetColumnMetadata(DataRow rs)
        {
            return new AvantageColumnMetaData(rs);
        }

        protected override string GetColumnName(DataRow rs)
        {
            string columnName;
            if (rs.Table.Columns.OfType<DataColumn>().Any(c => string.Equals(c.ColumnName, "ColumnName", StringComparison.OrdinalIgnoreCase)))
                columnName = "ColumnName";
            else
                columnName = "Name";
            return Convert.ToString(rs[columnName]);
        }

        protected override string GetConstraintName(DataRow rs)
        {
            // There is no thing like a constraint name for ASA9 - so
            // we just use the column name here ...
            return Convert.ToString(rs["COLUMN_NAME"]);
        }

        protected override IForeignKeyMetadata GetForeignKeyMetadata(DataRow rs)
        {
            return new AdvantageForeignKeyMetaData(rs);
        }

        protected override IIndexMetadata GetIndexMetadata(DataRow rs)
        {
            return new AdvantageIndexMetaData(rs);
        }

        protected override string GetIndexName(DataRow rs)
        {
            return (string)rs["Name"];
        }

        protected override void ParseTableInfo(DataRow rs)
        {
            Catalog = null;
            Schema = null;
            Name = Convert.ToString(rs["TABLE_NAME"]);
        }
    }

}