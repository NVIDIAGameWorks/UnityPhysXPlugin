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

public partial class PxArticulationJointReducedCoordinate : PxArticulationJointBase {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;

  internal PxArticulationJointReducedCoordinate(global::System.IntPtr cPtr, bool cMemoryOwn) : base(NativePINVOKE.PxArticulationJointReducedCoordinate_SWIGUpcast(cPtr), cMemoryOwn) {
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(PxArticulationJointReducedCoordinate obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  internal static new PxArticulationJointReducedCoordinate getWrapper(global::System.IntPtr cPtr, bool cMemoryOwn) {
      var wrapper = WrapperCache.find(cPtr);
      if (!(wrapper is PxArticulationJointReducedCoordinate)) {
          wrapper = new PxArticulationJointReducedCoordinate(cPtr, cMemoryOwn);
          WrapperCache.add(cPtr, wrapper);
      }
      return wrapper as PxArticulationJointReducedCoordinate;
  }

  public void setJointType(PxArticulationJointType jointType) {
    NativePINVOKE.PxArticulationJointReducedCoordinate_setJointType(swigCPtr, (int)jointType);
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
  }

  public PxArticulationJointType getJointType() {
    PxArticulationJointType ret = (PxArticulationJointType)NativePINVOKE.PxArticulationJointReducedCoordinate_getJointType(swigCPtr);
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void setMotion(PxArticulationAxis axis, PxArticulationMotion motion) {
    NativePINVOKE.PxArticulationJointReducedCoordinate_setMotion(swigCPtr, (int)axis, (int)motion);
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
  }

  public PxArticulationMotion getMotion(PxArticulationAxis axis) {
    PxArticulationMotion ret = (PxArticulationMotion)NativePINVOKE.PxArticulationJointReducedCoordinate_getMotion(swigCPtr, (int)axis);
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void setLimit(PxArticulationAxis axis, float lowLimit, float highLimit) {
    NativePINVOKE.PxArticulationJointReducedCoordinate_setLimit(swigCPtr, (int)axis, lowLimit, highLimit);
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
  }

  public void getLimit(PxArticulationAxis axis, out float lowLimit, out float highLimit) {
    NativePINVOKE.PxArticulationJointReducedCoordinate_getLimit(swigCPtr, (int)axis, out lowLimit, out highLimit);
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
  }

  public void setDrive(PxArticulationAxis axis, float stiffness, float damping, float maxForce, PxArticulationDriveType driveType) {
    NativePINVOKE.PxArticulationJointReducedCoordinate_setDrive__SWIG_0(swigCPtr, (int)axis, stiffness, damping, maxForce, (int)driveType);
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
  }

  public void setDrive(PxArticulationAxis axis, float stiffness, float damping, float maxForce) {
    NativePINVOKE.PxArticulationJointReducedCoordinate_setDrive__SWIG_1(swigCPtr, (int)axis, stiffness, damping, maxForce);
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
  }

  public void getDrive(PxArticulationAxis axis, out float stiffness, out float damping, out float maxForce, out PxArticulationDriveType driveType) {
    NativePINVOKE.PxArticulationJointReducedCoordinate_getDrive(swigCPtr, (int)axis, out stiffness, out damping, out maxForce, out driveType);
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
  }

  public void setDriveTarget(PxArticulationAxis axis, float target) {
    NativePINVOKE.PxArticulationJointReducedCoordinate_setDriveTarget(swigCPtr, (int)axis, target);
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
  }

  public void setDriveVelocity(PxArticulationAxis axis, float targetVel) {
    NativePINVOKE.PxArticulationJointReducedCoordinate_setDriveVelocity(swigCPtr, (int)axis, targetVel);
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
  }

  public float getDriveTarget(PxArticulationAxis axis) {
    float ret = NativePINVOKE.PxArticulationJointReducedCoordinate_getDriveTarget(swigCPtr, (int)axis);
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public float getDriveVelocity(PxArticulationAxis axis) {
    float ret = NativePINVOKE.PxArticulationJointReducedCoordinate_getDriveVelocity(swigCPtr, (int)axis);
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void setFrictionCoefficient(float coefficient) {
    NativePINVOKE.PxArticulationJointReducedCoordinate_setFrictionCoefficient(swigCPtr, coefficient);
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
  }

  public float getFrictionCoefficient() {
    float ret = NativePINVOKE.PxArticulationJointReducedCoordinate_getFrictionCoefficient(swigCPtr);
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void setMaxJointVelocity(float maxJointV) {
    NativePINVOKE.PxArticulationJointReducedCoordinate_setMaxJointVelocity(swigCPtr, maxJointV);
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
  }

  public float getMaxJointVelocity() {
    float ret = NativePINVOKE.PxArticulationJointReducedCoordinate_getMaxJointVelocity(swigCPtr);
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

}

}
