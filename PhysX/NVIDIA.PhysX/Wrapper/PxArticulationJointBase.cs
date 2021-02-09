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

public partial class PxArticulationJointBase : PxBase {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;

  internal PxArticulationJointBase(global::System.IntPtr cPtr, bool cMemoryOwn) : base(NativePINVOKE.PxArticulationJointBase_SWIGUpcast(cPtr), cMemoryOwn) {
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(PxArticulationJointBase obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  internal static new PxArticulationJointBase getWrapper(global::System.IntPtr cPtr, bool cMemoryOwn) {
      var wrapper = WrapperCache.find(cPtr);
      if (!(wrapper is PxArticulationJointBase)) {
          wrapper = new PxArticulationJointBase(cPtr, cMemoryOwn);
          WrapperCache.add(cPtr, wrapper);
      }
      return wrapper as PxArticulationJointBase;
  }

  public PxArticulationLink getParentArticulationLink() {
    PxArticulationLink ret = PxArticulationLink.getWrapper(NativePINVOKE.PxArticulationJointBase_getParentArticulationLink(swigCPtr), false);
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void setParentPose( PxTransform  pose) {
    NativePINVOKE.PxArticulationJointBase_setParentPose(swigCPtr,  pose.swigCPtr );
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
  }

  public  PxTransform  getParentPose() {
        global::System.IntPtr ptr = NativePINVOKE.PxArticulationJointBase_getParentPose(swigCPtr);
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
        //PxTransform ret = global::System.Runtime.InteropServices.Marshal.PtrToStructure<PxTransform>(ptr);
        PxTransform ret; unsafe { ret = *(PxTransform*)ptr; }
        return ret;
    }

  public PxArticulationLink getChildArticulationLink() {
    PxArticulationLink ret = PxArticulationLink.getWrapper(NativePINVOKE.PxArticulationJointBase_getChildArticulationLink(swigCPtr), false);
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void setChildPose( PxTransform  pose) {
    NativePINVOKE.PxArticulationJointBase_setChildPose(swigCPtr,  pose.swigCPtr );
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
  }

  public  PxTransform  getChildPose() {
        global::System.IntPtr ptr = NativePINVOKE.PxArticulationJointBase_getChildPose(swigCPtr);
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
        //PxTransform ret = global::System.Runtime.InteropServices.Marshal.PtrToStructure<PxTransform>(ptr);
        PxTransform ret; unsafe { ret = *(PxTransform*)ptr; }
        return ret;
    }

  public PxArticulationJoint getArticulationJoint() {
    global::System.IntPtr cPtr = NativePINVOKE.PxArticulationJointBase_getArticulationJoint(swigCPtr);
    PxArticulationJoint ret = (cPtr == global::System.IntPtr.Zero) ? null : PxArticulationJoint.getWrapper(cPtr, false);
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public PxArticulationJointReducedCoordinate getArticulationJointReducedCoordinate() {
    global::System.IntPtr cPtr = NativePINVOKE.PxArticulationJointBase_getArticulationJointReducedCoordinate(swigCPtr);
    PxArticulationJointReducedCoordinate ret = (cPtr == global::System.IntPtr.Zero) ? null : PxArticulationJointReducedCoordinate.getWrapper(cPtr, false);
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

}

}
