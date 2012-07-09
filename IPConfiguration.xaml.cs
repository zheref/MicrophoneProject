using System;
using System.Data;
using System.Windows.Controls;

namespace MicrophoneProject
{
	/// <summary>
	/// Lógica de interacción para IPConfiguration.xaml
	/// </summary>
	public partial class IPConfiguration : UserControl
	{
		DataSet dataset = new DataSet();
		DataRow dr;

		public IPConfiguration()
		{
			this.InitializeComponent();
			dataset.ReadXml("networkconfig.xml");
			dr = dataset.Tables["network"].Rows[0];
			string ip = dr["ip"].ToString();
			string port = dr["port"].ToString();
			txtIP.Text = ip;
			txtPuerto.Text = port;
		}

		public event EventHandler Close;

		private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			dr["ip"] = txtIP.Text;
			dr["port"] = txtPuerto.Text;
			dataset.WriteXml("networkconfig.xml");
			Close(this, null);
		}
	}
}