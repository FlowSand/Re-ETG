// Decompiled with JetBrains decompiler
// Type: tk2dTextGeomGen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public static class tk2dTextGeomGen
    {
      private static tk2dTextGeomGen.GeomData tmpData = new tk2dTextGeomGen.GeomData();
      private static readonly Color32[] channelSelectColors = new Color32[4]
      {
        new Color32((byte) 0, (byte) 0, byte.MaxValue, (byte) 0),
        (Color32) new Color(0.0f, (float) byte.MaxValue, 0.0f, 0.0f),
        (Color32) new Color((float) byte.MaxValue, 0.0f, 0.0f, 0.0f),
        (Color32) new Color(0.0f, 0.0f, 0.0f, (float) byte.MaxValue)
      };
      private static Color32 meshTopColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
      private static Color32 meshBottomColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
      private static float meshGradientTexU = 0.0f;
      private static int curGradientCount = 1;
      private static Color32 errorColor = new Color32(byte.MaxValue, (byte) 0, byte.MaxValue, byte.MaxValue);
      public static List<Vector3> inlineSpriteOffsetsForLastString;

      public static tk2dTextGeomGen.GeomData Data(
        tk2dTextMeshData textMeshData,
        tk2dFontData fontData,
        string formattedText)
      {
        tk2dTextGeomGen.tmpData.textMeshData = textMeshData;
        tk2dTextGeomGen.tmpData.fontInst = fontData;
        tk2dTextGeomGen.tmpData.formattedText = formattedText;
        return tk2dTextGeomGen.tmpData;
      }

      public static Vector2 GetMeshDimensionsForString(string str, tk2dTextGeomGen.GeomData geomData)
      {
        tk2dTextMeshData textMeshData = geomData.textMeshData;
        tk2dFontData fontInst = geomData.fontInst;
        float b = 0.0f;
        float a = 0.0f;
        float num1 = 0.0f;
        bool flag1 = false;
        int num2 = 0;
        for (int index = 0; index < str.Length && num2 < textMeshData.maxChars; ++index)
        {
          if (flag1)
          {
            flag1 = false;
          }
          else
          {
            int key = (int) str[index];
            if (key == 10)
            {
              b = Mathf.Max(a, b);
              a = 0.0f;
              num1 -= (fontInst.lineHeight + textMeshData.lineSpacing) * textMeshData.scale.y;
            }
            else
            {
              if (textMeshData.inlineStyling && key == 94 && index + 1 < str.Length)
              {
                if (str[index + 1] == '^')
                {
                  flag1 = true;
                }
                else
                {
                  int num3 = 0;
                  switch (str[index + 1])
                  {
                    case 'C':
                      num3 = 9;
                      break;
                    case 'G':
                      num3 = 17;
                      break;
                    case 'c':
                      num3 = 5;
                      break;
                    case 'g':
                      num3 = 9;
                      break;
                  }
                  index += num3;
                  continue;
                }
              }
              bool flag2 = key == 94;
              tk2dFontChar tk2dFontChar;
              if (fontInst.useDictionary)
              {
                if (!fontInst.charDict.ContainsKey(key))
                  key = 0;
                tk2dFontChar = fontInst.charDict[key];
              }
              else
              {
                if (key >= fontInst.chars.Length)
                  key = 0;
                tk2dFontChar = fontInst.chars[key];
              }
              if (flag2)
                ;
              a += (tk2dFontChar.advance + textMeshData.spacing) * textMeshData.scale.x;
              if (textMeshData.kerning && index < str.Length - 1)
              {
                foreach (tk2dFontKerning tk2dFontKerning in fontInst.kerning)
                {
                  if (tk2dFontKerning.c0 == (int) str[index] && tk2dFontKerning.c1 == (int) str[index + 1])
                  {
                    a += tk2dFontKerning.amount * textMeshData.scale.x;
                    break;
                  }
                }
              }
              ++num2;
            }
          }
        }
        return new Vector2(Mathf.Max(a, b), num1 - (fontInst.lineHeight + textMeshData.lineSpacing) * textMeshData.scale.y);
      }

      public static float GetYAnchorForHeight(float textHeight, tk2dTextGeomGen.GeomData geomData)
      {
        tk2dTextMeshData textMeshData = geomData.textMeshData;
        tk2dFontData fontInst = geomData.fontInst;
        int num1 = (int) textMeshData.anchor / 3;
        float num2 = (fontInst.lineHeight + textMeshData.lineSpacing) * textMeshData.scale.y;
        switch (num1)
        {
          case 0:
            return -num2;
          case 1:
            float yanchorForHeight = (float) (-(double) textHeight / 2.0) - num2;
            if (fontInst.version < 2)
              return yanchorForHeight;
            float num3 = fontInst.texelSize.y * textMeshData.scale.y;
            return Mathf.Floor(yanchorForHeight / num3) * num3;
          case 2:
            return -textHeight - num2;
          default:
            return -num2;
        }
      }

      public static float GetXAnchorForWidth(float lineWidth, tk2dTextGeomGen.GeomData geomData)
      {
        tk2dTextMeshData textMeshData = geomData.textMeshData;
        tk2dFontData fontInst = geomData.fontInst;
        switch ((int) textMeshData.anchor % 3)
        {
          case 0:
            return 0.0f;
          case 1:
            float xanchorForWidth = (float) (-(double) lineWidth / 2.0);
            if (fontInst.version < 2)
              return xanchorForWidth;
            float num = fontInst.texelSize.x * textMeshData.scale.x;
            return Mathf.Floor(xanchorForWidth / num) * num;
          case 2:
            return -lineWidth;
          default:
            return 0.0f;
        }
      }

      private static void PostAlignTextData(
        Vector3[] pos,
        int offset,
        int targetStart,
        int targetEnd,
        float offsetX,
        List<int> inlineSpritePositions = null)
      {
        for (int index = targetStart * 4; index < targetEnd * 4; ++index)
        {
          Vector3 po = pos[offset + index];
          po.x += offsetX;
          pos[offset + index] = po;
        }
        if (inlineSpritePositions == null)
          return;
        for (int index = 0; index < inlineSpritePositions.Count; ++index)
          tk2dTextGeomGen.inlineSpriteOffsetsForLastString[inlineSpritePositions[index]] += new Vector3(offsetX, 0.0f, 0.0f);
      }

      private static int GetFullHexColorComponent(int c1, int c2)
      {
        int num1 = 0;
        int num2;
        if (c1 >= 48 /*0x30*/ && c1 <= 57)
          num2 = num1 + (c1 - 48 /*0x30*/) * 16 /*0x10*/;
        else if (c1 >= 97 && c1 <= 102)
        {
          num2 = num1 + (10 + c1 - 97) * 16 /*0x10*/;
        }
        else
        {
          if (c1 < 65 || c1 > 70)
            return -1;
          num2 = num1 + (10 + c1 - 65) * 16 /*0x10*/;
        }
        if (c2 >= 48 /*0x30*/ && c2 <= 57)
          return num2 + (c2 - 48 /*0x30*/);
        if (c2 >= 97 && c2 <= 102)
          return num2 + (10 + c2 - 97);
        return c2 >= 65 && c2 <= 70 ? num2 + (10 + c2 - 65) : -1;
      }

      private static int GetCompactHexColorComponent(int c)
      {
        if (c >= 48 /*0x30*/ && c <= 57)
          return (c - 48 /*0x30*/) * 17;
        if (c >= 97 && c <= 102)
          return (10 + c - 97) * 17;
        return c >= 65 && c <= 70 ? (10 + c - 65) * 17 : -1;
      }

      private static int GetStyleHexColor(string str, bool fullHex, ref Color32 color)
      {
        int hexColorComponent1;
        int hexColorComponent2;
        int hexColorComponent3;
        int hexColorComponent4;
        if (fullHex)
        {
          if (str.Length < 8)
            return 1;
          hexColorComponent1 = tk2dTextGeomGen.GetFullHexColorComponent((int) str[0], (int) str[1]);
          hexColorComponent2 = tk2dTextGeomGen.GetFullHexColorComponent((int) str[2], (int) str[3]);
          hexColorComponent3 = tk2dTextGeomGen.GetFullHexColorComponent((int) str[4], (int) str[5]);
          hexColorComponent4 = tk2dTextGeomGen.GetFullHexColorComponent((int) str[6], (int) str[7]);
        }
        else
        {
          if (str.Length < 4)
            return 1;
          hexColorComponent1 = tk2dTextGeomGen.GetCompactHexColorComponent((int) str[0]);
          hexColorComponent2 = tk2dTextGeomGen.GetCompactHexColorComponent((int) str[1]);
          hexColorComponent3 = tk2dTextGeomGen.GetCompactHexColorComponent((int) str[2]);
          hexColorComponent4 = tk2dTextGeomGen.GetCompactHexColorComponent((int) str[3]);
        }
        if (hexColorComponent1 == -1 || hexColorComponent2 == -1 || hexColorComponent3 == -1 || hexColorComponent4 == -1)
          return 1;
        color = new Color32((byte) hexColorComponent1, (byte) hexColorComponent2, (byte) hexColorComponent3, (byte) hexColorComponent4);
        return 0;
      }

      private static int SetColorsFromStyleCommand(string args, bool twoColors, bool fullHex)
      {
        int num = (!twoColors ? 1 : 2) * (!fullHex ? 4 : 8);
        bool flag = false;
        if (args.Length >= num)
        {
          if (tk2dTextGeomGen.GetStyleHexColor(args, fullHex, ref tk2dTextGeomGen.meshTopColor) != 0)
            flag = true;
          if (twoColors)
          {
            if (tk2dTextGeomGen.GetStyleHexColor(args.Substring(!fullHex ? 4 : 8), fullHex, ref tk2dTextGeomGen.meshBottomColor) != 0)
              flag = true;
          }
          else
            tk2dTextGeomGen.meshBottomColor = tk2dTextGeomGen.meshTopColor;
        }
        else
          flag = true;
        if (flag)
          tk2dTextGeomGen.meshTopColor = tk2dTextGeomGen.meshBottomColor = tk2dTextGeomGen.errorColor;
        return num;
      }

      private static void SetGradientTexUFromStyleCommand(int arg)
      {
        tk2dTextGeomGen.meshGradientTexU = (float) (arg - 48 /*0x30*/) / (tk2dTextGeomGen.curGradientCount <= 0 ? 1f : (float) tk2dTextGeomGen.curGradientCount);
      }

      private static int HandleStyleCommand(string cmd)
      {
        if (cmd.Length == 0)
          return 0;
        int num1 = (int) cmd[0];
        string args = cmd.Substring(1);
        int num2 = 0;
        switch (num1)
        {
          case 67:
            num2 = 1 + tk2dTextGeomGen.SetColorsFromStyleCommand(args, false, true);
            break;
          case 71:
            num2 = 1 + tk2dTextGeomGen.SetColorsFromStyleCommand(args, true, true);
            break;
          case 99:
            num2 = 1 + tk2dTextGeomGen.SetColorsFromStyleCommand(args, false, false);
            break;
          case 103:
            num2 = 1 + tk2dTextGeomGen.SetColorsFromStyleCommand(args, true, false);
            break;
        }
        if (num1 >= 48 /*0x30*/ && num1 <= 57)
        {
          tk2dTextGeomGen.SetGradientTexUFromStyleCommand(num1);
          num2 = 1;
        }
        return num2;
      }

      public static void GetTextMeshGeomDesc(
        out int numVertices,
        out int numIndices,
        tk2dTextGeomGen.GeomData geomData)
      {
        tk2dTextMeshData textMeshData = geomData.textMeshData;
        numVertices = textMeshData.maxChars * 4;
        numIndices = textMeshData.maxChars * 6;
      }

      public static int SetTextMeshGeom(
        Vector3[] pos,
        Vector2[] uv,
        Vector2[] uv2,
        Color32[] color,
        int offset,
        tk2dTextGeomGen.GeomData geomData,
        int visibleCharacters,
        Vector2[] characterOffsetVectors,
        bool[] rainbowValues)
      {
        tk2dTextMeshData textMeshData = geomData.textMeshData;
        tk2dFontData fontInst = geomData.fontInst;
        string formattedText = geomData.formattedText;
        tk2dTextGeomGen.inlineSpriteOffsetsForLastString = new List<Vector3>();
        tk2dTextGeomGen.meshTopColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
        tk2dTextGeomGen.meshBottomColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
        tk2dTextGeomGen.meshGradientTexU = (float) textMeshData.textureGradient / (fontInst.gradientCount <= 0 ? 1f : (float) fontInst.gradientCount);
        tk2dTextGeomGen.curGradientCount = fontInst.gradientCount;
        float yanchorForHeight = tk2dTextGeomGen.GetYAnchorForHeight(tk2dTextGeomGen.GetMeshDimensionsForString(geomData.formattedText, geomData).y, geomData);
        float num1 = 0.0f;
        float num2 = 0.0f;
        int num3 = 0;
        int targetStart = 0;
        int num4 = 0;
        List<int> inlineSpritePositions = new List<int>();
        bool flag1 = false;
        for (int index = 0; index < rainbowValues.Length; ++index)
          flag1 = flag1 || rainbowValues[index];
        for (int index1 = 0; index1 < formattedText.Length && num3 < textMeshData.maxChars; ++index1)
        {
          int key = (int) formattedText[index1];
          tk2dFontChar tk2dFontChar = (tk2dFontChar) null;
          bool flag2 = false;
          if (key == 91 && index1 < formattedText.Length - 1 && formattedText[index1 + 1] != ']')
          {
            for (int index2 = index1; index2 < formattedText.Length; ++index2)
            {
              if (formattedText[index2] == ']')
              {
                flag2 = true;
                int num5 = index2 - index1;
                tk2dFontChar = tk2dTextGeomGen.GetSpecificSpriteCharDef(formattedText.Substring(index1 + 9, num5 - 10));
                index1 += num5;
                num4 += num5;
                break;
              }
            }
          }
          bool flag3 = key == 94;
          if (!flag2)
          {
            if (fontInst.useDictionary)
            {
              if (!fontInst.charDict.ContainsKey(key))
                key = 0;
              tk2dFontChar = fontInst.charDict[key];
            }
            else
            {
              if (key >= fontInst.chars.Length)
                key = 0;
              tk2dFontChar = fontInst.chars[key];
            }
          }
          if (flag3)
            key = 94;
          if (key == 10)
          {
            float lineWidth = num1;
            int targetEnd = num3;
            if (targetStart != num3)
            {
              float xanchorForWidth = tk2dTextGeomGen.GetXAnchorForWidth(lineWidth, geomData);
              tk2dTextGeomGen.PostAlignTextData(pos, offset, targetStart, targetEnd, xanchorForWidth, inlineSpritePositions);
            }
            targetStart = num3;
            num1 = 0.0f;
            num2 -= (fontInst.lineHeight + textMeshData.lineSpacing) * textMeshData.scale.y;
            inlineSpritePositions.Clear();
          }
          else
          {
            if (textMeshData.inlineStyling && key == 94)
            {
              if (index1 + 1 < formattedText.Length && formattedText[index1 + 1] == '^')
              {
                ++index1;
              }
              else
              {
                index1 += tk2dTextGeomGen.HandleStyleCommand(formattedText.Substring(index1 + 1));
                continue;
              }
            }
            Vector2 vector = characterOffsetVectors[index1];
            vector = BraveUtility.QuantizeVector(vector.ToVector3ZUp(), 32f).XY();
            pos[offset + num3 * 4] = new Vector3(num1 + tk2dFontChar.p0.x * textMeshData.scale.x + vector.x, (float) ((double) yanchorForHeight + (double) num2 + (double) tk2dFontChar.p0.y * (double) textMeshData.scale.y) + vector.y, 0.0f);
            pos[offset + num3 * 4 + 1] = new Vector3(num1 + tk2dFontChar.p1.x * textMeshData.scale.x + vector.x, (float) ((double) yanchorForHeight + (double) num2 + (double) tk2dFontChar.p0.y * (double) textMeshData.scale.y) + vector.y, 0.0f);
            pos[offset + num3 * 4 + 2] = new Vector3(num1 + tk2dFontChar.p0.x * textMeshData.scale.x + vector.x, (float) ((double) yanchorForHeight + (double) num2 + (double) tk2dFontChar.p1.y * (double) textMeshData.scale.y) + vector.y, 0.0f);
            pos[offset + num3 * 4 + 3] = new Vector3(num1 + tk2dFontChar.p1.x * textMeshData.scale.x + vector.x, (float) ((double) yanchorForHeight + (double) num2 + (double) tk2dFontChar.p1.y * (double) textMeshData.scale.y) + vector.y, 0.0f);
            if (flag2)
            {
              tk2dTextGeomGen.inlineSpriteOffsetsForLastString.Add(pos[offset + num3 * 4 + 2]);
              inlineSpritePositions.Add(tk2dTextGeomGen.inlineSpriteOffsetsForLastString.Count - 1);
            }
            if (tk2dFontChar.flipped)
            {
              uv[offset + num3 * 4] = new Vector2(tk2dFontChar.uv1.x, tk2dFontChar.uv1.y);
              uv[offset + num3 * 4 + 1] = new Vector2(tk2dFontChar.uv1.x, tk2dFontChar.uv0.y);
              uv[offset + num3 * 4 + 2] = new Vector2(tk2dFontChar.uv0.x, tk2dFontChar.uv1.y);
              uv[offset + num3 * 4 + 3] = new Vector2(tk2dFontChar.uv0.x, tk2dFontChar.uv0.y);
            }
            else
            {
              uv[offset + num3 * 4] = new Vector2(tk2dFontChar.uv0.x, tk2dFontChar.uv0.y);
              uv[offset + num3 * 4 + 1] = new Vector2(tk2dFontChar.uv1.x, tk2dFontChar.uv0.y);
              uv[offset + num3 * 4 + 2] = new Vector2(tk2dFontChar.uv0.x, tk2dFontChar.uv1.y);
              uv[offset + num3 * 4 + 3] = new Vector2(tk2dFontChar.uv1.x, tk2dFontChar.uv1.y);
            }
            if (fontInst.textureGradients)
            {
              uv2[offset + num3 * 4] = tk2dFontChar.gradientUv[0] + new Vector2(tk2dTextGeomGen.meshGradientTexU, 0.0f);
              uv2[offset + num3 * 4 + 1] = tk2dFontChar.gradientUv[1] + new Vector2(tk2dTextGeomGen.meshGradientTexU, 0.0f);
              uv2[offset + num3 * 4 + 2] = tk2dFontChar.gradientUv[2] + new Vector2(tk2dTextGeomGen.meshGradientTexU, 0.0f);
              uv2[offset + num3 * 4 + 3] = tk2dFontChar.gradientUv[3] + new Vector2(tk2dTextGeomGen.meshGradientTexU, 0.0f);
            }
            if (index1 - num4 > visibleCharacters)
            {
              Color32 clear = (Color32) Color.clear;
              color[offset + num3 * 4] = clear;
              color[offset + num3 * 4 + 1] = clear;
              color[offset + num3 * 4 + 2] = clear;
              color[offset + num3 * 4 + 3] = clear;
            }
            else if (fontInst.isPacked)
            {
              Color32 channelSelectColor = tk2dTextGeomGen.channelSelectColors[tk2dFontChar.channel];
              color[offset + num3 * 4] = channelSelectColor;
              color[offset + num3 * 4 + 1] = channelSelectColor;
              color[offset + num3 * 4 + 2] = channelSelectColor;
              color[offset + num3 * 4 + 3] = channelSelectColor;
            }
            else if (rainbowValues[index1])
            {
              color[offset + num3 * 4] = (Color32) BraveUtility.GetRainbowLerp(UnityEngine.Time.time * 3f);
              color[offset + num3 * 4 + 1] = (Color32) BraveUtility.GetRainbowLerp(UnityEngine.Time.time * 3f);
              color[offset + num3 * 4 + 2] = (Color32) BraveUtility.GetRainbowLerp(UnityEngine.Time.time * 3f);
              color[offset + num3 * 4 + 3] = (Color32) BraveUtility.GetRainbowLerp(UnityEngine.Time.time * 3f);
            }
            else if (flag1)
            {
              color[offset + num3 * 4] = (Color32) Color.black;
              color[offset + num3 * 4 + 1] = (Color32) Color.black;
              color[offset + num3 * 4 + 2] = (Color32) Color.black;
              color[offset + num3 * 4 + 3] = (Color32) Color.black;
            }
            else
            {
              color[offset + num3 * 4] = tk2dTextGeomGen.meshTopColor;
              color[offset + num3 * 4 + 1] = tk2dTextGeomGen.meshTopColor;
              color[offset + num3 * 4 + 2] = tk2dTextGeomGen.meshBottomColor;
              color[offset + num3 * 4 + 3] = tk2dTextGeomGen.meshBottomColor;
            }
            num1 += (tk2dFontChar.advance + textMeshData.spacing) * textMeshData.scale.x;
            if (textMeshData.kerning && index1 < formattedText.Length - 1)
            {
              foreach (tk2dFontKerning tk2dFontKerning in fontInst.kerning)
              {
                if (tk2dFontKerning.c0 == (int) formattedText[index1] && tk2dFontKerning.c1 == (int) formattedText[index1 + 1])
                {
                  num1 += tk2dFontKerning.amount * textMeshData.scale.x;
                  break;
                }
              }
            }
            ++num3;
          }
        }
        if (targetStart != num3)
        {
          float lineWidth = num1;
          int targetEnd = num3;
          float xanchorForWidth = tk2dTextGeomGen.GetXAnchorForWidth(lineWidth, geomData);
          tk2dTextGeomGen.PostAlignTextData(pos, offset, targetStart, targetEnd, xanchorForWidth, inlineSpritePositions);
        }
        for (int index = num3; index < textMeshData.maxChars; ++index)
        {
          pos[offset + index * 4] = pos[offset + index * 4 + 1] = pos[offset + index * 4 + 2] = pos[offset + index * 4 + 3] = Vector3.zero;
          uv[offset + index * 4] = uv[offset + index * 4 + 1] = uv[offset + index * 4 + 2] = uv[offset + index * 4 + 3] = Vector2.zero;
          if (fontInst.textureGradients)
            uv2[offset + index * 4] = uv2[offset + index * 4 + 1] = uv2[offset + index * 4 + 2] = uv2[offset + index * 4 + 3] = Vector2.zero;
          if (!fontInst.isPacked)
          {
            color[offset + index * 4] = color[offset + index * 4 + 1] = tk2dTextGeomGen.meshTopColor;
            color[offset + index * 4 + 2] = color[offset + index * 4 + 3] = tk2dTextGeomGen.meshBottomColor;
          }
          else
            color[offset + index * 4] = color[offset + index * 4 + 1] = color[offset + index * 4 + 2] = color[offset + index * 4 + 3] = (Color32) Color.clear;
        }
        return num3;
      }

      public static tk2dFontChar GetSpecificSpriteCharDef(string substringInsideBrackets)
      {
        tk2dSpriteDefinition spriteDefinition = ((GameObject) ResourceCache.Acquire("ControllerButtonSprite")).GetComponent<tk2dBaseSprite>().Collection.GetSpriteDefinition(substringInsideBrackets);
        if (spriteDefinition == null)
          return tk2dTextGeomGen.GetGenericSpriteCharDef();
        return new tk2dFontChar()
        {
          advance = Mathf.Abs(spriteDefinition.position1.x - spriteDefinition.position0.x),
          channel = 0,
          p0 = new Vector3(0.0f, 11f / 16f, 0.0f),
          p1 = new Vector3(11f / 16f, 0.0f, 0.0f),
          uv0 = Vector3.zero,
          uv1 = Vector3.zero,
          flipped = false
        };
      }

      public static tk2dFontChar GetGenericSpriteCharDef()
      {
        return new tk2dFontChar()
        {
          advance = 13f / 16f,
          channel = 0,
          p0 = new Vector3(0.0f, 11f / 16f, 0.0f),
          p1 = new Vector3(11f / 16f, 0.0f, 0.0f),
          uv0 = Vector3.zero,
          uv1 = Vector3.zero,
          flipped = false
        };
      }

      public static int SetTextMeshGeom(
        Vector3[] pos,
        Vector2[] uv,
        Vector2[] uv2,
        Color32[] color,
        int offset,
        tk2dTextGeomGen.GeomData geomData,
        int visibleCharacters)
      {
        tk2dTextMeshData textMeshData = geomData.textMeshData;
        tk2dFontData fontInst = geomData.fontInst;
        string formattedText = geomData.formattedText;
        tk2dTextGeomGen.inlineSpriteOffsetsForLastString = new List<Vector3>();
        tk2dTextGeomGen.meshTopColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
        tk2dTextGeomGen.meshBottomColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
        tk2dTextGeomGen.meshGradientTexU = (float) textMeshData.textureGradient / (fontInst.gradientCount <= 0 ? 1f : (float) fontInst.gradientCount);
        tk2dTextGeomGen.curGradientCount = fontInst.gradientCount;
        float yanchorForHeight = tk2dTextGeomGen.GetYAnchorForHeight(tk2dTextGeomGen.GetMeshDimensionsForString(geomData.formattedText, geomData).y, geomData);
        float num1 = 0.0f;
        float num2 = 0.0f;
        int num3 = 0;
        int targetStart = 0;
        int num4 = 0;
        List<int> inlineSpritePositions = new List<int>();
        for (int index1 = 0; index1 < formattedText.Length && num3 < textMeshData.maxChars; ++index1)
        {
          int key = (int) formattedText[index1];
          tk2dFontChar tk2dFontChar = (tk2dFontChar) null;
          bool flag1 = false;
          if (key == 91 && index1 < formattedText.Length - 1 && formattedText[index1 + 1] != ']')
          {
            for (int index2 = index1; index2 < formattedText.Length; ++index2)
            {
              if (formattedText[index2] == ']')
              {
                flag1 = true;
                int num5 = index2 - index1;
                tk2dFontChar = tk2dTextGeomGen.GetSpecificSpriteCharDef(formattedText.Substring(index1 + 9, num5 - 10));
                index1 += num5;
                num4 += num5;
                break;
              }
            }
          }
          bool flag2 = key == 94;
          if (!flag1)
          {
            if (fontInst.useDictionary)
            {
              if (!fontInst.charDict.ContainsKey(key))
                key = 0;
              tk2dFontChar = fontInst.charDict[key];
            }
            else
            {
              if (key >= fontInst.chars.Length)
                key = 0;
              tk2dFontChar = fontInst.chars[key];
            }
          }
          if (flag2)
            key = 94;
          if (key == 10)
          {
            float lineWidth = num1;
            int targetEnd = num3;
            if (targetStart != num3)
            {
              float xanchorForWidth = tk2dTextGeomGen.GetXAnchorForWidth(lineWidth, geomData);
              tk2dTextGeomGen.PostAlignTextData(pos, offset, targetStart, targetEnd, xanchorForWidth, inlineSpritePositions);
            }
            targetStart = num3;
            num1 = 0.0f;
            num2 -= (fontInst.lineHeight + textMeshData.lineSpacing) * textMeshData.scale.y;
            inlineSpritePositions.Clear();
          }
          else
          {
            if (textMeshData.inlineStyling && key == 94)
            {
              if (index1 + 1 < formattedText.Length && formattedText[index1 + 1] == '^')
              {
                ++index1;
              }
              else
              {
                index1 += tk2dTextGeomGen.HandleStyleCommand(formattedText.Substring(index1 + 1));
                continue;
              }
            }
            pos[offset + num3 * 4] = new Vector3(num1 + tk2dFontChar.p0.x * textMeshData.scale.x, (float) ((double) yanchorForHeight + (double) num2 + (double) tk2dFontChar.p0.y * (double) textMeshData.scale.y), 0.0f);
            pos[offset + num3 * 4 + 1] = new Vector3(num1 + tk2dFontChar.p1.x * textMeshData.scale.x, (float) ((double) yanchorForHeight + (double) num2 + (double) tk2dFontChar.p0.y * (double) textMeshData.scale.y), 0.0f);
            pos[offset + num3 * 4 + 2] = new Vector3(num1 + tk2dFontChar.p0.x * textMeshData.scale.x, (float) ((double) yanchorForHeight + (double) num2 + (double) tk2dFontChar.p1.y * (double) textMeshData.scale.y), 0.0f);
            pos[offset + num3 * 4 + 3] = new Vector3(num1 + tk2dFontChar.p1.x * textMeshData.scale.x, (float) ((double) yanchorForHeight + (double) num2 + (double) tk2dFontChar.p1.y * (double) textMeshData.scale.y), 0.0f);
            if (flag1)
            {
              tk2dTextGeomGen.inlineSpriteOffsetsForLastString.Add(pos[offset + num3 * 4 + 2]);
              inlineSpritePositions.Add(tk2dTextGeomGen.inlineSpriteOffsetsForLastString.Count - 1);
            }
            if (tk2dFontChar.flipped)
            {
              uv[offset + num3 * 4] = new Vector2(tk2dFontChar.uv1.x, tk2dFontChar.uv1.y);
              uv[offset + num3 * 4 + 1] = new Vector2(tk2dFontChar.uv1.x, tk2dFontChar.uv0.y);
              uv[offset + num3 * 4 + 2] = new Vector2(tk2dFontChar.uv0.x, tk2dFontChar.uv1.y);
              uv[offset + num3 * 4 + 3] = new Vector2(tk2dFontChar.uv0.x, tk2dFontChar.uv0.y);
            }
            else
            {
              uv[offset + num3 * 4] = new Vector2(tk2dFontChar.uv0.x, tk2dFontChar.uv0.y);
              uv[offset + num3 * 4 + 1] = new Vector2(tk2dFontChar.uv1.x, tk2dFontChar.uv0.y);
              uv[offset + num3 * 4 + 2] = new Vector2(tk2dFontChar.uv0.x, tk2dFontChar.uv1.y);
              uv[offset + num3 * 4 + 3] = new Vector2(tk2dFontChar.uv1.x, tk2dFontChar.uv1.y);
            }
            if (fontInst.textureGradients)
            {
              uv2[offset + num3 * 4] = tk2dFontChar.gradientUv[0] + new Vector2(tk2dTextGeomGen.meshGradientTexU, 0.0f);
              uv2[offset + num3 * 4 + 1] = tk2dFontChar.gradientUv[1] + new Vector2(tk2dTextGeomGen.meshGradientTexU, 0.0f);
              uv2[offset + num3 * 4 + 2] = tk2dFontChar.gradientUv[2] + new Vector2(tk2dTextGeomGen.meshGradientTexU, 0.0f);
              uv2[offset + num3 * 4 + 3] = tk2dFontChar.gradientUv[3] + new Vector2(tk2dTextGeomGen.meshGradientTexU, 0.0f);
            }
            if (index1 - num4 > visibleCharacters)
            {
              Color32 clear = (Color32) Color.clear;
              color[offset + num3 * 4] = clear;
              color[offset + num3 * 4 + 1] = clear;
              color[offset + num3 * 4 + 2] = clear;
              color[offset + num3 * 4 + 3] = clear;
            }
            else if (fontInst.isPacked)
            {
              Color32 channelSelectColor = tk2dTextGeomGen.channelSelectColors[tk2dFontChar.channel];
              color[offset + num3 * 4] = channelSelectColor;
              color[offset + num3 * 4 + 1] = channelSelectColor;
              color[offset + num3 * 4 + 2] = channelSelectColor;
              color[offset + num3 * 4 + 3] = channelSelectColor;
            }
            else
            {
              color[offset + num3 * 4] = tk2dTextGeomGen.meshTopColor;
              color[offset + num3 * 4 + 1] = tk2dTextGeomGen.meshTopColor;
              color[offset + num3 * 4 + 2] = tk2dTextGeomGen.meshBottomColor;
              color[offset + num3 * 4 + 3] = tk2dTextGeomGen.meshBottomColor;
            }
            num1 += (tk2dFontChar.advance + textMeshData.spacing) * textMeshData.scale.x;
            if (textMeshData.kerning && index1 < formattedText.Length - 1)
            {
              foreach (tk2dFontKerning tk2dFontKerning in fontInst.kerning)
              {
                if (tk2dFontKerning.c0 == (int) formattedText[index1] && tk2dFontKerning.c1 == (int) formattedText[index1 + 1])
                {
                  num1 += tk2dFontKerning.amount * textMeshData.scale.x;
                  break;
                }
              }
            }
            ++num3;
          }
        }
        if (targetStart != num3)
        {
          float lineWidth = num1;
          int targetEnd = num3;
          float xanchorForWidth = tk2dTextGeomGen.GetXAnchorForWidth(lineWidth, geomData);
          tk2dTextGeomGen.PostAlignTextData(pos, offset, targetStart, targetEnd, xanchorForWidth, inlineSpritePositions);
        }
        for (int index = num3; index < textMeshData.maxChars; ++index)
        {
          pos[offset + index * 4] = pos[offset + index * 4 + 1] = pos[offset + index * 4 + 2] = pos[offset + index * 4 + 3] = Vector3.zero;
          uv[offset + index * 4] = uv[offset + index * 4 + 1] = uv[offset + index * 4 + 2] = uv[offset + index * 4 + 3] = Vector2.zero;
          if (fontInst.textureGradients)
            uv2[offset + index * 4] = uv2[offset + index * 4 + 1] = uv2[offset + index * 4 + 2] = uv2[offset + index * 4 + 3] = Vector2.zero;
          if (!fontInst.isPacked)
          {
            color[offset + index * 4] = color[offset + index * 4 + 1] = tk2dTextGeomGen.meshTopColor;
            color[offset + index * 4 + 2] = color[offset + index * 4 + 3] = tk2dTextGeomGen.meshBottomColor;
          }
          else
            color[offset + index * 4] = color[offset + index * 4 + 1] = color[offset + index * 4 + 2] = color[offset + index * 4 + 3] = (Color32) Color.clear;
        }
        return num3;
      }

      public static void SetTextMeshIndices(
        int[] indices,
        int offset,
        int vStart,
        tk2dTextGeomGen.GeomData geomData,
        int target)
      {
        tk2dTextMeshData textMeshData = geomData.textMeshData;
        for (int index = 0; index < textMeshData.maxChars; ++index)
        {
          indices[offset + index * 6] = vStart + index * 4;
          indices[offset + index * 6 + 1] = vStart + index * 4 + 1;
          indices[offset + index * 6 + 2] = vStart + index * 4 + 3;
          indices[offset + index * 6 + 3] = vStart + index * 4 + 2;
          indices[offset + index * 6 + 4] = vStart + index * 4;
          indices[offset + index * 6 + 5] = vStart + index * 4 + 3;
        }
      }

      public class GeomData
      {
        internal tk2dTextMeshData textMeshData;
        internal tk2dFontData fontInst;
        internal string formattedText = string.Empty;
      }
    }

}
