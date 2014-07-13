using DictionarySearcher.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace DictionaryIndexSearcher.Domain.WordInfo
{
    public class HistoricOfSearchedWords
    {
        #region Variables
        private readonly string DefaultHistoricOfSearchedWordsFilePath =
           @"C:\Anderson\Projects\Way2\DictionaryIndexSearcher\Files\HistoricOfSearchedWords_Test.json";
        #endregion

        #region Properties
        public string LastUpdatedDate { get; set; }
        public bool MustSaveAndSearchInHistoricOfSearchedWordsFile { get; private set; }
        public SortedList<string, int> SearchedWords { get; set; }
        #endregion

        #region Constructor
        public HistoricOfSearchedWords()
        {
            this.LastUpdatedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff tt");
            this.SearchedWords = new SortedList<string, int>();
            this.MustSaveAndSearchInHistoricOfSearchedWordsFile = GetMustSaveAndSearchInHistoricOfSearchedWordsFile();
        }
        #endregion

        #region Methods
        public HistoricOfSearchedWords GetHistoricOfSearchedWords()
        {
            if (!this.MustSaveAndSearchInHistoricOfSearchedWordsFile) return this;
            
            var historicOfSearchedWordsFilePathDeserialized =
                JsonConvert.DeserializeObject<HistoricOfSearchedWords>(
                    FileHandler.ReadFromFile(GetHistoricOfSearchedWordsFilePath()));

            return historicOfSearchedWordsFilePathDeserialized != null ?
                historicOfSearchedWordsFilePathDeserialized : new HistoricOfSearchedWords();
        }

        public void SaveHistoricOfSearchedWordsInAJsonFile()
        {
            string jsonContent = JsonConvert.SerializeObject(
                this, new KeyValuePairConverter());

            FileHandler.WriteIntoFile(GetHistoricOfSearchedWordsFilePath(), jsonContent);
        }

        #region Get App Settings
        private string GetHistoricOfSearchedWordsFilePath()
        {
            return AppSettingConfigurationHandler.GetAppSettingConfiguration(
                "HistoricOfSearchedWordsFilePath", DefaultHistoricOfSearchedWordsFilePath);
        }

        private bool GetMustSaveAndSearchInHistoricOfSearchedWordsFile()
        {
            bool mustSaveAndSearchInHistoricOfSearchedWordsFile;
            if (!bool.TryParse(AppSettingConfigurationHandler.GetAppSettingConfiguration(
                    "MustSaveAndSearchInHistoricOfSearchedWordsFile", "False"),
                     out mustSaveAndSearchInHistoricOfSearchedWordsFile))
                mustSaveAndSearchInHistoricOfSearchedWordsFile = false;

            return mustSaveAndSearchInHistoricOfSearchedWordsFile;
        }
        #endregion
        #endregion
    }
}