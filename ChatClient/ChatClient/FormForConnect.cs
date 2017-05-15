using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatClient
{

    public partial class FormForConnect : Form
    {

        public FormForConnect()
        {
            this.Size = new Size(400, 280);


            var nickNameLabel = new Label
            {
                Location = new Point(10, 10),
                Size = new Size(ClientSize.Width, 30),
                Text = "Entrer your name:",
            };
            Controls.Add(nickNameLabel);


            var nickNameBox = new TextBox();
            nickNameBox.Location = new Point(10, nickNameLabel.Bottom);
            nickNameBox.Size = new Size(365, 20);
            Controls.Add(nickNameBox);

            var IPLabel = new Label
            {
                Location = new Point(10, nickNameBox.Bottom + 10),
                Size = new Size(ClientSize.Width, 30),
                Text = "Entrer IP of server:",
            };
            Controls.Add(IPLabel);

            var IPBox = new TextBox();
            IPBox.Location = new Point(10, IPLabel.Bottom);
            IPBox.Size = new Size(365, 20);
            Controls.Add(IPBox);

            var portLabel = new Label
            {
                Location = new Point(10, IPBox.Bottom + 10),
                Size = new Size(ClientSize.Width, 30),
                Text = "Entrer port:",
            };
            Controls.Add(portLabel);

            var portBox = new TextBox();
            portBox.Location = new Point(10, portLabel.Bottom);
            portBox.Size = new Size(365, 20);
            Controls.Add(portBox);

            var connectButton = new Button
            {
                Location = new Point(90, portBox.Bottom + 15),
                Size = new Size(200, 30),
                Text = "Connect"
            };
            Controls.Add(connectButton);


            connectButton.Click += (sender, args) =>
            {
                this.Hide();

                Client.ClientTcp = new TcpClient();
                try
                {
                    Client.UserName = nickNameBox.Text;
                    Client.Host = IPBox.Text;
                    Client.Port = Convert.ToInt32(portBox.Text);
                    Client.ClientTcp.Connect(Client.Host, Client.Port);
                    Client.Stream = Client.ClientTcp.GetStream();


                    string message = Client.UserName;
                    byte[] data = Encoding.Unicode.GetBytes(message);
                    Client.Stream.Write(data, 0, data.Length);


                    Thread receiveThread = new Thread(new ThreadStart(Client.ReceiveMessage));
                    receiveThread.Start();
                    var formForMessages = new FormForMessages();
                    formForMessages.ShowDialog();


                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    Client.Disconnect();
                }

            };


            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
        }
    }
}