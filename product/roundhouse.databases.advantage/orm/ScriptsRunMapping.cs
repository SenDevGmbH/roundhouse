namespace roundhouse.databases.advantage.orm
{
    using FluentNHibernate.Mapping;
    using infrastructure;
    using model;
    using System;

    [CLSCompliant(false)]
    public class ScriptsRunMapping : ClassMap<ScriptsRun>
    {
        public ScriptsRunMapping()
        {
            Table(MappingHelper.GetTableName(ApplicationParameters.CurrentMappings.scripts_run_table_name));
            Not.LazyLoad();
            HibernateMapping.DefaultAccess.Property();
            HibernateMapping.DefaultCascade.SaveUpdate();

            Id(x => x.id).Column("id").GeneratedBy.Identity().UnsavedValue(0);
            Map(x => x.version_id).CustomSqlType("integer");
            Map(x => x.script_name).Length(1024);
            Map(x => x.text_of_script).CustomType("StringClob").CustomSqlType("nmemo").LazyLoad();
            Map(x => x.text_hash).Length(512);
            Map(x => x.one_time_script);

            //audit
            Map(x => x.entry_date);
            Map(x => x.modified_date);
            Map(x => x.entered_by).Length(50);
        }
    }
}