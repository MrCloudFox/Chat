using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatClient
{

    class FormForMessages : Form
    {
        private readonly ListView listOfMessages = new ListView();
        private readonly ListView listOfParticipants = new ListView();

        public FormForMessages()
        {

            this.Size = new Size(670, 510);


            listOfMessages.Bounds = new Rectangle(new Point(40, 40), new Size(400, 370));

            listOfMessages.View = View.Details;
            listOfMessages.LabelEdit = true;
            listOfMessages.AllowColumnReorder = true;
            listOfMessages.FullRowSelect = true;
            listOfMessages.GridLines = true;
            listOfMessages.Sorting = SortOrder.Ascending;

            listOfMessages.Columns.Add("Participant", -2, HorizontalAlignment.Left);
            listOfMessages.Columns.Add("Message", -2, HorizontalAlignment.Left);

            Controls.Add(listOfMessages);


            listOfParticipants.Bounds = new Rectangle(new Point(470, 40), new Size(150, 370));

            listOfParticipants.View = View.Details;
            listOfParticipants.LabelEdit = true;
            listOfParticipants.AllowColumnReorder = true;
            listOfParticipants.FullRowSelect = true;
            listOfParticipants.GridLines = true;
            listOfParticipants.Sorting = SortOrder.Ascending;

            listOfParticipants.Columns.Add("Participants", -2, HorizontalAlignment.Left);

            Controls.Add(listOfParticipants);


            var enterMeassageBox = new TextBox();
            enterMeassageBox.Location = new Point(40, listOfMessages.Bottom);
            enterMeassageBox.Size = new Size(400, 100);
            Controls.Add(enterMeassageBox);


            var sendButton = new Button
            {
                Location = new Point(290, enterMeassageBox.Bottom),
                Size = new Size(150, 30),
                Text = "Send."
            };
            Controls.Add(sendButton);


            sendButton.Click += (sender, args) =>
            {
                Client.SendMessage(enterMeassageBox.Text);
            };


            var disconnectButton = new Button
            {
                Location = new Point(40, enterMeassageBox.Bottom),
                Size = new Size(150, 30),
                Text = "Disconnect."
            };
            Controls.Add(disconnectButton);


            disconnectButton.Click += (sender, args) =>
            {
                Client.SendMessage("~Disconnect");
            };


            Client.MessageReceived += AddToWindow;
            Client.ParticipantConnected += AddToParticipantList;
            Client.ParticipantDisconnected += DeleteFromParticipantList;
            

        }


        public void AddToWindowOfParticipantInThread(string nickName)
        {
            BeginInvoke(new Action(() =>
            {
                listOfParticipants.Items.Add(nickName);
            }));

        }


        public void AddToParticipantList(object sender, ParticipantEventArgs e)
        {
            new Action<string>(AddToWindowOfParticipantInThread).BeginInvoke(e.NickNameOfParticiapant, null, null);
        }


        public void AddToWindowInThread(string nickName ,string message)
        {
            BeginInvoke(new Action(() =>
            {
                ListViewItem currentMessage = new ListViewItem(nickName);
                currentMessage.SubItems.Add(message);
                listOfMessages.Items.Add(currentMessage);
            }));

        }


        public void AddToWindow(object sender, MessageEventArgs e)
        {
            new Action<string, string>(AddToWindowInThread).BeginInvoke(e.NickName, e.Message, null, null);
        }


        public void DeleteFromParticipantInThread(string nickName)
        {
            BeginInvoke(new Action(() =>
            {
                for(int i = 0; i < listOfParticipants.Items.Count; i++)
                {
                    if(Convert.ToString(listOfParticipants.Items[i]) == nickName)
                    {
                        listOfParticipants.Items.RemoveAt(i);
                        break;
                    }
                }
            }));

        }


        public void DeleteFromParticipantList(object sender, ParticipantEventArgs e)
        {
            new Action<string>(DeleteFromParticipantInThread).BeginInvoke(e.NickNameOfParticiapant, null, null);
        }

    }
}