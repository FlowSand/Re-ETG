// Decompiled with JetBrains decompiler
// Type: Dungeonator.DebugDungeonSettings
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace Dungeonator;

[Serializable]
public class DebugDungeonSettings
{
  public bool RAPID_DEBUG_DUNGEON_ITERATION_SEEKER;
  public bool RAPID_DEBUG_DUNGEON_ITERATION;
  public int RAPID_DEBUG_DUNGEON_COUNT = 50;
  public bool GENERATION_VIEWER_MODE;
  public bool FULL_MINIMAP_VISIBILITY;
  public bool COOP_TEST;
  [Header("Generation Options")]
  public bool DISABLE_ENEMIES;
  public bool DISABLE_LOOPS;
  public bool DISABLE_SECRET_ROOM_COVERS;
  public bool DISABLE_OUTLINES;
  public bool WALLS_ARE_PITS;
}
