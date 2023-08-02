using Floai.Models;
using Floai.Utils.View;
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
    public SettingsView(WindowManager windowManager, SettingsViewModel viewModel)
    {
        this.windowManager = windowManager;
        InitializeComponent();
        
        this.viewModel = viewModel;
        this.DataContext = this.viewModel;
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

    private void BtnOpen_Click(object sender, RoutedEventArgs e)
    {
        Process.Start(Environment.GetEnvironmentVariable("WINDIR") + @"\explorer.exe", Path.GetFullPath(DirTextBox.Text));
    }

    private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        viewModel.ReadApiClientOptions();
    }

    private void BtnSave_Click(object sender, RoutedEventArgs e)
    {
        viewModel.SaveApiClientOptions();
    }
}
