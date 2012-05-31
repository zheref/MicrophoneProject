using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net.Sockets;
using System.Net;

namespace MicrophoneProject.LOB
{
    public class ControlClient : Control
    {
        public ControlClient(string host, short port, IBoundary boundary) : base(host, port, boundary) { }

        protected override SocketPermission GetPermission()
        {
            try
            {
                SocketPermission permission = new SocketPermission
                    (
                        NetworkAccess.Connect,
                        TransportType.Tcp,
                        _host,
                        SocketPermission.AllPorts
                    );
                permission.Demand();
                return permission;
            }
            catch (Exception e)
            {
                _boundary.Notify(e.ToString());
                return null;
            }
        }

        protected override Socket GetConnection(short port)
        {
            try
            {
                IPHostEntry ipHost = Dns.GetHostEntry(_host);
                IPAddress ipAddress = ipHost.AddressList[0];
                IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, port);
                Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                sender.NoDelay = true;
                sender.Connect(ipEndPoint);
                _boundary.Notify("Client Socket Connected to " + sender.RemoteEndPoint.ToString());
                return sender;
            }
            catch (Exception e)
            {
                _boundary.Notify(e.ToString());
                return null;
            }
        }

        public override void Disconnect()
        {
            try
            {
                _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
            }
            catch (Exception e)
            {
                _boundary.Notify("Exception: " + e.ToString());
            }
        }
    }
}
