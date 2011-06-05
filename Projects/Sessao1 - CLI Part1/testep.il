
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.0.30319.1
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly extern p
{
  .ver 1:0:0:0
}
.assembly testep
{

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 07 01 00 00 00 00 ) 

  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module testep.exe
// MVID: {93554331-0110-41D4-BFCC-A6472CC3FA2A}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000003    //  ILONLY 32BITREQUIRED
// Image base: 0x00620000


// =============== CLASS MEMBERS DECLARATION ===================

.class private auto ansi beforefieldinit Sessao1.Program
       extends [mscorlib]System.Object
{
  .method private hidebysig static void  Main(string[] args) cil managed
  {
    .entrypoint
    // Code size       168 (0xa8)
    .maxstack  3
    .locals init ([0] class [p]Sessao1.Ponto p1,
             [1] class [p]Sessao1.Ponto p2,
             [2] int32 d1,
             [3] int32 d2,
             [4] string s1,
             [5] string s2,
             [6] string sres,
             [7] object[] CS$0$0000)
    IL_0000:  nop
    IL_0001:  ldc.i4.2
    IL_0002:  ldc.i4.3
    IL_0003:  newobj     instance void [p]Sessao1.Ponto::.ctor(int32,
                                                               int32)
    IL_0008:  stloc.0
    IL_0009:  ldc.i4.4
    IL_000a:  ldc.i4.5
    IL_000b:  newobj     instance void [p]Sessao1.Ponto::.ctor(int32,
                                                               int32)
    IL_0010:  stloc.1
    IL_0011:  ldloc.0
    IL_0012:  ldloc.1
    IL_0013:  callvirt   instance int32 [p]Sessao1.Ponto::Distance(class [p]Sessao1.Ponto)
    IL_0018:  stloc.2
    IL_0019:  ldloc.1
    IL_001a:  ldloc.0
    IL_001b:  callvirt   instance int32 [p]Sessao1.Ponto::Distance(class [p]Sessao1.Ponto)
    IL_0020:  stloc.3
    IL_0021:  ldstr      bytearray (4F 00 20 00 70 00 6F 00 6E 00 74 00 6F 00 20 00   // O. .p.o.n.t.o. .
                                    31 00 20 00 E9 00 20 00 )                         // 1. ... .
    IL_0026:  ldloc.0
    IL_0027:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_002c:  ldstr      ";"
    IL_0031:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_0036:  stloc.s    s1
    IL_0038:  ldstr      bytearray (4F 00 20 00 70 00 6F 00 6E 00 74 00 6F 00 20 00   // O. .p.o.n.t.o. .
                                    32 00 20 00 E9 00 20 00 )                         // 2. ... .
    IL_003d:  ldloc.1
    IL_003e:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0043:  ldstr      ";"
    IL_0048:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_004d:  stloc.s    s2
    IL_004f:  ldc.i4.5
    IL_0050:  newarr     [mscorlib]System.Object
    IL_0055:  stloc.s    CS$0$0000
    IL_0057:  ldloc.s    CS$0$0000
    IL_0059:  ldc.i4.0
    IL_005a:  ldstr      bytearray (41 00 20 00 64 00 69 00 73 00 74 00 E2 00 6E 00   // A. .d.i.s.t...n.
                                    63 00 69 00 61 00 20 00 72 00 65 00 73 00 75 00   // c.i.a. .r.e.s.u.
                                    6C 00 74 00 61 00 6E 00 74 00 65 00 20 00 65 00   // l.t.a.n.t.e. .e.
                                    6E 00 74 00 72 00 65 00 20 00 6F 00 73 00 20 00   // n.t.r.e. .o.s. .
                                    32 00 20 00 70 00 6F 00 6E 00 74 00 6F 00 73 00   // 2. .p.o.n.t.o.s.
                                    20 00 61 00 63 00 69 00 6D 00 61 00 20 00 E9 00   //  .a.c.i.m.a. ...
                                    20 00 )                                           //  .
    IL_005f:  stelem.ref
    IL_0060:  ldloc.s    CS$0$0000
    IL_0062:  ldc.i4.1
    IL_0063:  ldloc.2
    IL_0064:  box        [mscorlib]System.Int32
    IL_0069:  stelem.ref
    IL_006a:  ldloc.s    CS$0$0000
    IL_006c:  ldc.i4.2
    IL_006d:  ldstr      bytearray (20 00 71 00 75 00 65 00 20 00 E9 00 20 00 69 00   //  .q.u.e. ... .i.
                                    67 00 75 00 61 00 6C 00 20 00 61 00 20 00 )       // g.u.a.l. .a. .
    IL_0072:  stelem.ref
    IL_0073:  ldloc.s    CS$0$0000
    IL_0075:  ldc.i4.3
    IL_0076:  ldloc.3
    IL_0077:  box        [mscorlib]System.Int32
    IL_007c:  stelem.ref
    IL_007d:  ldloc.s    CS$0$0000
    IL_007f:  ldc.i4.4
    IL_0080:  ldstr      "."
    IL_0085:  stelem.ref
    IL_0086:  ldloc.s    CS$0$0000
    IL_0088:  call       string [mscorlib]System.String::Concat(object[])
    IL_008d:  stloc.s    sres
    IL_008f:  ldloc.s    s1
    IL_0091:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0096:  nop
    IL_0097:  ldloc.s    s2
    IL_0099:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_009e:  nop
    IL_009f:  ldloc.s    sres
    IL_00a1:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_00a6:  nop
    IL_00a7:  ret
  } // end of method Program::Main

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method Program::.ctor

} // end of class Sessao1.Program


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file testep.res
