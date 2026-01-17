// Decompiled with JetBrains decompiler
// Type: AkGameObj
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
[DisallowMultipleComponent]
[ExecuteInEditMode]
[AddComponentMenu("Wwise/AkGameObj")]
public class AkGameObj : MonoBehaviour
{
  [SerializeField]
  private AkGameObjListenerList m_listeners = new AkGameObjListenerList();
  public bool isEnvironmentAware = true;
  [SerializeField]
  private bool isStaticObject;
  private Collider m_Collider;
  private AkGameObjEnvironmentData m_envData;
  private AkGameObjPositionData m_posData;
  public AkGameObjPositionOffsetData m_positionOffsetData;
  private bool isRegistered;
  [SerializeField]
  private AkGameObjPosOffsetData m_posOffsetData;
  private const int AK_NUM_LISTENERS = 8;
  [SerializeField]
  private int listenerMask = 1;

  public bool IsUsingDefaultListeners => this.m_listeners.useDefaultListeners;

  public List<AkAudioListener> ListenerList => this.m_listeners.ListenerList;

  internal void AddListener(AkAudioListener listener) => this.m_listeners.Add(listener);

  internal void RemoveListener(AkAudioListener listener) => this.m_listeners.Remove(listener);

  public AKRESULT Register()
  {
    if (this.isRegistered)
      return AKRESULT.AK_Success;
    this.isRegistered = true;
    return AkSoundEngine.RegisterGameObj(this.gameObject, this.gameObject.name);
  }

  private void Awake()
  {
    if (!this.isStaticObject)
      this.m_posData = new AkGameObjPositionData();
    this.m_Collider = this.GetComponent<Collider>();
    if (this.Register() != AKRESULT.AK_Success)
      return;
    int num = (int) AkSoundEngine.SetObjectPosition(this.gameObject, this.GetPosition(), this.GetForward(), this.GetUpward());
    if (this.isEnvironmentAware)
    {
      this.m_envData = new AkGameObjEnvironmentData();
      if ((bool) (Object) this.m_Collider)
        this.m_envData.AddAkEnvironment(this.m_Collider, this.m_Collider);
      this.m_envData.UpdateAuxSend(this.gameObject, this.transform.position);
    }
    this.m_listeners.Init(this);
  }

  private void CheckStaticStatus()
  {
  }

  private void OnEnable() => this.enabled = !this.isStaticObject;

  private void OnDestroy()
  {
    foreach (AkUnityEventHandler component in this.gameObject.GetComponents<AkUnityEventHandler>())
    {
      if (component.triggerList.Contains(-358577003))
        component.DoDestroy();
    }
    if (!AkSoundEngine.IsInitialized())
      return;
    int num = (int) AkSoundEngine.UnregisterGameObj(this.gameObject);
  }

  private void Update()
  {
    if (this.m_envData != null)
      this.m_envData.UpdateAuxSend(this.gameObject, this.transform.position);
    if (this.isStaticObject)
      return;
    Vector3 position = this.GetPosition();
    Vector3 forward = this.GetForward();
    Vector3 upward = this.GetUpward();
    if (this.m_posData.position == position && this.m_posData.forward == forward && this.m_posData.up == upward)
      return;
    this.m_posData.position = position;
    this.m_posData.forward = forward;
    this.m_posData.up = upward;
    int num = (int) AkSoundEngine.SetObjectPosition(this.gameObject, position, forward, upward);
  }

  public Vector3 GetPosition()
  {
    Vector3 vector = this.m_positionOffsetData == null ? this.transform.position : this.transform.position + this.transform.rotation * this.m_positionOffsetData.positionOffset;
    return vector.WithZ(vector.y);
  }

  public virtual Vector3 GetForward() => this.transform.forward;

  public virtual Vector3 GetUpward() => this.transform.up;

  private void OnTriggerEnter(Collider other)
  {
    if (!this.isEnvironmentAware || this.m_envData == null)
      return;
    this.m_envData.AddAkEnvironment(other, this.m_Collider);
  }

  private void OnTriggerExit(Collider other)
  {
    if (!this.isEnvironmentAware || this.m_envData == null)
      return;
    this.m_envData.RemoveAkEnvironment(other, this.m_Collider);
  }
}
