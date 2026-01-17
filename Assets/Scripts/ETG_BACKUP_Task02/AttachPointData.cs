// Decompiled with JetBrains decompiler
// Type: AttachPointData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
[Serializable]
public class AttachPointData
{
  public tk2dSpriteDefinition.AttachPoint[] attachPoints;

  public AttachPointData(tk2dSpriteDefinition.AttachPoint[] bcs) => this.attachPoints = bcs;
}
