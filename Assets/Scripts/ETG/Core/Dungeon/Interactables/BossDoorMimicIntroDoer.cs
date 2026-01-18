using Dungeonator;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

[RequireComponent(typeof (GenericIntroDoer))]
public class BossDoorMimicIntroDoer : SpecificIntroDoer
  {
    private bool m_finished;
    private PlayerController m_enteringPlayer;
    private DungeonDoorController m_bossDoor;
    private Vector2 m_bossStartingPosition;
    private float m_cachedHeightOffGround;

    public DungeonDoorSubsidiaryBlocker PhantomDoorBlocker { get; set; }

    protected override void OnDestroy()
    {
      if (GameManager.HasInstance)
      {
        if ((bool) (Object) this.PhantomDoorBlocker)
          this.PhantomDoorBlocker.Unseal();
        if ((bool) (Object) this.m_bossDoor)
        {
          foreach (Renderer componentsInChild in this.m_bossDoor.sealAnimators[0].sprite.GetComponentsInChildren<Renderer>())
            componentsInChild.enabled = true;
        }
      }
      base.OnDestroy();
    }

    public override Vector2? OverrideIntroPosition
    {
      get => new Vector2?((Vector2) this.m_bossDoor.transform.position);
    }

    public override void PlayerWalkedIn(PlayerController player, List<tk2dSpriteAnimator> animators)
    {
      if ((Object) this.m_bossDoor != (Object) null)
        return;
      this.m_bossDoor = (DungeonDoorController) null;
      float num1 = float.MaxValue;
      foreach (DungeonDoorController dungeonDoorController in Object.FindObjectsOfType<DungeonDoorController>())
      {
        if (dungeonDoorController.name.StartsWith("GungeonBossDoor"))
        {
          SpeculativeRigidbody componentInChildren = dungeonDoorController.GetComponentInChildren<SpeculativeRigidbody>();
          float num2 = Vector2.Distance(player.specRigidbody.UnitCenter, componentInChildren.UnitCenter);
          if ((Object) this.m_bossDoor == (Object) null || (double) num2 < (double) num1)
          {
            this.m_bossDoor = dungeonDoorController;
            num1 = num2;
          }
        }
      }
      foreach (tk2dSpriteAnimator componentsInChild in this.m_bossDoor.GetComponentsInChildren<tk2dSpriteAnimator>())
      {
        if (componentsInChild.name == "Eye Fire")
          animators.Add(componentsInChild);
      }
      this.m_enteringPlayer = player;
      this.m_bossStartingPosition = (Vector2) this.transform.position;
      this.m_cachedHeightOffGround = this.sprite.HeightOffGround;
    }

    public override void StartIntro(List<tk2dSpriteAnimator> animators)
    {
      foreach (tk2dSpriteAnimator componentsInChild in this.GetComponentsInChildren<tk2dSpriteAnimator>(true))
      {
        if ((Object) componentsInChild != (Object) this.spriteAnimator)
          animators.Add(componentsInChild);
      }
      this.StartCoroutine(this.DoIntro());
    }

    public override bool IsIntroFinished => this.m_finished;

    public override void EndIntro()
    {
      this.StopAllCoroutines();
      foreach (Renderer componentsInChild in this.m_bossDoor.sealAnimators[0].sprite.GetComponentsInChildren<Renderer>())
        componentsInChild.enabled = false;
      this.transform.position = (Vector3) this.m_bossStartingPosition;
      this.specRigidbody.Reinitialize();
      tk2dBaseSprite component = this.aiActor.ShadowObject.GetComponent<tk2dBaseSprite>();
      component.color = component.color.WithAlpha(1f);
      this.aiAnimator.LockFacingDirection = false;
      this.aiAnimator.FacingDirection = 90f;
      this.aiAnimator.EndAnimation();
      this.sprite.HeightOffGround = this.m_cachedHeightOffGround;
      this.sprite.UpdateZDepth();
      this.SpawnDoorBlocker();
    }

    [DebuggerHidden]
    private IEnumerator DoIntro()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new BossDoorMimicIntroDoer__DoIntroc__Iterator0()
      {
        _this = this
      };
    }

    private void SpawnDoorBlocker()
    {
      if ((Object) GameManager.Instance.Dungeon.phantomBlockerDoorObjects == (Object) null || (bool) (Object) this.PhantomDoorBlocker)
        return;
      this.PhantomDoorBlocker = GameManager.Instance.Dungeon.phantomBlockerDoorObjects.InstantiateObjectDirectional(this.aiActor.ParentRoom, new IntVector2(22, -5), DungeonData.Direction.NORTH).GetComponent<DungeonDoorSubsidiaryBlocker>();
      this.PhantomDoorBlocker.Seal();
    }
  }

