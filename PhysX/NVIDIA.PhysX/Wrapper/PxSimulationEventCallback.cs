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

public partial class PxSimulationEventCallback {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal PxSimulationEventCallback(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(PxSimulationEventCallback obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  ~PxSimulationEventCallback() {
    destroy();
  }

  public virtual void destroy() {
    lock(this) {
      if (swigCPtr.Handle != global::System.IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          NativePINVOKE.delete_PxSimulationEventCallback(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
      global::System.GC.SuppressFinalize(this);
    }
  }

  public PxSimulationEventCallback() : this(NativePINVOKE.new_PxSimulationEventCallback(), true) {
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
    SwigDirectorConnect();
  }

  public virtual void onConstraintBreak(PxConstraintInfoList constraints) {
    if (SwigDerivedClassHasMethod("onConstraintBreak", swigMethodTypes0)) NativePINVOKE.PxSimulationEventCallback_onConstraintBreakSwigExplicitPxSimulationEventCallback(swigCPtr, PxConstraintInfoList.getCPtr(constraints)); else NativePINVOKE.PxSimulationEventCallback_onConstraintBreak(swigCPtr, PxConstraintInfoList.getCPtr(constraints));
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
  }

  public virtual void onWake(PxActorList actors) {
    if (SwigDerivedClassHasMethod("onWake", swigMethodTypes1)) NativePINVOKE.PxSimulationEventCallback_onWakeSwigExplicitPxSimulationEventCallback(swigCPtr, PxActorList.getCPtr(actors)); else NativePINVOKE.PxSimulationEventCallback_onWake(swigCPtr, PxActorList.getCPtr(actors));
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
  }

  public virtual void onSleep(PxActorList actors) {
    if (SwigDerivedClassHasMethod("onSleep", swigMethodTypes2)) NativePINVOKE.PxSimulationEventCallback_onSleepSwigExplicitPxSimulationEventCallback(swigCPtr, PxActorList.getCPtr(actors)); else NativePINVOKE.PxSimulationEventCallback_onSleep(swigCPtr, PxActorList.getCPtr(actors));
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
  }

  public virtual void onContact(PxContactPairHeader pairHeader, PxContactPairList pairs) {
    if (SwigDerivedClassHasMethod("onContact", swigMethodTypes3)) NativePINVOKE.PxSimulationEventCallback_onContactSwigExplicitPxSimulationEventCallback(swigCPtr, PxContactPairHeader.getCPtr(pairHeader), PxContactPairList.getCPtr(pairs)); else NativePINVOKE.PxSimulationEventCallback_onContact(swigCPtr, PxContactPairHeader.getCPtr(pairHeader), PxContactPairList.getCPtr(pairs));
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
  }

  public virtual void onTrigger(PxTriggerPairList pairs) {
    if (SwigDerivedClassHasMethod("onTrigger", swigMethodTypes4)) NativePINVOKE.PxSimulationEventCallback_onTriggerSwigExplicitPxSimulationEventCallback(swigCPtr, PxTriggerPairList.getCPtr(pairs)); else NativePINVOKE.PxSimulationEventCallback_onTrigger(swigCPtr, PxTriggerPairList.getCPtr(pairs));
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
  }

  public virtual void onAdvance(PxRigidBodyList bodyBuffer, PxTransformList poseBuffer) {
    if (SwigDerivedClassHasMethod("onAdvance", swigMethodTypes5)) NativePINVOKE.PxSimulationEventCallback_onAdvanceSwigExplicitPxSimulationEventCallback(swigCPtr, PxRigidBodyList.getCPtr(bodyBuffer), PxTransformList.getCPtr(poseBuffer)); else NativePINVOKE.PxSimulationEventCallback_onAdvance(swigCPtr, PxRigidBodyList.getCPtr(bodyBuffer), PxTransformList.getCPtr(poseBuffer));
    if (NativePINVOKE.SWIGPendingException.Pending) throw NativePINVOKE.SWIGPendingException.Retrieve();
  }

  private void SwigDirectorConnect() {
    if (SwigDerivedClassHasMethod("onConstraintBreak", swigMethodTypes0))
      swigDelegate0 = new SwigDelegatePxSimulationEventCallback_0(SwigDirectorMethodonConstraintBreak);
    if (SwigDerivedClassHasMethod("onWake", swigMethodTypes1))
      swigDelegate1 = new SwigDelegatePxSimulationEventCallback_1(SwigDirectorMethodonWake);
    if (SwigDerivedClassHasMethod("onSleep", swigMethodTypes2))
      swigDelegate2 = new SwigDelegatePxSimulationEventCallback_2(SwigDirectorMethodonSleep);
    if (SwigDerivedClassHasMethod("onContact", swigMethodTypes3))
      swigDelegate3 = new SwigDelegatePxSimulationEventCallback_3(SwigDirectorMethodonContact);
    if (SwigDerivedClassHasMethod("onTrigger", swigMethodTypes4))
      swigDelegate4 = new SwigDelegatePxSimulationEventCallback_4(SwigDirectorMethodonTrigger);
    if (SwigDerivedClassHasMethod("onAdvance", swigMethodTypes5))
      swigDelegate5 = new SwigDelegatePxSimulationEventCallback_5(SwigDirectorMethodonAdvance);
    NativePINVOKE.PxSimulationEventCallback_director_connect(swigCPtr, swigDelegate0, swigDelegate1, swigDelegate2, swigDelegate3, swigDelegate4, swigDelegate5);
  }

  private bool SwigDerivedClassHasMethod(string methodName, global::System.Type[] methodTypes) {
    global::System.Reflection.MethodInfo methodInfo = this.GetType().GetMethod(methodName, global::System.Reflection.BindingFlags.Public | global::System.Reflection.BindingFlags.NonPublic | global::System.Reflection.BindingFlags.Instance, null, methodTypes, null);
    bool hasDerivedMethod = methodInfo.DeclaringType.IsSubclassOf(typeof(PxSimulationEventCallback));
    return hasDerivedMethod;
  }

  private void SwigDirectorMethodonConstraintBreak(global::System.IntPtr constraints) {
    onConstraintBreak(new PxConstraintInfoList(constraints, false));
  }

  private void SwigDirectorMethodonWake(global::System.IntPtr actors) {
    onWake(new PxActorList(actors, false));
  }

  private void SwigDirectorMethodonSleep(global::System.IntPtr actors) {
    onSleep(new PxActorList(actors, false));
  }

  private void SwigDirectorMethodonContact(global::System.IntPtr pairHeader, global::System.IntPtr pairs) {
    onContact(new PxContactPairHeader(pairHeader, false), new PxContactPairList(pairs, false));
  }

  private void SwigDirectorMethodonTrigger(global::System.IntPtr pairs) {
    onTrigger(new PxTriggerPairList(pairs, false));
  }

  private void SwigDirectorMethodonAdvance(global::System.IntPtr bodyBuffer, global::System.IntPtr poseBuffer) {
    onAdvance(new PxRigidBodyList(bodyBuffer, false), new PxTransformList(poseBuffer, false));
  }

  public delegate void SwigDelegatePxSimulationEventCallback_0(global::System.IntPtr constraints);
  public delegate void SwigDelegatePxSimulationEventCallback_1(global::System.IntPtr actors);
  public delegate void SwigDelegatePxSimulationEventCallback_2(global::System.IntPtr actors);
  public delegate void SwigDelegatePxSimulationEventCallback_3(global::System.IntPtr pairHeader, global::System.IntPtr pairs);
  public delegate void SwigDelegatePxSimulationEventCallback_4(global::System.IntPtr pairs);
  public delegate void SwigDelegatePxSimulationEventCallback_5(global::System.IntPtr bodyBuffer, global::System.IntPtr poseBuffer);

  private SwigDelegatePxSimulationEventCallback_0 swigDelegate0;
  private SwigDelegatePxSimulationEventCallback_1 swigDelegate1;
  private SwigDelegatePxSimulationEventCallback_2 swigDelegate2;
  private SwigDelegatePxSimulationEventCallback_3 swigDelegate3;
  private SwigDelegatePxSimulationEventCallback_4 swigDelegate4;
  private SwigDelegatePxSimulationEventCallback_5 swigDelegate5;

  private static global::System.Type[] swigMethodTypes0 = new global::System.Type[] { typeof(PxConstraintInfoList) };
  private static global::System.Type[] swigMethodTypes1 = new global::System.Type[] { typeof(PxActorList) };
  private static global::System.Type[] swigMethodTypes2 = new global::System.Type[] { typeof(PxActorList) };
  private static global::System.Type[] swigMethodTypes3 = new global::System.Type[] { typeof(PxContactPairHeader), typeof(PxContactPairList) };
  private static global::System.Type[] swigMethodTypes4 = new global::System.Type[] { typeof(PxTriggerPairList) };
  private static global::System.Type[] swigMethodTypes5 = new global::System.Type[] { typeof(PxRigidBodyList), typeof(PxTransformList) };
}

}
