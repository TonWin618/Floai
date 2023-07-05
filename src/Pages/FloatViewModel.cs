using Floai.Models;
using Floai.Utils.Model;

namespace Floai.Pages;

public class FloatViewModel
{
    private readonly AppSettings appSettings;
    public FloatViewModel(AppSettings appSettings) 
    {
        this.appSettings = appSettings;
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
