using AutoScout24.SeleniumTestLibrary.Common;
using AutoScout24.SeleniumTestLibrary.Core;

namespace AutoScout24.SeleniumTestLibrary.Criteria
{
    public class ClassNameCriterion : ICriteria
    {
        private readonly string className;

        public ClassNameCriterion(string className)
        {
            this.className = className;
            CriteriaType = CriteriaType.CssClass;
        }

        public string GetCriterion()
        {
            return className;
        }

        public CriteriaType CriteriaType { get; set; }
    }
}