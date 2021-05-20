using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
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

namespace Test_GUI
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MySqlConnection connection = new MySqlConnection(@"Data Source=185.53.232.9;port=3306;Initial Catalog=state_juwx;User Id=db_checker;password='checkerMaster!Jwux'");
        int i;
        public MainWindow()
        {
            InitializeComponent();
            textbox2.Visibility = Visibility.Hidden;
        }


        string cript_pass;
        string hash = "D#@Jusq8SSj=/PUUUS";
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            //encrypt pass and == in DB
            if (textbox2.IsVisible)
            {
                byte[] data = UTF8Encoding.UTF8.GetBytes(textbox2.Text);
                using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
                {
                    byte[] keys = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(hash));
                    using (TripleDESCryptoServiceProvider tripDes = new TripleDESCryptoServiceProvider() { Key = keys, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
                    {
                        ICryptoTransform transform = tripDes.CreateEncryptor();
                        byte[] results = transform.TransformFinalBlock(data, 0, data.Length);
                        cript_pass = Convert.ToBase64String(results, 0, results.Length);
                    }
                }
            }
            else if (passbox1.IsVisible)
            {
                byte[] data = UTF8Encoding.UTF8.GetBytes(passbox1.Password);
                using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
                {
                    byte[] keys = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(hash));
                    using (TripleDESCryptoServiceProvider tripDes = new TripleDESCryptoServiceProvider() { Key = keys, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
                    {
                        ICryptoTransform transform = tripDes.CreateEncryptor();
                        byte[] results = transform.TransformFinalBlock(data, 0, data.Length);
                        cript_pass = Convert.ToBase64String(results, 0, results.Length);
                    }
                }
            }  
            
            //encrypt pass and == in DB


            //textBox3.Text = cript_pass; //out in debager textbox under window

            i = 0;

            //connect to BD and == encrypt pass, username
            connection.Open();

            

            MySqlCommand cmd = connection.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT * FROM `user` WHERE BINARY `username`= '" + textbox1.Text + "' AND `pass`= '" + cript_pass + "'";
            cmd.ExecuteNonQuery();
            DataTable dt = new DataTable();
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            da.Fill(dt);
            i = Convert.ToInt32(dt.Rows.Count.ToString());

            if (i == 0)
            {
                textblock4.Visibility = Visibility.Visible;
                textbox1.Clear();
                passbox1.Clear();
                textbox2.Clear();
            }
            else
            {
                username.Text = textbox1.Text;
                textblock4.Visibility = Visibility.Hidden;
                textbox1.Clear();
                passbox1.Clear();
                if (imgtrue.IsVisible)
                {
                    imgtrue.Visibility = Visibility.Hidden;
                    imgfalse.Visibility = Visibility.Visible;
                    passbox1.Visibility = Visibility.Visible;
                    textbox2.Visibility = Visibility.Hidden;
                }
                else
                grid1.Visibility = Visibility.Hidden;
                grid2.Visibility = Visibility.Visible;
                /*label5.Text = textBox1.Text;
                panel1.Show();
                panel2.Hide();
                label4.Hide();
                textBox1.Clear();
                textBox2.Clear();
                pictureBox2.Image = eyefalse;*/

            }

            connection.Close();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {

            if (imgfalse.IsVisible)
            {
                button2.ToolTip = "Скрыть пароль";
                textbox2.Text = passbox1.Password;
                passbox1.Visibility = Visibility.Hidden;
                textbox2.Visibility = Visibility.Visible;
                imgfalse.Visibility = Visibility.Hidden;
                imgtrue.Visibility = Visibility.Visible;
            }
            else if (imgtrue.IsVisible)
            {
                button2.ToolTip = "Показать пароль";
                passbox1.Password = textbox2.Text;
                passbox1.Visibility = Visibility.Visible;
                textbox2.Visibility = Visibility.Hidden;
                imgtrue.Visibility = Visibility.Hidden;
                imgfalse.Visibility = Visibility.Visible;
            }
        }

        private void btnexit_Click(object sender, RoutedEventArgs e)
        {
            grid1.Visibility = Visibility.Visible;
            grid2.Visibility = Visibility.Hidden;
        }

        string name_bot;
        string statusimg;
        private void btnstat_Click(object sender, RoutedEventArgs e)
        {
            statpanel.Visibility = Visibility.Visible;
            connection.Open();

            string name = "SELECT `name` FROM `discord_bots`";
            string desc = "SELECT `desc` FROM `discord_bots`";
            string status = "SELECT `status` FROM `discord_bots`";

            MySqlCommand cmd = new MySqlCommand(name, connection);
            tb_name.Text = cmd.ExecuteScalar().ToString();
            MySqlCommand cmd1 = new MySqlCommand(desc, connection);
            tb_desc.Text = cmd1.ExecuteScalar().ToString();
            MySqlCommand cmd2 = new MySqlCommand(status, connection);
            statusimg = cmd2.ExecuteScalar().ToString();
            if (statusimg == "1")
            {
                tb_statusgreen.Visibility = Visibility.Visible;
            }
            else if (statusimg == "0")
            {
                tb_statusred.Visibility = Visibility.Visible;
            }
            connection.Close();
        }
    }
}
