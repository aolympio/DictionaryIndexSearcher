using DictionaryIndexSearcher.Domain.WordInfo;
using System;

namespace DictionaryIndexSearcher
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("*****WAY2 DICTIONARY INDEX SEARCHER*****");
            Console.WriteLine();

            bool isWordSearchingEnabled = true;
            var wordInfoWrapper = new WordInfoWrapper();

            while (isWordSearchingEnabled)
            {
                Console.Write("Type desired word in order to retrieve its index: ");
                string desiredWord = Console.ReadLine().ToUpper().Trim();

                Console.WriteLine(string.Format("Searching index of {0}...", desiredWord));

                //Updated each every serach
                wordInfoWrapper.DesiredWord = desiredWord;
                wordInfoWrapper.UpdateWordInfoWrapperPropertiesOnNewSearch();

                wordInfoWrapper =
                    new DictionaryIndexSearcher.Domain
                        .DictionaryIndexSearcher(desiredWord)
                            .GetDesiredWordIndex(wordInfoWrapper);

                if (wordInfoWrapper.HistoricOfSearchedWords.MustSaveAndSearchInHistoricOfSearchedWordsFile)
                    wordInfoWrapper.HistoricOfSearchedWords.SaveHistoricOfSearchedWordsInAJsonFile();

                Console.WriteLine(wordInfoWrapper.CurrentRequestedIndex >= 0 ?
                             string.Format("- Index of {0} on dictionary: {1}",
                                desiredWord, wordInfoWrapper.CurrentRequestedIndex) :
                             string.Format("- {0} has no entry on dictionary.", desiredWord));
                Console.WriteLine(string.Format("- Number of Dead Cats: {0}", wordInfoWrapper.DeadCats));

                isWordSearchingEnabled = DoSearchRetryQuestion(isWordSearchingEnabled);

            }
            Console.WriteLine();
            Console.WriteLine("Press Enter to quit.");
            Console.ReadLine();
        }

        private static bool DoSearchRetryQuestion(bool isWordSearchingEnabled)
        {
            //Do question offering search for other word.
            bool retryQuestion = false;
            do
            {
                Console.WriteLine();
                Console.Write("Do you wish to search another word index (Y/N)? ");

                switch (Console.ReadLine().ToUpper().Trim())
                {
                    case "Y":
                    case "YES":
                        isWordSearchingEnabled = true;
                        retryQuestion = false;
                        break;
                    case "N":
                    case "NO":
                        isWordSearchingEnabled = retryQuestion = false;
                        break;
                    default:
                        Console.WriteLine("Unavailable answer. Please chosse Y(Yes) or N(No) as option.");
                        retryQuestion = true;
                        break;
                }
            } while (retryQuestion);
            return isWordSearchingEnabled;
        }
    }
}