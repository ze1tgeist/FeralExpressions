using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeralExpressions.TestHarness
{
    public class Entity
    {
        public Guid Id { get; internal set; }
        public string Name { get; internal set; }
    }

    public static partial class EntityExtensions
    {
        public static string NamePlusId(this Entity ent, string seperator) =>
            ent.Name + seperator + ent.Id.ToString();
    }
}
