using DictionaryIndexSearcher.Domain.WordInfo;
using DictionarySearcher.Utils;
using System;
using System.Net;

namespace DictionaryIndexSearcher.Domain
{
    public class DictionaryIndexSearcher
    {
        #region Properties
        public string DesiredWord { get; private set; }
        #endregion

        #region Constructor
        public DictionaryIndexSearcher(string desiredWord)
        {
            this.DesiredWord = desiredWord;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Method responsible for retrieve all info regarding desired word.
        /// </summary>
        /// <param name="wordInfoWrapper"></param>
        /// <returns></returns>
        public WordInfoWrapper GetDesiredWordIndex(WordInfoWrapper wordInfoWrapper)
        {
            bool retryResponse, maximumIndexReached, isRetrievedDesiredWord = false;

            try
            {
                wordInfoWrapper = wordInfoWrapper.GetWordInfoWrapperUpdated();

                do
                {
                    try
                    {
                        wordInfoWrapper.RetrievedWord =
                            new WordRequester().GetResponseFromWay2Dictionary(wordInfoWrapper.CurrentRequestedIndex);

                        //Check if desired word was retireved.
                        isRetrievedDesiredWord = wordInfoWrapper.RetrievedWord.Equals(wordInfoWrapper.DesiredWord);

                        HandleMaximumIndex(wordInfoWrapper, isRetrievedDesiredWord, out retryResponse, out maximumIndexReached);
                    }
                    catch (WebException)
                    {
                        WordInfoHandler.CalibrateMaximumAndCurrrentIndexes(wordInfoWrapper, mustDecreaseMaximumIndex: true);
                        retryResponse = true;
                    }
                    finally
                    {
                        wordInfoWrapper.DeadCats++;
                    }
                } while (retryResponse);

                return !isRetrievedDesiredWord ?
                    HandleCaseWhichDesiredWordWasNotRetrievedYet(wordInfoWrapper) : wordInfoWrapper;
            }
            catch 
            {
                Console.WriteLine("ERROR: Search failed. Please try a new one.");
                return wordInfoWrapper;
            }
        }

        private WordInfoWrapper HandleCaseWhichDesiredWordWasNotRetrievedYet(WordInfoWrapper wordInfoWrapper)
        {
            wordInfoWrapper.HistoricOfSearchedWords.SearchedWords.Remove(wordInfoWrapper.DesiredWord);

            if (!wordInfoWrapper.HistoricOfSearchedWords.SearchedWords.ContainsKey(wordInfoWrapper.RetrievedWord))
                wordInfoWrapper.HistoricOfSearchedWords.SearchedWords.Add(
                    wordInfoWrapper.RetrievedWord, wordInfoWrapper.CurrentRequestedIndex);
            else
            {
                wordInfoWrapper.HistoricOfSearchedWords.SearchedWords[wordInfoWrapper.RetrievedWord] =
                    wordInfoWrapper.CurrentRequestedIndex;
            }

            if (CheckIfWasLastRequestTry(wordInfoWrapper))
            {
                wordInfoWrapper.CurrentRequestedIndex = -1;
                return wordInfoWrapper;
            }

            return GetDesiredWordIndex(wordInfoWrapper);
        }

        private void HandleMaximumIndex(WordInfoWrapper wordInfoWrapper, bool isRetrievedDesiredWord,
            out bool retryResponse, out bool maximumIndexReached)
        {
            //Check if maximum index limit was reached.
            maximumIndexReached = wordInfoWrapper.CurrentRequestedIndex.Equals(wordInfoWrapper.CurrentMaximumIndex);
            //Check if it is necessary increave maximum index limit.
            retryResponse = ConfirmMaximumIndexIncreasing
                (maximumIndexReached, isRetrievedDesiredWord, wordInfoWrapper);
        }

        /// <summary>
        /// Confirm if it is necessary increse the maximum index limit.
        /// In positive case, it is necessary perform another request with updated index.
        /// </summary>
        /// <param name="maximumIndexReached"></param>
        /// <param name="isRetrievedWordTheDesiredOne"></param>
        /// <param name="wordInfoWrapper"></param>
        /// <returns></returns>
        private bool ConfirmMaximumIndexIncreasing(bool maximumIndexReached, bool isRetrievedWordTheDesiredOne,
            WordInfoWrapper wordInfoWrapper)
        {
            if (maximumIndexReached && !isRetrievedWordTheDesiredOne)
            {
                WordInfoHandler.CalibrateMaximumAndCurrrentIndexes(wordInfoWrapper, mustIncreaseMaximumIndex: true);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Last Request Try is considered when the range from Current Index to 
        /// Maximum and Minimum is 1. E.g.: index = 2, minimum=0, maximum = 1
        /// </summary>
        /// <param name="wordInfoWrapper"></param>
        /// <returns></returns>
        private bool CheckIfWasLastRequestTry(WordInfoWrapper wordInfoWrapper)
        {
            int previousIndex = wordInfoWrapper.CurrentRequestedIndex - 1;
            int nextIndex = wordInfoWrapper.CurrentRequestedIndex + 1;

            //If current index not exist to desired word, has previous and next indexes already requested,
            //it means this word does not exists in Dictionay
            if (wordInfoWrapper.CurrentMinimumIndex.Equals(previousIndex) &&
                   wordInfoWrapper.CurrentMaximumIndex.Equals(nextIndex))
            {
                return wordInfoWrapper.HistoricOfSearchedWords.SearchedWords.ContainsValue(previousIndex) &&
                       wordInfoWrapper.HistoricOfSearchedWords.SearchedWords.ContainsValue(nextIndex);
            }
            return false;
        }
        #endregion
    }
}
