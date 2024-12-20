using System;
using Gtk;
using SendingKeyEDS;
using UI = Gtk.Builder.ObjectAttribute;

namespace TestGtkApp
{
    public class Decoder : Window
    {
        [UI] public Label chat_label = null;
        [UI] private Button ToChats = null;
        [UI] private Button Sender = null;
        [UI] private Entry TextBox = null;

        public Decoder() : this(new Builder("OwnerChat1.glade")) { }

        private Decoder(Builder builder) : base(builder.GetRawOwnedObject("MainWindow"))
        {
            builder.Autoconnect(this);
            DeleteEvent += Window_DeleteEvent;
            Sender.Clicked += Sender_Clicked;
            ToChats.Clicked += ToChats_Clicked;
        }

        private void Window_DeleteEvent(object sender, DeleteEventArgs a)
        {
            Application.Quit();
        }

        private void Sender_Clicked(object sender, EventArgs a)
        {
            string mess = TextBox.Text;
            TextBox.Text = "";
            if(!mess.Equals(""))
            {
                chat_label.Text = GenerateKey.Decrypt(mess);
            }
            else {}
        }
        
        private void ToChats_Clicked(object sender, EventArgs a)
        {
            Destroy();
            OwnerChat mainWindow = [];
            mainWindow.ShowAll();
            Application.Run();
        }
    }
}