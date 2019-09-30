using Dynamics365.Api.Client.Enums;
using Dynamics365.Api.Client.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dynamics365.Api.Client.Types
{
    public class LogicalFilter : IFilter
    {
        public IFilter Left { get; set; }

        public LogicalOperations Operator { get; set; }

        public IFilter Right { get; set; }
    }
}
