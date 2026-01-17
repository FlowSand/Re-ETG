// Decompiled with JetBrains decompiler
// Type: CorpseSpawnController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class CorpseSpawnController : BraveBehaviour
    {
      [CheckDirectionalAnimation(null)]
      public string PrespawnAnim = "corpse_prespawn";
      [CheckDirectionalAnimation(null)]
      public string SpawnAnim = "corpse_spawn";
      [EnemyIdentifier]
      public string EnemyGuid;
      public Vector2 LeftSpawnOffset;
      public Vector2 RightSpawnOffset;
      public float PrespawnTime = 5f;
      public float AdditionalPrespawnTime = 5f;
      public bool CancelOnRoomClear = true;
      private CorpseSpawnController.State m_state;
      private bool m_isRight;
      private RoomHandler m_room;

      public void Start()
      {
        this.spriteAnimator.AnimationEventTriggered += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.AnimationEventTriggered);
      }

      public void Update()
      {
        if (this.m_state == CorpseSpawnController.State.Prespawn)
        {
          if (!this.aiAnimator.IsPlaying(this.PrespawnAnim))
          {
            this.aiAnimator.PlayUntilFinished(this.SpawnAnim);
            this.m_state = CorpseSpawnController.State.Spawning;
          }
        }
        else if (this.m_state == CorpseSpawnController.State.Spawning && !this.aiAnimator.IsPlaying(this.SpawnAnim))
        {
          Vector2 position = (Vector2) this.transform.position;
          Vector2 vector2 = !this.m_isRight ? this.LeftSpawnOffset : this.RightSpawnOffset;
          AIActor.Spawn(EnemyDatabase.GetOrLoadByGuid(this.EnemyGuid), position + vector2, GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(position.ToIntVector2()), true);
          UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
        }
        if (this.m_state == CorpseSpawnController.State.None || !this.CancelOnRoomClear || this.m_room.GetActiveEnemiesCount(RoomHandler.ActiveEnemyType.RoomClear) != 0)
          return;
        this.m_state = CorpseSpawnController.State.None;
        this.aiAnimator.PlayUntilCancelled(this.PrespawnAnim);
        this.aiAnimator.enabled = false;
        this.spriteAnimator.enabled = false;
        this.sprite.OverrideMaterialMode = tk2dBaseSprite.SpriteMaterialOverrideMode.OVERRIDE_MATERIAL_COMPLEX;
        this.GetComponent<DebrisObject>().FadeToOverrideColor(new Color(0.0f, 0.0f, 0.0f, 0.6f), 0.25f);
        this.GetComponent<Renderer>().material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTiltedCutoutFastPixelShadow");
      }

      protected override void OnDestroy() => base.OnDestroy();

      public void Init(AIActor aiActor)
      {
        this.m_room = aiActor.ParentRoom;
        float prespawnTime = this.PrespawnTime;
        CorpseSpawnController[] objectsOfType = UnityEngine.Object.FindObjectsOfType<CorpseSpawnController>();
        if (objectsOfType != null && objectsOfType.Length > 1)
          prespawnTime += (float) (objectsOfType.Length - 1) * this.AdditionalPrespawnTime;
        this.m_isRight = !aiActor.sprite.CurrentSprite.name.Contains("left");
        this.aiAnimator.FacingDirection = !this.m_isRight ? 180f : 0.0f;
        this.aiAnimator.PlayForDuration(this.PrespawnAnim, prespawnTime);
        this.m_state = CorpseSpawnController.State.Prespawn;
      }

      private void AnimationEventTriggered(
        tk2dSpriteAnimator animator,
        tk2dSpriteAnimationClip clip,
        int frame)
      {
        if (this.m_state == CorpseSpawnController.State.None || !(clip.GetFrame(frame).eventInfo == "perp"))
          return;
        this.sprite.IsPerpendicular = true;
      }

      private enum State
      {
        None,
        Prespawn,
        Spawning,
      }
    }

}
