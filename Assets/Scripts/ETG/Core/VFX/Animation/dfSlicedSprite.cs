using System;

using UnityEngine;

#nullable disable

[dfTooltip("Displays a sprite from a Texture Atlas using 9-slice scaling")]
[ExecuteInEditMode]
[dfCategory("Basic Controls")]
[AddComponentMenu("Daikon Forge/User Interface/Sprite/Sliced")]
[dfHelp("http://www.daikonforge.com/docs/df-gui/classdf_sliced_sprite.html")]
[Serializable]
public class dfSlicedSprite : dfSprite
    {
        private static int[] triangleIndices = new int[54]
        {
            0,
            1,
            2,
            2,
            3,
            0,
            4,
            5,
            6,
            6,
            7,
            4,
            8,
            9,
            10,
            10,
            11,
            8,
            12,
            13,
            14,
            14,
            15,
            12,
            1,
            4,
            7,
            7,
            2,
            1,
            9,
            12,
            15,
            15,
            10,
            9,
            3,
            2,
            9,
            9,
            8,
            3,
            7,
            6,
            13,
            13,
            12,
            7,
            2,
            7,
            12,
            12,
            9,
            2
        };
        private static int[][] horzFill = new int[4][]
        {
            new int[4]{ 0, 1, 4, 5 },
            new int[4]{ 3, 2, 7, 6 },
            new int[4]{ 8, 9, 12, 13 },
            new int[4]{ 11, 10, 15, 14 }
        };
        private static int[][] vertFill = new int[4][]
        {
            new int[4]{ 11, 8, 3, 0 },
            new int[4]{ 10, 9, 2, 1 },
            new int[4]{ 15, 12, 7, 4 },
            new int[4]{ 14, 13, 6, 5 }
        };
        private static int[][] fillIndices = new int[4][]
        {
            new int[4],
            new int[4],
            new int[4],
            new int[4]
        };
        private static Vector3[] verts = new Vector3[16];
        private static Vector2[] uv = new Vector2[16];

        protected override void OnRebuildRenderData()
        {
            if ((UnityEngine.Object) this.Atlas == (UnityEngine.Object) null)
                return;
            dfAtlas.ItemInfo spriteInfo = this.SpriteInfo;
            if (spriteInfo == (dfAtlas.ItemInfo) null)
                return;
            this.renderData.Material = this.Atlas.Material;
            if (spriteInfo.border.horizontal == 0 && spriteInfo.border.vertical == 0)
            {
                base.OnRebuildRenderData();
            }
            else
            {
                Color32 color32 = this.ApplyOpacity(!this.IsEnabled ? this.disabledColor : this.color);
                dfSlicedSprite.renderSprite(this.renderData, new dfSprite.RenderOptions()
                {
                    atlas = this.atlas,
                    color = color32,
                    fillAmount = this.fillAmount,
                    fillDirection = this.fillDirection,
                    flip = this.flip,
                    invertFill = this.invertFill,
                    offset = this.pivot.TransformToUpperLeft(this.Size),
                    pixelsToUnits = this.PixelsToUnits(),
                    size = this.Size,
                    spriteInfo = this.SpriteInfo
                });
            }
        }

        internal new static void renderSprite(dfRenderData renderData, dfSprite.RenderOptions options)
        {
            if ((double) options.fillAmount <= 1.4012984643248171E-45)
                return;
            options.baseIndex = renderData.Vertices.Count;
            dfSlicedSprite.rebuildTriangles(renderData, options);
            dfSlicedSprite.rebuildVertices(renderData, options);
            dfSlicedSprite.rebuildUV(renderData, options);
            dfSlicedSprite.rebuildColors(renderData, options);
            if ((double) options.fillAmount >= 1.0)
                return;
            dfSlicedSprite.doFill(renderData, options);
        }

        private static void rebuildTriangles(dfRenderData renderData, dfSprite.RenderOptions options)
        {
            int baseIndex = options.baseIndex;
            dfList<int> triangles = renderData.Triangles;
            for (int index = 0; index < dfSlicedSprite.triangleIndices.Length; ++index)
                triangles.Add(baseIndex + dfSlicedSprite.triangleIndices[index]);
        }

        private static void doFill(dfRenderData renderData, dfSprite.RenderOptions options)
        {
            int baseIndex = options.baseIndex;
            dfList<Vector3> vertices = renderData.Vertices;
            dfList<Vector2> uv = renderData.UV;
            int[][] fillIndices = dfSlicedSprite.getFillIndices(options.fillDirection, baseIndex);
            bool flag = options.invertFill;
            if (options.fillDirection == dfFillDirection.Vertical)
                flag = !flag;
            if (flag)
            {
                for (int index = 0; index < fillIndices.Length; ++index)
                    Array.Reverse((Array) fillIndices[index]);
            }
            int index1 = options.fillDirection != dfFillDirection.Horizontal ? 1 : 0;
            float num1 = vertices[fillIndices[0][flag ? 3 : 0]][index1];
            float num2 = vertices[fillIndices[0][flag ? 0 : 3]][index1];
            float num3 = Mathf.Abs(num2 - num1);
            float num4 = flag ? num2 - options.fillAmount * num3 : num1 + options.fillAmount * num3;
            for (int index2 = 0; index2 < fillIndices.Length; ++index2)
            {
                if (!flag)
                {
                    for (int index3 = 3; index3 > 0; --index3)
                    {
                        float num5 = vertices[fillIndices[index2][index3]][index1];
                        if ((double) num5 >= (double) num4)
                        {
                            Vector3 vector3 = vertices[fillIndices[index2][index3]];
                            vector3[index1] = num4;
                            vertices[fillIndices[index2][index3]] = vector3;
                            float num6 = vertices[fillIndices[index2][index3 - 1]][index1];
                            if ((double) num6 <= (double) num4)
                            {
                                float num7 = num5 - num6;
                                float t = (num4 - num6) / num7;
                                float b = uv[fillIndices[index2][index3]][index1];
                                float a = uv[fillIndices[index2][index3 - 1]][index1];
                                Vector2 vector2 = uv[fillIndices[index2][index3]];
                                vector2[index1] = Mathf.Lerp(a, b, t);
                                uv[fillIndices[index2][index3]] = vector2;
                            }
                        }
                    }
                }
                else
                {
                    for (int index4 = 1; index4 < 4; ++index4)
                    {
                        float num8 = vertices[fillIndices[index2][index4]][index1];
                        if ((double) num8 <= (double) num4)
                        {
                            Vector3 vector3 = vertices[fillIndices[index2][index4]];
                            vector3[index1] = num4;
                            vertices[fillIndices[index2][index4]] = vector3;
                            float num9 = vertices[fillIndices[index2][index4 - 1]][index1];
                            if ((double) num9 >= (double) num4)
                            {
                                float num10 = num8 - num9;
                                float t = (num4 - num9) / num10;
                                float b = uv[fillIndices[index2][index4]][index1];
                                float a = uv[fillIndices[index2][index4 - 1]][index1];
                                Vector2 vector2 = uv[fillIndices[index2][index4]];
                                vector2[index1] = Mathf.Lerp(a, b, t);
                                uv[fillIndices[index2][index4]] = vector2;
                            }
                        }
                    }
                }
            }
        }

        private static int[][] getFillIndices(dfFillDirection fillDirection, int baseIndex)
        {
            int[][] numArray = fillDirection != dfFillDirection.Horizontal ? dfSlicedSprite.vertFill : dfSlicedSprite.horzFill;
            for (int index1 = 0; index1 < 4; ++index1)
            {
                for (int index2 = 0; index2 < 4; ++index2)
                    dfSlicedSprite.fillIndices[index1][index2] = baseIndex + numArray[index1][index2];
            }
            return dfSlicedSprite.fillIndices;
        }

        private static void rebuildVertices(dfRenderData renderData, dfSprite.RenderOptions options)
        {
            float x1 = 0.0f;
            float y = 0.0f;
            float num1 = Mathf.Ceil(options.size.x);
            float num2 = Mathf.Ceil(-options.size.y);
            dfAtlas.ItemInfo spriteInfo = options.spriteInfo;
            float x2 = (float) spriteInfo.border.left;
            float num3 = (float) spriteInfo.border.top;
            float x3 = (float) spriteInfo.border.right;
            float num4 = (float) spriteInfo.border.bottom;
            if (options.flip.IsSet(dfSpriteFlip.FlipHorizontal))
            {
                float num5 = x3;
                x3 = x2;
                x2 = num5;
            }
            if (options.flip.IsSet(dfSpriteFlip.FlipVertical))
            {
                float num6 = num4;
                num4 = num3;
                num3 = num6;
            }
            dfSlicedSprite.verts[0] = new Vector3(x1, y, 0.0f) + options.offset;
            dfSlicedSprite.verts[1] = dfSlicedSprite.verts[0] + new Vector3(x2, 0.0f, 0.0f);
            dfSlicedSprite.verts[2] = dfSlicedSprite.verts[0] + new Vector3(x2, -num3, 0.0f);
            dfSlicedSprite.verts[3] = dfSlicedSprite.verts[0] + new Vector3(0.0f, -num3, 0.0f);
            dfSlicedSprite.verts[4] = new Vector3(num1 - x3, y, 0.0f) + options.offset;
            dfSlicedSprite.verts[5] = dfSlicedSprite.verts[4] + new Vector3(x3, 0.0f, 0.0f);
            dfSlicedSprite.verts[6] = dfSlicedSprite.verts[4] + new Vector3(x3, -num3, 0.0f);
            dfSlicedSprite.verts[7] = dfSlicedSprite.verts[4] + new Vector3(0.0f, -num3, 0.0f);
            dfSlicedSprite.verts[8] = new Vector3(x1, num2 + num4, 0.0f) + options.offset;
            dfSlicedSprite.verts[9] = dfSlicedSprite.verts[8] + new Vector3(x2, 0.0f, 0.0f);
            dfSlicedSprite.verts[10] = dfSlicedSprite.verts[8] + new Vector3(x2, -num4, 0.0f);
            dfSlicedSprite.verts[11] = dfSlicedSprite.verts[8] + new Vector3(0.0f, -num4, 0.0f);
            dfSlicedSprite.verts[12] = new Vector3(num1 - x3, num2 + num4, 0.0f) + options.offset;
            dfSlicedSprite.verts[13] = dfSlicedSprite.verts[12] + new Vector3(x3, 0.0f, 0.0f);
            dfSlicedSprite.verts[14] = dfSlicedSprite.verts[12] + new Vector3(x3, -num4, 0.0f);
            dfSlicedSprite.verts[15] = dfSlicedSprite.verts[12] + new Vector3(0.0f, -num4, 0.0f);
            for (int index = 0; index < dfSlicedSprite.verts.Length; ++index)
                renderData.Vertices.Add((dfSlicedSprite.verts[index] * options.pixelsToUnits).Quantize(options.pixelsToUnits));
        }

        private static void rebuildUV(dfRenderData renderData, dfSprite.RenderOptions options)
        {
            dfAtlas atlas = options.atlas;
            Vector2 vector2_1 = new Vector2((float) atlas.Texture.width, (float) atlas.Texture.height);
            dfAtlas.ItemInfo spriteInfo = options.spriteInfo;
            float num1 = (float) spriteInfo.border.top / vector2_1.y;
            float num2 = (float) spriteInfo.border.bottom / vector2_1.y;
            float num3 = (float) spriteInfo.border.left / vector2_1.x;
            float num4 = (float) spriteInfo.border.right / vector2_1.x;
            Rect region = spriteInfo.region;
            dfSlicedSprite.uv[0] = new Vector2(region.x, region.yMax);
            dfSlicedSprite.uv[1] = new Vector2(region.x + num3, region.yMax);
            dfSlicedSprite.uv[2] = new Vector2(region.x + num3, region.yMax - num1);
            dfSlicedSprite.uv[3] = new Vector2(region.x, region.yMax - num1);
            dfSlicedSprite.uv[4] = new Vector2(region.xMax - num4, region.yMax);
            dfSlicedSprite.uv[5] = new Vector2(region.xMax, region.yMax);
            dfSlicedSprite.uv[6] = new Vector2(region.xMax, region.yMax - num1);
            dfSlicedSprite.uv[7] = new Vector2(region.xMax - num4, region.yMax - num1);
            dfSlicedSprite.uv[8] = new Vector2(region.x, region.y + num2);
            dfSlicedSprite.uv[9] = new Vector2(region.x + num3, region.y + num2);
            dfSlicedSprite.uv[10] = new Vector2(region.x + num3, region.y);
            dfSlicedSprite.uv[11] = new Vector2(region.x, region.y);
            dfSlicedSprite.uv[12] = new Vector2(region.xMax - num4, region.y + num2);
            dfSlicedSprite.uv[13] = new Vector2(region.xMax, region.y + num2);
            dfSlicedSprite.uv[14] = new Vector2(region.xMax, region.y);
            dfSlicedSprite.uv[15] = new Vector2(region.xMax - num4, region.y);
            if (options.flip != dfSpriteFlip.None)
            {
                for (int index = 0; index < dfSlicedSprite.uv.Length; index += 4)
                {
                    Vector2 zero = Vector2.zero;
                    if (options.flip.IsSet(dfSpriteFlip.FlipHorizontal))
                    {
                        Vector2 vector2_2 = dfSlicedSprite.uv[index];
                        dfSlicedSprite.uv[index] = dfSlicedSprite.uv[index + 1];
                        dfSlicedSprite.uv[index + 1] = vector2_2;
                        Vector2 vector2_3 = dfSlicedSprite.uv[index + 2];
                        dfSlicedSprite.uv[index + 2] = dfSlicedSprite.uv[index + 3];
                        dfSlicedSprite.uv[index + 3] = vector2_3;
                    }
                    if (options.flip.IsSet(dfSpriteFlip.FlipVertical))
                    {
                        Vector2 vector2_4 = dfSlicedSprite.uv[index];
                        dfSlicedSprite.uv[index] = dfSlicedSprite.uv[index + 3];
                        dfSlicedSprite.uv[index + 3] = vector2_4;
                        Vector2 vector2_5 = dfSlicedSprite.uv[index + 1];
                        dfSlicedSprite.uv[index + 1] = dfSlicedSprite.uv[index + 2];
                        dfSlicedSprite.uv[index + 2] = vector2_5;
                    }
                }
                if (options.flip.IsSet(dfSpriteFlip.FlipHorizontal))
                {
                    Vector2[] vector2Array = new Vector2[dfSlicedSprite.uv.Length];
                    Array.Copy((Array) dfSlicedSprite.uv, (Array) vector2Array, dfSlicedSprite.uv.Length);
                    Array.Copy((Array) dfSlicedSprite.uv, 0, (Array) dfSlicedSprite.uv, 4, 4);
                    Array.Copy((Array) vector2Array, 4, (Array) dfSlicedSprite.uv, 0, 4);
                    Array.Copy((Array) dfSlicedSprite.uv, 8, (Array) dfSlicedSprite.uv, 12, 4);
                    Array.Copy((Array) vector2Array, 12, (Array) dfSlicedSprite.uv, 8, 4);
                }
                if (options.flip.IsSet(dfSpriteFlip.FlipVertical))
                {
                    Vector2[] vector2Array = new Vector2[dfSlicedSprite.uv.Length];
                    Array.Copy((Array) dfSlicedSprite.uv, (Array) vector2Array, dfSlicedSprite.uv.Length);
                    Array.Copy((Array) dfSlicedSprite.uv, 0, (Array) dfSlicedSprite.uv, 8, 4);
                    Array.Copy((Array) vector2Array, 8, (Array) dfSlicedSprite.uv, 0, 4);
                    Array.Copy((Array) dfSlicedSprite.uv, 4, (Array) dfSlicedSprite.uv, 12, 4);
                    Array.Copy((Array) vector2Array, 12, (Array) dfSlicedSprite.uv, 4, 4);
                }
            }
            for (int index = 0; index < dfSlicedSprite.uv.Length; ++index)
                renderData.UV.Add(dfSlicedSprite.uv[index]);
        }

        private static void rebuildColors(dfRenderData renderData, dfSprite.RenderOptions options)
        {
            for (int index = 0; index < 16; ++index)
                renderData.Colors.Add(options.color);
        }
    }

