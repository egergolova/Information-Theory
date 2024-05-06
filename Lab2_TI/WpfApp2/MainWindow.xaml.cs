using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace WpfApp2
{
  
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        StringBuilder str = new StringBuilder();
        string keys = "key_history.txt";
        int cipher_or = 0; //original
         double maxVal = Math.Pow(2, 26);//67108864*2;
        int[] bits = { 1, 7, 8 } ;
        const int size_reg = 26;
        public MainWindow()
        {
            InitializeComponent();
        }
       private string bin_transition(int a, int size)
        {
            char[] c = new char[size];
            for (int i = 0; i < size; i++)
            {

                char b = (char)(a & 1);
                c[size - i - 1] = (char)(b + '0');
                a = a >> 1;
            }

             StringBuilder str1 = new StringBuilder();
            for(int i=0; i<c.Length; i++)
            {
                str1.Append(c[i]);
            }
            string s = str1.ToString();
            return s;

        }

        private void change_key()
        {
            int cipher = cipher_or;
            int MaxVal = (int)maxVal;
            cipher_or = (cipher_or << 1) % MaxVal;
            char low = (char)((cipher << 1) / MaxVal);
            for (int i = 0; i < bits.Length; i++)
            {
                low ^= (char)(cipher >> (bits[i]-1));
            }

            low &= (char)1;
            cipher_or ^= low;
        }
        bool flag=true;
        public int mainLog()
        {
            ToCipher_bin.Text = "";
            Ciphered_bin.Text = "";
            string cipher_or_str;
            cipher_or_str = Key.Text.Replace("\r\n", string.Empty);
            string pattern = "[A-Za-zА-Яа-я2-9 ]";

                cipher_or_str = Regex.Replace(cipher_or_str, pattern, "");
            if (cipher_or_str.Length != 26)
            {
                flag = false;
                Key.Text = "wrong value";
            }
            if (flag)
            {
                for (int i = 0; i < cipher_or_str.Length; i++)
                {
                    cipher_or = cipher_or << 1;
                    cipher_or += cipher_or_str[i] - '0';
                }
                string toCipher = ToCipher.Text;
                string ciphered = Ciphered.Text;
                byte[] data = new byte[100000];
                FileInfo info = new FileInfo(toCipher);
                int len = (int)info.Length;
                using (FileStream reader = new FileStream(toCipher, FileMode.Open, FileAccess.Read))
                {
                    reader.Read(data, 0, 100000);
                }
                int cipher = cipher_or;
                byte byte_cipher = (byte)0;
                int counter = 0;
                byte a;
                int j = 0;
                int amount_Symb = 0;
                byte[] res = new byte[100000];
                while (j < len)
                {
                    string s = bin_transition(cipher, size_reg);
                    str.Append(s);
                    str.Append("\n");
                    while (counter + 8 < size_reg && j < len)
                    {
                        byte_cipher |= (byte)cipher;
                        a = data[j];
                        if (ToCipher_bin.Text.Length < 500)
                        {
                            ToCipher_bin.Text += bin_transition(a, 8);
                        }
                        j++;
                        a ^= byte_cipher;
                        if (Ciphered_bin.Text.Length < 500)
                        {
                            Ciphered_bin.Text += bin_transition(a, 8);

                        }
                        res[amount_Symb] = a;
                        cipher = cipher >> 8;
                        amount_Symb++;
                        counter += 8;
                        byte_cipher = (byte)0;
                    }
                    if (j < len)
                    {
                        byte_cipher = (byte)0;
                        byte_cipher |= (byte)cipher;
                        change_key();
                        cipher = cipher_or;
                        byte_cipher |= (byte)(cipher >> (size_reg - counter));
                        a = data[j];
                        if (ToCipher_bin.Text.Length < 500)
                        {
                            ToCipher_bin.Text += bin_transition(a, 8);
                        }
                        j++;
                        a ^= byte_cipher;
                        if (Ciphered_bin.Text.Length < 500)
                        {
                            Ciphered_bin.Text += bin_transition(a, 8);
                        }

                        res[amount_Symb] = a;

                        counter = 8 - (size_reg - counter);
                        amount_Symb++;
                    }
                }

                using (FileStream writer = new FileStream(ciphered, FileMode.Open, FileAccess.Write))
                {
                    writer.Write(res, 0, len);
                }
                amountSymbols.Text = amount_Symb.ToString();
                using (StreamWriter writer = new StreamWriter(keys))
                {
                    writer.Flush();
                    writer.Write(str);
                }
                str.Clear();

                cipher_or = 0;
              
            }
            return 0;
        }
       private void Cipher_Click(object sender, RoutedEventArgs e)
        {
            mainLog();
        }
    }
 
}


