using System;
using System.Windows.Controls;

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
        public event EventHandler Close;

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
            {
                if(this.SelectedTime == 0.0 || this.SelectedTime == 5.0)
                    t = new TimePickerEventArgs() { Mic = this.Mic, SelectedTime = sldPicker.Maximum * 10 };
                else
                    t = new TimePickerEventArgs() { Mic = this.Mic, SelectedTime = this.SelectedTime };
            }
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
            Close(this, null);
		}
	}

    public class TimePickerEventArgs : EventArgs
    {
        public Microfono Mic { get; set; }
        public double SelectedTime { get; set; }
    }
}