using NHibernate.Driver;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using System.Data;
using System.Data.Common;

namespace roundhouse.infrastructure.persistence
{
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

}