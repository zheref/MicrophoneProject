using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Data;

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