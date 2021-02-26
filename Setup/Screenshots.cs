using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using Allure.Commons;
using System.Threading;

namespace Framework
{
    [Binding]
    class Screenshots : TechTalk.SpecFlow.Steps
    {
        private IWebDriver Driver => ScenarioContext.Get<IWebDriver>("driver");
        private RunConfiguration Configuration => ScenarioContext.Get<RunConfiguration>("RunConfiguration");
        // [AfterStep()]
        /// <summary>
        /// old function using Driver and ITakeScreenshot to  create screenshot not javascript full screenshots
        /// 
        /// </summary>
        /// 
        private readonly ScenarioContext _scenarioContext;

        public Screenshots(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }
        public void MakeScreenshotAfterStep()
        {
            try
            {
                var takesScreenshot = Driver as ITakesScreenshot;
                if (takesScreenshot != null)
                {
                    var screenshot = takesScreenshot.GetScreenshot();
                    var tempFileName = Path.Combine(Directory.GetCurrentDirectory(), GetTempFileNameWithGuid("scr")) + ".jpg";
                    screenshot.SaveAsFile(tempFileName, ScreenshotImageFormat.Jpeg);
                    Console.WriteLine($"SCREENSHOT[ file:///{tempFileName} ]SCREENSHOT");
                }
                else
                {
                    Console.WriteLine("NO-SCREENSHOTS");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Encountered error in: MakeScreenshotAfterStep");
                Console.WriteLine("Encountered Source: " + e.Source);
                Console.WriteLine("Encountered ERROR: " + e.Message);
                Console.WriteLine("Encountered Inner exception: " + e.InnerException);
                Console.WriteLine("NO screenshot storred");
            }
        }
        /// <summary>
        /// function is creating full page screenshot after each step using javascirpt
        /// if IE is used, process is forwarded to MakeScreenshotAfterStep function
        /// can be switched off by "No"
        /// </summary>

        [AfterStep()]
        public void MakeScreenshotAfterStep2()
        {
            string tempFileName = "";

            if (Configuration.RunType == RUNTYPE.GUI && !ScenarioContext.ScenarioInfo.Tags.Contains("API"))
            {

                try
                {

                    if (!Configuration.Screenshots.ToLower().Equals("no"))
                    {
                        if (Configuration.Browser.Equals("internet explorer"))
                        {
                            MakeScreenshotAfterStep(); //using because off issue using javascript in IE
                        }
                        else
                        {
                            var takesScreenshot = Driver as ITakesScreenshot;

                            if (takesScreenshot != null)
                            {
                                var screenshot = GetEntireScreenshot(Driver);
                                //   Console.WriteLine($"Current directory: " + Directory.GetCurrentDirectory());
                                tempFileName = GetFileNameFromContext();
                                if (String.IsNullOrEmpty(tempFileName)) tempFileName = Path.Combine(Directory.GetCurrentDirectory(), GetTempFileNameWithGuid("scr")) + ".jpg";
                                else tempFileName = Path.Combine(Directory.GetCurrentDirectory(), tempFileName) + ".jpg";
                                // var tempFileName = Path.Combine(@"C:\Users\UI119817\Source\Repos\E2ETestAutomation\TestResults", Path.GetFileNameWithoutExtension(Path.GetTempFileName())) + ".jpg";
                                //  Console.WriteLine("file name:" + tempFileName);
                                screenshot.Save(tempFileName, ImageFormat.Jpeg);
                                //AllureLifecycle.Instance.AddAttachment(GetTempFileNameWithGuid("scr"), ".jpg", Directory.GetCurrentDirectory());        
                                
                                if(ScenarioContext.TestError != null)
                                {
                                    AllureLifecycle.Instance.AddAttachment(tempFileName);
                                }   
                                Console.WriteLine($"SCREENSHOT[ file:///{tempFileName} ]SCREENSHOT");
                            }
                            else
                            {
                                Console.WriteLine("NO-SCREENSHOTS");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("NO-SCREENSHOTS");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Encountered error in: MakeScreenshotAfterStep2 (javascript screenshot) ");
                    Console.WriteLine("Encountered Source: " + e.Source);
                    Console.WriteLine("Encountered ERROR: " + e.Message);
                    Console.WriteLine("Encountered Inner exception: " + e.InnerException);
                    Console.WriteLine("NO screenshot storred");
                    Console.WriteLine("Running no javascript screenshoting: ");
                    MakeScreenshotAfterStep();
                }
            }
        }


        /// <summary>
        /// this method is used du
        /// </summary>
        /// <param name="_Driver"></param>
        /// <param name="full_screen"></param>
        public static void MakeScreenshot(IWebDriver _Driver, bool full_screen)
        {

            try
            {
                // Console.WriteLine($"Current directory: " + Directory.GetCurrentDirectory());
                var tempFileName = Path.Combine(Directory.GetCurrentDirectory(), GetTempFileNameWithGuid("scr")) + ".jpg";
                // Console.WriteLine("file name:" + tempFileName);
                ICapabilities capabilities = ((OpenQA.Selenium.Remote.RemoteWebDriver)_Driver).Capabilities;
                //Console.WriteLine(_Driver.GetType());
                String BrowserName = capabilities.GetCapability("browserName").ToString();
                if (BrowserName.Equals("internet explorer"))
                {
                    full_screen = false;
                }

                if (full_screen)
                {
                    var screenshot = GetEntireScreenshot(_Driver);
                    //  Console.WriteLine($"Current directory: " +  Directory.GetCurrentDirectory());
                    tempFileName = Path.Combine(Directory.GetCurrentDirectory(), GetTempFileNameWithGuid("scr")) + ".jpg";
                    Console.WriteLine($"SCREENSHOT[ file:///{tempFileName} ]SCREENSHOT");
                    screenshot.Save(tempFileName, ImageFormat.Jpeg);

                }
                else
                {
                    var takesScreenshot = _Driver as ITakesScreenshot;

                    if (takesScreenshot != null)
                    {
                        var screenshot = takesScreenshot.GetScreenshot();
                        screenshot.SaveAsFile(tempFileName, ScreenshotImageFormat.Jpeg);
                        Console.WriteLine($"SCREENSHOT[ file:///{tempFileName} ]SCREENSHOT");
                    }
                    else
                    {
                        Console.WriteLine("NO-SCREENSHOTS");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Encountered Source: " + e.Source);
                Console.WriteLine("Encountered ERROR: " + e.Message);
                Console.WriteLine("Encountered Inner exception: " + e.InnerException);
                Console.WriteLine("NO screenshot storred");
            }



        }

        //depricated old version
        /// <summary>
        /// 
        /// </summary>
        //public static void MakeScreenshot()
        //{
        //    var takesScreenshot = Driver as ITakesScreenshot;
        //    if (takesScreenshot != null)
        //    {
        //        Image screenshot = GetEntireScreenshot();
        //        var tempFileName = Path.Combine(Directory.GetCurrentDirectory(), Path.GetFileNameWithoutExtension(Path.GetTempFileName())) + ".jpg";
        //        // Bitmap.set.
        //        screenshot.Save(tempFileName, ImageFormat.Jpeg);

        //        Console.WriteLine($"SCREENSHOT[ file:///{tempFileName} ]SCREENSHOT");
        //    }
        //    else
        //    {
        //        Console.WriteLine("NO-SCREENSHOTS");
        //    }

        //}
        //used for getting full page screenshots by javascript
        public static Image GetEntireScreenshot(IWebDriver _Driver)
        {
            IWebDriver driver = _Driver;
            ((IJavaScriptExecutor)driver).ExecuteScript(String.Format("window.scrollTo(0,0)"));
            // Get the total size of the page
            var totalWidth = (int)(long)((IJavaScriptExecutor)driver).ExecuteScript("return document.body.offsetWidth"); //documentElement.scrollWidth");
            var totalHeight = (int)(long)((IJavaScriptExecutor)driver).ExecuteScript("return  document.body.parentNode.scrollHeight");
            // Get the size of the viewport
            var viewportWidth = (int)(long)((IJavaScriptExecutor)driver).ExecuteScript("return document.body.clientWidth"); //documentElement.scrollWidth");
            var viewportHeight = (int)(long)((IJavaScriptExecutor)driver).ExecuteScript("return window.innerHeight"); //documentElement.scrollWidth");

            // We only care about taking multiple images together if it doesn't already fit
            if (totalWidth <= viewportWidth && totalHeight <= viewportHeight)
            {
                var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                return ScreenshotToImage(screenshot);
            }
            // Split the screen in multiple Rectangles
            var rectangles = new List<Rectangle>();
            // Loop until the totalHeight is reached
            for (var y = 0; y < totalHeight; y += viewportHeight)
            {
                var newHeight = viewportHeight;
                // Fix if the height of the element is too big
                if (y + viewportHeight > totalHeight)
                {
                    newHeight = totalHeight - y;
                }
                // Loop until the totalWidth is reached
                for (var x = 0; x < totalWidth; x += viewportWidth)
                {
                    var newWidth = viewportWidth;
                    // Fix if the Width of the Element is too big
                    if (x + viewportWidth > totalWidth)
                    {
                        newWidth = totalWidth - x;
                    }
                    // Create and add the Rectangle
                    var currRect = new Rectangle(x, y, newWidth, newHeight);
                    rectangles.Add(currRect);
                }
            }
            // Build the Image
            var stitchedImage = new Bitmap(totalWidth, totalHeight);
            // Get all Screenshots and stitch them together
            var previous = Rectangle.Empty;
            foreach (var rectangle in rectangles)
            {
                // Calculate the scrolling (if needed)
                if (previous != Rectangle.Empty)
                {
                    var xDiff = rectangle.Right - previous.Right;
                    var yDiff = rectangle.Bottom - previous.Bottom;
                    // Scroll
                    ((IJavaScriptExecutor)driver).ExecuteScript(String.Format("window.scrollBy({0}, {1})", xDiff, yDiff));
                }
                // Take Screenshot
                var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                // Build an Image out of the Screenshot
                var screenshotImage = ScreenshotToImage(screenshot);
                // Calculate the source Rectangle
                var sourceRectangle = new Rectangle(viewportWidth - rectangle.Width, viewportHeight - rectangle.Height, rectangle.Width, rectangle.Height);
                // Copy the Image
                using (var graphics = Graphics.FromImage(stitchedImage))
                {
                    graphics.DrawImage(screenshotImage, rectangle, sourceRectangle, GraphicsUnit.Pixel);
                }
                // Set the Previous Rectangle
                previous = rectangle;
            }
            // Actions actions = new Actions(driver);
            //  actions.MoveToElement(driver.FindElement(By.TagName("head")));
            //actions.Perform();



            ((IJavaScriptExecutor)driver).ExecuteScript(String.Format("window.scrollTo(0,0)"));
            //  Console.WriteLine("width : " + stitchedImage.Width + ", height: " + stitchedImage.Height);
            float width = (float)(totalWidth * 0.5);
            //   Console.WriteLine("A>" + width);
            float height = (float)(totalHeight * 0.5);
            //   Console.WriteLine("b>" + height);

            stitchedImage = new Bitmap(stitchedImage, (int)width, (int)height);
            //   Console.WriteLine("width : " + stitchedImage.Width + ", height: " + stitchedImage.Height);
            return stitchedImage;
        }

        private static Image ScreenshotToImage(Screenshot screenshot)
        {
            Image screenshotImage;
            using (var memStream = new MemoryStream(screenshot.AsByteArray))
            {
                screenshotImage = Image.FromStream(memStream);
            }
            return screenshotImage;
        }
        // generating unique file name for screenshot
        public static string GetTempFileNameWithGuid(string filePrefix)
        {
            string retFileName = string.Format("{0}{1}{2}",
                Guid.NewGuid(), filePrefix, Guid.NewGuid());
            return retFileName;
        }

        private string GetFileNameFromContext()
        {
            string fileName = "", imgCounterTxt = "01", imgContractId;
            int imgCounterInt = 0;

            if (!ScenarioContext.ScenarioInfo.Tags.Contains("userScanning"))
                return ""; // It is not user scanning scenario and the name of the screenshot should be created in a different way
            try
            {

                try
                { // increasing counter
                    _scenarioContext["Number1"] = "test";
                    imgCounterTxt = _scenarioContext["imgCounter"].ToString(); ;
                    if(imgCounterTxt[0] == '0') imgCounterTxt = imgCounterTxt.Substring(1, imgCounterTxt.Length - 1);
                    
                    imgCounterInt = Int32.Parse(imgCounterTxt);
                    imgCounterInt++;
                    if (imgCounterInt < 9) imgCounterTxt = "0" + imgCounterInt.ToString();
                    else imgCounterTxt = imgCounterInt.ToString();
                    _scenarioContext["imgCounter"] = imgCounterTxt;
                }
                catch
                {
                    //imgCounter = 1; // initial value for counter
                    Console.WriteLine("Initial value was set for step imgCounter: " + imgCounterTxt);
                    _scenarioContext["imgCounter"] = imgCounterTxt; // initial value for counter
                }

                string user = _scenarioContext["username"].ToString();
                string imgPageName = _scenarioContext["imgPageName"].ToString().Trim();

                try
                {
                    imgContractId = _scenarioContext["imgContractId"].ToString();
                }
                catch
                {
                    imgContractId = "ContractIdNull";
                }

                fileName = user + "_" + imgPageName + "_" + imgContractId + "_" + imgCounterTxt;
                // if(imgCounterInt > 4) Thread.Sleep(2000); //  waiting to complete a page before taking a screenshot when TC went through a Login process
            }
            catch (Exception e)
            {
                fileName = "";
                Console.WriteLine("The file name for screeshots is not available: " + e.Source + " " + e.Message);
            }

            return fileName;
        }
    }
}
