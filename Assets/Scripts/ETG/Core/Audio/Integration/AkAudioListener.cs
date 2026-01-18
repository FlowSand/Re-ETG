using System.Collections.Generic;

using UnityEngine;

#nullable disable

[AddComponentMenu("Wwise/AkAudioListener")]
[DisallowMultipleComponent]
[RequireComponent(typeof (AkGameObj))]
public class AkAudioListener : MonoBehaviour
    {
        private static readonly AkAudioListener.DefaultListenerList defaultListeners = new AkAudioListener.DefaultListenerList();
        private ulong akGameObjectID = ulong.MaxValue;
        private List<AkGameObj> EmittersToStartListeningTo = new List<AkGameObj>();
        private List<AkGameObj> EmittersToStopListeningTo = new List<AkGameObj>();
        public bool isDefaultListener = true;
        [SerializeField]
        public int listenerId;

        public static AkAudioListener.DefaultListenerList DefaultListeners
        {
            get => AkAudioListener.defaultListeners;
        }

        public void StartListeningToEmitter(AkGameObj emitter)
        {
            this.EmittersToStartListeningTo.Add(emitter);
            this.EmittersToStopListeningTo.Remove(emitter);
        }

        public void StopListeningToEmitter(AkGameObj emitter)
        {
            this.EmittersToStartListeningTo.Remove(emitter);
            this.EmittersToStopListeningTo.Add(emitter);
        }

        public void SetIsDefaultListener(bool isDefault)
        {
            if (this.isDefaultListener == isDefault)
                return;
            this.isDefaultListener = isDefault;
            if (isDefault)
                AkAudioListener.DefaultListeners.Add(this);
            else
                AkAudioListener.DefaultListeners.Remove(this);
        }

        private void Awake()
        {
            AkGameObj orAddComponent = this.gameObject.GetOrAddComponent<AkGameObj>();
            if ((bool) (Object) orAddComponent)
            {
                int num = (int) orAddComponent.Register();
            }
            this.akGameObjectID = AkSoundEngine.GetAkGameObjectID(this.gameObject);
        }

        private void OnEnable()
        {
            if (!this.isDefaultListener)
                return;
            AkAudioListener.DefaultListeners.Add(this);
        }

        private void OnDisable()
        {
            if (!this.isDefaultListener)
                return;
            AkAudioListener.DefaultListeners.Remove(this);
        }

        private void Update()
        {
            for (int index = 0; index < this.EmittersToStartListeningTo.Count; ++index)
                this.EmittersToStartListeningTo[index].AddListener(this);
            this.EmittersToStartListeningTo.Clear();
            for (int index = 0; index < this.EmittersToStopListeningTo.Count; ++index)
                this.EmittersToStopListeningTo[index].RemoveListener(this);
            this.EmittersToStopListeningTo.Clear();
        }

        public ulong GetAkGameObjectID() => this.akGameObjectID;

        public void Migrate14()
        {
            bool flag = this.listenerId == 0;
            Debug.Log((object) ("WwiseUnity: AkAudioListener.Migrate14 for " + this.gameObject.name));
            this.isDefaultListener = flag;
        }

        public class BaseListenerList
        {
            private readonly List<ulong> listenerIdList = new List<ulong>();
            private readonly List<AkAudioListener> listenerList = new List<AkAudioListener>();

            public List<AkAudioListener> ListenerList => this.listenerList;

            public virtual bool Add(AkAudioListener listener)
            {
                if ((Object) listener == (Object) null)
                    return false;
                ulong akGameObjectId = listener.GetAkGameObjectID();
                if (this.listenerIdList.Contains(akGameObjectId))
                    return false;
                this.listenerIdList.Add(akGameObjectId);
                this.listenerList.Add(listener);
                return true;
            }

            public virtual bool Remove(AkAudioListener listener)
            {
                if ((Object) listener == (Object) null)
                    return false;
                ulong akGameObjectId = listener.GetAkGameObjectID();
                if (!this.listenerIdList.Contains(akGameObjectId))
                    return false;
                this.listenerIdList.Remove(akGameObjectId);
                this.listenerList.Remove(listener);
                return true;
            }

            public ulong[] GetListenerIds() => this.listenerIdList.ToArray();
        }

        public class DefaultListenerList : AkAudioListener.BaseListenerList
        {
            public override bool Add(AkAudioListener listener)
            {
                bool flag = base.Add(listener);
                if (flag && AkSoundEngine.IsInitialized())
                {
                    int num = (int) AkSoundEngine.AddDefaultListener(listener.gameObject);
                }
                return flag;
            }

            public override bool Remove(AkAudioListener listener)
            {
                bool flag = base.Remove(listener);
                if (flag && AkSoundEngine.IsInitialized())
                {
                    int num = (int) AkSoundEngine.RemoveDefaultListener(listener.gameObject);
                }
                return flag;
            }
        }
    }

