using NHibernate.Dialect.Schema;
using System.Data;

namespace roundhouse.infrastructure.persistence
{
    public class AdvantageForeignKeyMetaData : AbstractForeignKeyMetadata
    {
        public AdvantageForeignKeyMetaData(DataRow rs) : base(rs)
        {
            Name = (string)rs["COLUMN_NAME"];
        }
    }

}