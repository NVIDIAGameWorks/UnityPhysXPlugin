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

public partial class PxJointLimitCone : PxJointLimitParameters {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;

  internal PxJointLimitCone(global::System.IntPtr cPtr, bool cMemoryOwn) : base(NativePINVOKE.PxJointLimitCone_SWIGUpcast(cPtr), cMemoryOwn) {
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(PxJointLimitCone obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  public override void destroy() {
    lock(this) {
      if (swigCPtr.Handle != global::System.IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          NativePINVOKE.delete_PxJointLimitCone(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
      global::System.GC.SuppressFinalize(this);
      base.destroy();
    }
  }

  public float yAngle {
    set {
      NativePINVOKE.PxJointLimitCone_yAngle_set(swigCPtr, value);
      if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
    } 
    get {
      float ret = NativePINVOKE.PxJointLimitCone_yAngle_get(swigCPtr);
      if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
      return ret;
    } 
  }

  public float zAngle {
    set {
      NativePINVOKE.PxJointLimitCone_zAngle_set(swigCPtr, value);
      if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
    } 
    get {
      float ret = NativePINVOKE.PxJointLimitCone_zAngle_get(swigCPtr);
      if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
      return ret;
    } 
  }

  public PxJointLimitCone(float yLimitAngle, float zLimitAngle, float contactDist) : this(NativePINVOKE.new_PxJointLimitCone__SWIG_0(yLimitAngle, zLimitAngle, contactDist), true) {
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
  }

  public PxJointLimitCone(float yLimitAngle, float zLimitAngle) : this(NativePINVOKE.new_PxJointLimitCone__SWIG_1(yLimitAngle, zLimitAngle), true) {
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
  }

  public PxJointLimitCone(float yLimitAngle, float zLimitAngle, PxSpring spring) : this(NativePINVOKE.new_PxJointLimitCone__SWIG_2(yLimitAngle, zLimitAngle, PxSpring.getCPtr(spring)), true) {
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
  }

  public new bool isValid() {
    bool ret = NativePINVOKE.PxJointLimitCone_isValid(swigCPtr);
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

}

}
