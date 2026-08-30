using System;
using System.Windows;

namespace DesktopClockWidget
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            // Ensure single instance
            var mutex = new System.Threading.Mutex(true, "DesktopClockWidget_SingleInstance", out bool createdNew);
            if (!createdNew)
            {
                Current.Shutdown();
                return;
            }
            // Keep mutex alive
            GC.KeepAlive(mutex);
        }
    }
}