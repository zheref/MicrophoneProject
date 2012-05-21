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
	/// Lógica de interacción para TimePicker.xaml
	/// </summary>
	public partial class TimePicker : UserControl
	{
		public TimePicker()
		{
			this.InitializeComponent();
		}

		private void Slider_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
		{
			if(sldPicker.Value != 0 && sldPicker.Value != 5)
			{
				double d = Math.Truncate(sldPicker.Value * 10) / 10;
				btnFree.Content = d + " mins";
			}
			else
				btnFree.Content = "Libre";
		}
	}
}