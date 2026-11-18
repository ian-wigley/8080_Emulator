using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Emulator
{
    public partial class Emulate : Form, IMessageFilter
    {
        private readonly uint[,] color = new uint[32, 2];
        private readonly uint[,] color2 = new uint[32, 2];
        private readonly List<byte> rom = new List<byte>();
        private readonly int width = 256;
        private uint[] color3 = new uint[32 * 2];
        private string fileName;
        private CPU m_cpu;
        private IO m_io;

        // Frame pacing
        private const double FrameMs = 1000.0 / 60.0;
        private readonly Stopwatch clock = Stopwatch.StartNew();
        private double nextFrameMs;

        public Emulate()
        {
            Application.Idle += HandleApplicationIdle;
            InitializeComponent();
            Application.AddMessageFilter(this);
            timeBeginPeriod(1);

            for (var i = 0; i < 32; i++)
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

        [DllImport("winmm.dll")]
        private static extern uint timeBeginPeriod(uint milliseconds);

        [DllImport("winmm.dll")]
        private static extern uint timeEndPeriod(uint milliseconds);

        [DllImport("user32.dll")]
        private static extern bool PeekMessage(out Message message, IntPtr window, uint messageFilterMinimum,
            uint messageFilterMaximum, uint shouldRemoveMessage);

        private bool IsApplicationIdle()
        {
            return !PeekMessage(out var result, IntPtr.Zero, 0, 0, 0);
        }

        private void HandleApplicationIdle(object sender, EventArgs e)
        {
            while (IsApplicationIdle())
            {
                if (m_cpu == null)
                {
                    System.Threading.Thread.Sleep(10);
                    continue;
                }

                var now = clock.Elapsed.TotalMilliseconds;
                if (now < nextFrameMs)
                {
                    System.Threading.Thread.Sleep(1);
                    continue;
                }

                if (now - nextFrameMs > 5 * FrameMs) nextFrameMs = now;
                nextFrameMs += FrameMs;

                UpdateLoop();
            }
        }

        private void UpdateLoop()
        {
            m_cpu.Run();
            DrawVertical();
            //DrawHorizontal();
        }

        // Open File method
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog
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
                    using (var b = new BinaryReader(File.Open(fileName, FileMode.Open)))
                    {
                        // Read the input stream & display the contents
                        while (b.BaseStream.Position < b.BaseStream.Length) rom.Add(b.ReadByte());
                        // tempString += b.ReadByte() + ",";
                    }

                    // File.WriteAllText("Rom.csv", tempString);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(@"The file could not be read: " + ex.Message);
                }
                finally
                {
                    InitialiseComponents();
                }
            }
        }

        private void InitialiseComponents()
        {
            var start = rom.Count;
            var end = rom.Count * 2;

            for (var i = start; i < end + 1001; i++) rom.Add(0);

            m_io = new IO();
            m_cpu = new CPU(rom, m_io, label1);
            m_io.SetCPU(m_cpu);
        }

        // Exit the application
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void SetPixel(byte[] imagedata, int x, int y, uint color)
        {
            var i = (y * (width | 0) + x) * 4;
            imagedata[i++] = (byte)((color >> 16) & 0xFF);
            imagedata[i++] = (byte)((color >> 8) & 0xFF);
            imagedata[i++] = (byte)(color & 0xFF);
            imagedata[i] = (byte)((color >> 24) & 0xFF);
        }

        private void SetFrame(Bitmap bitmap)
        {
            var old = pictureBox1.Image;
            pictureBox1.Image = bitmap;
            old?.Dispose();
            Invalidate();
        }

        private void DrawVertical()
        {
            if (rom.Count > 0)
            {
                // Number of channels (ie. assuming 24 bit RGB in this case)
                var ch = 1;
                var imageData = new byte[256 * 256 * ch];
                var pitch = 256;

                for (var j = 0; j < 224; j++)
                {
                    var src = 0x2400 + (j << 5);
                    var dst = 255 * pitch + j;

                    for (var i = 0; i < 32; i++)
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

                        for (var bit = 0; bit < 8; bit++)
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

                var bitmap = new Bitmap(256, 256, PixelFormat.Format8bppIndexed);
                var bmData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite,
                    bitmap.PixelFormat);
                var pNative = bmData.Scan0;
                Marshal.Copy(imageData, 0, pNative, 256 * 256 * ch);
                bitmap.UnlockBits(bmData);
                SetFrame(bitmap);
            }
        }

        private void DrawHorizontal()
        {
            if (rom.Count > 0)
            {
                // Number of channels (ie. assuming 24 bit RGB in this case)
                var ch = 1;
                var imageData = new byte[256 * 256 * ch];
                var pitch = 256;

                for (var j = 0; j < 224; j++) //224
                {
                    var src = 0x2400 + (j << 5);
                    var dst = j * pitch;

                    for (var i = 0; i < 32; i++)
                    {
                        int vram = rom[src];
                        src += 1;

                        /*
                         *  Screen data is located at hex: $2400 dec: 9216
                         *  Each line is made up of 256 pixels
                         *  Each byte contains 8 pixels worth of data (8 bits in 1 byte)
                         *  Each line consists of 32 bytes (32 * 8 = 256)
                         */

                        for (var bit = 0; bit < 8; bit++)
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

                var bitmap = new Bitmap(256, 256, PixelFormat.Format8bppIndexed);
                var bmData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite,
                    bitmap.PixelFormat);
                var pNative = bmData.Scan0;
                Marshal.Copy(imageData, 0, pNative, 256 * 256 * ch);
                bitmap.UnlockBits(bmData);
                SetFrame(bitmap);
            }
        }

        // Sees every key down AND key up for the app, whatever control has focus,
        // so we can track which keys are held.
        public bool PreFilterMessage(ref Message m)
        {
            const int WM_KEYDOWN = 0x100;
            const int WM_KEYUP = 0x101;
            const int WM_SYSKEYDOWN = 0x104;
            const int WM_SYSKEYUP = 0x105;

            if (m_io == null || ActiveForm != this) return false;

            bool down;
            switch (m.Msg)
            {
                case WM_KEYDOWN:
                case WM_SYSKEYDOWN:
                    down = true;
                    break;
                case WM_KEYUP:
                case WM_SYSKEYUP:
                    down = false;
                    break;
                default:
                    return false;
            }

            // OS key-repeat just re-sends "down", which is harmless now that we track state
            switch ((Keys)m.WParam.ToInt32())
            {
                case Keys.C: m_io.SetCoin(down); break;       // Coin
                case Keys.D1: m_io.SetStart1(down); break;    // Start player 1
                case Keys.D2: m_io.SetStart2(down); break;    // Start player 2
                case Keys.T: m_io.SetTilt(down); break;       // Tilt
                case Keys.Left: m_io.SetLeft(down); break;    // Move left
                case Keys.Right: m_io.SetRight(down); break;  // Move right
                case Keys.Space: m_io.SetFire(down); break;   // Fire
                default: return false;
            }

            return true; // handled
        }

        protected override void OnDeactivate(EventArgs e)
        {
            m_io?.ReleaseAll(); // we won't see the key up if focus is lost while a key is held
            base.OnDeactivate(e);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            Application.RemoveMessageFilter(this);
            timeEndPeriod(1);
            base.OnFormClosed(e);
        }

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
    }
}