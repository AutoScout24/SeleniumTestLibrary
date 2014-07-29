using AutoScout24.SeleniumTestLibrary.Common;

namespace AutoScout24.SeleniumTestLibrary.Criteria
{
    public interface ICriteria
    {
        string GetCriterion();
        CriteriaType CriteriaType { get; set; }
    }    
}