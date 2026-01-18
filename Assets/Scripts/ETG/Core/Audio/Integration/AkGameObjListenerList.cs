// Decompiled with JetBrains decompiler
// Type: AkGameObjListenerList
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

[Serializable]
public class AkGameObjListenerList : AkAudioListener.BaseListenerList
  {
    [NonSerialized]
    private AkGameObj akGameObj;
    [SerializeField]
    public List<AkAudioListener> initialListenerList = new List<AkAudioListener>();
    [SerializeField]
    public bool useDefaultListeners = true;

    public void SetUseDefaultListeners(bool useDefault)
    {
      if (this.useDefaultListeners == useDefault)
        return;
      this.useDefaultListeners = useDefault;
      if (useDefault)
      {
        int num1 = (int) AkSoundEngine.ResetListenersToDefault(this.akGameObj.gameObject);
        for (int index = 0; index < this.ListenerList.Count; ++index)
        {
          int num2 = (int) AkSoundEngine.AddListener(this.akGameObj.gameObject, this.ListenerList[index].gameObject);
        }
      }
      else
      {
        ulong[] listenerIds = this.GetListenerIds();
        int num = (int) AkSoundEngine.SetListeners(this.akGameObj.gameObject, listenerIds, listenerIds != null ? (uint) listenerIds.Length : 0U);
      }
    }

    public void Init(AkGameObj akGameObj)
    {
      this.akGameObj = akGameObj;
      if (!this.useDefaultListeners)
      {
        int num = (int) AkSoundEngine.SetListeners(akGameObj.gameObject, (ulong[]) null, 0U);
      }
      for (int index = 0; index < this.initialListenerList.Count; ++index)
        this.initialListenerList[index].StartListeningToEmitter(akGameObj);
    }

    public override bool Add(AkAudioListener listener)
    {
      bool flag = base.Add(listener);
      if (flag && AkSoundEngine.IsInitialized())
      {
        int num = (int) AkSoundEngine.AddListener(this.akGameObj.gameObject, listener.gameObject);
      }
      return flag;
    }

    public override bool Remove(AkAudioListener listener)
    {
      bool flag = base.Remove(listener);
      if (flag && AkSoundEngine.IsInitialized())
      {
        int num = (int) AkSoundEngine.RemoveListener(this.akGameObj.gameObject, listener.gameObject);
      }
      return flag;
    }
  }

