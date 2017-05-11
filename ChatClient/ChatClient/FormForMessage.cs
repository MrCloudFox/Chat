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

        public FormForMessages()
        {

            this.Size = new Size(500, 500);


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

            Client.MessageReceived += AddToWindow;

        }


        public void AddToWindowInThread(String message)
        {
            BeginInvoke(new Action(() =>
            {
                ListViewItem currentMessage = new ListViewItem(Client.UserName);
                currentMessage.SubItems.Add(message);
                listOfMessages.Items.Add(currentMessage);
            }));

        }


        public void AddToWindow(object sender, MessageEventArgs e)
        {
            new Action<string>(AddToWindowInThread).BeginInvoke(e.Message, null, null);
        }


    }
}