using System;
using System.IO;

namespace Floai.Utils.View;

public static class AppAutoStarter
{
    public static string LnkFilePath
    {
        get
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Startup),
                "Floai.lnk");
        }
    }

    public static void DisableAutoStart()
    {
        if (File.Exists(LnkFilePath))
        {
            File.Delete(LnkFilePath);
        }
    }

    public static void EnableAutoStart()
    {
        var shellType = Type.GetTypeFromProgID("WScript.Shell");
        dynamic shell = Activator.CreateInstance(shellType);
        var shortcut = shell.CreateShortcut(LnkFilePath);
        shortcut.TargetPath = System.Windows.Forms.Application.ExecutablePath;
        shortcut.WorkingDirectory = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        shortcut.Save();
    }
}
