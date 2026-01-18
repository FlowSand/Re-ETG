using UnityEngine;

#nullable disable

[AddComponentMenu("2D Toolkit/UI/Core/tk2dUIAudioManager")]
public class tk2dUIAudioManager : MonoBehaviour
    {
        private static tk2dUIAudioManager instance;
        private AudioSource audioSrc;

        public static tk2dUIAudioManager Instance
        {
            get
            {
                if ((Object) tk2dUIAudioManager.instance == (Object) null)
                {
                    tk2dUIAudioManager.instance = Object.FindObjectOfType(typeof (tk2dUIAudioManager)) as tk2dUIAudioManager;
                    if ((Object) tk2dUIAudioManager.instance == (Object) null)
                        tk2dUIAudioManager.instance = new GameObject(nameof (tk2dUIAudioManager)).AddComponent<tk2dUIAudioManager>();
                }
                return tk2dUIAudioManager.instance;
            }
        }

        private void Awake()
        {
            if ((Object) tk2dUIAudioManager.instance == (Object) null)
                tk2dUIAudioManager.instance = this;
            else if ((Object) tk2dUIAudioManager.instance != (Object) this)
            {
                Object.Destroy((Object) this);
                return;
            }
            this.Setup();
        }

        private void Setup()
        {
            if ((Object) this.audioSrc == (Object) null)
                this.audioSrc = this.gameObject.GetComponent<AudioSource>();
            if (!((Object) this.audioSrc == (Object) null))
                return;
            this.audioSrc = this.gameObject.AddComponent<AudioSource>();
            this.audioSrc.playOnAwake = false;
        }

        public void Play(AudioClip clip) => this.audioSrc.PlayOneShot(clip, AudioListener.volume);
    }

