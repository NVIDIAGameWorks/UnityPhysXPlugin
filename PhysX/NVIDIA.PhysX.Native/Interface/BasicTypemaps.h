// This code contains NVIDIA Confidential Information and is disclosed to you
// under a form of NVIDIA software license agreement provided separately to you.
//
// Notice
// NVIDIA Corporation and its licensors retain all intellectual property and
// proprietary rights in and to this software and related documentation and
// any modifications thereto. Any use, reproduction, disclosure, or
// distribution of this software and related documentation without an express
// license agreement from NVIDIA Corporation is strictly prohibited.
//
// ALL NVIDIA DESIGN SPECIFICATIONS, CODE ARE PROVIDED "AS IS.". NVIDIA MAKES
// NO WARRANTIES, EXPRESSED, IMPLIED, STATUTORY, OR OTHERWISE WITH RESPECT TO
// THE MATERIALS, AND EXPRESSLY DISCLAIMS ALL IMPLIED WARRANTIES OF NONINFRINGEMENT,
// MERCHANTABILITY, AND FITNESS FOR A PARTICULAR PURPOSE.
//
// Information and code furnished is believed to be accurate and reliable.
// However, NVIDIA Corporation assumes no responsibility for the consequences of use of such
// information or for any infringement of patents or other rights of third parties that may
// result from its use. No license is granted by implication or otherwise under any patent
// or patent rights of NVIDIA Corporation. Details are subject to change without notice.
// This code supersedes and replaces all information previously supplied.
// NVIDIA Corporation products are not authorized for use as critical
// components in life support devices or systems without express written approval of
// NVIDIA Corporation.
//
// Copyright (c) 2019 NVIDIA Corporation. All rights reserved.

#ifdef SWIG

// VOID_INT_PTR extra
%typemap(csvarout, excode=SWIGEXCODE) void *VOID_INT_PTR %{
    get {
      global::System.IntPtr ret = $imcall; $excode
      return ret;
    }
%}

%define DECLARE_BASIC_TYPEMAPS

%typemap(csclassmodifiers) SWIGTYPE "public partial class"

%typemap(csinterfaces) SWIGTYPE ""

%typemap(csdispose) SWIGTYPE %{
  ~$csclassname() {
    destroy();
  }
%}

%typemap(csdisposing, methodname="destroy", methodmodifiers="public") SWIGTYPE {
    lock(this) {
      if (swigCPtr.Handle != global::System.IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          $imcall;
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
      global::System.GC.SuppressFinalize(this);
    }
  }

%typemap(csdisposing_derived, methodname="destroy", methodmodifiers="public") SWIGTYPE {
    lock(this) {
      if (swigCPtr.Handle != global::System.IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          $imcall;
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
      global::System.GC.SuppressFinalize(this);
      base.destroy();
    }
  }

%enddef

%define REFERENCE_SEMANTIC

%typemap(csclassmodifiers) SWIGTYPE "public partial class"

%typemap(csinterfaces) SWIGTYPE ""

%typemap(csbody) SWIGTYPE %{
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal $csclassname(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr($csclassname obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  internal static $csclassname getWrapper(global::System.IntPtr cPtr, bool cMemoryOwn) {
      var wrapper = ($csclassname)WrapperCache.find(cPtr);
      if (wrapper == null) {
          wrapper = new $csclassname(cPtr, cMemoryOwn);
          WrapperCache.add(cPtr, wrapper);
      }
      return wrapper;
  }
%}

%typemap(csbody_derived) SWIGTYPE %{
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;

  internal $csclassname(global::System.IntPtr cPtr, bool cMemoryOwn) : base($imclassname.$csclazznameSWIGUpcast(cPtr), cMemoryOwn) {
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr($csclassname obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  internal static new $csclassname getWrapper(global::System.IntPtr cPtr, bool cMemoryOwn) {
      var wrapper = ($csclassname)WrapperCache.find(cPtr);
      if (wrapper == null) {
          wrapper = new $csclassname(cPtr, cMemoryOwn);
          WrapperCache.add(cPtr, wrapper);
      }
      return wrapper;
  }
%}

%typemap(csdispose) SWIGTYPE %{
  ~$csclassname() {
     WrapperCache.remove(swigCPtr.Handle);
  }
%}

%typemap(csdisposing) SWIGTYPE ""

%typemap(csdisposing_derived) SWIGTYPE ""

//
%typemap(csdirectorin) SWIGTYPE *, SWIGTYPE (CLASS::*) "($iminput == global::System.IntPtr.Zero) ? null : $csclassname.getWrapper($iminput, false)"
%typemap(csdirectorin) SWIGTYPE & "$csclassname.getWrapper($iminput, false)"
%typemap(csdirectorin) SWIGTYPE && "$csclassname.getWrapper($iminput, false)"

//
%typemap(csout, excode=SWIGEXCODE) SWIGTYPE {
    $&csclassname ret = $&csclassname.getWrapper($imcall, true);$excode
    return ret;
  }
%typemap(csout, excode=SWIGEXCODE) SWIGTYPE & {
    $csclassname ret = $csclassname.getWrapper($imcall, $owner);$excode
    return ret;
  }
%typemap(csout, excode=SWIGEXCODE) SWIGTYPE && {
    $csclassname ret = $csclassname.getWrapper($imcall, $owner);$excode
    return ret;
  }
%typemap(csout, excode=SWIGEXCODE) SWIGTYPE *, SWIGTYPE [] {
    global::System.IntPtr cPtr = $imcall;
    $csclassname ret = (cPtr == global::System.IntPtr.Zero) ? null : $csclassname.getWrapper(cPtr, $owner);$excode
    return ret;
  }
%typemap(csout, excode=SWIGEXCODE) SWIGTYPE (CLASS::*) {
    string cMemberPtr = $imcall;
    $csclassname ret = (cMemberPtr == null) ? null : $csclassname.getWrapper(cMemberPtr, $owner);$excode
    return ret;
  }

//
%typemap(csvarout, excode=SWIGEXCODE2) SWIGTYPE %{
    get {
      $&csclassname ret = $&csclassname.getWrapper($imcall, true);$excode
      return ret;
    } %}
%typemap(csvarout, excode=SWIGEXCODE2) SWIGTYPE & %{
    get {
      $csclassname ret = $csclassname.getWrapper($imcall, $owner);$excode
      return ret;
    } %}
%typemap(csvarout, excode=SWIGEXCODE2) SWIGTYPE && %{
    get {
      $csclassname ret = $csclassname.getWrapper($imcall, $owner);$excode
      return ret;
    } %}
%typemap(csvarout, excode=SWIGEXCODE2) SWIGTYPE *, SWIGTYPE [] %{
    get {
      global::System.IntPtr cPtr = $imcall;
      $csclassname ret = (cPtr == global::System.IntPtr.Zero) ? null : $csclassname.getWrapper(cPtr, $owner);$excode
      return ret;
    } %}

%typemap(csvarout, excode=SWIGEXCODE2) SWIGTYPE (CLASS::*) %{
    get {
      string cMemberPtr = $imcall;
      $csclassname ret = (cMemberPtr == null) ? null : $csclassname.getWrapper(cMemberPtr, $owner);$excode
      return ret;
    } %}

%enddef

// ByRef array

%define CSHARP_BYREF_ARRAY(CTYPE, CSTYPE)

%typemap(ctype)   CTYPE BYREF[] "CTYPE*"
%typemap(cstype)  CTYPE BYREF[] "ref CSTYPE"
%typemap(imtype)  CTYPE BYREF[] "ref CSTYPE"
%typemap(csin)    CTYPE BYREF[] "ref $csinput"

%typemap(in)      CTYPE BYREF[] "$1 = $input;"
%typemap(freearg) CTYPE BYREF[] ""
%typemap(argout)  CTYPE BYREF[] ""

%enddef

// Object array

%define CSHARP_OBJECT_ARRAY(CTYPE, CSTYPE)

%typemap(ctype)   CTYPE* INPUT[] "CTYPE**"
%typemap(cstype)  CTYPE* INPUT[] "CSTYPE[]"
%typemap(imtype)  CTYPE* INPUT[] "global::System.Runtime.InteropServices.HandleRef[]"
%typemap(csin)    CTYPE* INPUT[] "global::System.Array.ConvertAll($csinput, x => CSTYPE.getCPtr(x))"

%typemap(in)      CTYPE* INPUT[] "$1 = $input;"
%typemap(freearg) CTYPE* INPUT[] ""
%typemap(argout)  CTYPE* INPUT[] ""

%enddef

%define CSHARP_OBJECT_ARRAY2(CTYPE, CSTYPE)

%typemap(ctype) CTYPE* OUTPUT[] "CTYPE**"
%typemap(imtype, inattributes = "[global::System.Runtime.InteropServices.Out, global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPArray)]") CTYPE* OUTPUT[] "global::System.IntPtr[]"
%typemap(cstype) CTYPE* OUTPUT[] "CSTYPE[]"
%typemap(csin,
    pre = "    global::System.IntPtr[] cPtr_$csinput = new global::System.IntPtr[$csinput.Length];",
    post = "    global::System.Array.Copy(global::System.Array.ConvertAll(cPtr_$csinput, x => new $*csclassname(x, true)), $csinput, $csinput.Length);",
    cshin = "out $csinput") CTYPE* OUTPUT[] "cPtr_$csinput"
%typemap(in) CTYPE* OUTPUT[] %{ $1 = ($1_ltype)$input; %}
%typemap(freearg) CTYPE* OUTPUT[] ""

%enddef

// Simplify enum wrapped into a struct

%define SIMPLIFY_ENUM(NAME, ITEMS...)
    %ignore NAME;
    %rename(NAME) NAME##_swigEnum;
    enum struct NAME##_swigEnum {
        ITEMS
    };
    struct NAME { using Enum = NAME##_swigEnum; };
%enddef

// The same for flag enums

%define SIMPLIFY_FLAGS_ENUM(NAME, ITEMS...)
    %ignore NAME;
    %rename(NAME) NAME##_swigEnum;
    %typemap(csattributes) NAME##_swigEnum "[global::System.FlagsAttribute()]"
    enum struct NAME##_swigEnum {
        ITEMS
    };
    struct NAME { using Enum = NAME##_swigEnum; };
%enddef

// A plain struct which content is copied directly to C# struct

%define FLAT_STRUCT3(TYPE, CTYPE, CSTYPE, CSCODE...)
    %typemap(ctype) TYPE*, TYPE&, TYPE[ANY] %{ CTYPE* %}
    %typemap(in) TYPE*, TYPE&, TYPE[ANY] %{ $1 = $input; %}
    %typemap(varin) TYPE*, TYPE&, TYPE[ANY] %{ $1 = $input; %}
    //%typemap(memberin) TYPE*, TYPE&, TYPE[ANY] %{ $1 = $input; %}
    %typemap(out, null="NULL") TYPE*, TYPE& %{ $result = $1; %}
    %typemap(varout, null="NULL") TYPE*, TYPE& %{ $result = $1; %}
    %typemap(memberout, null="NULL") TYPE*, TYPE& %{ $result = $1; %}
    //%typemap(imtype, out="global::System.IntPtr") TYPE*, TYPE& %{ ref CSTYPE %}
    //%typemap(imtype, out="global::System.IntPtr", inattributes = "[global::System.Runtime.InteropServices.Out, global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPStruct)]") TYPE*, TYPE& %{ CSTYPE %}
    %typemap(imtype, out="global::System.IntPtr") TYPE*, TYPE& %{ global::System.IntPtr %}
    %typemap(imtype) TYPE* OUTPUT, TYPE& OUTPUT %{ out CSTYPE %}
    %typemap(imtype, out="global::System.IntPtr") TYPE[ANY] %{ CSTYPE[] %}
    %typemap(cstype, out="CSTYPE") TYPE*, TYPE&  %{ ref CSTYPE %}
    %typemap(cstype, out="CSTYPE") TYPE* OUTPUT, TYPE& OUTPUT %{ out CSTYPE %}
    %typemap(cstype, out="global::System.IntPtr")  TYPE[ANY] %{ CSTYPE[] %}
    %typemap(cstype) const TYPE*, const TYPE& %{ CSTYPE %}
    %typemap(csin) TYPE*, TYPE& %{ $csinput.swigCPtr %}
    %typemap(csin) TYPE* OUTPUT, TYPE& OUTPUT %{ out $csinput %}
    %typemap(csin) const TYPE*, const TYPE& %{ $csinput.swigCPtr %}
    %typemap(csin) TYPE[ANY] %{ $csinput %}
    %typemap(csout, excode=SWIGEXCODE) TYPE*, TYPE& {
        global::System.IntPtr ptr = $imcall;$excode
        //CSTYPE ret = global::System.Runtime.InteropServices.Marshal.PtrToStructure<CSTYPE>(ptr);
        CSTYPE ret; unsafe { ret = *(CSTYPE*)ptr; }
        return ret;
    }
    %typemap(csvarout, excode=SWIGEXCODE2) TYPE*, TYPE& %{
        get { global::System.IntPtr ptr = $imcall;$excode
              //CSTYPE ret = global::System.Runtime.InteropServices.Marshal.PtrToStructure<CSTYPE>(ptr);
              CSTYPE ret; unsafe { ret = *(CSTYPE*)ptr; }
              return ret; }
    %}
    %apply TYPE* OUTPUT { TYPE* result };
    %apply TYPE& OUTPUT { TYPE& result };
    %typemap(ctype, out="CTYPE*") TYPE %{ CTYPE %}
    %typemap(in) TYPE %{ $1 = *(CTYPE*)&$input; %}
    %typemap(varin) TYPE %{ $1 = *(CTYPE*)&$input; %}
    //%typemap(memberin) TYPE %{ $1 = *(CTYPE*)&$input; %}
    %typemap(out, null="NULL") TYPE %{
        thread_local CTYPE out_temp;
        out_temp = *(CTYPE*)&$1; 
        $result = &out_temp; 
    %}
    %typemap(varout, null="NULL") TYPE %{ 
        thread_local CTYPE out_temp;
        out_temp = *(CTYPE*)&$1; 
        $result = &out_temp; 
    %}
    %typemap(memberout, null="NULL") TYPE %{ 
        thread_local CTYPE out_temp;
        out_temp = *(CTYPE*)&$1; 
        $result = &out_temp; 
    %}
    %typemap(imtype, out="global::System.IntPtr") TYPE %{ global::System.IntPtr %}
    %typemap(cstype) TYPE %{ CSTYPE %}
    %typemap(csin) TYPE %{ $csinput.swigCPtr %}
    %typemap(csout, excode=SWIGEXCODE) TYPE {
        global::System.IntPtr ptr = $imcall;$excode
        //CSTYPE ret = global::System.Runtime.InteropServices.Marshal.PtrToStructure<CSTYPE>(ptr);
        CSTYPE ret; unsafe { ret = *(CSTYPE*)ptr; }
        return ret;
    }
    %typemap(csvarout, excode=SWIGEXCODE2) TYPE %{
        get{ global::System.IntPtr ptr = $imcall; $excode
            //CSTYPE ret = global::System.Runtime.InteropServices.Marshal.PtrToStructure<CSTYPE>(ptr);
            CSTYPE ret; unsafe { ret = *(CSTYPE*)ptr; }
            return ret;
    }
    %}
    %typemap(csclassmodifiers) TYPE, TYPE& %{[global::System.Runtime.InteropServices.StructLayout(global::System.Runtime.InteropServices.LayoutKind.Sequential)]
public partial struct%}
    %typemap(csinterfaces) TYPE, TYPE& ""
    %typemap(csdispose) TYPE, TYPE& ""
    %typemap(csdisposing) TYPE, TYPE& ""
    %typemap(csdisposing_derived) TYPE, TYPE& ""
    %typemap(csconstruct, excode=SWIGEXCODE) TYPE, TYPE& %{: this($imcall, true) { $excode }%}
    %typemap(csbody) TYPE, TYPE& %{

  CSCODE

  internal global::System.IntPtr swigCPtr {
    get { unsafe { fixed(CSTYPE* p = &this) return (global::System.IntPtr)p; } }
  }

  internal CSTYPE(global::System.IntPtr ptr, bool unused) {
      //this = global::System.Runtime.InteropServices.Marshal.PtrToStructure<CSTYPE>(ptr);
      unsafe { this = *(CSTYPE*)ptr; }
  }
    %}
    %typemap(csbody_derived) TYPE, TYPE& ""
    %typemap(csdirectorin) TYPE, TYPE& "new $csclassname($iminput, false)"
%enddef

%define FLAT_STRUCT(CTYPE, CSTYPE, CSCODE...)
    FLAT_STRUCT3(CTYPE, CTYPE, CSTYPE, CSCODE)
%enddef

%define OUTPUT_TYPEMAP(TYPE, CTYPE, CSTYPE, TYPECHECKPRECEDENCE)
%typemap(ctype, out="void *") TYPE *OUTPUT, TYPE &OUTPUT "CTYPE *"
%typemap(imtype, out="global::System.IntPtr") TYPE *OUTPUT, TYPE &OUTPUT "out CSTYPE"
%typemap(cstype, out="$csclassname") TYPE *OUTPUT, TYPE &OUTPUT "out CSTYPE"
%typemap(csin) TYPE *OUTPUT, TYPE &OUTPUT "out $csinput"

%typemap(in) TYPE *OUTPUT, TYPE &OUTPUT
%{ $1 = ($1_ltype)$input; %}

%typecheck(SWIG_TYPECHECK_##TYPECHECKPRECEDENCE) TYPE *OUTPUT, TYPE &OUTPUT ""
%enddef

// An opaque struct holding a pointer to unmanaged data

%define WRAPPER_STRUCT(TYPE)

%typemap(csclassmodifiers) TYPE "public partial struct"

%typemap(csinterfaces) TYPE ""

%typemap(csdispose) TYPE ""

%typemap(csdisposing) TYPE ""

%typemap(csdisposing_derived) TYPE ""

%typemap(csbody) TYPE %{
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  private bool swigCMemOwn;

  internal $csclassname(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr($csclassname obj) {
    return obj.swigCPtr;
  }
%}

%enddef

// A free class which can be new'ed and delete'd from C#

%define PROXY_CLASS(TYPE)

%typemap(csclassmodifiers) TYPE "public partial class"

%typemap(csinterfaces) TYPE ""

%typemap(csdispose) TYPE %{
    ~$csclassname() {
        destroy();
    }
%}

%typemap(csdisposing, methodname="destroy", methodmodifiers="public") TYPE {
    lock(this) {
        if (swigCPtr.Handle != global::System.IntPtr.Zero) {
            if (swigCMemOwn) {
                swigCMemOwn = false;
                $imcall;
            }
            swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
        }
        global::System.GC.SuppressFinalize(this);
    }
}

%typemap(csdisposing_derived, methodname="destroy", methodmodifiers="public") TYPE {
    lock(this) {
        if (swigCPtr.Handle != global::System.IntPtr.Zero) {
            if (swigCMemOwn) {
                swigCMemOwn = false;
                $imcall;
            }
            swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
        }
        global::System.GC.SuppressFinalize(this);
        base.destroy();
    }
}

%enddef

// A class owned by PhysX. Created and destroyed by PhysX SDK

%define WRAPPER_CLASS(TYPE)

%typemap(csclassmodifiers) TYPE "public partial class"

%typemap(csinterfaces) TYPE ""

%typemap(csbody) TYPE %{
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  protected bool swigCMemOwn;
  public object userData;

  internal $csclassname(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr($csclassname obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  internal static $csclassname getWrapper(global::System.IntPtr cPtr, bool cMemoryOwn) {
      var wrapper = WrapperCache.find(cPtr);
      if (!(wrapper is $csclassname)) {
          wrapper = new $csclassname(cPtr, cMemoryOwn);
          WrapperCache.add(cPtr, wrapper);
      }
      return wrapper as $csclassname;
  }

  ~$csclassname() {
    WrapperCache.remove(swigCPtr.Handle, this);
  }
%}

%typemap(csbody_derived) TYPE %{
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;

  internal $csclassname(global::System.IntPtr cPtr, bool cMemoryOwn) : base($imclassname.$csclazznameSWIGUpcast(cPtr), cMemoryOwn) {
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr($csclassname obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  internal static new $csclassname getWrapper(global::System.IntPtr cPtr, bool cMemoryOwn) {
      var wrapper = WrapperCache.find(cPtr);
      if (!(wrapper is $csclassname)) {
          wrapper = new $csclassname(cPtr, cMemoryOwn);
          WrapperCache.add(cPtr, wrapper);
      }
      return wrapper as $csclassname;
  }
%}

%typemap(csdispose) TYPE ""

%typemap(csdisposing) TYPE ""

%typemap(csdisposing_derived) TYPE ""

//
%typemap(csdirectorin) TYPE *, TYPE(CLASS::*) "($iminput == global::System.IntPtr.Zero) ? null : $csclassname.getWrapper($iminput, false)"
%typemap(csdirectorin) TYPE & "$csclassname.getWrapper($iminput, false)"
%typemap(csdirectorin) TYPE && "$csclassname.getWrapper($iminput, false)"

//
%typemap(csout, excode=SWIGEXCODE) TYPE {
    $&csclassname ret = $&csclassname.getWrapper($imcall, true);$excode
    return ret;
  }
%typemap(csout, excode=SWIGEXCODE) TYPE & {
    $csclassname ret = $csclassname.getWrapper($imcall, $owner);$excode
    return ret;
  }
%typemap(csout, excode=SWIGEXCODE) TYPE && {
    $csclassname ret = $csclassname.getWrapper($imcall, $owner);$excode
    return ret;
  }
%typemap(csout, excode=SWIGEXCODE) TYPE *, TYPE [] {
    global::System.IntPtr cPtr = $imcall;
    $csclassname ret = (cPtr == global::System.IntPtr.Zero) ? null : $csclassname.getWrapper(cPtr, $owner);$excode
    return ret;
  }
%typemap(csout, excode=SWIGEXCODE) TYPE (CLASS::*) {
    string cMemberPtr = $imcall;
    $csclassname ret = (cMemberPtr == null) ? null : $csclassname.getWrapper(cMemberPtr, $owner);$excode
    return ret;
  }

//
%typemap(csvarout, excode=SWIGEXCODE2) TYPE %{
    get {
      $&csclassname ret = $&csclassname.getWrapper($imcall, true);$excode
      return ret;
    } %}
%typemap(csvarout, excode=SWIGEXCODE2) TYPE & %{
    get {
      $csclassname ret = $csclassname.getWrapper($imcall, $owner);$excode
      return ret;
    } %}
%typemap(csvarout, excode=SWIGEXCODE2) TYPE && %{
    get {
      $csclassname ret = $csclassname.getWrapper($imcall, $owner);$excode
      return ret;
    } %}
%typemap(csvarout, excode=SWIGEXCODE2) TYPE *, TYPE [] %{
    get {
      global::System.IntPtr cPtr = $imcall;
      $csclassname ret = (cPtr == global::System.IntPtr.Zero) ? null : $csclassname.getWrapper(cPtr, $owner);$excode
      return ret;
    } %}

%typemap(csvarout, excode=SWIGEXCODE2) TYPE (CLASS::*) %{
    get {
      string cMemberPtr = $imcall;
      $csclassname ret = (cMemberPtr == null) ? null : $csclassname.getWrapper(cMemberPtr, $owner);$excode
      return ret;
    } %}

%enddef

#endif