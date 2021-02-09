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

public partial class PxPrismaticJoint : PxJoint {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;

  internal PxPrismaticJoint(global::System.IntPtr cPtr, bool cMemoryOwn) : base(NativePINVOKE.PxPrismaticJoint_SWIGUpcast(cPtr), cMemoryOwn) {
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(PxPrismaticJoint obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  internal static new PxPrismaticJoint getWrapper(global::System.IntPtr cPtr, bool cMemoryOwn) {
      var wrapper = WrapperCache.find(cPtr);
      if (!(wrapper is PxPrismaticJoint)) {
          wrapper = new PxPrismaticJoint(cPtr, cMemoryOwn);
          WrapperCache.add(cPtr, wrapper);
      }
      return wrapper as PxPrismaticJoint;
  }

  public float getPosition() {
    float ret = NativePINVOKE.PxPrismaticJoint_getPosition(swigCPtr);
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public float getVelocity() {
    float ret = NativePINVOKE.PxPrismaticJoint_getVelocity(swigCPtr);
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void setLimit(PxJointLinearLimitPair arg0) {
    NativePINVOKE.PxPrismaticJoint_setLimit(swigCPtr, PxJointLinearLimitPair.getCPtr(arg0));
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
  }

  public PxJointLinearLimitPair getLimit() {
    PxJointLinearLimitPair ret = new PxJointLinearLimitPair(NativePINVOKE.PxPrismaticJoint_getLimit(swigCPtr), true);
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void setPrismaticJointFlags(PxPrismaticJointFlag flags) {
    NativePINVOKE.PxPrismaticJoint_setPrismaticJointFlags(swigCPtr, (int)flags);
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
  }

  public void setPrismaticJointFlag(PxPrismaticJointFlag flag, bool value) {
    NativePINVOKE.PxPrismaticJoint_setPrismaticJointFlag(swigCPtr, (int)flag, value);
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
  }

  public PxPrismaticJointFlag getPrismaticJointFlags() {
    PxPrismaticJointFlag ret = (PxPrismaticJointFlag)NativePINVOKE.PxPrismaticJoint_getPrismaticJointFlags(swigCPtr);
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void setProjectionLinearTolerance(float tolerance) {
    NativePINVOKE.PxPrismaticJoint_setProjectionLinearTolerance(swigCPtr, tolerance);
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
  }

  public float getProjectionLinearTolerance() {
    float ret = NativePINVOKE.PxPrismaticJoint_getProjectionLinearTolerance(swigCPtr);
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void setProjectionAngularTolerance(float tolerance) {
    NativePINVOKE.PxPrismaticJoint_setProjectionAngularTolerance(swigCPtr, tolerance);
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
  }

  public float getProjectionAngularTolerance() {
    float ret = NativePINVOKE.PxPrismaticJoint_getProjectionAngularTolerance(swigCPtr);
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

}

}
