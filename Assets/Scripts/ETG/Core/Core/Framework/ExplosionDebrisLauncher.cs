using System;
using UnityEngine;

#nullable disable

public class ExplosionDebrisLauncher : BraveBehaviour
  {
    public int minShards = 4;
    public int maxShards = 4;
    public float minExpulsionForce = 15f;
    public float maxExpulsionForce = 15f;
    public bool specifyArcDegrees;
    [ShowInInspectorIf("specifyArcDegrees", true)]
    public float arcDegrees;
    public float angleVariance = 20f;
    public DebrisObject[] debrisSources;
    public bool UsesCustomAxialVelocity;
    [ShowInInspectorIf("UsesCustomAxialVelocity", false)]
    public Vector3 CustomAxialVelocity = Vector3.zero;
    public AIActor SpecifyActor;
    public bool LaunchOnActorPreDeath;
    public bool LaunchOnActorDeath;
    public bool LaunchOnAnimationEvent;
    [ShowInInspectorIf("LaunchOnAnimationEvent", true)]
    public tk2dSpriteAnimator SpecifyAnimator;
    [ShowInInspectorIf("LaunchOnAnimationEvent", true)]
    public string EventName;
    [ShowInInspectorIf("LaunchOnAnimationEvent", true)]
    public bool UseDeathDir = true;
    private Vector2 m_deathDir;

    private void Start()
    {
      if (this.LaunchOnActorDeath)
      {
        if (!(bool) (UnityEngine.Object) this.SpecifyActor)
          this.SpecifyActor = this.aiActor;
        this.SpecifyActor.healthHaver.OnDeath += new Action<Vector2>(this.OnDeath);
      }
      if (this.LaunchOnActorPreDeath || this.LaunchOnAnimationEvent)
      {
        if (!(bool) (UnityEngine.Object) this.SpecifyActor)
          this.SpecifyActor = this.aiActor;
        this.SpecifyActor.healthHaver.OnPreDeath += new Action<Vector2>(this.OnPreDeath);
      }
      if (!this.LaunchOnAnimationEvent)
        return;
      if (!(bool) (UnityEngine.Object) this.SpecifyAnimator)
        this.SpecifyAnimator = this.spriteAnimator;
      this.SpecifyAnimator.AnimationEventTriggered += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.AnimationEventTriggered);
    }

    protected override void OnDestroy() => base.OnDestroy();

    private void OnPreDeath(Vector2 deathDir)
    {
      this.m_deathDir = deathDir;
      if (!this.LaunchOnActorPreDeath)
        return;
      this.Launch(deathDir);
    }

    private void OnDeath(Vector2 deathDir) => this.Launch(deathDir);

    private void AnimationEventTriggered(
      tk2dSpriteAnimator spriteAnimator,
      tk2dSpriteAnimationClip clip,
      int frame)
    {
      if (!(clip.GetFrame(frame).eventInfo == this.EventName))
        return;
      Vector2 surfaceNormal = Vector2.zero;
      if (!this.UseDeathDir)
        surfaceNormal = this.transform.position.XY() - this.SpecifyAnimator.aiActor.CenterPosition;
      if (this.UseDeathDir || surfaceNormal == Vector2.zero)
        surfaceNormal = this.m_deathDir;
      this.Launch(surfaceNormal);
    }

    public void Launch()
    {
      int num1 = UnityEngine.Random.Range(this.minShards, this.maxShards + 1);
      float num2 = UnityEngine.Random.Range(0.0f, 360f);
      float num3 = 360f / (float) num1;
      for (int index = 0; index < num1; ++index)
      {
        DebrisObject component = SpawnManager.SpawnDebris(this.debrisSources[UnityEngine.Random.Range(0, this.debrisSources.Length)].gameObject, this.transform.position, Quaternion.identity).GetComponent<DebrisObject>();
        Vector3 vector3 = (Quaternion.Euler(0.0f, 0.0f, num2 + num3 * (float) index + UnityEngine.Random.Range(-this.angleVariance, this.angleVariance)) * Vector3.right * UnityEngine.Random.Range(this.minExpulsionForce, this.maxExpulsionForce)).WithZ(2f);
        if (this.UsesCustomAxialVelocity)
          vector3 = Vector3.Scale(vector3, this.CustomAxialVelocity);
        component.Trigger(vector3, 1f);
        component.additionalHeightBoost = -3f;
      }
    }

    public void Launch(Vector2 surfaceNormal)
    {
      int num1 = UnityEngine.Random.Range(this.minShards, this.maxShards + 1);
      if (num1 == 0)
        return;
      float angle = surfaceNormal.ToAngle();
      float num2 = 0.0f;
      if (this.specifyArcDegrees)
      {
        angle -= this.arcDegrees / 2f;
        num2 = this.arcDegrees / (float) (num1 - 1);
      }
      else if (num1 == 2)
      {
        angle -= 45f;
        num2 = 90f;
      }
      else if (num1 > 2)
      {
        angle -= 90f;
        num2 = 180f / (float) (num1 - 1);
      }
      for (int index = 0; index < num1; ++index)
      {
        DebrisObject component = SpawnManager.SpawnDebris(this.debrisSources[UnityEngine.Random.Range(0, this.debrisSources.Length)].gameObject, this.transform.position, Quaternion.identity).GetComponent<DebrisObject>();
        Vector3 vector3 = (Quaternion.Euler(0.0f, 0.0f, Mathf.Clamp(angle + num2 * (float) index + UnityEngine.Random.Range(-this.angleVariance, this.angleVariance), angle, angle + 180f)) * Vector3.right * UnityEngine.Random.Range(this.minExpulsionForce, this.maxExpulsionForce)).WithZ(UnityEngine.Random.Range(1.5f, 3f));
        if (this.UsesCustomAxialVelocity)
          vector3 = Vector3.Scale(vector3, this.CustomAxialVelocity);
        component.Trigger(vector3, UnityEngine.Random.Range(1f, 2f));
      }
    }
  }

