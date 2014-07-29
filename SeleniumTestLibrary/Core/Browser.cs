using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using AutoScout24.SeleniumTestLibrary.Common;
using AutoScout24.SeleniumTestLibrary.Criteria;
using AutoScout24.SeleniumTestLibrary.Setup;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;

namespace AutoScout24.SeleniumTestLibrary.Core
{
    public class Browser : IBrowse
    {
        private const string TestRequestFormat = "http://{0}:{1}/wd/hub";
        private readonly IWebDriver browser;
        private readonly string type;

        public Browser(string browserType)
        {
            var testConfig = TestConfigReader.GetTestConfig(browserType);
            var desiredCapabilities = new DesiredCapabilities();
            var internetExplorerOptions = new InternetExplorerOptions
            {
                IgnoreZoomLevel = true
            };
            switch (testConfig.BrowserType)
            {
                case BrowserType.InternetExplorer32:
                    browser = new InternetExplorerDriver(DriverPath(BrowserType.InternetExplorer32), internetExplorerOptions);
                    type = BrowserType.InternetExplorer32;
                    break;
                case BrowserType.Chrome:
                    browser = new ChromeDriver(DriverPath(BrowserType.Chrome));
                    type = BrowserType.Chrome;
                    break;
                case BrowserType.Firefox:
                    browser = new FirefoxDriver();
                    type = BrowserType.Firefox;
                    break;
                case BrowserType.RemoteChrome:
                    desiredCapabilities.SetCapability(CapabilityType.BrowserName, "chrome");
                    var address = new Uri(string.Format(TestRequestFormat, testConfig.MachineName, testConfig.Port));
                    browser = new ExtendedRemoteWebDriver(address, desiredCapabilities);
                    type = BrowserType.RemoteChrome;
                    break;
                case BrowserType.RemoteFirefox:
                    desiredCapabilities.SetCapability(CapabilityType.BrowserName, "firefox");
                    browser = new ExtendedRemoteWebDriver(new Uri(string.Format(TestRequestFormat, testConfig.MachineName, testConfig.Port)), desiredCapabilities);
                    type = BrowserType.RemoteFirefox;
                    break;
                case BrowserType.RemoteIe:
                    desiredCapabilities.SetCapability(CapabilityType.BrowserName, "ie");
                    desiredCapabilities.SetCapability("ignoreProtectedModeSettings", true);
                    browser = new ExtendedRemoteWebDriver(new Uri(string.Format(TestRequestFormat, testConfig.MachineName, testConfig.Port)), desiredCapabilities);
                    type = BrowserType.RemoteIe;
                    break;
            }
            browser.Manage().Window.Maximize();
        }

        public string Url
        {
            get { return browser.Url; }
        }

        public IElement Find(ICriteria criterion, bool onlyVisible = true)
        {
            return FindOneWithCriterion(criterion, onlyVisible);
        }

        public IEnumerable<IElement> FindAll(ICriteria criterion, bool onlyVisible = true)
        {
            return FindAllWithCriterion(criterion, onlyVisible);
        }

        public IElement WaitFor(ICriteria criterion, bool onlyVisible = true, int timeout = 30)
        {
            return WaitOneWithCriterion(criterion, onlyVisible);
        }        

        public void NavigateTo(string url, bool killPromptWindows = true)
        {
            var currentWindowHandle = browser.CurrentWindowHandle;
            try
            {
                if (killPromptWindows)
                {
                    browser.KillDialogs();
                }
                browser.Navigate().GoToUrl(url);
                if (killPromptWindows)
                {
                    browser.KillDialogs();
                }
            }
            catch (UnhandledAlertException ex)
            {
                Console.WriteLine(ex.AlertText);
                browser.SwitchTo().Alert().Accept();
                browser.SwitchTo().Window(currentWindowHandle);
                browser.Navigate().GoToUrl(url);
            }
        }

        public string GetSelectedDropDownText(string dropDownId, int waitForSeconds = 0)
        {
            if (waitForSeconds <= 0)
            {
                return browser.FindElement(By.CssSelector(string.Format("#{0}>option[selected='selected']", dropDownId))).Text;
            }
            WaitElementWithIdIsVisible(dropDownId, waitForSeconds);
            return browser.FindElement(By.CssSelector(string.Format("#{0}>option[selected='selected']", dropDownId))).Text;
        }

        public string GetDropDownText(string dropDownId, int waitForSeconds = 0)
        {
            if (waitForSeconds <= 0)
            {
                return browser.ExecuteScript(string.Format("var theOptions = document.getElementById('{0}');return theOptions.options[theOptions.selectedIndex].text", dropDownId));
            }
            WaitElementWithIdIsVisible(dropDownId, waitForSeconds);
            return browser.ExecuteScript(string.Format("var theOptions = document.getElementById('{0}');return theOptions.options[theOptions.selectedIndex].text", dropDownId));
        }

        public string GetSelectedDropDownValue(string dropDownId, int waitForSeconds = 0)
        {
            return GetElementValue(dropDownId, waitForSeconds);
        }

        public string GetElementValue(string elementId, int waitForSeconds = 0)
        {
            return waitForSeconds > 0
                ? WaitElementWithIdIsVisible(elementId, waitForSeconds).GetAttribute("value")
                : browser.FindElement(By.Id(string.Format("{0}", elementId))).GetAttribute("value");
        }

        public IElement FindElementByCss(string css)
        {
            return new WebElement(browser.FindElement(By.CssSelector(css)));
        }

        public IElement FindElementById(string id)
        {
            return new WebElement(browser.FindElement(By.Id(id)));
        }

        public IElement FindElementByName(string name)
        {
            return new WebElement(browser.FindElement(By.Name(name)));
        }

        public IElement WaitElementWithCssIsVisible(string css, int waitForSeconds = 30)
        {
            return new WebElement(browser.WaitElementIsVisible(By.CssSelector(css), waitForSeconds));
        }

        public void WaitForNoOfElements(string css, int noOfElements, int waitForSeconds = 30)
        {
            browser.WaitForElements(noOfElements, css, waitForSeconds);
        }

        public IElement MoveMouseOverElementWithId(string id, int waitForMiliseconds)
        {
            return new WebElement(browser.MoveMouseOver(browser.FindElement(By.Id(id)), waitForMiliseconds));
        }

        public IElement WaitForElementWithCss(string css, int waitForSeconds = 30)
        {
            return new WebElement(browser.FindElement(By.CssSelector(css), waitForSeconds));
        }

        public IElement WaitElementWithIdIsVisible(string id, int waitForSeconds = 30)
        {
            return new WebElement(browser.WaitElementIsVisible(By.Id(id), waitForSeconds));
        }

        public bool ElementWithIdExists(string id)
        {
            return browser.CheckElementExists(By.Id(id));
        }

        public bool ElementWithNameExists(string name)
        {
            return browser.CheckElementExists(By.Name(name));
        }

        public bool ElementWithCssExists(string css)
        {
            return browser.CheckElementExists(By.CssSelector(css));
        }

        public bool ElementWithIdIsVisible(string id)
        {
            return browser.CheckElementIsVisible(By.Id(id));
        }

        public bool ElementWithNameIsVisible(string name)
        {
            return browser.CheckElementIsVisible(By.Name(name));
        }

        public bool ElementWithCssIsVisible(string css)
        {
            return browser.CheckElementIsVisible(By.CssSelector(css));
        }

        public void AddCookie(string name, string value)
        {
            browser.Manage().Cookies.AddCookie(new OpenQA.Selenium.Cookie(name, value));
        }

        public void EnableFeature(string featureBeeName)
        {
            //e.g: cookie name: featureBee cookie value: #GuidedTourButton=true#
            //cookie value when more toggles are enabled #GuidedTourButton=false#PMVM-2745-MarketAnalysisSurvey=true#
            var cookieNamed = browser.Manage().Cookies.GetCookieNamed("featureBee");
            var cookieValue = CreateCookieValue(true, featureBeeName, cookieNamed);
            browser.Manage().Cookies.DeleteCookieNamed("featureBee");
            browser.Manage().Cookies.AddCookie(new OpenQA.Selenium.Cookie("featureBee", cookieValue));
        }

        public void DisableFeature(string featureBeeName)
        {
            var cookieNamed = browser.Manage().Cookies.GetCookieNamed("featureBee");
            var cookieValue = CreateCookieValue(false, featureBeeName, cookieNamed);
            browser.Manage().Cookies.DeleteCookieNamed("featureBee");
            browser.Manage().Cookies.AddCookie(new OpenQA.Selenium.Cookie("featureBee", cookieValue));
        }

        public bool IsFeatureEnabled(string featureBeeName)
        {
            var featureDisabled = string.Format("#{0}=false", featureBeeName);
            var featureEnabled = string.Format("#{0}=true", featureBeeName);

            var cookieNamed = browser.Manage().Cookies.GetCookieNamed("featureBee");
            if (cookieNamed != null)
            {
                if (cookieNamed.Value.Contains(featureDisabled))
                {
                    return false;
                }
                if (cookieNamed.Value.Contains(featureEnabled))
                {
                    return true;
                }
            }
            return false;
        }

        public void WaitForPageLoad(int waitForSeconds = 30)
        {
            browser.WaitForPageLoad(waitForSeconds);
        }

        public void WaitUrlContains(string value, int waitForSeconds = 30)
        {
            browser.WaitUrlContains(value, waitForSeconds);
        }

        public void RegisterWaitForEvent(string eventName)
        {
            browser.RegisterForEvent(eventName);
        }

        public void WaitForEvent(string eventName, int timeoutInSeconds = 30)
        {
            browser.WaitForEvent(eventName, timeoutInSeconds);
        }

        public IElement WaitForElementWithName(string name, int waitForSeconds = 30)
        {
            return new WebElement(browser.FindElement(By.Name(name), waitForSeconds));
        }

        public IEnumerable<IElement> FindElementsWithCss(string css)
        {
            return GetElements(browser.FindElements(By.CssSelector(css)));
        }

        public void Close()
        {
            browser.Quit();
        }

        public void DismissDialog()
        {
            browser.KillDialogs();
        }

        public string ExecuteScript(string javascript)
        {
            return browser.ExecuteScript(javascript);
        }

        public void ScrollIntoViewCss(string elementCss)
        {
            browser.ScrollIntoViewCss(elementCss);
        }

        public void ScrollIntoViewId(string elementId)
        {
            browser.ScrollIntoViewId(elementId);
        }

        public void SwitchTo(string windowId = null)
        {
            if (string.IsNullOrEmpty(windowId))
            {
                browser.SwitchTo().DefaultContent();
            }
            else
            {
                browser.SwitchTo().Frame(windowId);
                browser.SwitchTo().ActiveElement();
            }
        }

        public int GetStatusCode(string url)
        {
            try
            {
                return (int) GetCode(url).Result;
            }
            catch (Exception)
            {
                return (int) HttpStatusCode.ServiceUnavailable;
            }
        }

        public void CaptureScreenShot(string prefferedFileName)
        {
            if (string.IsNullOrEmpty(prefferedFileName))
            {
                prefferedFileName = Guid.NewGuid().ToString();
            }

            var screenshotPath = TestConfigReader.ScreenshotPath;
            var screenshotUrl = TestConfigReader.ScreenshotUrl;
            var appendBuildNumber = TestConfigReader.AppendBuildNumberToPath;

            if (string.IsNullOrEmpty(screenshotPath))
            {
                Console.WriteLine("No screenshot path set (You might run locally and then there is none.)");
                return;
            }
            var path = screenshotPath;
            if (appendBuildNumber)
            {
                path = string.Format(screenshotPath, Environment.GetEnvironmentVariable("BUILD_NUMBER"));
                screenshotUrl = string.Format(screenshotUrl, Environment.GetEnvironmentVariable("BUILD_NUMBER"));
            }

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            path += string.Format(@"\{0}.jpg", prefferedFileName);

            try
            {
                Screenshot screenshot;
                switch (type)
                {
                    case BrowserType.Chrome:
                        screenshot = ((ChromeDriver) browser).GetScreenshot();
                        break;
                    case BrowserType.Firefox:
                        screenshot = ((FirefoxDriver) browser).GetScreenshot();
                        break;
                    case BrowserType.InternetExplorer32:
                        screenshot = ((InternetExplorerDriver) browser).GetScreenshot();
                        break;
                    case BrowserType.RemoteIe:
                    case BrowserType.RemoteFirefox:
                    case BrowserType.RemoteChrome:
                        screenshot = ((ExtendedRemoteWebDriver) browser).GetScreenShot();
                        break;
                    default:
                        throw new InvalidOperationException("Unknown browser type");
                }
                path = RemoveIllegalCharactersFromPath(path);
                if (!path.EndsWith(".jpg"))
                {
                    path += ".jpg";
                }
                if (screenshot != null)
                {
                    screenshot.SaveAsFile(path, ImageFormat.Jpeg);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return;
            }

            var message = new StringBuilder("\nScreenshot Time: " + DateTime.Now);
            var url = string.Format(@"{0}/{1}", screenshotUrl, Path.GetFileName(path)).Replace("\\", "/");
            message.AppendFormat("\nScreenshot: url({0})", url);
            Console.WriteLine(message.ToString());
        }

        public void WaitPageTitleContains(string title, int waitForSeconds = 30)
        {
            var wait = new WebDriverWait(browser, TimeSpan.FromSeconds(waitForSeconds));
            wait.Until(d => d.Title.Contains(title));
        }

        public IElement WaitForElementWithId(string id, int waitForSeconds = 30)
        {
            return new WebElement(browser.FindElement(By.Id(id), waitForSeconds));
        }

        public void WaitForDropDownValue(string value, int waitForSeconds = 30)
        {
            browser.WaitForDropDownValue(value, waitForSeconds);
        }

        public void WaitForElementsInDropDown(string parentId, int noOfElements, int waitForSeconds = 30)
        {
            browser.WaitForElementsInDropDown(parentId, noOfElements, waitForSeconds);
        }

        public string GetPageSource()
        {
            return browser.PageSource;
        }

        public string GetUrl()
        {
            return browser.Url;
        }

        public void SetDisplaySize(Device device)
        {
            switch (device)
            {
                case Device.IpadVertical:
                    browser.Manage().Window.Size = new Size(768, 1024);
                    break;
                case Device.Ipadhorizontal:
                    browser.Manage().Window.Size = new Size(1024, 768);
                    break;
                case Device.Iphone4S:
                    browser.Manage().Window.Size = new Size(320, 480);
                    break;
                case Device.Iphone5S:
                    browser.Manage().Window.Size = new Size(320, 568);
                    break;
                case Device.Samsung:
                    browser.Manage().Window.Size = new Size(360, 640);
                    break;
                default:
                    browser.Manage().Window.Maximize();
                    break;
            }
        }

        private IElement WaitOneWithCriterion(ICriteria criterion, bool onlyVisible, int timeout = 30)
        {
            var criteriaType = criterion.CriteriaType;
            string criterionString = criterion.GetCriterion();
            switch (criteriaType)
            {
                case CriteriaType.Id:
                    return WaitForOneWithCriterion(criterionString, By.Id(criterionString), onlyVisible, timeout);
                case CriteriaType.CssClass:
                    return WaitForOneWithCriterion(criterionString, By.ClassName(criterionString), onlyVisible, timeout);
                case CriteriaType.DataAttribute:
                case CriteriaType.Selector:
                    return WaitForOneWithCriterion(criterionString, By.CssSelector(criterionString), onlyVisible, timeout);
            }
            throw new ArgumentException(string.Format("Don't know this criterion: {0}", criterionString));
        }

        private static string CreateCookieValue(bool shouldEnable, string featureName, OpenQA.Selenium.Cookie cookie)
        {
            var featureDisabled = string.Format("#{0}=false", featureName);
            var featureEnabled = string.Format("#{0}=true", featureName);

            if (cookie != null)
            {
                var value = cookie.Value;
                if (string.IsNullOrEmpty(value))
                {
                    if (shouldEnable)
                    {
                        return featureEnabled;
                    }
                    return featureDisabled;
                }

                if (shouldEnable)
                {
                    if (value.Contains(featureEnabled))
                    {
                        return value;
                    }
                    if (value.Contains(featureDisabled))
                    {
                        return value.Replace(featureDisabled, featureEnabled);
                    }
                    value += featureEnabled;
                    return value;
                }
                // should disable
                if (value.Contains(featureDisabled))
                {
                    return value;
                }
                if (value.Contains(featureEnabled))
                {
                    value = value.Replace(featureEnabled, featureDisabled);
                    return value;
                }
                value += featureEnabled;
                return value;
            }

            if (shouldEnable)
            {
                return featureEnabled;
            }
            return featureDisabled;
        }

        public Cookie GetCookie(string name)
        {
            var cookie = browser.Manage().Cookies.GetCookieNamed(name);
            return new Cookie
            {
                Domain = cookie.Domain,
                Expires = cookie.Expiry,
                IsSecure = cookie.Secure,
                Name = cookie.Name,
                Path = cookie.Path,
                Value = cookie.Value
            };
        }

        private static string RemoveIllegalCharactersFromPath(string path)
        {
            var invalidPathChars = Path.GetInvalidPathChars();
            return invalidPathChars.Aggregate(path, (current, invalidPathChar) => current.Replace(invalidPathChar, '_'));
        }

        private static string DriverPath(string browserType)
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            switch (browserType)
            {
                case BrowserType.Chrome:
                    return Path.Combine(basePath, @"Drivers\");
                case BrowserType.InternetExplorer32:
                    return Path.Combine(basePath, @"Drivers\");
            }
            throw new ArgumentOutOfRangeException(string.Format("I don't know this Browser type: {0}", browserType));
        }

        public void SetDropDownValue(string dropDownId, string value)
        {
            browser.FindElement(By.Id(dropDownId)).FindElement(By.CssSelector(string.Format("option[value='{0}']", value))).Click();
        }

        public void SetTextBoxValue(string textBoxId, string value)
        {
            browser.FindElement(By.Id(textBoxId)).SendKeys(value);
        }

        public IElement FindElementByXPath(string xpath)
        {
            return new WebElement(browser.FindElement(By.XPath(xpath)));
        }

        private IElement FindOneWithCriterion(ICriteria criterion, bool onlyVisible = true)
        {
            var criteriaType = criterion.CriteriaType;
            var criterionString = criterion.GetCriterion();
            switch (criteriaType)
            {
                case CriteriaType.Id:
                    return FindOneWithCriterion(onlyVisible, By.Id(criterionString));
                case CriteriaType.CssClass:
                    return FindOneWithCriterion(onlyVisible, By.ClassName(criterionString));
                case CriteriaType.DataAttribute:
                case CriteriaType.Selector:
                    return FindOneWithCriterion(onlyVisible, By.CssSelector(criterionString));
            }
            return null;
        }

        private IEnumerable<IElement> FindAllWithCriterion(ICriteria criterion, bool onlyVisible = true)
        {
            var criteriaType = criterion.CriteriaType;
            switch (criteriaType)
            {
                case CriteriaType.Id:
                    return FindAllWithCriterion(onlyVisible, By.Id(criterion.GetCriterion()));
                case CriteriaType.CssClass:
                    return FindAllWithCriterion(onlyVisible, By.ClassName(criterion.GetCriterion()));
                case CriteriaType.DataAttribute:
                case CriteriaType.Selector:
                    return FindAllWithCriterion(onlyVisible,
                        By.CssSelector(criterion.GetCriterion()));
            }
            return null;
        }

        private IElement FindOneWithCriterion(bool onlyVisible, By by)
        {
            var elements = browser.FindElements(by);
            if (onlyVisible)
            {
                foreach (var webElement in elements)
                {
                    if (webElement.Displayed)
                    {
                        return new WebElement(webElement);
                    }
                    
                }                
            }
            else
            {
                return new WebElement(elements.FirstOrDefault());    
            }
            return null;
        }

        private IEnumerable<IElement> FindAllWithCriterion(bool onlyVisible, By by)
        {
            var visibleElements = new List<WebElement>();
            var allElements = new List<WebElement>();
            var elements = browser.FindElements(by);
            foreach (var webElement in elements)
            {
                if (webElement.Displayed)
                {
                    visibleElements.Add(new WebElement(webElement));
                }
                allElements.Add(new WebElement(webElement));
            }
            return onlyVisible ? visibleElements : allElements;
        }

        private IElement WaitForOneWithCriterion(string criterion, By by, bool onlyVisible, int timeout = 30)
        {
            if (onlyVisible)
            {
                int counter = 0;
                do
                {
                    counter++;
                    var element = FindOneWithCriterion(true, by);                    
                    if (element == null)
                    {
                        Console.WriteLine("Element is null");
                        Thread.Sleep(100);
                    }
                    else
                    {
                        Console.WriteLine(element.IsVisible);
                        return element;
                    }
                    if (counter == 300)
                    {
                        throw new Exception(string.Format("No element matching criterion {0} could be found in {1}", criterion, timeout));
                    }
                } 
                while (true);
            }
            return FindOneWithCriterion(false, by);
        }    

        private static async Task<HttpStatusCode> GetCode(string url)
        {
            var client = new HttpClient();
            var response = await client.GetAsync(url);
            return response != null ? response.StatusCode : HttpStatusCode.ServiceUnavailable;
        }

        private static IEnumerable<WebElement> GetElements(IEnumerable<IWebElement> elements)
        {
            return elements.Select(webElement => new WebElement(webElement)).ToList();
        }
    }
}
