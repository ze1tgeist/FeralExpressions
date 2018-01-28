using System;
using System.Collections.Generic;
using System.Text;

namespace FeralExpressionsCore.Tests.Domain
{
    public interface IEntity
    {
        string Key { get; set; }
        string Data { get; set; }
    }
}
