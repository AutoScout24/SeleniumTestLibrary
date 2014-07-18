using System;

namespace AutoScout24.SeleniumTestLibrary.Core
{
    public class Cookie
    {
        public string  Domain { get; set; }
        public DateTime? Expires { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public bool IsSecure { get; set; }
        public string Value { get; set; }
    }
}
