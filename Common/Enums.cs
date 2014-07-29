namespace AutoScout24.SeleniumTestLibrary.Common
{
    public class BrowserType
    {
        public const string Firefox = "Firefox";
        public const string Chrome = "Chrome";
        public const string InternetExplorer32 = "IE";
        public const string RemoteChrome = "RemoteChrome";
        public const string RemoteFirefox = "RemoteFirefox";
        public const string RemoteIe = "RemoteIe";             
    }

    public enum ClientType
    {
        Chrome,
        Firefox,
        Ie
    }

    public enum Device
    {
        IpadVertical,
        Ipadhorizontal,
        Iphone4S,
        Iphone5S,
        Samsung,
        Pc
    }

    public enum CriteriaType
    {
        Id,
        CssClass,
        DataAttribute,
        Selector
    }
}      