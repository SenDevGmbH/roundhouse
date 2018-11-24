using Iesi.Collections.Generic;
using NHibernate.Dialect.Schema;
using System;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace roundhouse.infrastructure.persistence
{
    public class AdvantageDataBaseMetaData : AbstractDataBaseSchema
    {
        public AdvantageDataBaseMetaData(DbConnection pObjConnection) : base(pObjConnection) { }

        public override ITableMetadata GetTableMetadata(DataRow rs, bool extras)
        {
            return new AdvantageTableMetaData(rs, this, extras);
        }

        public override ISet<string> GetReservedWords()
        {
            var result = new HashedSet<string>();
            DataTable dtReservedWords = Connection.GetSchema(DbMetaDataCollectionNames.ReservedWords);
            foreach (DataRow row in dtReservedWords.Rows)
            {
                result.Add(row["ReservedWord"].ToString());
            }
            return result;
        }

        public override DataTable GetTables(string catalog, string schemaPattern, string tableNamePattern, string[] types)
        {
            DataTable objTbl = Connection.GetSchema("Tables");
            var notMatchingRows = objTbl.Rows.OfType<DataRow>().Where(r => !string.Equals(r["TABLE_NAME"]?.ToString(), tableNamePattern, StringComparison.OrdinalIgnoreCase)).ToList();
            notMatchingRows.ForEach(r => objTbl.Rows.Remove(r));
            return objTbl;
        }

        public override DataTable GetIndexInfo(string catalog, string schemaPattern, string tableName)
        {
            var restrictions = new[] { null, null,tableName, null};
            DataTable objTbl = Connection.GetSchema("Indexes", restrictions);
            return objTbl;
        }

        public override DataTable GetIndexColumns(string catalog, string schemaPattern, string tableName, string indexName)
        {
            var restrictions = new[] { null, null, tableName, indexName};
            DataTable objTbl = Connection.GetSchema("IndexColumns", restrictions);
            return objTbl;
        }

        public override DataTable GetColumns(string catalog, string schemaPattern, string tableNamePattern,
                                                string columnNamePattern)
        {
            var restrictions = new[] { null, null, tableNamePattern };
            DataTable objTbl = Connection.GetSchema("Columns", restrictions);
            return objTbl;
        }

        public override DataTable GetForeignKeys(string catalog, string schema, string table)
        {
            var restrictions = new[] { null, null, table };
            DataTable objTbl = Connection.GetSchema("ForeignKeys", restrictions);
            return objTbl;
        }
    }

}