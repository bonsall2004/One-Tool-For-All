using Newtonsoft.Json;
using Microsoft.Win32;
using AdonisUI.Controls;
using System;
using System.IO;
using System.Windows.Documents;
using System.Collections.Generic;

namespace OTFA
{
    public static class UserSettings
    {
        public static List<string> failed = new List<string>();
        public static void Initialize()
        {
            try
            {
                string json = AssemblyExtensions.GetJSONfromAssembly();
                if (String.IsNullOrEmpty(json)) return;
                var data = JsonConvert.DeserializeObject<Root>(json);
                foreach (var generalInfo in data.general.Values)
                {
                    bool active = true;
                    foreach(var item in generalInfo.getValues.scripts)
                    {
                        try
                        {
                            var check = Run.StringToRegistryKey(item.keyLocation).GetValue(item.keyPath);
                            var keyValue = Registry.GetValue(item.keyName, item.valueName, "null").ToString() == item.keyOnValue ? true : false;
                            if (keyValue == false) active = keyValue;
                            continue;
                        }
                        catch (Exception e){
                            Errors.ShowOther($"{item.keyName}", "Tweak unavaliable");
                            failed.Add(generalInfo.codeName);
                            continue;
                            throw e;
                        }
                    }
                    UpdateAndSaveUserSetting(generalInfo.codeName, active);
                }

            }
            catch (Exception ex)
            {
                var messageBox = new MessageBoxModel
                {
                    Text = "An Error has Occurred",
                    Caption = ex.Message,
                    Icon = MessageBoxImage.Error,
                    Buttons = new[] {
                            MessageBoxButtons.Ok(),
                        }
                };
                MessageBox.Show(messageBox);
            }
        }

        public static void UpdateAndSaveUserSetting(string keyName, bool value)
        {
            try
            {
                string json = AssemblyExtensions.GetJSONfromAssembly();
                if (String.IsNullOrEmpty(json)) return;
                var data = JsonConvert.DeserializeObject<Root>(json);

                // Check if the key exists in the general section

                if (data.general.ContainsKey(keyName))
                {
                    data.general[keyName].userSetting = value;
                }
                else
                {
                    return;
                }

                if (!File.Exists(AssemblyExtensions.GetFileLocation("TweakList.json")))
                {
                    AssemblyExtensions.GenerateFile("TweakList.json", AssemblyExtensions.GetJSONfromAssembly());
                }
                else
                {
                    string updatedJson = JsonConvert.SerializeObject(data, Formatting.Indented);
                    File.WriteAllText(AssemblyExtensions.GetFileLocation("TweakList.json"), updatedJson);
                }
            }
            catch (Exception ex)
            {
                var messageBox = new MessageBoxModel
                {
                    Text = "An Error has Occurred",
                    Caption = ex.Message,
                    Icon = MessageBoxImage.Error,
                    Buttons = new[] {
                            MessageBoxButtons.Ok(),
                        }
                };
                MessageBox.Show(messageBox);
            }


        }
    }

}