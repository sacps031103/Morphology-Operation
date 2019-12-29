using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        string img_FileName;
        Bitmap Image;
        Bitmap ImageDilation;
        Bitmap ImageErosion;
        private int[,] Grayscale;
        private bool[,] GrayscaleSet;
        private bool[,] GrayscaleAfter;
        private bool[,] OperationArray;
        private bool[,] ReverseOne;
        private bool[,] ReverseTwo;
        bool haveImage = false;
        bool haveOperation = false;
        bool haveGrayscalesetting = false;
        int Operation = 1;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            trackBar1.Maximum = 256;
            trackBar1.Minimum = 0;
            trackBar1.Enabled = false;
            button1.Font = new System.Drawing.Font("新細明體", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            button3.Font = new System.Drawing.Font("新細明體", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.White;
                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.SelectionBackColor = Color.White;
                OperationArray[e.ColumnIndex, e.RowIndex] = false;
            }
            else
            {
                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.Black;
                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.SelectionBackColor = Color.Black;
                OperationArray[e.ColumnIndex, e.RowIndex] = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.RestoreDirectory = true;
            dlg.Title = "Open Image File";
            dlg.Filter = "JPeg Image|*.jpg|Bitmap Image|*.bmp|Png Image|*.png";
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK && dlg.FileName != null)
            {
                haveGrayscalesetting = false;
                img_FileName = dlg.FileName;
                Image = new Bitmap(img_FileName);
                ImageDilation = new Bitmap(img_FileName);
                ImageErosion = new Bitmap(img_FileName);
                pictureBox2.Image = null;
                pictureBox3.Image = null;
                int height = Image.Height;
                int width = Image.Width;
                Grayscale = new int[width, height];
                GrayscaleSet = new bool[width, height];
                GrayscaleAfter = new bool[width, height];
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        //存取圖像的像素
                        Color p = Image.GetPixel(x, y);
                        //灰階化
                        double intensity = 0.299 * p.R + 0.587 * p.G + 0.14 * p.B;
                        if (intensity > 255) intensity = 255;
                        if (intensity < 0) intensity = 0;
                        Grayscale[x, y] = (int)intensity;
                        Image.SetPixel(x, y, Color.FromArgb(255, (int)intensity, (int)intensity, (int)intensity));                       
                    }
                }
                haveGrayscalesetting = true;
                trackBar1.Value = 128;
                for (int y = 0; y < Grayscale.GetLength(1); y++)
                {
                    for (int x = 0; x < Grayscale.GetLength(0); x++)
                    {
                        if (Grayscale[x, y] >= trackBar1.Value)
                        {
                            GrayscaleSet[x, y] = false;
                            Image.SetPixel(x, y, Color.FromArgb(255, 255, 255, 255));
                        }
                        else
                        {
                            GrayscaleSet[x, y] = true;
                            Image.SetPixel(x, y, Color.FromArgb(255, 0, 0, 0));
                        }
                    }
                }
                haveOperation = true;
                Operation = Operation + 2;
                SetDataGridView(Operation);
                pictureBox1.Image = Image;
                label1.Text = "灰階: " + trackBar1.Value;
                label2.Text = "圖片尺寸 " + width + " x " + height + "  " + img_FileName;
                pictureBox1.Image = Image;
                haveImage = true;
                trackBar1.Enabled = true;
            }
            else
            {
                img_FileName = "";
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            haveGrayscalesetting = true;
            for (int y = 0; y < Grayscale.GetLength(1); y++)
            {
                for (int x = 0; x < Grayscale.GetLength(0); x++)
                {
                    if(Grayscale[x, y]>=trackBar1.Value)
                    {
                        GrayscaleSet[x, y] = false;
                        Image.SetPixel(x, y, Color.FromArgb(255,255, 255, 255));
                    }
                    else
                    {
                        GrayscaleSet[x, y] = true;
                        Image.SetPixel(x, y, Color.FromArgb(255,0, 0, 0));
                    }
                }
            }
            pictureBox1.Image = Image;
            label1.Text = "灰階: " + trackBar1.Value;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            haveOperation = true;
            if (!(Operation == 3))
            {
                Operation = Operation - 2;
                SetDataGridView(Operation);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            haveOperation = true;
            Operation = Operation + 2;
            SetDataGridView(Operation);
        }
        void SetDataGridView(int Data)
        {
            dataGridView1.Rows.Clear();
            OperationArray = new bool[Data, Data];
            ReverseOne = new bool[Data, Data];
            ReverseTwo = new bool[Data, Data];
            int height = OperationArray.GetLength(0);
            int width = OperationArray.GetLength(1);
            this.dataGridView1.ColumnCount = width;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.White;
            dataGridView1.ColumnHeadersVisible = false;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.AllowUserToResizeColumns = false;
            dataGridView1.AllowUserToResizeRows = false;
            for (int r = 0; r < width; r++)
            {
                dataGridView1.RowHeadersVisible = false;
                dataGridView1.Columns[r].Width = 20;
                dataGridView1.Columns[r].ReadOnly = true;
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(this.dataGridView1);
                row.Height = 20;
                for (int c = 0; c < height; c++)
                {
                    row.Cells[c].Style.BackColor = Color.White;
                    OperationArray[c, r] = false;
                }
                this.dataGridView1.Rows.Add(row);
            }
        }
       void Reverse()
        {
            for (int x = 0; x < OperationArray.GetLength(0); x++)
            {
                ReverseOne[x,((OperationArray.GetLength(1)-1) / 2)] = OperationArray[x, ((OperationArray.GetLength(1)-1) / 2)];
                for (int i = 0; i < (OperationArray.GetLength(1) - 1) / 2; i++)
                {
                    ReverseOne[x, i] = OperationArray[x, OperationArray.GetLength(0) - i - 1];
                    ReverseOne[x, OperationArray.GetLength(0) - i - 1] = OperationArray[x, i];
                }
            }
            for (int x = 0; x < OperationArray.GetLength(0); x++)
            {
                ReverseTwo[((OperationArray.GetLength(1) - 1) / 2), x] = ReverseOne[((OperationArray.GetLength(1) - 1) / 2), x];
                for (int i = 0; i < (OperationArray.GetLength(1) - 1) / 2; i++)
                {
                    ReverseTwo[i, x] = ReverseOne[OperationArray.GetLength(0) - i - 1, x];
                    ReverseTwo[OperationArray.GetLength(0) - i - 1, x] = ReverseOne[i, x];
                }
            }
        }
        
        void DilationClosing()
        {
            for (int y = 0; y < Grayscale.GetLength(1); y++)
            {
                for (int x = 0; x < Grayscale.GetLength(0); x++)
                {
                    int SquareY = y-((OperationArray.GetLength(1) - 1) / 2);
                    for (int i = 0; i < OperationArray.GetLength(1); i++)
                    {
                        bool Detection = false;
                        int SquareX = x-((OperationArray.GetLength(0) - 1) / 2);
                        for (int j = 0; j < OperationArray.GetLength(0); j++)
                        {
                            if (SquareX >= 0 && SquareY >= 0 && SquareX < Grayscale.GetLength(0) && SquareY < Grayscale.GetLength(1))
                            {
                                if (GrayscaleSet[SquareX, SquareY] && ReverseTwo[j, i])
                                {
                                    Detection = true;
                                    ImageDilation.SetPixel(x, y, Color.FromArgb(255, 0, 0, 0));
                                    GrayscaleAfter[x, y] = true;
                                    break;
                                }
                                else
                                {
                                    ImageDilation.SetPixel(x, y, Color.FromArgb(255, 255, 255, 255));
                                    GrayscaleAfter[x, y] = false;
                                }
                            }
                            SquareX++;
                        }
                        SquareY++;
                        if (Detection) {
                            break;
                        }                  
                    }
                }
            }               
        }
        void ErosionClosing()
        {
            int Quantity = 0;
            for (int i = 0; i < OperationArray.GetLength(1); i++)
            {
                for (int j = 0; j < OperationArray.GetLength(0); j++)
                {
                    if(OperationArray[j, i])
                    {
                        Quantity++;
                    }
                }
            }
            for (int y = 0; y < Grayscale.GetLength(1); y++)
            {
                for (int x = 0; x < Grayscale.GetLength(0); x++)
                {
                    int OperationQuantity = 0;
                    int SquareY = y - ((OperationArray.GetLength(1) - 1) / 2);
                    for (int i = 0; i < OperationArray.GetLength(1); i++)
                    {
                        int SquareX = x - ((OperationArray.GetLength(0) - 1) / 2);
                        for (int j = 0; j < OperationArray.GetLength(0); j++)
                        {
                            if (SquareX >= 0 && SquareY >= 0 && SquareX < Grayscale.GetLength(0) && SquareY < Grayscale.GetLength(1))
                            {
                                if (GrayscaleAfter[SquareX, SquareY] && ReverseTwo[j, i])
                                {
                                    OperationQuantity++;
                                }
                            }
                            SquareX++;
                        }
                        SquareY++;
                    }
                    if(OperationQuantity== Quantity)
                    {
                        ImageErosion.SetPixel(x, y, Color.FromArgb(255, 0, 0, 0));
                    }
                    else
                    {
                        ImageErosion.SetPixel(x, y, Color.FromArgb(255, 255, 255, 255));
                    }
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (haveImage && haveOperation && haveGrayscalesetting)
            {
                Reverse();
                DilationClosing();
                pictureBox2.Image = ImageDilation;
                ErosionClosing();
                pictureBox3.Image = ImageErosion;
            }
            else
            {
                MessageBox.Show("未完成設定");
            }
        }
        void DilationOpening()
        {
            for (int y = 0; y < Grayscale.GetLength(1); y++)
            {
                for (int x = 0; x < Grayscale.GetLength(0); x++)
                {
                    int SquareY = y - ((OperationArray.GetLength(1) - 1) / 2);//Structuring Element 0到中心點的值
                    for (int i = 0; i < OperationArray.GetLength(1); i++)
                    {
                        bool Detection = false;
                        int SquareX = x - ((OperationArray.GetLength(0) - 1) / 2);//Structuring Element 0到中心點的值
                        for (int j = 0; j < OperationArray.GetLength(0); j++)
                        {
                            if (SquareX >= 0 && SquareY >= 0 && SquareX < Grayscale.GetLength(0) && SquareY < Grayscale.GetLength(1))//不大於原圖片, 小於原圖片的坐標
                            {
                                if (GrayscaleAfter[SquareX, SquareY] && ReverseTwo[j, i])
                                {
                                    Detection = true;
                                    ImageDilation.SetPixel(x, y, Color.FromArgb(255, 0, 0, 0));
                                    break;
                                }
                                else
                                {
                                    ImageDilation.SetPixel(x, y, Color.FromArgb(255, 255, 255, 255));
                                }
                            }
                            SquareX++;
                        }
                        SquareY++;
                        if (Detection)
                        {
                            break;
                        }
                    }
                }
            }
        }
        void ErosionOpening()
        {
            int Quantity = 0;
            for (int i = 0; i < OperationArray.GetLength(1); i++)
            {
                for (int j = 0; j < OperationArray.GetLength(0); j++)
                {
                    if (OperationArray[j, i])
                    {
                        Quantity++;
                    }
                }
            }
            for (int y = 0; y < Grayscale.GetLength(1); y++)
            {
                for (int x = 0; x < Grayscale.GetLength(0); x++)
                {
                    int OperationQuantity = 0;
                    int SquareY = y - ((OperationArray.GetLength(1) - 1) / 2);//Structuring Element 0到中心點的值
                    for (int i = 0; i < OperationArray.GetLength(1); i++)
                    {
                        int SquareX = x - ((OperationArray.GetLength(0) - 1) / 2);//Structuring Element 0到中心點的值
                        for (int j = 0; j < OperationArray.GetLength(0); j++)
                        {
                            if (SquareX >= 0 && SquareY >= 0 && SquareX < Grayscale.GetLength(0) && SquareY < Grayscale.GetLength(1))//不大於原圖片, 小於原圖片的坐標
                            {
                                if (GrayscaleSet[SquareX, SquareY] && ReverseTwo[j, i])
                                {
                                    OperationQuantity++;
                                }
                            }
                            SquareX++;
                        }
                        SquareY++;
                    }
                    if (OperationQuantity == Quantity)
                    {
                        ImageErosion.SetPixel(x, y, Color.FromArgb(255, 0, 0, 0));
                        GrayscaleAfter[x, y] = true;
                    }
                    else
                    {
                        ImageErosion.SetPixel(x, y, Color.FromArgb(255, 255, 255, 255));
                        GrayscaleAfter[x, y] = false;
                    }
                }
            }
        }
        private void button5_Click(object sender, EventArgs e)
        {
            if (haveImage && haveOperation && haveGrayscalesetting)
            {
                Reverse();
                ErosionOpening();
                pictureBox2.Image = ImageErosion;
                DilationOpening();
                pictureBox3.Image = ImageDilation;
            }
            else
            {
                MessageBox.Show("未完成設定");
            }
        }
    }
}
