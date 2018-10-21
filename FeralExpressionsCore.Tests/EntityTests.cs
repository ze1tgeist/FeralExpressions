using FeralExpressionsCore.Tests.Domain;
using FeralExpressionsCore.Tests.EF;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        public void Interface_Realization_Works_For_EF()
        {
            var entities = new List<TestEntity>()
            {
                new TestEntity() { Key = "first second", Data = "def" },
                new TestEntity() { Key = "QueenP", Data = "def" },
            };

            var insertContext = new TestDbContext();
            insertContext.Database.EnsureDeleted();
            insertContext.Database.EnsureCreated();

            insertContext.Entities.AddRange(entities);
            insertContext.SaveChanges();

            var readContext = new TestDbContext();
            IEntityRepository repo = new EntityRepository(readContext);

            IInterfaceWithFunction impl = new ImplementationOfInterfaceWithFunction("first");

            var actual = repo.Entities.Where(e => e.Key == impl.Function("second")).ToArray();
            Assert.True(actual.Count() == 1);

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

        [Fact]
        public void Cast_Mapping_Works()
        {
            Expression<Func<IQueryable<IEntity>, IQueryable<IEntity>>> unmappedExpression = (IQueryable<IEntity> queryable) => queryable.Include(e => ((ISubEntity)e).Children);

            Expression<Func<IQueryable<IEntity>, IQueryable<IEntity>>> expectedExpression = (IQueryable<IEntity> queryable) => queryable.Include(e => ((SubEntity)e).Children);

            var actualExpression = unmappedExpression.MapTypes(new Dictionary<Type, Type> { { typeof(ISubEntity), typeof(SubEntity) } });

            var expectedString = expectedExpression.ToString();
            var actualString = actualExpression.ToString();
            Assert.Equal(expectedString, actualString);
        }

        [Fact]
        public void IsA_Mapping_Works()
        {
            Expression<Func<IQueryable<IEntity>, IQueryable<IEntity>>> unmappedExpression = (IQueryable<IEntity> queryable) => queryable.Where(e => e is ISubEntity);

            Expression<Func<IQueryable<IEntity>, IQueryable<IEntity>>> expectedExpression = (IQueryable<IEntity> queryable) => queryable.Where(e => e is SubEntity);

            var actualExpression = unmappedExpression.MapTypes(new Dictionary<Type, Type> { { typeof(ISubEntity), typeof(SubEntity) } });

            var expectedString = expectedExpression.ToString();
            var actualString = actualExpression.ToString();
            Assert.Equal(expectedString, actualString);
        }

        [Fact]
        public void Include_Works()
        {
            var entities = new List<TestEntity>()
            {
                new SubEntity() { Key = "abc", Data = "def", Children = new List<ChildEntity>() { new ChildEntity(), new ChildEntity() } },
                new SubEntity() { Key = "QueenP", Data = "def", Children = new List<ChildEntity>() { new ChildEntity(), new ChildEntity() } },
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
                .Include(ce => ce.Children)
                .ToArray();
            Assert.True(actual.Count() == 1);
            Assert.True(actual.First().Children.Count() == 2);
        }
    }
}
