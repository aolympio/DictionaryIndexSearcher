using DictionarySearcher.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Linq;

namespace DictionaryIndexSearcher.Domain.WordInfo
{
    public class WordInfoWrapper
    {
       

        #region Properties
        public int CurrentRequestedIndex { get; set; }
        public int CurrentMaximumIndex { get; set; }
        public int CurrentMinimumIndex { get; set; }
        public int DefaultMaximumIndex { get; set; }
        public int DefaultMinimumIndex { get; set; }
        public int DeadCats { get; set; }
        public string DesiredWord { get; set; }
        public HistoricOfSearchedWords HistoricOfSearchedWords { get; set; }
        public string RetrievedWord { get; set; }
        #endregion

        #region Constructor
        public WordInfoWrapper()
        {
            this.CurrentRequestedIndex = -1;
            this.CurrentMinimumIndex = this.DefaultMinimumIndex = GetDefaultMinimumIndex();
            this.CurrentMaximumIndex = this.DefaultMaximumIndex = GetDefaultMaximumIndex();
            this.HistoricOfSearchedWords = new HistoricOfSearchedWords().GetHistoricOfSearchedWords();
        } 
        #endregion

        #region Get App Settings

        private int GetDefaultMinimumIndex()
        {
            int defaultMinimumIndex;
            if (!int.TryParse(AppSettingConfigurationHandler.GetAppSettingConfiguration(
                    "DefaultMinimumIndex", "0"), out defaultMinimumIndex))
                defaultMinimumIndex = 0;

            return defaultMinimumIndex;
        }

        private int GetDefaultMaximumIndex()
        {
            int defaultMaximumIndex;
            if (!int.TryParse(AppSettingConfigurationHandler.GetAppSettingConfiguration(
                   "DefaultMaximumIndex", "50000"), out defaultMaximumIndex))
                defaultMaximumIndex = 50000;

            return defaultMaximumIndex;
        }

        #endregion

        #region Methods
        public WordInfoWrapper GetWordInfoWrapperUpdated()
        {
            bool isDesiredWordExistentOnHistoricOfSearchedWords = false;

            if (this.HistoricOfSearchedWords.SearchedWords != null &&
                this.HistoricOfSearchedWords.SearchedWords != null)
            {
                if (!this.HistoricOfSearchedWords.SearchedWords.ContainsKey(this.DesiredWord))
                {
                    this.HistoricOfSearchedWords.SearchedWords.Add(this.DesiredWord, -1);
                    int totalOfItems = this.HistoricOfSearchedWords.SearchedWords.Count;
                    int indexCreatedInSortedList =
                        this.HistoricOfSearchedWords.SearchedWords.IndexOfKey(this.DesiredWord);
                    int previousIndex = indexCreatedInSortedList - 1;
                    int nextIndex = indexCreatedInSortedList + 1;

                    //Case of mapped index is the lowest one of list
                    if (indexCreatedInSortedList.Equals(0))
                    {
                        CurrentMaximumIndex = totalOfItems > 1 ?
                            CurrentMaximumIndex = this.HistoricOfSearchedWords.SearchedWords
                                .ElementAt(nextIndex).Value : CurrentMaximumIndex;
                    }
                    //Case of mapped index is the highest one of list
                    else if (indexCreatedInSortedList.Equals(totalOfItems - 1))
                    {
                        CurrentMinimumIndex =
                            this.HistoricOfSearchedWords.SearchedWords.ElementAt(previousIndex).Value;
                    }
                    //Case of mapped index is the middle one of list (at least 3 items)
                    else
                    {
                        CurrentMinimumIndex =
                            this.HistoricOfSearchedWords.SearchedWords.ElementAt(previousIndex).Value;
                        CurrentMaximumIndex = totalOfItems > 2 ?
                            CurrentMaximumIndex = this.HistoricOfSearchedWords.SearchedWords
                                .ElementAt(nextIndex).Value : CurrentMaximumIndex;
                    }
                }
                else
                {
                    CurrentRequestedIndex = CurrentMaximumIndex = CurrentMinimumIndex =
                        this.HistoricOfSearchedWords.SearchedWords[this.DesiredWord];
                    isDesiredWordExistentOnHistoricOfSearchedWords = true;
                    //Do Request to confirm through this index the desired word is retrieved.                
                }
            }
            CurrentRequestedIndex = !isDesiredWordExistentOnHistoricOfSearchedWords ?
                WordInfoHandler.ApplyIndexCalculatorFormule(CurrentMinimumIndex, CurrentMaximumIndex) : CurrentRequestedIndex;

            //Update Word Info Wrapper
            this.HistoricOfSearchedWords.SearchedWords[this.DesiredWord] = CurrentRequestedIndex;

            return this;
        }        

        public void UpdateWordInfoWrapperPropertiesOnNewSearch()
        {
            this.CurrentMinimumIndex = this.DefaultMinimumIndex;
            this.CurrentMaximumIndex = this.DefaultMaximumIndex;
            this.DeadCats = 0;
            this.HistoricOfSearchedWords.LastUpdatedDate =
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff tt");
        } 
        #endregion
    }
}
