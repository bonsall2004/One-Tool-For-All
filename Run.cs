using Newtonsoft.Json;
using System.Diagnostics;
using Microsoft.Win32;
using System.Collections.Generic;
using System;

namespace OTFA
{
    public static class Run
    {
        private static void DeleteRegistryValue(string keyLocation, string keyName, string valueName)
        {
            try
            {
                RegistryKey key = StringToRegistryKey(keyLocation);

                if (key != null)
                {
                    using (RegistryKey subKey = key.OpenSubKey(keyName, true))
                    {
                        if (subKey != null)
                        {
                            subKey.DeleteValue(valueName);
                            Console.WriteLine("Registry value deleted successfully.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Errors.ShowException(ex);
                return;
            }
        }

        private static RegistryValueKind GetKind(string valueKind)
        {
            switch(valueKind.ToLower())
            {
                case "dword":
                    return RegistryValueKind.DWord;
                case "string":
                    return RegistryValueKind.String;
                case "qword":
                    return RegistryValueKind.QWord;
                case "binary":
                    return RegistryValueKind.Binary;
                case "multistring":
                    return RegistryValueKind.MultiString;
                default:
                    return RegistryValueKind.None;

            }
        }

        public static RegistryKey StringToRegistryKey(string keyLocation)
        {
            RegistryKey key = null;
            switch (keyLocation)
            {
                case "LocalMachine":
                    key = Registry.LocalMachine;
                    break;
                case "CurrentUser":
                    key = Registry.CurrentUser;
                    break;
                case "Users":
                    key = Registry.Users;
                    break;
                case "CurrentConfig":
                    key = Registry.CurrentConfig;
                    break;
                case "ClassesRoot":
                    key = Registry.ClassesRoot;
                    break;
            }

            return key;
        }

        private static void SetRegistryValue(string keyLocation = "LocalMachine", string keyName = "", string valueName = "", string keyValue = "", string keyType = "dword")
        {
            try
            {
                RegistryKey key = StringToRegistryKey(keyLocation);

                if (key != null)
                {
                    try
                    {
                        using (RegistryKey subKey = key.OpenSubKey(keyName, true))
                        {
                            if (subKey != null)
                            {
                                subKey.SetValue(valueName, keyValue, GetKind(keyType));
                            }
                        }
                    } catch {
                        Errors.ShowOther("There was an error, try running as administrator", "Error");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Errors.ShowException(ex);
                return;
            }
        }

        public static void ToggleTweak(bool selected, string codeName)
        {
            try
            {
                string json = AssemblyExtensions.GetJSONfromAssembly();
                var data = JsonConvert.DeserializeObject<Root>(json);
                data.general.TryGetValue(codeName, out var tweak);

                foreach (var item in tweak.getValues.scripts)
                {
                    try
                    {
                        if (selected == true)
                        {
                            if (item.keyOnValue == "null")
                            {
                                DeleteRegistryValue(item.keyLocation, item.keyPath, item.valueName);
                                UserSettings.UpdateAndSaveUserSetting(tweak.codeName, true);
                                continue;
                            }
                            else
                            {
                                SetRegistryValue(item.keyLocation, item.keyPath, item.valueName, item.keyOnValue, item.keyType);
                                UserSettings.UpdateAndSaveUserSetting(tweak.codeName, true);
                                continue;
                            }
                        }
                        else
                        {
                            if (item.keyOffValue == "null")
                            {
                                DeleteRegistryValue(item.keyLocation, item.keyPath, item.valueName);
                                UserSettings.UpdateAndSaveUserSetting(tweak.codeName, false);
                                continue;
                            }
                            else
                            {
                                SetRegistryValue(item.keyLocation, item.keyPath, item.valueName, item.keyOffValue, item.keyType);
                                UserSettings.UpdateAndSaveUserSetting(tweak.codeName, false);
                                continue;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Errors.ShowException(e);
                        throw e;
                    }
                }
            }
            catch (Exception ex)
            {
                Errors.ShowException(ex);
                return;
            }
        }

        public static bool ExecuteCommand(string command)
        {
            try
            {
                ProcessStartInfo procStartInfo = new ProcessStartInfo("cmd", "/user:adminUser /c " + command)
                {
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process proc = new Process())
                {
                    proc.StartInfo = procStartInfo;
                    proc.Start();

                    string output = proc.StandardOutput.ReadToEnd();

                    if (output.ToLower().Contains("error"))
                    {
                        Errors.ShowOther("An Error has Occurred", "An error has occurred, try running as administrator.");
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Errors.ShowException(ex);
                return false;
            }
        }
    }

    public class CommandScheduler<T>
    {
        private HashSet<T> CommandsScheduled;
        public CommandScheduler()
        {
            CommandsScheduled = new HashSet<T>();
        }
        public void Add(T item)
        {
            CommandsScheduled.Add(item);
        }
        public void Remove(T item)
        {
            CommandsScheduled.Remove(item);
        }
        public void ExecuteScheduledCommands()
        {
            try
            {
                if (CommandsScheduled.Count == 0) return;
                foreach (var command in CommandsScheduled)
                {
                    Run.ExecuteCommand(command.ToString());
                }
            }
            catch (Exception ex)
            {
                Errors.ShowException(ex);
                return;
            }
        }
    }
}

