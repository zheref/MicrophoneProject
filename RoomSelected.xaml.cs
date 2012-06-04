using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;
using System;
using System.Net.Sockets;
using System.IO;
using Instartius.Net.Sockets;

namespace MicrophoneProject
{
    /// <summary>
    /// Lógica de interacción para RoomSelected.xaml
    /// </summary>
    public partial class RoomSelected : UserControl, INETLogger
    {
        Queue<Turno> _cola = new Queue<Turno>();
        List<Microfono> _microfonos;
        DispatcherTimer _timer = new DispatcherTimer();
        SocketClient _cliente;

        readonly int DIF_ENCENDER_APAGAR = 20;

        public RoomSelected()
        {
            InitializeComponent();
            InicializarMicrofonos();
            _timer.Interval = new TimeSpan(0, 0, 1);
            _timer.Tick += new EventHandler(_timer_Tick);
            _cliente = new SocketClient(this);
        }
        
        public void InicializarMicrofonos()
        {
            _microfonos = new List<Microfono>();
            #region
            _microfonos.Add(new Microfono { Number = 1, Control = btn1 });
            _microfonos.Add(new Microfono { Number = 2, Control = btn2 });
            _microfonos.Add(new Microfono { Number = 3, Control = btn3 });
            _microfonos.Add(new Microfono { Number = 4, Control = btn4 });
            _microfonos.Add(new Microfono { Number = 5, Control = btn5 });
            _microfonos.Add(new Microfono { Number = 6, Control = btn6 });
            _microfonos.Add(new Microfono { Number = 7, Control = btn7 });
            _microfonos.Add(new Microfono { Number = 8, Control = btn8 });
            _microfonos.Add(new Microfono { Number = 9, Control = btn9 });
            _microfonos.Add(new Microfono { Number = 10, Control = btn10 });
            _microfonos.Add(new Microfono { Number = 11, Control = btn11 });
            _microfonos.Add(new Microfono { Number = 12, Control = btn12 });
            _microfonos.Add(new Microfono { Number = 13, Control = btn13 });
            _microfonos.Add(new Microfono { Number = 14, Control = btn14 });
            _microfonos.Add(new Microfono { Number = 15, Control = btn15 });
            _microfonos.Add(new Microfono { Number = 16, Control = btn16 });
            _microfonos.Add(new Microfono { Number = 17, Control = btn17 });
            _microfonos.Add(new Microfono { Number = 18, Control = btn18 });
            #endregion
        }

        private void Microfono_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Button but = sender as Button;
            var mics = from m in _microfonos
                       where m.Control == but
                       select m;
            Microfono mic = mics.Single();
            var turns = from t in _cola
                        where t.Mic == mic
                        select t;
            if (turns.ToList().Count == 0)
            {
                TimePicker tmpick = new TimePicker() { Mic = mic };
                tmpick.Confirm += new EventHandler<TimePickerEventArgs>(TimeConfirmed);
                tmpick.Close += new EventHandler<TimePickerEventArgs>(TimeConfirmed);
                grdMain.Children.Add(tmpick);
                DockPanel.SetDock(tmpick, Dock.Right);
            }
            else
            {
                QuitarDeLista(turns.Single());
            }
        }

        private void QuitarDeLista(Turno turno)
        {
            List<Turno> turnos = _cola.ToList();
            turnos.Remove(turno);
            _cola.Clear();
            foreach (Turno t in turnos)
                _cola.Enqueue(t);
            RefreshMics();
        }

        public void Enqueue(object sender, EventArgs e)
        {
            var mics = from m in _microfonos
                       where m.Control == (sender as Button)
                       select m;
            Microfono mic = mics.Single();
        }

        public void TimeConfirmed(object sender, TimePickerEventArgs args)
        {
            long time = (long) (args.SelectedTime * 60);
            Turno turno = new Turno { Time = time, TotalTime = time , Mic = args.Mic };
            _cola.Enqueue(turno);
            grdMain.Children.Remove(sender as TimePicker);
            RefreshMics();
            if (!_timer.IsEnabled)
            {
                _cliente.Connect(4510);
                _timer.Start();
            }
        }

        public void TimeClosed(object sender, TimePickerEventArgs args)
        {

        }

        public void RefreshMics()
        {
            CleanMics();
            List<Turno> turnos = _cola.ToList();
            for (int i = 0; i < turnos.Count; i++)
                turnos[i].Mic.Control.Content = (i + 1).ToString();
        }

        private void CleanMics()
        {
            foreach (Microfono m in _microfonos)
                m.Control.Content = string.Empty;
        }

        void _timer_Tick(object sender, EventArgs e)
        {
            if (Time > 0)
            {
                Turno turno = _cola.Peek();
                if (turno.Time > 0)
                {
                    if (turno.TotalTime - turno.Time == 0)
                        TurnMicOn(turno.Mic.Number);
                    turno.Time--;
                    turno.Mic.Control.Content = (turno.Time).ToString();
                }
                else
                {
                    TurnMicOff(turno.Mic.Number);
                    _cola.Dequeue();
                    RefreshMics();
                }
            }
            else
            {
                Detenerse();
                _cliente.Disconnect();
            }
        }

        public long Time
        {
            get
            {
                long time = 0;
                List<Turno> turnos = _cola.ToList();
                foreach (Turno t in turnos)
                    time += t.Time;
                return time;
            }
        }

        void Detenerse()
        {
            _timer.Stop();
            Turno t = _cola.Dequeue();
            TurnMicOff(t.Mic.Number);
            CleanMics();
        }

        void TurnMicOn(byte num)
        {
            _cliente.Send((num).ToString(), true);
        }

        void TurnMicOff(byte num)
        {
            _cliente.Send((num + DIF_ENCENDER_APAGAR).ToString(), true);
        }

        public void Notify(string msg)
        {
            System.Windows.MessageBox.Show(msg);
        }

        public void Log(string msg)
        {
            txtNotificador.AppendText(msg + "\n");
        }

        public void Clean()
        {
            txtNotificador.Clear();
        }
    }

    public class Microfono
    {
        public byte Number { get; set; }
        public Button Control { get; set; }
    }

    public class Turno
    {
        public long Time { get; set; }
        public long TotalTime { get; set; }
        public Microfono Mic { get; set; }
    }
}
