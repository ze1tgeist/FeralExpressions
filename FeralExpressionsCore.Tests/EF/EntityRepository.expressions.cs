using FeralExpressionsCore.Tests.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace FeralExpressionsCore.Tests.EF
{

    public static partial class EntityRepositoryExtensions
    {
        public static Expression<Func<IQueryable<IEntity>,IQueryable<IEntity>>> GetEntitiesStartingWithQAndEndingWithP_Expression =>
        (IQueryable<IEntity> entities) =>
            entities.Where(e => StartsWithQAndEndingWithP(e));
        public static Expression<Func<IEntity,bool>> StartsWithQAndEndingWithP_Expression =>
        (IEntity entity) => StartsWith(entity, "Q") && EndsWith(entity, "P");
        public static Expression<Func<IEntity,string,bool>> StartsWith_Expression =>
        (IEntity entity, string letter) => entity.Key.StartsWith(letter);
        public static Expression<Func<IEntity,string,bool>> EndsWith_Expression =>
        (IEntity entity, string letter) => entity.Key.EndsWith(letter);

    }
}
