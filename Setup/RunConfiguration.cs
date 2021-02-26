using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework
{
    /// <summary>
    /// Class is used for storing important information for execution of automated tests
    /// </summary>
    class RunConfiguration
    {

        public string Browser { get; set; }
        public string Screenshots { get; set; }
        public string DownloadDirectory { get; set; }
        public string SeleniumExecution { get; set; }
        public string Environment { get; set; }
        public string BasePath { get; set; }
        public DateTime StartExecution { get; set; }
        public DateTime EndExecution { get; set; }
        public RUNTYPE RunType { get; set; }
        public LANGUAGE Language { get; set; }
        
        public IDictionary<String, String> Parameters { set; get; }

        /// <summary>
        /// constructor setting new instance of Parameters dictionary
        /// </summary>
        public RunConfiguration()
        {
            Parameters = new Dictionary<String, String>();
        }
    }
}
