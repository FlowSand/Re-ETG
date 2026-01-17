// Decompiled with JetBrains decompiler
// Type: SpawnPlayerProcessorOnAcquisition
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
public class SpawnPlayerProcessorOnAcquisition : MonoBehaviour
{
  public GameObject PrefabToSpawn;
  public string Identifier;
  private PassiveItem m_passiveItem;
  private PlayerItem m_playerItem;

  public void Awake()
  {
    this.m_passiveItem = this.GetComponent<PassiveItem>();
    this.m_playerItem = this.GetComponent<PlayerItem>();
    if ((bool) (UnityEngine.Object) this.m_passiveItem)
      this.m_passiveItem.OnPickedUp += new Action<PlayerController>(this.HandlePickedUp);
    if (!(bool) (UnityEngine.Object) this.m_playerItem)
      return;
    this.m_playerItem.OnPickedUp += new Action<PlayerController>(this.HandlePickedUp);
  }

  private void HandlePickedUp(PlayerController p)
  {
    if (!(bool) (UnityEngine.Object) p || p.SpawnedSubobjects.ContainsKey(this.Identifier))
      return;
    GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.PrefabToSpawn);
    gameObject.transform.parent = p.transform;
    gameObject.transform.localPosition = Vector3.zero;
    p.SpawnedSubobjects.Add(this.Identifier, gameObject);
  }
}
