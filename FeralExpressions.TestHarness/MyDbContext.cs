using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeralExpressions.TestHarness
{
    class MyDbContext
    {
        public MyDbContext(EntityFactory factory)
        {
            this.factory = factory;
        }
        private IQueryable<EfEntity> EfEntities { get; }

        public IQueryable<Entity> MyEntities =>
            EfEntities.Select(efe => factory.CreateEntity(efe.EfEntityId, efe.Label));

        private EntityFactory factory;
    }

    public class EfEntity
    {
        public Guid EfEntityId { get; set; }

        public string Label { get; set; }
    }
}
