using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace Framework.SeleniumConfigurations
{
    /// <summary>
    /// Abstract class covering Driver config base functions
    /// </summary>
    abstract class DriverConfig
    {
        public string Browser { get; set; }
        public string ProxyUrl { get; set; }
        public string DownloadDirectory { get; set; }
        public string ConfigType { get; set; }

        public DriverConfig()
        {

        }
        /// <summary>
        /// basic constructor with inputs
        /// </summary>
        /// <param name="Browser"></param>
        /// <param name="ProxyUrl"></param>
        /// <param name="DownloadDirectory"></param>
        public DriverConfig(string Browser, string ProxyUrl, string DownloadDirectory)
        {
            this.Browser = Browser;
            this.ProxyUrl = ProxyUrl;
            this.DownloadDirectory = DownloadDirectory;
        }
    }
}
