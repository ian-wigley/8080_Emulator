using System.Collections.Generic;

namespace EmulatorTests
{
    public class Trace
    {
        public int iteration { get; set; }
        public string opcode { get; set; }
        public ushort m_byte { get; set; }
        public int pc { get; set; }
        public ushort sp { get; set; }
        public ushort a { get; set; }
        public ushort b { get; set; }
        public ushort c { get; set; }
        public ushort d { get; set; }
        public ushort e { get; set; }
        public ushort h { get; set; }
        public ushort l { get; set; }
        public ushort bc { get; set; }
        public ushort de { get; set; }
        public ushort hl { get; set; }
        public ushort sign { get; set; }
        public ushort zero { get; set; }
        public ushort carry { get; set; }
        public ushort halfcarry { get; set; }
        public bool parity { get; set; }
        public bool interrupt { get; set; }
        public Dictionary<ushort, byte> memory { get; set; } = new Dictionary<ushort, byte>();
        
        public override string ToString()
        {
            var sb = new System.Text.StringBuilder();
            sb.Append($"Iteration: {iteration}");
            sb.Append($", Opcode: {opcode}");
            sb.Append($", Byte: {m_byte}");
            sb.Append($", PC: {pc}");
            sb.Append($", SP: {sp}");
            sb.Append($", A: {a}");
            sb.Append($", B: {b}");
            sb.Append($", C: {c}");
            sb.Append($", D: {d}");
            sb.Append($", E: {e}");
            sb.Append($", H: {h}");
            sb.Append($", L: {l}");
            sb.Append($", BC: {bc}");
            sb.Append($", DE: {de}");
            sb.Append($", HL: {hl}");
            sb.Append($", SIGN: {sign}");
            sb.Append($", ZERO: {zero}");
            sb.Append($", HALFCARRY: {halfcarry}");
            sb.Append($", PARITY: {parity.ToString().ToLower()}");
            sb.Append($", CARRY: {carry}");
            sb.Append($", INTERRUPT: {interrupt.ToString().ToLower()}");

            foreach (var mem in memory)
            {
                 sb.Append($", {mem.Key}: {mem.Value}");
            }

            return sb.ToString();
        }
    }
}