using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework
{
    /// <summary>
    ///  Class wrapping all necessarz constats within framework
    /// </summary>
    class Constants
    {
        //timeout constants
        public const int SHORT_TIMEOUT = 2;
        public const int NAV_TIMEOUT = 5; 
        public const int DEFAULT_TIMEOUT = 10;
        public const int SAVING_TIMEOUT = 70;
        public const int OPENING_TIMEOUT = 60;
        public const int MEDIUM_TIMEOUT = 30;
        public const int PAGE_LOAD_TIMEOUT = 120;

        public static readonly List<String> UserDataFilePaths = new List<string> {
            "D:\\Testautomation\\ACC\\Setup\\ExternalData\\",
            "C:\\DBR\\TestAutomation\\Setup\\ExternalData\\",
            "Z:\\UI544918\\DBR\\TestAutomation\\Setup\\ExternalData\\"};

    }


    public enum RUNTYPE
    {
        GUI,
        WEBSERVICE,
        NOGUI
    }

    public enum BROWSER
    {
        CHROME,
        FIREFOX,
        IE
    }
    public enum LANGUAGE
    {
        EN,
        DE,
        OTHER
    }
}
