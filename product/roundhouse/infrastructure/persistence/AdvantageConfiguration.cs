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
                while(reader.Read())
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
            var restrictions = new[] { schemaPattern, tableNamePattern, null };
            DataTable objTbl = Connection.GetSchema("Tables", restrictions);
            return objTbl;
        }

        public override DataTable GetIndexInfo(string catalog, string schemaPattern, string tableName)
        {
            var restrictions = new[] { schemaPattern, tableName, null };
            DataTable objTbl = Connection.GetSchema("Indexes", restrictions);
            return objTbl;
        }

        public override DataTable GetIndexColumns(string catalog, string schemaPattern, string tableName, string indexName)
        {
            var restrictions = new[] { schemaPattern, tableName, indexName, null };
            DataTable objTbl = Connection.GetSchema("IndexColumns", restrictions);
            return objTbl;
        }

        public override DataTable GetColumns(string catalog, string schemaPattern, string tableNamePattern,
                                                string columnNamePattern)
        {
            var restrictions = new[] { schemaPattern, tableNamePattern, null };
            DataTable objTbl = Connection.GetSchema("Columns", restrictions);
            return objTbl;
        }

        public override DataTable GetForeignKeys(string catalog, string schema, string table)
        {
            var restrictions = new[] { schema, table, null };
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
            return Convert.ToString(rs["COLUMN_NAME"]);
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
            return (string)rs["INDEX_NAME"];
        }

        protected override void ParseTableInfo(DataRow rs)
        {
            Catalog = null;
            Schema = Convert.ToString(rs["TABLE_SCHEMA"]);
            if (string.IsNullOrEmpty(Schema))
            {
                Schema = null;
            }
            Name = Convert.ToString(rs["TABLE_NAME"]);
        }
    }

    public class AvantageColumnMetaData : AbstractColumnMetaData
    {
        public AvantageColumnMetaData(DataRow rs) : base(rs)
        {
            Name = Convert.ToString(rs["COLUMN_NAME"]);

            this.SetColumnSize(rs["COLUMN_SIZE"]);
            this.SetNumericalPrecision(rs["PRECISION"]);

            Nullable = Convert.ToString(rs["IS_NULLABLE"]);
            TypeName = Convert.ToString(rs["DATA_TYPE"]);
        }
    }

    public class AdvantageIndexMetaData : AbstractIndexMetadata
    {
        public AdvantageIndexMetaData(DataRow rs) : base(rs)
        {
            Name = (string)rs["INDEX_NAME"];
        }
    }

    public class AdvantageForeignKeyMetaData : AbstractForeignKeyMetadata
    {
        public AdvantageForeignKeyMetaData(DataRow rs) : base(rs)
        {
            // There is no thing like a constraint name for ASA9 - so
            // we just use the column name here ...
            Name = (string)rs["COLUMN_NAME"];
        }
    }

}