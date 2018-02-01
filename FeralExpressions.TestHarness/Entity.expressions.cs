using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace FeralExpressions.TestHarness
{

    public static partial class EntityExtensions
    {
        public static Expression<Func<Entity,string,string>> NamePlusId_Expression =>
        (Entity ent, string seperator) =>
            ent.Name + seperator + ent.Id.ToString();
    }
}
