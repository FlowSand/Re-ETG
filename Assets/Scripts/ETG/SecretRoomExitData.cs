// Decompiled with JetBrains decompiler
// Type: SecretRoomExitData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using UnityEngine;

#nullable disable
public class SecretRoomExitData
{
  public GameObject colliderObject;
  public DungeonData.Direction exitDirection;

  public SecretRoomExitData(GameObject g, DungeonData.Direction d)
  {
    this.colliderObject = g;
    this.exitDirection = d;
  }
}
