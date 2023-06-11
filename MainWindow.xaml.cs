using Newtonsoft.Json;
using System;
using System.IO;
using System.Collections.Generic;
using System.Windows;
using System.Data;
using AdonisUI.Controls;
using MessageBox = AdonisUI.Controls.MessageBox;
using MessageBoxImage = AdonisUI.Controls.MessageBoxImage;
using System.Reflection.PortableExecutable;
using System.Linq;
using MessageBoxResult = AdonisUI.Controls.MessageBoxResult;
using NetDiscordRpc;
using System.ComponentModel;
using System.Collections.ObjectModel;
using OTFA.Utils;
using OTFA.DataTypes;

namespace OTFA
{
    public class RowItems : INotifyPropertyChanged
    {
        private bool enabled;
        public bool Enabled
        {
            get { return enabled; }
            set
            {
                if (enabled != value)
                {
                    enabled = value;
                    OnPropertyChanged(nameof(Enabled));
                }
            }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        private string description;
        public string Description
        {
            get { return description; }
            set
            {
                if (description != value)
                {
                    description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }

        private string safe;
        public string Safe
        {
            get { return safe; }
            set
            {
                if (safe != value)
                {
                    safe = value;
                    OnPropertyChanged(nameof(Safe));
                }
            }
        }

        private string codeName;
        public string CodeName
        {
            get { return codeName; }
            set
            {
                if (codeName != value)
                {
                    codeName = value;
                    OnPropertyChanged(nameof(CodeName));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<RowItems> rowItems;
        public ObservableCollection<RowItems> RowItems
        {
            get { return rowItems; }
            set
            {
                if (rowItems != value)
                {
                    rowItems = value;
                    OnPropertyChanged(nameof(RowItems));
                }
            }
        }

        public ObservableCollection<RowItems> SortedRowItems
        {
            get { return new ObservableCollection<RowItems>(rowItems?.OrderBy(rowItem => rowItem.Name)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public partial class MainWindow : AdonisWindow
    {
        public static string currentCategory;
        public MainWindow()
        {
            UserSettings.Initialize();
            InitializeComponent();
            RPCDiscord.Init();
            this.DataContext = new ViewModel {RowItems = GetRowItems("general")};

        }

        private void FileExplorerClick(object sender, RoutedEventArgs e)
        {
            this.DataContext = new ViewModel
            {
                RowItems = GetRowItems("fileExplorer")
            };
            RPCDiscord.changeState("File Explorer");
        }

        private void ContextMenuClick(object sender, RoutedEventArgs e)
        {
            RPCDiscord.changeState("Context Menu");
        }

        private void GeneralClick(object sender, RoutedEventArgs e)
        {
            this.DataContext = new ViewModel
            {
                RowItems = GetRowItems("general")
            };
            RPCDiscord.changeState("General");

        }

        public static ObservableCollection<RowItems> GetRowItems(string category)
        {
            ObservableCollection<RowItems> Items = new ObservableCollection<RowItems>();
            string json = AssemblyExtensions.GetJSONfromAssembly();
            if (String.IsNullOrEmpty(json)) return null;
            var data = JsonConvert.DeserializeObject<Root>(json);

            foreach (var row in data.general.Values.OrderBy(item => item.name))
            {
                if (row.category.ToLower() == category.ToLower())
                {

                        Items.Add(new RowItems()
                        {
                            Enabled = row.userSetting,
                            Name = row.name,
                            Description = row.description,
                            Safe = (row.safe == true ? "Yes" : "No"),
                            CodeName = row.codeName
                        });
                    
                    }
            }
            return Items;
        }

        private void TweaksList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
            TweaksList.UnselectAllCells();
        }

        internal bool showWarnings = true;
        private void ApplyTweaks(object sender, RoutedEventArgs e)
        {
            var json = AssemblyExtensions.GetJSONfromAssembly();
            var data = JsonConvert.DeserializeObject<Root>(json);
            bool applied = false;
            CommandScheduler<string> scheduler = new CommandScheduler<string>();

            foreach (var item in (DataContext as ViewModel).RowItems)
            {
                if (!data.general.ContainsKey(item.CodeName)) continue;
                if (item.Enabled == data.general[item.CodeName].userSetting) continue;
                if ((data.general[item.CodeName].getValues.warning.Length > 0) && (showWarnings == true))
                {
                    const string IGNORE_BUTTON_ID = "ignore";
                    var messageBox = new MessageBoxModel
                    {
                        Text = data.general[item.CodeName].getValues.warning + " \n\nDo you wish to continue?",
                        Caption = item.Name,
                        Icon = MessageBoxImage.Warning,
                        Buttons = new[] {
                            MessageBoxButtons.Custom("Ignore", IGNORE_BUTTON_ID),
                            MessageBoxButtons.Custom("Ignore all", IGNORE_BUTTON_ID),
                            MessageBoxButtons.Cancel(),
                        }
                    };
                    MessageBox.Show(messageBox);

                    switch (messageBox.Result)
                    {
                        case MessageBoxResult.Cancel:
                            item.Enabled = !item.Enabled;
                            
                            continue;

                        case MessageBoxResult.Custom:
                            if(messageBox.ButtonPressed.Label == "Ignore all") this.showWarnings = false;
                            break;
                                
                    }
                }
                Run.ToggleTweak(item.Enabled, item.CodeName.ToString());
                applied = true;
                data.general.TryGetValue(item.CodeName.ToString(), out var tweak);
                if (tweak.getValues.command.Count > 0) foreach (var command in data.general[item.CodeName].getValues.command) scheduler.Add(command);

            }
            scheduler.ExecuteScheduledCommands();
            
            if(applied == true) Info.Show("Applied all Queued Tweaks.", "One Tool For All");
            this.showWarnings = true;
        }
    }
}