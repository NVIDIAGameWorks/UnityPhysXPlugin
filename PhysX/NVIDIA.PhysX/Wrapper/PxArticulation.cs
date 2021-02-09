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

public partial class PxArticulation : PxArticulationBase {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;

  internal PxArticulation(global::System.IntPtr cPtr, bool cMemoryOwn) : base(NativePINVOKE.PxArticulation_SWIGUpcast(cPtr), cMemoryOwn) {
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(PxArticulation obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  internal static new PxArticulation getWrapper(global::System.IntPtr cPtr, bool cMemoryOwn) {
      var wrapper = WrapperCache.find(cPtr);
      if (!(wrapper is PxArticulation)) {
          wrapper = new PxArticulation(cPtr, cMemoryOwn);
          WrapperCache.add(cPtr, wrapper);
      }
      return wrapper as PxArticulation;
  }

  public override void release() {
    NativePINVOKE.PxArticulation_release(swigCPtr);
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
  }

  public void setMaxProjectionIterations(uint iterations) {
    NativePINVOKE.PxArticulation_setMaxProjectionIterations(swigCPtr, iterations);
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
  }

  public uint getMaxProjectionIterations() {
    uint ret = NativePINVOKE.PxArticulation_getMaxProjectionIterations(swigCPtr);
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void setSeparationTolerance(float tolerance) {
    NativePINVOKE.PxArticulation_setSeparationTolerance(swigCPtr, tolerance);
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
  }

  public float getSeparationTolerance() {
    float ret = NativePINVOKE.PxArticulation_getSeparationTolerance(swigCPtr);
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void setInternalDriveIterations(uint iterations) {
    NativePINVOKE.PxArticulation_setInternalDriveIterations(swigCPtr, iterations);
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
  }

  public uint getInternalDriveIterations() {
    uint ret = NativePINVOKE.PxArticulation_getInternalDriveIterations(swigCPtr);
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void setExternalDriveIterations(uint iterations) {
    NativePINVOKE.PxArticulation_setExternalDriveIterations(swigCPtr, iterations);
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
  }

  public uint getExternalDriveIterations() {
    uint ret = NativePINVOKE.PxArticulation_getExternalDriveIterations(swigCPtr);
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public PxArticulationDriveCache createDriveCache(float compliance, uint driveIterations) {
    global::System.IntPtr cPtr = NativePINVOKE.PxArticulation_createDriveCache(swigCPtr, compliance, driveIterations);
    PxArticulationDriveCache ret = (cPtr == global::System.IntPtr.Zero) ? null : new PxArticulationDriveCache(cPtr, false);
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void updateDriveCache(PxArticulationDriveCache driveCache, float compliance, uint driveIterations) {
    NativePINVOKE.PxArticulation_updateDriveCache(swigCPtr, PxArticulationDriveCache.getCPtr(driveCache), compliance, driveIterations);
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
  }

  public void releaseDriveCache(PxArticulationDriveCache driveCache) {
    NativePINVOKE.PxArticulation_releaseDriveCache(swigCPtr, PxArticulationDriveCache.getCPtr(driveCache));
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
  }

  public void applyImpulse(PxArticulationLink link, PxArticulationDriveCache driveCache,  PxVec3  linearImpulse,  PxVec3  angularImpulse) {
    NativePINVOKE.PxArticulation_applyImpulse(swigCPtr, PxArticulationLink.getCPtr(link), PxArticulationDriveCache.getCPtr(driveCache),  linearImpulse.swigCPtr ,  angularImpulse.swigCPtr );
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
  }

  public void computeImpulseResponse(PxArticulationLink link,  ref PxVec3  linearResponse,  ref PxVec3  angularResponse, PxArticulationDriveCache driveCache,  PxVec3  linearImpulse,  PxVec3  angularImpulse) {
    NativePINVOKE.PxArticulation_computeImpulseResponse(swigCPtr, PxArticulationLink.getCPtr(link),  linearResponse.swigCPtr ,  angularResponse.swigCPtr , PxArticulationDriveCache.getCPtr(driveCache),  linearImpulse.swigCPtr ,  angularImpulse.swigCPtr );
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
  }

}

}