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

namespace MicrophoneProject
{
	/// <summary>
	/// Lógica de interacción para AboutInstartius.xaml
	/// </summary>
	public partial class AboutInstartius : UserControl
	{
		public AboutInstartius()
		{
			this.InitializeComponent();
		}

        public event EventHandler Close;

		private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
		{
            Close(this, null);
		}
	}
}