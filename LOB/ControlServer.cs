using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net.Sockets;
using System.Net;

namespace MicrophoneProject.LOB
{
    public class ControlServer : Control
    {
        public ControlServer(string host, short port, IBoundary boundary) : base(host, port, boundary) { }

        protected override SocketPermission GetPermission()
        {
            SocketPermission permission = new SocketPermission
                    (
                        NetworkAccess.Accept, // changes
                        TransportType.Tcp,
                        _host,
                        SocketPermission.AllPorts
                    );
            try
            {
                
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
                IPHostEntry host = Dns.GetHostEntry(_host);
                IPAddress address = host.AddressList[0];
                IPEndPoint ipEndPoint = new IPEndPoint(address, port); // OUTPUT POINT OF THE SOCKET CONNECTION FLOW
                Socket listener = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                listener.Bind(ipEndPoint);
                listener.Listen(10);
                _boundary.Notify("Waiting for a Connection on port " + ipEndPoint);
                AsyncCallback ascallback = new AsyncCallback(AcceptCallback);
                listener.BeginAccept(ascallback, listener);
                return listener;
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
                _socket.Shutdown(SocketShutdown.Receive);
                _socket.Close();
            }
            catch (Exception e)
            {
                _boundary.Notify("Exception: " + e.ToString());
            }
        }

        public void AcceptCallback(IAsyncResult result)
        {
            Socket listener = null;
            Socket handler = null;
            try
            {
                byte[] buffer = new byte[1024];
                listener = (Socket)result.AsyncState;
                handler = listener.EndAccept(result);
                handler.NoDelay = true;
                object[] obj = new object[2];
                obj[0] = buffer;
                obj[1] = handler;

                handler.BeginReceive
                    (
                        buffer,
                        0,
                        buffer.Length,
                        SocketFlags.None,
                        new AsyncCallback(ReceiveCallback), 
                        obj
                    );

                AsyncCallback aCallback = new AsyncCallback(AcceptCallback);
                listener.BeginAccept(aCallback, listener);
            }
            catch (Exception e)
            {
                _boundary.Notify("Exception: " + e.ToString());
            }
        }

        public static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                object[] obj = new object[2];
                obj = (object[])ar.AsyncState;

                byte[] buffer = (byte[])obj[0];
                Socket handler = (Socket)obj[1];

                string msg = string.Empty;
                int numberBytes = handler.EndReceive(ar);

                if (numberBytes > 0)
                {
                    msg += Encoding.Unicode.GetString(buffer, 0, numberBytes);
                    // If message contains "<Client Quit>", finish receiving 
                    if (msg.IndexOf("<Client Quit>") > -1)
                    {
                        // Convert byte array to string 
                        string str =
                            msg.Substring(0, msg.LastIndexOf("<Client Quit>"));
                        Console.WriteLine(
                            "Read {0} bytes from client.\n Data: {1}",
                            str.Length, str);

                        // Prepare the reply message 
                        byte[] byteData =
                            Encoding.Unicode.GetBytes(str);

                        // Sends data asynchronously to a connected Socket 
                        handler.BeginSend(byteData, 0, byteData.Length, 0,
                            new AsyncCallback(ReceiveCallback), handler);
                    }
                    else
                    {
                        // Continues to asynchronously receive data 
                        byte[] buffernew = new byte[1024];
                        obj[0] = buffernew;
                        obj[1] = handler;
                        handler.BeginReceive(buffernew, 0, buffernew.Length,
                            SocketFlags.None,
                            new AsyncCallback(ReceiveCallback), obj);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: {0}", ex.ToString());
            }
        } 

    }
}
