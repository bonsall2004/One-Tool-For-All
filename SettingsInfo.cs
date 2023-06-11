using System.Collections.Generic;

namespace OTFA
{
    public class GetValues
    {
        public string warning { get; set; }
        public List<Scripts> scripts { get; set; }
        public List<string> command { get; set; }
    }

    public class Scripts
    {
        public bool isRegistry { get; set; }
        public string keyLocation { get; set; }
        public string keyName { get; set; }
        public string keyPath { get; set; }
        public string valueName { get; set; }
        public string keyOffValue { get; set; }
        public string keyOnValue { get; set; }
        public string keyType { get; set; }

    }
    public class GeneralInfo
    {
        public string codeName { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public bool safe { get; set; }
        public string category { get; set; }
        public string version { get; set; }
        public bool userSetting { get; set; }
        public GetValues getValues { get; set; }

    }

    public class Root
    {
        public Dictionary<string, GeneralInfo> general { get; set; }
        public string version { get; set; }
    }
}