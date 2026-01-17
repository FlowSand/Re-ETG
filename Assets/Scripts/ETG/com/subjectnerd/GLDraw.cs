// Decompiled with JetBrains decompiler
// Type: com.subjectnerd.GLDraw
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace com.subjectnerd
{
  public class GLDraw
  {
    protected static bool clippingEnabled;
    protected static Rect clippingBounds;
    public static Material lineMaterial;

    protected static bool clip_test(float p, float q, ref float u1, ref float u2)
    {
      bool flag = true;
      if ((double) p < 0.0)
      {
        float num = q / p;
        if ((double) num > (double) u2)
          flag = false;
        else if ((double) num > (double) u1)
          u1 = num;
      }
      else if ((double) p > 0.0)
      {
        float num = q / p;
        if ((double) num < (double) u1)
          flag = false;
        else if ((double) num < (double) u2)
          u2 = num;
      }
      else if ((double) q < 0.0)
        flag = false;
      return flag;
    }

    protected static bool segment_rect_intersection(Rect bounds, ref Vector2 p1, ref Vector2 p2)
    {
      float u1 = 0.0f;
      float u2 = 1f;
      float p3 = p2.x - p1.x;
      if (GLDraw.clip_test(-p3, p1.x - bounds.xMin, ref u1, ref u2) && GLDraw.clip_test(p3, bounds.xMax - p1.x, ref u1, ref u2))
      {
        float p4 = p2.y - p1.y;
        if (GLDraw.clip_test(-p4, p1.y - bounds.yMin, ref u1, ref u2) && GLDraw.clip_test(p4, bounds.yMax - p1.y, ref u1, ref u2))
        {
          if ((double) u2 < 1.0)
          {
            p2.x = p1.x + u2 * p3;
            p2.y = p1.y + u2 * p4;
          }
          if ((double) u1 > 0.0)
          {
            p1.x += u1 * p3;
            p1.y += u1 * p4;
          }
          return true;
        }
      }
      return false;
    }

    public static void BeginGroup(Rect position)
    {
      GLDraw.clippingEnabled = true;
      GLDraw.clippingBounds = new Rect(0.0f, 0.0f, position.width, position.height);
      GUI.BeginGroup(position);
    }

    public static void EndGroup()
    {
      GUI.EndGroup();
      GLDraw.clippingBounds = new Rect(0.0f, 0.0f, (float) Screen.width, (float) Screen.height);
      GLDraw.clippingEnabled = false;
    }

    public static void CreateMaterial()
    {
      if (!((Object) GLDraw.lineMaterial == (Object) null))
        return;
      GLDraw.lineMaterial = new Material(ShaderCache.Acquire("Brave/DebugLines"));
      GLDraw.lineMaterial.hideFlags = HideFlags.HideAndDontSave;
      GLDraw.lineMaterial.shader.hideFlags = HideFlags.None;
    }

    public static void DrawLine(Vector2 start, Vector2 end, Color color, float width)
    {
      if (Event.current == null || Event.current.type != EventType.Repaint || GLDraw.clippingEnabled && !GLDraw.segment_rect_intersection(GLDraw.clippingBounds, ref start, ref end))
        return;
      GLDraw.CreateMaterial();
      GLDraw.lineMaterial.SetPass(0);
      if ((double) width == 1.0)
      {
        GL.Begin(1);
        GL.Color(color);
        Vector3 v1 = new Vector3(start.x, start.y, 0.0f);
        Vector3 v2 = new Vector3(end.x, end.y, 0.0f);
        GL.Vertex(v1);
        GL.Vertex(v2);
      }
      else
      {
        GL.Begin(7);
        GL.Color(color);
        Vector3 vector3_1 = (new Vector3(end.y, start.x, 0.0f) - new Vector3(start.y, end.x, 0.0f)).normalized * width;
        Vector3 vector3_2 = new Vector3(start.x, start.y, 0.0f);
        Vector3 vector3_3 = new Vector3(end.x, end.y, 0.0f);
        GL.Vertex(vector3_2 - vector3_1);
        GL.Vertex(vector3_2 + vector3_1);
        GL.Vertex(vector3_3 + vector3_1);
        GL.Vertex(vector3_3 - vector3_1);
      }
      GL.End();
    }

    public static void DrawBox(Rect box, Color color, float width)
    {
      Vector2 vector2_1 = new Vector2(box.xMin, box.yMin);
      Vector2 vector2_2 = new Vector2(box.xMax, box.yMin);
      Vector2 vector2_3 = new Vector2(box.xMax, box.yMax);
      Vector2 vector2_4 = new Vector2(box.xMin, box.yMax);
      GLDraw.DrawLine(vector2_1, vector2_2, color, width);
      GLDraw.DrawLine(vector2_2, vector2_3, color, width);
      GLDraw.DrawLine(vector2_3, vector2_4, color, width);
      GLDraw.DrawLine(vector2_4, vector2_1, color, width);
    }

    public static void DrawBox(
      Vector2 topLeftCorner,
      Vector2 bottomRightCorner,
      Color color,
      float width)
    {
      GLDraw.DrawBox(new Rect(topLeftCorner.x, topLeftCorner.y, bottomRightCorner.x - topLeftCorner.x, bottomRightCorner.y - topLeftCorner.y), color, width);
    }

    public static void DrawRoundedBox(Rect box, float radius, Color color, float width)
    {
      Vector2 vector2_1 = new Vector2(box.xMin + radius, box.yMin);
      Vector2 vector2_2 = new Vector2(box.xMax - radius, box.yMin);
      Vector2 vector2_3 = new Vector2(box.xMax, box.yMin + radius);
      Vector2 vector2_4 = new Vector2(box.xMax, box.yMax - radius);
      Vector2 vector2_5 = new Vector2(box.xMax - radius, box.yMax);
      Vector2 vector2_6 = new Vector2(box.xMin + radius, box.yMax);
      Vector2 vector2_7 = new Vector2(box.xMin, box.yMax - radius);
      Vector2 vector2_8 = new Vector2(box.xMin, box.yMin + radius);
      GLDraw.DrawLine(vector2_1, vector2_2, color, width);
      GLDraw.DrawLine(vector2_3, vector2_4, color, width);
      GLDraw.DrawLine(vector2_5, vector2_6, color, width);
      GLDraw.DrawLine(vector2_7, vector2_8, color, width);
      float num = radius / 2f;
      Vector2 startTangent = new Vector2(vector2_8.x, vector2_8.y + num);
      Vector2 endTangent = new Vector2(vector2_1.x - num, vector2_1.y);
      GLDraw.DrawBezier(vector2_8, startTangent, vector2_1, endTangent, color, width);
      startTangent = new Vector2(vector2_2.x + num, vector2_2.y);
      endTangent = new Vector2(vector2_3.x, vector2_3.y - num);
      GLDraw.DrawBezier(vector2_2, startTangent, vector2_3, endTangent, color, width);
      startTangent = new Vector2(vector2_4.x, vector2_4.y + num);
      endTangent = new Vector2(vector2_5.x + num, vector2_5.y);
      GLDraw.DrawBezier(vector2_4, startTangent, vector2_5, endTangent, color, width);
      startTangent = new Vector2(vector2_6.x - num, vector2_6.y);
      endTangent = new Vector2(vector2_7.x, vector2_7.y + num);
      GLDraw.DrawBezier(vector2_6, startTangent, vector2_7, endTangent, color, width);
    }

    public static void DrawConnectingCurve(Vector2 start, Vector2 end, Color color, float width)
    {
      Vector2 vector2 = start - end;
      Vector2 startTangent = start;
      startTangent.x -= (vector2 / 2f).x;
      Vector2 endTangent = end;
      endTangent.x += (vector2 / 2f).x;
      int segments = Mathf.FloorToInt((float) ((double) vector2.magnitude / 20.0 * 3.0));
      GLDraw.DrawBezier(start, startTangent, end, endTangent, color, width, segments);
    }

    public static void DrawBezier(
      Vector2 start,
      Vector2 startTangent,
      Vector2 end,
      Vector2 endTangent,
      Color color,
      float width)
    {
      int segments = Mathf.FloorToInt((start - end).magnitude / 20f) * 3;
      GLDraw.DrawBezier(start, startTangent, end, endTangent, color, width, segments);
    }

    public static void DrawBezier(
      Vector2 start,
      Vector2 startTangent,
      Vector2 end,
      Vector2 endTangent,
      Color color,
      float width,
      int segments)
    {
      Vector2 start1 = GLDraw.CubeBezier(start, startTangent, end, endTangent, 0.0f);
      for (int index = 1; index <= segments; ++index)
      {
        Vector2 end1 = GLDraw.CubeBezier(start, startTangent, end, endTangent, (float) index / (float) segments);
        GLDraw.DrawLine(start1, end1, color, width);
        start1 = end1;
      }
    }

    private static Vector2 CubeBezier(Vector2 s, Vector2 st, Vector2 e, Vector2 et, float t)
    {
      float num1 = 1f - t;
      float num2 = num1 * t;
      return num1 * num1 * num1 * s + 3f * num1 * num2 * st + 3f * num2 * t * et + t * t * t * e;
    }
  }
}
