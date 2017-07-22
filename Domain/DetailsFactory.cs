using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionCompilingTester
{
    public class Details
    {
        public String RegisterId { get; internal set; }
    }
    public partial class DetailsFactory
    {
        public string Test { get; set; }

        public void TestWithNormalBody()
        {

        }
        public Details CreateDetails(string registerId) =>
            ValidateDetails(registerId)
                ? new Details() { RegisterId = registerId }
                : null;

        public bool ValidateDetails(string registerId) =>
            !String.IsNullOrEmpty(registerId);
    }

    public partial class DetailsFactory
    {

        public Expression<Func<string, Details>> CreateDetails_Expression =>
            (registerId) =>
                ValidateDetails(registerId)
                    ? new Details() { RegisterId = registerId }
                    : null;

        public Expression<Func<string, bool>> ValidateDetails_Expression =>
            (registerId) =>
                 !String.IsNullOrEmpty(registerId);
    }
}
