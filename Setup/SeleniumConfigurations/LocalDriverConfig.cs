using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.SeleniumConfigurations
{
    /// <summary>
    /// Class covering local config type
    /// </summary>
    class LocalDriverConfing : DriverConfig
    {
        public LocalDriverConfing(string Browser, string ProxyUrl, string DownloadDirectory) : base(Browser, ProxyUrl, DownloadDirectory)
        {
            ConfigType = "local";
        }
    }
}
