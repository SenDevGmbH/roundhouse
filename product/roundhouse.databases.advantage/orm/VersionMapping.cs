namespace roundhouse.databases.advantage.orm
{
    using FluentNHibernate.Mapping;
    using infrastructure;
    using System;

    [CLSCompliant(false)]
    public class VersionMapping : ClassMap<roundhouse.model.Version>
    {
        public VersionMapping()
        {
            Table(MappingHelper.GetTableName(ApplicationParameters.CurrentMappings.version_table_name));
            Not.LazyLoad();
            HibernateMapping.DefaultAccess.Property();
            HibernateMapping.DefaultCascade.SaveUpdate();

            Id(x => x.id).Column("id").CustomSqlType("integer").GeneratedBy.Identity().UnsavedValue(0);
            Map(x => x.repository_path).Length(1024);
            Map(x => x.version).Length(50);

            //audit
            Map(x => x.entry_date);
            Map(x => x.modified_date);
            Map(x => x.entered_by).Length(50);
        }
    }
}