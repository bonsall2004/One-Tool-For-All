using Microsoft.Win32;
using Newtonsoft.Json;
using System.IO;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace OTFA
{
    public static class AssemblyExtensions
    {
        public static string GetFileLocation(string FileName)
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            string folderName = ".otfa";
            string folderPath = Path.Combine(appDataPath, folderName);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            return Path.Combine(folderPath, FileName);
        }

        public static string GetJSONfromAssembly()
        {
            try
            {
                if (File.Exists(GetFileLocation("TweakList.json"))) return File.ReadAllText(GetFileLocation("TweakList.json"));
                else
                {
                    var assembly = Assembly.GetEntryAssembly();
                    var resourceStream = assembly.GetManifestResourceStream("OTFA.TweakList.json");
                    using (var reader = new StreamReader(resourceStream))
                    {
                        var data = JsonConvert.DeserializeObject<Root>(reader.ReadToEnd());

                        foreach (var generalInfo in data.general.Values)
                        {
                            foreach (var item in generalInfo.getValues.scripts)
                            {
                                try
                                {
                                    var keyValue = Registry.GetValue(item.keyName, item.valueName, "null").ToString() != item.keyOnValue ? false : true;
                                    data.general[generalInfo.codeName].userSetting = keyValue;
                                    continue;
                                }
                                catch (Exception ex)
                                {
                                    continue;
                                    throw ex;
                                }
                            }
                        }

                        string updatedJson = JsonConvert.SerializeObject(data, Formatting.Indented);
                        File.WriteAllText(AssemblyExtensions.GetFileLocation("TweakList.json"), updatedJson);
                        return File.ReadAllText(GetFileLocation("TweakList.json"));
                    };
                }
            } catch
            (Exception e) {
                Errors.ShowException(e);
                Environment.Exit(1);
                return "";
            }

        }

        public static void GenerateFile(string FileName, string RawData)
        {
            Task.Run(() =>
            {
                string[] fileData = { RawData };

                File.WriteAllLines(GetFileLocation(FileName), fileData);
            });

        }
    }
}
