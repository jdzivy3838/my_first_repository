using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace Framework.SeleniumConfigurations
{
    /// <summary>
    /// class covering remote config type
    /// </summary>
    class RemoteDriverConfig:DriverConfig
    {
        public PlatformType Platform { get; set; }

        public string BrowserVersion { get; set; }

        public string SeleniumHubUrl { get; set; }

        public string SeleniumHubPort { get; set; }

        /// <summary>
        /// constructor for selenium grid remote web driver
        /// </summary>
        /// <param name="Browser"></param>
        /// <param name="ProxyUrl"></param>
        /// <param name="DownloadDirectory"></param>
        /// <param name="BrowserVersion"></param>
        /// <param name="Platform"></param>
        /// <param name="SeleniumHubUrl"></param>
        /// <param name="SeleniumHupPort"></param>
        public RemoteDriverConfig(string Browser, string ProxyUrl, string DownloadDirectory, string BrowserVersion, PlatformType Platform, string SeleniumHubUrl, string SeleniumHubPort) : base(Browser, ProxyUrl, DownloadDirectory)
        {
            ConfigType = "remote";
            this.BrowserVersion = BrowserVersion;
            this.Platform = Platform;
            this.SeleniumHubPort = SeleniumHubPort;
            this.SeleniumHubUrl = SeleniumHubUrl;
        }

    }
}
