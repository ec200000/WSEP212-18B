using System.Data.Entity.Migrations;

namespace WSEP212.DomainLayer
{
    internal sealed class ConfigurationMock : DbMigrationsConfiguration<SystemDBMock>
    {
        public ConfigurationMock()
        {
            AutomaticMigrationsEnabled = true;
            CommandTimeout = 120;
        }

        protected override void Seed(SystemDBMock context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.
        }
    }
}