using Microsoft.Win32;
using System.Windows;

using System.Collections.Generic;
using System.Configuration;
using System.IO;
using Microsoft.Xaml.Behaviors;

namespace LivetSmart.Messaging {
	/// <summary>
	/// 「ファイルを開く」ダイアログを表示するアクションです。<see cref="OpenFileDialogMessage"/>に対応します。
	/// </summary>
	public class OpenFileDialogMessageAction : TriggerAction<DependencyObject>
    {

        static Settings? setting;
        static Dictionary<string, string>? InitialDirectoryGroupList;

        /// <summary>
        /// ファイル ダイアログに表示される初期ディレクトリのグループを取得または設定します。
        /// </summary>
        public string? InitialDirectoryGroup { get; set; }

        protected override void Invoke(object parameter)
        {
            if (parameter is not OpenFileDialogMessage openFileDialogMessage)
            {
                return;
            }

            openFileDialogMessage.InitialDirectoryGroup ??= InitialDirectoryGroup ?? "";

            Action(AssociatedObject, openFileDialogMessage);
        }

        public static void Action(DependencyObject element, OpenFileDialogMessage message)
        {
            message.IsHandled = true;

            var window = Window.GetWindow(element);

            if (InitialDirectoryGroupList is null || setting is null)
            {
                setting = new Settings("OpenFileDialogSettings");
                if (setting.IsUpgrade != true)
                {
                    setting.Upgrade();
                }

                setting.IsUpgrade = true;
                InitialDirectoryGroupList = (setting.Group ??= new Dictionary<string, string>());
            }

            var initialDirectory = message.InitialDirectory;
            var group = message.InitialDirectoryGroup;

            if (group is not null && InitialDirectoryGroupList.ContainsKey(group))
            {
                initialDirectory = InitialDirectoryGroupList[group];
            }

            var dialog = new OpenFileDialog()
            {
                FileName = message.FileName,
                InitialDirectory = !string.IsNullOrEmpty(initialDirectory) ? Path.GetFullPath(initialDirectory) : initialDirectory,
                AddExtension = message.AddExtension,
                Filter = message.Filter,
                Title = message.Title,
                Multiselect = message.MultiSelect,
                FilterIndex = message.FilterIndex,
                DefaultExt = message.DefaultExt,
                CheckPathExists = message.CheckPathExists,
                CheckFileExists = message.CheckFileExists,
            };

            if (dialog.ShowDialog(window) == true)
            {
                message.Response = dialog.FileNames;

                if (group is not null)
                {
                    InitialDirectoryGroupList[group] = Path.GetDirectoryName(dialog.FileName) ?? "";
                    setting.Save();
                }
            }
            else
            {
                message.Response = null;
            }
        }

        public class Settings : ApplicationSettingsBase
        {
            public Settings(string settingsKey) : base(settingsKey) { }

            [UserScopedSetting]
            public Dictionary<string, string>? Group
            {
                get { return (Dictionary<string, string>?)this["Group"]; }
                set { this["Group"] = value; }
            }

            [UserScopedSetting]
            public bool? IsUpgrade
            {
                get { return (bool?)this["IsUpgrade"]; }
                set { this["IsUpgrade"] = value; }
            }
        }
    }
}
