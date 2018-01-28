using FeralExpressionsCore.Tests.Domain;
using FeralExpressionsCore.Tests.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace FeralExpressionsCore.Tests
{
    public class EntityTests
    {
        [Fact]
        public void Expression_SubstitutionWorks()
        {
            var entities = new List<TestEntity>()
            {
                new TestEntity() { Key = "abc", Data = "def" },
                new TestEntity() { Key = "QueenP", Data = "def" },
                new TestEntity() { Key = "QoP", Data = "def" },
                new TestEntity() { Key = "ghi", Data = "def" },
                new TestEntity() { Key = "lqf", Data = "def" }
            };

            var insertContext = new TestDbContext();
            insertContext.Database.EnsureDeleted();
            insertContext.Database.EnsureCreated();

            insertContext.Entities.AddRange(entities);
            insertContext.SaveChanges();

            var readContext = new TestDbContext();
            IEntityRepository repo = new EntityRepository(readContext);

            var actual = repo.EntitiesStartingWithQAndEndingWithP.ToArray();
            Assert.True(actual.Count() == 2);

        }

        [Fact]
        public void OfType_Mapping_Works()
        {
            var entities = new List<TestEntity>()
            {
                new SubEntity() { Key = "abc", Data = "def" },
                new SubEntity() { Key = "QueenP", Data = "def" },
                new TestEntity() { Key = "QoP", Data = "def" },
                new TestEntity() { Key = "ghi", Data = "def" },
                new TestEntity() { Key = "lqf", Data = "def" }
            };

            var insertContext = new TestDbContext();
            insertContext.Database.EnsureDeleted();
            insertContext.Database.EnsureCreated();

            insertContext.Entities.AddRange(entities);
            insertContext.SaveChanges();

            var readContext = new TestDbContext();
            IEntityRepository repo = new EntityRepository(readContext);

            var actual = repo
                .EntitiesStartingWithQAndEndingWithP
                .OfType<ISubEntity>()
                .ToArray();
            Assert.True(actual.Count() == 1);

        }
    }
}
