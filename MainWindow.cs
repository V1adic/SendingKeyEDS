using System;
using System.Linq;
using System.Numerics;
using Gtk;
using SendingKeyEDS;
using Elliptic_curve;
using UI = Gtk.Builder.ObjectAttribute;

namespace TestGtkApp
{
    class MainWindow : Window
    {
        [UI] private Label _label1 = null;
        [UI] private Label _label2 = null;
        [UI] private Button _button1 = null;
        [UI] private Entry TextBox = null;

        public MainWindow() : this(new Builder("MainWindow.glade")) { }

        private MainWindow(Builder builder) : base(builder.GetRawOwnedObject("MainWindow"))
        {
            builder.Autoconnect(this);
            DeleteEvent += Window_DeleteEvent;
            _button1.Clicked += Button1_Clicked;

            BigInteger d;
            do
            {
                byte[] buffer = new byte[32];
                GenerateKey.GetRng.GetBytes(buffer);
                d = new BigInteger(buffer, true);

            } while (d > GenerateKey.GetN);

            GenerateKey.SetKey(d);

            Point Q = GenerateKey.GetP * d;
            _label2.Text = $"Q is -> \n{Q}";
        }

        private void Window_DeleteEvent(object sender, DeleteEventArgs a)
        {
            Application.Quit();
        }

        private void Button1_Clicked(object sender, EventArgs a)
        {
            string Key = TextBox.Text;
            if(!Key.Equals(""))
            {
                var val = Key.Split(' ');
                if(val.Length == 2)
                {
                    if(val[0].Trim().All(char.IsDigit) && val[1].Trim().All(char.IsDigit))
                    {
                        _label1.Text = "Проверка пройдена";
                        GenerateKey.Generate(BigInteger.Parse(val[0].Trim()), BigInteger.Parse(val[1].Trim()));
                        Destroy();
                        OwnerChat mainWindow = [];
                        mainWindow.ShowAll();
                        Application.Run();
                    }
                    else
                    {
                        _label1.Text = "Не прошло валидацию";
                    }
                }
                else
                {
                    _label1.Text = "Не прошло валидацию";
                }
            }
            else
            {
                _label1.Text = "Ключ не должен быть пустым";
            }
        }
    }
}
