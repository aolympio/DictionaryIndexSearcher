using System;
using System.IO;

namespace DictionarySearcher.Utils
{
    public class FileHandler
    {
        public static void WriteIntoFile(string filePath, string fileContent)
        {
            try
            {
                File.WriteAllText(filePath, fileContent);
            }
            catch
            {
                Console.WriteLine("WARNING: Historic of Searched Words will not be available for now but you can proceed with Word Index search normally.");
            }
        }

        public static string ReadFromFile(string filePath)
        {
            try
            {
                if (!File.Exists(filePath)) File.Create(filePath);
                return File.ReadAllText(filePath);
            }
            catch
            {
                Console.WriteLine("WARNING: Historic of Searched Words will not be available for now but you can proceed with Word Index search normally.");
                return string.Empty; //Historic of Searched Words is unacessible so let's proceed.
            }
        }      
    }
}
