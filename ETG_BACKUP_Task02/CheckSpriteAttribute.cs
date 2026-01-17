// Decompiled with JetBrains decompiler
// Type: CheckSpriteAttribute
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class CheckSpriteAttribute : PropertyAttribute
{
  public string sprite;

  public CheckSpriteAttribute(string sprite = null) => this.sprite = sprite;
}
