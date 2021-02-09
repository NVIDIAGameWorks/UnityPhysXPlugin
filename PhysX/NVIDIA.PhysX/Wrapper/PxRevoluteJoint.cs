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

public partial class PxRevoluteJoint : PxJoint {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;

  internal PxRevoluteJoint(global::System.IntPtr cPtr, bool cMemoryOwn) : base(NativePINVOKE.PxRevoluteJoint_SWIGUpcast(cPtr), cMemoryOwn) {
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(PxRevoluteJoint obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  internal static new PxRevoluteJoint getWrapper(global::System.IntPtr cPtr, bool cMemoryOwn) {
      var wrapper = WrapperCache.find(cPtr);
      if (!(wrapper is PxRevoluteJoint)) {
          wrapper = new PxRevoluteJoint(cPtr, cMemoryOwn);
          WrapperCache.add(cPtr, wrapper);
      }
      return wrapper as PxRevoluteJoint;
  }

  public float getAngle() {
    float ret = NativePINVOKE.PxRevoluteJoint_getAngle(swigCPtr);
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public float getVelocity() {
    float ret = NativePINVOKE.PxRevoluteJoint_getVelocity(swigCPtr);
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void setLimit(PxJointAngularLimitPair limits) {
    NativePINVOKE.PxRevoluteJoint_setLimit(swigCPtr, PxJointAngularLimitPair.getCPtr(limits));
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
  }

  public PxJointAngularLimitPair getLimit() {
    PxJointAngularLimitPair ret = new PxJointAngularLimitPair(NativePINVOKE.PxRevoluteJoint_getLimit(swigCPtr), true);
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void setDriveVelocity(float velocity, bool autowake) {
    NativePINVOKE.PxRevoluteJoint_setDriveVelocity__SWIG_0(swigCPtr, velocity, autowake);
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
  }

  public void setDriveVelocity(float velocity) {
    NativePINVOKE.PxRevoluteJoint_setDriveVelocity__SWIG_1(swigCPtr, velocity);
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
  }

  public float getDriveVelocity() {
    float ret = NativePINVOKE.PxRevoluteJoint_getDriveVelocity(swigCPtr);
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void setDriveForceLimit(float limit) {
    NativePINVOKE.PxRevoluteJoint_setDriveForceLimit(swigCPtr, limit);
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
  }

  public float getDriveForceLimit() {
    float ret = NativePINVOKE.PxRevoluteJoint_getDriveForceLimit(swigCPtr);
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void setDriveGearRatio(float ratio) {
    NativePINVOKE.PxRevoluteJoint_setDriveGearRatio(swigCPtr, ratio);
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
  }

  public float getDriveGearRatio() {
    float ret = NativePINVOKE.PxRevoluteJoint_getDriveGearRatio(swigCPtr);
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void setRevoluteJointFlags(PxRevoluteJointFlag flags) {
    NativePINVOKE.PxRevoluteJoint_setRevoluteJointFlags(swigCPtr, (int)flags);
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
  }

  public void setRevoluteJointFlag(PxRevoluteJointFlag flag, bool value) {
    NativePINVOKE.PxRevoluteJoint_setRevoluteJointFlag(swigCPtr, (int)flag, value);
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
  }

  public PxRevoluteJointFlag getRevoluteJointFlags() {
    PxRevoluteJointFlag ret = (PxRevoluteJointFlag)NativePINVOKE.PxRevoluteJoint_getRevoluteJointFlags(swigCPtr);
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void setProjectionLinearTolerance(float tolerance) {
    NativePINVOKE.PxRevoluteJoint_setProjectionLinearTolerance(swigCPtr, tolerance);
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
  }

  public float getProjectionLinearTolerance() {
    float ret = NativePINVOKE.PxRevoluteJoint_getProjectionLinearTolerance(swigCPtr);
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void setProjectionAngularTolerance(float tolerance) {
    NativePINVOKE.PxRevoluteJoint_setProjectionAngularTolerance(swigCPtr, tolerance);
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
  }

  public float getProjectionAngularTolerance() {
    float ret = NativePINVOKE.PxRevoluteJoint_getProjectionAngularTolerance(swigCPtr);
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

}

}
