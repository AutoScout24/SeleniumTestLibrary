using AutoScout24.SeleniumTestLibrary.Common;
using AutoScout24.SeleniumTestLibrary.Core;

namespace AutoScout24.SeleniumTestLibrary.Criteria
{
    public class CssSelectorCriterion : ICriteria
    {
        private readonly string _selector;

        public CssSelectorCriterion(string selector)
        {
            _selector = selector;
            CriteriaType = CriteriaType.Selector;
        }

        public string GetCriterion()
        {
            return _selector;
        }

        public CriteriaType CriteriaType { get; set; }
    }
}