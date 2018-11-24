using NHibernate.Dialect;
using NHibernate.Dialect.Schema;
using System.Data;
using System.Data.Common;
using System.Globalization;

namespace roundhouse.infrastructure.persistence
{
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

}