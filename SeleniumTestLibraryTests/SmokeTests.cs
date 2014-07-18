using AutoScout24.SeleniumTestLibrary.Common;
using AutoScout24.SeleniumTestLibrary.Core;

using NUnit.Framework;

namespace SeleniumTestLibraryTests
{
    [TestFixture]
    public class SmokeTests : TestRunFixture
    {
        [TearDown]
        public void TearDown()
        {
            if (TestContext.CurrentContext.Result.Status == TestStatus.Failed)
            {
                if (browser != null)
                {
                    browser.CaptureScreenShot(TestContext.CurrentContext.Test.Name);
                }
            }
            if (browser != null)
            {
                browser.Close();
            }
        }

        private readonly string browserType;
        private Browser browser;

        public SmokeTests(string btype) : base(btype)
        {
            browserType = btype;
        }

        [TestCase]
        public void GetStatusCodeReturn200()
        {
            browser = new Browser(browserType);

            var code = browser.GetStatusCode("http://www.google.com/");

            Assert.AreEqual(200, code);
        }

        [TestCase("The path where the file will be saved")]
        [Ignore("Set the path and then unignore to test this")]
        public void ScreenshotTest(string urlThatPointsToPath)
        {
            browser = new Browser(browserType);

            browser.NavigateTo("http://www.google.com/");

            var element = browser.FindElementByName("q");

            element.SendKeys("Cheese");

            element.Submit();

            browser.WaitPageTitleContains("Cheese - Google", 10);
            browser.CaptureScreenShot(TestContext.CurrentContext.Test.Name);
            
            var statusCode = browser.GetStatusCode(string.Format(urlThatPointsToPath));

            Assert.AreEqual(200, statusCode);
        }

        [TestCase]
        public void SmokeTest()
        {
            browser = new Browser(browserType);

            browser.NavigateTo("http://www.google.com/");

            var element = browser.FindElementByName("q");

            element.SendKeys("Cheese");

            element.Submit();

            browser.WaitPageTitleContains("Cheese - Google", 10);
        }       
    }

    [TestFixture(BrowserType.RemoteChrome)]
    [TestFixture(BrowserType.RemoteFirefox)]
    [TestFixture(BrowserType.RemoteIe)]
    public class TestRunFixture
    {
        public TestRunFixture(string btype) {}
    }
}
