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
            var dbContext = new MyDbContext(factory);

            var query = dbContext.MyEntities.Where(e => e.Name.StartsWith("abc"));

            var test = query.ToArray();
        }

        
    }
}
