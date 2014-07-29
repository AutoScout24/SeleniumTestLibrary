using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace AutoScout24.SeleniumTestLibrary.Core
{
    public static class WebDriverExtensions
    {
        private const string UniqueVariableName = "CFEEEC82924B40B4A22A0F916E0D1238";
        private const string WaitUntilPageIsLoaded = "return document['readyState'] ? 'complete' == document.readyState : true";

        private const string WaitUntilPageIsLoadedInIe = "var seleniumReady = false; if (typeof window['jQuery'] != 'undefined') { window.jQuery(function () { seleniumReady = true; }); } else { seleniumReady = true; } return seleniumReady;";

        public static IWebElement FindElement(this IWebDriver driver, By by, int timeoutInSeconds)
        {            
            if (timeoutInSeconds > 0)
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
                wait.IgnoreExceptionTypes(typeof (NoSuchElementException));
                return wait.Until(ExpectedConditions.ElementExists(by));
            }
            return driver.FindElement(by);
        }       

        public static IWebElement WaitElementIsVisible(this IWebDriver driver, By by, int timeoutInSeconds)
        {            
            if (timeoutInSeconds > 0)
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
                wait.IgnoreExceptionTypes(typeof (NoSuchElementException));
                return wait.Until(ExpectedConditions.ElementIsVisible(by));
            }         
            return driver.FindElement(by);
        }

        public static IWebElement MoveMouseOver(this IWebDriver driver, IWebElement element, int waitForMiliseconds)
        {
            var builder = new Actions(driver);
            builder.MoveToElement(element).Build().Perform();
            if (waitForMiliseconds > 0)
            {
                Thread.Sleep(waitForMiliseconds);
            }
            return element;
        }

        public static bool CheckElementExists(this IWebDriver driver, By by)
        {
            IWebElement element;
            try
            {
                element = driver.FindElement(by);
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            return element != null;
        }

        public static bool CheckElementIsVisible(this IWebDriver driver, By by)
        {
            IWebElement element;
            try
            {
                element = driver.WaitElementIsVisible(by, 1);
            }            
            catch (WebDriverTimeoutException)
            {
                return false;
            }
            return element != null;
        }

        public static IWebElement WaitForDropDownValue(this IWebDriver driver, string value, int timeoutInSeconds)
        {
            if (timeoutInSeconds > 0)
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
                return wait.Until(drv => drv.FindElement(By.CssSelector(string.Format("option[value='{0}']", value))));
            }
            return driver.FindElement(By.CssSelector(string.Format("option[value='{0}']", value)));
        }        

        public static void WaitForElementsInDropDown(this IWebDriver driver, string parentId, int noOfElements, int timeoutInSeconds)
        {
            if (timeoutInSeconds <= 0)
            {
                return;
            }
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
            wait.Until(drv => drv.FindElement(By.Id(parentId)).FindElements(By.TagName("option")).Count >= noOfElements);
        }

        public static void WaitForElements(this IWebDriver driver, int noOfElements, string cssSelector, int timeoutInSeconds)
        {
            if (timeoutInSeconds <= 0)
            {
                return;
            }
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
            wait.Until(drv => drv.FindElements(By.CssSelector(cssSelector)).Count == noOfElements);
        }

        public static void WaitUrlContains(this IWebDriver driver, string value, int timeoutInSeconds)
        {
            if (timeoutInSeconds <= 0)
            {
                return;
            }
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
            wait.Until(drv => drv.Url.Contains(value));
        }

        public static void WaitForPageLoad(this IWebDriver driver, int timeoutInSeconds)
        {
            var codeToExecute = driver.GetType().Name.ToLower() == "internet explorer"
                                    ? WaitUntilPageIsLoadedInIe
                                    : WaitUntilPageIsLoaded;


            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
            wait.Until(drv =>
                {
                    var javaScriptExecutor = driver as IJavaScriptExecutor;
                    var executeScript = (bool) javaScriptExecutor.ExecuteScript(codeToExecute);
                    Debug.WriteLine(executeScript);
                    return executeScript;
                });
        }      

        public static void RegisterForEvent(this IWebDriver driver, string eventName)
        {
            try
            {                
                var javaScriptCode = string.Format("$(document).one('{0}', function () {{ window.{1}++; }}); window.{1} = 0;", eventName, UniqueVariableName);
                driver.ExecuteScript(javaScriptCode);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static void WaitForEvent(this IWebDriver driver, string eventName, int timeoutInSeconds)
        {
            try
            {
                driver.Manage().Timeouts().SetScriptTimeout(TimeSpan.FromSeconds(timeoutInSeconds));                       
                var tries = 0;
                do
                {
                    var eventWasFird = QueryEventCallsCounterValue(driver);
                    if (eventWasFird)
                    {
                        return;        
                    }
                    else
                    {
                        Thread.Sleep(50);
                        tries++;
                        if (tries >= 600)
                        {
                            throw new TimeoutException(string.Format("Event {0} was not fired while waiting for 30 seconds", eventName));
                        }
                    }
                } 
                while (true);
            }
            finally
            {
                driver.Manage().Timeouts().SetScriptTimeout(TimeSpan.FromSeconds(30));
            }
        }

        private static bool QueryEventCallsCounterValue(IWebDriver driver)
        {
            try
            {
                var javaScriptCode = string.Format("return window.{0};", UniqueVariableName);
                var result = driver.ExecuteScript(javaScriptCode);                                
                var counter = Convert.ToInt32(result);
                return counter == 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public static void KillDialogs(this IWebDriver driver)
        {
            try
            {
                const double TimeoutInSeconds = 30;
                driver.Manage().Timeouts().SetScriptTimeout(TimeSpan.FromSeconds(TimeoutInSeconds));
                const string JavaScriptCode = @"window.onbeforeunload = null; window.onunload = null; if (window.jQuery) { $(window).unbind('beforeunload'); }";

                var javaScriptExecutor = driver as IJavaScriptExecutor;
                javaScriptExecutor.ExecuteScript(JavaScriptCode.ToString(CultureInfo.InvariantCulture));
            }
            finally
            {
                driver.Manage().Timeouts().SetScriptTimeout(TimeSpan.FromSeconds(30));
            }
        }

        public static string ExecuteScript(this IWebDriver driver, string script)
        {
            try
            {
                const double TimeoutInSeconds = 30;
                driver.Manage().Timeouts().SetScriptTimeout(TimeSpan.FromSeconds(TimeoutInSeconds));
                var result = ((IJavaScriptExecutor) driver).ExecuteScript(script);
                if (result != null)
                {
                    return Convert.ToString(result);                    
                }
                else
                {
                    return null;
                }
            }
            finally
            {
                driver.Manage().Timeouts().SetScriptTimeout(TimeSpan.FromSeconds(30));
            }
        }

        public static void ScrollIntoViewCss(this IWebDriver driver, string elementCss)
        {
            ExecuteScript(driver, string.Format("document.querySelector(\"{0}\").scrollIntoView()", elementCss));
        }

        public static void ScrollIntoViewId(this IWebDriver driver, string elementId)
        {
            ExecuteScript(driver, string.Format("document.getElementById('{0}').scrollIntoView()", elementId));
        }
    }
}