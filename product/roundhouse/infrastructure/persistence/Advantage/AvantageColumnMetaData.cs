using NHibernate.Dialect.Schema;
using System;
using System.Data;

namespace roundhouse.infrastructure.persistence
{
    public class AvantageColumnMetaData : AbstractColumnMetaData
    {
        public AvantageColumnMetaData(DataRow rs) : base(rs)
        {
            Name = Convert.ToString(rs["ColumnName"]);

            this.SetColumnSize(rs["ColumnSize"]);
            this.SetNumericalPrecision(rs["NumericPrecision"]);

            Nullable = Convert.ToString(rs["AllowDBNull"]);
            TypeName = Convert.ToString(rs["DataType"]);
        }
    }

}