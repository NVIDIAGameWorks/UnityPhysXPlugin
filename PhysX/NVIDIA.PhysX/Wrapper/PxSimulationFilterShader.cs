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
public partial struct PxSimulationFilterShader {

  global::System.IntPtr fnPtr;

  internal global::System.IntPtr swigCPtr {
    get { unsafe { fixed(PxSimulationFilterShader* p = &this) return (global::System.IntPtr)p; } }
  }

  internal PxSimulationFilterShader(global::System.IntPtr ptr, bool unused) {
      //this = global::System.Runtime.InteropServices.Marshal.PtrToStructure<PxSimulationFilterShader>(ptr);
      unsafe { this = *(PxSimulationFilterShader*)ptr; }
  }
    
}

}
