using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

public class BulletLimbController : BraveBehaviour
  {
    public string LimitPrefix;
    public bool OverrideHeightOffGround;
    [ShowInInspectorIf("OverrideHeightOffGround", true)]
    public float HeightOffGround;
    public string OverrideLimbName;
    public string OverrideFinalLimbName;
    public bool CollideWithOthers = true;
    public bool DisableTileMapCollisions;
    public bool RotateToMatchTransforms;
    public bool WarpBullets;
    private bool m_doingTell;
    private AIActor m_body;
    private List<Transform> m_transforms = new List<Transform>();
    private List<Projectile> m_projectiles = new List<Projectile>();

    public bool HideBullets { get; set; }

    public void Start()
    {
      this.m_body = this.transform.parent.GetComponent<AIActor>();
      if ((UnityEngine.Object) this.m_body == (UnityEngine.Object) null)
        this.m_body = this.aiAnimator.SpecifyAiActor;
      Transform[] componentsInChildren = this.GetComponentsInChildren<Transform>(true);
      for (int index = 0; index < componentsInChildren.Length; ++index)
      {
        if (!((UnityEngine.Object) componentsInChildren[index] == (UnityEngine.Object) this.transform) && (string.IsNullOrEmpty(this.LimitPrefix) || componentsInChildren[index].name.StartsWith(this.LimitPrefix)))
          this.m_transforms.Add(componentsInChildren[index]);
      }
      for (int index = 0; index < this.m_transforms.Count; ++index)
        this.m_projectiles.Add(this.CreateProjectile(this.m_transforms[index]));
      this.m_body.bulletBank.transforms.AddRange((IEnumerable<Transform>) this.m_transforms);
      this.m_body.specRigidbody.CanCarry = true;
      this.m_body.specRigidbody.OnPostRigidbodyMovement += new Action<SpeculativeRigidbody, Vector2, IntVector2>(this.PostBodyMovement);
    }

    public void Update() => this.renderer.enabled = false;

    public void LateUpdate()
    {
      if ((double) BraveTime.DeltaTime != 0.0)
        return;
      this.PostBodyMovement(this.specRigidbody, Vector2.zero, IntVector2.Zero);
    }

    public void PostBodyMovement(
      SpeculativeRigidbody specRigidbody,
      Vector2 unitDelta,
      IntVector2 pixelDelta)
    {
      if (!(bool) (UnityEngine.Object) this.m_body)
        return;
      for (int index = 0; index < this.m_transforms.Count; ++index)
      {
        GameObject gameObject = this.m_transforms[index].gameObject;
        Projectile projectile = this.m_projectiles[index];
        if ((bool) (UnityEngine.Object) projectile && (bool) (UnityEngine.Object) this.m_body && this.m_body.IsBlackPhantom != projectile.IsBlackBullet)
        {
          if (this.m_body.IsBlackPhantom)
          {
            projectile.ForceBlackBullet = true;
            projectile.BecomeBlackBullet();
          }
          else
          {
            projectile.ForceBlackBullet = false;
            projectile.ReturnFromBlackBullet();
          }
        }
        if (gameObject.activeSelf && !this.HideBullets)
        {
          if (!(bool) (UnityEngine.Object) projectile)
          {
            this.m_projectiles[index] = this.CreateProjectile(gameObject.transform);
          }
          else
          {
            if (!projectile.gameObject.activeSelf)
            {
              projectile.gameObject.SetActive(true);
              projectile.specRigidbody.enabled = true;
              projectile.transform.position = gameObject.transform.position;
              projectile.specRigidbody.Reinitialize();
            }
            if ((double) BraveTime.DeltaTime > 0.0)
            {
              if (this.WarpBullets)
              {
                projectile.specRigidbody.Velocity = Vector2.zero;
                projectile.specRigidbody.transform.position = gameObject.transform.position;
                projectile.specRigidbody.Reinitialize();
                projectile.sprite.UpdateZDepth();
              }
              else
                projectile.specRigidbody.Velocity = (gameObject.transform.position.XY() - projectile.specRigidbody.Position.UnitPosition) / BraveTime.DeltaTime;
            }
            else
            {
              projectile.specRigidbody.Velocity = Vector2.zero;
              projectile.transform.position = gameObject.transform.position;
              projectile.transform.position = projectile.transform.position.WithZ(projectile.transform.position.y);
              projectile.specRigidbody.sprite.UpdateZDepth();
            }
            if (this.RotateToMatchTransforms)
              projectile.transform.rotation = gameObject.transform.rotation;
          }
        }
        else if ((bool) (UnityEngine.Object) projectile && projectile.gameObject.activeSelf)
        {
          projectile.gameObject.SetActive(false);
          projectile.specRigidbody.enabled = false;
          projectile.specRigidbody.Velocity = Vector2.zero;
        }
      }
    }

    protected override void OnDestroy()
    {
      this.DestroyProjectiles();
      base.OnDestroy();
    }

    public void DestroyProjectiles()
    {
      for (int index = 0; index < this.m_projectiles.Count; ++index)
      {
        Projectile projectile = this.m_projectiles[index];
        if ((bool) (UnityEngine.Object) projectile)
        {
          if (projectile.gameObject.activeSelf)
          {
            projectile.gameObject.SetActive(false);
            projectile.specRigidbody.enabled = false;
            projectile.specRigidbody.Velocity = Vector2.zero;
          }
          projectile.DieInAir(!projectile.gameObject.activeSelf);
        }
      }
    }

    public bool DoingTell
    {
      set
      {
        this.m_doingTell = value;
        for (int index = 0; index < this.m_projectiles.Count; ++index)
        {
          Projectile projectile = this.m_projectiles[index];
          if ((bool) (UnityEngine.Object) projectile)
          {
            if (this.m_doingTell)
              projectile.spriteAnimator.Play();
            else
              projectile.spriteAnimator.StopAndResetFrameToDefault();
          }
        }
      }
    }

    private Projectile CreateProjectile(Transform transform)
    {
      if (BossKillCam.BossDeathCamRunning)
        return (Projectile) null;
      string bulletName = "limb";
      if (!string.IsNullOrEmpty(this.OverrideFinalLimbName) && this.IsFinalLimb(transform))
        bulletName = this.OverrideFinalLimbName;
      else if (!string.IsNullOrEmpty(this.OverrideLimbName))
        bulletName = this.OverrideLimbName;
      Projectile component = this.m_body.bulletBank.CreateProjectileFromBank((Vector2) transform.position, 0.0f, bulletName).GetComponent<Projectile>();
      component.ManualControl = true;
      component.SkipDistanceElapsedCheck = true;
      component.BulletScriptSettings.surviveRigidbodyCollisions = true;
      component.BulletScriptSettings.surviveTileCollisions = true;
      component.gameObject.SetActive(false);
      component.specRigidbody.enabled = false;
      component.specRigidbody.Velocity = Vector2.zero;
      component.specRigidbody.CanBeCarried = true;
      component.specRigidbody.CollideWithOthers = this.CollideWithOthers;
      if (this.DisableTileMapCollisions)
        component.specRigidbody.CollideWithTileMap = false;
      if ((bool) (UnityEngine.Object) this.m_body && this.m_body.IsBlackPhantom)
        component.ForceBlackBullet = true;
      if (this.OverrideHeightOffGround)
        component.sprite.HeightOffGround = this.HeightOffGround;
      return component;
    }

    private bool IsFinalLimb(Transform transform)
    {
      return (UnityEngine.Object) transform == (UnityEngine.Object) this.m_transforms[this.m_transforms.Count - 1];
    }
  }

