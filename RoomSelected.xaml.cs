using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Threading;
using Instartius.Net.Sockets;
using System.Windows.Input;
using System.Windows;
using System.Windows.Media;

namespace MicrophoneProject
{
	/// <summary>
	/// Lógica de interacción para RoomSelected.xaml
	/// </summary>
	public partial class RoomSelected : UserControl, INETLogger
	{
		MicrophoneClient b;

		public RoomSelected()
		{
			InitializeComponent();
			//PreConf();
			b = new MicrophoneClient(this);
			InicializarMicrofonos();
			b.Connect();
			if (b.Conectado)
			{
				btnConnect.Content = "Conectado";
				btnConf.Visibility = Visibility.Hidden;
			}
			else
			{
				btnConnect.Content = "Desconectado";
				btnConf.Visibility = Visibility.Visible;
			}
		}

		#region Preconfiguracion
		/// <summary>
		/// Preconfigura los botones para que desencadene evento automaticamente tras un touch
		/// </summary>
		private void PreConf()
		{
			#region Stylus Preconf
			Stylus.SetIsPressAndHoldEnabled(btn1, false);
			Stylus.SetIsPressAndHoldEnabled(btn2, false);
			Stylus.SetIsPressAndHoldEnabled(btn3, false);
			Stylus.SetIsPressAndHoldEnabled(btn4, false);
			Stylus.SetIsPressAndHoldEnabled(btn5, false);
			Stylus.SetIsPressAndHoldEnabled(btn6, false);
			Stylus.SetIsPressAndHoldEnabled(btn7, false);
			Stylus.SetIsPressAndHoldEnabled(btn8, false);
			Stylus.SetIsPressAndHoldEnabled(btn9, false);
			Stylus.SetIsPressAndHoldEnabled(btn10, false);
			Stylus.SetIsPressAndHoldEnabled(btn11, false);
			Stylus.SetIsPressAndHoldEnabled(btn12, false);
			Stylus.SetIsPressAndHoldEnabled(btn13, false);
			Stylus.SetIsPressAndHoldEnabled(btn14, false);
			Stylus.SetIsPressAndHoldEnabled(btn15, false);
			Stylus.SetIsPressAndHoldEnabled(btn16, false);
			Stylus.SetIsPressAndHoldEnabled(btn17, false);
			Stylus.SetIsPressAndHoldEnabled(btn18, false);
			Stylus.SetIsPressAndHoldEnabled(btn10, false);
			#endregion
		}
		
		/// <summary>
		/// Inicializa los microfonos y les establece su control de boton asociado
		/// </summary>
		public void InicializarMicrofonos()
		{
			#region Inicializacion de Microfonos
			b.AgregarMicrofono(new Microfono { Number = 1, Control = btn1, Tag = lbl1 });
			b.AgregarMicrofono(new Microfono { Number = 2, Control = btn2, Tag = lbl2 });
			b.AgregarMicrofono(new Microfono { Number = 3, Control = btn3, Tag = lbl3 });
			b.AgregarMicrofono(new Microfono { Number = 4, Control = btn4, Tag = lbl4 });
			b.AgregarMicrofono(new Microfono { Number = 5, Control = btn5, Tag = lbl5 });
			b.AgregarMicrofono(new Microfono { Number = 6, Control = btn6, Tag = lbl6 });
			b.AgregarMicrofono(new Microfono { Number = 7, Control = btn7, Tag = lbl7 });
			b.AgregarMicrofono(new Microfono { Number = 8, Control = btn8, Tag = lbl8 });
			b.AgregarMicrofono(new Microfono { Number = 9, Control = btn9, Tag = lbl9 });
			b.AgregarMicrofono(new Microfono { Number = 10, Control = btn10, Tag = lbl10 });
			b.AgregarMicrofono(new Microfono { Number = 11, Control = btn11, Tag = lbl11 });
			b.AgregarMicrofono(new Microfono { Number = 12, Control = btn12, Tag = lbl12 });
			b.AgregarMicrofono(new Microfono { Number = 13, Control = btn13, Tag = lbl13 });
			b.AgregarMicrofono(new Microfono { Number = 14, Control = btn14, Tag = lbl14 });
			b.AgregarMicrofono(new Microfono { Number = 15, Control = btn15, Tag = lbl15 });
			b.AgregarMicrofono(new Microfono { Number = 16, Control = btn16, Tag = lbl16 });
			b.AgregarMicrofono(new Microfono { Number = 17, Control = btn17, Tag = lbl17 });
			b.AgregarMicrofono(new Microfono { Number = 18, Control = btn18, Tag = lbl18 });
			b.ActualizarTagDeTodosMicrofonos();
			#endregion
		}
		#endregion

		#region Gestion Timepicker
		/// <summary>
		/// Muestra el control de Time Picker para el microfono seleccionado
		/// </summary>
		/// <param name="mic">El microfono al que se le desea asignar un tiempo como turno</param>
		public void MostrarControlTiempo(Microfono mic)
		{
			TimePicker tmpick = new TimePicker() { Mic = mic };
			tmpick.Confirm += new EventHandler<TimePickerEventArgs>(TimeConfirmed);
			tmpick.Close += new EventHandler(TimeClosed);
			CleanMain();
			RemoverBotonesLateralesPrincipales();
			grdMain.Children.Add(tmpick);
			tmpick.Margin = new Thickness(39, 21.765, 38, 0);
			Grid.SetRow(tmpick, 0);
			Grid.SetColumn(tmpick, 1);
		}

		private void RemoverBotonesLateralesPrincipales()
		{
			grdMain.Children.Remove(btnFreeMode);
			grdMain.Children.Remove(btnTurnAlloff);
			grdMain.Children.Remove(btnConnect);
		}

		private void ColocarBotonesLateralesPrincipales()
		{
			grdMain.Children.Add(btnFreeMode);
			grdMain.Children.Add(btnTurnAlloff);
			grdMain.Children.Add(btnConnect);
		}

		/// <summary>
		/// Ocultamos el control TimePicker del contenedor una vez ya lo hemos usado exitosamente
		/// </summary>
		/// <param name="tm">El control TimePicker a remover del contenedor</param>
		public void OcultarControlTiempo(TimePicker tm)
		{
			grdMain.Children.Remove(tm);
			ColocarBotonesLateralesPrincipales();
		}
		#endregion

		#region Gestion Labelchanger

		#endregion

		#region Muestreos
		void MostrarTableroSala()
		{
			grdMain.Children.Add(grdSala);
			Grid.SetRow(grdSala, 0);
		}
		#endregion

		#region Refresh
		public void ActualizarBotonesMicrofonos()
		{
			LimpiarTextoBotonesMicrofonos();
			if (b.IsFreeMode)
			{
				List<Microfono> mics = b.FreeMicrofonos;
				foreach (Microfono mic in mics)
					mic.Control.Content = "LIBRE";
			}
			else
			{
				List<Turno> turnos = b.Turnos;
				for (int i = 0; i < turnos.Count; i++)
					turnos[i].Mic.Control.Content = (i + 1).ToString();
			}
		}

		public void LimpiarTextoBotonesMicrofonos()
		{
			foreach (Microfono m in b.Microfonos)
				m.Control.Content = string.Empty;
		}
		#endregion

		#region Eventos
		/// <summary>
		/// Evento para cuando se hace click en el boton asociado a un microfono
		/// </summary>
		/// <param name="sender">El boton que desencadeno el evento</param>
		/// <param name="e">Los parametros subyacentes detras de la operacion del evento</param>
		void Microfono_Click(object sender, RoutedEventArgs e) { b.SeleccionDeMicrofono(sender as Button); }

		/// <summary>
		/// Evento para cuando se hace touch en el boton asociado a un microfono
		/// </summary>
		/// <param name="sender">El boton que desencadeno el evento</param>
		/// <param name="e">Los parametros subyacentes detras de la operacion del evento</param>
		void Microfono_TouchDown(object sender, TouchEventArgs e) { /*b.SeleccionDeMicrofono(sender as Button);*/ }

		/// <summary>
		/// Evento para cuando se confirma un tiempo en el controlador TimePicker
		/// </summary>
		/// <param name="sender">El TimePicker que desencadeno el evento</param>
		/// <param name="args">Los argumentos necesarios (microfono y tiempo seleccionado)</param>
		public void TimeConfirmed(object sender, TimePickerEventArgs args)
		{
			OcultarControlTiempo(sender as TimePicker);
			b.AgregarACola(args.SelectedTime, args.Mic);
		}

		/// <summary>
		/// Evento para cuando se cancela la operacion de seleccion de tiempo para el controlador TimePicker
		/// </summary>
		/// <param name="sender">El TimePicker que desencadeno el evento</param>
		/// <param name="args">NULL</param>
		public void TimeClosed(object sender, EventArgs args)
		{
			OcultarControlTiempo(sender as TimePicker);
			ActualizarBotonesMicrofonos();
		}

		private void btnLogSeen_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			if (txtNotificador.IsVisible)
			{
				txtNotificador.Visibility = System.Windows.Visibility.Hidden;
				btnLogSeen.Content = "Mostrar Log";
			}
			else
			{
				txtNotificador.Visibility = System.Windows.Visibility.Visible;
				btnLogSeen.Content = "Ocultar Log";
			}
		}

		private void btnFreeMode_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			SolidColorBrush brush = new SolidColorBrush();
			brush.Color = Colors.White;
			if (b.IsFreeMode)
			{
				b.IsFreeMode = false;
				brush.Opacity = 0.1;
				btnFreeMode.Background = brush;
				btnFreeMode.Foreground = new SolidColorBrush(Colors.White);
			}
			else
			{
				b.IsFreeMode = true;
				brush.Opacity = 1;
				btnFreeMode.Background = brush;
				btnFreeMode.Foreground = new SolidColorBrush(Colors.Black);
			}
		}

		private void btnTurnAlloff_Click(object sender, System.Windows.RoutedEventArgs e) { b.OrdenApagarTodo(); }

		private void btnAbout_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			CleanMain();
			grdMain.Children.Remove(grdSala);
			AboutInstartius ins = new AboutInstartius();
			ins.Close += new EventHandler(ins_Close);
			grdMain.Children.Add(ins);
			Grid.SetRow(ins, 0);
		}

		void ins_Close(object sender, EventArgs e)
		{
			CleanMain();
			grdMain.Children.Remove(sender as AboutInstartius);
			MostrarTableroSala();
		}

		void btnConf_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			CleanMain();
			grdMain.Children.Remove(grdSala);
			IPConfiguration conf = new IPConfiguration();
			conf.Close += new EventHandler(conf_Close);
			grdMain.Children.Add(conf);
			Grid.SetRow(conf, 0);
		}

		void conf_Close(object sender, EventArgs e)
		{
			CleanMain();
			grdMain.Children.Remove(sender as IPConfiguration);
			MostrarTableroSala();
		}

		private void btnConnect_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			bool connected = b.Connect();
			if (connected)
				btnConnect.Content = "Conectado";
		}
		
		private void Label_MouseDown(object sender, MouseButtonEventArgs e)
		{
			
		}

		private void Caption_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
            CleanMain();
            grdMain.Children.Remove(grdSala);
            Microfono mic = b.MicrofonoPropietarioDeTag(sender as Label);
            LabelChanger changer = new LabelChanger() { Mic = mic };
            changer.Set();
            changer.ActionAtBack += new EventHandler<LabelChangingArgs>(changer_ActionAtBack);
            grdMain.Children.Add(changer);
            Grid.SetRow(changer, 0);
		}

        void changer_ActionAtBack(object sender, LabelChangingArgs e)
        {
            CleanMain();
            grdMain.Children.Remove(sender as LabelChanger);
            MostrarTableroSala();
            b.ActualizarTagDelMicrofono(e.Mic);
        }

		void CleanMain()
		{
			/*
			grdMain.Children.Clear();
			grdMain.Children.Add(txtNotificador);
			grdMain.Children.Add(btnFreeMode);
			grdMain.Children.Add(btnTurnAlloff);
			grdMain.Children.Add(btnAbout);
			grdMain.Children.Add(btnLogSeen);
			 * */
		}
		#endregion

		#region  INETLogger Contracts
		/// <summary>
		/// Notifica de manera emergente el mensaje
		/// </summary>
		/// <param name="msg">El mensaje a notificar</param>
		public void Notify(string msg) { System.Windows.MessageBox.Show(msg); }

		/// <summary>
		/// Imprime en el registro la transaccion descrita en el mensaje
		/// </summary>
		/// <param name="msg">El mensaje que describe la transaccion</param>
		public void Log(string msg) { txtNotificador.AppendText(msg + "\n"); }

		/// <summary>
		/// Limpia el registro
		/// </summary>
		public void Clean() { txtNotificador.Clear(); }
		#endregion

	}
}
