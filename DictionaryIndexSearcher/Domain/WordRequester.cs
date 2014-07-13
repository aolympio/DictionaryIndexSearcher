using DictionarySearcher.Utils;
using System.IO;
using System.Net;

namespace DictionaryIndexSearcher.Domain
{
    public class WordRequester
    {
        #region Variables
        private readonly static string DefaultDictionayUrl = "http://teste.way2.com.br/dic/api/words/";
        #endregion

        #region Properties
        public string UrlToBeRequested { get; private set; }
        #endregion
       

        public WordRequester()
        {
            this.UrlToBeRequested = 
                AppSettingConfigurationHandler.GetAppSettingConfiguration(
                "Way2DictionaryUrl", DefaultDictionayUrl); ;
        }

        public string GetResponseFromWay2Dictionary(int indexToBeRequested)
        {
            string wholeUrlToBeRequested = string.Format("{0}{1}", this.UrlToBeRequested, indexToBeRequested);
            using (WebResponse response = WebRequest.Create(wholeUrlToBeRequested).GetResponse())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    return response == null ? string.Empty : reader.ReadToEnd();
                }
            }
        }      
    }
}
