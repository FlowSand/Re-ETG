using System;

using UnityEngine;

#nullable disable

[dfTooltip("Implements a Sprite that can be tiled horizontally and vertically")]
[dfHelp("http://www.daikonforge.com/docs/df-gui/classdf_tiled_sprite.html")]
[ExecuteInEditMode]
[dfCategory("Basic Controls")]
[AddComponentMenu("Daikon Forge/User Interface/Sprite/Tiled")]
[Serializable]
public class dfTiledSprite : dfSprite
    {
        private static int[] quadTriangles = new int[6]
        {
            0,
            1,
            3,
            3,
            1,
            2
        };
        private static Vector2[] quadUV = new Vector2[4];
        [SerializeField]
        protected Vector2 tileScale = Vector2.one;
        [SerializeField]
        protected Vector2 tileScroll = Vector2.zero;
        public bool EnableBlackLineFix;

        public Vector2 TileScale
        {
            get => this.tileScale;
            set
            {
                if ((double) Vector2.Distance(value, this.tileScale) <= 1.4012984643248171E-45)
                    return;
                this.tileScale = Vector2.Max(Vector2.one * 0.1f, value);
                this.Invalidate();
            }
        }

        public Vector2 TileScroll
        {
            get => this.tileScroll;
            set
            {
                if ((double) Vector2.Distance(value, this.tileScroll) <= 1.4012984643248171E-45)
                    return;
                this.tileScroll = value;
                this.Invalidate();
            }
        }

        protected override void OnRebuildRenderData()
        {
            if ((UnityEngine.Object) this.Atlas == (UnityEngine.Object) null)
                return;
            dfAtlas.ItemInfo spriteInfo = this.SpriteInfo;
            if (spriteInfo == (dfAtlas.ItemInfo) null)
                return;
            this.renderData.Material = this.Atlas.Material;
            dfList<Vector3> vertices = this.renderData.Vertices;
            dfList<Vector2> uv = this.renderData.UV;
            dfList<Color32> colors = this.renderData.Colors;
            dfList<int> triangles = this.renderData.Triangles;
            Vector2[] spriteUV = this.buildQuadUV();
            Vector2 vector2_1 = Vector2.Scale(spriteInfo.sizeInPixels, this.tileScale);
            Vector2 vector2_2 = new Vector2(this.tileScroll.x % 1f, this.tileScroll.y % 1f);
            float num1 = !this.EnableBlackLineFix ? 0.0f : -0.1f;
            for (float num2 = -Mathf.Abs(vector2_2.y * vector2_1.y); (double) num2 < (double) this.size.y; num2 += vector2_1.y)
            {
                for (float x = -Mathf.Abs(vector2_2.x * vector2_1.x); (double) x < (double) this.size.x; x += vector2_1.x)
                {
                    int count = vertices.Count;
                    vertices.Add(new Vector3(x, -num2));
                    vertices.Add(new Vector3(x + vector2_1.x, -num2));
                    vertices.Add(new Vector3(x + vector2_1.x, (float) (-(double) num2 + -(double) vector2_1.y) + num1));
                    vertices.Add(new Vector3(x, (float) (-(double) num2 + -(double) vector2_1.y) + num1));
                    this.addQuadTriangles(triangles, count);
                    this.addQuadUV(uv, spriteUV);
                    this.addQuadColors(colors);
                }
            }
            this.clipQuads(vertices, uv);
            float units = this.PixelsToUnits();
            Vector3 upperLeft = this.pivot.TransformToUpperLeft(this.size);
            for (int index = 0; index < vertices.Count; ++index)
                vertices[index] = (vertices[index] + upperLeft) * units;
        }

        private void clipQuads(dfList<Vector3> verts, dfList<Vector2> uv)
        {
            float a1 = 0.0f;
            float b1 = this.size.x;
            float b2 = -this.size.y;
            float a2 = 0.0f;
            if ((double) this.fillAmount < 1.0)
            {
                if (this.fillDirection == dfFillDirection.Horizontal)
                {
                    if (!this.invertFill)
                        b1 = this.size.x * this.fillAmount;
                    else
                        a1 = this.size.x - this.size.x * this.fillAmount;
                }
                else if (!this.invertFill)
                    b2 = -this.size.y * this.fillAmount;
                else
                    a2 = (float) (-(double) this.size.y * (1.0 - (double) this.fillAmount));
            }
            for (int index1 = 0; index1 < verts.Count; index1 += 4)
            {
                Vector3 vector3_1 = verts[index1];
                Vector3 vector3_2 = verts[index1 + 1];
                Vector3 vector3_3 = verts[index1 + 2];
                Vector3 vector3_4 = verts[index1 + 3];
                float num1 = vector3_2.x - vector3_1.x;
                float num2 = vector3_1.y - vector3_4.y;
                if ((double) vector3_1.x < (double) a1)
                {
                    float t = (a1 - vector3_1.x) / num1;
                    dfList<Vector3> dfList1 = verts;
                    int index2 = index1;
                    vector3_1 = new Vector3(Mathf.Max(a1, vector3_1.x), vector3_1.y, vector3_1.z);
                    Vector3 vector3_5 = vector3_1;
                    dfList1[index2] = vector3_5;
                    dfList<Vector3> dfList2 = verts;
                    int index3 = index1 + 1;
                    vector3_2 = new Vector3(Mathf.Max(a1, vector3_2.x), vector3_2.y, vector3_2.z);
                    Vector3 vector3_6 = vector3_2;
                    dfList2[index3] = vector3_6;
                    dfList<Vector3> dfList3 = verts;
                    int index4 = index1 + 2;
                    vector3_3 = new Vector3(Mathf.Max(a1, vector3_3.x), vector3_3.y, vector3_3.z);
                    Vector3 vector3_7 = vector3_3;
                    dfList3[index4] = vector3_7;
                    dfList<Vector3> dfList4 = verts;
                    int index5 = index1 + 3;
                    vector3_4 = new Vector3(Mathf.Max(a1, vector3_4.x), vector3_4.y, vector3_4.z);
                    Vector3 vector3_8 = vector3_4;
                    dfList4[index5] = vector3_8;
                    float x = Mathf.Lerp(uv[index1].x, uv[index1 + 1].x, t);
                    uv[index1] = new Vector2(x, uv[index1].y);
                    uv[index1 + 3] = new Vector2(x, uv[index1 + 3].y);
                    num1 = vector3_2.x - vector3_1.x;
                }
                if ((double) vector3_2.x > (double) b1)
                {
                    float t = (float) (1.0 - ((double) b1 - (double) vector3_2.x + (double) num1) / (double) num1);
                    dfList<Vector3> dfList5 = verts;
                    int index6 = index1;
                    vector3_1 = new Vector3(Mathf.Min(vector3_1.x, b1), vector3_1.y, vector3_1.z);
                    Vector3 vector3_9 = vector3_1;
                    dfList5[index6] = vector3_9;
                    dfList<Vector3> dfList6 = verts;
                    int index7 = index1 + 1;
                    vector3_2 = new Vector3(Mathf.Min(vector3_2.x, b1), vector3_2.y, vector3_2.z);
                    Vector3 vector3_10 = vector3_2;
                    dfList6[index7] = vector3_10;
                    dfList<Vector3> dfList7 = verts;
                    int index8 = index1 + 2;
                    vector3_3 = new Vector3(Mathf.Min(vector3_3.x, b1), vector3_3.y, vector3_3.z);
                    Vector3 vector3_11 = vector3_3;
                    dfList7[index8] = vector3_11;
                    dfList<Vector3> dfList8 = verts;
                    int index9 = index1 + 3;
                    vector3_4 = new Vector3(Mathf.Min(vector3_4.x, b1), vector3_4.y, vector3_4.z);
                    Vector3 vector3_12 = vector3_4;
                    dfList8[index9] = vector3_12;
                    float x = Mathf.Lerp(uv[index1 + 1].x, uv[index1].x, t);
                    uv[index1 + 1] = new Vector2(x, uv[index1 + 1].y);
                    uv[index1 + 2] = new Vector2(x, uv[index1 + 2].y);
                    float num3 = vector3_2.x - vector3_1.x;
                }
                if ((double) vector3_4.y < (double) b2)
                {
                    float t = (float) (1.0 - (double) Mathf.Abs(-b2 + vector3_1.y) / (double) num2);
                    dfList<Vector3> dfList9 = verts;
                    int index10 = index1;
                    vector3_1 = new Vector3(vector3_1.x, Mathf.Max(vector3_1.y, b2), vector3_2.z);
                    Vector3 vector3_13 = vector3_1;
                    dfList9[index10] = vector3_13;
                    dfList<Vector3> dfList10 = verts;
                    int index11 = index1 + 1;
                    vector3_2 = new Vector3(vector3_2.x, Mathf.Max(vector3_2.y, b2), vector3_2.z);
                    Vector3 vector3_14 = vector3_2;
                    dfList10[index11] = vector3_14;
                    dfList<Vector3> dfList11 = verts;
                    int index12 = index1 + 2;
                    vector3_3 = new Vector3(vector3_3.x, Mathf.Max(vector3_3.y, b2), vector3_3.z);
                    Vector3 vector3_15 = vector3_3;
                    dfList11[index12] = vector3_15;
                    dfList<Vector3> dfList12 = verts;
                    int index13 = index1 + 3;
                    vector3_4 = new Vector3(vector3_4.x, Mathf.Max(vector3_4.y, b2), vector3_4.z);
                    Vector3 vector3_16 = vector3_4;
                    dfList12[index13] = vector3_16;
                    float y = Mathf.Lerp(uv[index1 + 3].y, uv[index1].y, t);
                    uv[index1 + 3] = new Vector2(uv[index1 + 3].x, y);
                    uv[index1 + 2] = new Vector2(uv[index1 + 2].x, y);
                    num2 = Mathf.Abs(vector3_4.y - vector3_1.y);
                }
                if ((double) vector3_1.y > (double) a2)
                {
                    float t = Mathf.Abs(a2 - vector3_1.y) / num2;
                    dfList<Vector3> dfList13 = verts;
                    int index14 = index1;
                    vector3_1 = new Vector3(vector3_1.x, Mathf.Min(a2, vector3_1.y), vector3_1.z);
                    Vector3 vector3_17 = vector3_1;
                    dfList13[index14] = vector3_17;
                    dfList<Vector3> dfList14 = verts;
                    int index15 = index1 + 1;
                    vector3_2 = new Vector3(vector3_2.x, Mathf.Min(a2, vector3_2.y), vector3_2.z);
                    Vector3 vector3_18 = vector3_2;
                    dfList14[index15] = vector3_18;
                    dfList<Vector3> dfList15 = verts;
                    int index16 = index1 + 2;
                    vector3_3 = new Vector3(vector3_3.x, Mathf.Min(a2, vector3_3.y), vector3_3.z);
                    Vector3 vector3_19 = vector3_3;
                    dfList15[index16] = vector3_19;
                    dfList<Vector3> dfList16 = verts;
                    int index17 = index1 + 3;
                    vector3_4 = new Vector3(vector3_4.x, Mathf.Min(a2, vector3_4.y), vector3_4.z);
                    Vector3 vector3_20 = vector3_4;
                    dfList16[index17] = vector3_20;
                    float y = Mathf.Lerp(uv[index1].y, uv[index1 + 3].y, t);
                    uv[index1] = new Vector2(uv[index1].x, y);
                    uv[index1 + 1] = new Vector2(uv[index1 + 1].x, y);
                }
            }
        }

        private void addQuadTriangles(dfList<int> triangles, int baseIndex)
        {
            for (int index = 0; index < dfTiledSprite.quadTriangles.Length; ++index)
                triangles.Add(dfTiledSprite.quadTriangles[index] + baseIndex);
        }

        private void addQuadColors(dfList<Color32> colors)
        {
            colors.EnsureCapacity(colors.Count + 4);
            Color32 color32 = this.ApplyOpacity(!this.IsEnabled ? this.disabledColor : this.color);
            for (int index = 0; index < 4; ++index)
                colors.Add(color32);
        }

        private void addQuadUV(dfList<Vector2> uv, Vector2[] spriteUV) => uv.AddRange(spriteUV);

        private Vector2[] buildQuadUV()
        {
            Rect region = this.SpriteInfo.region;
            dfTiledSprite.quadUV[0] = new Vector2(region.x, region.yMax);
            dfTiledSprite.quadUV[1] = new Vector2(region.xMax, region.yMax);
            dfTiledSprite.quadUV[2] = new Vector2(region.xMax, region.y);
            dfTiledSprite.quadUV[3] = new Vector2(region.x, region.y);
            Vector2 zero = Vector2.zero;
            if (this.flip.IsSet(dfSpriteFlip.FlipHorizontal))
            {
                Vector2 vector2_1 = dfTiledSprite.quadUV[1];
                dfTiledSprite.quadUV[1] = dfTiledSprite.quadUV[0];
                dfTiledSprite.quadUV[0] = vector2_1;
                Vector2 vector2_2 = dfTiledSprite.quadUV[3];
                dfTiledSprite.quadUV[3] = dfTiledSprite.quadUV[2];
                dfTiledSprite.quadUV[2] = vector2_2;
            }
            if (this.flip.IsSet(dfSpriteFlip.FlipVertical))
            {
                Vector2 vector2_3 = dfTiledSprite.quadUV[0];
                dfTiledSprite.quadUV[0] = dfTiledSprite.quadUV[3];
                dfTiledSprite.quadUV[3] = vector2_3;
                Vector2 vector2_4 = dfTiledSprite.quadUV[1];
                dfTiledSprite.quadUV[1] = dfTiledSprite.quadUV[2];
                dfTiledSprite.quadUV[2] = vector2_4;
            }
            return dfTiledSprite.quadUV;
        }
    }

