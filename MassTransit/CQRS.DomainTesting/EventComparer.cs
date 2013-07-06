using System.Collections.Generic;
using KellermanSoftware.CompareNetObjects;

namespace CQRS.DomainTesting
{
    public class EventComparer
    {
        public static EventComparer Instance {
            get
            {
                return new EventComparer();
            }
        }

        private readonly CompareObjects _comparer;

        private EventComparer()
        {
            _comparer = new CompareObjects
                {
                    MaxDifferences = 20, //should be plenty deep for most cases.
                    ExpectedName = "Expected",
                    ActualName = "Actual",
                    //the rest of the defaults look good to me
                };
        }

        public string Compare(object exptected, object actual)
        {
            if (_comparer.Compare(exptected, actual))
                return null;

            return _comparer.DifferencesString;
        }

        public string Compare(object exptected, object actual, IEnumerable<string> elementsToIgnore)
        {
            _comparer.ElementsToIgnore = new List<string>(elementsToIgnore);

            return Compare(exptected, actual);
        }
    }
}
