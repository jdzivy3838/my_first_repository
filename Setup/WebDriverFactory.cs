using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Drawing;
using Framework.SeleniumConfigurations;

namespace Framework
{
    /// <summary>
    /// Class is responsible for creating WebDriver instance base on configuration or inputs
    /// </summary>
    class WebDriverFactory
    {
        //DesiredCapabilites is used to add specific browser requirements to RemoteWebDriver, such as version, OS.
        //  private static DesiredCapabilities _capabilities;
        //Declare an instance of an IWebDriver to assign a browser driver to.
        //We use IWebDriver as all browsers use this interface, so all compatible.
        // private static IWebDriver _driver;

        //   public static String _downloadDirectory;
      

        /// <summary>
        /// This method is creating local driver according to inputs
        /// </summary>
        /// <param name="browser"> type browser </param>
        /// <param name="downloadDirectory">path to download directory</param>
        /// <param name="ProxyUrl"> proxy url</param>
        /// <returns></returns>
        public static IWebDriver Create(String browser, String downloadDirectory, String ProxyUrl)
        {
            IWebDriver _driver;
            //    DesiredCapabilities _capabilities;
            try
            {

                //Console.WriteLine("AAA:" + Directory.GetCurrentDirectory());
                //   string proxyUrl = "proxy.rwe.com:8080"; //hardcoded please change
                string proxyUrl = ProxyUrl;
                Proxy proxy = new Proxy();
                proxy.Kind = ProxyKind.Manual;
                proxy.IsAutoDetect = false;
                  proxy.HttpProxy = proxyUrl;
                  proxy.SslProxy = proxyUrl;
                //  proxy.FtpProxy = proxyUrl;
                //A simple switch statement to determine which driver/service to create.
                switch (browser)
                {
                    case "chrome":
                        ChromeOptions options_chrome = new ChromeOptions();
                        options_chrome.Proxy = proxy;
                        options_chrome.AddArgument("start-maximized");
                        options_chrome.AddArgument("ignore-certificate-errors");
                        options_chrome.AddArgument("disable-gpu");
                        options_chrome.AddArgument("--no-sandbox");
                        options_chrome.PageLoadStrategy = PageLoadStrategy.None;
                        //   options_chrome.AddArguments(@"--proxy-server=http://GROUP\UI1198170:Heslo321@" + proxyUrl);
                        options_chrome.AddUserProfilePreference("download.default_directory", downloadDirectory);
                        options_chrome.UnhandledPromptBehavior = UnhandledPromptBehavior.Ignore;
                        _driver = new ChromeDriver(options_chrome);
                        break;
                    case "chrome_headless":
                        ChromeOptions options_chrome_headless = new ChromeOptions();
                        options_chrome_headless.Proxy = proxy;
                        options_chrome_headless.AddArgument("ignore-certificate-errors");
                        options_chrome_headless.AddUserProfilePreference("download.default_directory", downloadDirectory);
                        options_chrome_headless.AddArgument("headless");
                        options_chrome_headless.AddArgument("disable-gpu");
                        options_chrome_headless.AddArgument("window-size=1600x1050");
                        options_chrome_headless.UnhandledPromptBehavior = UnhandledPromptBehavior.Ignore;
                        _driver = new ChromeDriver(options_chrome_headless);
                        break;
                    case "chrome_mobile":
                        ChromeOptions options_chrome_mobile = new ChromeOptions();
                        options_chrome_mobile.Proxy = proxy;
                        options_chrome_mobile.AddArgument("ignore-certificate-errors");
                        options_chrome_mobile.AddUserProfilePreference("download.default_directory", downloadDirectory);
                        options_chrome_mobile.UnhandledPromptBehavior = UnhandledPromptBehavior.Ignore;
                        //ChromeMobileEmulationDeviceSettings settings = new ChromeMobileEmulationDeviceSettings();                       
                        options_chrome_mobile.EnableMobileEmulation("Nexus 5");
                        _driver = new ChromeDriver(options_chrome_mobile);
                        break;
                    case "internet explorer":
                        var option_ie = new InternetExplorerOptions { EnableNativeEvents = false };
                        option_ie.Proxy = proxy;
                        option_ie.IntroduceInstabilityByIgnoringProtectedModeSettings = true;
                        option_ie.IgnoreZoomLevel = true;
                        //  option1s.EnableFullPageScreenshot = true;
                        option_ie.EnableNativeEvents = true;
                        option_ie.RequireWindowFocus = true;
                        option_ie.EnsureCleanSession = true;
                        option_ie.BrowserVersion = "11";
                        option_ie.UnhandledPromptBehavior = UnhandledPromptBehavior.Ignore;
                        //  option1s.EnablePersistentHover = true;
                        _driver = new InternetExplorerDriver(option_ie);
                        break;
                    case "firefox":
                        //Environment.SetEnvironmentVariable("webdriver.gecko.driver", @"C:\Users\UI119817\Source\Repos\E2ETestAutomation\TestAutomation\DriverServices\geckodriver.exe");
                        FirefoxProfile profile = new FirefoxProfile();
                        profile.AcceptUntrustedCertificates = true;
                        profile.SetPreference("webdriver_assume_untrusted_issuer", false);
                        profile.SetPreference("browser.download.dir", downloadDirectory);
                        profile.SetPreference("browser.download.folderList", 2);
                        profile.SetPreference("browser.helperApps.neverAsk.saveToDisk",
                                "image/jpeg, application/pdf, application/octet-stream");
                        profile.SetPreference("pdfjs.disabled", true);

                        FirefoxOptions options_firefox = new FirefoxOptions();
                        options_firefox.Proxy = proxy;
                        options_firefox.AddArgument("--ignore-certificate-errors");
                        options_firefox.Profile = profile;
                        options_firefox.AcceptInsecureCertificates = true;
                        //  options1.UnhandledPromptBehavior = UnhandledPromptBehavior.Ignore;
                        // options1.AddAdditionalCapability(CapabilityType.UnhandledPromptBehavior, UnhandledPromptBehavior.Ignore);
                        // _driver = new FirefoxDriver(Directory.GetCurrentDirectory(), options1);
                        _driver = new FirefoxDriver(options_firefox);

                        break;
                    case "firefox_headless":
                        //Environment.SetEnvironmentVariable("webdriver.gecko.driver", @"C:\Users\UI119817\Source\Repos\E2ETestAutomation\TestAutomation\DriverServices\geckodriver.exe");
                        FirefoxProfile profile_h = new FirefoxProfile();
                        profile_h.AcceptUntrustedCertificates = true;
                        profile_h.SetPreference("webdriver_assume_untrusted_issuer", false);
                        profile_h.SetPreference("browser.download.dir", downloadDirectory);
                        profile_h.SetPreference("browser.download.folderList", 2);
                        profile_h.SetPreference("browser.helperApps.neverAsk.saveToDisk",
                                "image/jpeg, application/pdf, application/octet-stream");
                        profile_h.SetPreference("pdfjs.disabled", true);

                        FirefoxOptions options_firefox_h = new FirefoxOptions();
                        options_firefox_h.Proxy = proxy;
                        options_firefox_h.AddArgument("--ignore-certificate-errors");
                        options_firefox_h.AddArguments("--headless");
                        options_firefox_h.Profile = profile_h;
                        options_firefox_h.AcceptInsecureCertificates = true;
                        //  options1.UnhandledPromptBehavior = UnhandledPromptBehavior.Ignore;
                        // options1.AddAdditionalCapability(CapabilityType.UnhandledPromptBehavior, UnhandledPromptBehavior.Ignore);
                        // _driver = new FirefoxDriver(Directory.GetCurrentDirectory(), options1);
                        _driver = new FirefoxDriver(options_firefox_h);

                        break;
                    /* case "phantomjs": //phantomJS is no mor supported by Selenium
                         var phantomJSDriverService = PhantomJSDriverService.CreateDefaultService();
                         PhantomJSOptions pjsoptions = new PhantomJSOptions();
                         pjsoptions.AddAdditionalCapability("phantomjs.page.settings.userAgent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_10_1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.95 Safari/537.36");
                         _driver = new PhantomJSDriver(pjsoptions);
                         _driver.Manage().Window.Size = new Size(1600, 1050);
                         break;*/

                    default:
                        throw new Exception("choose correct supported browser driver");
                        // break;
                }

                //Return the driver instance to the calling class.
                return _driver;
            }
            catch (WebDriverException e)
            {
                Log.Message(Log.LOG_INFO, "Caught exception:" + e.Message);


                throw e;
            }
        }

        /// <summary>
        /// Static function is creating instance of WebDriver based on configuration -> local or remote
        /// Remote configuration is utlising Selenium Grid
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns>instance of IWebDriver</returns>
        public static IWebDriver Create(DriverConfig configuration)
        {

            if (configuration.ConfigType.ToLower().Equals("local"))
            {
                return WebDriverFactory.Create(configuration.Browser, configuration.DownloadDirectory, configuration.ProxyUrl);
            }
            else// everything what is not local is remote?
            {

                try
                {
                    RemoteDriverConfig remote_config = (RemoteDriverConfig)configuration;
                    var remoteServer = BuildRemoteServer(remote_config.SeleniumHubUrl, remote_config.SeleniumHubPort);
                    Log.Message(Log.LOG_INFO, "REMOTE SERVER: " + remoteServer);
                    DriverOptions options = null;

                    //Console.WriteLine("AAA:" + Directory.GetCurrentDirectory());
                    string proxyUrl = remote_config.ProxyUrl; //hardcoded please change
                    Proxy proxy = new Proxy();
                    proxy.Kind = ProxyKind.Manual;
                    proxy.IsAutoDetect = false;
                    proxy.HttpProxy = proxyUrl;
                    proxy.SslProxy = proxyUrl;
                    proxy.FtpProxy = proxyUrl;

                    //A simple switch statement to determine which driver/service to create.
                    switch (remote_config.Browser)
                    {
                        case "chrome":
                            ChromeOptions options_chrome = new ChromeOptions();
                            options_chrome.Proxy = proxy;
                            options_chrome.AddArgument("ignore-certificate-errors");
                            options_chrome.AddUserProfilePreference("download.default_directory", remote_config.DownloadDirectory);
                            options_chrome.UnhandledPromptBehavior = UnhandledPromptBehavior.Ignore;
                            options = options_chrome;
                            break;
                        case "chrome_headless":
                            ChromeOptions options_chrome_headless = new ChromeOptions();
                            options_chrome_headless.Proxy = proxy;
                            options_chrome_headless.AddArgument("ignore-certificate-errors");
                            options_chrome_headless.AddUserProfilePreference("download.default_directory", remote_config.DownloadDirectory);
                            options_chrome_headless.AddArgument("headless");
                            options_chrome_headless.AddArgument("window-size=1600x1050");
                            options = options_chrome_headless;
                            break;
                        case "chrome_mobile":
                            ChromeOptions options_chrome_mobile = new ChromeOptions();
                            options_chrome_mobile.Proxy = proxy;
                            options_chrome_mobile.AddArgument("ignore-certificate-errors");
                            options_chrome_mobile.AddUserProfilePreference("download.default_directory", remote_config.DownloadDirectory);
                            options_chrome_mobile.UnhandledPromptBehavior = UnhandledPromptBehavior.Ignore;
                            options_chrome_mobile.EnableMobileEmulation("Nexus 5");
                            options = options_chrome_mobile;
                            break;
                        case "internet explorer":
                            InternetExplorerOptions option_ie = new InternetExplorerOptions { EnableNativeEvents = false };
                            option_ie.IntroduceInstabilityByIgnoringProtectedModeSettings = true;
                            option_ie.IgnoreZoomLevel = true;
                            //  option1s.EnableFullPageScreenshot = true;
                            option_ie.EnableNativeEvents = true;
                            option_ie.RequireWindowFocus = true;
                            option_ie.EnsureCleanSession = true;
                            option_ie.BrowserVersion = "11";
                            option_ie.UnhandledPromptBehavior = UnhandledPromptBehavior.Ignore;
                            options = option_ie;
                            break;

                        case "firefox":
                            //Environment.SetEnvironmentVariable("webdriver.gecko.driver", @"C:\Users\UI119817\Source\Repos\E2ETestAutomation\TestAutomation\DriverServices\geckodriver.exe");
                            FirefoxProfile profile = new FirefoxProfile();
                            profile.AcceptUntrustedCertificates = true;
                            profile.SetPreference("webdriver_assume_untrusted_issuer", false);
                            profile.SetPreference("browser.download.dir", remote_config.DownloadDirectory);
                            profile.SetPreference("browser.download.folderList", 2);
                            profile.SetPreference("browser.helperApps.neverAsk.saveToDisk",
                                    "image/jpeg, application/pdf, application/octet-stream");
                            profile.SetPreference("pdfjs.disabled", true);

                            FirefoxOptions options_firefox = new FirefoxOptions();
                            options_firefox.Proxy = proxy;
                            options_firefox.AddArgument("--ignore-certificate-errors");
                            options_firefox.Profile = profile;
                            options_firefox.AcceptInsecureCertificates = true;
                            options = options_firefox;
                            //  options1.UnhandledPromptBehavior = UnhandledPromptBehavior.Ignore;
                            // options1.AddAdditionalCapability(CapabilityType.UnhandledPromptBehavior, UnhandledPromptBehavior.Ignore);
                            // _driver = new FirefoxDriver(Directory.GetCurrentDirectory(), options1);
                            break;

                        default:
                            throw new Exception("choose correct supported browser driver");
                            // break;
                    }

                    //Return the driver instance to the calling class.
                    Uri u = new Uri(remoteServer);
                    return new RemoteWebDriver(u, options); // return new RemoteWebDriver(new Uri(remoteServer), options);

                }
                catch (WebDriverException e)
                {
                    Log.Message(Log.LOG_INFO, "Caught exception:" + e.Message);

                    throw e;
                }
            }
        }

        
        /// <summary>
        /// Build a Uri for your GRID Hub instance
        /// </summary>
        /// <param name="remoteServer">The hostname or IP address of your GRID instance, include the http://</param>
        /// <param name="remoteServerPort">Port of your GRID Hub instance</param>
        /// <returns>The correct Uri as a string</returns>
        private static string BuildRemoteServer(string remoteServer, string remoteServerPort)
        {
            return string.Format("{0}:{1}/wd/hub/", remoteServer, remoteServerPort);
        }
    }
}
