using Floai.Utils.Data;

namespace Floai.Pages;

public class FloatViewModel
{
    public (double, double) ReadWindowPostion()
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
