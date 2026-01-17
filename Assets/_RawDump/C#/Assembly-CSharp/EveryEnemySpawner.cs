// Decompiled with JetBrains decompiler
// Type: EveryEnemySpawner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections;
using System.Diagnostics;

#nullable disable
public class EveryEnemySpawner : DungeonPlaceableBehaviour, IPlaceConfigurable
{
  public string[] ignoreList;
  public bool reinforce;
  private RoomHandler m_room;
  private AIActor m_blobulinPrefab;

  public void Start()
  {
    this.m_room.Entered += new RoomHandler.OnEnteredEventHandler(this.PlayerEntered);
    this.m_blobulinPrefab = EnemyDatabase.Instance.Entries.Find((Predicate<EnemyDatabaseEntry>) (e => e.path.Contains("/Blobulin.prefab"))).GetPrefab<AIActor>();
  }

  public void ConfigureOnPlacement(RoomHandler room) => this.m_room = room;

  public void PlayerEntered(PlayerController playerController)
  {
    this.StartCoroutine(this.SpawnAllEnemies());
  }

  [DebuggerHidden]
  private IEnumerator SpawnAllEnemies()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new EveryEnemySpawner.\u003CSpawnAllEnemies\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }
}
