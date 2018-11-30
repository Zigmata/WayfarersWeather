using System.ComponentModel;

namespace WeatherBotService
{
    [RunInstaller(true)]
    public abstract partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        protected ProjectInstaller()
        {
            InitializeComponent();
        }
    }
}
