using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
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
        int min = 255;
        int max = 0;
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

        private bool is_Prime(int a)
        {
            if (a == 2)
            {
                return true;
            }
            if ((a & 1)==0)
            {
                return false;
            }
            int c = (int)(Math.Sqrt(a) + 0.5);
            int d = 3;
            while (d < c){
                if (a % d == 0)
                {
                    return false;
                }
                d += 2;
            }
            return true;
        }
        int fast_exp(int Base, int mod, int exp)
        {
            int res = Base;
            int x = 1;
            while (exp != 0)
            {
                while (exp % 2 == 0)
                {
                    exp /= 2;
                    res=(res*res)%mod ;
                }
                if (exp > 0) { 
                exp--;
                x = (res*x) % mod;
                 }
                
            }
            return x;
        }

        unsafe int Euclid(int a, int b, int *y1)
        {
            int d0 = a;
            int d1 = b;
            int x0 = 1;
            int x1 = 0;
            int y0 = 0;
             *y1 = 1;
            while (d1>1)
            {
                int q = d0 / d1;
                int d2 = d0 % d1;
                int x2 = x0 - q * x1;
                int y2 = y0 - q * (*y1);
                d0= d1;
                d1 = d2;
                x0 = x1;
                x1 = x2;
                y0 = *y1; 
                *y1 = y2;
            }
            d1 = *y1 *b  + x1 * a;
            if (*y1 < 0)
            {
                *y1 += a;
            }
            return d1;

        }

        int ciph(byte[] data, short[] res, int len,int key, int n) {
            int amount_Symb = 0;
            for (int j = 0; j < len; j++)
            {
                res[j] = (short)fast_exp(data[j], n, key);
                amount_Symb++;
                if (j < 200)
                {
                    ToCipher_dec.Text += data[j] + " ";
                    Ciphered_dec.Text += res[j] + " ";
                }

            }
            return amount_Symb;
        }

        int deciph(short[] data, byte[] res, int len, int key, int n)
        {
            int amount_Symb = 0;
            for (int j = 0; j < len; j++)
            {

                res[j] = (byte)fast_exp(data[j], n, key);
                amount_Symb++;
                if (j < 200)
                {
                    ToCipher_dec.Text += data[j]+ " ";
                    Ciphered_dec.Text += res[j] + " ";
                }

            }
            return amount_Symb;
        }

        bool flag=true;
        bool decipher = false;
        public unsafe int mainLog()
        {
            flag = true;
            int p_ = 0;
            int q_ = 0;
            int Kc_base = 0;
            ToCipher_dec.Text = "";
            Ciphered_dec.Text = "";
            string cipher_str;
            cipher_str = Kc.Text.Replace("\r\n", string.Empty);
            string pattern = "[A-Za-zА-Яа-я]";
            cipher_str = Regex.Replace(cipher_str, pattern, "");
            Kc_base = Convert.ToInt32(cipher_str);

            p_ = Convert.ToInt32(p.Text);
            q_ = Convert.ToInt32(q.Text);
            if(!is_Prime(p_)|| !is_Prime(q_))
            {
                flag = false;
                ToCipher_dec.Text = "не простые q и p";
            }
            int fi = (q_ - 1) * (p_ - 1);
            int Ko_base = 0;
            if (Euclid(fi, Kc_base,&Ko_base) != 1 &&flag)
            {
                flag = false;
                ToCipher_dec.Text = "не взаимно простые";

            }
            if (p_ * q_ > 65535)
            {
                flag = false;
                ToCipher_dec.Text = "слишком большой модуль";
            }

            if (flag)
            {
                Ko.Text = Ko_base.ToString();
                string toCipher = ToCipher.Text;
                string ciphered = Ciphered.Text;
                byte[] data = new byte[100000];
                byte[] res = new byte[100000];
                int amount_Symb = 0;
                FileInfo info = new FileInfo(toCipher);
                int len = (int)info.Length;
                using (FileStream reader = new FileStream(toCipher, FileMode.Open, FileAccess.Read))
                {
                    reader.Read(data, 0, 100000);
                }
                for(int i=0; i<len; i++)
                {

                    if (max < data[i])
                    {
                        max = data[i];
                    }
                }
                if (max > p_ * q_)
                {
                    ToCipher_dec.Text = "мал диапазон";
                }
                else
                {
                    int key;
                    if (decipher)
                    {
                        key = Kc_base;
                        short[] sdata = new short[50000];
                        Buffer.BlockCopy(data, 0, sdata, 0, data.Length);
                        len /= 2;
                        amount_Symb =deciph(sdata, res, len, key, p_ * q_);
                    }
                    else
                    {
                        var short_res = Array.ConvertAll(res, b => (short)b);
                        key = Ko_base;
                        amount_Symb=ciph(data, short_res, len, key, p_ * q_);                   
                        int k = 0;
                        for(int i=0; i < len * 2; i+=2)
                        {

                            byte[] conv = BitConverter.GetBytes(short_res[k]);
                            res[i] = conv[0];
                            res[i + 1] = conv[1];
                            k++;
                        }
                        len *= 2;
                    }
                }




                using (FileStream writer = new FileStream(ciphered, FileMode.Open, FileAccess.Write))
                {
                    writer.Flush();
                    writer.Write(res, 0, len);
                }
                amountSymbols.Text = amount_Symb.ToString();        
                str.Clear();


            }
           
            return 0;
        }
       private void Cipher_Click(object sender, RoutedEventArgs e)
        {
            decipher = false;
            mainLog();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            decipher = true;
            mainLog();
        }
    }
 
}


