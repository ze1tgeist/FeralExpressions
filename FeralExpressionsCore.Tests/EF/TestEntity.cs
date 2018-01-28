using FeralExpressionsCore.Tests.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace FeralExpressionsCore.Tests.EF
{
    public class TestEntity : IEntity
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string Data { get; set; }
    }
}
