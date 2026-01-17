// Decompiled with JetBrains decompiler
// Type: RoomMotionHandler
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable
public class RoomMotionHandler : MonoBehaviour
{
  private float c_roomSpeed = 3f;
  private Transform m_transform;
  private float m_zOffset;
  private IntVector2 currentCellPosition;
  private bool m_isMoving;

  public void Initialize(RoomHandler parentRoom)
  {
    this.m_transform = this.transform;
    this.m_zOffset = this.m_transform.position.z - this.m_transform.position.y;
    this.currentCellPosition = parentRoom.area.basePosition;
  }

  public void TriggerMoveTo(IntVector2 targetPosition)
  {
    if (this.m_isMoving || targetPosition == this.currentCellPosition)
      return;
    this.StartCoroutine(this.HandleMove(targetPosition));
  }

  [DebuggerHidden]
  private IEnumerator HandleMove(IntVector2 targetPosition)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new RoomMotionHandler.\u003CHandleMove\u003Ec__Iterator0()
    {
      targetPosition = targetPosition,
      \u0024this = this
    };
  }
}
