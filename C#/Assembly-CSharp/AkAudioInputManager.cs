// Decompiled with JetBrains decompiler
// Type: AkAudioInputManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

#nullable disable
public static class AkAudioInputManager
{
  private static bool initialized;
  private static readonly Dictionary<uint, AkAudioInputManager.AudioSamplesDelegate> audioSamplesDelegates = new Dictionary<uint, AkAudioInputManager.AudioSamplesDelegate>();
  private static readonly Dictionary<uint, AkAudioInputManager.AudioFormatDelegate> audioFormatDelegates = new Dictionary<uint, AkAudioInputManager.AudioFormatDelegate>();
  private static readonly AkAudioFormat audioFormat = new AkAudioFormat();
  private static readonly AkAudioInputManager.AudioSamplesInteropDelegate audioSamplesDelegate;
  private static readonly AkAudioInputManager.AudioFormatInteropDelegate audioFormatDelegate;

  public static uint PostAudioInputEvent(
    AK.Wwise.Event akEvent,
    GameObject gameObject,
    AkAudioInputManager.AudioSamplesDelegate sampleDelegate,
    AkAudioInputManager.AudioFormatDelegate formatDelegate = null)
  {
    AkAudioInputManager.TryInitialize();
    AK.Wwise.Event @event = akEvent;
    GameObject gameObject1 = gameObject;
    // ISSUE: reference to a compiler-generated field
    if (AkAudioInputManager.\u003C\u003Ef__mg\u0024cache0 == null)
    {
      // ISSUE: reference to a compiler-generated field
      AkAudioInputManager.\u003C\u003Ef__mg\u0024cache0 = new AkCallbackManager.EventCallback(AkAudioInputManager.EventCallback);
    }
    // ISSUE: reference to a compiler-generated field
    AkCallbackManager.EventCallback fMgCache0 = AkAudioInputManager.\u003C\u003Ef__mg\u0024cache0;
    uint playingID = @event.Post(gameObject1, 1U, fMgCache0);
    AkAudioInputManager.AddPlayingID(playingID, sampleDelegate, formatDelegate);
    return playingID;
  }

  public static uint PostAudioInputEvent(
    uint akEventID,
    GameObject gameObject,
    AkAudioInputManager.AudioSamplesDelegate sampleDelegate,
    AkAudioInputManager.AudioFormatDelegate formatDelegate = null)
  {
    AkAudioInputManager.TryInitialize();
    int in_eventID = (int) akEventID;
    GameObject in_gameObjectID = gameObject;
    // ISSUE: reference to a compiler-generated field
    if (AkAudioInputManager.\u003C\u003Ef__mg\u0024cache1 == null)
    {
      // ISSUE: reference to a compiler-generated field
      AkAudioInputManager.\u003C\u003Ef__mg\u0024cache1 = new AkCallbackManager.EventCallback(AkAudioInputManager.EventCallback);
    }
    // ISSUE: reference to a compiler-generated field
    AkCallbackManager.EventCallback fMgCache1 = AkAudioInputManager.\u003C\u003Ef__mg\u0024cache1;
    uint playingID = AkSoundEngine.PostEvent((uint) in_eventID, in_gameObjectID, 1U, fMgCache1, (object) null);
    AkAudioInputManager.AddPlayingID(playingID, sampleDelegate, formatDelegate);
    return playingID;
  }

  public static uint PostAudioInputEvent(
    string akEventName,
    GameObject gameObject,
    AkAudioInputManager.AudioSamplesDelegate sampleDelegate,
    AkAudioInputManager.AudioFormatDelegate formatDelegate = null)
  {
    AkAudioInputManager.TryInitialize();
    string in_pszEventName = akEventName;
    GameObject in_gameObjectID = gameObject;
    // ISSUE: reference to a compiler-generated field
    if (AkAudioInputManager.\u003C\u003Ef__mg\u0024cache2 == null)
    {
      // ISSUE: reference to a compiler-generated field
      AkAudioInputManager.\u003C\u003Ef__mg\u0024cache2 = new AkCallbackManager.EventCallback(AkAudioInputManager.EventCallback);
    }
    // ISSUE: reference to a compiler-generated field
    AkCallbackManager.EventCallback fMgCache2 = AkAudioInputManager.\u003C\u003Ef__mg\u0024cache2;
    uint playingID = AkSoundEngine.PostEvent(in_pszEventName, in_gameObjectID, 1U, fMgCache2, (object) null);
    AkAudioInputManager.AddPlayingID(playingID, sampleDelegate, formatDelegate);
    return playingID;
  }

  [AOT.MonoPInvokeCallback(typeof (AkAudioInputManager.AudioSamplesInteropDelegate))]
  private static bool InternalAudioSamplesDelegate(
    uint playingID,
    float[] samples,
    uint channelIndex,
    uint frames)
  {
    return AkAudioInputManager.audioSamplesDelegates.ContainsKey(playingID) && AkAudioInputManager.audioSamplesDelegates[playingID](playingID, channelIndex, samples);
  }

  [AOT.MonoPInvokeCallback(typeof (AkAudioInputManager.AudioFormatInteropDelegate))]
  private static void InternalAudioFormatDelegate(uint playingID, IntPtr format)
  {
    if (!AkAudioInputManager.audioFormatDelegates.ContainsKey(playingID))
      return;
    AkAudioInputManager.audioFormat.setCPtr(format);
    AkAudioInputManager.audioFormatDelegates[playingID](playingID, AkAudioInputManager.audioFormat);
  }

  private static void TryInitialize()
  {
    if (AkAudioInputManager.initialized)
      return;
    AkAudioInputManager.initialized = true;
    AkSoundEngine.SetAudioInputCallbacks(AkAudioInputManager.audioSamplesDelegate, AkAudioInputManager.audioFormatDelegate);
  }

  private static void AddPlayingID(
    uint playingID,
    AkAudioInputManager.AudioSamplesDelegate sampleDelegate,
    AkAudioInputManager.AudioFormatDelegate formatDelegate)
  {
    if (playingID == 0U || sampleDelegate == null)
      return;
    AkAudioInputManager.audioSamplesDelegates.Add(playingID, sampleDelegate);
    if (formatDelegate == null)
      return;
    AkAudioInputManager.audioFormatDelegates.Add(playingID, formatDelegate);
  }

  private static void EventCallback(
    object cookie,
    AkCallbackType type,
    AkCallbackInfo callbackInfo)
  {
    if (type != AkCallbackType.AK_EndOfEvent || !(callbackInfo is AkEventCallbackInfo eventCallbackInfo))
      return;
    AkAudioInputManager.audioSamplesDelegates.Remove(eventCallbackInfo.playingID);
    AkAudioInputManager.audioFormatDelegates.Remove(eventCallbackInfo.playingID);
  }

  static AkAudioInputManager()
  {
    // ISSUE: reference to a compiler-generated field
    if (AkAudioInputManager.\u003C\u003Ef__mg\u0024cache3 == null)
    {
      // ISSUE: reference to a compiler-generated field
      AkAudioInputManager.\u003C\u003Ef__mg\u0024cache3 = new AkAudioInputManager.AudioSamplesInteropDelegate(AkAudioInputManager.InternalAudioSamplesDelegate);
    }
    // ISSUE: reference to a compiler-generated field
    AkAudioInputManager.audioSamplesDelegate = AkAudioInputManager.\u003C\u003Ef__mg\u0024cache3;
    // ISSUE: reference to a compiler-generated field
    if (AkAudioInputManager.\u003C\u003Ef__mg\u0024cache4 == null)
    {
      // ISSUE: reference to a compiler-generated field
      AkAudioInputManager.\u003C\u003Ef__mg\u0024cache4 = new AkAudioInputManager.AudioFormatInteropDelegate(AkAudioInputManager.InternalAudioFormatDelegate);
    }
    // ISSUE: reference to a compiler-generated field
    AkAudioInputManager.audioFormatDelegate = AkAudioInputManager.\u003C\u003Ef__mg\u0024cache4;
  }

  public delegate void AudioFormatDelegate(uint playingID, AkAudioFormat format);

  [UnmanagedFunctionPointer(CallingConvention.StdCall)]
  public delegate void AudioFormatInteropDelegate(uint playingID, IntPtr format);

  public delegate bool AudioSamplesDelegate(uint playingID, uint channelIndex, float[] samples);

  [UnmanagedFunctionPointer(CallingConvention.StdCall)]
  public delegate bool AudioSamplesInteropDelegate(
    uint playingID,
    [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3), In, Out] float[] samples,
    uint channelIndex,
    uint frames);
}
