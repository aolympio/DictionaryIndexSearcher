using DictionaryIndexSearcher.Domain;
using DictionaryIndexSearcher.Domain.WordInfo;
using DictionarySearcher.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NUnit.Framework;
using System.IO;

namespace DicitionaryIndexSearcherTest
{
    [TestFixture]
    public class DicitionaryIndexSearcherTest
    {
        private string HistoricOfSearchedWordsFilePath;
        private readonly string DefaultHistoricOfSearchedWordsFilePath = @"C:\Anderson\Projects\Way2\DictionaryIndexSearcher\Files\HistoricOfSearchedWords_Test.json";
        private string Way2DictionaryUrl;
        private readonly static string DefaultDictionayUrl = "http://teste.way2.com.br/dic/api/words/";
        private HistoricOfSearchedWords GlobalHistoricOfSearchedWords;
        private DictionaryIndexSearcher.Domain.DictionaryIndexSearcher DictionaryIndexSearcher;

        [TestFixtureSetUp]
        public void Initialize()
        {
            //App Settings Configurations
            Way2DictionaryUrl = AppSettingConfigurationHandler.GetAppSettingConfiguration("Way2DictionaryUrl", DefaultDictionayUrl);
            HistoricOfSearchedWordsFilePath =
                 AppSettingConfigurationHandler.GetAppSettingConfiguration("HistoricOfSearchedWordsFilePath", DefaultHistoricOfSearchedWordsFilePath);

            //Initiling Global Variables
            DictionaryIndexSearcher = new DictionaryIndexSearcher.Domain.DictionaryIndexSearcher(string.Empty);
            GlobalHistoricOfSearchedWords = this.GetHistoricOfSerachedWordsStub();
        }

        /// <summary>
        /// Save request made on Dictionary into historic of searched words file.
        /// </summary>
        [Test]
        public void A_SaveRequestMadeOnHistoricOfSearchedWordsFile()
        {
            //Arrange
            string wordName = "HAVIDA";

            //Act
            //Save word info into JSON file
            string json = JsonConvert.SerializeObject(GlobalHistoricOfSearchedWords, new KeyValuePairConverter());
            FileHandler.WriteIntoFile(HistoricOfSearchedWordsFilePath, json);

            //Assert	
            var historicOfSearchedWordsRetrieved =
                JsonConvert.DeserializeObject<HistoricOfSearchedWords>(FileHandler.ReadFromFile(HistoricOfSearchedWordsFilePath));
            GlobalHistoricOfSearchedWords = historicOfSearchedWordsRetrieved;
            Assert.IsTrue(historicOfSearchedWordsRetrieved.SearchedWords.ContainsKey(wordName));//Query word info in JSON file then confirm it was found into
        }

        /// <summary>
        /// Have to read JSON File prior to search desirable word on words previously registered
        /// </summary>
        [Test]
        public void B_SearchDesirableWordInPreviousWordsRegistered()
        {
            //Arrange
            string wordName = "ALENTO";
            bool hasWordIntoHistoricOfSearchedWords = false;
            int indexRetrieved = -1;

            //Act
            if (GlobalHistoricOfSearchedWords.SearchedWords.ContainsKey(wordName))
            {
                indexRetrieved = GlobalHistoricOfSearchedWords.SearchedWords[wordName];
                hasWordIntoHistoricOfSearchedWords = true;
            }

            //Assert
            Assert.IsTrue(hasWordIntoHistoricOfSearchedWords);
            Assert.IsTrue(indexRetrieved >= 0);
        }

        /// <summary>
        /// Update historic of  Searched words dictionary structure if the word index has changed
        ///  and in the end of process (index searched found) update JSON file.
        /// </summary>
        [Test]
        public void C_UpdateRequestWordIndex()
        {
            //Arrange
            //Load JSON file to dictionary structure
            string wordAlentoName = "ALENTO";
            int wordAlentoIndex = 2171;//Originally 2170
            string wordAbelName = "ABEL";
            int wordAbelIndex = 86;//Originally 85

            var historicOfSearchedWordsUpdated = this.GetHistoricOfSerachedWordsStub();

            //Act
            //Update Dictionary structure
            //Save Dictionary in JSON file
            if (historicOfSearchedWordsUpdated.SearchedWords.ContainsKey(wordAlentoName))
                historicOfSearchedWordsUpdated.SearchedWords[wordAlentoName] = wordAlentoIndex;
            if (historicOfSearchedWordsUpdated.SearchedWords.ContainsKey(wordAbelName))
                historicOfSearchedWordsUpdated.SearchedWords[wordAbelName] = wordAbelIndex;

            //Assert	
            Assert.IsTrue(historicOfSearchedWordsUpdated.SearchedWords[wordAlentoName].Equals(2171));////Query word info in JSON file then confirm ALENTO 1 was found into
            Assert.IsTrue(historicOfSearchedWordsUpdated.SearchedWords[wordAbelName].Equals(86));////Query word info in JSON file then confirm ABEL 86 was found into 
        }

        [Test]
        public void D_PerformSuccededHttpCallToWay2Dicitonary()
        {
            //Arrange
            int requestedIndex = 1000;

            //Act
            string retrievedWord = new WordRequester().GetResponseFromWay2Dictionary(requestedIndex);

            //Assert
            Assert.IsNotNullOrEmpty(retrievedWord);
        }

        [Test]
        public void E_RetrieveCurrentIndexToBeRequestedBasedOnMinAndMaxIndexes()
        {
            //Arrange
            string desiredWord = "ADORMECIDO";
            var wordInfoWrapper = new WordInfoWrapper { DesiredWord = desiredWord };

            //Act
            wordInfoWrapper = wordInfoWrapper.GetWordInfoWrapperUpdated();

            //Assert
            Assert.AreEqual(1128, wordInfoWrapper.CurrentRequestedIndex);
        }

        [Test]
        public void F_RetrieveMaxAndMinIndexesCloseEnoughAGivenPointWhenIndexCreatedInListIsTheLowestOne()
        {
            //Arrange
            string desiredWord = "AARÃO";//lowest
            var wordInfoWrapper = new WordInfoWrapper { DesiredWord = desiredWord };

            //Act
            wordInfoWrapper = wordInfoWrapper.GetWordInfoWrapperUpdated();

            //Assert
            Assert.AreEqual(0, wordInfoWrapper.CurrentMinimumIndex);
            Assert.AreEqual(85, wordInfoWrapper.CurrentMaximumIndex);
        }

        [Test]
        public void G_RetrieveMaxAndMinIndexesCloseEnoughAGivenPointWhenIndexCreatedInListIsTheHighestOne()
        {
            //Arrange
            string desiredWord = "SINTOMA";//highest
            var wordInfoWrapper = new WordInfoWrapper { DesiredWord = desiredWord };

            //Act
            wordInfoWrapper = wordInfoWrapper.GetWordInfoWrapperUpdated();

            //Assert
            Assert.AreEqual(22912, wordInfoWrapper.CurrentMinimumIndex);
            Assert.AreEqual(50000, wordInfoWrapper.CurrentMaximumIndex);
        }

        [Test]
        public void H_RetrieveMaxAndMinIndexesCloseEnoughAGivenPointWhenIndexCreatedInListIsInTheMiddleOfList()
        {
            //Arrange
            string desiredWord = "ADORMECIDO";//middle
            var wordInfoWrapper = new WordInfoWrapper { DesiredWord = desiredWord };

            //Act
            wordInfoWrapper = wordInfoWrapper.GetWordInfoWrapperUpdated();

            //Assert
            Assert.AreEqual(85, wordInfoWrapper.CurrentMinimumIndex);
            Assert.AreEqual(2170, wordInfoWrapper.CurrentMaximumIndex);
        }

        [Test]
        public void I_RetrieveMaxAndMinIndexesCloseEnoughAGivenPointWhenIndexCreatedInListIsEquealToTheGivenPoint()
        {
            //Arrange
            string desiredWord = "ABEL";//equal to given one
            var wordInfoWrapper = new WordInfoWrapper { DesiredWord = desiredWord };

            //Act
            wordInfoWrapper = wordInfoWrapper.GetWordInfoWrapperUpdated();

            //Assert
            Assert.AreEqual(85, wordInfoWrapper.CurrentMinimumIndex);
            Assert.AreEqual(85, wordInfoWrapper.CurrentMaximumIndex);
            Assert.AreEqual(85, wordInfoWrapper.CurrentRequestedIndex);
        }

        [Test]
        public void J_GetIndexOfWordEstalactite()
        {
            //Arrange
            string desiredWord = "ESTALACTITE";
            var wordInfoWrapper = new WordInfoWrapper { DesiredWord = desiredWord };
                        
            //Act
            wordInfoWrapper = DictionaryIndexSearcher.GetDesiredWordIndex(wordInfoWrapper);

            //Assert
            Assert.AreEqual(desiredWord, wordInfoWrapper.DesiredWord);
            Assert.AreEqual(19000, wordInfoWrapper.CurrentRequestedIndex);
        }

        [Test]
        public void K_SaveHistoricOfSearchedWordsInJsonFileRightAfterIndexIsFound()
        {
            //Arrange
            string desiredWord = "ESTALACTITE";
            var wordInfoWrapper = new WordInfoWrapper 
                                { 
                                    DesiredWord = desiredWord, 
                                    HistoricOfSearchedWords = GetHistoricOfSerachedWordsStub() 
                                };            
            
            wordInfoWrapper = DictionaryIndexSearcher.GetDesiredWordIndex(wordInfoWrapper);

            //Act
            string jsonContent = JsonConvert.SerializeObject(
                wordInfoWrapper.HistoricOfSearchedWords, new KeyValuePairConverter());
            FileHandler.WriteIntoFile(HistoricOfSearchedWordsFilePath, jsonContent);

            //Assert	
            var historicOfSearchedWordsRetrieved =
                JsonConvert.DeserializeObject<HistoricOfSearchedWords>(
                    FileHandler.ReadFromFile(HistoricOfSearchedWordsFilePath));
            
            //Assert
            Assert.IsTrue(historicOfSearchedWordsRetrieved.SearchedWords.ContainsKey(desiredWord));
            Assert.AreEqual(19000, historicOfSearchedWordsRetrieved.SearchedWords[desiredWord]);
        }

        [Test]
        public void L_DoNotGetIndexOfNonExistentWord()
        {
            //Arrange
            string desiredWord = "OLYMPIO";
            var wordInfoWrapper = new WordInfoWrapper { DesiredWord = desiredWord };

            //Act
            wordInfoWrapper = DictionaryIndexSearcher.GetDesiredWordIndex(wordInfoWrapper);

            //Assert
            Assert.IsFalse(wordInfoWrapper.HistoricOfSearchedWords.SearchedWords.ContainsKey(desiredWord));
            Assert.AreEqual(-1, wordInfoWrapper.CurrentRequestedIndex);
        }

        [Test]
        public void M_GetIndexOfWordEvoluaWhenHistoricOfSearchedWordsListIsCleared()
        {
            //Arrange
            string desiredWord = "EVOLUA";
            var wordInfoWrapper = new WordInfoWrapper
            {
                DesiredWord = desiredWord,
                HistoricOfSearchedWords = new HistoricOfSearchedWords()//cleared
            };

            //Act
            wordInfoWrapper = DictionaryIndexSearcher.GetDesiredWordIndex(wordInfoWrapper);

            //Assert
            Assert.AreEqual(desiredWord, wordInfoWrapper.DesiredWord);
            Assert.AreEqual(19562, wordInfoWrapper.CurrentRequestedIndex);
        }

        #region Private methods

        private HistoricOfSearchedWords GetHistoricOfSerachedWordsStub()
        {
            HistoricOfSearchedWords historicOfSearchedWords = new HistoricOfSearchedWords();

            if (!historicOfSearchedWords.SearchedWords.ContainsKey("HAVIDA"))
                historicOfSearchedWords.SearchedWords.Add("HAVIDA", 22912);
            if (!historicOfSearchedWords.SearchedWords.ContainsKey("ALENTO"))
                historicOfSearchedWords.SearchedWords.Add("ALENTO", 2170);
            if (!historicOfSearchedWords.SearchedWords.ContainsKey("ABEL"))
                historicOfSearchedWords.SearchedWords.Add("ABEL", 85);

            return historicOfSearchedWords;
        }      
       
        #endregion

        [TestFixtureTearDown]
        public void Dipose()
        {
            File.Delete(DefaultHistoricOfSearchedWordsFilePath);
        }
    }
}