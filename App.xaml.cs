using System.Diagnostics;
using System.Windows;

namespace MicrophoneProject
{
	/// <summary>
	/// Lógica de interacción para App.xaml
	/// </summary>
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
		    Process thisProc = Process.GetCurrentProcess();
		    if (Process.GetProcessesByName(thisProc.ProcessName).Length > 1)
		    {
			   // Si ya existe sacamos un mensaje.
			   MessageBox.Show("La aplicación ya se esta ejecutando. No se puede ejecutar esta aplicacion mas de una vez");
			   Application.Current.Shutdown();
			   return;
		    }
			base.OnStartup(e);
		}
	}
}