using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
using static System.Net.WebRequestMethods;

namespace AutoMorse
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "img Files (*.png)|*.png|bmp Files (*.bmp)|*.bmp";
            if (openFile.ShowDialog() ?? false == true && (!string.IsNullOrEmpty(openFile.FileName)))
            {
                string fileName = openFile.FileName;
                //MessageBox.Show(fileName);
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(fileName);
                bitmap.EndInit();
                img.Source = bitmap;

                var bitmap1 = ConventBitmap(bitmap);
                Dictionary<System.Drawing.Point, int> dic = new Dictionary<System.Drawing.Point, int>();
                for (int i = 0; i < bitmap1.Size.Width; i++)
                    for (int j = 0; j < bitmap1.Size.Height; j++)
                        dic.Add(new System.Drawing.Point(i, j), bitmap1.GetPixel(i, j).R);

                Dictionary<int, int> ndic = new Dictionary<int, int>();
                foreach (var item in dic)
                {
                    if (ndic.ContainsKey(item.Key.Y))
                        ndic[item.Key.Y] += item.Value;
                    else
                        ndic.Add(item.Key.Y, item.Value);
                }
                var redY = ndic.MaxBy(e => e.Value).Key;
                // var item = dic.GroupBy(e => e.Key.X).MaxBy;

                var xDic = new Dictionary<int, int>();
                for (int i = 0; i < bitmap1.Width; i++)
                {
                    xDic.Add(i, bitmap1.GetPixel(i, redY).R);
                }
                var morseCode = string.Empty;



                ///生成摩斯码
                int ticklen = 0;
                bool istick = false;
                foreach (var item in xDic)
                {
                    if (item.Value >= (xDic.Max(e=>e.Value)-30))
                    {
                        ticklen++;

                        istick = true;
                    }
                    else
                    {
                        if (istick)
                        {
                            if (ticklen < 5)
                            {
                                morseCode += ".";
                            }
                            else
                            {
                                morseCode += "-";
                            }
                            istick = false;
                            ticklen = 0;
                        }
                        else
                            morseCode += " ";
                        istick = false;
                    }
                }

                //处理摩斯码 大于6个空格
                ticklen = 0;
                var miniC = morseCode.IndexOf('.');
                if (morseCode.IndexOf('-') < miniC)
                    miniC = morseCode.IndexOf('-');
                morseCode = morseCode.Remove(0, miniC);
                var newMorse = string.Empty;
                foreach (var item in morseCode)
                {
                    if (item == '-' || item == '.')
                    {
                        if (ticklen > 5)
                        {

                            newMorse += " ";
                        }
                        newMorse += item;
                        ticklen = 0;

                    }
                    else
                    {
                        ticklen++;
                    }
                }
                MorseCode.Text = newMorse;
                var newStringList = newMorse.Split(' ').ToList();

                var result = string.Empty;
               foreach(var item in newStringList)
                {
                    if (morseDic.ContainsKey(item))
                        result += morseDic[item];
                    else
                            result += item;
                }
                EndResult.Text = result;
            }


        }

        private static Bitmap ConventBitmap(BitmapImage bitmap)
        {
            try
            {
                using (MemoryStream outStream = new())
                {
                    TiffBitmapEncoder enc = new TiffBitmapEncoder();
                    enc.Frames.Add(BitmapFrame.Create(bitmap));
                    enc.Save(outStream);
                    Bitmap result = new Bitmap(outStream);
                    return result;
                }


            }
            catch (Exception ex)
            {
                return new Bitmap(10, 10);
            }
        }
        private Dictionary<string, string> morseDic = new Dictionary<string, string>()
                {
            { ".-","A"},
            { "-...","B"},
            { "-.-.","C"},
            { "-..","D"},
            { ".","E"},
            { "..-.","F"},
            { "--.","G"},
            {"....","H" },
            {"..","I" },
            { ".---","J"},
            { "-.-","K"},
            { ".-..","L"},
            {"--","M" },
            {"-.","N" },
            {"---","O" },
            {".--.","P" },
            { "--.-","Q"},
            { ".-.","R"},
            { "...","S"},
            { "-","T"},
            { "..-","U"},
            { "...-","V"},
            { ".--","W"},
            { "-..-","X"},
            { "-.--","Y"},
            { "--..","Z"}
                };
    }
}
