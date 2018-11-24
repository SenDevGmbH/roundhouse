using NHibernate.Dialect.Schema;
using System.Data;

namespace roundhouse.infrastructure.persistence
{
    public class AdvantageIndexMetaData : AbstractIndexMetadata
    {
        public AdvantageIndexMetaData(DataRow rs) : base(rs)
        {
            Name = (string)rs["Name"];
        }
    }

}