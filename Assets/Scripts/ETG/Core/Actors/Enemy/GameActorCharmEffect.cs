using System;

#nullable disable

[Serializable]
public class GameActorCharmEffect : GameActorEffect
  {
    public override void OnEffectApplied(
      GameActor actor,
      RuntimeGameActorEffectData effectData,
      float partialAmount = 1f)
    {
      if (!(actor is AIActor))
        return;
      int num = (int) AkSoundEngine.PostEvent("Play_OBJ_enemy_charmed_01", GameManager.Instance.gameObject);
      AIActor aiActor = actor as AIActor;
      aiActor.CanTargetEnemies = true;
      aiActor.CanTargetPlayers = false;
    }

    public override void OnEffectRemoved(GameActor actor, RuntimeGameActorEffectData effectData)
    {
      if (!(actor is AIActor))
        return;
      AIActor aiActor = actor as AIActor;
      aiActor.CanTargetEnemies = false;
      aiActor.CanTargetPlayers = true;
    }
  }

