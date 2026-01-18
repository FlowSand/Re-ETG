using UnityEngine;

#nullable disable

public class FuseController : MonoBehaviour
    {
        public tk2dTiledSprite[] fuseSegments;
        public GameObject sparkVFXPrefab;
        public float duration = 5f;
        private float TotalPixelLength;
        private Transform m_sparkInstance;
        private float UsedPixelLength;
        private bool m_triggered;

        private void Start()
        {
            for (int index = 0; index < this.fuseSegments.Length; ++index)
                this.TotalPixelLength += this.fuseSegments[index].dimensions.x;
        }

        public void Trigger() => this.m_triggered = true;

        private void Update()
        {
            if (!this.m_triggered)
                return;
            float usedPixelLength = this.UsedPixelLength;
            this.UsedPixelLength = usedPixelLength + this.TotalPixelLength / this.duration * BraveTime.DeltaTime;
            float x = this.UsedPixelLength - usedPixelLength;
            for (int index = 0; index < this.fuseSegments.Length; ++index)
            {
                if ((double) this.fuseSegments[index].dimensions.x > 0.0)
                {
                    if ((double) this.fuseSegments[index].dimensions.x < (double) x)
                    {
                        x -= this.fuseSegments[index].dimensions.x;
                        this.fuseSegments[index].dimensions = new Vector2(0.0f, this.fuseSegments[index].dimensions.y);
                    }
                    else
                    {
                        this.fuseSegments[index].dimensions -= new Vector2(x, 0.0f);
                        break;
                    }
                }
            }
        }
    }

