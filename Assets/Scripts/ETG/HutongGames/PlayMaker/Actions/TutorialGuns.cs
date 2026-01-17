// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.TutorialGuns
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Controls the wall guns in the tutorial.")]
[ActionCategory(".NPCs")]
public class TutorialGuns : FsmStateAction
{
  public FsmBool enable;
  public FsmBool disable;

  public override void OnEnter()
  {
    List<AIActor> activeEnemies = this.Owner.GetComponent<TalkDoerLite>().ParentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
    for (int index = 0; index < activeEnemies.Count; ++index)
    {
      AIActor aiActor = activeEnemies[index];
      if (this.enable.Value)
      {
        aiActor.enabled = true;
        aiActor.specRigidbody.enabled = true;
        aiActor.State = AIActor.ActorState.Normal;
      }
      if (this.disable.Value)
      {
        aiActor.enabled = false;
        aiActor.aiAnimator.PlayUntilCancelled("deactivate");
        aiActor.specRigidbody.enabled = false;
      }
    }
    if (this.disable.Value)
    {
      ReadOnlyCollection<Projectile> allProjectiles = StaticReferenceManager.AllProjectiles;
      for (int index = allProjectiles.Count - 1; index >= 0; --index)
      {
        if ((bool) (Object) allProjectiles[index])
          allProjectiles[index].DieInAir();
      }
    }
    this.Finish();
  }
}
