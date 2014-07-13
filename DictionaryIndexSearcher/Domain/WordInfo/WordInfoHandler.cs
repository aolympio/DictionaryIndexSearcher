using DictionarySearcher.Utils;
using Newtonsoft.Json;
using System.IO;
using System.Net;

namespace DictionaryIndexSearcher.Domain.WordInfo
{
    public class WordInfoHandler 
    {
        #region Variables
        private static int DefaultCalibrationRangeOfMaximumAndCurrrentIndexes = 1000; 
        #endregion

        #region Methods
        public static int ApplyIndexCalculatorFormule(int currentMinimumIndex, int currentMaximumIndex)
        {
            return currentMaximumIndex - ((currentMaximumIndex - currentMinimumIndex) / 2);
        }

        /// <summary>
        /// Decrease maximum index when Dictionary is samaller than ecpected.
        /// Increase maximum index when Dictionary is bigger than ecpected.
        /// </summary>
        /// <param name="wordInfoWrapper"></param>
        /// <param name="mustIncreaseMaximumIndex"></param>
        /// <param name="mustDecreaseMaximumIndex"></param>
        public static void CalibrateMaximumAndCurrrentIndexes(WordInfoWrapper wordInfoWrapper,
            bool mustIncreaseMaximumIndex = false, bool mustDecreaseMaximumIndex = false)
        {
            if (mustIncreaseMaximumIndex)
            {
                //Calibrating Maximum Index
                wordInfoWrapper.CurrentMaximumIndex = wordInfoWrapper.DefaultMaximumIndex =
                    wordInfoWrapper.CurrentMaximumIndex + CalibrationRangeOfMaximumAndCurrrentIndexes;
            }
            if (mustDecreaseMaximumIndex)
            {
                int previousIndex = wordInfoWrapper.CurrentRequestedIndex - 1;
                //Calibrating Maximum Index
                wordInfoWrapper.CurrentMaximumIndex = wordInfoWrapper.DefaultMaximumIndex = previousIndex;
            }
            //Calibrating Current Index
            wordInfoWrapper.CurrentRequestedIndex =
                ApplyIndexCalculatorFormule(wordInfoWrapper.CurrentMinimumIndex, wordInfoWrapper.CurrentMaximumIndex);
            //Updating index of desired word on Historic Of Searched Words
            wordInfoWrapper.HistoricOfSearchedWords.SearchedWords[wordInfoWrapper.DesiredWord] =
                wordInfoWrapper.CurrentRequestedIndex;
        }

        private static int CalibrationRangeOfMaximumAndCurrrentIndexes = GetCalibrationRangeOfMaximumAndCurrrentIndexes();

        #region Get App Setting
        private static int GetCalibrationRangeOfMaximumAndCurrrentIndexes()
        {
            if (!int.TryParse(AppSettingConfigurationHandler.GetAppSettingConfiguration(
                     "CalibrationRangeOfMaximumAndCurrrentIndexes",
                     DefaultCalibrationRangeOfMaximumAndCurrrentIndexes.ToString()),
                              out CalibrationRangeOfMaximumAndCurrrentIndexes))
                CalibrationRangeOfMaximumAndCurrrentIndexes = DefaultCalibrationRangeOfMaximumAndCurrrentIndexes;


            return CalibrationRangeOfMaximumAndCurrrentIndexes;
        }
        #endregion 
        #endregion        
    }
}
