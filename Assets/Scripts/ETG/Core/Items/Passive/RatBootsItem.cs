// Decompiled with JetBrains decompiler
// Type: RatBootsItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Items.Passive
{
    public class RatBootsItem : PassiveItem
    {
      public float HoverTime = 2f;
      public float FlickerPortion = 0.5f;
      public float FlickerFrequency = 0.1f;
      public GameObject FloorVFX;
      public tk2dSpriteAnimation RatAnimationLibrary;
      private tk2dSprite m_extantFloor;
      private bool m_transformed;
      private PlayerController m_lastPlayer;
      private bool m_frameWasPartialPit;
      private bool m_invulnerable;
      private bool m_wasAboutToFallLastFrame;
      private float m_elapsedAboutToFall;
      private int m_lastFrameAboutToFall;

      public override void Pickup(PlayerController player)
      {
        base.Pickup(player);
        PlayerController playerController = player;
        playerController.OnAboutToFall = playerController.OnAboutToFall + new Func<bool, bool>(this.HandleAboutToFall);
      }

      protected void EnableShader(PlayerController user)
      {
        if (!(bool) (UnityEngine.Object) user)
          return;
        Material[] materialArray = user.SetOverrideShader(ShaderCache.Acquire("Brave/Internal/RainbowChestShader"));
        for (int index = 0; index < materialArray.Length; ++index)
        {
          if (!((UnityEngine.Object) materialArray[index] == (UnityEngine.Object) null))
            materialArray[index].SetFloat("_AllColorsToggle", 1f);
        }
      }

      protected override void Update()
      {
        base.Update();
        if ((bool) (UnityEngine.Object) this.Owner && (bool) (UnityEngine.Object) this.m_extantFloor)
        {
          Vector2 centerPosition = this.Owner.CenterPosition;
          this.m_extantFloor.renderer.sharedMaterial.SetVector("_PlayerPos", new Vector4(centerPosition.x, centerPosition.y, 0.0f, 0.0f));
        }
        if ((double) UnityEngine.Time.timeScale <= 0.0)
        {
          this.m_lastFrameAboutToFall = UnityEngine.Time.frameCount;
        }
        else
        {
          if (!this.m_wasAboutToFallLastFrame && (bool) (UnityEngine.Object) this.m_extantFloor)
          {
            SpawnManager.Despawn(this.m_extantFloor.gameObject);
            this.m_extantFloor = (tk2dSprite) null;
          }
          this.m_wasAboutToFallLastFrame = false;
        }
        this.ProcessRatStatus(this.Owner);
      }

      private void ProcessRatStatus(PlayerController player, bool forceDisable = false)
      {
        bool flag = (bool) (UnityEngine.Object) player && player.HasActiveBonusSynergy(CustomSynergyType.RESOURCEFUL_RAT) && !forceDisable;
        if (flag && !this.m_transformed)
        {
          this.m_lastPlayer = player;
          if (!(bool) (UnityEngine.Object) player)
            return;
          this.m_transformed = true;
          player.OverrideAnimationLibrary = this.RatAnimationLibrary;
          player.SetOverrideShader(ShaderCache.Acquire(player.LocalShaderName));
          if (player.characterIdentity == PlayableCharacters.Eevee)
            player.GetComponent<CharacterAnimationRandomizer>().AddOverrideAnimLibrary(this.RatAnimationLibrary);
          player.PlayerIsRatTransformed = true;
          player.stats.RecalculateStats(player);
        }
        else
        {
          if (!this.m_transformed || flag)
            return;
          if ((bool) (UnityEngine.Object) this.m_lastPlayer)
          {
            this.m_lastPlayer.OverrideAnimationLibrary = (tk2dSpriteAnimation) null;
            this.m_lastPlayer.ClearOverrideShader();
            if (this.m_lastPlayer.characterIdentity == PlayableCharacters.Eevee)
              this.m_lastPlayer.GetComponent<CharacterAnimationRandomizer>().RemoveOverrideAnimLibrary(this.RatAnimationLibrary);
            this.m_lastPlayer.PlayerIsRatTransformed = false;
            this.m_lastPlayer.stats.RecalculateStats(this.m_lastPlayer);
            this.m_lastPlayer = (PlayerController) null;
          }
          this.m_transformed = false;
        }
      }

      private void LateUpdate()
      {
        if (!(bool) (UnityEngine.Object) this.m_extantFloor)
          return;
        this.m_extantFloor.UpdateZDepth();
      }

      public override DebrisObject Drop(PlayerController player)
      {
        PlayerController playerController = player;
        playerController.OnAboutToFall = playerController.OnAboutToFall - new Func<bool, bool>(this.HandleAboutToFall);
        if (this.m_invulnerable)
          player.healthHaver.IsVulnerable = true;
        return base.Drop(player);
      }

      protected override void OnDestroy()
      {
        if ((bool) (UnityEngine.Object) this.Owner)
        {
          PlayerController owner = this.Owner;
          owner.OnAboutToFall = owner.OnAboutToFall - new Func<bool, bool>(this.HandleAboutToFall);
          if (this.m_invulnerable)
            this.Owner.healthHaver.IsVulnerable = true;
        }
        if (this.m_transformed)
          this.ProcessRatStatus((PlayerController) null, true);
        base.OnDestroy();
      }

      [DebuggerHidden]
      private IEnumerator HandleInvulnerability()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new RatBootsItem.\u003CHandleInvulnerability\u003Ec__Iterator0()
        {
          \u0024this = this
        };
      }

      private bool HandleAboutToFall(bool partialPit)
      {
        if ((bool) (UnityEngine.Object) this.Owner && this.Owner.IsFlying)
          return false;
        if (!partialPit && !this.m_invulnerable)
          this.StartCoroutine(this.HandleInvulnerability());
        this.m_frameWasPartialPit = partialPit;
        this.m_wasAboutToFallLastFrame = true;
        if (UnityEngine.Time.frameCount <= this.m_lastFrameAboutToFall)
          this.m_lastFrameAboutToFall = UnityEngine.Time.frameCount - 1;
        if (UnityEngine.Time.frameCount != this.m_lastFrameAboutToFall + 1)
          this.m_elapsedAboutToFall = 0.0f;
        if (partialPit)
          this.m_elapsedAboutToFall = 0.0f;
        this.m_lastFrameAboutToFall = UnityEngine.Time.frameCount;
        this.m_elapsedAboutToFall += BraveTime.DeltaTime;
        if ((double) this.m_elapsedAboutToFall < (double) this.HoverTime)
        {
          if (!(bool) (UnityEngine.Object) this.m_extantFloor)
          {
            GameObject gameObject = SpawnManager.SpawnVFX(this.FloorVFX);
            gameObject.transform.parent = this.Owner.transform;
            tk2dSprite component = gameObject.GetComponent<tk2dSprite>();
            component.PlaceAtPositionByAnchor(this.Owner.SpriteBottomCenter, tk2dBaseSprite.Anchor.MiddleCenter);
            component.IsPerpendicular = false;
            component.HeightOffGround = -2.25f;
            component.UpdateZDepth();
            this.m_extantFloor = component;
          }
          if ((double) this.m_elapsedAboutToFall > (double) this.HoverTime - (double) this.FlickerPortion)
            this.m_extantFloor.renderer.enabled = (double) Mathf.PingPong(this.m_elapsedAboutToFall - (this.HoverTime - this.FlickerPortion), this.FlickerFrequency * 2f) < (double) this.FlickerFrequency;
          else
            this.m_extantFloor.renderer.enabled = true;
          return false;
        }
        if ((bool) (UnityEngine.Object) this.m_extantFloor)
        {
          SpawnManager.Despawn(this.m_extantFloor.gameObject);
          this.m_extantFloor = (tk2dSprite) null;
        }
        return true;
      }
    }

}
