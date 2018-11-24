namespace roundhouse.databases.advantage.orm
{
    using System;
    using FluentNHibernate.Mapping;
    using infrastructure;
    using model;

    [CLSCompliant(false)]
    public class ScriptsRunErrorMapping : ClassMap<ScriptsRunError>
    {
        public ScriptsRunErrorMapping()
        {
            Table(MappingHelper.GetTableName(ApplicationParameters.CurrentMappings.scripts_run_errors_table_name));
            Not.LazyLoad();
            HibernateMapping.DefaultAccess.Property();
            HibernateMapping.DefaultCascade.SaveUpdate();

            Id(x => x.id).Column("id").CustomSqlType("autoinc").GeneratedBy.Identity().UnsavedValue(0);
            Map(x => x.repository_path).Length(1024);
            Map(x => x.version).Length(50);
            Map(x => x.script_name).Length(1024);
            Map(x => x.text_of_script).CustomType("StringClob").CustomSqlType("nmemo");
            Map(x => x.erroneous_part_of_script).CustomType("StringClob").CustomSqlType("nmemo");
            Map(x => x.error_message).CustomType("StringClob").CustomSqlType("nmemo");

            //audit
            Map(x => x.entry_date);
            Map(x => x.modified_date);
            Map(x => x.entered_by).Length(50);
        }
    }
}