// Decompiled with JetBrains decompiler
// Type: tk2dRuntime.SpriteCollectionGenerator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using System.IO;
using UnityEngine;

#nullable disable
namespace tk2dRuntime
{
  internal static class SpriteCollectionGenerator
  {
    public static tk2dSpriteCollectionData CreateFromTexture(
      Texture texture,
      tk2dSpriteCollectionSize size,
      Rect region,
      Vector2 anchor)
    {
      return SpriteCollectionGenerator.CreateFromTexture(texture, size, new string[1]
      {
        "Unnamed"
      }, new Rect[1]{ region }, new Vector2[1]{ anchor });
    }

    public static tk2dSpriteCollectionData CreateFromTexture(
      Texture texture,
      tk2dSpriteCollectionSize size,
      string[] names,
      Rect[] regions,
      Vector2[] anchors)
    {
      Vector2 textureDimensions = new Vector2((float) texture.width, (float) texture.height);
      return SpriteCollectionGenerator.CreateFromTexture(texture, size, textureDimensions, names, regions, (Rect[]) null, anchors, (bool[]) null);
    }

    public static tk2dSpriteCollectionData CreateFromTexture(
      Texture texture,
      tk2dSpriteCollectionSize size,
      Vector2 textureDimensions,
      string[] names,
      Rect[] regions,
      Rect[] trimRects,
      Vector2[] anchors,
      bool[] rotated)
    {
      return SpriteCollectionGenerator.CreateFromTexture((GameObject) null, texture, size, textureDimensions, names, regions, trimRects, anchors, rotated, string.Empty);
    }

    public static tk2dSpriteCollectionData CreateFromTexture(
      GameObject parentObject,
      Texture texture,
      tk2dSpriteCollectionSize size,
      Vector2 textureDimensions,
      string[] names,
      Rect[] regions,
      Rect[] trimRects,
      Vector2[] anchors,
      bool[] rotated,
      string CustomShader = "")
    {
      tk2dSpriteCollectionData fromTexture = (!((Object) parentObject != (Object) null) ? new GameObject("SpriteCollection") : parentObject).AddComponent<tk2dSpriteCollectionData>();
      fromTexture.Transient = true;
      fromTexture.version = 3;
      fromTexture.invOrthoSize = 1f / size.OrthoSize;
      fromTexture.halfTargetHeight = size.TargetHeight * 0.5f;
      fromTexture.premultipliedAlpha = false;
      string name = string.IsNullOrEmpty(CustomShader) ? "tk2d/BlendVertexColor" : CustomShader;
      fromTexture.material = new Material(Shader.Find(name));
      fromTexture.material.mainTexture = texture;
      fromTexture.materials = new Material[1]
      {
        fromTexture.material
      };
      fromTexture.textures = new Texture[1]{ texture };
      fromTexture.buildKey = Random.Range(0, int.MaxValue);
      float scale = 2f * size.OrthoSize / size.TargetHeight;
      Rect trimRect = new Rect(0.0f, 0.0f, 0.0f, 0.0f);
      fromTexture.spriteDefinitions = new tk2dSpriteDefinition[regions.Length];
      for (int index = 0; index < regions.Length; ++index)
      {
        bool rotated1 = rotated != null && rotated[index];
        if (trimRects != null)
          trimRect = trimRects[index];
        else if (rotated1)
          trimRect.Set(0.0f, 0.0f, regions[index].height, regions[index].width);
        else
          trimRect.Set(0.0f, 0.0f, regions[index].width, regions[index].height);
        fromTexture.spriteDefinitions[index] = SpriteCollectionGenerator.CreateDefinitionForRegionInTexture(names[index], textureDimensions, scale, regions[index], trimRect, anchors[index], rotated1);
      }
      foreach (tk2dSpriteDefinition spriteDefinition in fromTexture.spriteDefinitions)
        spriteDefinition.material = fromTexture.material;
      return fromTexture;
    }

    private static tk2dSpriteDefinition CreateDefinitionForRegionInTexture(
      string name,
      Vector2 textureDimensions,
      float scale,
      Rect uvRegion,
      Rect trimRect,
      Vector2 anchor,
      bool rotated)
    {
      float height = uvRegion.height;
      float width = uvRegion.width;
      float x = textureDimensions.x;
      float y = textureDimensions.y;
      tk2dSpriteDefinition forRegionInTexture = new tk2dSpriteDefinition();
      forRegionInTexture.flipped = !rotated ? tk2dSpriteDefinition.FlipMode.None : tk2dSpriteDefinition.FlipMode.TPackerCW;
      forRegionInTexture.extractRegion = false;
      forRegionInTexture.name = name;
      forRegionInTexture.colliderType = tk2dSpriteDefinition.ColliderType.Unset;
      Vector2 vector2_1 = new Vector2(1f / 1000f, 1f / 1000f);
      Vector2 vector2_2 = new Vector2((uvRegion.x + vector2_1.x) / x, (float) (1.0 - ((double) uvRegion.y + (double) uvRegion.height + (double) vector2_1.y) / (double) y));
      Vector2 vector2_3 = new Vector2((uvRegion.x + uvRegion.width - vector2_1.x) / x, (float) (1.0 - ((double) uvRegion.y - (double) vector2_1.y) / (double) y));
      Vector2 vector2_4 = new Vector2(trimRect.x - anchor.x, -trimRect.y + anchor.y);
      if (rotated)
        vector2_4.y -= width;
      vector2_4 *= scale;
      Vector3 vector3_1 = new Vector3(-anchor.x * scale, anchor.y * scale, 0.0f);
      Vector3 vector3_2 = vector3_1 + new Vector3(trimRect.width * scale, -trimRect.height * scale, 0.0f);
      Vector3 vector3_3 = new Vector3(0.0f, -height * scale, 0.0f);
      Vector3 vector3_4 = vector3_3 + new Vector3(width * scale, height * scale, 0.0f);
      if (rotated)
      {
        forRegionInTexture.position0 = new Vector3(-vector3_4.y + vector2_4.x, vector3_3.x + vector2_4.y, 0.0f);
        forRegionInTexture.position1 = new Vector3(-vector3_3.y + vector2_4.x, vector3_3.x + vector2_4.y, 0.0f);
        forRegionInTexture.position2 = new Vector3(-vector3_4.y + vector2_4.x, vector3_4.x + vector2_4.y, 0.0f);
        forRegionInTexture.position3 = new Vector3(-vector3_3.y + vector2_4.x, vector3_4.x + vector2_4.y, 0.0f);
        forRegionInTexture.uvs = new Vector2[4]
        {
          new Vector2(vector2_2.x, vector2_3.y),
          new Vector2(vector2_2.x, vector2_2.y),
          new Vector2(vector2_3.x, vector2_3.y),
          new Vector2(vector2_3.x, vector2_2.y)
        };
      }
      else
      {
        forRegionInTexture.position0 = new Vector3(vector3_3.x + vector2_4.x, vector3_3.y + vector2_4.y, 0.0f);
        forRegionInTexture.position1 = new Vector3(vector3_4.x + vector2_4.x, vector3_3.y + vector2_4.y, 0.0f);
        forRegionInTexture.position2 = new Vector3(vector3_3.x + vector2_4.x, vector3_4.y + vector2_4.y, 0.0f);
        forRegionInTexture.position3 = new Vector3(vector3_4.x + vector2_4.x, vector3_4.y + vector2_4.y, 0.0f);
        forRegionInTexture.uvs = new Vector2[4]
        {
          new Vector2(vector2_2.x, vector2_2.y),
          new Vector2(vector2_3.x, vector2_2.y),
          new Vector2(vector2_2.x, vector2_3.y),
          new Vector2(vector2_3.x, vector2_3.y)
        };
      }
      forRegionInTexture.normals = new Vector3[0];
      forRegionInTexture.tangents = new Vector4[0];
      forRegionInTexture.indices = new int[6]
      {
        0,
        3,
        1,
        2,
        3,
        0
      };
      Vector3 vector3_5 = new Vector3(vector3_1.x, vector3_2.y, 0.0f);
      Vector3 vector3_6 = new Vector3(vector3_2.x, vector3_1.y, 0.0f);
      forRegionInTexture.boundsDataCenter = (vector3_6 + vector3_5) / 2f;
      forRegionInTexture.boundsDataExtents = vector3_6 - vector3_5;
      forRegionInTexture.untrimmedBoundsDataCenter = (vector3_6 + vector3_5) / 2f;
      forRegionInTexture.untrimmedBoundsDataExtents = vector3_6 - vector3_5;
      forRegionInTexture.texelSize = new Vector2(scale, scale);
      return forRegionInTexture;
    }

    public static tk2dSpriteCollectionData CreateFromTexturePacker(
      tk2dSpriteCollectionSize spriteCollectionSize,
      string texturePackerFileContents,
      Texture texture)
    {
      List<string> stringList = new List<string>();
      List<Rect> rectList1 = new List<Rect>();
      List<Rect> rectList2 = new List<Rect>();
      List<Vector2> vector2List = new List<Vector2>();
      List<bool> boolList = new List<bool>();
      int num = 0;
      TextReader textReader = (TextReader) new StringReader(texturePackerFileContents);
      bool flag1 = false;
      bool flag2 = false;
      string str1 = string.Empty;
      Rect rect1 = new Rect();
      Rect rect2 = new Rect();
      Vector2 zero1 = Vector2.zero;
      Vector2 zero2 = Vector2.zero;
      for (string str2 = textReader.ReadLine(); str2 != null; str2 = textReader.ReadLine())
      {
        if (str2.Length > 0)
        {
          char ch = str2[0];
          switch (num)
          {
            case 0:
              switch (ch)
              {
                case 'h':
                  zero1.y = (float) int.Parse(str2.Substring(2));
                  continue;
                case 'w':
                  zero1.x = (float) int.Parse(str2.Substring(2));
                  continue;
                case '~':
                  ++num;
                  continue;
                default:
                  continue;
              }
            case 1:
              switch (ch)
              {
                case 'n':
                  str1 = str2.Substring(2);
                  continue;
                case 'o':
                  string[] strArray1 = str2.Split();
                  rect2.Set((float) int.Parse(strArray1[1]), (float) int.Parse(strArray1[2]), (float) int.Parse(strArray1[3]), (float) int.Parse(strArray1[4]));
                  flag2 = true;
                  continue;
                case 'r':
                  flag1 = int.Parse(str2.Substring(2)) == 1;
                  continue;
                case 's':
                  string[] strArray2 = str2.Split();
                  rect1.Set((float) int.Parse(strArray2[1]), (float) int.Parse(strArray2[2]), (float) int.Parse(strArray2[3]), (float) int.Parse(strArray2[4]));
                  continue;
                default:
                  if (ch == '~')
                  {
                    stringList.Add(str1);
                    boolList.Add(flag1);
                    rectList1.Add(rect1);
                    if (!flag2)
                    {
                      if (flag1)
                        rect2.Set(0.0f, 0.0f, rect1.height, rect1.width);
                      else
                        rect2.Set(0.0f, 0.0f, rect1.width, rect1.height);
                    }
                    rectList2.Add(rect2);
                    zero2.Set((float) (int) ((double) rect2.width / 2.0), (float) (int) ((double) rect2.height / 2.0));
                    vector2List.Add(zero2);
                    str1 = string.Empty;
                    flag2 = false;
                    flag1 = false;
                    continue;
                  }
                  continue;
              }
            default:
              continue;
          }
        }
      }
      return SpriteCollectionGenerator.CreateFromTexture(texture, spriteCollectionSize, zero1, stringList.ToArray(), rectList1.ToArray(), rectList2.ToArray(), vector2List.ToArray(), boolList.ToArray());
    }
  }
}
