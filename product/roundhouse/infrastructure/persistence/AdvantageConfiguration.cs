using FluentNHibernate.Cfg.Db;
using Iesi.Collections.Generic;
using NHibernate.Dialect;
using NHibernate.Dialect.Schema;
using NHibernate.Driver;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using System;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;

namespace roundhouse.infrastructure.persistence
{

    public class AdvantageConfiguration : PersistenceConfiguration<AdvantageConfiguration, SybaseSQLAnywhereConnectionStringBuilder>
    {
        protected AdvantageConfiguration()
        {
        }

        public static AdvantageConfiguration Standard
        {
            get
            {
                return new AdvantageConfiguration().Dialect<AdvantageDialect>().Driver<AdvantageDriver>();
            }
        }
    }

    public class AdvantageDriver : DriverBase
    {
        private const string providerName = "Advantage.Data.Provider";

        private readonly DbProviderFactory factory;


        public AdvantageDriver()
        {
            factory = DbProviderFactories.GetFactory(providerName);
        }
        public override IDbCommand CreateCommand()
        {
            return factory.CreateCommand();
        }


        protected override void InitializeParameter(IDbDataParameter dbParam, string name, SqlType sqlType)
        {
            if (!name.StartsWith(":")) name = ":" + name;
            base.InitializeParameter(dbParam, name, sqlType);
        }
        public override IDbCommand GenerateCommand(CommandType type, SqlString sqlString, SqlType[] parameterTypes)
        {
            var command = base.GenerateCommand(type, sqlString, parameterTypes);

            return command;
        }

        public override void AdjustCommand(IDbCommand command)
        {
            base.AdjustCommand(command);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                }
            }
        }
        public override IDbConnection CreateConnection()
        {
            return factory.CreateConnection();
        }

        public override bool UseNamedPrefixInSql
        {
            get
            {
                return true;
            }
        }

        public override bool UseNamedPrefixInParameter { get { return false; } }

        public override string NamedPrefix { get { return ":"; } }

    }

    public class AdvantageDialect : Dialect
    {


        public AdvantageDialect()
        {
            RegisterColumnType(DbType.String, string.Format(CultureInfo.InvariantCulture, "NVarChar({0})", TypeNames.LengthPlaceHolder));
            RegisterColumnType(DbType.DateTime, "timestamp");
            RegisterColumnType(DbType.Int64, "rowversion");
            RegisterColumnType(DbType.Boolean, "logical");

        }

        public override string AddColumnString => "add";

        public override bool SupportsIdentityColumns { get { return true; } }

        public override bool HasDataTypeInIdentityColumn
        {
            get
            {
                return false;
            }
        }
        public override string GetIdentityColumnString(DbType type)
        {
            return "AutoInc";
        }

        public override string IdentitySelectString
        {
            get
            {
                return "Select top 1 LastAutoInc(CONNECTION) from system.objects";
            }
        }
        public override IDataBaseSchema GetDataBaseSchema(DbConnection connection)
        {
            return new AdvantageDataBaseMetaData(connection);
        }
    }

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

    public class AdvantageIndexMetaData : AbstractIndexMetadata
    {
        public AdvantageIndexMetaData(DataRow rs) : base(rs)
        {
            Name = (string)rs["Name"];
        }
    }

    public class AdvantageForeignKeyMetaData : AbstractForeignKeyMetadata
    {
        public AdvantageForeignKeyMetaData(DataRow rs) : base(rs)
        {
            Name = (string)rs["COLUMN_NAME"];
        }
    }

}