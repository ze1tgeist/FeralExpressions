using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeralExpressions.TestHarness
{
    partial class EntityFactory
    {
        public Entity CreateEntity(Guid id, string name) =>
            this.ValidateEntity(id, name) && ValidateEntity(id, name)
                ? new Entity() { Id = id, Name = name.Substring(1).Substring(1) }
                : null;

        public bool ValidateEntity(Guid id, string name) =>
            id != Guid.Empty
            && !String.IsNullOrEmpty(name);
    }
}
