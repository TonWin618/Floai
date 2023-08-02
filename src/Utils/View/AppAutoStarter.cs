using System;
using System.IO;

namespace Floai.Utils.View;

/// <summary>
/// A utility class to manage automatic application startup with a shortcut (lnk file) in the Startup folder.
/// </summary>
public static class AppAutoStarter
{
    /// <summary>
    /// Gets the full path to the lnk (shortcut) file in the Startup folder.
    /// </summary>
    public static string LnkFilePath
    {
        get
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Startup),
                "Floai.lnk");
        }
    }

    /// <summary>
    /// Disables automatic application startup by removing the lnk file from the Startup folder.
    /// </summary>
    public static void DisableAutoStart()
    {
        if (File.Exists(LnkFilePath))
        {
            File.Delete(LnkFilePath);
        }
    }

    /// <summary>
    /// Enables automatic application startup by creating a lnk file in the Startup folder.
    /// </summary>
    public static void EnableAutoStart()
    {
        // Get the type of the WScript.Shell COM object.
        var shellType = Type.GetTypeFromProgID("WScript.Shell");

        // Create an instance of the WScript.Shell object.
        dynamic shell = Activator.CreateInstance(shellType);

        // Set the target path (executable path of the application).
        var shortcut = shell.CreateShortcut(LnkFilePath);

        // Set the target path (executable path of the application).
        shortcut.TargetPath = System.Windows.Forms.Application.ExecutablePath;

        // Set the working directory to the application's base directory.
        shortcut.WorkingDirectory = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

        // Save the shortcut.
        shortcut.Save();
    }
}
