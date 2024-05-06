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
using static System.Net.Mime.MediaTypeNames;

namespace WpfApp1
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

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }








        private void Table_sort(string key, int[] arInd)
        {
            //string key_copy;
            //copy(key.begin(), key.end(), std::back_inserter(key_copy));
            StringBuilder keyForSort = new StringBuilder(key);

            for (int i = 0; i < key.Length; i++)
            {
                arInd[i] = i;
            }
            for (int i = 0; i <= key.Length - 1; i++)
            {
                int curr_min = i;
                for (int j = i + 1; j < key.Length; j++)
                {
                    if (keyForSort[curr_min] > keyForSort[j])
                    {
                        curr_min = j;
                    }
                }
                int buff_ = arInd[curr_min];
                arInd[curr_min] = arInd[i];
                arInd[i] = buff_;

                char buff = keyForSort[curr_min];
                keyForSort[curr_min] = keyForSort[i];
                keyForSort[i] = buff;
            }
        }



        private string Cipher_Table(string ToCipher, string key, string outputPath)
        {
            using (StreamWriter writer = new StreamWriter(outputPath))
            {
                if (key.Length > ToCipher.Length || key.Length == 0)
                {
                    cipheredText.Text = ToCipher;
                    return "";
                }

                string pattern = "[А-Яа-я0-9 ]";
                ToCipher = Regex.Replace(ToCipher, pattern, "");
                ToCipher=ToCipher.ToLower();
                key=key.ToLower();
                key = Regex.Replace(key, pattern, "");
                string new_key = "";

                if (key.Length > ToCipher.Length || key.Length == 0)
                {
                    cipheredText.Text = ToCipher;
                    return "";
                }

                int[] arInd = new int[key.Length];
                Table_sort(key, arInd);
                string str = "";
                for (int i = 0; i < key.Length; i++)
                {
                    int j = 0;
                    while (j * key.Length + arInd[i] < ToCipher.Length)
                    {
                        str += ToCipher[j * key.Length + arInd[i]];
                        j++;
                    }
                }
                cipheredText.Text = str;
                writer.WriteLine(key);
                writer.Write(str);
                return str;
            }
        }

        private string Cipher_Vigenere(string key, string text, string outputPath)
        {                                                 // filename
            if ( key.Length == 0)
            {
                cipheredText.Text = text;
                // std::cout << Text << "\n";
                return "";
            }
            string pattern = "[A-Za-z0-9 ]";
            text=text.ToLower();
            text = Regex.Replace(text, pattern, "");
            key=key.ToLower();
            key = Regex.Replace(key, pattern, "");
            //cout << "\n" << new_key << "\n";
            if ( key.Length == 0)
            {
                cipheredText.Text = text;
                //std::cout << ToCipher << "\n";
                return "";
            }

            using (StreamWriter writer = new StreamWriter(outputPath))
            {
                const string RusAlph = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";
                int posKey = 0;
                int indOff = 0;
                cipheredText.Text = "";
                StringBuilder forWrite = new StringBuilder();
                StringBuilder keyW = new StringBuilder();
                for (global::System.Int32 posText = 0; posText < text.Length; posText++, posKey++)
                {
                    int indC = RusAlph.IndexOf(text[posText]);
                    int resC;
                    int indK = (RusAlph.IndexOf(key[posKey]) + indOff) % RusAlph.Length;
                    resC = (indK + indC) % RusAlph.Length;
                    keyW.Append(RusAlph[indK]);

                    cipheredText.Text += RusAlph[resC];
                    forWrite.Append(RusAlph[resC]);
                    if (posKey + 1 >= key.Length)
                    {
                        posKey = -1;
                        indOff++;
                    }

                }
                writer.WriteLine(keyW);
                writer.Write(forWrite);
                return cipheredText.Text;
            }
        }

        static string ReadAndRemoveFirstLine(ref string content)
        {
            int newlineIndex = content.IndexOf('\r');
            string firstLine = content.Substring(0, newlineIndex);
            content = content.Substring(newlineIndex + 2);
            return firstLine;
        }
        private void DeCipher_Vigenere(string key, string text, string outputPath)
        {
           // using (StreamWriter writer = new StreamWriter(outputPath))
            {
                const string RusAlph = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";
                int posKey = 0;
                StringBuilder forWrite = new StringBuilder();
                for (global::System.Int32 posText = 0; posText < text.Length; posText++, posKey++)
                {
                    int indC = RusAlph.IndexOf(text[posText]);
                    int resC;
                    resC = ((RusAlph.Length - RusAlph.IndexOf(key[posKey])) + indC) % RusAlph.Length;

                      //  cipheredText.Text += RusAlph[resC];
                    forWrite.Append(RusAlph[resC]);

                }
            //    writer.Write(forWrite);
                string stri = forWrite.ToString();
                Deciphered.Text = stri;
            }
        }

        private string DeCipher_Table(string key, string text)
        {
            // string key_copy;
            string pattern = "[А-Яа-я0-9 ]";
            key = key.ToLower();
            key = Regex.Replace(key, pattern, "");
            int[] arInd = new int[key.Length];
            Table_sort(key, arInd);
            int text_i = 0;
            StringBuilder resStr = new StringBuilder(text);
            // copy(text.Begin, text.end(), std::back_inserter(resStr)); ;
            for (int i = 0; i < key.Length; i++)
            {
                int j = 0;
                while (arInd[i] + j * key.Length < text.Length)
                {
                    resStr[arInd[i] + j * key.Length] = text[text_i];
                    text_i++;
                    j++;
                }
            }
            string res = resStr.ToString();
            Deciphered.Text = res;
            return res;

            //for decoding neccessary parts - knowledge of the order of reading (sort letters of a key and return indexes)
            // may be needed buffer of the same size as the text. after that insert letters of the coded text by next rule:
            // key's letter position+ i*key.length() while this sum is less then the actual length of text.
            // example cab heyyou
            // __h__e -> y_hy_e->yohyue
        }


        private bool check_data_rus(string key, string Text)
        {
            // need to make the same as with ciphered text to key (get hello from heL12lO for example) 
            if ( key.Length == 0)
            {
                cipheredText.Text = Text;
                // std::cout << Text << "\n";
                return false;
            }
            string pattern = "[A-Za-z0-9 ]";
            Text.ToLower();
            Text = Regex.Replace(Text, pattern, "");
            key.ToLower();
            key = Regex.Replace(key, pattern, "");
            //cout << "\n" << new_key << "\n";
            if (key.Length == 0)
            {
                cipheredText.Text = Text;
                //std::cout << ToCipher << "\n";
                return false;
            }
            return true;
        }

        private string ReadFromFile(string filePath)
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                return reader.ReadToEnd();
            }
        }
        bool Table;
        bool Fily;
        private void main_log()
        {
           // cipheredText.Text = "";
            string filePath_notC = "C:\\C++\\C#\\WpfApp1\\WpfApp1\\to_cipher.txt";
            string filePath_C = "C:\\C++\\C#\\WpfApp1\\WpfApp1\\ciphered.txt";
            //    string toCipher = ReadFromFile(filePath_notC);
            //cout << "Give me the key word\n";
            // FILE * in;
            // FILE * out;
            //  fopen_s(&in, name1, "r");
            //fopen_s(&out,name2, "w");
            //putc('a', out);
            string key = "";
            string ToCipher = "";
            key = Key.Text;
            if (Fily)
            {
               // Table = true;
                ToCipher = ReadFromFile(filePath_notC).Replace("\r\n", string.Empty);
            }
            else
            {
                if (!Table)
                {
                    ToCipher = Tabletext.Text.Replace("\r\n", string.Empty);


                }
                if (Table)
                {
                    ToCipher = Tabletext.Text.Replace("\r\n", string.Empty);


                }
            }
            //a=fgetc(in);
            //while (fgets(a, 30, in))
            //  / {
            //     ToCipher += a;
            // }
            // fclose(in);
            //    cin >> key;
            // cout << "Give me the string to cipher word\n";
            //  cin >> ToCipher;

            //   ToCipher_1 = MainWindow.check_data_rus
            //               (&key, ToCipher);
            string zero = "";
            if (string.Equals(ToCipher, zero))
            {
                cipheredText.Text = "Smth wrong";
                return;

            }
            //string Ciphered=Cipher_Table(ToCipher_1, key);

            //string	DeCiphered = DeCipher_Table(key, Ciphered);
            //std::cout << DeCiphered << "\n";
            int i = 2;
            if (!Table)
            {
                bool is_ok = check_data_rus(key, ToCipher);
                if (is_ok)
                {
                    ToCipher = Cipher_Vigenere(key, ToCipher, filePath_C);
                    // std::cout << "\n";
                    string ToDecode = ReadFromFile(filePath_C);
                    string new_key = ReadAndRemoveFirstLine(ref ToDecode);
                    DeCipher_Vigenere(new_key, ToDecode, filePath_C);
                }
                else Deciphered.Text = "smth wrong";
            }
            if (Table)
            {
                //bool is_ok=Check_data(&key, &ToCipher);
                //if (is_ok)
                // {
                string correct=Cipher_Table(ToCipher, key, filePath_C);
                if (correct != "")
                {
                    string ToDecode = ReadFromFile(filePath_C);
                    string new_key = ReadAndRemoveFirstLine(ref ToDecode);
                    DeCipher_Table(key, ToDecode);
                }
                else Deciphered.Text = "smth wrong";
                // }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            main_log();
        }

        private void TableM_Checked(object sender, RoutedEventArgs e)
        {
            Table = true;
         //   Fily = false;
        }

        private void Viegener_Checked(object sender, RoutedEventArgs e)
        {
            Table = false;
          //  Fily = false;
        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {

        }

        private void Filya_Click(object sender, RoutedEventArgs e)
        {
            Fily = true;
        }
    }
}
