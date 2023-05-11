using Floai.Model;
using Floai.Utils;
using OpenAI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;

namespace Floai.Pages
{
    public class ChatViewModel
    {
        public void SetWindowSize(double width, double height)
        {
            AppConfiger.SetValue("initialWindowHeight", width.ToString());
            AppConfiger.SetValue("initialWindowWidth", height.ToString());
        }
        public string GetMsgSaveDir()
        {
            return AppConfiger.GetValue("messageSaveDirectory");
        }

        public string GetApiKey()
        {
            return AppConfiger.GetValue("apiKey");
        }
    }
}
