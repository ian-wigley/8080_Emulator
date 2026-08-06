using Emulator;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Microsoft.Testing.Platform.Extensions.TestFramework;

namespace EmulatorTests
{
    public class TestCpuEmulationTracing : CPU
    {
        private readonly List<Trace> _iterationTraces = new List<Trace>();
        private readonly List<Trace> _perIterationTraces = new List<Trace>();

        public TestCpuEmulationTracing(List<byte> rom, IO io, Label label) : base(rom, io, label)
        {
        }

        public void Execute()
        {
            while (iteration < 1)
            {
                Run();
            }

            using (var writer = new StreamWriter("csharp_per_iteration_trace.txt"))
            {
                foreach (var trace in _perIterationTraces)
                {
                    writer.WriteLine(trace);
                }
            }
            
            using (var writer = new StreamWriter("csharp_iterations_trace.txt"))
            {
                foreach (var trace in _iterationTraces)
                {
                    writer.WriteLine(trace);
                }
            }
        }

        private new void Run()
        {
            for (var i = 0; i < instruction_per_frame; i++) ExecuteInstruction();

            if (CRASHED) return;
            PopulateIterationsTraceInformation();
            iteration += 1;
        }

        private void PopulateIterationsTraceInformation()
        {
            _iterationTraces.Add(PopulateTrace(""));
        }

        public Trace PopulateTrace(string opcode)
        {
            return new Trace
            {
                iteration = iteration,
                opcode = opcode,
                pc = PC,
                sp = SP,
                a = A,
                b = B,
                c = C,
                d = D,
                e = E,
                h = H,
                l = L,
                bc = BC,
                de = DE,
                hl = HL,
                sign = SIGN,
                zero = ZERO,
                carry = CARRY,
                halfcarry = HALFCARRY,
                parity = false,
                interrupt = false,
                Memory =
                {
                    [9206] = rom[9206],
                    [9207] = rom[9207],
                    [9210] = rom[9210],
                    [9211] = rom[9211],
                    [9212] = rom[9212],
                    [9213] = rom[9213],
                    [9214] = rom[9214],
                    [9215] = rom[9215],
                }
            };
        } 
        
        
        public void OutputInfo(string opcode)
        {
            _perIterationTraces.Add(PopulateTrace(opcode));
        }

        private new void ExecuteInstruction()
        {
            if (CRASHED) return;
            bytes = FetchRomByte();

            switch (bytes)
            {
                case 0x00:
                    NOP();
                    OutputInfo("NOP");
                    break;
                case 0xc3:
                case 0xc2:
                case 0xca:
                case 0xd2:
                case 0xda:
                case 0xf2:
                case 0xfa:
                    Instruction_JMP(bytes);
                    OutputInfo("JMP");
                    break;
                case 0x01:
                case 0x11:
                case 0x21:
                case 0x31:
                    Instruction_LXI(bytes);
                    OutputInfo("LXI");
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
                    OutputInfo("MVI");
                    break;
                case 0xcd:
                case 0xc4:
                case 0xcc:
                case 0xd4:
                case 0xdc:
                    Instruction_CALL(bytes);
                    OutputInfo("CALL");
                    break;
                case 0x0a:
                case 0x1a:
                case 0x3a:
                    Instruction_LDA(bytes);
                    OutputInfo("LDA");
                    break;
                case 0x77:
                case 0x70:
                case 0x71:
                case 0x72:
                case 0x73:
                case 0x74:
                case 0x75:
                    Instruction_MOVHL(bytes);
                    OutputInfo("MOVHL");
                    break;
                case 0x03:
                case 0x13:
                case 0x23:
                case 0x33:
                    Instruction_INX(bytes);
                    OutputInfo("INX");
                    break;
                case 0x0b:
                case 0x1b:
                case 0x2b:
                case 0x3b:
                    Instruction_DCX(bytes);
                    OutputInfo("DCX");
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
                    OutputInfo("DEC");
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
                    OutputInfo("INC");
                    break;
                case 0xc9:
                case 0xc0:
                case 0xc8:
                case 0xd0:
                case 0xd8:
                    Instruction_RET(bytes);
                    OutputInfo("RET");
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
                    OutputInfo("MOV");
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
                    OutputInfo("MOV");
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
                    OutputInfo("MOV");
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
                    OutputInfo("MOV");
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
                    OutputInfo("MOV");
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
                    OutputInfo("MOV");
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
                    OutputInfo("MOV");
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
                    OutputInfo("CMP");
                    break;
                case 0xc5:
                case 0xd5:
                case 0xe5:
                case 0xf5:
                    Instruction_PUSH(bytes);
                    OutputInfo("PUSH");
                    break;
                case 0xc1:
                case 0xd1:
                case 0xe1:
                case 0xf1:
                    Instruction_POP(bytes);
                    OutputInfo("POP");
                    break;
                case 0x09:
                case 0x19:
                case 0x29:
                case 0x39:
                    Instruction_DAD(bytes);
                    OutputInfo("DAD");
                    break;
                case 0xeb:
                    Instruction_XCHG();
                    OutputInfo("XCHG");
                    break;
                case 0xe3:
                    Instruction_XTHL();
                    OutputInfo("XTHL");
                    break;
                case 0xd3:
                    Instruction_OUTP();
                    OutputInfo("OUTP");
                    break;
                case 0xdb:
                    Instruction_INP();
                    OutputInfo("INP");
                    break;
                case 0xe9:
                    Instruction_PCHL();
                    OutputInfo("PCHL");
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
                    OutputInfo("RST");
                    break;
                case 0x07:
                    Instruction_RLC();
                    OutputInfo("RLC");
                    break;
                case 0x17:
                    Instruction_RAL();
                    OutputInfo("RAL");
                    break;
                case 0x0f:
                    Instruction_RRC();
                    OutputInfo("RRC");
                    break;
                case 0x1f:
                    Instruction_RAR();
                    OutputInfo("RAR");
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
                    OutputInfo("AND");
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
                    OutputInfo("ADD");
                    break;
                case 0x02:
                case 0x12:
                case 0x32:
                    Instruction_STA(bytes);
                    OutputInfo("STA");
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
                    OutputInfo("XOR");
                    break;
                case 0xf3:
                    Instruction_DI();
                    OutputInfo("DI");
                    break;
                case 0xfb:
                    Instruction_EI();
                    OutputInfo("EI");
                    break;
                case 0x37:
                    Instruction_STC();
                    OutputInfo("STC");
                    break;
                case 0x3f:
                    Instruction_CMC();
                    OutputInfo("CMC");
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
                    OutputInfo("OR");
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
                    OutputInfo("SUB");
                    break;
                case 0x2a:
                    Instruction_LHLD();
                    OutputInfo("LHLD");
                    break;
                case 0x22:
                    Instruction_SHLD();
                    OutputInfo("SHLD");
                    break;
                case 0xde:
                    Instruction_SBBI();
                    OutputInfo("SBBI");
                    break;
                case 0x27:
                    Instruction_DAA();
                    OutputInfo("DAA");
                    break;
                case 0x2f:
                    Instruction_CMA();
                    OutputInfo("CMA");
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
                    OutputInfo("ADC");
                    break;
                default:
                    CRASHED = true;
                    MessageBox.Show("Emulator Crashed @ instruction : " + instructionCounter + " " + bytes);
                    break;
            }

            instructionCounter++;
            if (instructionCounter >= half_instruction_per_frame)
            {
                if (INTERRUPT)
                {
                    // There are two interrupts that occur every frame (address $08 and $10)
                    if (interrupt_alternate == 0)
                        CallInterrupt(0x08);
                    else
                        CallInterrupt(0x10);
                }

                interrupt_alternate = 1 - interrupt_alternate;
                instructionCounter = 0;
            }
        }
    }

    [TestFixture]
    public class EmulateTests
    {
        [Test]
        public void EmulateTest()
        {
            var rom = new List<byte>();

            var path = Directory.GetCurrentDirectory();
            path += "/invaders.rom";
            if (File.Exists(path))
            {
                using (var b = new BinaryReader(File.Open(path, FileMode.Open)))
                {
                    // Read the input stream & display the contents
                    while (b.BaseStream.Position < b.BaseStream.Length) rom.Add(b.ReadByte());
                }

                var start = rom.Count;
                var end = rom.Count * 2;

                for (var i = start; i < end + 1001; i++) rom.Add(0);

                var io = new IO();
                var emulatorTracing = new TestCpuEmulationTracing(rom, io, null);
                io.SetCPU(emulatorTracing);
                emulatorTracing.Execute();
            }
        }
    }
}