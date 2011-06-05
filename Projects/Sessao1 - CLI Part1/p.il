
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.0.30319.1
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly p
{

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 2:0:0:0
}
.module p.dll
// MVID: {3B09DEAD-BD34-4036-81F2-B08B4649AEEC}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000003    //  ILONLY 32BITREQUIRED
// Image base: 0x01D40000


// =============== CLASS MEMBERS DECLARATION ===================

.class public auto ansi Sessao1.Ponto
       extends [mscorlib]System.Object
{
  .field private int32 _x
  .field private int32 _y
  .method private hidebysig specialname rtspecialname static 
          void  .cctor() cil managed
  {
    // Code size       41 (0x29)
    .maxstack  8
    IL_0000:  ldstr      "Codebase={0}"
    IL_0005:  call       class [mscorlib]System.Reflection.Assembly [mscorlib]System.Reflection.Assembly::GetExecutingAssembly()
    IL_000a:  callvirt   instance string [mscorlib]System.Reflection.Assembly::get_CodeBase()
    IL_000f:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                  object)
    IL_0014:  ldstr      "Location={0}"
    IL_0019:  call       class [mscorlib]System.Reflection.Assembly [mscorlib]System.Reflection.Assembly::GetExecutingAssembly()
    IL_001e:  callvirt   instance string [mscorlib]System.Reflection.Assembly::get_Location()
    IL_0023:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                  object)
    IL_0028:  ret
  } // end of method Ponto::.cctor

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(int32 x,
                               int32 y) cil managed
  {
    // Code size       40 (0x28)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ldarg.1
    IL_0007:  ldc.i4.0
    IL_0008:  blt.s      IL_000e

    IL_000a:  ldarg.2
    IL_000b:  ldc.i4.0
    IL_000c:  bge.s      IL_0019

    IL_000e:  ldstr      "point coordinates must be positive!"
    IL_0013:  newobj     instance void [mscorlib]System.Exception::.ctor(string)
    IL_0018:  throw

    IL_0019:  ldarg.0
    IL_001a:  ldarg.1
    IL_001b:  stfld      int32 Sessao1.Ponto::_x
    IL_0020:  ldarg.0
    IL_0021:  ldarg.2
    IL_0022:  stfld      int32 Sessao1.Ponto::_y
    IL_0027:  ret
  } // end of method Ponto::.ctor

  .method public hidebysig instance int32 
          Distance(class Sessao1.Ponto p1) cil managed
  {
    // Code size       43 (0x2b)
    .maxstack  3
    .locals init (int32 V_0,
             int32 V_1)
    IL_0000:  ldarg.1
    IL_0001:  ldfld      int32 Sessao1.Ponto::_x
    IL_0006:  ldarg.0
    IL_0007:  ldfld      int32 Sessao1.Ponto::_x
    IL_000c:  sub
    IL_000d:  stloc.0
    IL_000e:  ldarg.1
    IL_000f:  ldfld      int32 Sessao1.Ponto::_y
    IL_0014:  ldarg.0
    IL_0015:  ldfld      int32 Sessao1.Ponto::_y
    IL_001a:  sub
    IL_001b:  stloc.1
    IL_001c:  ldloc.0
    IL_001d:  ldloc.0
    IL_001e:  mul
    IL_001f:  ldloc.1
    IL_0020:  ldloc.1
    IL_0021:  mul
    IL_0022:  add
    IL_0023:  conv.r8
    IL_0024:  call       float64 [mscorlib]System.Math::Sqrt(float64)
    IL_0029:  conv.i4
    IL_002a:  ret
  } // end of method Ponto::Distance

  .method public hidebysig virtual instance string 
          ToString() cil managed
  {
    // Code size       33 (0x21)
    .maxstack  8
    IL_0000:  ldstr      "({0},{1})"
    IL_0005:  ldarg.0
    IL_0006:  ldfld      int32 Sessao1.Ponto::_x
    IL_000b:  box        [mscorlib]System.Int32
    IL_0010:  ldarg.0
    IL_0011:  ldfld      int32 Sessao1.Ponto::_y
    IL_0016:  box        [mscorlib]System.Int32
    IL_001b:  call       string [mscorlib]System.String::Format(string,
                                                                object,
                                                                object)
    IL_0020:  ret
  } // end of method Ponto::ToString

    .method public hidebysig instance int32 
          CompareTo(class Sessao1.Ponto p1) cil managed
  {
	//Prompt-11-S1.pdf - 3 f)
	.maxstack 10
    .locals init (int32 V_0,
             int32 V_1)
	ldarg.1
	ldfld	int32 Sessao1.Ponto::_x
	ldarg.0
	ldfld	int32 Sessao1.Ponto::_x
	
  }

} // end of class Sessao1.Ponto


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
