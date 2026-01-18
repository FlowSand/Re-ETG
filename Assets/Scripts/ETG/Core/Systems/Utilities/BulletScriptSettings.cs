// Decompiled with JetBrains decompiler
// Type: BulletScriptSettings
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable

[Serializable]
public class BulletScriptSettings
  {
    public bool overrideMotion;
    public bool preventPooling;
    public bool surviveRigidbodyCollisions;
    public bool surviveTileCollisions;

    public BulletScriptSettings()
    {
    }

    public BulletScriptSettings(BulletScriptSettings other) => this.SetAll(other);

    public void SetAll(BulletScriptSettings other)
    {
      this.overrideMotion = other.overrideMotion;
      this.preventPooling = other.preventPooling;
      this.surviveRigidbodyCollisions = other.surviveRigidbodyCollisions;
      this.surviveTileCollisions = other.surviveTileCollisions;
    }
  }

