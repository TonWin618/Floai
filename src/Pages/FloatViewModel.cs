using Floai.Models;
using Floai.Utils.Model;

namespace Floai.Pages;

public class FloatViewModel
{
    private readonly GeneralSettings generalSettings;
    public FloatViewModel(GeneralSettings generalSettings) 
    {
        this.generalSettings = generalSettings;
    }
    public (double, double) ReadWindowPostion()
    {
        double positionX = generalSettings.InitialPositionX;
        double positionY = generalSettings.InitialPositionY;
        return (positionX, positionY);
    }

    public void WriteWindowPostion(double positionX, double positionY)
    {
        generalSettings.InitialPositionX = positionX;
        generalSettings.InitialPositionY = positionY;
    }
}
