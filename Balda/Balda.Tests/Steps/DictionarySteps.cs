using System.Linq;
using Balda.WebApi;
using TechTalk.SpecFlow;
using Xunit;
using Xunit.Abstractions;

namespace Balda.Tests.Steps
{
    [Binding]
    public class DictionarySteps
    {
        private readonly ITestOutputHelper _output;
        private readonly WordDictionary _dictionary = new ();
        private bool _contains;
        private int _length;
        private string _word = string.Empty;

        public DictionarySteps(ITestOutputHelper output)
        {
            _output = output;
        }
        
        [Given("the dictionary has the following words")]
        public void SetUpDictionary(Table table)
        {
            _dictionary.Words = table.Rows.Select(r => r[0]);
        }

        [When(@"I check the word ""(.+)""")]
        public void CheckTheWord(string word) => _contains = _dictionary.Contains(word);

        [When(@"I pick a (.+) characters long random word")]
        public void PickRandomWord(int length)
        {
            _length = length;
            _word = _dictionary.PickRandomWord(length);
        }

        [Then(@"the dictionary should contain it")]
        public void AssertContains() => Assert.True(_contains);

        [Then(@"the dictionary should not contain it")]
        public void AssertDoNotContain() => Assert.False(_contains);

        [Then(@"it should be one of the words in the list ""(.*)""")]
        public void AssertPickedCorrectWord(string words)
        {
            if (words.Length == 0)
            {
                Assert.Equal(0, _word.Length);
                return;
            }
            
            Assert.Equal(_length, _word.Length);
            Assert.Contains(words.Split(", "), w => w == _word);
        }
    }
}