using AutoScout24.SeleniumTestLibrary.Common;
using AutoScout24.SeleniumTestLibrary.Core;

namespace AutoScout24.SeleniumTestLibrary.Criteria
{
    public class IdCriterion : ICriteria
    {
        private readonly string id;

        public IdCriterion(string id)
        {
            this.id = id;
            CriteriaType = CriteriaType.Id;
        }

        public string GetCriterion()
        {
            return id;
        }

        public CriteriaType CriteriaType { get; set; }
    }
}