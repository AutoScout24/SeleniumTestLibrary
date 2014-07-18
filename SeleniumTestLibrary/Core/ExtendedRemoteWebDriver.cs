using System;

using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace AutoScout24.SeleniumTestLibrary.Core
{
    public class ExtendedRemoteWebDriver : RemoteWebDriver
    {
        public ExtendedRemoteWebDriver(ICommandExecutor commandExecutor, ICapabilities desiredCapabilities) : base(commandExecutor, desiredCapabilities) {}
        public ExtendedRemoteWebDriver(ICapabilities desiredCapabilities) : base(desiredCapabilities) {}
        public ExtendedRemoteWebDriver(Uri remoteAddress, ICapabilities desiredCapabilities) : base(remoteAddress, desiredCapabilities) {}
        public ExtendedRemoteWebDriver(Uri remoteAddress, ICapabilities desiredCapabilities, TimeSpan commandTimeout) : base(remoteAddress, desiredCapabilities, commandTimeout) {}        

        public Screenshot GetScreenShot()
        {
            var screenshotResponse = Execute(DriverCommand.Screenshot, null);
            var base64 = screenshotResponse.Value.ToString();            
            return new Screenshot(base64); 
        }
    }
}
