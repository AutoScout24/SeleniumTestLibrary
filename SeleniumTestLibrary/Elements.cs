using AutoScout24.SeleniumTestLibrary.Criteria;

namespace AutoScout24.SeleniumTestLibrary
{
    public static class Elements
    {        
        public static ICriteria WithClassName(string className, bool onlyVisible = true)
        {
            return new ClassNameCriterion(className);
        }

        public static ICriteria WithDataAttribute(string attributeName, string attributeValue, bool onlyVisible = true)
        {
            return new DataAttributeValueCriterion(attributeName, attributeValue);
        }

        public static ICriteria WithSelector(string selector, bool onlyVisible = true)
        {
            return new CssSelectorCriterion(selector);
        }
    }   
}