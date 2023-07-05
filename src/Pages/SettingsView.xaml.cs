using Floai.Utils.App;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Floai.Pages;

public partial class SettingsView : Window,ISetWindowProperties
{
    SettingsViewModel viewModel;
    private readonly WindowManager windowManager;
    public SettingsView(WindowManager windowManager)
    {
        this.windowManager = windowManager;
        InitializeComponent();
        
        viewModel = new SettingsViewModel();
        this.DataContext = viewModel;
    }

    public void SetWindowProperties(WindowProperties? properties = null)
    {
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
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

    private void BtnAddApiKey_Click(object sender, RoutedEventArgs e)
    {
        viewModel.AppendApiKey(ApiKeyTextBox.Text);
        ApiKeyTextBox.Text = "";
    }

    private void BtnRemoveApiKey_Click(object sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        var item = button.DataContext as string;
        viewModel.RemoveApiKey(item);
    }

    private void BtnOpen_Click(object sender, RoutedEventArgs e)
    {
        Process.Start(Environment.GetEnvironmentVariable("WINDIR") + @"\explorer.exe", Path.GetFullPath(DirTextBox.Text));
    }
}
