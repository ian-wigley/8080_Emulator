using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Emulator
{
    public partial class Form1 : Form
    {
        private string fileName;
        private CPU m_cpu;
        private IO m_io;
        private int width = 256;
        private List<byte> rom = new List<byte>();
        private uint[,] color = new uint[32, 2];
        private uint[,] color2 = new uint[32, 2];
        private uint[] color3 = new uint[32 * 2];

        [StructLayout(LayoutKind.Sequential)]
        public struct NativeMessage
        {
            public IntPtr Handle;
            public uint Message;
            public IntPtr WParameter;
            public IntPtr LParameter;
            public uint Time;
            public Point Location;
        }

        [DllImport("user32.dll")]
        static extern bool PeekMessage(out Message message, IntPtr window, uint messageFilterMinimum, uint messageFilterMaximum, uint shouldRemoveMessage);

        public Form1()
        {
            Application.Idle += HandleApplicationIdle;
            InitializeComponent();

            for (int i = 0; i < 32; i++)
            {
                color[i, 0] = 0;
                color2[i, 0] = 0x44444400;

                if (i >= 26 && i <= 27)
                {
                    color[i, 1] = 0xFF000000;
                    color2[i, 1] = 0xAA000000;
                }
                else if (i >= 2 && i <= 7)
                {
                    color[i, 1] = 0x00FF0000;
                    color2[i, 1] = 0xAA000000;
                }
                else
                {
                    color[i, 1] = 0x00FF0000;
                    color2[i, 1] = 0xAA000000;
                }
            }
        }

        bool IsApplicationIdle()
        {
            Message result;
            return !PeekMessage(out result, IntPtr.Zero, 0, 0, 0);
        }

        void HandleApplicationIdle(object sender, EventArgs e)
        {
            while (IsApplicationIdle())
            {
                if (m_cpu != null)
                {
                    UpdateLoop();
                }
            }
        }

        void UpdateLoop()
        {
            m_io.Update();
            m_cpu.Run();
            DrawVertical();
            //DrawHorizontal();
        }

        // Open File method
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Load ROM files",
                InitialDirectory = Directory.GetCurrentDirectory(),
                Filter = "All files (*.*)|*.*|All files (*.ROM)|*.rom",
                FilterIndex = 2,
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                fileName = openFileDialog.FileName;
                try
                {
                    using (BinaryReader b = new BinaryReader(File.Open(fileName, FileMode.Open)))
                    {
                        // Read the input stream & display the contents
                        while (b.BaseStream.Position < b.BaseStream.Length)
                        {
                            rom.Add(b.ReadByte());
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("The file could not be read: " + ex.Message);
                }
                finally
                {
                    initialiseComponents();
                }
            }
        }

        private void initialiseComponents()
        {
            int start = rom.Count;
            int end = rom.Count * 2;

            for (int i = start; i < end + 1001; i++)
            {
                rom.Add(0);
            }

            m_io = new IO();
            m_cpu = new CPU(rom, m_io, label1);
            m_io.SetCPU(m_cpu);
        }

        // Save File method
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
        }

        // Exit the application
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


        private void Form1_Paint(object sender, PaintEventArgs e)
        {
        }

        private void setPixel(byte[] imagedata, int x, int y, uint color)
        {
            var i = (y * (width | 0) + x) * 4;
            imagedata[i++] = (byte)((color >> 16) & 0xFF);
            imagedata[i++] = (byte)((color >> 8) & 0xFF);
            imagedata[i++] = (byte)(color & 0xFF);
            imagedata[i] = (byte)((color >> 24) & 0xFF);
        }

        private void DrawVertical()
        {
            if (rom.Count > 0)
            {
                // Number of channels (ie. assuming 24 bit RGB in this case)
                int ch = 1;
                byte[] imageData = new byte[256 * 256 * ch];
                int pitch = 256;

                for (int j = 0; j < 224; j++)
                {
                    int src = 0x2400 + (j << 5);
                    int dst = 255 * pitch + j;

                    for (int i = 0; i < 32; i++)
                    {
                        int vram = rom[src];
                        src += 1;

                        /*
                         *  Screen data is located at hex: $2400 dec: 9216
                         *
                         *  Each line is made up of 256 pixels
                         *  Each byte contains 8 pixels worth of data (8 bits in 1 byte)
                         *  Each line consists of 32 bytes (32 * 8 = 256)
                         */

                        for (int bit = 0; bit < 8; bit++)
                        {
                            byte color0 = 0x00;
                            byte color1 = 0x00;

                            if ((vram & 1) != 0)
                            {
                                color0 = 0xff;
                                color1 = 0xff;
                            }
                            imageData[dst + 0] = color0;
                            imageData[dst + 1] = color1;

                            dst -= pitch;
                            vram >>= 1;
                        }
                    }
                }

                Bitmap bitmap = new Bitmap(256, 256, PixelFormat.Format8bppIndexed);
                BitmapData bmData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
                IntPtr pNative = bmData.Scan0;
                Marshal.Copy(imageData, 0, pNative, 256 * 256 * ch);
                bitmap.UnlockBits(bmData);
                pictureBox1.Image = bitmap;
                Invalidate();
            }
        }

        private void DrawHorizontal()
        {
            if (rom.Count > 0)
            {
                // Number of channels (ie. assuming 24 bit RGB in this case)
                int ch = 1;
                byte[] imageData = new byte[256 * 256 * ch];
                int pitch = 256;

                for (int j = 0; j < 224; j++)//224
                {
                    int src = 0x2400 + (j << 5);
                    int dst = j * pitch;

                    for (int i = 0; i < 32; i++)
                    {
                        int vram = rom[src];
                        src += 1;

                        /*
                         *  Screen data is located at hex: $2400 dec: 9216
                         *  Each line is made up of 256 pixels
                         *  Each byte contains 8 pixels worth of data (8 bits in 1 byte)
                         *  Each line consists of 32 bytes (32 * 8 = 256)
                         */

                        for (int bit = 0; bit < 8; bit++)
                        {
                            byte color0 = 0x00;
                            byte color1 = 0x00;

                            if ((vram & 1) != 0)
                            {
                                color0 = 0xff;
                                color1 = 0xff;
                            }
                            imageData[dst + 0] = color0;
                            imageData[dst + 1] = color1;

                            dst += 1;
                            vram = vram >> 1;
                        }
                    }
                }
                Bitmap bitmap = new Bitmap(256, 256, PixelFormat.Format8bppIndexed);
                BitmapData bmData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
                IntPtr pNative = bmData.Scan0;
                Marshal.Copy(imageData, 0, pNative, 256 * 256 * ch);
                bitmap.UnlockBits(bmData);
                pictureBox1.Image = bitmap;
                this.Invalidate();
            }
        }

        //https://social.msdn.microsoft.com/Forums/vstudio/en-US/7fc58f05-fccc-46ec-b58e-8755afb6e90f/monitoring-a-shortcut-key-while-application-running-in-background?forum=csharpgeneral#57de7967-b7ba-4cd0-8fe8-48fde41466dd
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            const int WM_KEYDOWN = 0x100;
            const int WM_SYSKEYDOWN = 0x104;

            if ((msg.Msg == WM_KEYDOWN) || (msg.Msg == WM_SYSKEYDOWN))
            {
                switch (keyData)
                {
                    // Coin ?
                    case Keys.C:
                        m_io.KetDownCoin();
                        break;

                    // Start player 1
                    case Keys.D1:
                        m_io.KeyDown1();
                        break;

                    // Start player 2
                    case Keys.D2:
                        m_io.KeyDown2();
                        break;

                    // Tilt
                    case Keys.T:
                        m_io.KeyDownT();
                        break;

                    // Move left
                    case Keys.Left:
                        m_io.KeyDownLeft();
                        break;

                    // Move right
                    case Keys.Right:
                        m_io.KeyDownRight();
                        break;

                    // Fire
                    case Keys.Space:
                        m_io.KeyDownSpace();
                        break;
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
