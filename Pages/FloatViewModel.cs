using Floai.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Floai.Pages;

public class FloatViewModel
{
    public (double,double) ReadWindowPostion()
    {
        double window_x = AppConfiger.GetValue<double>("initialPositionX");
        double window_y = AppConfiger.GetValue<double>("initialPositionY");
        return (window_x,window_y);
    }

    public void WriteWindowPostion(double window_x,double window_y)
    {
        AppConfiger.SetValue("initialPositionX", window_x.ToString());
        AppConfiger.SetValue("initialPositionY", window_y.ToString());
    }
}
