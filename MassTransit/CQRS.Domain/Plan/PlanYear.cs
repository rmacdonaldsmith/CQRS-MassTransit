using System;

namespace CQRS.Domain.Plan
{
    public sealed class PlanYear
    {
        public int Year { get; set; }
        public DateTime Starts { get; set; }
        public DateTime Ends { get; set; }
    }
}
