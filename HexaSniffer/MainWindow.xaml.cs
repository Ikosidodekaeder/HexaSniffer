using HexaSniffer.GUI;
using HexaSniffer.Hexa;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HexaSniffer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region WindowSpecifics
        string WindowTitle = "";
        public delegate void UpdateTextCallback(string message);

        void UpdateText(object content)
        {
            RawLog.Dispatcher.Invoke
            (
                new UpdateTextCallback
                (
                    msg => {
                        RawLog.Text += msg;
                    }
                ),
                new object[] { content }
            );
        }
        #endregion

        #region NetWorkSpecificsd
        ConcurrentQueue<PacketData> concurrentQueue = new ConcurrentQueue<PacketData>();
        Thread thread;
        #endregion

        #region ViewSpecifics
        AvailablePackets availablePackets = new AvailablePackets();
        #endregion

        

        public MainWindow()
        {
            InitializeComponent();
            SelectPacket.ItemsSource = typeof(PacketTypes).GetEnumNames().ToList();
            
        }




        #region Events
        private void connect_Click(object sender, RoutedEventArgs e)
        {
            string ip_text = IPAddress.Text;
            string port_text = Port.Text;

            if (ip_text.Length == 0)
                ip_text = "svdragster.dtdns.net";
            if (port_text.Length == 0)
                port_text = "25565";


            if (thread != null)
            {
                /*
                 * Stop old thread so it destroys old TCP connection
                 */
                thread.Abort();
            }

            /*
             * Create new  Thread which will contain the TCP connection to the
             * server
             */
            #region NetworkThread
            thread = new Thread(() =>
            {
                string register = "1;550e8400-e29b-11d4-a716-446655440000;Raum " + 1 + ";0";
                string keepalive = "0;550e8400-e29b-11d4-a716-446655440000;0";
                string server_list = 0x09 + ";550e8400-e29b-11d4-a716-446655440000;0";
                byte[] buffer = new byte[256];

                TcpClient ClientConnection;
                NetworkStream Stream;

                int port = int.Parse(port_text);
                ClientConnection = new TcpClient(ip_text, port);
                Stream = ClientConnection.GetStream();


                Stream.Write(buffer: Encoding.ASCII.GetBytes(server_list), offset: 0, size: server_list.Length);


                UpdateText("## Connected to " + ip_text + "\n");


                try
                {
                    while (true)
                    {
                        /*
                         * IF our concurrent queue has elements in it try to send them.
                         * 
                         */
                        //Thread.BeginCriticalRegion();
                        if (!concurrentQueue.IsEmpty)
                        {
                            PacketData tmp = null; ;
                            while (concurrentQueue.TryDequeue(out tmp))
                            {
                                if (tmp != null)
                                    try
                                    {
                                        if (Stream.CanWrite && ClientConnection.Connected)
                                            Stream.Write(Encoding.ASCII.GetBytes(tmp.ToString()), 0, tmp.ToString().Length);
                                    }
                                    catch (SocketException socketexception)
                                    {
                                        UpdateText("## " + socketexception.StackTrace);
                                    }
                                    catch (IOException ioexception)
                                    {
                                        UpdateText("## " + ioexception.StackTrace);
                                    }

                            }
                        }
                        //Thread.EndCriticalRegion();


                        /*
                         * Check for any possible answer by simple try to read any number of bytes
                         * if the number is zero we obvisuouly have not read anything.
                         */
                        int BytesRead = 0;
                        try
                        {
                            if (Stream.CanRead && ClientConnection.Connected)
                                BytesRead = Stream.Read(buffer, 0, buffer.Length);
                        }
                        catch (SocketException socketexception)
                        {
                            UpdateText("## " + socketexception.StackTrace);
                        }
                        catch (IOException ioexception)
                        {
                            UpdateText("## " + ioexception.StackTrace);
                        }

                        if (BytesRead > 0)
                        {
                            UpdateText(Encoding.ASCII.GetString(buffer.Take(BytesRead).ToArray()));
                        }

                    }
                }
                catch (ThreadAbortException ex)
                {
                    RawLog.Dispatcher.Invoke
                    (
                           new UpdateTextCallback(msg => {
                               RawLog.Text += "\n ############### \n";
                               RawLog.Text += msg;
                               RawLog.Text += "\n ############### \n";
                           }),
                           new object[] { ex.StackTrace }
                    );

                    Stream.Close();
                    ClientConnection.Close();
                    ClientConnection.Dispose();
                    ClientConnection = null;
                    Stream = null;

                }
            });
            #endregion

            thread.Start();
        }

        private void disconnect_Click(object sender, RoutedEventArgs e)
        {
            if (thread != null)
                thread.Abort();
        }

        private void serverliste_Click(object sender, RoutedEventArgs e)
        {
            //Thread.BeginCriticalRegion();
            concurrentQueue.Enqueue(new PacketData());
            //Thread.EndCriticalRegion();
        }


        private void SelectPacket_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            string value = comboBox.SelectedItem as string;
            Title = WindowTitle = "Selected: " + value;
        }
        #endregion

        private void Send_Packet_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
