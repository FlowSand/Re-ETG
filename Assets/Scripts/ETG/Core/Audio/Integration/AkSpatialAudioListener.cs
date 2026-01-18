using System.Collections.Generic;

using UnityEngine;

#nullable disable

[RequireComponent(typeof (AkAudioListener))]
[AddComponentMenu("Wwise/AkSpatialAudioListener")]
[DisallowMultipleComponent]
public class AkSpatialAudioListener : AkSpatialAudioBase
    {
        private static AkSpatialAudioListener s_SpatialAudioListener;
        private static readonly AkSpatialAudioListener.SpatialAudioListenerList spatialAudioListeners = new AkSpatialAudioListener.SpatialAudioListenerList();
        private AkAudioListener AkAudioListener;

        public static AkAudioListener TheSpatialAudioListener
        {
            get
            {
                return (Object) AkSpatialAudioListener.s_SpatialAudioListener != (Object) null ? AkSpatialAudioListener.s_SpatialAudioListener.AkAudioListener : (AkAudioListener) null;
            }
        }

        public static AkSpatialAudioListener.SpatialAudioListenerList SpatialAudioListeners
        {
            get => AkSpatialAudioListener.spatialAudioListeners;
        }

        private void Awake() => this.AkAudioListener = this.GetComponent<AkAudioListener>();

        private void OnEnable() => AkSpatialAudioListener.spatialAudioListeners.Add(this);

        private void OnDisable() => AkSpatialAudioListener.spatialAudioListeners.Remove(this);

        public class SpatialAudioListenerList
        {
            private readonly List<AkSpatialAudioListener> listenerList = new List<AkSpatialAudioListener>();

            public List<AkSpatialAudioListener> ListenerList => this.listenerList;

            public bool Add(AkSpatialAudioListener listener)
            {
                if ((Object) listener == (Object) null || this.listenerList.Contains(listener))
                    return false;
                this.listenerList.Add(listener);
                this.Refresh();
                return true;
            }

            public bool Remove(AkSpatialAudioListener listener)
            {
                if ((Object) listener == (Object) null || !this.listenerList.Contains(listener))
                    return false;
                this.listenerList.Remove(listener);
                this.Refresh();
                return true;
            }

            private void Refresh()
            {
                if (this.ListenerList.Count == 1)
                {
                    if ((Object) AkSpatialAudioListener.s_SpatialAudioListener != (Object) null)
                    {
                        int num = (int) AkSoundEngine.UnregisterSpatialAudioListener(AkSpatialAudioListener.s_SpatialAudioListener.gameObject);
                    }
                    AkSpatialAudioListener.s_SpatialAudioListener = this.ListenerList[0];
                    if (AkSoundEngine.RegisterSpatialAudioListener(AkSpatialAudioListener.s_SpatialAudioListener.gameObject) != AKRESULT.AK_Success)
                        return;
                    AkSpatialAudioListener.s_SpatialAudioListener.SetGameObjectInRoom();
                }
                else
                {
                    if (this.ListenerList.Count != 0 || !((Object) AkSpatialAudioListener.s_SpatialAudioListener != (Object) null))
                        return;
                    int num = (int) AkSoundEngine.UnregisterSpatialAudioListener(AkSpatialAudioListener.s_SpatialAudioListener.gameObject);
                    AkSpatialAudioListener.s_SpatialAudioListener = (AkSpatialAudioListener) null;
                }
            }
        }
    }

