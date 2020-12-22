using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 字模製作與目標比對
{
    public partial class Form1 : Form
    {
        static int width_word = 25;
        static int height_word = 50;//字模的寬與高
        byte[,,,] array_License_Plate = new byte[2, 36, width_word, height_word];//六與七碼車牌所有英數字二值化陣列
        byte[,,] array_Deform_69 = new byte[2, width_word, height_word];//變形的六碼車牌6與9字型
        byte[,] array_Compared_Font = new byte[width_word, height_word];
        byte[,] array_Targect = new byte[width_word, height_word];//待比對的字模與目標二值化陣列
        FastPixel f = new FastPixel();//宣告快速影像處理物件


        public Form1()
        {
            InitializeComponent();
        }

        //用影像載入字模
        private void button1_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                string d = folderBrowserDialog1.SelectedPath;
                for(int k = 0; k < 36; k++)
                {
                    //六碼字型
                    string kk = k.ToString();
                    if (kk.Length == 1) kk = "0" + kk;
                    Bitmap b6 = new Bitmap(d + "\\" + kk + ".gif");
                    for(int j = 0; j < height_word; j++)
                    {
                        for(int i = 0; i < width_word; i++)
                        {
                            Color C = b6.GetPixel(i, j);
                            if (C.R < 128) array_License_Plate[0, k, i, j]=1;//黑點
                        }
                    }
                    //七碼字型
                    kk = (k + 36).ToString();
                    Bitmap b7 = new Bitmap(d + "\\" + kk + ".gif");
                    for(int j = 0; j < height_word; j++)
                    {
                        for(int i = 0; i < width_word; i++)
                        {
                            Color C = b7.GetPixel(i, j);
                            if (C.R < 128) array_License_Plate[1, k, i, j] = 1;//黑點
                        }
                    }
                }
                //六碼變形的69字型
                for(int k = 72; k < 74; k++)
                {
                    Bitmap b69 = new Bitmap(d + "\\" + k.ToString() + ".gif");
                    for(int j = 0; j < height_word; j++)
                    {
                        for(int i = 0; i < width_word; i++)
                        {
                            Color C = b69.GetPixel(i, j);
                            if (C.R < 128) array_Deform_69[k - 72, i, j] = 1;//黑點
                        }
                    }
                }
            }
        }

        //選擇字模
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int knd = listBox1.SelectedIndex;//六或七碼
            if (knd < 0) return;//未選擇
            int Sc = listBox2.SelectedIndex;//選擇的字元
            if (Sc < 0) return;//未選擇
            if (knd == 1 && Sc > 35) return;//七碼車牌字元無變形的69
            array_Targect = new byte[width_word, height_word];
            for(int i = 0; i < width_word; i++)
            {
                for(int j = 0; j < height_word; j++)
                {
                    if(knd==0)//六碼字型
                    {
                        if (Sc <= 35)
                        {
                            array_Targect[i, j] = array_License_Plate[0, Sc, i, j];
                        }
                        else
                        {
                            array_Targect[i, j] = array_Deform_69[Sc - 36, i, j];
                        }
                    }
                    else//七碼字型
                    {
                        array_Targect[i, j] = array_License_Plate[1, Sc, i, j];
                    }
                }
            }
            pictureBox1.Image = f.BWImg(array_Targect);//顯示字模影像
        }

        //建立字模二進位檔案
        private void button2_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (System.IO.File.Exists(saveFileDialog1.FileName))
                {
                    System.IO.File.Delete(saveFileDialog1.FileName);
                }
            }
            //寫出六與七碼字模
            byte[] x = new byte[width_word * height_word * (36 * 2 + 2)];
            int n = 0;
            //六與七碼字模
            for(int m = 0; m < 2; m++)
            {
                for(int k = 0; k < 36; k++)
                {
                    for(int j = 0; j < height_word; j++)
                    {
                        for(int i = 0; i < width_word; i++)
                        {
                            x[n] = array_License_Plate[m, k, i, j];
                            n += 1;
                        }
                    }
                }
            }
            //六碼變形69字模
            for(int k = 0; k < 2; k++)
            {
                for(int j = 0; j < height_word; j++)
                {
                    for(int i = 0; i < width_word; i++)
                    {
                        x[n] = array_Deform_69[k, i, j];
                        n += 1;
                    }
                }
            }
            //寫出檔案
            System.IO.File.WriteAllBytes(saveFileDialog1.FileName, x);
        }

        //程式啟動載入字模
        private void Form1_Load(object sender, EventArgs e)
        {
            FontLoad();
        }
        //載入字模
        private void FontLoad()
        {
            byte[] q = Properties.Resources.font;
            int n = 0;
            for(int m = 0; m < 2; m++)
            {
                for(int k = 0; k < 36; k++)
                {
                    for (int j=0; j < height_word; j++)
                    {
                        for(int i = 0; i < width_word; i++)
                        {
                            array_License_Plate[m, k, i, j] = q[n];
                            n += 1;
                        }
                    }   
                }
            }
            for(int k = 0; k < 2; k++)
            {
                for(int j = 0; j < height_word; j++)
                {
                    for(int i = 0; i < width_word; i++)
                    {
                        array_Deform_69[k, i, j] = q[n];
                        n += 1;
                    }
                }
            }
        }

        //載入比對目標
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Bitmap m = new Bitmap(openFileDialog1.FileName);
                pictureBox2.Image = m;
                array_Compared_Font = new byte[width_word, height_word];//待比對的目標二值化陣列
                for(int j = 0; j < height_word; j++)
                {
                    for(int i = 0; i < width_word; i++)
                    {
                        Color C = m.GetPixel(i, j);
                        if (C.R < 127) array_Compared_Font[i, j] = 1;//黑點
                    }
                }
            }
        }

        //執行字模比對
        private void button3_Click(object sender, EventArgs e)
        {
            int n0 = 0;//字模黑點數
            int nf = 0;//符合的黑點數
            for(int j = 0; j < height_word; j++)
            {
                for(int i = 0; i < width_word; i++)
                {
                    if (array_Targect[i, j] == 0)
                    {
                        if (array_Compared_Font[i, j] == 1) nf -= 1;
                    }
                    else
                    {
                        n0 += 1;//字模黑點數累計
                        if (array_Compared_Font[i, j] == 1) nf += 1;//目標與字模符合點數
                    }
                }
            }
            int pc = nf * 1000 / n0;//符合點數千分比
            label1.Text = pc.ToString();
        }
    }
}
