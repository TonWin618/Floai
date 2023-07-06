using Floai.Models;
using Floai.Utils.Model;

namespace Floai.Pages;

public class FloatViewModel
{
    private readonly GeneralSettings appSettings;
    public FloatViewModel(GeneralSettings generalSettings) 
    {
        this.appSettings = generalSettings;
    }
    public (double, double) ReadWindowPostion()
    {
        double positionX = appSettings.InitialPositionX;
        double positionY = appSettings.InitialPositionY;
        return (positionX, positionY);
    }

    public void WriteWindowPostion(double positionX, double positionY)
    {
        appSettings.InitialPositionX = positionX;
        appSettings.InitialPositionY = positionY;
    }
}
