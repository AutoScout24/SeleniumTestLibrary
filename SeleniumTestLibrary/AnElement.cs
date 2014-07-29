using AutoScout24.SeleniumTestLibrary.Criteria;

namespace AutoScout24.SeleniumTestLibrary
{
    public static class AnElement
    {
        public static ICriteria WithId(string id, bool onlyVisible = true)
        {
            return new IdCriterion(id);
        }

        public static ICriteria WithClassName(string className, bool onlyVisible = true)
        {
            return new ClassNameCriterion(className);
        }

        public static ICriteria WithDataAttribute(string dataName, string dataValue, bool onlyVisible = true)
        {
            return new DataAttributeValueCriterion(dataName, dataValue);
        }

        public static ICriteria WithSelector(string selector, bool onlyVisible = true)
        {
            return new CssSelectorCriterion(selector);
        }
    }
}
