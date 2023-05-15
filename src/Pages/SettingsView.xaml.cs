using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows;

namespace Floai.Pages;

public partial class SettingsView : Window
{
    public SettingsView()
    {
        InitializeComponent();
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        var viewModel = new SettingsViewModel();
        this.DataContext = viewModel;
    }

    private void BtnBrowse_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new CommonOpenFileDialog();
        dlg.IsFolderPicker = true;
        if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
        {
            DirTextBox.Text = dlg.FileName;
        }
    }
}
