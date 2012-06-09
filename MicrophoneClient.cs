using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Threading;
using Instartius.Net.Sockets;
using System;
using System.Linq;

namespace MicrophoneProject
{
    public class MicrophoneClient
    {
        DispatcherTimer _timer = new DispatcherTimer();
        Queue<Turno> _cola = new Queue<Turno>();
        List<Microfono> _microfonos;
        List<Microfono> _activeFreeMics = new List<Microfono>();
        bool isfreemode = false;
        SocketClient _cliente;
        RoomSelected _p;

        #region Bindings semanticos
        readonly int DIF_ENCENDER_APAGAR = 30;

        public bool IsFreeMode 
        { 
            get { return isfreemode; }
            set
            {
                isfreemode = value;
                if (value == true)
                {
                    _cola.Clear();
                    if (_timer.IsEnabled)
                        _timer.Stop();
                    ApagarTodos();
                }
                else
                {
                    _activeFreeMics.Clear();
                    ApagarTodos();
                }
                _p.LimpiarTextoBotonesMicrofonos();
            }
        }
        
        public List<Turno> Turnos { get { return _cola.ToList(); } }

        public List<Microfono> Microfonos { get { return _microfonos; } }

        public List<Microfono> FreeMicrofonos { get { return _activeFreeMics; } }
        #endregion

        public MicrophoneClient(RoomSelected roomselecter)
        {
            this._p = roomselecter;
            _timer.Interval = new TimeSpan(0, 0, 1);
            _timer.Tick += new EventHandler(_timer_Tick);
            _cliente = new SocketClient(roomselecter);
            _microfonos = new List<Microfono>();
            ConectarseAServidor();
            ApagarTodos();
        }

        public void AgregarMicrofono(Microfono mic) { _microfonos.Add(mic); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        public void SeleccionDeMicrofono(Button but)
        {
            var mics = from m in _microfonos
                       where m.Control == but
                       select m;
            Microfono mic = mics.Single();
            if (this.IsFreeMode)
            {
                if (MicrofonoEstaFreeActivo(mic))
                {
                    _activeFreeMics.Remove(mic);
                    _p.ActualizarBotonesMicrofonos();
                    ApagarMicrofono(mic.Number);
                }
                else
                {
                    _activeFreeMics.Add(mic);
                    _p.ActualizarBotonesMicrofonos();
                    EncenderMicrofono(mic.Number);
                }
            }
            else
            {
                if (MicrofonoEstaEnCola(mic))
                    QuitarDeCola(TurnoAsignadoDeMicrofono(mic));
                else
                    MostrarControlTiempo(mic);
            }
        }

        public void OrdenApagarTodo()
        {
            ApagarTodos();
            if (IsFreeMode)
                _activeFreeMics.Clear();
            else
            {
                _cola.Clear();
                if (_timer.IsEnabled)
                    _timer.Stop();
            }
            _p.LimpiarTextoBotonesMicrofonos();
        }

        void MostrarControlTiempo(Microfono mic) { _p.MostrarControlTiempo(mic); }

        Turno TurnoAsignadoDeMicrofono(Microfono mic)
        {
            var turns = from t in _cola
                        where t.Mic == mic
                        select t;
            return turns.Single();
        }

        void _timer_Tick(object sender, EventArgs e)
        {
            if (Time > 0)
            {
                Turno turno = _cola.Peek();
                if (turno.Time > 0)
                {
                    if (turno.IsBeginning)
                        EncenderMicrofono(turno.Mic.Number);
                    if (!turno.IsFree)
                        turno.Time--;
                    turno.Mic.Control.Content = turno.ToString();
                }
                else
                {
                    ApagarMicrofono(turno.Mic.Number);
                    _cola.Dequeue();
                    _p.ActualizarBotonesMicrofonos();
                }
            }
            else
            {
                Detenerse();
                DesconectarseDeServidor();
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
            ApagarMicrofono(t.Mic.Number);
            _p.LimpiarTextoBotonesMicrofonos();
        }

        bool MicrofonoEstaEnCola(Microfono mic)
        {
            var turns = from t in _cola
                        where t.Mic == mic
                        select t;
            return turns.ToList().Count != 0;
        }

        bool MicrofonoEstaFreeActivo(Microfono mic)
        {
            var mics = from m in _activeFreeMics
                        where m == mic
                        select m;
            return mics.ToList().Count != 0;
        }

        public void AgregarACola(double selectedTime, Microfono mic)
        {
            long time = (long)(selectedTime * 60);
            Turno turno = new Turno { Time = time, TotalTime = time, Mic = mic };
            _cola.Enqueue(turno);
            _p.ActualizarBotonesMicrofonos();
            if (!_timer.IsEnabled)
            {
                ConectarseAServidor();
                _timer.Start();
            }
        }

        private void QuitarDeCola(Turno turno)
        {
            if (EstaCorriendoTurno(turno))
                ApagarMicrofono(turno.Mic.Number);
            List<Turno> turnos = this.Turnos;
            turnos.Remove(turno);
            RefrescarCola(turnos);
            if (_cola.ToList().Count <= 0)
            {
                _timer.Stop();
                DesconectarseDeServidor();
            }
            _p.ActualizarBotonesMicrofonos();
        }

        bool EstaCorriendoTurno(Turno turno)
        {
            return _cola.Peek() == turno;
        }

        void RefrescarCola(List<Turno> turnos)
        {
            _cola.Clear();
            foreach (Turno t in turnos)
                _cola.Enqueue(t);
        }

        #region Envio de ordenes para microfonos
        void EncenderMicrofono(byte num) 
        {
            if (_cliente.Connected)
                _cliente.Send((num).ToString(), true);
            else
                _p.Notify("Intentando enviar instruccion de ON pero no se encuentra un servidor conectado. Por favor arranque o reinicie el servidor correspondiente.");
        }

        void ApagarMicrofono(byte num) 
        {
            if (_cliente.Connected)
                _cliente.Send((num + DIF_ENCENDER_APAGAR).ToString(), true);
            else
                _p.Notify("Intentando enviar instruccion de ON pero no se encuentra un servidor conectado. Por favor arranque o reinicie el servidor correspondiente.");
        }

        void ApagarTodos()
        {
            DispatcherTimer tim = new DispatcherTimer();
            tim.Interval = new TimeSpan(0, 0, 0, 0, 70);
            tim.Tick += new EventHandler(tim_Tick);
            this.i = 0;
            tim.Start();
        }

        void tim_Tick(object sender, EventArgs e)
        {
            i++;
            if (i < 21)
                ApagarMicrofono(i);
            else
                (sender as DispatcherTimer).Stop();
        }

        byte i = 0;

        #endregion

        #region Conexion Cliente-Servidor
        /// <summary>
        /// Intenta conectarse el cliente al servidor correspondiente
        /// </summary>
        /// <returns>true si el cliente consigue estar conectado al servidor al finalizar la operacion</returns>
        bool ConectarseAServidor()
        {
            //_cliente.Connect(4510, 186, 114, 146, 28);
            if (_cliente.Connected)
            {
                _p.Log("Esta intentando conectar el cliente al servidor, pero este ya esta conectado");
                return true;
            }
            else
                return _cliente.Connect(4510, 192, 168, 114, 1);
        }
        
        /// <summary>
        /// Intenta desconectar el cliente del servidor correspondiente
        /// </summary>
        /// <returns>true si el cliente consigue estar conectado al servidor al finalizar la operacion</returns>
        bool DesconectarseDeServidor()
        {
            if (_cliente.Connected)
                return _cliente.Disconnect();
            else
            {
                _p.Log("Esta intentando conectar el cliente al servidor, pero este ya esta conectado");
                return true;
            }
        }
        #endregion
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
        bool _begin = true;

        public bool IsBeginning
        {
            get
            {
                if (IsFree)
                {
                    bool real = _begin;
                    _begin = false;
                    return real;
                }
                else
                    return TotalTime - Time == 0;
            }
        }
        public bool IsFree { get { return Time >= 50; } }

        public override string ToString()
        {
            if (IsFree)
                return "LIBRE";
            else
                return this.Time.ToString();
        }
    }
}
