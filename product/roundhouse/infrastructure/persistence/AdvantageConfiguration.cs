using System;
using System.Data;
using System.Data.Common;
using System.Globalization;
using FluentNHibernate.Cfg.Db;
using NHibernate.Dialect;
using NHibernate.Driver;

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

        public override bool UseNamedPrefixInParameter { get { return true; } }

        public override string NamedPrefix { get { return "@"; } }
    }

    public class AdvantageDialect : Dialect
    {

    }

}