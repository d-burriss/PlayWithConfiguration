using System;

namespace ConfigurationTests
{
    internal class DatabaseSettings
    {
        public string Name { get; set; }
        public OtherSettings Child { get; set; }
    }

    public class OtherSettings
    {
        public string Name { get; set; }
        public DateTime Dob { get; set; }
    }
}