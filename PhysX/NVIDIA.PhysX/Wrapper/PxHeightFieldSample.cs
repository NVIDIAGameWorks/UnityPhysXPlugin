//------------------------------------------------------------------------------
// <auto-generated />
//
// This file was automatically generated by SWIG (http://www.swig.org).
// Version 4.0.0
//
// Do not make changes to this file unless you know what you are doing--modify
// the SWIG interface file instead.
//------------------------------------------------------------------------------

namespace NVIDIA.PhysX {

[global::System.Runtime.InteropServices.StructLayout(global::System.Runtime.InteropServices.LayoutKind.Sequential)]
public partial struct PxHeightFieldSample {

  public short height; public byte materialIndex0,materialIndex1;

  internal global::System.IntPtr swigCPtr {
    get { unsafe { fixed(PxHeightFieldSample* p = &this) return (global::System.IntPtr)p; } }
  }

  internal PxHeightFieldSample(global::System.IntPtr ptr, bool unused) {
      //this = global::System.Runtime.InteropServices.Marshal.PtrToStructure<PxHeightFieldSample>(ptr);
      unsafe { this = *(PxHeightFieldSample*)ptr; }
  }
    
  public byte tessFlag() {
    byte ret = NativePINVOKE.PxHeightFieldSample_tessFlag(swigCPtr);
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void setTessFlag() {
    NativePINVOKE.PxHeightFieldSample_setTessFlag(swigCPtr);
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
  }

  public void clearTessFlag() {
    NativePINVOKE.PxHeightFieldSample_clearTessFlag(swigCPtr);
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
  }

}

}