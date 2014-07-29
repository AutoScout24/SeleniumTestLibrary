using AutoScout24.SeleniumTestLibrary.Common;

namespace AutoScout24.SeleniumTestLibrary.Criteria
{
    public class DataAttributeValueCriterion : ICriteria
    {
        private readonly string dataName;
        private readonly string dataValue;

        public DataAttributeValueCriterion(string dataName, string dataValue)
        {
            this.dataName = dataName;
            this.dataValue = dataValue;
            CriteriaType = CriteriaType.DataAttribute;
        }

        public string GetCriterion()
        {
            return string.Format("[data-{0}='{1}']",dataName,dataValue);
        }

        public CriteriaType CriteriaType { get; set; }
    }
}