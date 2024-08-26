using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Emulator
{
    public class CPU
    {
        public List<byte> rom;

        public int PC;    // Program Counter: This is the current instruction pointer. 16-bit register.

        public ushort SP; // Stack Pointer. 16-bit register
        public ushort A;  // Accumulator. 8-bit register
        public ushort B;  // Register B. 8-bit register
        public ushort C;  // Register C. 8-bit register
        public ushort D;  // Register D. 8-bit register
        public ushort E;  // Register E. 8-bit register
        public ushort H;  // Register H. 8-bit register
        public ushort L;  // Register L. 8-bit register
        public ushort BC; // Virtual register BC (16-bit) combination of registers B and C
        public ushort DE; // Virtual register DE (16-bit) combination of registers D and E
        public ushort HL; // Virtual register HL (16-bit) combination of registers H and L

        public ushort SIGN = 0;        // Sign flag
        public ushort ZERO = 0;        // Zero flag
        public ushort CARRY = 0;       // Carry flag
        public ushort HALFCARRY = 0;   // Half-carry (or Auxiliary Carry) flag

        public bool PARITY = false;    // Parity flag
        public bool INTERRUPT = false; // Interrupt Enabled flag
        public bool CRASHED = false;   // Special flag that tells if the CPU is currently crashed (stopped)

        public int instruction_per_frame = 4000; // Approximate real machine speed

        // Additional debug fields, not used by CPU
        public byte BIT0 = 1;
        public byte BIT4 = 16;
        public byte BIT5 = 32;
        public byte BIT6 = 64;
        public byte BIT7 = 128;

        // Interrupt handling
        public int interrupt_alternate = 0;
        public int half_instruction_per_frame = 0;

        public ushort source = 0;
        public ushort value = 0;
        public byte bytes = 0;

        public int instructionCounter = 0;
        public int iteration = 0;

        public IO io;
        public Label label;

        public CPU(List<byte> rom, IO io, Label label)
        {
            PC = 0;
            this.rom = rom;
            this.io = io;
            this.label = label;
            half_instruction_per_frame = instruction_per_frame / 2;
            Reset();
        }

        public void Run()
        {
            for (int i = 0; i < instruction_per_frame; i++)
            {
                ExecuteInstruction();
            }

            if (!CRASHED)
            {
                iteration += 1;
            }
        }

        // All opcodes are 1 byte wide
        public void ExecuteInstruction()
        {
            if (!CRASHED)
            {
                bytes = FetchRomByte();

                switch (bytes)
                {
                    case 0x00:
                        NOP();
                        break;
                    case 0xc3:
                    case 0xc2:
                    case 0xca:
                    case 0xd2:
                    case 0xda:
                    case 0xf2:
                    case 0xfa:
                        Instruction_JMP(bytes);
                        break;
                    case 0x01:
                    case 0x11:
                    case 0x21:
                    case 0x31:
                        Instruction_LXI(bytes);
                        break;
                    case 0x3e:
                    case 0x06:
                    case 0x0e:
                    case 0x16:
                    case 0x1e:
                    case 0x26:
                    case 0x2e:
                    case 0x36:
                        Instruction_MVI(bytes);
                        break;
                    case 0xcd:
                    case 0xc4:
                    case 0xcc:
                    case 0xd4:
                    case 0xdc:
                        Instruction_CALL(bytes);
                        break;
                    case 0x0a:
                    case 0x1a:
                    case 0x3a:
                        Instruction_LDA(bytes);
                        break;
                    case 0x77:
                    case 0x70:
                    case 0x71:
                    case 0x72:
                    case 0x73:
                    case 0x74:
                    case 0x75:
                        Instruction_MOVHL(bytes);
                        break;
                    case 0x03:
                    case 0x13:
                    case 0x23:
                    case 0x33:
                        Instruction_INX(bytes);
                        break;
                    case 0x0b:
                    case 0x1b:
                    case 0x2b:
                    case 0x3b:
                        Instruction_DCX(bytes);
                        break;
                    case 0x3d:
                    case 0x05:
                    case 0x0d:
                    case 0x15:
                    case 0x1d:
                    case 0x25:
                    case 0x2d:
                    case 0x35:
                        Instruction_DEC(bytes);
                        break;
                    case 0x3c:
                    case 0x04:
                    case 0x0c:
                    case 0x14:
                    case 0x1c:
                    case 0x24:
                    case 0x2c:
                    case 0x34:
                        Instruction_INC(bytes);
                        break;
                    case 0xc9:
                    case 0xc0:
                    case 0xc8:
                    case 0xd0:
                    case 0xd8:
                        Instruction_RET(bytes);
                        break;
                    case 0x7F:
                    case 0x78:
                    case 0x79:
                    case 0x7A:
                    case 0x7B:
                    case 0x7C:
                    case 0x7D:
                    case 0x7E:
                        Instruction_MOV(bytes);
                        break;
                    case 0x47:
                    case 0x40:
                    case 0x41:
                    case 0x42:
                    case 0x43:
                    case 0x44:
                    case 0x45:
                    case 0x46:
                        Instruction_MOV(bytes);
                        break;
                    case 0x4f:
                    case 0x48:
                    case 0x49:
                    case 0x4a:
                    case 0x4b:
                    case 0x4c:
                    case 0x4d:
                    case 0x4e:
                        Instruction_MOV(bytes);
                        break;
                    case 0x57:
                    case 0x50:
                    case 0x51:
                    case 0x52:
                    case 0x53:
                    case 0x54:
                    case 0x55:
                    case 0x56:
                        Instruction_MOV(bytes);
                        break;
                    case 0x5f:
                    case 0x58:
                    case 0x59:
                    case 0x5a:
                    case 0x5b:
                    case 0x5c:
                    case 0x5d:
                    case 0x5e:
                        Instruction_MOV(bytes);
                        break;
                    case 0x67:
                    case 0x60:
                    case 0x61:
                    case 0x62:
                    case 0x63:
                    case 0x64:
                    case 0x65:
                    case 0x66:
                        Instruction_MOV(bytes);
                        break;
                    case 0x6f:
                    case 0x68:
                    case 0x69:
                    case 0x6a:
                    case 0x6b:
                    case 0x6c:
                    case 0x6d:
                    case 0x6e:
                        Instruction_MOV(bytes);
                        break;
                    case 0xbf:
                    case 0xb8:
                    case 0xb9:
                    case 0xba:
                    case 0xbb:
                    case 0xbc:
                    case 0xbd:
                    case 0xbe:
                    case 0xfe:
                        Instruction_CMP(bytes);
                        break;
                    case 0xc5:
                    case 0xd5:
                    case 0xe5:
                    case 0xf5:
                        Instruction_PUSH(bytes);
                        break;
                    case 0xc1:
                    case 0xd1:
                    case 0xe1:
                    case 0xf1:
                        Instruction_POP(bytes);
                        break;
                    case 0x09:
                    case 0x19:
                    case 0x29:
                    case 0x39:
                        Instruction_DAD(bytes);
                        break;
                    case 0xeb:
                        Instruction_XCHG();
                        break;
                    case 0xe3:
                        Instruction_XTHL();
                        break;
                    case 0xd3:
                        Instruction_OUTP();
                        break;
                    case 0xdb:
                        Instruction_INP();
                        break;
                    case 0xe9:
                        Instruction_PCHL();
                        break;
                    case 0xc7:
                    case 0xcf:
                    case 0xd7:
                    case 0xdf:
                    case 0xe7:
                    case 0xef:
                    case 0xf7:
                    case 0xff:
                        Instruction_RST(bytes);
                        break;
                    case 0x07:
                        Instruction_RLC();
                        break;
                    case 0x17:
                        Instruction_RAL();
                        break;
                    case 0x0f:
                        Instruction_RRC();
                        break;
                    case 0x1f:
                        Instruction_RAR();
                        break;
                    case 0xa7:
                    case 0xa0:
                    case 0xa1:
                    case 0xa2:
                    case 0xa3:
                    case 0xa4:
                    case 0xa5:
                    case 0xa6:
                    case 0xe6:
                        Instruction_AND(bytes);
                        break;
                    case 0x80:
                    case 0x81:
                    case 0x82:
                    case 0x83:
                    case 0x84:
                    case 0x85:
                    case 0x86:
                    case 0x87:
                    case 0xc6:
                        Instruction_ADD(bytes);
                        break;
                    case 0x02:
                    case 0x12:
                    case 0x32:
                        Instruction_STA(bytes);
                        break;
                    case 0xaf:
                    case 0xa8:
                    case 0xa9:
                    case 0xaa:
                    case 0xab:
                    case 0xac:
                    case 0xad:
                    case 0xae:
                    case 0xee:
                        Instruction_XOR(bytes);
                        break;
                    case 0xf3:
                        Instruction_DI();
                        break;
                    case 0xfb:
                        Instruction_EI();
                        break;
                    case 0x37:
                        Instruction_STC();
                        break;
                    case 0x3f:
                        Instruction_CMC();
                        break;
                    case 0xb7:
                    case 0xb0:
                    case 0xb1:
                    case 0xb2:
                    case 0xb3:
                    case 0xb4:
                    case 0xb5:
                    case 0xb6:
                    case 0xf6:
                        Instruction_OR(bytes);
                        break;
                    case 0x97:
                    case 0x90:
                    case 0x91:
                    case 0x92:
                    case 0x93:
                    case 0x94:
                    case 0x95:
                    case 0x96:
                    case 0xd6:
                        Instruction_SUB(bytes);
                        break;
                    case 0x2a:
                        Instruction_LHLD();
                        break;
                    case 0x22:
                        Instruction_SHLD();
                        break;
                    case 0xde:
                        Instruction_SBBI();
                        break;
                    case 0x27:
                        Instruction_DAA();
                        break;
                    case 0x2f:
                        Instruction_CMA();
                        break;
                    case 0x8f:
                    case 0x88:
                    case 0x89:
                    case 0x8a:
                    case 0x8b:
                    case 0x8c:
                    case 0x8d:
                    case 0x8e:
                    case 0xce:
                        Instruction_ADC(bytes);
                        break;
                    default:
                        CRASHED = true;
                        MessageBox.Show("Emulator Crashed @ instruction : " + instructionCounter.ToString() + " " + bytes.ToString());
                        break;
                }

                instructionCounter++;
                if (instructionCounter >= half_instruction_per_frame)
                {

                    if (INTERRUPT)
                    {
                        // There are two interrupts that occur every frame (address $08 and $10)
                        if (interrupt_alternate == 0)
                        {
                            CallInterrupt(0x08);
                        }
                        else
                        {
                            CallInterrupt(0x10);
                        }
                    }
                    interrupt_alternate = 1 - interrupt_alternate;
                    instructionCounter = 0;
                }
            }
        }

        public void CallInterrupt(short inAddress)
        {
            // Call the interrupt by pushing current PC on the stack and then jump to interrupt address
            INTERRUPT = false;
            StackPush((ushort)PC);
            PC = inAddress;
        }

        public void NOP()
        {
            // No Operation - Do nothing !
        }

        public void Instruction_JMP(byte inByte)
        {
            ushort data16 = FetchRomShort();
            var m_condition = true;

            switch (inByte)
            {
                case 0xc3:
                    // Do nothing apart from incrementing the Programme Counter
                    break;
                case 0xc2:
                    m_condition = !Convert.ToBoolean(ZERO);
                    break;
                case 0xca:
                    m_condition = Convert.ToBoolean(ZERO);
                    break;
                case 0xd2:
                    m_condition = !Convert.ToBoolean(CARRY);
                    break;
                case 0xda:
                    m_condition = Convert.ToBoolean(CARRY);
                    break;
                case 0xf2:
                    m_condition = !Convert.ToBoolean(SIGN);
                    break;
                case 0xfa:
                    m_condition = Convert.ToBoolean(SIGN);
                    break;
            }
            if (m_condition)
            {
                PC = data16;
            }
        }

        public void Instruction_LXI(byte inByte)
        {
            switch (inByte)
            {
                case 0x01:
                    SetBC(FetchRomShort());
                    break;
                case 0x11:
                    SetDE(FetchRomShort());
                    break;
                case 0x21:
                    SetHL(FetchRomShort());
                    break;
                case 0x31:
                    SetSP(FetchRomShort());
                    break;
            }
        }

        public void Instruction_MVI(byte inByte)
        {
            switch (inByte)
            {
                case 0x3e:
                    SetA(FetchRomByte());
                    break;
                case 0x06:
                    SetB(FetchRomByte());
                    break;
                case 0x0e:
                    SetC(FetchRomByte());
                    break;
                case 0x16:
                    SetD(FetchRomByte());
                    break;
                case 0x1e:
                    SetE(FetchRomByte());
                    break;
                case 0x26:
                    SetH(FetchRomByte());
                    break;
                case 0x2e:
                    SetL(FetchRomByte());
                    break;
                case 0x36:
                    WriteByte(HL, FetchRomByte());
                    break;
            }
        }

        public void Instruction_CALL(byte inByte)
        {
            ushort data16 = FetchRomShort();
            bool m_condition = true;

            switch (inByte)
            {
                case 0xcd:
                    break;
                case 0xc4:
                    m_condition = !Convert.ToBoolean(ZERO);
                    break;
                case 0xcc:
                    m_condition = Convert.ToBoolean(ZERO);
                    break;
                case 0xd4:
                    m_condition = !Convert.ToBoolean(CARRY);
                    break;
                case 0xdc:
                    m_condition = Convert.ToBoolean(CARRY);
                    break;
            }
            if (m_condition)
            {
                StackPush((ushort)PC);
                PC = data16;
            }
        }

        public void Instruction_LDA(byte inByte)
        {
            switch (inByte)
            {
                case 0x0a:
                    source = BC;
                    break;
                case 0x1a:
                    source = DE;
                    break;
                case 0x3a:
                    source = FetchRomShort();
                    break;
            }
            SetA(ReadByte(source));
        }

        public void Instruction_MOVHL(byte inByte)
        {
            switch (inByte)
            {
                case 0x77:
                    WriteByte(HL, A);
                    break;
                case 0x70:
                    WriteByte(HL, B);
                    break;
                case 0x71:
                    WriteByte(HL, C);
                    break;
                case 0x72:
                    WriteByte(HL, D);
                    break;
                case 0x73:
                    WriteByte(HL, E);
                    break;
                case 0x74:
                    WriteByte(HL, H);
                    break;
                case 0x75:
                    WriteByte(HL, L);
                    break;
            }
        }

        public void Instruction_INX(byte inByte)
        {
            switch (inByte)
            {
                case 0x03:
                    SetBC(BC + 1);
                    break;
                case 0x13:
                    SetDE(DE + 1);
                    break;
                case 0x23:
                    SetHL(HL + 1);
                    break;
                case 0x33:
                    SetSP(SP + 1);
                    break;
            }
        }

        public void Instruction_DCX(byte inByte)
        {
            switch (inByte)
            {
                case 0x0b:
                    SetBC(BC - 1);
                    break;
                case 0x1b:
                    SetDE(DE - 1);
                    break;
                case 0x2b:
                    SetHL(HL - 1);
                    break;
                case 0x3b:
                    SetSP(SP - 1);
                    break;
            }
        }

        public void Instruction_DEC(byte inByte)
        {
            switch (inByte)
            {
                case 0x3d:
                    SetA(PerformDec(A));
                    break;
                case 0x05:
                    SetB(PerformDec(B));
                    break;
                case 0x0d:
                    SetC(PerformDec(C));
                    break;
                case 0x15:
                    SetD(PerformDec(D));
                    break;
                case 0x1d:
                    SetE(PerformDec(E));
                    break;
                case 0x25:
                    SetH(PerformDec(H));
                    break;
                case 0x2d:
                    SetL(PerformDec(L));
                    break;
                case 0x35:
                    WriteByte(HL, (byte)PerformDec(ReadByte(HL)));
                    break;
            }
        }

        public void Instruction_INC(byte inByte)
        {
            switch (inByte)
            {
                case 0x3c:
                    SetA(PerformInc(A));
                    break;
                case 0x04:
                    SetB(PerformInc(B));
                    break;
                case 0x0c:
                    SetC(PerformInc(C));
                    break;
                case 0x14:
                    SetD(PerformInc(D));
                    break;
                case 0x1c:
                    SetE(PerformInc(E));
                    break;
                case 0x24:
                    SetH(PerformInc(H));
                    break;
                case 0x2c:
                    SetL(PerformInc(L));
                    break;
                case 0x34:
                    WriteByte(HL, (byte)PerformInc(ReadByte(HL)));
                    break;
            }
        }

        public void Instruction_RET(byte inByte)
        {
            bool m_condition = true;

            switch (inByte)
            {
                case 0xc9:
                    break;
                case 0xc0:
                    m_condition = !Convert.ToBoolean(ZERO);
                    break;
                case 0xc8:
                    m_condition = Convert.ToBoolean(ZERO);
                    break;
                case 0xd0:
                    m_condition = !Convert.ToBoolean(CARRY);
                    break;
                case 0xd8:
                    m_condition = Convert.ToBoolean(CARRY);
                    break;
            }
            if (m_condition)
            {
                PC = StackPop();
            }
        }

        public void Instruction_MOV(byte inByte)
        {
            switch (inByte)
            {
                case 0x7F:
                    SetA(A);
                    break;
                case 0x78:
                    SetA(B);
                    break;
                case 0x79:
                    SetA(C);
                    break;
                case 0x7A:
                    SetA(D);
                    break;
                case 0x7B:
                    SetA(E);
                    break;
                case 0x7C:
                    SetA(H);
                    break;
                case 0x7D:
                    SetA(L);
                    break;
                case 0x7E:
                    SetA(ReadByte(HL));
                    break;
                case 0x47:
                    SetB(A);
                    break;
                case 0x40:
                    SetB(B);
                    break;
                case 0x41:
                    SetB(C);
                    break;
                case 0x42:
                    SetB(D);
                    break;
                case 0x43:
                    SetB(E);
                    break;
                case 0x44:
                    SetB(H);
                    break;
                case 0x45:
                    SetB(L);
                    break;
                case 0x46:
                    SetB(ReadByte(HL));
                    break;
                case 0x4f:
                    SetC(A);
                    break;
                case 0x48:
                    SetC(B);
                    break;
                case 0x49:
                    SetC(C);
                    break;
                case 0x4a:
                    SetC(D);
                    break;
                case 0x4b:
                    SetC(E);
                    break;
                case 0x4c:
                    SetC(H);
                    break;
                case 0x4d:
                    SetC(L);
                    break;
                case 0x4e:
                    SetC(ReadByte(HL));
                    break;
                case 0x57:
                    SetD(A);
                    break;
                case 0x50:
                    SetD(B);
                    break;
                case 0x51:
                    SetD(C);
                    break;
                case 0x52:
                    SetD(D);
                    break;
                case 0x53:
                    SetD(E);
                    break;
                case 0x54:
                    SetD(H);
                    break;
                case 0x55:
                    SetD(L);
                    break;
                case 0x56:
                    SetD(ReadByte(HL));
                    break;
                case 0x5f:
                    SetE(A);
                    break;
                case 0x58:
                    SetE(B);
                    break;
                case 0x59:
                    SetE(C);
                    break;
                case 0x5a:
                    SetE(D);
                    break;
                case 0x5b:
                    SetE(E);
                    break;
                case 0x5c:
                    SetE(H);
                    break;
                case 0x5d:
                    SetE(L);
                    break;
                case 0x5e:
                    SetE(ReadByte(HL));
                    break;
                case 0x67:
                    SetH(A);
                    break;
                case 0x60:
                    SetH(B);
                    break;
                case 0x61:
                    SetH(C);
                    break;
                case 0x62:
                    SetH(D);
                    break;
                case 0x63:
                    SetH(E);
                    break;
                case 0x64:
                    SetH(H);
                    break;
                case 0x65:
                    SetH(L);
                    break;
                case 0x66:
                    SetH(ReadByte(HL));
                    break;
                case 0x6f:
                    SetL(A);
                    break;
                case 0x68:
                    SetL(B);
                    break;
                case 0x69:
                    SetL(C);
                    break;
                case 0x6a:
                    SetL(D);
                    break;
                case 0x6b:
                    SetL(E);
                    break;
                case 0x6c:
                    SetL(H);
                    break;
                case 0x6d:
                    SetL(L);
                    break;
                case 0x6e:
                    SetL(ReadByte(HL));
                    break;
            }
        }

        public void Instruction_CMP(byte inByte)
        {
            switch (inByte)
            {
                case 0xbf:
                    value = A;
                    break;
                case 0xb8:
                    value = B;
                    break;
                case 0xb9:
                    value = C;
                    break;
                case 0xba:
                    value = D;
                    break;
                case 0xbb:
                    value = E;
                    break;
                case 0xbc:
                    value = H;
                    break;
                case 0xbd:
                    value = L;
                    break;
                case 0xbe:
                    value = ReadByte(HL);
                    break;
                case 0xfe:
                    value = FetchRomByte();
                    break;
            }
            PerformCompSub((byte)value);
        }

        public void Instruction_PUSH(byte inByte)
        {
            switch (inByte)
            {
                case 0xc5:
                    value = BC;
                    break;
                case 0xd5:
                    value = DE;
                    break;
                case 0xe5:
                    value = HL;
                    break;
                case 0xf5:
                    value = (ushort)(A << 8);
                    if (Convert.ToBoolean(SIGN))
                    {
                        value = (ushort)(value | BIT7);
                    }
                    if (Convert.ToBoolean(ZERO))
                    {
                        value = (ushort)(value | BIT6);
                    }
                    if (INTERRUPT)
                    {
                        value = (ushort)(value | BIT5);
                    }
                    if (Convert.ToBoolean(HALFCARRY))
                    {
                        value = (ushort)(value | BIT4);
                    }
                    if (Convert.ToBoolean(CARRY))
                    {
                        value = (ushort)(value | BIT0);
                    }
                    break;
            }
            StackPush(value);
        }

        public void Instruction_POP(byte inByte)
        {
            value = StackPop();
            switch (inByte)
            {
                case 0xc1:
                    SetBC(value);
                    break;
                case 0xd1:
                    SetDE(value);
                    break;
                case 0xe1:
                    SetHL(value);
                    break;
                case 0xf1:
                    A = (byte)(value >> 8);
                    SIGN = (ushort)(value & 0x80);
                    ZERO = (ushort)(value & 0x40);
                    INTERRUPT = Convert.ToBoolean(value & 0x20);
                    HALFCARRY = (ushort)(value & BIT4);
                    CARRY = (ushort)(value & BIT0);
                    break;
            }
        }

        public void Instruction_DAD(byte inByte)
        {
            switch (inByte)
            {
                case 0x09:
                    AddHL(BC);
                    break;
                case 0x19:
                    AddHL(DE);
                    break;
                case 0x29:
                    AddHL(HL);
                    break;
                case 0x39:
                    AddHL(SP);
                    break;
            }
        }

        public void Instruction_XCHG()
        {
            ushort temp = DE;
            SetDE(HL);
            SetHL(temp);
        }

        public void Instruction_XTHL()
        {
            ushort temp = H;
            SetH(ReadByte(SP + 1));
            WriteByte((ushort)(SP + 1), temp);
            temp = L;
            SetL(ReadByte(SP));
            WriteByte(SP, temp);
        }

        public void Instruction_OUTP()
        {
            byte port = FetchRomByte();
            io.OutputPort(port, (byte)A);
        }

        public void Instruction_INP()
        {
            byte port = FetchRomByte();
            SetA(io.InputPort(port));
        }

        public void Instruction_PCHL()
        {
            PC = HL;
        }

        public void Instruction_RST(byte inByte)
        {
            ushort address = 0;
            switch (inByte)
            {
                case 0xc7:
                    address = 0x00;
                    break;
                case 0xcf:
                    address = 0x08;
                    break;
                case 0xd7:
                    address = 0x10;
                    break;
                case 0xdf:
                    address = 0x18;
                    break;
                case 0xe7:
                    address = 0x20;
                    break;
                case 0xef:
                    address = 0x28;
                    break;
                case 0xf7:
                    address = 0x30;
                    break;
                case 0xff:
                    address = 0x38;
                    break;
            }
            StackPush((ushort)PC);
            PC = address;
        }

        public void Instruction_RLC()
        {
            SetA((ushort)((A << 1) | (A >> 7)));
            var temp = (A & 1);
            bool testCarry = Convert.ToBoolean(temp);
            CARRY = (ushort)(A & BIT0);
        }

        public void Instruction_RAL()
        {
            ushort temp = A;
            SetA((ushort)(A << 1));
            if (Convert.ToBoolean(CARRY))
            {
                SetA((ushort)(A | BIT0));
            }

            CARRY = (ushort)(temp & 0x80);
        }

        public void Instruction_RRC()
        {
            SetA((ushort)((A >> 1) | (A << 7)));
            CARRY = (ushort)(A & BIT7);
        }

        public void Instruction_RAR()
        {
            ushort temp = A;
            SetA((ushort)(A >> 1));
            if (Convert.ToBoolean(CARRY))
            {
                SetA((ushort)(A | BIT7));
            }
            CARRY = (ushort)(temp & 1);
        }

        public void Instruction_AND(byte inByte)
        {
            switch (inByte)
            {
                case 0xa7:
                    PerformAnd(A);
                    break;
                case 0xa0:
                    PerformAnd(B);
                    break;
                case 0xa1:
                    PerformAnd(C);
                    break;
                case 0xa2:
                    PerformAnd(D);
                    break;
                case 0xa3:
                    PerformAnd(E);
                    break;
                case 0xa4:
                    PerformAnd(H);
                    break;
                case 0xa5:
                    PerformAnd(L);
                    break;
                case 0xa6:
                    PerformAnd(ReadByte(HL));
                    break;
                case 0xe6:
                    byte immediate = FetchRomByte();
                    PerformAnd(immediate);
                    break;
            }
        }

        public void Instruction_ADD(byte inByte)
        {
            switch (inByte)
            {
                case 0x87:
                    PerformByteAdd(A, 0);
                    break;
                case 0x80:
                    PerformByteAdd(B, 0);
                    break;
                case 0x81:
                    PerformByteAdd(C, 0);
                    break;
                case 0x82:
                    PerformByteAdd(D, 0);
                    break;
                case 0x83:
                    PerformByteAdd(E, 0);
                    break;
                case 0x84:
                    PerformByteAdd(H, 0);
                    break;
                case 0x85:
                    PerformByteAdd(L, 0);
                    break;
                case 0x86:
                    PerformByteAdd(ReadByte(HL), 0);
                    break;
                case 0xc6:
                    byte immediate = FetchRomByte();
                    PerformByteAdd(immediate, 0);
                    break;
            }
        }

        public void Instruction_STA(byte inByte)
        {
            switch (inByte)
            {
                case 0x02:
                    WriteByte(BC, A);
                    break;
                case 0x12:
                    WriteByte(DE, A);
                    break;
                case 0x32:
                    ushort immediate = FetchRomShort();
                    WriteByte(immediate, A);
                    break;
            }
        }

        public void Instruction_XOR(byte inByte)
        {
            switch (inByte)
            {
                case 0xaf:
                    PerformXor(A);
                    break;
                case 0xa8:
                    PerformXor(B);
                    break;
                case 0xa9:
                    PerformXor(C);
                    break;
                case 0xaa:
                    PerformXor(D);
                    break;
                case 0xab:
                    PerformXor(E);
                    break;
                case 0xac:
                    PerformXor(H);
                    break;
                case 0xad:
                    PerformXor(L);
                    break;
                case 0xae:
                    PerformXor(ReadByte(HL));
                    break;
                case 0xee:
                    byte immediate = FetchRomByte();
                    PerformXor(immediate);
                    break;
            }
        }

        public void Instruction_DI()
        {
            INTERRUPT = false;
        }

        public void Instruction_EI()
        {
            INTERRUPT = true;
        }

        public void Instruction_STC()
        {
            CARRY = 1;
        }

        public void Instruction_CMC()
        {
            CARRY = 0;
        }

        public void Instruction_OR(byte inByte)
        {
            switch (inByte)
            {
                case 0xb7:
                    PerformOr(A);
                    break;
                case 0xb0:
                    PerformOr(B);
                    break;
                case 0xb1:
                    PerformOr(C);
                    break;
                case 0xb2:
                    PerformOr(D);
                    break;
                case 0xb3:
                    PerformOr(E);
                    break;
                case 0xb4:
                    PerformOr(H);
                    break;
                case 0xb5:
                    PerformOr(L);
                    break;
                case 0xb6:
                    PerformOr(ReadByte(HL));
                    break;
                case 0xf6:
                    byte immediate = FetchRomByte();
                    PerformOr(immediate);
                    break;
            }
        }

        public void Instruction_SUB(byte inByte)
        {
            switch (inByte)
            {
                case 0x97:
                    PerformByteSub(A, 0);
                    break;
                case 0x90:
                    PerformByteSub(B, 0);
                    break;
                case 0x91:
                    PerformByteSub(C, 0);
                    break;
                case 0x92:
                    PerformByteSub(D, 0);
                    break;
                case 0x93:
                    PerformByteSub(E, 0);
                    break;
                case 0x94:
                    PerformByteSub(H, 0);
                    break;
                case 0x95:
                    PerformByteSub(L, 0);
                    break;
                case 0x96:
                    PerformByteSub(ReadByte(HL), 0);
                    break;
                case 0xd6:
                    byte immediate = FetchRomByte();
                    PerformByteSub(immediate, 0);
                    break;
            }
        }

        public void Instruction_LHLD()
        {
            ushort immediate = FetchRomShort();
            SetHL(ReadShort(immediate));
        }

        public void Instruction_SHLD()
        {
            ushort immediate = FetchRomShort();
            WriteShort(immediate, HL);
        }

        public void Instruction_SBBI()
        {
            byte immediate = FetchRomByte();
            byte carryvalue = 0;
            if (Convert.ToBoolean(CARRY))
            {
                carryvalue = 1;
            }
            PerformByteSub(immediate, carryvalue);
        }

        public void Instruction_DAA()
        {
            if (((A & 0x0F) > 9) || Convert.ToBoolean(HALFCARRY))
            {
                A += 0x06;
                HALFCARRY = 1;
            }
            else
            {
                HALFCARRY = 0;
            }

            if ((A > 0x9F) || (Convert.ToBoolean(CARRY)))
            {
                A += 0x60;
                CARRY = 1;
            }
            else
            {
                CARRY = 0;
            }
            setFlagZeroSign();
        }

        public void Instruction_CMA()
        {
            SetA((ushort)(A ^ 0xff));
        }

        public void Instruction_ADC(byte inByte)
        {
            byte carryvalue = 0;
            if (Convert.ToBoolean(CARRY))
            {
                carryvalue = 1;
            }
            switch (inByte)
            {
                case 0x8f:
                    PerformByteAdd(A, carryvalue);
                    break;
                case 0x88:
                    PerformByteAdd(B, carryvalue);
                    break;
                case 0x89:
                    PerformByteAdd(C, carryvalue);
                    break;
                case 0x8a:
                    PerformByteAdd(D, carryvalue);
                    break;
                case 0x8b:
                    PerformByteAdd(E, carryvalue);
                    break;
                case 0x8c:
                    PerformByteAdd(H, carryvalue);
                    break;
                case 0x8d:
                    PerformByteAdd(L, carryvalue);
                    break;
                case 0x8e:
                    PerformByteAdd(ReadByte(HL), carryvalue);
                    break;
                case 0xce:
                    byte immediate = FetchRomByte();
                    PerformByteAdd(immediate, carryvalue);
                    break;
            }
        }

        public void SetA(ushort inByte)
        {
            A = (ushort)(inByte & 0xFF);
        }

        public void SetB(int inByte)
        {
            B = (ushort)(inByte & 0xFF);
            BC = (ushort)((B << 8) | C);
        }

        public void SetC(int inByte)
        {
            C = (ushort)(inByte & 0xFF);
            BC = (ushort)((B << 8) | C);
        }

        public void SetD(int inByte)
        {
            D = (ushort)inByte;
            DE = (ushort)((D << 8) + E);
        }

        public void SetE(int inByte)
        {
            E = (byte)inByte;
            DE = (ushort)((D << 8) + E);
        }

        public void SetH(int inByte)
        {
            H = (ushort)(inByte);
            HL = (ushort)((H << 8) + L);
        }

        public void SetL(int inByte)
        {
            L = (ushort)inByte;
            HL = (ushort)((H << 8) + L);
        }

        public void SetBC(int inShort)
        {
            BC = (ushort)inShort;
            B = (ushort)(BC >> 8);
            C = (ushort)(BC & 0xFF);
        }

        public void SetDE(int inShort)
        {
            DE = (ushort)inShort;
            D = (ushort)(DE >> 8);
            E = (ushort)(DE & 0xFF);
        }

        public void SetHL(int inShort)
        {
            HL = (ushort)inShort;
            H = (ushort)(HL >> 8);
            L = (ushort)(HL & 0xFF);
        }

        public void SetSP(int inShort)
        {
            SP = (ushort)inShort;
        }

        public byte FetchRomByte()
        {
            byte value = rom[PC];
            PC += 1;
            return value;
        }

        public ushort FetchRomShort()
        {
            byte[] bytes = new byte[2];
            bytes[0] = rom[PC + 0];
            bytes[1] = rom[PC + 1];
            PC += 2;
            return BitConverter.ToUInt16(bytes, 0);
        }

        public byte ReadByte(int count)
        {
            return rom[count];
        }

        public ushort ReadShort(ushort inAddress)
        {
            return (ushort)((rom[inAddress + 1] << 8) + (rom[inAddress + 0]));
        }

        public void WriteShort(ushort inAddress, ushort inWord)
        {
            rom[inAddress + 1] = (byte)(inWord >> 8);
            rom[inAddress + 0] = (byte)(inWord);
        }

        public void WriteByte(ushort inAddress, ushort inByte)
        {
            rom[inAddress] = (byte)(inByte);
        }

        public void StackPush(ushort inValue)
        {
            SP -= 2;
            WriteShort(SP, inValue);
        }

        public ushort StackPop()
        {
            ushort temp = ReadShort(SP);
            SP += 2;
            return temp;
        }

        public ushort PerformDec(ushort inSource)
        {
            ushort value = (ushort)((inSource - 1) & 0xFF);
            HALFCARRY = Convert.ToUInt16((value & 0x0F) == 0);
            ZERO = Convert.ToUInt16((value & 255) == 0);
            SIGN = (ushort)(value & 128);
            return value;
        }

        public ushort PerformInc(ushort inSource)
        {
            ushort value = (ushort)(inSource + 1);
            HALFCARRY = Convert.ToUInt16((value & 0xF) < 0 || (value & 0xF) > 0);
            ZERO = Convert.ToUInt16((value & 255) == 0);
            SIGN = (ushort)(value & 128);
            return value;
        }

        public void setFlagZeroSign()
        {
            ZERO = Convert.ToUInt16(A == 0);
            SIGN = (ushort)(A & 128);
        }

        public void PerformAnd(ushort inValue)
        {
            SetA((ushort)(A & inValue));
            CARRY = 0;
            HALFCARRY = 0;
            setFlagZeroSign();
        }

        public void PerformXor(ushort inValue)
        {
            SetA((ushort)(A ^ inValue));
            CARRY = 0;
            HALFCARRY = 0;
            setFlagZeroSign();
        }

        public void PerformOr(ushort inValue)
        {
            SetA((ushort)(A | inValue));
            CARRY = 0;
            HALFCARRY = 0;
            setFlagZeroSign();
        }

        public void PerformByteAdd(ushort inValue, short inCarryValue)
        {
            int value = A + inValue + inCarryValue;
            HALFCARRY = (ushort)((A ^ inValue ^ value) & 0x10);
            SetA((ushort)(value));

            if (value > 255)
            {
                CARRY = 1;
            }
            else
            {
                CARRY = 0;
            }

            setFlagZeroSign();
        }

        public void PerformByteSub(ushort inValue, ushort inCarryValue)
        {
            byte value = (byte)(A - inValue - inCarryValue);

            if ((value >= A) && (inValue | inCarryValue) > 0)
            {
                CARRY = 1;
            }
            else
            {
                CARRY = 0;
            }
            HALFCARRY = (ushort)((A ^ inValue ^ value) & 0x10);
            SetA(value);
            setFlagZeroSign();
        }

        public void PerformCompSub(byte inValue)
        {
            var value = (A - inValue) & 0xFF;
            if ((value >= A) && Convert.ToBoolean(inValue))
            {
                CARRY = inValue;
            }
            else
            {
                CARRY = 0;
            }

            HALFCARRY = (ushort)((A ^ inValue ^ value) & 0x10);
            ZERO = Convert.ToUInt16(value == 0);
            SIGN = (ushort)(value & 128);
        }


        public void AddHL(ushort inValue)
        {
            int value = HL + inValue;
            SetHL(value);
            CARRY = Convert.ToUInt16(value > 65535);
        }

        public void Reset()
        {
            PC = 0;
            A = 0;
            BC = 0;
            DE = 0;
            HL = 0;
            SIGN = 0;
            ZERO = 0;
            HALFCARRY = 0;
            PARITY = false;
            CARRY = 0;
            INTERRUPT = false;
            CRASHED = false;
        }
    }
}