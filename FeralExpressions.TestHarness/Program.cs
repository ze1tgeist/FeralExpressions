using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FeralExpressions.TestHarness
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new EntityFactory();

            Expression<Func<Entity>> create = () => factory.CreateEntity(Guid.NewGuid(), "abcdef".Substring(2));

            var create2 = create.Inline();
        }

        
    }
}
