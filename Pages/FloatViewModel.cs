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
        double positionX = AppConfiger.GetValue<double>("initialPositionX");
        double positionY = AppConfiger.GetValue<double>("initialPositionY");
        return (positionX, positionY);
    }

    public void WriteWindowPostion(double positionX, double positionY)
    {
        AppConfiger.SetValue("initialPositionX", positionX.ToString());
        AppConfiger.SetValue("initialPositionY", positionY.ToString());
    }
}
