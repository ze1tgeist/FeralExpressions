using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FeralExpressionsCore.Tests.Domain
{
    public interface IEntityRepository
    {
        IQueryable<IEntity> EntitiesStartingWithQAndEndingWithP { get; }

        IQueryable<IEntity> Entities { get; }
    }
}
