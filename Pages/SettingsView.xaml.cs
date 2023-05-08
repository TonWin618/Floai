using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.WindowsAPICodePack.Dialogs;

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
        dlg.InitialDirectory = "C:/";

        if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
        {
            var binding = DirTextBox.GetBindingExpression(TextBox.TextProperty);
            if (binding != null)
            {
                binding.UpdateTarget();
            }
            DirTextBox.Text = dlg.FileName;
        }
    }
}
