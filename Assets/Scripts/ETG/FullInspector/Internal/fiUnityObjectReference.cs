using System;
using UnityEngine;

#nullable disable
namespace FullInspector.Internal
{
  [Serializable]
  public class fiUnityObjectReference
  {
    [SerializeField]
    private UnityEngine.Object _target;

    public fiUnityObjectReference()
    {
    }

    public fiUnityObjectReference(UnityEngine.Object target) => this.Target = target;

    public bool IsValid => this.Target != (UnityEngine.Object) null;

    public UnityEngine.Object Target
    {
      get
      {
        if (this._target == (UnityEngine.Object) null)
          this.TryRestoreFromInstanceId();
        return this._target;
      }
      set => this._target = value;
    }

    private void TryRestoreFromInstanceId()
    {
      if (object.ReferenceEquals((object) this._target, (object) null))
        return;
      this._target = fiLateBindings.EditorUtility.InstanceIDToObject(this._target.GetInstanceID());
    }

    public override int GetHashCode() => !this.IsValid ? 0 : this.Target.GetHashCode();

    public override bool Equals(object obj)
    {
      return obj is fiUnityObjectReference unityObjectReference && unityObjectReference.Target == this.Target;
    }
  }
}
