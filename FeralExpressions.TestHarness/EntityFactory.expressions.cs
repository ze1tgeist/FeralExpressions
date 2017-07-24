using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace FeralExpressions.TestHarness
{
    partial class EntityFactory
    {
        public static Expression<Func<EntityFactory,Guid,string,Entity>> CreateEntity_Expression =>
        (EntityFactory _this, Guid id, string name) =>
            _this.ValidateEntity(id, name)
                ? new Entity() { Id = id, Name = name }
                : null;
        public static Expression<Func<EntityFactory,Guid,string,bool>> ValidateEntity_Expression =>
        (EntityFactory _this, Guid id, string name) =>
            id != Guid.Empty
            && !String.IsNullOrWhiteSpace(name);
    }
}
