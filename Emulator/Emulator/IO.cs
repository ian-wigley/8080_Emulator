namespace Emulator
{
    public class IO
    {
        private readonly byte BIT0 = 1;
        private readonly byte BIT1 = 2;
        private readonly byte BIT2 = 4;
        private readonly byte BIT4 = 16;
        private readonly byte BIT5 = 32;
        private readonly byte BIT6 = 64;
        private readonly byte BIT7 = 128;
        private byte BIT3 = 8;
        private byte IN_PORT1;

        private byte IN_PORT2;

        // Input and output ports
        private byte OUT_PORT2;
        private byte OUT_PORT3;
        private byte OUT_PORT4HI;
        private byte OUT_PORT4LO;
        private byte OUT_PORT5;

        private CPU m_cpu;

        public IO()
        {
            // Dipswitch: BIT0 and BIT1 controls starting number of life (from 3 to 6)
            // BIT7 prints additional coin message on intro screen
            IN_PORT2 = (byte)(IN_PORT2 | BIT0 | BIT1);
            IN_PORT2 = (byte)(IN_PORT2 | BIT7);
        }


        public void SetCPU(CPU cpu)
        {
            m_cpu = cpu;
        }

        // Key state is *held*, not pulsed: the port bit stays set while the key is down and is
        // cleared on key up (port bit 1 = pressed). The game reads the ports from its interrupt
        // routine, so no per-frame Update() is needed any more.
        private static void SetBit(ref byte port, byte mask, bool down)
        {
            port = (byte)(down ? (port | mask) : (port & ~mask));
        }

        public void SetCoin(bool down)   { SetBit(ref IN_PORT1, BIT0, down); }
        public void SetStart2(bool down) { SetBit(ref IN_PORT1, BIT1, down); }
        public void SetStart1(bool down) { SetBit(ref IN_PORT1, BIT2, down); }
        public void SetTilt(bool down)   { SetBit(ref IN_PORT2, BIT2, down); }

        // Left / right / fire: same key used for player 1 and 2
        public void SetLeft(bool down)
        {
            SetBit(ref IN_PORT1, BIT5, down);
            SetBit(ref IN_PORT2, BIT5, down);
        }

        public void SetRight(bool down)
        {
            SetBit(ref IN_PORT1, BIT6, down);
            SetBit(ref IN_PORT2, BIT6, down);
        }

        public void SetFire(bool down)
        {
            SetBit(ref IN_PORT1, BIT4, down);
            SetBit(ref IN_PORT2, BIT4, down);
        }

        // Call when the window loses focus so no key is left "stuck" down
        public void ReleaseAll()
        {
            IN_PORT1 = (byte)(IN_PORT1 & ~(BIT0 | BIT1 | BIT2 | BIT4 | BIT5 | BIT6));
            IN_PORT2 = (byte)(IN_PORT2 & ~(BIT2 | BIT4 | BIT5 | BIT6));
        }

        public void OutputPort(byte port, byte value)
        {
            switch (port)
            {
                // Port 2 simply stores a 8-bit value
                case 2:
                    OUT_PORT2 = value;
                    break;

                // Port 3 controls play of various sound effects
                case 3:
                    if ((value & BIT0) == 0)
                    {
                        // sound.StartUfo();
                    }

                    // sound.StopUfo();
                    //if ((value & BIT1) && !(OUT_PORT3 & BIT1)) { sound.PlayShot(); }
                    //if ((value & BIT2) && !(OUT_PORT3 &BIT2)) { sound.PlayBaseHit(); }
                    //if ((value & BIT3) && !(OUT_PORT3 &BIT3)) { sound.PlayInvHit(); }
                    //if ((value & BIT4) && !(OUT_PORT3 &BIT4)) { sound.PlayExtraLife(); }
                    //if ((value & BIT5) && !(OUT_PORT3 &BIT5)) { sound.PlayBeginPlay(); }
                    OUT_PORT3 = value;
                    break;

                case 4:
                    OUT_PORT4LO = OUT_PORT4HI;
                    OUT_PORT4HI = value;
                    break;

                // Port 5 also controls sound
                case 5:
                    //if ((value & BIT0) && !(OUT_PORT5 &BIT0)) sound.PlayWalk1();
                    //if ((value & BIT1) && !(OUT_PORT5 &BIT1)) sound.PlayWalk2();
                    //if ((value & BIT2) && !(OUT_PORT5 &BIT2)) sound.PlayWalk3();
                    //if ((value & BIT3) && !(OUT_PORT5 &BIT3)) sound.PlayWalk4();
                    //if ((value & BIT4) && !(OUT_PORT5 &BIT4)) sound.PlayUfoHit();
                    OUT_PORT5 = value;
                    break;
            }
        }

        public byte InputPort(byte port)
        {
            byte result = port switch
            {
                // Player 1 Input keys
                1 => IN_PORT1,
                // Player 2 Input keys And dip switches:
                2 => IN_PORT2,
                3 => (byte)((((OUT_PORT4HI << 8) | OUT_PORT4LO) << OUT_PORT2) >> 8),
                _ => 0
            };

            return result;
        }
    }
}