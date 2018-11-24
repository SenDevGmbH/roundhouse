using FluentNHibernate.Cfg.Db;

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

}