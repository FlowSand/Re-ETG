// Decompiled with JetBrains decompiler
// Type: AkSpatialAudioEmitter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using AK.Wwise;
using UnityEngine;

#nullable disable

[AddComponentMenu("Wwise/AkSpatialAudioEmitter")]
[RequireComponent(typeof (AkGameObj))]
public class AkSpatialAudioEmitter : AkSpatialAudioBase
  {
    [Header("Early Reflections")]
    public AuxBus reflectAuxBus;
    public float reflectionMaxPathLength = 1000f;
    [Range(0.0f, 1f)]
    public float reflectionsAuxBusGain = 1f;
    [Range(1f, 4f)]
    public uint reflectionsOrder = 1;
    [Range(0.0f, 1f)]
    [Header("Rooms")]
    public float roomReverbAuxBusGain = 1f;

    private void OnEnable()
    {
      if (AkSoundEngine.RegisterEmitter(this.gameObject, new AkEmitterSettings()
      {
        reflectAuxBusID = (uint) this.reflectAuxBus.ID,
        reflectionMaxPathLength = this.reflectionMaxPathLength,
        reflectionsAuxBusGain = this.reflectionsAuxBusGain,
        reflectionsOrder = this.reflectionsOrder,
        reflectorFilterMask = uint.MaxValue,
        roomReverbAuxBusGain = this.roomReverbAuxBusGain,
        useImageSources = (byte) 0
      }) != AKRESULT.AK_Success)
        return;
      this.SetGameObjectInRoom();
    }

    private void OnDisable()
    {
      int num = (int) AkSoundEngine.UnregisterEmitter(this.gameObject);
    }
  }

