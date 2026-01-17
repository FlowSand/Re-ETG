// Decompiled with JetBrains decompiler
// Type: FullInspector.Internal.fiUnityObjectReference
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

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
