namespace AutoScout24.SeleniumTestLibrary.Core
{
    public interface IElement
    {
        void SendKeys(string keys);
        void Submit();
        void Click();
        void Clear();
        void SelectDropDown(string byValue);
        void SelectDropDown(string byDataAttribute, string value);
        string GetCssValue(string cssProperty);
        string GetAttribute(string attributeName);
        string GetHtmlContent();        
        bool IsVisible { get; }
        bool IsChecked { get; }        
        bool IsEnabled { get; }        
    }
}