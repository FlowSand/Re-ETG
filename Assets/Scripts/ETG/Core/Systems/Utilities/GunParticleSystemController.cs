using System;
using UnityEngine;

#nullable disable

public class GunParticleSystemController : MonoBehaviour
  {
    public ParticleSystem TargetSystem;
    public bool DoesParticlesOnFire;
    public int MinParticlesOnFire = 10;
    public int MaxParticlesOnFire = 10;
    public bool DoesParticlesOnReload;
    public int MinParticlesOnReload = 20;
    public int MaxParticlesOnReload = 20;
    private Gun m_gun;
    private Vector3 m_localPositionOffset;

    private void Awake()
    {
      this.m_gun = this.GetComponent<Gun>();
      if (!(bool) (UnityEngine.Object) this.TargetSystem)
        return;
      this.m_localPositionOffset = this.TargetSystem.transform.localPosition;
    }

    private void Start()
    {
      this.m_gun = this.GetComponent<Gun>();
      if (this.DoesParticlesOnFire)
        this.m_gun.OnPostFired += new Action<PlayerController, Gun>(this.HandlePostFired);
      if (!this.DoesParticlesOnReload)
        return;
      this.m_gun.OnReloadPressed += new Action<PlayerController, Gun, bool>(this.HandleReload);
    }

    private void LateUpdate()
    {
      if (!(bool) (UnityEngine.Object) this.TargetSystem)
        return;
      if (this.m_gun.GetSprite().FlipY)
        this.TargetSystem.transform.localPosition = this.m_localPositionOffset.WithY(this.m_localPositionOffset.y * -1f);
      else
        this.TargetSystem.transform.localPosition = this.m_localPositionOffset;
    }

    private void HandleReload(PlayerController arg1, Gun arg2, bool arg3)
    {
      if (GameManager.Options.ShaderQuality != GameOptions.GenericHighMedLowOption.HIGH && GameManager.Options.ShaderQuality != GameOptions.GenericHighMedLowOption.MEDIUM)
        return;
      this.TargetSystem.Emit(UnityEngine.Random.Range(this.MinParticlesOnReload, this.MaxParticlesOnReload + 1));
    }

    private void HandlePostFired(PlayerController arg1, Gun arg2)
    {
      if (GameManager.Options.ShaderQuality != GameOptions.GenericHighMedLowOption.HIGH && GameManager.Options.ShaderQuality != GameOptions.GenericHighMedLowOption.MEDIUM)
        return;
      this.TargetSystem.Emit(UnityEngine.Random.Range(this.MinParticlesOnFire, this.MaxParticlesOnFire + 1));
    }

    private void OnEnable()
    {
      if (this.DoesParticlesOnFire)
        this.m_gun.OnPostFired += new Action<PlayerController, Gun>(this.HandlePostFired);
      if (!this.DoesParticlesOnReload)
        return;
      this.m_gun.OnReloadPressed += new Action<PlayerController, Gun, bool>(this.HandleReload);
    }

    private void OnDisable()
    {
      this.m_gun.OnPostFired -= new Action<PlayerController, Gun>(this.HandlePostFired);
      this.m_gun.OnReloadPressed -= new Action<PlayerController, Gun, bool>(this.HandleReload);
    }

    private void OnDestroy()
    {
      this.m_gun.OnPostFired -= new Action<PlayerController, Gun>(this.HandlePostFired);
      this.m_gun.OnReloadPressed -= new Action<PlayerController, Gun, bool>(this.HandleReload);
    }
  }

