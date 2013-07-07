using TechTalk.SpecFlow;

namespace CQRS.AcceptanceTests.StepDefinitions
{
    [Binding]
    public class ElectionTerminationSteps
    {
        [Given(@"I have an existing election")]
        public void GivenIHaveAnExistingElection()
        {
            ScenarioContext.Current.Pending();
        }

        [When(@"I send a terminate election command")]
        public void WhenISendATerminateElectionCommand()
        {
            ScenarioContext.Current.Pending();
        }

        [Then(@"the election terminated event should be raised")]
        public void ThenTheElectionTerminatedEventShouldBeRaised()
        {
            ScenarioContext.Current.Pending();
        }
    }
}
