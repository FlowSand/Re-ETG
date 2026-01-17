// Decompiled with JetBrains decompiler
// Type: AkEnvironment
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
[RequireComponent(typeof (Collider))]
[AddComponentMenu("Wwise/AkEnvironment")]
[ExecuteInEditMode]
public class AkEnvironment : MonoBehaviour
{
  public const int MAX_NB_ENVIRONMENTS = 4;
  public static AkEnvironment.AkEnvironment_CompareByPriority s_compareByPriority = new AkEnvironment.AkEnvironment_CompareByPriority();
  public static AkEnvironment.AkEnvironment_CompareBySelectionAlgorithm s_compareBySelectionAlgorithm = new AkEnvironment.AkEnvironment_CompareBySelectionAlgorithm();
  public bool excludeOthers;
  public bool isDefault;
  public int m_auxBusID;
  private Collider m_Collider;
  public int priority;

  public uint GetAuxBusID() => (uint) this.m_auxBusID;

  public void SetAuxBusID(int in_auxBusID) => this.m_auxBusID = in_auxBusID;

  public float GetAuxSendValueForPosition(Vector3 in_position) => 1f;

  public Collider GetCollider() => this.m_Collider;

  public void Awake() => this.m_Collider = this.GetComponent<Collider>();

  public class AkEnvironment_CompareByPriority : IComparer<AkEnvironment>
  {
    public virtual int Compare(AkEnvironment a, AkEnvironment b)
    {
      int num = a.priority.CompareTo(b.priority);
      return num == 0 && (Object) a != (Object) b ? 1 : num;
    }
  }

  public class AkEnvironment_CompareBySelectionAlgorithm : 
    AkEnvironment.AkEnvironment_CompareByPriority
  {
    public override int Compare(AkEnvironment a, AkEnvironment b)
    {
      if (a.isDefault)
        return b.isDefault ? base.Compare(a, b) : 1;
      if (b.isDefault)
        return -1;
      return a.excludeOthers ? (b.excludeOthers ? base.Compare(a, b) : -1) : (b.excludeOthers ? 1 : base.Compare(a, b));
    }
  }
}
