using NUnit.Framework;
using Emulator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Emulator.Tests
{
    [TestFixture()]
    public class CPUTests
    {
        private CPU cpu;
        private IO io;

        [SetUp]
        protected void SetUp()
        {
            List<byte> rom = new List<byte>();
            cpu = new CPU(rom, io, null);
        }

        [Test()]
        public void CPUTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void RunTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void ExecuteInstructionTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void CallInterruptTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void NOPTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void Instruction_JMPTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void Instruction_LXITest()
        {
            Assert.Fail();
        }

        [Test()]
        public void Instruction_MVITest()
        {
            Assert.Fail();
        }

        [Test()]
        public void Instruction_CALLTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void Instruction_LDATest()
        {
            Assert.Fail();
        }

        [Test()]
        public void Instruction_MOVHLTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void Instruction_INXTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void Instruction_DCXTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void Instruction_DECTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void Instruction_INCTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void Instruction_RETTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void Instruction_MOVTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void Instruction_CMPTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void Instruction_PUSHTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void Instruction_POPTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void Instruction_DADTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void Instruction_XCHGTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void Instruction_XTHLTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void Instruction_OUTPTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void Instruction_INPTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void Instruction_PCHLTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void Instruction_RSTTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void Instruction_RLCTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void Instruction_RALTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void Instruction_RRCTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void Instruction_RARTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void Instruction_ANDTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void Instruction_ADDTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void Instruction_STATest()
        {
            Assert.Fail();
        }

        [Test()]
        public void Instruction_XORTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void Instruction_DITest()
        {
            Assert.Fail();
        }

        [Test()]
        public void Instruction_EITest()
        {
            Assert.Fail();
        }

        [Test()]
        public void Instruction_STCTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void Instruction_CMCTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void Instruction_ORTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void Instruction_SUBTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void Instruction_LHLDTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void Instruction_SHLDTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void Instruction_SBBITest()
        {
            Assert.Fail();
        }

        [Test()]
        public void Instruction_DAATest()
        {
            Assert.Fail();
        }

        [Test()]
        public void Instruction_CMATest()
        {
            Assert.Fail();
        }

        [Test()]
        public void Instruction_ADCTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void SetATest()
        {
            Assert.Fail();
        }

        [Test()]
        public void SetBTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void SetCTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void SetDTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void SetETest()
        {
            Assert.Fail();
        }

        [Test()]
        public void SetHTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void SetLTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void SetBCTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void SetDETest()
        {
            Assert.Fail();
        }

        [Test()]
        public void SetHLTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void SetSPTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void FetchRomByteTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void FetchRomShortTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void ReadByteTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void ReadShortTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void WriteShortTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void WriteByteTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void StackPushTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void StackPopTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void PerformDecTest()
        {
            ushort invalue = 256;
            var result = cpu.PerformDec(invalue);
            Assert.AreEqual(result, invalue + 1);
        }

        [Test()]
        public void PerformIncTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void setFlagZeroSignTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void PerformAndTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void PerformXorTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void PerformOrTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void PerformByteAddTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void PerformByteSubTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void PerformCompSubTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void AddHLTest()
        {
            ushort invalue = 256;
            cpu.AddHL(invalue);
            Assert.AreEqual(cpu.HL, invalue);
            Assert.AreEqual(cpu.CARRY, 0);
        }

        [Test()]
        public void ResetTest()
        {
            Assert.Fail();
        }
    }
}