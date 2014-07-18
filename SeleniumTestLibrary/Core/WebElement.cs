using OpenQA.Selenium;

namespace AutoScout24.SeleniumTestLibrary.Core
{
    public class WebElement : IElement
    {
        private readonly IWebElement webElement;

        public WebElement(IWebElement webElement)
        {
            this.webElement = webElement;
        }

        public void SendKeys(string keys)
        {
            webElement.SendKeys(keys);
        }

        public void Submit()
        {
            webElement.Submit();
        }

        public void Click()
        {
            webElement.Click();
        }

        public void Clear()
        {
            webElement.Clear();
        }

        public void SelectDropDown(string byValue)
        {            
            webElement.FindElement(By.CssSelector(string.Format("option[value='{0}']", byValue))).Click();
        }

        public void SelectDropDown(string byDataAttribute, string value)
        {
            webElement.FindElement(By.CssSelector(string.Format("option[data-{0}='{1}']", byDataAttribute, value))).Click();
        }

        public string GetCssValue(string cssProperty)
        {
            return webElement.GetCssValue(cssProperty);
        }

        public string GetAttribute(string attributeName)
        {            
            return webElement.GetAttribute(attributeName);
        }

        public string GetHtmlContent()
        {
            return webElement.Text;
        }        

        public bool IsVisible
        {
            get { return webElement.Displayed; }
        }

        public bool IsChecked
        {
            get { return webElement.Selected; }
        }

        public bool IsEnabled
        {
            get { return webElement.Enabled; }
        }
    }
}
