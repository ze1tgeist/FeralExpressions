using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace FeralExpressions.TestHarness
{
    class EntityDbInitializer : DropCreateDatabaseIfModelChanges<MyDbContext>
    {
        protected override void Seed(MyDbContext context)
        {
            context.EfEntities.Add(new EfEntity() { EfEntityId = Guid.NewGuid(), Label = "abcdef" });
            context.EfEntities.Add(new EfEntity() { EfEntityId = Guid.NewGuid(), Label = "abchji" });
            context.EfEntities.Add(new EfEntity() { EfEntityId = Guid.NewGuid(), Label = "abcklm" });
            context.EfEntities.Add(new EfEntity() { EfEntityId = Guid.NewGuid(), Label = "ruben" });
            context.EfEntities.Add(new EfEntity() { EfEntityId = Guid.NewGuid(), Label = "test" });
            context.EfEntities.Add(new EfEntity() { EfEntityId = Guid.NewGuid(), Label = "other label" });

            context.SaveChanges();
        }
    }
}
