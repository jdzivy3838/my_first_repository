using TechTalk.SpecFlow;
using OpenQA.Selenium;
using Framework;

namespace SpecFlowTemplateProject.Steps
{
    [Binding]
    public sealed class BrowserSteps
    {

        // For additional details on SpecFlow step definitions see https://go.specflow.org/doc-stepdef

        private readonly ScenarioContext _scenarioContext = ScenarioContext.Current;
        private IWebDriver Driver => _scenarioContext.Get<IWebDriver>("driver");
        private RunConfiguration Configuration => _scenarioContext.Get<RunConfiguration>("RunConfiguration");

        //private PageObjects.SiteCore.BenefitsResultsPage BenefitsResultPage;
        //private PageObjects.SiteCore.BenefitsDetailPage BenefitsDetailPage;

        [BeforeScenario]
        public void Before()
        {
            // BenefitsResultPage = null;
        }

        //public BrowserSteps()
        //{
        //    _scenarioContext = ScenarioContext.Current;
        //}

        [Given(@"I navigate to page '(.*)'")]
        public void GivenINavigateToPage(string uri_name)
        {
            Log.Message(Log.LOG_INFO, "Navigating to Benefits page");
            if (Driver == null)
            {
                IWebDriver NewDriver = Setup.InitDriver(Configuration);
                _scenarioContext["driver"] = NewDriver;
            }

            Driver.Navigate().GoToUrl(Setup.GetData(uri_name));

            //BenefitsPage BenefitsPage = new PageObjects.SiteCore.BenefitsPage(Driver);
            //BenefitsPage.clickOnAcceptCookie();
            //ScenarioContext["BenefitsPage"] = BenefitsPage;
            //Driver.WaitElementToBeClickable(BenefitsPage.GetTxtSearchInput());
            //Driver.WaitElementToBeClickable(BenefitsPage.GetBtnFinden());
            //Assert.IsTrue(BenefitsPage.VerifyIfPageLoaded(), "Benefits pages was not loaded");
        }
        [Given(@"I search company '(.*)'")]
        public void GivenISearchCompany(string p0)
        {
            // ScenarioContext.Current.Pending();
        }

    }
}
