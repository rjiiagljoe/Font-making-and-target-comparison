using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace 字模製作與目標比對
{
    class FastPixel
    {
        public int imagewidth, imageheight;//影像寬與高
        public byte[,] arrayR, arrayG, arrayB;//RGB陣列
        byte[] arrayRGB; //影像的可存取副本資料陣列
        System.Drawing.Imaging.BitmapData imageData;//影像資料
        IntPtr imageptr; //影像資料所在的記憶體指標(位置)
        int imageTotalBit, singleRowBit, singlePointBit;//影像總位元組數,單列位元組數，單點位元組數

        //鎖定點陣圖(Bitmap)物件的記憶體位置，建立一個可操作的位元組陣列副本
        private void LockBMP(Bitmap bmp)
        {
            //矩形物件，定義影像範圍
            Rectangle rectangle = new Rectangle(0, 0, bmp.Width, bmp.Height);
            //鎖定影像區記憶體(暫時不接受作業系統的移動)
            imageData = bmp.LockBits(rectangle,System.Drawing.Imaging.ImageLockMode.ReadWrite,bmp.PixelFormat);
            imageptr = imageData.Scan0;//影像區塊的記憶體指標
            singleRowBit = imageData.Stride;//每一影像列的長度(bytes)
            singlePointBit = (int)Math.Floor((double)singleRowBit / (double)bmp.Width);//每一像素的位元組數(3或4)
            imageTotalBit = singleRowBit * bmp.Height;//影像總位元組數
            arrayRGB = new byte[imageTotalBit];//宣告影像副本資料陣列
            //拷貝點陣圖資料到副本陣列
            System.Runtime.InteropServices.Marshal.Copy(imageptr, arrayRGB, 0, imageTotalBit);
        }

        //複製位元組陣列副本的處理結果到BitMap物件，並解除其記憶體鎖定
        private void UnLockBMP(Bitmap bmp)
        {
            //拷貝副本陣列到點陣圖位置
            System.Runtime.InteropServices.Marshal.Copy(arrayRGB, 0, imageptr, imageTotalBit);
            bmp.UnlockBits(imageData);//解除鎖定
        }

        //取得RGB陣列
        public void BMP2RGB(Bitmap bmp)
        {
            imagewidth = bmp.Width;
            imageheight = bmp.Height;//影像寬高
            arrayR = new byte[imagewidth, imageheight];
            arrayG = new byte[imagewidth, imageheight];
            arrayB = new byte[imagewidth, imageheight];//RGB
            LockBMP(bmp);
            for(int j = 0;j<imageheight;j++)
            {
                int Lj = j * imageData.Stride;
                for(int i=0;i<imagewidth;i++)
                {
                    int k = Lj + i * singlePointBit;
                    arrayR[i, j] = arrayRGB[k + 2];//Red
                    arrayG[i, j] = arrayRGB[k + 1];//Green
                    arrayB[i, j] = arrayRGB[k];//Blue
                }
            }
            UnLockBMP(bmp);
        }

        //灰階圖
        public Bitmap GrayImg(byte [,] b)
        {
            Bitmap bmp = new Bitmap(b.GetLength(0), b.GetLength(1));
            LockBMP(bmp);
            for(int j= 0;j<b.GetLength(1);j++)
            {
                for(int i=0;i<b.GetLength(0);i++)
                {
                    int k = j * singleRowBit + i * singlePointBit;
                    byte c = b[i, j];
                    arrayRGB[k] = c;
                    arrayRGB[k + 1] = c;
                    arrayRGB[k + 2] = c;//RGB一致
                    arrayRGB[k + 3] = 255;//實心不透明
                }
            }
            UnLockBMP(bmp);
            return bmp;
        }

        //黑白圖
        public Bitmap BWImg(byte[,] b)
        {
            Bitmap bmp = new Bitmap(b.GetLength(0), b.GetLength(1));
            LockBMP(bmp);
            for(int j = 0; j < b.GetLength(1); j++)
            {
                for(int i= 0; i < b.GetLength(0); i++)
                {
                    int k = j * singleRowBit + i * singlePointBit;
                    if (b[i, j] == 1)
                    {
                        arrayRGB[k] = 0;
                        arrayRGB[k + 1] = 0;
                        arrayRGB[k + 2] = 0;//黑
                    }
                    else
                    {
                        arrayRGB[k] = 255;
                        arrayRGB[k + 1] = 255;
                        arrayRGB[k + 2] = 255;//白
                    }
                    arrayRGB[k + 3] = 255;
                }
            }
            UnLockBMP(bmp);
            return bmp;
        }
    }

    class TgInfo
    {
        public int targetPoint = 0;//目標點數
        public ArrayList targetPointList = null;//目標點的集合
        public int x_max_negative = 0, x_max_positive = 0, y_max_negative = 0, y_max_positive = 0;//四面座標極值
        public int width = 0, height = 0;//寬與高
        public int contrast_target_back = 0;//目標與背景的對比強度
        public int x_target = 0, y_target = 0;//目標中心點座標
        public int ID_contrast = 0;//依對比度排序的序號

    }
}
