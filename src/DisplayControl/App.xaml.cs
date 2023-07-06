using System.Windows;

namespace DisplayControl
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            CompositionManager compositionManager = new();
            DisplayManager displayManager = new(compositionManager);

            displayManager.StartDisplay();
        }
    }
}