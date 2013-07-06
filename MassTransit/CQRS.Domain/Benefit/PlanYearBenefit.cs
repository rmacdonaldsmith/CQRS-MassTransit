using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;

namespace MHM.WinFlexOne.CQRS.Domain.Benefit
{
    public class PlanYearBenefit
    {
        private int _planYear;
        private bool _hasAnnualLimit;
        private Money _maxAnnualAmount;
        private DateTime _startDate;

        public int PlanYear
        {
            get { return _planYear; }
            set { _planYear = value; }
        }

        public bool HasAnnualLimit
        {
            get { return _hasAnnualLimit; }
            set { _hasAnnualLimit = value; }
        }

        public Money MaxAnnualAmount
        {
            get { return _maxAnnualAmount; }
            set { _maxAnnualAmount = value; }
        }

        public DateTime StartDate
        {
            get { return _startDate; }
            set { _startDate = value; }
        }
    }
}
