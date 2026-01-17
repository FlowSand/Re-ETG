// Decompiled with JetBrains decompiler
// Type: AdvancedDraGunIntroDoer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Dungeon.Interactables
{
    [RequireComponent(typeof (GenericIntroDoer))]
    public class AdvancedDraGunIntroDoer : SpecificIntroDoer
    {
      private bool m_isFinished;
      private tk2dSpriteAnimator m_introDummy;
      private tk2dSpriteAnimator m_introBabyDummy;
      private tk2dSpriteAnimator m_introVfxDummy;
      private GameObject m_neck;
      private GameObject m_wings;
      private GameObject m_leftArm;
      private GameObject m_rightArm;

      protected override void OnDestroy() => base.OnDestroy();

      public override IntVector2 OverrideExitBasePosition(
        DungeonData.Direction directionToWalk,
        IntVector2 exitBaseCenter)
      {
        return exitBaseCenter + new IntVector2(0, DraGunRoomPlaceable.HallHeight);
      }

      public override Vector2? OverrideIntroPosition
      {
        get => new Vector2?(this.specRigidbody.UnitCenter + new Vector2(0.0f, 4f));
      }

      public override void PlayerWalkedIn(PlayerController player, List<tk2dSpriteAnimator> animators)
      {
        this.m_introDummy = this.transform.Find("IntroDummy").GetComponent<tk2dSpriteAnimator>();
        this.m_introBabyDummy = this.transform.Find("IntroDummy/baby").GetComponent<tk2dSpriteAnimator>();
        this.m_introVfxDummy = this.transform.Find("IntroDummy/vfx").GetComponent<tk2dSpriteAnimator>();
        this.m_introDummy.aiAnimator = this.aiAnimator;
        this.m_introBabyDummy.aiAnimator = this.aiAnimator;
        this.m_introVfxDummy.aiAnimator = this.aiAnimator;
        this.m_introVfxDummy.sprite.usesOverrideMaterial = false;
        this.m_neck = this.transform.Find("Neck").gameObject;
        this.m_wings = this.transform.Find("Wings").gameObject;
        this.m_leftArm = this.transform.Find("LeftArm").gameObject;
        this.m_rightArm = this.transform.Find("RightArm").gameObject;
        this.m_introDummy.gameObject.SetActive(true);
        this.m_introBabyDummy.gameObject.SetActive(true);
        this.m_introVfxDummy.gameObject.SetActive(true);
        this.renderer.enabled = false;
        this.m_neck.SetActive(false);
        this.m_wings.SetActive(false);
        this.m_leftArm.SetActive(false);
        this.m_rightArm.SetActive(false);
        this.StartCoroutine(this.RunEmbers());
      }

      [DebuggerHidden]
      private IEnumerator RunEmbers()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new AdvancedDraGunIntroDoer.<RunEmbers>c__Iterator0()
        {
          _this = this
        };
      }

      public override void StartIntro(List<tk2dSpriteAnimator> animators)
      {
        animators.Add(this.m_introDummy);
        animators.Add(this.m_introBabyDummy);
        animators.Add(this.m_introVfxDummy);
        this.GetComponent<DragunCracktonMap>().ConvertToGold();
        this.StartCoroutine(this.DoIntro());
      }

      [DebuggerHidden]
      public IEnumerator DoIntro()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new AdvancedDraGunIntroDoer.<DoIntro>c__Iterator1()
        {
          _this = this
        };
      }

      public override bool IsIntroFinished => this.m_isFinished;

      public override void EndIntro()
      {
        this.m_introDummy.gameObject.SetActive(false);
        this.m_introBabyDummy.gameObject.SetActive(false);
        this.m_introVfxDummy.gameObject.SetActive(false);
        this.renderer.enabled = true;
        this.m_neck.SetActive(true);
        this.m_wings.SetActive(true);
        this.m_leftArm.SetActive(true);
        this.m_rightArm.SetActive(true);
        this.aiAnimator.EndAnimation();
        DraGunController component = this.GetComponent<DraGunController>();
        component.ModifyCamera(true);
        component.BlockPitTiles(true);
        component.HasDoneIntro = true;
        this.m_isFinished = true;
      }
    }

}
