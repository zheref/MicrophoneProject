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

        public Microfono Mic { private get; set; }
        public double SelectedTime { get; set; }

        public event EventHandler<TimePickerEventArgs> Confirm;
        public event EventHandler<TimePickerEventArgs> Close;

		private void Slider_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
		{
            if (sldPicker.Value != 0 && sldPicker.Value != 5)
            {
                SelectedTime = Math.Truncate(sldPicker.Value * 10) / 10;
                btnFree.Content = SelectedTime + " mins";
            }
            else
            {
                btnFree.Content = "Libre";
                SelectedTime = 50;
            }
		}

		private void btnConfirm_Click(object sender, System.Windows.RoutedEventArgs e)
		{
            Button boton = sender as Button;
            TimePickerEventArgs t = null;
            if (boton == btnFree)
                t = new TimePickerEventArgs() { Mic = this.Mic, SelectedTime = this.SelectedTime };
            else if (boton == btnOne)
                t = new TimePickerEventArgs() { Mic = this.Mic, SelectedTime = 1 };
            else if (boton == btnHalf)
                t = new TimePickerEventArgs() { Mic = this.Mic, SelectedTime = 0.5 };
            else if (boton == btnTen)
                t = new TimePickerEventArgs() { Mic = this.Mic, SelectedTime = 0.2 };
            if(t != null)
                Confirm(this, t);
		}

		private void btnCancel_Click(object sender, System.Windows.RoutedEventArgs e)
		{
            TimePickerEventArgs t = new TimePickerEventArgs() { Mic = this.Mic, SelectedTime = this.SelectedTime };
            Close(this, t);
		}
	}

    public class TimePickerEventArgs : EventArgs
    {
        public Microfono Mic { get; set; }
        public double SelectedTime { get; set; }
    }
}