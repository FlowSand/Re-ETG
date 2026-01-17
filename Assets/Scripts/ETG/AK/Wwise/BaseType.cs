// Decompiled with JetBrains decompiler
// Type: AK.Wwise.BaseType
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace AK.Wwise
{
  [Serializable]
  public class BaseType
  {
    public int ID;

    protected uint GetID() => (uint) this.ID;

    public virtual bool IsValid() => this.ID != 0;

    public bool Validate()
    {
      if (this.IsValid())
        return true;
      Debug.LogWarning((object) $"Wwise ID has not been resolved. Consider picking a new {this.GetType().Name}.");
      return false;
    }

    protected void Verify(AKRESULT result)
    {
    }
  }
}
