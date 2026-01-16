// Decompiled with JetBrains decompiler
// Type: AkCallbackManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

#nullable disable
public static class AkCallbackManager
{
  private static readonly AkEventCallbackInfo AkEventCallbackInfo = new AkEventCallbackInfo(IntPtr.Zero, false);
  private static readonly AkDynamicSequenceItemCallbackInfo AkDynamicSequenceItemCallbackInfo = new AkDynamicSequenceItemCallbackInfo(IntPtr.Zero, false);
  private static readonly AkMIDIEventCallbackInfo AkMIDIEventCallbackInfo = new AkMIDIEventCallbackInfo(IntPtr.Zero, false);
  private static readonly AkMarkerCallbackInfo AkMarkerCallbackInfo = new AkMarkerCallbackInfo(IntPtr.Zero, false);
  private static readonly AkDurationCallbackInfo AkDurationCallbackInfo = new AkDurationCallbackInfo(IntPtr.Zero, false);
  private static readonly AkMusicSyncCallbackInfo AkMusicSyncCallbackInfo = new AkMusicSyncCallbackInfo(IntPtr.Zero, false);
  private static readonly AkMusicPlaylistCallbackInfo AkMusicPlaylistCallbackInfo = new AkMusicPlaylistCallbackInfo(IntPtr.Zero, false);
  private static readonly AkAudioSourceChangeCallbackInfo AkAudioSourceChangeCallbackInfo = new AkAudioSourceChangeCallbackInfo(IntPtr.Zero, false);
  private static readonly AkMonitoringCallbackInfo AkMonitoringCallbackInfo = new AkMonitoringCallbackInfo(IntPtr.Zero, false);
  private static readonly AkBankCallbackInfo AkBankCallbackInfo = new AkBankCallbackInfo(IntPtr.Zero, false);
  private static readonly Dictionary<int, AkCallbackManager.EventCallbackPackage> m_mapEventCallbacks = new Dictionary<int, AkCallbackManager.EventCallbackPackage>();
  private static readonly Dictionary<int, AkCallbackManager.BankCallbackPackage> m_mapBankCallbacks = new Dictionary<int, AkCallbackManager.BankCallbackPackage>();
  private static AkCallbackManager.EventCallbackPackage m_LastAddedEventPackage;
  private static IntPtr m_pNotifMem = IntPtr.Zero;
  private static AkCallbackManager.MonitoringCallback m_MonitoringCB;
  private static AkCallbackManager.BGMCallbackPackage ms_sourceChangeCallbackPkg;

  public static void RemoveEventCallback(uint in_playingID)
  {
    List<int> intList = new List<int>();
    foreach (KeyValuePair<int, AkCallbackManager.EventCallbackPackage> mapEventCallback in AkCallbackManager.m_mapEventCallbacks)
    {
      if ((int) mapEventCallback.Value.m_playingID == (int) in_playingID)
      {
        intList.Add(mapEventCallback.Key);
        break;
      }
    }
    int count = intList.Count;
    for (int index = 0; index < count; ++index)
      AkCallbackManager.m_mapEventCallbacks.Remove(intList[index]);
    AkSoundEnginePINVOKE.CSharp_CancelEventCallback(in_playingID);
  }

  public static void RemoveEventCallbackCookie(object in_cookie)
  {
    List<int> intList = new List<int>();
    foreach (KeyValuePair<int, AkCallbackManager.EventCallbackPackage> mapEventCallback in AkCallbackManager.m_mapEventCallbacks)
    {
      if (mapEventCallback.Value.m_Cookie == in_cookie)
        intList.Add(mapEventCallback.Key);
    }
    int count = intList.Count;
    for (int index = 0; index < count; ++index)
    {
      int num = intList[index];
      AkCallbackManager.m_mapEventCallbacks.Remove(num);
      AkSoundEnginePINVOKE.CSharp_CancelEventCallbackCookie((IntPtr) num);
    }
  }

  public static void RemoveBankCallback(object in_cookie)
  {
    List<int> intList = new List<int>();
    foreach (KeyValuePair<int, AkCallbackManager.BankCallbackPackage> mapBankCallback in AkCallbackManager.m_mapBankCallbacks)
    {
      if (mapBankCallback.Value.m_Cookie == in_cookie)
        intList.Add(mapBankCallback.Key);
    }
    int count = intList.Count;
    for (int index = 0; index < count; ++index)
    {
      int num = intList[index];
      AkCallbackManager.m_mapBankCallbacks.Remove(num);
      AkSoundEnginePINVOKE.CSharp_CancelBankCallbackCookie((IntPtr) num);
    }
  }

  public static void SetLastAddedPlayingID(uint in_playingID)
  {
    if (AkCallbackManager.m_LastAddedEventPackage == null || AkCallbackManager.m_LastAddedEventPackage.m_playingID != 0U)
      return;
    AkCallbackManager.m_LastAddedEventPackage.m_playingID = in_playingID;
  }

  public static AKRESULT Init(int BufferSize)
  {
    AkCallbackManager.m_pNotifMem = BufferSize <= 0 ? IntPtr.Zero : Marshal.AllocHGlobal(BufferSize);
    return AkCallbackSerializer.Init(AkCallbackManager.m_pNotifMem, (uint) BufferSize);
  }

  public static void Term()
  {
    if (!(AkCallbackManager.m_pNotifMem != IntPtr.Zero))
      return;
    AkCallbackSerializer.Term();
    Marshal.FreeHGlobal(AkCallbackManager.m_pNotifMem);
    AkCallbackManager.m_pNotifMem = IntPtr.Zero;
  }

  public static void SetMonitoringCallback(
    AkMonitorErrorLevel in_Level,
    AkCallbackManager.MonitoringCallback in_CB)
  {
    AkCallbackSerializer.SetLocalOutput(in_CB == null ? 0U : (uint) in_Level);
    AkCallbackManager.m_MonitoringCB = in_CB;
  }

  public static void SetBGMCallback(AkCallbackManager.BGMCallback in_CB, object in_cookie)
  {
    AkCallbackManager.ms_sourceChangeCallbackPkg = new AkCallbackManager.BGMCallbackPackage()
    {
      m_Callback = in_CB,
      m_Cookie = in_cookie
    };
  }

  public static int PostCallbacks()
  {
    if (AkCallbackManager.m_pNotifMem == IntPtr.Zero)
      return 0;
    try
    {
      int num1 = 0;
      IntPtr jarg1 = AkCallbackSerializer.Lock();
      while (jarg1 != IntPtr.Zero)
      {
        IntPtr key = AkSoundEnginePINVOKE.CSharp_AkSerializedCallbackHeader_pPackage_get(jarg1);
        AkCallbackType in_type = (AkCallbackType) AkSoundEnginePINVOKE.CSharp_AkSerializedCallbackHeader_eType_get(jarg1);
        IntPtr data = AkSoundEnginePINVOKE.CSharp_AkSerializedCallbackHeader_GetData(jarg1);
        switch (in_type)
        {
          case AkCallbackType.AK_Monitoring:
            if (AkCallbackManager.m_MonitoringCB != null)
            {
              AkCallbackManager.AkMonitoringCallbackInfo.setCPtr(data);
              AkCallbackManager.m_MonitoringCB(AkCallbackManager.AkMonitoringCallbackInfo.errorCode, AkCallbackManager.AkMonitoringCallbackInfo.errorLevel, AkCallbackManager.AkMonitoringCallbackInfo.playingID, AkCallbackManager.AkMonitoringCallbackInfo.gameObjID, AkCallbackManager.AkMonitoringCallbackInfo.message);
              goto case AkCallbackType.AK_AudioInterruption;
            }
            goto case AkCallbackType.AK_AudioInterruption;
          case AkCallbackType.AK_AudioInterruption:
            jarg1 = AkSoundEnginePINVOKE.CSharp_AkSerializedCallbackHeader_pNext_get(jarg1);
            ++num1;
            continue;
          case AkCallbackType.AK_AudioSourceChange:
            if (AkCallbackManager.ms_sourceChangeCallbackPkg != null && AkCallbackManager.ms_sourceChangeCallbackPkg.m_Callback != null)
            {
              AkCallbackManager.AkAudioSourceChangeCallbackInfo.setCPtr(data);
              int num2 = (int) AkCallbackManager.ms_sourceChangeCallbackPkg.m_Callback(AkCallbackManager.AkAudioSourceChangeCallbackInfo.bOtherAudioPlaying, AkCallbackManager.ms_sourceChangeCallbackPkg.m_Cookie);
              goto case AkCallbackType.AK_AudioInterruption;
            }
            goto case AkCallbackType.AK_AudioInterruption;
          case AkCallbackType.AK_Bank:
            AkCallbackManager.BankCallbackPackage bankCallbackPackage = (AkCallbackManager.BankCallbackPackage) null;
            if (!AkCallbackManager.m_mapBankCallbacks.TryGetValue((int) key, out bankCallbackPackage))
            {
              Debug.LogError((object) $"WwiseUnity: BankCallbackPackage not found for <{(object) key}>.");
              return num1;
            }
            AkCallbackManager.m_mapBankCallbacks.Remove((int) key);
            if (bankCallbackPackage != null && bankCallbackPackage.m_Callback != null)
            {
              AkCallbackManager.AkBankCallbackInfo.setCPtr(data);
              bankCallbackPackage.m_Callback(AkCallbackManager.AkBankCallbackInfo.bankID, AkCallbackManager.AkBankCallbackInfo.inMemoryBankPtr, AkCallbackManager.AkBankCallbackInfo.loadResult, (uint) AkCallbackManager.AkBankCallbackInfo.memPoolId, bankCallbackPackage.m_Cookie);
              goto case AkCallbackType.AK_AudioInterruption;
            }
            goto case AkCallbackType.AK_AudioInterruption;
          default:
            AkCallbackManager.EventCallbackPackage eventCallbackPackage = (AkCallbackManager.EventCallbackPackage) null;
            if (!AkCallbackManager.m_mapEventCallbacks.TryGetValue((int) key, out eventCallbackPackage))
            {
              Debug.LogError((object) $"WwiseUnity: EventCallbackPackage not found for <{(object) key}>.");
              return num1;
            }
            AkCallbackInfo in_info = (AkCallbackInfo) null;
            switch (in_type)
            {
              case AkCallbackType.AK_EndOfEvent:
                AkCallbackManager.m_mapEventCallbacks.Remove(eventCallbackPackage.GetHashCode());
                if (eventCallbackPackage.m_bNotifyEndOfEvent)
                {
                  AkCallbackManager.AkEventCallbackInfo.setCPtr(data);
                  in_info = (AkCallbackInfo) AkCallbackManager.AkEventCallbackInfo;
                  break;
                }
                break;
              case AkCallbackType.AK_EndOfDynamicSequenceItem:
                AkCallbackManager.AkDynamicSequenceItemCallbackInfo.setCPtr(data);
                in_info = (AkCallbackInfo) AkCallbackManager.AkDynamicSequenceItemCallbackInfo;
                break;
              case AkCallbackType.AK_Marker:
                AkCallbackManager.AkMarkerCallbackInfo.setCPtr(data);
                in_info = (AkCallbackInfo) AkCallbackManager.AkMarkerCallbackInfo;
                break;
              case AkCallbackType.AK_Duration:
                AkCallbackManager.AkDurationCallbackInfo.setCPtr(data);
                in_info = (AkCallbackInfo) AkCallbackManager.AkDurationCallbackInfo;
                break;
              default:
                if (in_type != AkCallbackType.AK_MusicPlaylistSelect)
                {
                  if (in_type != AkCallbackType.AK_MusicPlayStarted)
                  {
                    if (in_type != AkCallbackType.AK_MusicSyncBeat && in_type != AkCallbackType.AK_MusicSyncBar && in_type != AkCallbackType.AK_MusicSyncEntry && in_type != AkCallbackType.AK_MusicSyncExit && in_type != AkCallbackType.AK_MusicSyncGrid && in_type != AkCallbackType.AK_MusicSyncUserCue && in_type != AkCallbackType.AK_MusicSyncPoint)
                    {
                      if (in_type == AkCallbackType.AK_MIDIEvent)
                      {
                        AkCallbackManager.AkMIDIEventCallbackInfo.setCPtr(data);
                        in_info = (AkCallbackInfo) AkCallbackManager.AkMIDIEventCallbackInfo;
                        break;
                      }
                      Debug.LogError((object) $"WwiseUnity: PostCallbacks aborted due to error: Undefined callback type <{(object) in_type}> found. Callback object possibly corrupted.");
                      return num1;
                    }
                    AkCallbackManager.AkMusicSyncCallbackInfo.setCPtr(data);
                    in_info = (AkCallbackInfo) AkCallbackManager.AkMusicSyncCallbackInfo;
                    break;
                  }
                  AkCallbackManager.AkEventCallbackInfo.setCPtr(data);
                  in_info = (AkCallbackInfo) AkCallbackManager.AkEventCallbackInfo;
                  break;
                }
                AkCallbackManager.AkMusicPlaylistCallbackInfo.setCPtr(data);
                in_info = (AkCallbackInfo) AkCallbackManager.AkMusicPlaylistCallbackInfo;
                break;
            }
            if (in_info != null)
            {
              eventCallbackPackage.m_Callback(eventCallbackPackage.m_Cookie, in_type, in_info);
              goto case AkCallbackType.AK_AudioInterruption;
            }
            goto case AkCallbackType.AK_AudioInterruption;
        }
      }
      return num1;
    }
    finally
    {
      AkCallbackSerializer.Unlock();
    }
  }

  public delegate void EventCallback(
    object in_cookie,
    AkCallbackType in_type,
    AkCallbackInfo in_info);

  public delegate void MonitoringCallback(
    AkMonitorErrorCode in_errorCode,
    AkMonitorErrorLevel in_errorLevel,
    uint in_playingID,
    ulong in_gameObjID,
    string in_msg);

  public delegate void BankCallback(
    uint in_bankID,
    IntPtr in_InMemoryBankPtr,
    AKRESULT in_eLoadResult,
    uint in_memPoolId,
    object in_Cookie);

  public class EventCallbackPackage
  {
    public bool m_bNotifyEndOfEvent;
    public AkCallbackManager.EventCallback m_Callback;
    public object m_Cookie;
    public uint m_playingID;

    public static AkCallbackManager.EventCallbackPackage Create(
      AkCallbackManager.EventCallback in_cb,
      object in_cookie,
      ref uint io_Flags)
    {
      if (io_Flags == 0U || in_cb == null)
      {
        io_Flags = 0U;
        return (AkCallbackManager.EventCallbackPackage) null;
      }
      AkCallbackManager.EventCallbackPackage eventCallbackPackage = new AkCallbackManager.EventCallbackPackage();
      eventCallbackPackage.m_Callback = in_cb;
      eventCallbackPackage.m_Cookie = in_cookie;
      eventCallbackPackage.m_bNotifyEndOfEvent = ((int) io_Flags & 1) != 0;
      io_Flags |= 1U;
      AkCallbackManager.m_mapEventCallbacks[eventCallbackPackage.GetHashCode()] = eventCallbackPackage;
      AkCallbackManager.m_LastAddedEventPackage = eventCallbackPackage;
      return eventCallbackPackage;
    }

    ~EventCallbackPackage()
    {
      if (this.m_Cookie == null)
        return;
      AkCallbackManager.RemoveEventCallbackCookie(this.m_Cookie);
    }
  }

  public class BankCallbackPackage
  {
    public AkCallbackManager.BankCallback m_Callback;
    public object m_Cookie;

    public BankCallbackPackage(AkCallbackManager.BankCallback in_cb, object in_cookie)
    {
      this.m_Callback = in_cb;
      this.m_Cookie = in_cookie;
      AkCallbackManager.m_mapBankCallbacks[this.GetHashCode()] = this;
    }
  }

  public delegate AKRESULT BGMCallback(bool in_bOtherAudioPlaying, object in_Cookie);

  public class BGMCallbackPackage
  {
    public AkCallbackManager.BGMCallback m_Callback;
    public object m_Cookie;
  }
}
