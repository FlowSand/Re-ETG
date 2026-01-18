using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class TetrisBuff : AppliedEffectBase
  {
    public TetrisBuff.TetrisType type;
    public ExplosionData tetrisExplosion;
    public float ExplosionDamagePerTetromino = 6f;
    [Tooltip("How long each application lasts.")]
    public float lifetime;
    [Tooltip("The maximum length of time this debuff can be extended to by repeat applications.")]
    public float maxLifetime;
    public GameObject vfx;
    [NonSerialized]
    public bool shouldBurst;
    private float elapsed;
    private GameObject instantiatedVFX;
    private HealthHaver hh;
    private bool wasDuplicate;

    private void InitializeSelf(float length, float maxLength)
    {
      this.hh = this.GetComponent<HealthHaver>();
      this.lifetime = length;
      this.maxLifetime = maxLength;
      if ((UnityEngine.Object) this.hh != (UnityEngine.Object) null)
        this.StartCoroutine(this.ApplyModification());
      else
        UnityEngine.Object.Destroy((UnityEngine.Object) this);
    }

    public override void Initialize(AppliedEffectBase source)
    {
      if (source is TetrisBuff)
      {
        TetrisBuff tetrisBuff = source as TetrisBuff;
        this.InitializeSelf(tetrisBuff.lifetime, tetrisBuff.maxLifetime);
        this.type = tetrisBuff.type;
        if (!((UnityEngine.Object) tetrisBuff.vfx != (UnityEngine.Object) null))
          return;
        this.instantiatedVFX = SpawnManager.SpawnVFX(tetrisBuff.vfx, this.transform.position, Quaternion.identity);
        tk2dSprite component1 = this.instantiatedVFX.GetComponent<tk2dSprite>();
        tk2dSprite component2 = this.GetComponent<tk2dSprite>();
        if ((UnityEngine.Object) component1 != (UnityEngine.Object) null && (UnityEngine.Object) component2 != (UnityEngine.Object) null)
        {
          component2.AttachRenderer((tk2dBaseSprite) component1);
          component1.HeightOffGround = 0.1f;
          component1.IsPerpendicular = true;
          component1.usesOverrideMaterial = true;
        }
        BuffVFXAnimator component3 = this.instantiatedVFX.GetComponent<BuffVFXAnimator>();
        if (!((UnityEngine.Object) component3 != (UnityEngine.Object) null))
          return;
        component3.ClearData();
        component3.Initialize(this.GetComponent<GameActor>());
      }
      else
        UnityEngine.Object.Destroy((UnityEngine.Object) this);
    }

    public void ExtendLength(float time)
    {
      this.lifetime = Mathf.Min(this.lifetime + time, this.elapsed + this.maxLifetime);
    }

    public override void AddSelfToTarget(GameObject target)
    {
      if ((UnityEngine.Object) target.GetComponent<HealthHaver>() == (UnityEngine.Object) null)
        return;
      bool flag1 = this.type == TetrisBuff.TetrisType.LINE;
      bool flag2 = false;
      TetrisBuff[] components = target.GetComponents<TetrisBuff>();
      for (int index = 0; index < components.Length; ++index)
      {
        components[index].shouldBurst = components[index].shouldBurst || flag1;
        if (components[index].type == this.type)
        {
          if (false)
          {
            components[index].ExtendLength(this.lifetime);
            return;
          }
          flag2 = true;
        }
      }
      TetrisBuff tetrisBuff = target.AddComponent<TetrisBuff>();
      tetrisBuff.shouldBurst = flag1;
      tetrisBuff.tetrisExplosion = this.tetrisExplosion;
      tetrisBuff.Initialize((AppliedEffectBase) this);
    }

    [DebuggerHidden]
    private IEnumerator ApplyModification()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new TetrisBuff__ApplyModificationc__Iterator0()
      {
        _this = this
      };
    }

    public enum TetrisType
    {
      BLOCK,
      L,
      L_REVERSED,
      S,
      S_REVERSED,
      T,
      LINE,
    }
  }

