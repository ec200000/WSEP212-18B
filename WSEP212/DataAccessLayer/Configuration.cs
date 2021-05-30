using System.Data.Entity.Migrations;

namespace WSEP212.DataAccessLayer
{
    internal sealed class Configuration : DbMigrationsConfiguration<SystemDBAccess>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            CommandTimeout = 120;
        }

        protected override void Seed(SystemDBAccess context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.
        }
    }
}