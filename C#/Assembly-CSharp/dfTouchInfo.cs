// Decompiled with JetBrains decompiler
// Type: dfTouchInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Runtime.InteropServices;
using UnityEngine;

#nullable disable
[Serializable]
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct dfTouchInfo(
  int fingerID,
  TouchPhase phase,
  int tapCount,
  Vector2 position,
  Vector2 positionDelta,
  float timeDelta)
{
  private int m_FingerId = fingerID;
  private Vector2 m_Position = position;
  private Vector2 m_RawPosition = position;
  private Vector2 m_PositionDelta = positionDelta;
  private float m_TimeDelta = timeDelta;
  private int m_TapCount = tapCount;
  private TouchPhase m_Phase = phase;

  public int fingerId => this.m_FingerId;

  public Vector2 position => this.m_Position;

  public Vector2 rawPosition => this.m_RawPosition;

  public Vector2 deltaPosition => this.m_PositionDelta;

  public float deltaTime => this.m_TimeDelta;

  public int tapCount => this.m_TapCount;

  public TouchPhase phase => this.m_Phase;

  public static implicit operator dfTouchInfo(Touch touch)
  {
    return new dfTouchInfo()
    {
      m_PositionDelta = touch.deltaPosition,
      m_TimeDelta = touch.deltaTime,
      m_FingerId = touch.fingerId,
      m_Phase = touch.phase,
      m_Position = touch.position,
      m_TapCount = touch.tapCount
    };
  }
}
