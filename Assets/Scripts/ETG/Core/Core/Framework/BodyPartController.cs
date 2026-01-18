// Decompiled with JetBrains decompiler
// Type: BodyPartController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

public class BodyPartController : BraveBehaviour
  {
    public AIActor specifyActor;
    public bool hasOutlines;
    public bool faceTarget;
    [ShowInInspectorIf("faceTarget", true)]
    public float faceTargetTurnSpeed = -1f;
    [ShowInInspectorIf("faceTarget", true)]
    public BodyPartController.AimFromType aimFrom = BodyPartController.AimFromType.Transform;
    public bool autoDepth = true;
    public bool redirectHealthHaver;
    public bool independentFlashOnDamage;
    public int myPixelCollider = -1;
    protected AIActor m_body;
    private float m_heightOffBody;
    private bool m_bodyFound;

    public bool OverrideFacingDirection { get; set; }

    public virtual void Awake()
    {
      if ((bool) (Object) this.specifyActor)
        this.m_body = this.specifyActor;
      if (!(bool) (Object) this.m_body)
        this.m_body = this.aiActor;
      if (!(bool) (Object) this.m_body && (bool) (Object) this.transform.parent)
        this.m_body = this.transform.parent.GetComponent<AIActor>();
      if (!(bool) (Object) this.m_body)
        return;
      if (this.independentFlashOnDamage)
        this.m_body.healthHaver.RegisterBodySprite(this.sprite, true, this.myPixelCollider);
      else
        this.m_body.healthHaver.RegisterBodySprite(this.sprite);
      this.m_bodyFound = true;
    }

    public virtual void Start()
    {
      this.m_heightOffBody = this.sprite.HeightOffGround;
      if (this.hasOutlines)
      {
        SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.black, this.sprite.HeightOffGround + 0.1f);
        if ((bool) (Object) this.m_body)
        {
          ObjectVisibilityManager component = this.m_body.GetComponent<ObjectVisibilityManager>();
          if ((bool) (Object) component)
            component.ResetRenderersList();
        }
      }
      if (!this.m_bodyFound && (bool) (Object) this.m_body)
      {
        this.m_body.healthHaver.RegisterBodySprite(this.sprite);
        this.m_bodyFound = true;
      }
      if (!(bool) (Object) this.specRigidbody)
        this.specRigidbody = this.m_body.specRigidbody;
      if (this.faceTarget & (bool) (Object) this.aiAnimator && (bool) (Object) this.m_body.aiAnimator)
      {
        this.aiAnimator.LockFacingDirection = true;
        this.aiAnimator.FacingDirection = this.m_body.aiAnimator.FacingDirection;
      }
      if (!(bool) (Object) this.specRigidbody || !this.redirectHealthHaver)
        return;
      this.specRigidbody.healthHaver = this.m_body.healthHaver;
    }

    public virtual void Update()
    {
      float angle;
      if (!this.OverrideFacingDirection && this.faceTarget && this.TryGetAimAngle(out angle))
      {
        if ((double) this.faceTargetTurnSpeed > 0.0)
          angle = Mathf.MoveTowardsAngle(!(bool) (Object) this.aiAnimator ? this.transform.eulerAngles.z : this.aiAnimator.FacingDirection, angle, this.faceTargetTurnSpeed * BraveTime.DeltaTime);
        if ((bool) (Object) this.aiAnimator)
        {
          this.aiAnimator.LockFacingDirection = true;
          this.aiAnimator.FacingDirection = angle;
        }
        else
          this.transform.rotation = Quaternion.Euler(0.0f, 0.0f, angle);
      }
      if (!this.autoDepth || !(bool) (Object) this.aiAnimator)
        return;
      float num1 = BraveMathCollege.ClampAngle180(this.m_body.aiAnimator.FacingDirection);
      float num2 = BraveMathCollege.ClampAngle180(this.aiAnimator.FacingDirection);
      this.sprite.HeightOffGround = (double) num1 > 155.0 || (double) num1 < 25.0 || (double) num2 > 155.0 || (double) num2 < 25.0 ? this.m_heightOffBody : -this.m_heightOffBody;
    }

    protected override void OnDestroy() => base.OnDestroy();

    protected virtual bool TryGetAimAngle(out float angle)
    {
      angle = 0.0f;
      if ((bool) (Object) this.m_body.TargetRigidbody)
      {
        Vector2 unitCenter = this.m_body.TargetRigidbody.GetUnitCenter(ColliderType.HitBox);
        Vector2 vector2 = this.transform.position.XY();
        if (this.aimFrom == BodyPartController.AimFromType.ActorHitBoxCenter)
          vector2 = this.m_body.specRigidbody.GetUnitCenter(ColliderType.HitBox);
        angle = (unitCenter - vector2).ToAngle();
        return true;
      }
      if (!(bool) (Object) this.m_body.aiAnimator)
        return false;
      angle = this.m_body.aiAnimator.FacingDirection;
      return true;
    }

    public enum AimFromType
    {
      Transform = 10, // 0x0000000A
      ActorHitBoxCenter = 20, // 0x00000014
    }
  }

