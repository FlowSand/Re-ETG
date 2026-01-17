// Decompiled with JetBrains decompiler
// Type: OnActiveItemUsedSynergyProcessor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class OnActiveItemUsedSynergyProcessor : MonoBehaviour
{
  [LongNumericEnum]
  public CustomSynergyType SynergyToCheck;
  public bool FiresOnActivation;
  [ShowInInspectorIf("FiresOnActivation", false)]
  public RadialBurstInterface ActivationBurst;
  [ShowInInspectorIf("FiresOnActivation", false)]
  public float ActivationBurstCooldown;
  public bool CreatesHoveringGun;
  [ShowInInspectorIf("CreatesHoveringGun", false)]
  public HoveringGunController.HoverPosition PositionType;
  [ShowInInspectorIf("CreatesHoveringGun", false)]
  public HoveringGunController.AimType AimType;
  [ShowInInspectorIf("CreatesHoveringGun", false)]
  public HoveringGunController.FireType FireType;
  [ShowInInspectorIf("CreatesHoveringGun", false)]
  public float HoverDuration = 5f;
  private PlayerItem m_item;
  private float m_internalCooldown;
  private List<HoveringGunController> m_hovers = new List<HoveringGunController>();
  private List<bool> m_hoverInitialized = new List<bool>();

  public void Awake()
  {
    this.m_item = this.GetComponent<PlayerItem>();
    this.m_item.OnActivationStatusChanged += new Action<PlayerItem>(this.HandleActivationStatusChanged);
    this.m_item.OnPreDropEvent += new System.Action(this.HandlePreDrop);
  }

  private void HandlePreDrop()
  {
    if (!this.CreatesHoveringGun)
      return;
    this.DisableAllHoveringGuns();
  }

  private void Update()
  {
    this.m_internalCooldown -= BraveTime.DeltaTime;
    if (!this.CreatesHoveringGun || this.m_hovers.Count <= 0 || (bool) (UnityEngine.Object) this.m_item && (bool) (UnityEngine.Object) this.m_item.LastOwner && !((UnityEngine.Object) this.m_item.LastOwner.CurrentItem != (UnityEngine.Object) this.m_item) && !this.m_item.LastOwner.IsGhost)
      return;
    this.DisableAllHoveringGuns();
  }

  private void HandleActivationStatusChanged(PlayerItem sourceItem)
  {
    if (!(bool) (UnityEngine.Object) this.m_item.LastOwner || !this.m_item.LastOwner.HasActiveBonusSynergy(this.SynergyToCheck))
      return;
    if (sourceItem.IsCurrentlyActive)
      this.HandleActivated();
    else
      this.HandleDeactivated();
  }

  private void HandleActivated()
  {
    if (this.FiresOnActivation && (double) this.m_internalCooldown <= 0.0)
    {
      this.ActivationBurst.DoBurst(this.m_item.LastOwner);
      this.m_internalCooldown = this.ActivationBurstCooldown;
    }
    if (!this.CreatesHoveringGun)
      return;
    this.EnableHoveringGun(0);
  }

  private void HandleDeactivated()
  {
    if (!this.CreatesHoveringGun)
      return;
    this.DisableAllHoveringGuns();
  }

  private void EnableHoveringGun(int index)
  {
    if (this.m_hoverInitialized.Count > index && this.m_hoverInitialized[index])
      return;
    PlayerController lastOwner = this.m_item.LastOwner;
    GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(ResourceCache.Acquire("Global Prefabs/HoveringGun") as GameObject, lastOwner.CenterPosition.ToVector3ZisY(), Quaternion.identity);
    gameObject.transform.parent = lastOwner.transform;
    while (this.m_hovers.Count < index + 1)
    {
      this.m_hovers.Add((HoveringGunController) null);
      this.m_hoverInitialized.Add(false);
    }
    this.m_hovers[index] = gameObject.GetComponent<HoveringGunController>();
    this.m_hovers[index].Position = this.PositionType;
    this.m_hovers[index].Aim = this.AimType;
    this.m_hovers[index].Trigger = this.FireType;
    Gun currentGun = lastOwner.CurrentGun;
    this.m_hovers[index].CooldownTime = 10f;
    this.m_hovers[index].ShootDuration = this.HoverDuration;
    this.m_hovers[index].Initialize(currentGun, lastOwner);
    this.m_hoverInitialized[index] = true;
  }

  private void DisableHoveringGun(int index)
  {
    if (!(bool) (UnityEngine.Object) this.m_hovers[index])
      return;
    UnityEngine.Object.Destroy((UnityEngine.Object) this.m_hovers[index].gameObject);
  }

  private void DisableAllHoveringGuns()
  {
    for (int index = 0; index < this.m_hovers.Count; ++index)
    {
      if ((bool) (UnityEngine.Object) this.m_hovers[index])
        UnityEngine.Object.Destroy((UnityEngine.Object) this.m_hovers[index].gameObject);
    }
    this.m_hovers.Clear();
    this.m_hoverInitialized.Clear();
  }

  private void OnDestroy()
  {
    if (!this.CreatesHoveringGun)
      return;
    this.DisableAllHoveringGuns();
  }
}
