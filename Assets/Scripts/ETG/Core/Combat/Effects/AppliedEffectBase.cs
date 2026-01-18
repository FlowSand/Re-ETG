using UnityEngine;

#nullable disable

  public abstract class AppliedEffectBase : MonoBehaviour
  {
    public abstract void Initialize(AppliedEffectBase source);

    public abstract void AddSelfToTarget(GameObject target);
  }

