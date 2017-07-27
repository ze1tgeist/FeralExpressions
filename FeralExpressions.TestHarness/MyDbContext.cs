using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeralExpressions.TestHarness
{
    class MyDbContext : DbContext
    {
        public MyDbContext(EntityFactory factory)
        {
            this.factory = factory;

            Database.SetInitializer<MyDbContext>(new EntityDbInitializer());
        }

        public DbSet<EfEntity> EfEntities { get; set; }

        public IQueryable<Entity> MyEntities =>
            EfEntities.Inline().Select(efe => factory.CreateEntity(efe.EfEntityId, efe.Label));

        private EntityFactory factory;
    }

    public class EfEntity
    {
        public Guid EfEntityId { get; set; }

        public string Label { get; set; }
    }
}
