using FeralExpressionsCore.Tests.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FeralExpressionsCore.Tests.EF
{
    public partial class EntityRepository : IEntityRepository
    {
        public EntityRepository(TestDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public IQueryable<IEntity> EntitiesStartingWithQAndEndingWithP =>
            dbContext.Entities
            .Inline()
            .MapTypes(mappings)
            .GetEntitiesStartingWithQAndEndingWithP();

        public IQueryable<IEntity> Entities => dbContext.Entities;

        private readonly TestDbContext dbContext;
        private static Dictionary<Type, Type> mappings = new Dictionary<Type, Type>()
        {
            { typeof(IEntity), typeof(TestEntity) },
            { typeof(ISubEntity), typeof(SubEntity) }
        };
    }

    public static partial class EntityRepositoryExtensions
    {
        public static IQueryable<IEntity> GetEntitiesStartingWithQAndEndingWithP(this IQueryable<IEntity> entities) =>
            entities.Where(e => StartsWithQAndEndingWithP(e));

        public static bool StartsWithQAndEndingWithP(IEntity entity) => StartsWith(entity, "Q") && EndsWith(entity, "P");

        public static bool StartsWith(IEntity entity, string letter) => entity.Key.StartsWith(letter);
        public static bool EndsWith(IEntity entity, string letter) => entity.Key.EndsWith(letter);

    }
}
