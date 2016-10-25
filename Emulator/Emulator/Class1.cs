using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Automation_work
{
    class Class1
    {



        /*


        SuperStrict
        Import "util.bmx"
        Import "io.bmx"

        Global cpu: Tcpu = New Tcpu

        Type Tcpu
            Field memory: TBank ' memory contains both ROM and RAM address space
            Field memoryptr: Byte Ptr ' byte ptr to the memory bank (faster access)
            Field PC: Short 'Program Counter: This is the current instruction pointer. 16-bit register.
            Field SP: Short 'Stack Pointer. 16-bit register
            Field A: Byte 'Accumulator. 8-bit register
            Field B: Byte 'Register B. 8-bit register
            Field C: Byte 'Register C. 8-bit register
            Field D: Byte 'Register D. 8-bit register
            Field E: Byte 'Register E. 8-bit register
            Field H: Byte 'Register H. 8-bit register
            Field L: Byte 'Register L. 8-bit register
            Field BC: Short 'Virtual register BC (16-bit) combinaison of registers B and C
            Field DE: Short 'Virtual register DE (16-bit) combinaison of registers D and E
            Field HL: Short 'Virtual register HL (16-bit) combinaison of registers H and L
            Field SIGN: Byte 'Sign flag
            Field ZERO: Byte 'Zero flag
            Field HALFCARRY: Byte 'Half-carry (or Auxiliary Carry) flag
            Field PARITY: Byte 'Parity flag
            Field CARRY: Byte 'Carry flag
            Field INTERRUPT: Byte 'Interrupt Enabled flag
            Field CRASHED:Byte 'Special flag that tells if the CPU is currently crashed (stopped)
	
            Field instruction_per_frame: Int = 4000 ' Approximate real machine speed
            Field current_inst:Byte

            ' Interrupt handling
            Field interrupt_alternate: Int
            Field count_instructions: Int
            Field half_instruction_per_frame: Int = instruction_per_frame Shr 1
	
            ' Addionnal debug fields, not used by CPU
            Field disassembly_pc: Short
            Field debug_output$
	
            Method Instruction_NOP()
                'Disassembly("NOP")
            End Method
            Method Instruction_JMP()
                'Local name$
                Local condition:Byte = True
                Local data16:Short = FetchRomShort()
	
                Select current_inst
                    Case $c3
                        'name = "JMP"
                    Case $c2
                        'name = "JNZ"
                        condition = Not ZERO
                    Case $ca
                        'name = "JZ"
                        condition = ZERO
                    Case $d2
                        'name = "JNC"
                        condition = Not CARRY
                    Case $da
                        'name = "JC"
                        condition = CARRY
                    Case $f2
                        'name = "JP"
                        condition = Not SIGN
                    Case $fa
                        'name = "JM"
                        condition = SIGN
                End Select
	
                'Disassembly(name$+" "+HWord$(data16)+"  condition="+condition)
                If condition Then
                    PC = data16
                End If
            End Method

            Method Instruction_LXI()
                'Local name$
                Local data16:Short = FetchRomShort()
	
                Select current_inst
                    Case $01
                        'name = "BC"
                        SetBC(data16)
                    Case $11
                        'name = "DE"
                        SetDE(data16)
                    Case $21
                        'name = "HL"
                        SetHL(data16)
                    Case $31
                        'name = "SP"
                        SetSP(data16)
                End Select
	
                'Disassembly("LXI "+name$+","+HWord$(data16))
            End Method

            Method Instruction_MVI()
                'Local name$
                Local data8:Byte = FetchRomByte()
	
                Select current_inst
                    Case $3e
                        'name = "A"
                        SetA(data8)
                    Case $06
                        'name = "B"
                        SetB(data8)
                    Case $0e
                        'name = "C"
                        SetC(data8)
                    Case $16
                        'name = "D"
                        SetD(data8)
                    Case $1e
                        'name = "E"
                        SetE(data8)
                    Case $26
                        'name = "H"
                        SetH(data8)
                    Case $2e
                        'name = "L"
                        SetL(data8)
                    Case $36
                        'name$ = "[HL]"
                        WriteByte(HL, data8)
                End Select
	
                'Disassembly("MVI "+name$+","+HByte$(data8))
            End Method
	
            Method Instruction_CALL()
                'Local name$
                Local condition:Byte = True
                Local data16:Short = FetchRomShort()
	
                Select current_inst
                    Case $cd
                        'name = "CALL"
                    Case $c4
                        'name = "CALL NZ"
                        condition = Not ZERO
                    Case $cc
                        'name = "CALL Z"
                        condition = ZERO
                    Case $d4
                        'name = "CALL NC"
                        condition = Not CARRY
                    Case $dc
                        'name = "CALL C"
                        condition = CARRY
                End Select
	
                'Disassembly(name$+" "+HWord$(data16)+"  condition="+condition)
                If condition Then
                    StackPush(PC)
                    PC = data16
                End If
            End Method
	
            Method Instruction_RET()
                'Local name$
                Local condition:Byte = True
	
                Select current_inst
                    Case $c9
                        'name = "RET"
                    Case $c0
                        'name = "RET NZ"
                        condition = Not ZERO
                    Case $c8
                        'name = "RET Z"
                        condition = ZERO
                    Case $d0
                        'name = "RET NC"
                        condition = Not CARRY
                    Case $d8
                        'name = "RET C"
                        condition = CARRY
                End Select
	
                'Disassembly(name$+"   condition="+condition)
                If condition Then
                    PC = StackPop()
                End If
            End Method

            Method Instruction_LDA()
                'Local name$
                Local source:Short
		
                Select current_inst
                    Case $0a
                        'name = "BC"
                        source = BC
                    Case $1a
                        'name = "DE"
                        source = DE
                    Case $3a
                        source = FetchRomShort()
                        'name = HWord$(source)
                End Select
		
                SetA(ReadByte(source))
	
                'Disassembly("LDA ["+name$+"]")
            End Method

            Method Instruction_PUSH()
                'Local name$
                Local value:Short
		
                Select current_inst
                    Case $c5
                        'name = "BC"
                        value = BC
                    Case $d5
                        'name = "DE"
                        value = DE
                    Case $e5
                        'name = "HL"
                        value = HL
                    Case $f5
                        'name ="AF"
                        value = (A Shl 8)
                        If SIGN Then value = value | BIT7
                        If ZERO Then value = value | BIT6
                        If INTERRUPT Then value = value | BIT5
                        If HALFCARRY Then value = value | BIT4
                        If CARRY Then value = value | BIT0
                End Select
		
                StackPush(value)
	
                'Disassembly("PUSH "+name$)
            End Method

            Method Instruction_POP()
                'Local name$
                Local value:Short = StackPop()
		
                Select current_inst
                    Case $c1
                        'name = "BC"
                        SetBC(value)
                    Case $d1
                        'name = "DE"
                        SetDE(value)
                    Case $e1
                        'name = "HL"
                        SetHL(value)
                    Case $f1
                        'name ="AF"
                        A = value Shr 8
                        SIGN = (value & BIT7)
                        ZERO = (value & BIT6)
                        INTERRUPT = (value & BIT5)
                        HALFCARRY = (value & BIT4)
                        CARRY = (value & BIT0)
                End Select
		
                'Disassembly("POP "+name$)
            End Method

            Method Instruction_MOVHL()
                'Local name$
	
                Select current_inst
                    Case $77
                        'name = "A"
                        WriteByte(HL, A)
                    Case $70
                        'name = "B"
                        WriteByte(HL, B)
                    Case $71
                        'name = "C"
                        WriteByte(HL, C)
                    Case $72
                        'name = "D"
                        WriteByte(HL, D)
                    Case $73
                        'name = "E"
                        WriteByte(HL, E)
                    Case $74
                        'name = "H"
                        WriteByte(HL, H)
                    Case $75
                        'name = "L"
                        WriteByte(HL, L)
                End Select
	
                'Disassembly("MOV [HL],"+ name$)
            End Method
	
            Method Instruction_MOV()
                'Local src$, dst$
	
                Select current_inst
                    Case $7f
                        'dst = "A"; src = "A"
                        SetA(A)
                    Case $78
                        'dst = "A"; src = "B"
                        SetA(B)
                    Case $79
                        'dst = "A"; src = "C"
                        SetA(C)
                    Case $7a
                        'dst = "A"; src = "D"
                        SetA(D)
                    Case $7b
                        'dst = "A"; src = "E"
                        SetA(E)
                    Case $7c
                        'dst = "A"; src = "H"
                        SetA(H)
                    Case $7d
                        'dst = "A"; src = "L"
                        SetA(L)
                    Case $7e
                        'dst = "A"; src = "[HL]"
                        SetA(ReadByte(HL))
				
                    Case $47
                        'dst = "B"; src = "A"
                        SetB(A)
                    Case $40
                        'dst = "B"; src = "B"
                        SetB(B)
                    Case $41
                        'dst = "B"; src = "C"
                        SetB(C)
                    Case $42
                        'dst = "B"; src = "D"
                        SetB(D)
                    Case $43
                        'dst = "B"; src = "E"
                        SetB(E)
                    Case $44
                        'dst = "B"; src = "H"
                        SetB(H)
                    Case $45
                        'dst = "B"; src = "L"
                        SetB(L)
                    Case $46
                        'dst = "B"; src = "[HL]"
                        SetB(ReadByte(HL))

                    Case $4f
                        'dst = "C"; src = "A"
                        SetC(A)
                    Case $48
                        'dst = "C"; src = "B"
                        SetC(B)
                    Case $49
                        'dst = "C"; src = "C"
                        SetC(C)
                    Case $4a
                        'dst = "C"; src = "D"
                        SetC(D)
                    Case $4b
                        'dst = "C"; src = "E"
                        SetC(E)
                    Case $4c
                        'dst = "C"; src = "H"
                        SetC(H)
                    Case $4d
                        'dst = "C"; src = "L"
                        SetC(L)
                    Case $4e
                        'dst = "C"; src = "[HL]"
                        SetC(ReadByte(HL))
		
                    Case $57
                        'dst = "D"; src = "A"
                        SetD(A)
                    Case $50
                        'dst = "D"; src = "B"
                        SetD(B)
                    Case $51
                        'dst = "D"; src = "C"
                        SetD(C)
                    Case $52
                        'dst = "D"; src = "D"
                        SetD(D)
                    Case $53
                        'dst = "D"; src = "E"
                        SetD(E)
                    Case $54
                        'dst = "D"; src = "H"
                        SetD(H)
                    Case $55
                        'dst = "D"; src = "L"
                        SetD(L)
                    Case $56
                        'dst = "D"; src = "[HL]"
                        SetD(ReadByte(HL))

                    Case $5f
                        'dst = "E"; src = "A"
                        SetE(A)
                    Case $58
                        'dst = "E"; src = "B"
                        SetE(B)
                    Case $59
                        'dst = "E"; src = "C"
                        SetE(C)
                    Case $5a
                        'dst = "E"; src = "D"
                        SetE(D)
                    Case $5b
                        'dst = "E"; src = "E"
                        SetE(E)
                    Case $5c
                        'dst = "E"; src = "H"
                        SetE(H)
                    Case $5d
                        'dst = "E"; src = "L"
                        SetE(L)
                    Case $5e
                        'dst = "E"; src = "[HL]"
                        SetE(ReadByte(HL))

                    Case $67
                        'dst = "H"; src = "A"
                        SetH(A)
                    Case $60
                        'dst = "H"; src = "B"
                        SetH(B)
                    Case $61
                        'dst = "H"; src = "C"
                        SetH(C)
                    Case $62
                        'dst = "H"; src = "D"
                        SetH(D)
                    Case $63
                        'dst = "H"; src = "E"
                        SetH(E)
                    Case $64
                        'dst = "H"; src = "H"
                        SetH(H)
                    Case $65
                        'dst = "H"; src = "L"
                        SetH(L)
                    Case $66
                        'dst = "H"; src = "[HL]"
                        SetH(ReadByte(HL))
				
                    Case $6f
                        'dst = "L"; src = "A"
                        SetL(A)
                    Case $68
                        'dst = "L"; src = "B"
                        SetL(B)
                    Case $69
                        'dst = "L"; src = "C"
                        SetL(C)
                    Case $6a
                        'dst = "L"; src = "D"
                        SetL(D)
                    Case $6b
                        'dst = "L"; src = "E"
                        SetL(E)
                    Case $6c
                        'dst = "L"; src = "H"
                        SetL(H)
                    Case $6d
                        'dst = "L"; src = "L"
                        SetL(L)
                    Case $6e
                        'dst = "L"; src = "[HL]"
                        SetL(ReadByte(HL))
                End Select
	
                'Disassembly("MOV "+dst$+","+src$)
            End Method
	
            Method Instruction_INX()
                'Local name$
	
                Select current_inst
                    Case $03
                        'name = "BC"
                        SetBC(BC+1)
                    Case $13
                        'name = "DE"
                        SetDE(DE+1)
                    Case $23
                        'name = "HL"
                        SetHL(HL+1)
                    Case $33
                        'name = "SP"
                        SetSP(SP+1)
                End Select
	
                'Disassembly("INX "+name$)
            End Method
	
            Method Instruction_DAD()
                'Local name$
	
                Select current_inst
                    Case $09
                        'name = "BC"
                        AddHL(BC)
                    Case $19
                        'name = "DE"
                        AddHL(DE)
                    Case $29
                        'name = "HL"
                        AddHL(HL)
                    Case $39
                        'name = "SP"
                        AddHL(SP)
                End Select
		
                'Disassembly("DAD HL,"+name$)
            End Method

            Method AddHL(inValue:Short)
                Local value:Int = HL + inValue;
                SetHL(value)
	
                CARRY = (value > 65535)
            End Method
	
            Method Instruction_DCX()
                'Local name$
	
                Select current_inst
                    Case $0b
                        'name = "BC"
                        SetBC(BC-1)
                    Case $1b
                        'name = "DE"
                        SetDE(DE-1)
                    Case $2b
                        'name = "HL"
                        SetHL(HL-1)
                    Case $3b
                        'name = "SP"
                        SetSP(SP-1)
                End Select
	
                'Disassembly("DCX "+name$)
            End Method
	
            Method Instruction_DEC()
                'Local name$
	
                Select current_inst
                    Case $3d
                        'name = "A"
                        SetA(PerformDec(A))
                    Case $05
                        'name = "B"
                        SetB(PerformDec(B))
                    Case $0d
                        'name = "C"
                        SetC(PerformDec(C))
                    Case $15
                        'name = "D"
                        SetD(PerformDec(D))
                    Case $1d
                        'name = "E"
                        SetE(PerformDec(E))
                    Case $25
                        'name = "H"
                        SetH(PerformDec(H))
                    Case $2d
                        'name = "L"
                        SetL(PerformDec(L))
                    Case $35
                        'name = "[HL]"
                        Local data8:Byte = ReadByte(HL)
                        WriteByte(HL, PerformDec(data8))
                End Select
	
                'Disassembly("DEC "+ name$)
            End Method
	
            Method Instruction_INC()
                'Local name$
	
                Select current_inst
                    Case $3c
                        'name = "A"
                        SetA(PerformInc(A))
                    Case $04
                        'name = "B"
                        SetB(PerformInc(B))
                    Case $0c
                        'name = "C"
                        SetC(PerformInc(C))
                    Case $14
                        'name = "D"
                        SetD(PerformInc(D))
                    Case $1c
                        'name = "E"
                        SetE(PerformInc(E))
                    Case $24
                        'name = "H"
                        SetH(PerformInc(H))
                    Case $2c
                        'name = "L"
                        SetL(PerformInc(L))
                    Case $34
                        'name = "[HL]"
                        Local data8:Byte = ReadByte(HL)
                        WriteByte(HL, PerformInc(data8))
                End Select
	
                'Disassembly("INC "+ name$)
            End Method
	
            Method Instruction_AND()
                'Local name$
	
                Select current_inst
                    Case $a7
                        'name = "A"
                        PerformAnd(A)
                    Case $a0
                        'name = "B"
                        PerformAnd(B)
                    Case $a1
                        'name = "C"
                        PerformAnd(C)
                    Case $a2
                        'name = "D"
                        PerformAnd(D)
                    Case $a3
                        'name = "E"
                        PerformAnd(E)
                    Case $a4
                        'name = "H"
                        PerformAnd(H)
                    Case $a5
                        'name = "L"
                        PerformAnd(L)
                    Case $a6
                        'name = "[HL]"
                        PerformAnd(ReadByte(HL))
                    Case $e6
                        Local immediate:Byte = FetchRomByte()
                        'name = HByte$(immediate)
                        PerformAnd(immediate)
                End Select
	
                'Disassembly("AND "+ name$)
            End Method
	
            Method Instruction_XOR()
                'Local name$
	
                Select current_inst
                    Case $af
                        'name = "A"
                        PerformXor(A)
                    Case $a8
                        'name = "B"
                        PerformXor(B)
                    Case $a9
                        'name = "C"
                        PerformXor(C)
                    Case $aa
                        'name = "D"
                        PerformXor(D)
                    Case $ab
                        'name = "E"
                        PerformXor(E)
                    Case $ac
                        'name = "H"
                        PerformXor(H)
                    Case $ad
                        'name = "L"
                        PerformXor(L)
                    Case $ae
                        'name = "[HL]"
                        PerformXor(ReadByte(HL))
                    Case $ee
                        Local immediate:Byte = FetchRomByte()
                        'name = HByte$(immediate)
                        PerformXor(immediate)
                End Select
	
                'Disassembly("XOR "+ name$)
            End Method
	
            Method Instruction_OR()
                'Local name$
	
                Select current_inst
                    Case $b7
                        'name = "A"
                        PerformOr(A)
                    Case $b0
                        'name = "B"
                        PerformOr(B)
                    Case $b1
                        'name = "C"
                        PerformOr(C)
                    Case $b2
                        'name = "D"
                        PerformOr(D)
                    Case $b3
                        'name = "E"
                        PerformOr(E)
                    Case $b4
                        'name = "H"
                        PerformOr(H)
                    Case $b5
                        'name = "L"
                        PerformOr(L)
                    Case $b6
                        'name = "[HL]"
                        PerformOr(ReadByte(HL))
                    Case $f6
                        Local immediate:Byte = FetchRomByte()
                        'name = HByte$(immediate)
                        PerformOr(immediate)
                End Select
	
                'Disassembly("OR "+ name$)
            End Method
		
            Method Instruction_ADD()
                'Local name$
	
                Select current_inst
                    Case $87
                        'name = "A"
                        PerformByteAdd(A)
                    Case $80
                        'name = "B"
                        PerformByteAdd(B)
                    Case $81
                        'name = "C"
                        PerformByteAdd(C)
                    Case $82
                        'name = "D"
                        PerformByteAdd(D)
                    Case $83
                        'name = "E"
                        PerformByteAdd(E)
                    Case $84
                        'name = "H"
                        PerformByteAdd(H)
                    Case $85
                        'name = "L"
                        PerformByteAdd(L)
                    Case $86
                        'name = "[HL]"
                        PerformByteAdd(ReadByte(HL))
                    Case $c6
                        Local immediate:Byte = FetchRomByte()
                        'name = HByte$(immediate)
                        PerformByteAdd(immediate)
                End Select
	
                'Disassembly("ADD "+ name$)
            End Method
	
            Method Instruction_ADC()
                'Local name$
                Local carryvalue:Byte = 0
                If CARRY Then carryvalue = 1
	
                Select current_inst
                    Case $8f
                        'name = "A"
                        PerformByteAdd(A, carryvalue)
                    Case $88
                        'name = "B"
                        PerformByteAdd(B, carryvalue)
                    Case $89
                        'name = "C"
                        PerformByteAdd(C, carryvalue)
                    Case $8a
                        'name = "D"
                        PerformByteAdd(D, carryvalue)
                    Case $8b
                        'name = "E"
                        PerformByteAdd(E, carryvalue)
                    Case $8c
                        'name = "H"
                        PerformByteAdd(H, carryvalue)
                    Case $8d
                        'name = "L"
                        PerformByteAdd(L, carryvalue)
                    Case $8e
                        'name = "[HL]"
                        PerformByteAdd(ReadByte(HL), carryvalue)
                    Case $ce
                        Local immediate:Byte = FetchRomByte()
                        'name = HByte$(immediate)
                        PerformByteAdd(immediate, carryvalue)
                End Select
	
                'Disassembly("ADC "+ name$)
            End Method
		
            Method Instruction_SUB()
                'Local name$
	
                Select current_inst
                    Case $97
                        'name = "A"
                        PerformByteSub(A)
                    Case $90
                        'name = "B"
                        PerformByteSub(B)
                    Case $91
                        'name = "C"
                        PerformByteSub(C)
                    Case $92
                        'name = "D"
                        PerformByteSub(D)
                    Case $93
                        'name = "E"
                        PerformByteSub(E)
                    Case $94
                        'name = "H"
                        PerformByteSub(H)
                    Case $95
                        'name = "L"
                        PerformByteSub(L)
                    Case $96
                        'name = "[HL]"
                        PerformByteSub(ReadByte(HL))
                    Case $d6
                        Local immediate:Byte = FetchRomByte()
                        'name = HByte$(immediate)
                        PerformByteSub(immediate)
                End Select
	
                'Disassembly("SUB "+ name$)
            End Method
	
            Method Instruction_SBBI()
                Local immediate:Byte = FetchRomByte()
                Local carryvalue:Byte = 0
                If CARRY Then carryvalue = 1
                PerformByteSub(immediate, carryvalue)
                'Disassembly("SBBI "+HByte$(immediate))
            End Method
	
            Method Instruction_CMP()
                'Local name$
                Local value:Byte 
	
                Select current_inst
                    Case $bf
                        'name = "A"
                        value = A
                    Case $b8
                        'name = "B"
                        value = B
                    Case $b9
                        'name = "C"
                        value = C
                    Case $ba
                        'name = "D"
                        value = D
                    Case $bb
                        'name = "E"
                        value = E
                    Case $bc
                        'name = "H"
                        value = H
                    Case $bd
                        'name = "L"
                        value = L
                    Case $be
                        'name = "[HL]"
                        value = ReadByte(HL)
                    Case $fe
                        value = FetchRomByte()
                        'name = HByte$(value)
                End Select
		
                PerformCompSub(value)
                'Disassembly("CMP "+name$)
            End Method
	
            Method Instruction_XCHG()
                Local temp:Short = DE
                SetDE(HL)
                SetHL(temp)
                'Disassembly("XCHG DE,HL")
            End Method
	
            Method Instruction_XTHL()
                Local temp:Byte = H
                SetH(ReadByte(SP+1))
                WriteByte(SP+1, temp)
	
                temp = L
                SetL(ReadByte(SP))
                WriteByte(SP, temp)
	
                'Disassembly("XTHL HL,[SP]")
            End Method
	
            Method Instruction_OUTP()
                Local port:Byte = FetchRomByte()
                io.OutputPort(port, A)
		
                'Disassembly("OUTP "+HByte$(port)+" A="+A)
            End Method
	
            Method Instruction_INP()
                Local port:Byte = FetchRomByte()
                SetA(io.InputPort(port))
		
                'Disassembly("INP "+HByte$(port)+" A="+A)
            End Method
	
            Method Instruction_PCHL()
                PC = HL
                'Disassembly("PCHL")
            End Method
	
            Method Instruction_RST()
                'Local name$
                Local address:Short
	
                Select current_inst
                    Case $c7
                        'name = "RST0"
                        address = $0
                    Case $cf
                        'name = "RST1"
                        address = $8
                    Case $d7
                        'name = "RST2"
                        address = $10
                    Case $df
                        'name = "RST3"
                        address = $18
                    Case $e7
                        'name = "RST4"
                        address = $20
                    Case $ef
                        'name = "RST5"
                        address = $28
                    Case $f7
                        'name = "RST6"
                        address = $30
                    Case $ff
                        'name = "RST7"
                        address = $38
                End Select
	
                'Disassembly(name)
	
                StackPush(PC)
                PC = address
            End Method
	
            Method Instruction_RLC()
                SetA((A Shl 1) | (A Shr 7))
                CARRY = A & BIT0
	
                'Disassembly("RLC")
            End Method
	
            Method Instruction_RAL()
                Local temp:Byte = A
	
                setA(A Shl 1)
                If(CARRY) Then setA(A | BIT0)
                CARRY = temp & BIT7
	
                'Disassembly("RAL")
            End Method
	
            Method Instruction_RRC()
                SetA((A Shr 1) | (A Shl 7))
                CARRY = A & BIT7
                'Disassembly("RRC")
            End Method
	
            Method Instruction_RAR()	
                Local temp:Byte = A
	
                setA(A Shr 1)
                If(CARRY) Then setA(A | BIT7)
                CARRY = temp & BIT0
	
                'Disassembly("RAR")
            End Method
	
            Method Instruction_STA()
                'Local src$
	
                Select current_inst
                    Case $02
                        'src = "BC"
                        WriteByte(BC, A)
                    Case $12
                        'src = "DE"
                        WriteByte(DE, A)
                    Case $32
                        Local immediate:Short = FetchRomShort()
                        WriteByte(immediate, A)
                        'src = HWord$(immediate)
                End Select
	
                'Disassembly("STA "+src$)
            End Method
	
            Method Instruction_DI()
                INTERRUPT = False
                'Disassembly("DI")
            End Method
	
            Method Instruction_EI()
                INTERRUPT = True
                'Disassembly("EI")
            End Method
	
            Method Instruction_STC()
                CARRY = True
                'Disassembly("STC")
            End Method
	
            Method Instruction_CMC()
                CARRY = Not CARRY
                'Disassembly("CMC")
            End Method
	
            Method Instruction_LHLD()
                Local immediate:Short = FetchRomShort()
                SetHL(ReadShort(immediate))
                'Disassembly("LHLD ["+HWord$(immediate)+"]")
            End Method
	
            Method Instruction_SHLD()
                Local immediate:Short = FetchRomShort()
                WriteShort(immediate, HL)
                'Disassembly("SHLD ["+HWord$(immediate)+"]")
            End Method
	
            Method Instruction_DAA()
                If(((A & $0F) > 9) Or (HALFCARRY))
                    A :+ $06
                    HALFCARRY = True
                Else
                    HALFCARRY = False
                End If
	
                If((A > $9F) Or (CARRY))
                    A :+ $60
                    CARRY = True
                Else
                    CARRY = False
                End If
	
                setFlagZeroSign()
                'Disassembly("DAA")
            End Method
	
            Method Instruction_CMA()
                setA(A ~ 255)
                'Disassembly("CMA")
            End Method
	
            Method setFlagZeroSign()
                ZERO = (A = 0)
                SIGN = (A&128)
            End Method

            Method PerformAnd(inValue:Byte)
                SetA(A & inValue)
                CARRY = False
                HALFCARRY = False
                setFlagZeroSign()
            End Method

            Method PerformXor(inValue:Byte)
                SetA(A ~ inValue)
                CARRY = False
                HALFCARRY = False
                setFlagZeroSign()
            End Method

            Method PerformOr(inValue:Byte)
                SetA(A | inValue)
                CARRY = False
                HALFCARRY = False
                setFlagZeroSign()
            End Method
	
            Method PerformByteAdd(inValue:Byte, inCarryValue:Byte=0)
                Local value:Int = A + inValue + inCarryValue
                HALFCARRY = ((A ~ inValue ~ value) & $10)
                setA(value)

                CARRY = (value > 255)
                setFlagZeroSign()
            End Method
	
            Method PerformInc:Byte(inSource:Byte)
                Local value:Int = inSource + 1

                HALFCARRY = ((value & $F) <> 0)

                ZERO = ((value & 255) = 0)
                SIGN = (value & 128)
                Return value
            End Method
	
            Method PerformDec:Byte(inSource:Byte)
                Local value:Int = inSource - 1

                HALFCARRY = ((value & $F) = 0)

                ZERO = ((value & 255) = 0)
                SIGN = (value & 128)
                Return value
            End Method
	
            Method PerformByteSub(inValue:Byte, inCarryValue:Byte=0)
                Local value:Byte = A - inValue - inCarryValue

                CARRY = ((value >= A) And (inValue | inCarryValue))
	
                HALFCARRY = ((A ~ inValue ~ value) & $10)

                setA(value)
                setFlagZeroSign()
            End Method

            Method PerformCompSub:Byte(inValue:Byte)
                Local value:Byte = A - inValue

                CARRY = ((value >= A) And (inValue))
                HALFCARRY = ((A ~ inValue ~ value) & $10)
                ZERO = (value = 0)
                SIGN = (value & 128)
            End Method

            Method Init()
                memory = LoadBank("data/invaders.rom")
                If Not memory Then RuntimeError("Missing ROM file!")
                If memory.Size() <> 8192 Then RuntimeError("Bad ROM size!")
                ResizeBank(memory, 16384) ' Resize memory to allow 8KB of RAM
		
                memoryptr = LockBank(memory)
		
                Reset()
                DebugLog("CPU initialized.")	
            End Method

            Method Reset()
                PC = 0
                A = 0
                BC = 0
                DE = 0
                HL = 0
                SIGN = False
                ZERO = False
                HALFCARRY = False
                PARITY = False
                CARRY = False
                INTERRUPT = False
                CRASHED = False
            End Method
	
            Method Run()
                'DrawText("-> " + cpu.debug_output$, 0, 20)

                ' Run about one frame worth of instruction code
                For Local i:Int=0 Until instruction_per_frame
                    If CRASHED Then Return
                    ExecuteInstruction()
                Next
            End Method
	
            Method ExecuteInstruction()
                disassembly_pc = PC
                current_inst = FetchRomByte()
		
                Select current_inst
                    Case $00
                        Instruction_NOP()
                    Case $c3, $c2, $ca, $d2, $da, $f2, $fa
                        Instruction_JMP()
                    Case $01, $11, $21, $31
                        Instruction_LXI()
                    Case $3e, $06, $0e, $16, $1e, $26, $2e, $36
                        Instruction_MVI()
                    Case $cd, $c4, $cc, $d4, $dc
                        Instruction_CALL()
                    Case $0a, $1a, $3a
                        Instruction_LDA()
                    Case $77, $70, $71, $72, $73, $74, $75
                        Instruction_MOVHL()
                    Case $03, $13, $23, $33
                        Instruction_INX()
                    Case $0b, $1b, $2b, $3b
                        Instruction_DCX()
                    Case $3d, $05, $0d, $15, $1d, $25, $2d, $35
                        Instruction_DEC()
                    Case $3c, $04, $0c, $14, $1c, $24, $2c, $34
                        Instruction_INC()
                    Case $c9, $c0, $c8, $d0, $d8
                        Instruction_RET()
                    Case $7F, $78, $79, $7A, $7B, $7C, $7D, $7E
                        Instruction_MOV()
                    Case $47, $40, $41, $42, $43, $44, $45, $46
                        Instruction_MOV()
                    Case $4f, $48, $49, $4a, $4b, $4c, $4d, $4e
                        Instruction_MOV()
                    Case $57, $50, $51, $52, $53, $54, $55, $56
                        Instruction_MOV()
                    Case $5f, $58, $59, $5a, $5b, $5c, $5d, $5e
                        Instruction_MOV()
                    Case $67, $60, $61, $62, $63, $64, $65, $66
                        Instruction_MOV()
                    Case $6f, $68, $69, $6a, $6b, $6c, $6d, $6e
                        Instruction_MOV()
                    Case $bf, $b8, $b9, $ba, $bb, $bc, $bd, $be, $fe
                        Instruction_CMP()
                    Case $c5, $d5, $e5, $f5
                        Instruction_PUSH()
                    Case $c1, $d1, $e1, $f1
                        Instruction_POP()
                    Case $09, $19, $29, $39
                        Instruction_DAD()
                    Case $eb
                        Instruction_XCHG()
                    Case $e3
                        Instruction_XTHL()
                    Case $d3
                        Instruction_OUTP()
                    Case $db
                        Instruction_INP()
                    Case $e9
                        Instruction_PCHL()
                    Case $c7, $cf, $d7, $df, $e7, $ef, $f7, $ff
                        Instruction_RST()
                    Case $07
                        Instruction_RLC()
                    Case $17
                        Instruction_RAL()
                    Case $0f
                        Instruction_RRC()
                    Case $1f
                        Instruction_RAR()
                    Case $a7, $a0, $a1, $a2, $a3, $a4, $a5, $a6, $e6
                        Instruction_AND()
                    Case $87, $80, $81, $82, $83, $84, $85, $86, $c6
                        Instruction_ADD()
                    Case $02, $12, $32
                        Instruction_STA()
                    Case $af, $a8, $a9, $aa, $ab, $ac, $ad, $ae, $ee
                        Instruction_XOR()
                    Case $f3
                        Instruction_DI()
                    Case $fb
                        Instruction_EI()
                    Case $37
                        Instruction_STC()
                    Case $3f
                        Instruction_CMC()
                    Case $b7, $b0, $b1, $b2, $b3, $b4, $b5, $b6, $f6
                        Instruction_OR()
                    Case $97, $90, $91, $92, $93, $94, $95, $96, $d6
                        Instruction_SUB()
                    Case $2a
                        Instruction_LHLD()
                    Case $22
                        Instruction_SHLD()
                    Case $de
                        Instruction_SBBI()
                    Case $27
                        Instruction_DAA()
                    Case $2f
                        Instruction_CMA()
                    Case $8f, $88, $89, $8a, $8b, $8c, $8d, $8e, $ce
                        Instruction_ADC()
				
                    Default
                        Disassembly("Undefined instruction: "+HByte$(current_inst))
                        CRASHED = True
                End Select
		
                count_instructions :+ 1
				
                If count_instructions >= half_instruction_per_frame Then
                    If INTERRUPT Then		
                        ' There are two interrupt that occur every frame (address $08 and $10)
                        If interrupt_alternate = 0 Then
                            CallInterrupt($08)
                        Else
                            CallInterrupt($10)
                        End If
                        interrupt_alternate = 1 - interrupt_alternate
                        count_instructions = 0
                    End If
                End If
			
            End Method
	
            Method CallInterrupt(inAddress:Short)
                ' call the interrupt by pushing current PC on the stack and then jump to interrupt address
                INTERRUPT = False
                StackPush(PC)
                PC = inAddress
            End Method
	
            Method SetA(inByte:Byte)
                A = inByte
            End Method
            Method SetB(inByte:Byte)
                B = inByte
                BC = (B Shl 8) + C
            End Method
            Method SetC(inByte:Byte)
                C = inByte
                BC = (B Shl 8) + C
            End Method
            Method SetD(inByte:Byte)
                D = inByte
                DE = (D Shl 8) + E
            End Method
            Method SetE(inByte:Byte)
                E = inByte
                DE = (D Shl 8) + E
            End Method
            Method SetH(inByte:Byte)
                H = inByte
                HL = (H Shl 8) + L
            End Method
            Method SetL(inByte:Byte)
                L = inByte
                HL = (H Shl 8) + L
            End Method
            Method SetBC(inShort:Short)
                BC = inShort
                B = BC Shr 8
                C = BC
            End Method
            Method SetDE(inShort:Short)
                DE = inShort
                D = DE Shr 8
                E = DE
            End Method
            Method SetHL(inShort:Short)
                HL = inShort
                H = HL Shr 8
                L = HL
            End Method
            Method SetSP(inShort:Short)
                SP = inShort
            End Method

            Method FetchRomByte:Byte()
                Local b:Byte = memoryptr[PC]
                PC :+ 1
                Return b
            End Method

            Method FetchRomShort:Short()
                Local s:Short = (memoryptr[PC+1] Shl 8) + (memoryptr[PC])
                PC :+ 2
                Return s
            End Method
	
            Method ReadByte:Byte(inAddress:Short)
                Return memoryptr[inAddress]
            End Method
	
            Method ReadShort:Short(inAddress:Short)
                Return (memoryptr[inAddress+1] Shl 8) + (memoryptr[inAddress])
            End Method

            Method WriteByte(inAddress:Short, inByte:Byte)
                memoryptr[inAddress] = inByte
            End Method
	
            Method WriteShort(inAddress:Short, inWord:Short)
                memoryptr[inAddress+1] = inWord Shr 8
                memoryptr[inAddress] = inWord
            End Method

            Method StackPush(inValue:Short)
                SP :- 2
                WriteShort(SP, inValue)
            End Method
	
            Method StackPop:Short()
                Local temp:Short = ReadShort(SP)
                SP :+ 2
                Return temp
            End Method
	
            Method Disassembly(inText$)
                debug_output$ = HWord$(disassembly_pc)+": " + inText$
            End Method
        End Type



        */


    }
}
