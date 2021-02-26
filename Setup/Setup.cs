using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using TechTalk.SpecFlow;
 using Framework.SeleniumConfigurations;
using System.Collections.Specialized;
using Allure.Commons;

namespace Framework
{
    [Binding]
    class Setup : TechTalk.SpecFlow.Steps
    {
        private const char separatorValue = ','; // '\t'

        public static Boolean IsGettingAdditionalLogs { get; set; }
        public static IDictionary<String, String> usersData { get; set; }



        //    [BeforeTestRun]
        /// <summary>
        /// InitEnvironment
        ///  is starter function for whole framework, loading of all setting and preform initialisation
        /// </summary>
        [BeforeScenario]
        public void InitEnvironment()
        {
            //loading of environemnt and run congiguration

            RunConfiguration _runConfiguration = new RunConfiguration();
            _runConfiguration.StartExecution = DateTime.Now;

            Log.Init(LOGTYPE.CONSOLE, LOGLEVEL.INFO);
            Log.Message(Log.LOG_INFO, "Initialising of environment");

            //  Log.Message(Log.LOG_INFO, "Starting WebDriver");
            _runConfiguration.Screenshots = GetData("Screenshots");
            _runConfiguration.Environment = GetData("Environment");
            Log.Message(Log.LOG_INFO, "Environment: " + _runConfiguration.Environment);
            _runConfiguration.BasePath = GetData("BasePath");
            Log.Message(Log.LOG_INFO, "BasePath: " + _runConfiguration.BasePath);
            switch (GetData("RunType").ToUpper())
            {
                case "GUI":
                    _runConfiguration.RunType = RUNTYPE.GUI;
                    Log.Message(Log.LOG_INFO, "RUNNING IN GUI MODE");
                    _runConfiguration.SeleniumExecution = GetData("SeleniumExecution");
                    break;
                case "NOGUI":
                    _runConfiguration.RunType = RUNTYPE.NOGUI;
                    Log.Message(Log.LOG_INFO, "RUNNING IN NOGUI MODE");
                    break;
                case "WEBSERVICE":
                    _runConfiguration.RunType = RUNTYPE.WEBSERVICE;
                    Log.Message(Log.LOG_INFO, "RUNNING IN WEBSERVICE MODE");
                    break;
                default:
                    _runConfiguration.RunType = RUNTYPE.GUI;
                    break;
            }
            switch (GetData("Language").ToUpper())
            {
                case "EN":
                    _runConfiguration.Language = LANGUAGE.EN;
                    Log.Message(Log.LOG_INFO, "LANGUAGE is EN");
                    break;
                case "DE":
                    _runConfiguration.Language = LANGUAGE.DE;
                    Log.Message(Log.LOG_INFO, "LANGUAGE is DE");
                    break;
                default:
                    _runConfiguration.Language = LANGUAGE.OTHER;
                    Log.Message(Log.LOG_INFO, "LANGUAGE is OTHER");
                    break;

            }

            if (_runConfiguration.RunType.Equals(RUNTYPE.GUI) && !ScenarioContext.ScenarioInfo.Tags.Contains("API"))
            {
                IWebDriver Driver = null;

                Log.Message(Log.LOG_INFO, "Setting GUI MODE");
                _runConfiguration.DownloadDirectory = GetData("DownloadDirectory");
                Log.Message(Log.LOG_INFO, "DOWNLOAD DIRECTORY: " + _runConfiguration.DownloadDirectory);
                _runConfiguration.Browser = GetData("Browser");
                Log.Message(Log.LOG_INFO, "USED BROWSER: " + _runConfiguration.Browser);


                if (_runConfiguration.SeleniumExecution.ToLower().Equals("local"))
                {
                    //todo use local driver config instead old call >> old call should be internal
                    String Proxy = GetData("proxy"); //todo proxy as parameter for run configuration??
                    Driver = WebDriverFactory.Create(_runConfiguration.Browser, _runConfiguration.DownloadDirectory, Proxy);

                }
                else if (_runConfiguration.SeleniumExecution.ToLower().Equals("remote"))
                {
                    String SeleniumHubUrl = GetData("SeleniumHubUrl");
                    String SeleniumHubPort = GetData("SeleniumHubPort");
                    String Proxy = GetData("proxy");
                    RemoteDriverConfig remote_config = new RemoteDriverConfig(_runConfiguration.Browser, Proxy, _runConfiguration.DownloadDirectory, "", GetPlatformType(GetData("Platform")), SeleniumHubUrl, SeleniumHubPort);
                    Driver = WebDriverFactory.Create(remote_config);
                }


                //loading al PARAM_XXX from app.config
                _runConfiguration.Parameters = LoadPARAMS();


                // Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);
                Driver.Manage().Window.Maximize();
                ScenarioContext.Add("driver", Driver);
                //   ScenarioContext.Current["driver"]= Driver;
            }
            else if (_runConfiguration.RunType.Equals(RUNTYPE.WEBSERVICE))
            {

                //Selenium is not going to by inicialized
                //add incialization of

                //loading al PARAM_XXX from app.config
                _runConfiguration.Parameters = LoadPARAMS();
            }
            else if (_runConfiguration.RunType.Equals(RUNTYPE.NOGUI))
            {

                //Selenium is not going to by inicialized
                //loading al PARAM_XXX from app.config
                _runConfiguration.Parameters = LoadPARAMS();
            }

            ScenarioContext.Add("RunConfiguration", _runConfiguration); //think about storing also driver to configuration
                                                                        // ScenarioContext.Current["configuration"] = runConfiguration;
            IsGettingAdditionalLogs = Boolean.Parse(GetData("IsGettingAdditionalLogs"));
            Log.Message(Log.LOG_INFO, "IsGettingAdditionalLogs " + Setup.IsGettingAdditionalLogs);
            usersData = ReadUsersData(_runConfiguration.Environment);

        }

        
        /// <summary>
        /// GetData
        /// function is wrapper for Configuration Manager
        /// </summary>
        /// <param name="name"> name o parameter to get from App config</param>
        /// <returns></returns>
        public static string GetData(string name)
        {
            //  add if name exists in UsersData , else see below
            if(usersData != null && usersData.Count > 0 && usersData.ContainsKey(name))
            {
                Log.Message(Log.LOG_INFO, "Key " + name + " was found in the users file");
                return usersData[name];
            }
            else
            {
                return ConfigurationManager.AppSettings[name];
            }
        }
        /// <summary>
        /// LoadPARAMS
        /// 
        /// </summary>
        /// 
        /// <returns></returns>
        public IDictionary<String, String> LoadPARAMS()
        {
            NameValueCollection settings = ConfigurationManager.GetSection("params")
                    as System.Collections.Specialized.NameValueCollection;
            
            IDictionary<String, String> dicParams = new Dictionary<String, String>();

            foreach (var item in settings.AllKeys)
            {
                dicParams.Add(item, settings[item].ToString());
            }

            return dicParams;
        }

        [AfterScenario(Order = 19)]
        public void UpdateAllureReportScenarios()
        {
            TestResult testresult;
            ScenarioContext.TryGetValue(out testresult);

            string scenario_name = testresult.historyId;
            try
            {
                scenario_name = scenario_name + " " + ScenarioContext.Current.Get<string>("scenario_name");
                Log.Message(Log.LOG_INFO, "Update Allure report -> Extended Scenario name is: " + scenario_name);
            }
            catch
            {
                
                // scenario_name = scenario_name + " " + Guid.NewGuid().ToString(); // to remove this line after fixing the all Scenario Outline
                Log.Message(Log.LOG_INFO, "Update Allure report -> There is not Extended Scenario name: " + scenario_name);
            }
            bool successGet = ScenarioContext.TryGetValue("brokenTestcase", out bool broken);
            AllureLifecycle.Instance.UpdateTestCase(testresult.uuid, tc =>
            {
                tc.name = ScenarioContext.Current.ScenarioInfo.Title; //In order to have different tests in report
                tc.historyId = scenario_name; /* this line ensures correct number of TCs for Scenario Outline */
                if (successGet && broken)
                {
                    tc.status = Status.broken;
                }
            });

        }

        /// <summary>
        ///  After scenario function stopping Web driver and performing clean up
        /// </summary>
        [AfterScenario(Order = 100)]
        public void StopTestServerAfterScenario()
        {
            Log.Message(Log.LOG_INFO, "RUNNING: AfterScenario - StopTestServerAfterScenario ");
            Log.Message(Log.LOG_INFO, ".........");
            
            Log.Message(Log.LOG_WARNING, "RES: " + ScenarioContext.Current.TestError);
            RunConfiguration Configuration = ScenarioContext.Get<RunConfiguration>("RunConfiguration");
            if (Configuration.RunType.Equals(RUNTYPE.GUI) && !ScenarioContext.ScenarioInfo.Tags.Contains("API"))
            {
                IWebDriver Driver = ScenarioContext.Get<IWebDriver>("driver");
                if (Driver != null)
                {
                    if (IsGettingAdditionalLogs == true)
                    {
                        Console.WriteLine("URL:" + Driver.Url);
                        if (!Driver.Url.Contains("kkobackoffice") || !Driver.Url.Contains("evocs"))
                        {
                            ReportAdditionalLogs(Driver, Configuration.Browser);
                        }
                    }
                    Driver.Close();
                    Driver.Quit();
                    Driver.Dispose();
                }
                Driver = null;
                ScenarioContext.Current["driver"] = null;
            }
            else
            {
                Log.Message(Log.LOG_INFO, "Running in NO GUI mode");

            }

        }

        /// <summary>
        /// Method will close provided Driver
        /// </summary>
        /// <param name="Driver"></param>
        public static void StopTestServer(IWebDriver Driver)
        {  
            Log.Message(Log.LOG_WARNING, "RES: " + ScenarioContext.Current.TestError);
            

            if (Driver != null)
            {
                if (IsGettingAdditionalLogs == true)
                {
                    Console.WriteLine("URL:" + Driver.Url);
                    if (!Driver.Url.Contains("kkobackoffice") || !Driver.Url.Contains("evocs"))
                    {
                        ReportAdditionalLogs(Driver, GetData("Browser"));
                    }
                }
                Driver.Close();
                Driver.Quit();
                Driver.Dispose();
            }
            Driver = null;
            ScenarioContext.Current["driver"] = null;
        }


        /// <summary>
        /// Method is providing basic process clean up
        /// --todo create better clean up
        /// </summary>
        public static void ProcessCleanup()
        {
            Log.Message(Log.LOG_INFO, "Process driver clean up");

            var allProcesses = System.Diagnostics.Process.GetProcesses();
            //todo find posibility how to kill process by start time
            foreach (var item in allProcesses)
            {
                
                // Log.Message(Log.LOG_INFO, "AAAAAprocess: " + item.ProcessName);
                if (item.ProcessName.Equals("geckodriver"))
                {
                    Log.Message(Log.LOG_INFO, "killing gecko driver with pid:" + item.Id);
                    item.Kill();
                    BrowserCleanup("firefox");
                    
                }
                if (item.ProcessName.Equals("chromedriver"))
                {
                    Log.Message(Log.LOG_INFO, "killing chrome driver with pid:" + item.Id);
                    
                    //item.Kill();
                    //BrowserCleanup("chrome");
                }
                if (item.ProcessName.Equals("iedriverserver"))
                {
                    Log.Message(Log.LOG_INFO, "killing ie driver with pid:" + item.Id);
                    item.Kill();
                }
            }
        }

        /// <summary>
        /// Kill browser instance: chrome.exe firefox.exe iexplore.exe
        /// </summary>
        /// <param name="browser"></param>
        private static void BrowserCleanup(string browser)
        {
            Log.Message(Log.LOG_INFO, "Process browser clean up");
            var allProcesses = System.Diagnostics.Process.GetProcesses();

            
            foreach (var item in allProcesses)
            {
                // Log.Message(Log.LOG_INFO, "AAAAAprocess: " + item.ProcessName);
                if (item.ProcessName.Equals(browser))
                {
                    Log.Message(Log.LOG_INFO, "killing browser instance: " + browser + " with id: " + item.Id);
                    item.Kill();
                }

            }
        }

        /// <summary>
        /// Method is providin additional console log report
        /// </summary>
        /// <param name="Driver"></param>
        /// <param name="Browser"></param>
        public static void ReportAdditionalLogs(IWebDriver Driver, String Browser)
        {

            if (!Browser.Equals("internet explorer") && !Browser.Equals("firefox"))
            {
                Log.Message(Log.LOG_INFO, "----GETTING ADDITIONAL LOGS----");
                
                    foreach (string item in Driver.Manage().Logs.AvailableLogTypes)
                    {
                        Log.Message(Log.LOG_INFO, "Log type: " + item);

                        foreach (LogEntry logEntry in Driver.Manage().Logs.GetLog(item))
                        {
                            Log.Message(Log.LOG_INFO, "\n" + logEntry.Timestamp + " >> " + logEntry.Message);

                        }
                    }

                    Log.Message(Log.LOG_INFO, "----END OF ADDITIONAL LOGS----");
                
            }
        }

        /// <summary>
        /// static method for initialisation of WebDriver based by configuration
        /// </summary>
        /// <param name="runConfiguration"></param>
        /// <returns>WebDriver instance</returns>
        public static IWebDriver InitDriver(RunConfiguration runConfiguration)
        {
            IWebDriver Driver;

            Log.Message(Log.LOG_INFO, "New Driver init ");
            Driver = WebDriverFactory.Create(runConfiguration.Browser, runConfiguration.DownloadDirectory, GetData("proxy"));
            Driver.Manage().Window.Maximize();
            return Driver;
        }
        /// <summary>
        /// This method converts the OS string in the app.config to the matching value in the PlatformType Enum.
        /// </summary>
        /// <returns>A PlatformType instance</returns>
        private static PlatformType GetPlatformType(String platformValue)
        {

            PlatformType platform;
            return Enum.TryParse(FirstCharToUpper(platformValue), out platform) ? platform : PlatformType.Windows;
        }
        /// <summary>
        /// In order to match the enum will have to Title case the OS string value.
        /// This is more protection for when someone enters the OS all lowercase
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string FirstCharToUpper(string input)
        {
            if (String.IsNullOrEmpty(input))
                throw new ArgumentException("The string was null or empty.");
            return input.First().ToString().ToUpper() + String.Join("", input.Skip(1)).ToLower();
        }
 

        //this function Convert to Encord your Password 
        public static string EncodeToBase64(string password)
        {
            try
            {
                byte[] encData_byte = new byte[password.Length];
                encData_byte = System.Text.Encoding.UTF8.GetBytes(password);
                string encodedData = Convert.ToBase64String(encData_byte);
                return encodedData;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in base64Encode" + ex.Message);
            }
        }

        //this function Convert to Decord your Password
        public static string DecodeFromBase64(string encodedData)
        {
            System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
            System.Text.Decoder utf8Decode = encoder.GetDecoder();
            byte[] todecode_byte = Convert.FromBase64String(encodedData);
            int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
            char[] decoded_char = new char[charCount];
            utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
            string result = new String(decoded_char);
            return result;
        }

        //encrypting password with secret key
        public static string Encrypt(string data)
        {
            Encoding unicode = Encoding.Unicode;
            string key = "seC135Ret";
           // return Convert.ToBase64String(Encrypt(unicode.GetBytes(key), unicode.GetBytes(data)));
            return Convert.ToBase64String(EncryptOutput(unicode.GetBytes(key), unicode.GetBytes(data)).ToArray());
        }
        //decrypting with secret key
        public static string Decrypt(string data)
        {
            Encoding unicode = Encoding.Unicode;
            string key = "seC135Ret";

            return unicode.GetString(EncryptOutput(unicode.GetBytes(key), Convert.FromBase64String(data)).ToArray());
        }

        //key encrypt
        private static byte[] EncryptInitalize(byte[] key)
        {
            byte[] s = Enumerable.Range(0, 256)
              .Select(i => (byte)i)
              .ToArray();

            for (int i = 0, j = 0; i < 256; i++)
            {
                j = (j + key[i % key.Length] + s[i]) & 255;

                Swap(s, i, j);
            }

            return s;
        }
        //output with key
        private static IEnumerable<byte> EncryptOutput(byte[] key, IEnumerable<byte> data)
        {
            byte[] s = EncryptInitalize(key);

            int i = 0;
            int j = 0;

            return data.Select((b) =>
            {
                i = (i + 1) & 255;
                j = (j + s[i]) & 255;

                Swap(s, i, j);

                return (byte)(b ^ s[(s[i] + s[j]) & 255]);
            });
        }

        private static void Swap(byte[] s, int i, int j)
        {
            byte c = s[i];

            s[i] = s[j];
            s[j] = c;
        }

        public IDictionary<String, String> ReadUsersData(string env)
        {
            usersData = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            string userDataFile = "no_file_found";
            bool foundDataFile = false;

            foreach (string dataPath in Constants.UserDataFilePaths)
            {
                userDataFile = dataPath + "Users_" + env + ".csv";
                if (System.IO.File.Exists(userDataFile))
                {
                    Log.Message(Log.LOG_INFO, "Reading  file " + userDataFile + " exists");
                    foundDataFile = true;
                    break;
                }
            }

            if (!foundDataFile)
            {
                Log.Message(Log.LOG_WARNING, "Reading  file doesn't exist.");
                return null;
            }

            string[] lines = System.IO.File.ReadAllLines(userDataFile);
            string[] readValues;

            foreach (string line in lines)
            {
                if (line.Contains(separatorValue))
                {
                    readValues = line.Split(separatorValue);
                    try
                    {
                        if (!String.IsNullOrEmpty(readValues[0]) && !String.IsNullOrEmpty(readValues[1]))
                            usersData.Add(readValues[0], readValues[1]); // usersData.Add(Key, Value);
                        else Log.Message(Log.LOG_INFO, "Key or value was empty. Key: " + readValues[0] + " value: " + readValues[1] + "...");
                    }
                    catch
                    {
                        Log.Message(Log.LOG_ERROR, "Wrong data:" + readValues[0] + ";" + readValues[1] + ";");
                        return null;
                    }

                }
                else Log.Message(Log.LOG_INFO, "Line missing separator value. Content of the line: " + line + "...");
            }
            Log.Message(Log.LOG_INFO, "Users settings was reading from the file " + userDataFile);
            return usersData;
        }
    }
}
