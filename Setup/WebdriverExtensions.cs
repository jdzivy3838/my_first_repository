using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Configuration;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using SeleniumExtras.WaitHelpers;
using System.Linq;

namespace Framework
{
    /// <summary>
    /// Webdriver extensions - custom waits, screenshot functionality, element highlight, etc.
    /// </summary>
    public static class WebdriverExtensions
    {

        /// <summary>
        /// Takes screenshot and saves it as jpg
        /// </summary>
        /// <param name="driver">Current webdriver instance</param>
        /// <param name="testName">Name of actually running test</param>
        /// //please use Setup.MakeScreenshotAfter2
        public static void TakeScreenshot(this IWebDriver driver, string testName)
        {
            string timestamp = DateTime.Now.ToString("MMddHHmm");

            try
            {
                string screenshotsLocation = ConfigurationManager.AppSettings["screenshotsLocation"];
                Screenshot screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                Directory.CreateDirectory(screenshotsLocation);
                //  screenshot.SaveAsFile(screenshotsLocation + testName + "_" + timestamp + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, "Screenshot was not taken");
            }
        }


        /// <summary>
        /// Waits for the element to be clickable
        /// </summary>
        /// <param name="element">Element to wait until clickable</param>
        /// <param name="driver">Current webdriver instance</param>
        public static void WaitElementToBeClickable(this IWebDriver driver, IWebElement element, int timeout = Constants.DEFAULT_TIMEOUT)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
            wait.Until<IWebElement>(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(element));
        }

        /// <summary>
        /// Wait for element to exist in DOM
        /// </summary>
        /// <param name="Driver">Current webdriver instance</param>
        /// <param name="element_selector">Selector of element to wait until exists</param>
        /// <param name="timeout">Duration in seconds to wait for element until exists</param>
        /// <returns></returns>
        public static IWebElement WaitForElementExists(this IWebDriver Driver, By element_selector, int timeout = Constants.MEDIUM_TIMEOUT)
        {
            WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeout));

            IWebElement element = wait.Until<IWebElement>(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(element_selector));

            return element;
        }

        /// <summary>
        /// Waits for element to disappear - useful e.g. on page reloads
        /// Checks whether an element is not visible or not present in the DOM
        /// </summary>
        /// <param name="driver">Current webdriver instance</param>
        /// <param name="element">Locator of element to check whether element has disappeared</param>
        /// <param name="timeout">Time to wait for element in seconds until it has disappeared</param>
        public static void WaitForElementToDisappear(this IWebDriver driver, By element_locater, int timeout = Constants.DEFAULT_TIMEOUT)
        {
            //
            try
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.InvisibilityOfElementLocated(element_locater));
            }
            catch (WebDriverTimeoutException e)
            {
                Log.Message(Log.LOG_ERROR, "Element did not disappear");
                Log.Message(Log.LOG_ERROR, e.Source);
                //throw;
            }
        }
        public static bool WaitForElementToBeDisappeared(this IWebDriver driver, By element_locater, int timeout = Constants.DEFAULT_TIMEOUT)
        {
            bool result = false;
            try
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
                result = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.InvisibilityOfElementLocated(element_locater));
                Log.Message(Log.LOG_INFO, "Element disappeared");

            }
            catch (WebDriverTimeoutException e)
            {
                Log.Message(Log.LOG_ERROR, "Element did not disappear");
                Log.Message(Log.LOG_ERROR, e.Source);
                //throw;
            }
            return result;
        }
        /// <summary>
        /// Waits for element to disappear - useful e.g. on page reloads
        /// </summary>
        /// <param name="driver">Current webdriver instance</param>
        /// <param name="element"></param>
        /// <param name="timeout">Max. Time to wait for element in seconds until it is displayed</param>
        public static void WaitForElementToDisplayed(this IWebDriver driver, IWebElement element, int timeout = Constants.DEFAULT_TIMEOUT)
        {
            try
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
                wait.Until(d => element.Displayed);
            }
            catch (WebDriverException e)
            {
                Log.Message(Log.LOG_ERROR, " Element did not display as expected");
                Log.Message(Log.LOG_ERROR, e.Message);
                Log.Message(Log.LOG_ERROR, e.Source);

            }
        }
        /// <summary>
        /// Waits for element to disappear - useful e.g. on page reloads
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="element"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static bool WaitForElementToBeDisplayed(this IWebDriver driver, IWebElement element, int timeout = Constants.DEFAULT_TIMEOUT)
        {
            bool eval = false;
            try
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
                eval = wait.Until(d => element.Displayed);
            }
            catch (WebDriverException e)
            {
                Log.Message(Log.LOG_ERROR, " Element did not display as expected");
                Log.Message(Log.LOG_ERROR, e.Message);
                Log.Message(Log.LOG_ERROR, e.Source);
                eval = false;
            }
            return eval;
        }

        public static void FrameVisibleAndSwitchTo(this IWebDriver driver, By iFrame, int timeout = Constants.DEFAULT_TIMEOUT)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
            wait.Until(d => SeleniumExtras.WaitHelpers.ExpectedConditions.FrameToBeAvailableAndSwitchToIt(iFrame));
        }
        /// <summary>
        /// Wait for text to appear in webelement
        /// </summary>
        /// <param name="driver">Current webdriver instance</param>
        /// <param name="element">element to be expected to contain text</param>
        /// <param name="textToAppear">text to be waited for</param>
        public static void WaitForTextToAppear(this IWebDriver driver, IWebElement element, string textToAppear, int timeout = Constants.DEFAULT_TIMEOUT)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.TextToBePresentInElement(element, textToAppear));
        }

        /// <summary>
        /// Waits for page javascript to be completed
        /// </summary>
        /// <param name="driver">Current webdriver instance</param>
        public static void WaitForScriptToFinish(this IWebDriver driver, int timeout = Constants.OPENING_TIMEOUT)
        {
            var javaScriptExecutor = driver as IJavaScriptExecutor;
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
            wait.Until(d => Equals(javaScriptExecutor.ExecuteScript("return document.readyState"), "complete"));
        }

        /// <summary>
        /// Execute Script
        /// </summary>
        /// <param name="driver">Current webdriver instance</param>
        public static IJavaScriptExecutor Scripts(this IWebDriver driver)
        {
            return (IJavaScriptExecutor)driver;
        }


        /// <summary>
        /// Waits for url to contain a string
        /// </summary>
        /// <param name="driver">Current webdriver instance</param>
        /// <param name="urlPart">A string that has to be included in url</param>
        public static void WaitForUrlToContain(this IWebDriver driver, string urlPart, int timeout = Constants.MEDIUM_TIMEOUT)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
            try
            {
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.UrlContains(urlPart));
            }
            catch (WebDriverTimeoutException e)
            {
                Log.Message(Log.LOG_ERROR, "Given url that contains " + urlPart + " was not loaded");
                Log.Message(Log.LOG_ERROR, e.Message);
                Log.Message(Log.LOG_ERROR, e.Source);
                throw;
            }
        }

        /// <summary>
        /// Wait until current url has changed to expected url
        /// </summary>
        /// <param name="driver">Current Webdriver instance</param>
        /// <param name="url">Url to be extpected</param>
        public static void WaitForUrlToBe(this IWebDriver driver, string url, int timeout = Constants.DEFAULT_TIMEOUT)
        {

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
            try
            {
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.UrlToBe(url));

            }
            catch (WebDriverTimeoutException)
            {
                Log.Message(Log.LOG_ERROR, "Given url " + url + " was not loaded");
                throw;
            }
        }

        /// <summary>
        /// Changes window width to a width set by parameter while leaving the height as it was.
        /// </summary>
        /// <param name="driver">Current webdriver instance</param>
        /// <param name="windowWidth">Intended windo width</param>
        public static void ChangeWindowWidth(this IWebDriver driver, int windowWidth)
        {
            int actualWindowHeight = driver.Manage().Window.Size.Height;
            driver.Manage().Window.Size = new Size(windowWidth, actualWindowHeight);
        }

        /// <summary>
        /// Wait until list of elements greater than 1
        /// </summary>
        /// <param name="driver">Current webdriver instance</param>
        /// <param name="list">List of webelements</param>
        public static void WaitForListOfElements(this IWebDriver driver, IList<IWebElement> list, int timeout = Constants.DEFAULT_TIMEOUT)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
            wait.Until(d => (list.Count > 1));
        }

        public static void WaitForElementVisibility(this IWebDriver driver, By element, int timeout = Constants.DEFAULT_TIMEOUT)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.VisibilityOfAllElementsLocatedBy(element));
        }
        /// <summary>
        /// Wait until page is loaded
        /// </summary>
        /// <param name="driver"></param>
        public static void WaitForPageToLoad(this IWebDriver driver)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(Constants.PAGE_LOAD_TIMEOUT));
            // Does not work for sitecore_backend
            //wait.Until(WaitforAttributeValue(By.TagName("html"), "class", "fonts-all-loaded"));

            wait.Until(d => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
        }

        /// <summary>
        /// wait for atrribute which contains value
        /// </summary>
        /// <param name="locator">Locator of element which attribute is checked</param>
        /// <param name="attribute">Attribute of element that is checked</param>
        /// <param name="expectedValue">Value that is expected </param>
        /// <returns>Return value of element attribute</returns>
        public static Func<IWebDriver, string> WaitforAttributeValue(By locator, string attribute, string expectedValue)
        {
            return (driver) =>
            {
                try
                {
                    var value = driver.FindElement(locator).GetAttribute(attribute);
                    return value.Contains(expectedValue) ? value : null;
                }
                catch (NoSuchElementException)
                {
                    Log.Message(Log.LOG_WARNING, "NoSuchElementException catched during waiting for element's attribute");
                    return null;
                }
                catch (StaleElementReferenceException)
                {
                    Log.Message(Log.LOG_WARNING, "StaleElementReferenceException catched during waiting for element's attribute");
                    return null;
                }
            };
        }

        public static void WaitForElementAttributeToBe(this IWebDriver driver, IWebElement element, String attribute, String expectedString, int timeout = Constants.DEFAULT_TIMEOUT)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
            wait.Until(d => element.GetAttribute(attribute).Contains(expectedString));
        }

        /// <summary>
        /// Wait for Ajax to be complete
        /// </summary>
        /// <param name="driver">Current webdriver instance</param>
        /// <param name="maxSeconds">Max seconds to wait until ajax is complete</param>
        public static void WaitForAjaxComplete(this IWebDriver driver, int maxSeconds)
        {
            bool isAjaxCallComplete = false;
            for (int i = 1; i <= maxSeconds; i++)
            {
                isAjaxCallComplete = (bool)((IJavaScriptExecutor)driver).
                ExecuteScript("return window.jQuery != undefined && jQuery.active == 0");
                if (isAjaxCallComplete)
                {
                    return;
                }
                Thread.Sleep(1000);
            }
            throw new Exception(string.Format("Timed out after {0} seconds", maxSeconds));
        }

        /// <summary>
        /// Wait until number of window handles becomes 2
        /// </summary>
        /// <param name="driver">Current webdriver instance</param>
        /// <returns>true if 2 window handles exists</returns>
        public static bool WaitForWindow(this IWebDriver driver)
        {
            //wait until number of window handles become 2 or until 6 seconds are completed.
            //true if window appeared
            //false if window not appeared
            int timecount = 1;
            do
            {
                Thread.Sleep(200);
                timecount++;
                if (timecount > 30)
                {
                    return false;
                }
            } while (driver.WindowHandles.Count != 2);

            return true;
        }

        /// <summary>
        /// Move element into viewport
        /// </summary>
        /// <param name="driver">Current webdriver instance</param>
        /// <param name="element">Webelement to be moved into viewport</param>
        public static void MoveElementIntoViewport(this IWebDriver driver, IWebElement element)
        {
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", element);
        }

        /// <summary>
        /// Switch to modal dialog window
        /// </summary>
        /// <param name="driver">Current webdriver instance</param>
        /// <param name="parent">Current window handle</param>
        /// <returns></returns>
        public static bool SwitchToModalDialog(this IWebDriver driver, String parent)
        {
            //Switch to Modal dialog
            if (driver.WindowHandles.Count == 2)
            {
                foreach (String window in driver.WindowHandles)
                {
                    if (!window.Equals(parent))
                    {
                        driver.SwitchTo().Window(window);
                        // System.out.println("Modal dialog found");
                        return true;
                    }
                }

            }
            return false;
        }

        /// <summary>
        /// Switch to common alert window
        /// </summary>
        /// <param name="driver">Current webdriver instance</param>
        /// <returns>Alert element</returns>
        public static IAlert switchToAlert(this IWebDriver driver)
        {
            //Switch to Alert
            if (driver.WindowHandles.Count == 2)
            {
                return driver.SwitchTo().Alert();

            }
            Log.Message(Log.LOG_INFO, "No alert window appeared");
            return null;
        }


        /// <summary>
        /// Navigate to given URL
        /// </summary>
        /// <param name="Driver">Current webdriver instance</param>
        /// <param name="Url">URL to navigate</param>
        public static void GoToURL(IWebDriver Driver, string Url)
        {
            Log.Message(Log.LOG_INFO, "Open the given URL:" + Url);
            Driver.Navigate().GoToUrl(Url);
            string currentUrl = Driver.Url;
            Log.Message(Log.LOG_INFO, "Current Url is: " + currentUrl);
        }

        /// <summary>
        /// Open new tab in browser and switch to tab automatically
        /// </summary>
        /// <param name="Driver">Current webdriver instance</param>
        public static void OpenNewTab(this IWebDriver Driver)
        {
            // Store current tab to switch back later

            Log.Message(Log.LOG_INFO, "Open new tab in browser");
            ((IJavaScriptExecutor)Driver).ExecuteScript("window.open()");
            List<String> tabs = new List<String>(Driver.WindowHandles);

            Log.Message(Log.LOG_INFO, "Switch to new opened tab");
            Driver.SwitchTo().Window(tabs.Last());

        }

        /// <summary>
        /// Close current tab which is displayed and switch to next tab
        /// if no tab is open do nothing
        /// </summary>
        /// <param name="Driver">Current webdriver instance</param>
        public static void CloseCurrentTab(this IWebDriver Driver)
        {
            Log.Message(Log.LOG_INFO, "Close current tab in browser");
            ((IJavaScriptExecutor)Driver).ExecuteScript("window.close()");

            List<String> tabs = new List<String>(Driver.WindowHandles);
            Log.Message(Log.LOG_INFO, "Try to switch to next tab automatically");

            Driver.SwitchTo().Window(Driver.WindowHandles.First());

        }

        /// <summary>
        /// Switch to next tab
        /// </summary>
        /// <param name="Driver">Current webdriver instance</param>
        public static void NextTab(this IWebDriver Driver)
        {
            IList<string> tabs = Driver.WindowHandles;
            string currentTab = Driver.CurrentWindowHandle;

            IEnumerator<string> enumerator = tabs.GetEnumerator();

            // If no tab is open, do nothing
            if (tabs.Count == 0)
            {
                Log.Message(Log.LOG_INFO, "No tab in browser is opened");
                return;
            }
            if (tabs.Count() == 1)
            {
                // Nothin should happen as there is no additional tab to switch to
                Log.Message(Log.LOG_INFO, "Only one tab is opened");
                return;
            }

            if (enumerator.MoveNext())
            {
                Driver.SwitchTo().Window(enumerator.Current);
            }
            // if enumerator.MoveNext() is false than we start at beginning of tabs list again
            else
            {
                Driver.SwitchTo().Window(tabs.First());
            }
        }

        /// <summary>
        /// Highlights given element with red border
        /// </summary>
        /// <param name="driver">Current webdriver instance</param>
        /// <param name="element">Element to be highlighted </param>
        public static void HighlightElement(this IWebDriver driver, IWebElement element)
        {
            var jsDriver = (IJavaScriptExecutor)driver;
            string highlightJavascript = @"arguments[0].style.cssText = ""border-width: 2px; border-style: solid; border-color: red"";";
            jsDriver.ExecuteScript(highlightJavascript, new object[] { element });
        }

        /// <summary>
        /// Navigate outsite of test environment and wait until page is loaded
        /// </summary>
        /// <param name="driver">Current webdriver instance</param>
        /// <param name="external_page">External page to navigate to</param>
        public static void NavigateToUrl(this IWebDriver driver, string external_page)
        {
            Log.Message(Log.LOG_INFO, "Navigate to page outside of test environment: " + external_page);
            driver.Url = external_page;
            WaitForPageToLoad(driver);
        }

        /// <summary>
        /// Navigate back to previous page and wait until page is reloaded
        /// </summary>
        /// <param name="driver">Current webdriver instance</param>
        public static void NavigateBack(this IWebDriver driver)
        {
            Log.Message(Log.LOG_INFO, "Navigate back to previous page");
            driver.Navigate().Back();
            WaitForPageToLoad(driver);
        }
        /// <summary>
        /// compares two different dictionaries 
        /// </summary>
        /// <param name="first_dictionary"></param>
        /// <param name="second_dictionary"></param>
        /// <returns></returns>
        public static bool CompareDictionaries(Dictionary<string, string> first_dictionary, Dictionary<string, string> second_dictionary)
        {
            bool result = true;

            foreach (var kvp in first_dictionary)
            {
                string key = kvp.Key;
                if (second_dictionary.Keys.Contains(key))
                {
                    Log.Message(Log.LOG_INFO, "Found same key " + key + " in both dictionaries.");
                    string value = second_dictionary[key];

                    if (value.Equals(kvp.Value))
                    {
                        Log.Message(Log.LOG_INFO, "Value for key " + key + " is same in both dictionaries. Value = " + value);
                    }
                    else
                    {
                        Log.Message(Log.LOG_WARNING, "Unequal values for key " + key);
                        Log.Message(Log.LOG_INFO, "first value = " + kvp.Value);
                        Log.Message(Log.LOG_INFO, "second value = " + value);
                        result = false;
                    }
                }
                else
                {
                    Log.Message(Log.LOG_WARNING, "Key "+key+" not found in second dictionary!");
                    result = false;
                }
            }
            return result;
        }

        public static bool IsVisibleInViewport(this IWebDriver driver, IWebElement element)
        {
            return (bool)((IJavaScriptExecutor)driver).ExecuteScript(
                    "var elem = arguments[0],                 " +
                    "  box = elem.getBoundingClientRect(),    " +
                    "  cx = box.left + box.width / 2,         " +
                    "  cy = box.top + box.height / 2,         " +
                    "  e = document.elementFromPoint(cx, cy); " +
                    "for (; e; e = e.parentElement) {         " +
                    "  if (e === elem)                        " +
                    "    return true;                         " +
                    "}                                        " +
                    "return false;                            "
                    , element);
        }

        public static bool IsElementOnFocus(this IWebDriver driver,IWebElement element)
        {
            IWebElement activeElemet = (IWebElement)driver.Scripts().ExecuteScript("return document.activeElement");
            if (element.Equals(activeElemet))
            {
                Log.Message(Log.LOG_INFO, "Given Element was focused");
                return true;
            }
            else
            {
                Log.Message(Log.LOG_ERROR, "Given Element wasn't focused");
                return false;
            }
        }
    }

}
