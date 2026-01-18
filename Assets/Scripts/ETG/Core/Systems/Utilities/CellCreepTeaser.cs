using UnityEngine;

using Dungeonator;

#nullable disable

public class CellCreepTeaser : MonoBehaviour
    {
        public tk2dSpriteAnimator bodySprite;
        public tk2dSprite shadowSprite;
        private bool isPlaying;

        public void Update()
        {
            if (!this.isPlaying)
            {
                if (GameManager.Instance.IsPaused || Dungeon.IsGenerating || GameManager.Instance.IsLoadingLevel)
                    return;
                this.bodySprite.Play();
                this.isPlaying = true;
            }
            else
            {
                this.shadowSprite.color = this.shadowSprite.color.WithAlpha(Mathf.InverseLerp(3.75f, 3.17f, this.bodySprite.ClipTimeSeconds));
                if (this.bodySprite.Playing)
                    return;
                Object.Destroy((Object) this.gameObject);
            }
        }
    }

