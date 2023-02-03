using Microsoft.Win32;
using Microsoft.Xaml.Behaviors;
using System.Windows;

namespace LivetSmart.Messaging {
	/// <summary>
	/// 「ファイルを保存する」ダイアログを表示するアクションです。<see cref="SaveFileDialogMessage"/>に対応します。
	/// </summary>
	public class SaveFileDialogMessageAction : TriggerAction<DependencyObject>
    {
        protected override void Invoke(object parameter)
        {
            if (parameter is not SaveFileDialogMessage saveFileMessage)
            {
                return;
            }

            Action(AssociatedObject, saveFileMessage);
        }


        public static void Action(DependencyObject element, SaveFileDialogMessage message)
        {
            message.IsHandled = true;

            var dialog = new SaveFileDialog
            {
                FileName = message.FileName,
                InitialDirectory = message.InitialDirectory,
                AddExtension = message.AddExtension,
                CreatePrompt = message.CreatePrompt,
                Filter = message.Filter,
                OverwritePrompt = message.OverwritePrompt,
                Title = message.Title,
                DefaultExt = message.DefaultExt,
                FilterIndex = message.FilterIndex,
                CheckFileExists = message.CheckFileExists,
                CheckPathExists = message.CheckPathExists,
            };

            var window = Window.GetWindow(element);

            message.Response = dialog.ShowDialog(window) == true ? dialog.FileNames : null;
        }
    }
}
