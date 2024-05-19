using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;

namespace LAB4
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        const int H0_start = 100;
        bool cipher_f;
        int fast_exp(int Base, int mod, int exp)
        {
            int res = Base;
            int x = 1;
            while (exp != 0)
            {
                while (exp % 2 == 0)
                {
                    exp /= 2;
                    res = (res * res) % mod;
                }
                if (exp > 0)
                {
                    exp--;
                    x = (res * x) % mod;
                }

            }
            return x;
        }
        private bool is_Prime(int a)
        {
            if (a == 2)
            {
                return true;
            }
            if ((a & 1) == 0)
            {
                return false;
            }
            int c = (int)(Math.Sqrt(a) + 0.5);
            int d = 3;
            while (d < c)
            {
                if (a % d == 0)
                {
                    return false;
                }
                d += 2;
            }
            return true;
        }
        private int itt_Hash(int H0, int M, int Mo)
        {
            return (H0 + M) * (H0 + M) % Mo;
        }

        private int Hash_count(byte[] data, int Mod,int len)
        {
            int H = H0_start;
            for (int i = 0; i < len; i++)
            {
                H = itt_Hash(H, data[i], Mod);
            }
            return H;
        }
        private int Eucl_alg(int a, int b)
        {
            while (a != 0 && b != 0)
            {
                if (a > b)
                {
                    a %= b;
                }
                else
                {
                    b %= a;
                }
            }
            return a == 0 ? b : a;
        }

        private void factorize(int m, List<int> prim_div)
        {
            for (int i = 2; i < Math.Sqrt(m); i++)
            {
                if (m % i == 0)
                {
                    prim_div.Add(i);
                    while (m % i == 0)
                    {
                        m /= i;
                    }
                }
            }
            if (m != 1)
            {
                prim_div.Add(m);
            }
        }

        private void find_primitive(List<int> primitives, int p)
        {
            int Eul = p - 1;
            List<int> prim_div = new List<int>();
            factorize(Eul, prim_div);
            for (int i = 2; i < Eul; i++)
            {
                if (fast_exp(i, p, Eul) != 1 || Eucl_alg(p, i) != 1 || fast_exp(i, p, 1) == 1)
                {
                    continue;
                }
                else
                {
                    bool not_right = false;
                    foreach (int val in prim_div)
                    {
                        if (fast_exp(i, p, Eul / val) == 1)
                        {
                            not_right = true;
                            break;
                        }
                    }
                    if (!not_right)
                    {
                        primitives.Add(i);
                    }
                }
            }
        }


        private int mainLog()
        {
            List<int> primitives = new List<int>();
            int k = Convert.ToInt32(K.Text);
            int q = Convert.ToInt32(Q.Text);
            int p = Convert.ToInt32(P.Text);
            int x = Convert.ToInt32(X.Text);
            int h = Convert.ToInt32(H.Text);
            Output.Text = "";
            if(p<=1 || q <= 1)
            {
                Output.Text = "no less then 2 please";
                return 0;
            }
            if ((p - 1) % q != 0)
            {
                Output.Text = "q is not divisor of p-1";
                return 0;
            }
            if (!is_Prime(q) || !is_Prime(p))
            {
                Output.Text = "q or p is not prime";
                return 0;
            }
            int g = fast_exp(h, p, (p - 1) / q);
            int Ko = fast_exp(g, p, x);
            string cipher_str;
            cipher_str = File_name.Text.Replace("\r\n", string.Empty);
            string pattern = "[А-Яа-я]";
            cipher_str = Regex.Replace(cipher_str, pattern, "");
            byte[] data = new byte[100000];
            FileInfo info = new FileInfo(cipher_str);
            int len = (int)info.Length;
            using (FileStream reader = new FileStream(cipher_str, FileMode.Open, FileAccess.Read))
            {
                reader.Read(data, 0, 100000);
            }
            
            if (cipher_f)
            {

                Output.Text = File.ReadAllText(cipher_str);

                data[len] = 10;
                int hash_val = Hash_count(data, q, len);
                int r = fast_exp(g, p, k) % q;
                int s = fast_exp(k, q, q-2);
                s = (s * (hash_val + x * r)) % q;
                if (r == 0 || s == 0)
                {
                    Output.Text = "please, enter different k";
                    return 0;
                }
                string val = r.ToString();
                Hash_val.Text +="r= "+ r.ToString()+" s=";
                for (int i=0; i < val.Length; i++)
                {
                    len++;
                    data[len] = (byte)val[i];
                }
                val = s.ToString();
                Hash_val.Text += s.ToString();
                ++len;
                data[len] = (byte)' ';
                for (int i = 0; i < val.Length; i++)
                {
                    len++;
                    data[len] = (byte)val[i];
                }
                len++;
                string ciphered;
                ciphered = File_ciph.Text;            
                using (FileStream writer = new FileStream(ciphered, FileMode.Create, FileAccess.Write))
                {
                    writer.Flush();
                    writer.Write(data, 0, len);

                }
            }
            else
            {
               Output.Text= File.ReadAllText(cipher_str);
                int i = len;
                int i_=len;
                while (true)
                {
                    if (data[i]==' ')
                    {
                        i_ = i;
                    }
                    i--;
                    if (data[i] == 10)
                    {
                        break;
                    }
                    if (i == 0)
                    {
                        break;
                    }
                }
                if (i == 0 && data[0]!=10)
                {
                    Output.Text = "there is no signing";
                    return 0;
                }
                StringBuilder s_str=new StringBuilder();
                for(int j=i_+1; j<len; j++)
                {
                   s_str.Append((char)data[j]);
                }
                StringBuilder r_str = new StringBuilder();
                for (int j = i+1; j < i_; j++)
                {
                    r_str.Append((char)data[j]);
                }
                string r_ = r_str.ToString();
                int r = Convert.ToInt32(r_);
                r_ = s_str.ToString();
                int s = Convert.ToInt32(r_);
                len = i;
                int hash_val = Hash_count(data, q, len);
                //   int r= 1; //change here. need to somehow read data from file and divide it propperly
                //  int s= 1;
                int w = fast_exp(s, q, q - 2);
                int e1 = (hash_val * w) % q;
                int e2 = (r * w) % q;
               int v = ((fast_exp(g, p, e1) * fast_exp(Ko, p, e2)) % p) % q;
                if (v == r)
                {
                    Output.Text += "\n верно";
                }
                else
                {
                    Output.Text += "\n неверно"; 
                }
                Hash_val.Text = "v="+v.ToString()+" r="+r.ToString();
            }
            // find_primitive(primitives, a);
            return 0;
        }

        private void start_Click(object sender, RoutedEventArgs e)
        {
            cipher_f = true;
            mainLog();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            cipher_f = false;
            mainLog();
        }
    }


}

