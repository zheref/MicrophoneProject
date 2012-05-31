using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net.Sockets;
using System.Net;

namespace MicrophoneProject.LOB
{
    public abstract class Control
    {
        protected IBoundary _boundary;
        protected string _host = string.Empty;
        protected SocketPermission _permission;
        protected Socket _socket = null;
        protected byte[] bytes = new byte[1024];

        public Control(string host, short port, IBoundary boundary)
        {
            this._boundary = boundary;
            this._host = host;
            Permiss();
            Connect(port);
        }

        public void Permiss()
        {
            _permission = GetPermission();
        }

        protected abstract SocketPermission GetPermission();

        public void Connect(short port)
        {
            _socket = GetConnection(port);
        }

        protected abstract Socket GetConnection(short port);

        public int Send(string message)
        {
            try
            {
                byte[] msg = Encoding.Unicode.GetBytes(message);
                return _socket.Send(msg);
            }
            catch (Exception e)
            {
                _boundary.Notify("Exception: " + e.ToString());
                return -1;
            }
        }

        public string Receive(out int feedback)
        {
            try
            {
                feedback = _socket.Receive(bytes);
                return Encoding.Unicode.GetString(bytes, 0, feedback);
            }
            catch (Exception e)
            {
                _boundary.Notify("Exception: " + e.ToString());
                feedback = -1;
                return string.Empty;
            }
        }

        public void ReceivingLoop()
        {
            while (_socket.Available > 0)
            {
                int feedback = 0;
                string msg = Receive(out feedback);
                Console.WriteLine("The server reply: {0}", msg);
            }
        }

        public abstract void Disconnect();
    }
}
