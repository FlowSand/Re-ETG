// Decompiled with JetBrains decompiler
// Type: BlinkPassiveItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Items.Passive
{
    public class BlinkPassiveItem : PassiveItem
    {
      public bool ModifiesDodgeRoll;
      [ShowInInspectorIf("ModifiesDodgeRoll", false)]
      public float DodgeRollTimeMultiplier = 0.9f;
      [ShowInInspectorIf("ModifiesDodgeRoll", false)]
      public float DodgeRollDistanceMultiplier = 1.25f;
      [ShowInInspectorIf("ModifiesDodgeRoll", false)]
      public int AdditionalInvulnerabilityFrames;
      public ScarfAttachmentDoer ScarfPrefab;
      public GameObject BlinkpoofVfx;
      private ScarfAttachmentDoer m_scarf;
      private AfterImageTrailController afterimage;

      public override void Pickup(PlayerController player)
      {
        if (this.m_pickedUp)
          return;
        if (player.IsDodgeRolling)
          player.ForceStopDodgeRoll();
        if ((bool) (UnityEngine.Object) this.ScarfPrefab)
        {
          this.m_scarf = UnityEngine.Object.Instantiate<GameObject>(this.ScarfPrefab.gameObject).GetComponent<ScarfAttachmentDoer>();
          this.m_scarf.Initialize((GameActor) player);
        }
        if (this.ModifiesDodgeRoll)
        {
          player.rollStats.rollDistanceMultiplier *= this.DodgeRollDistanceMultiplier;
          player.rollStats.rollTimeMultiplier *= this.DodgeRollTimeMultiplier;
          player.rollStats.additionalInvulnerabilityFrames += this.AdditionalInvulnerabilityFrames;
        }
        if (!PassiveItem.ActiveFlagItems.ContainsKey(player))
          PassiveItem.ActiveFlagItems.Add(player, new Dictionary<System.Type, int>());
        if (!PassiveItem.ActiveFlagItems[player].ContainsKey(this.GetType()))
          PassiveItem.ActiveFlagItems[player].Add(this.GetType(), 1);
        else
          PassiveItem.ActiveFlagItems[player][this.GetType()] = PassiveItem.ActiveFlagItems[player][this.GetType()] + 1;
        this.afterimage = player.gameObject.AddComponent<AfterImageTrailController>();
        this.afterimage.spawnShadows = false;
        this.afterimage.shadowTimeDelay = 0.05f;
        this.afterimage.shadowLifetime = 0.3f;
        this.afterimage.minTranslation = 0.05f;
        this.afterimage.dashColor = Color.black;
        this.afterimage.maxEmission = 0.0f;
        this.afterimage.minEmission = 0.0f;
        this.afterimage.OverrideImageShader = ShaderCache.Acquire("Brave/Internal/DownwellAfterImage");
        player.OnRollStarted += new Action<PlayerController, Vector2>(this.OnRollStarted);
        player.OnBlinkShadowCreated += new Action<tk2dSprite>(this.OnBlinkCloneCreated);
        base.Pickup(player);
      }

      public void OnBlinkCloneCreated(tk2dSprite cloneSprite)
      {
        SpawnManager.SpawnVFX(this.BlinkpoofVfx, (Vector3) cloneSprite.WorldCenter, Quaternion.identity);
      }

      private void OnRollStarted(PlayerController obj, Vector2 dirVec)
      {
        if ((bool) (UnityEngine.Object) GameManager.Instance.Dungeon && GameManager.Instance.Dungeon.IsEndTimes)
          return;
        obj.StartCoroutine(this.HandleAfterImageStop(obj));
      }

      [DebuggerHidden]
      private IEnumerator HandleAfterImageStop(PlayerController player)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BlinkPassiveItem.<HandleAfterImageStop>c__Iterator0()
        {
          player = player,
          _this = this
        };
      }

      public override DebrisObject Drop(PlayerController player)
      {
        DebrisObject debrisObject = base.Drop(player);
        if (this.ModifiesDodgeRoll)
        {
          player.rollStats.rollDistanceMultiplier /= this.DodgeRollDistanceMultiplier;
          player.rollStats.rollTimeMultiplier /= this.DodgeRollTimeMultiplier;
          player.rollStats.additionalInvulnerabilityFrames -= this.AdditionalInvulnerabilityFrames;
          player.rollStats.additionalInvulnerabilityFrames = Mathf.Max(player.rollStats.additionalInvulnerabilityFrames, 0);
        }
        if (PassiveItem.ActiveFlagItems[player].ContainsKey(this.GetType()))
        {
          PassiveItem.ActiveFlagItems[player][this.GetType()] = Mathf.Max(0, PassiveItem.ActiveFlagItems[player][this.GetType()] - 1);
          if (PassiveItem.ActiveFlagItems[player][this.GetType()] == 0)
            PassiveItem.ActiveFlagItems[player].Remove(this.GetType());
        }
        if ((bool) (UnityEngine.Object) this.m_scarf)
        {
          UnityEngine.Object.Destroy((UnityEngine.Object) this.m_scarf.gameObject);
          this.m_scarf = (ScarfAttachmentDoer) null;
        }
        player.OnRollStarted -= new Action<PlayerController, Vector2>(this.OnRollStarted);
        player.OnBlinkShadowCreated -= new Action<tk2dSprite>(this.OnBlinkCloneCreated);
        if ((bool) (UnityEngine.Object) this.afterimage)
          UnityEngine.Object.Destroy((UnityEngine.Object) this.afterimage);
        this.afterimage = (AfterImageTrailController) null;
        debrisObject.GetComponent<BlinkPassiveItem>().m_pickedUpThisRun = true;
        return debrisObject;
      }

      protected override void OnDestroy()
      {
        if ((bool) (UnityEngine.Object) this.m_scarf)
        {
          UnityEngine.Object.Destroy((UnityEngine.Object) this.m_scarf.gameObject);
          this.m_scarf = (ScarfAttachmentDoer) null;
        }
        if (this.m_pickedUp && (bool) (UnityEngine.Object) this.m_owner && PassiveItem.ActiveFlagItems != null && PassiveItem.ActiveFlagItems.ContainsKey(this.m_owner) && PassiveItem.ActiveFlagItems[this.m_owner].ContainsKey(this.GetType()))
        {
          PassiveItem.ActiveFlagItems[this.m_owner][this.GetType()] = Mathf.Max(0, PassiveItem.ActiveFlagItems[this.m_owner][this.GetType()] - 1);
          if (PassiveItem.ActiveFlagItems[this.m_owner][this.GetType()] == 0)
            PassiveItem.ActiveFlagItems[this.m_owner].Remove(this.GetType());
        }
        if ((UnityEngine.Object) this.m_owner != (UnityEngine.Object) null)
        {
          this.m_owner.OnRollStarted -= new Action<PlayerController, Vector2>(this.OnRollStarted);
          this.m_owner.OnBlinkShadowCreated -= new Action<tk2dSprite>(this.OnBlinkCloneCreated);
          if ((bool) (UnityEngine.Object) this.afterimage)
            UnityEngine.Object.Destroy((UnityEngine.Object) this.afterimage);
          this.afterimage = (AfterImageTrailController) null;
        }
        base.OnDestroy();
      }
    }

}
