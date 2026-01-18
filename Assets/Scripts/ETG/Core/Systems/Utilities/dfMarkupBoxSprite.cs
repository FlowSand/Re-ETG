using System;

using UnityEngine;

#nullable disable

public class dfMarkupBoxSprite : dfMarkupBox
    {
        public dfMarkupBoxSprite(
            dfMarkupElement element,
            dfMarkupDisplayType display,
            dfMarkupStyle style) : base(element, display, style)
        {
        }

        private static int[] TRIANGLE_INDICES = new int[6]
        {
            0,
            1,
            2,
            0,
            2,
            3
        };
        private dfRenderData renderData = new dfRenderData();

        public dfAtlas Atlas { get; set; }

        public string Source { get; set; }

        internal void LoadImage(dfAtlas atlas, string source)
        {
            dfAtlas.ItemInfo atla = atlas[source];
            if (atla == (dfAtlas.ItemInfo) null)
                throw new InvalidOperationException("Sprite does not exist in atlas: " + source);
            this.Atlas = atlas;
            this.Source = source;
            this.Size = atla.sizeInPixels;
            this.Baseline = (int) this.Size.y;
        }

        protected override dfRenderData OnRebuildRenderData()
        {
            this.renderData.Clear();
            if ((UnityEngine.Object) this.Atlas != (UnityEngine.Object) null && this.Atlas[this.Source] != (dfAtlas.ItemInfo) null)
            {
                dfSlicedSprite.renderSprite(this.renderData, new dfSprite.RenderOptions()
                {
                    atlas = this.Atlas,
                    spriteInfo = this.Atlas[this.Source],
                    pixelsToUnits = 1f,
                    size = this.Size,
                    color = (Color32) this.Style.Color,
                    baseIndex = 0,
                    fillAmount = 1f,
                    flip = dfSpriteFlip.None
                });
                this.renderData.Material = this.Atlas.Material;
                this.renderData.Transform = Matrix4x4.identity;
            }
            return this.renderData;
        }

        private static void addTriangleIndices(dfList<Vector3> verts, dfList<int> triangles)
        {
            int count = verts.Count;
            foreach (int triangleIndex in dfMarkupBoxSprite.TRIANGLE_INDICES)
                triangles.Add(count + triangleIndex);
        }
    }

