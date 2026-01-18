using System.Collections;
using System.Diagnostics;

using UnityEngine;

#nullable disable

public class IntroMovieClipPlayer : MonoBehaviour
    {
        public MovieTexture movieTexture;
        public AudioClip movieAudio;
        public dfTextureSprite guiTexture;
        private AudioSource m_source;

        private void Start()
        {
            GameManager.AttemptSoundEngineInitialization();
            this.m_source = Camera.main.gameObject.AddComponent<AudioSource>();
            this.m_source.clip = this.movieAudio;
            this.m_source.pitch = 1.02f;
            this.movieTexture.loop = false;
            this.guiTexture.Texture = (Texture) this.movieTexture;
        }

        public void TriggerMovie() => this.StartCoroutine(this.Do());

        [DebuggerHidden]
        private IEnumerator Do()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new IntroMovieClipPlayer__Doc__Iterator0()
            {
                _this = this
            };
        }
    }

