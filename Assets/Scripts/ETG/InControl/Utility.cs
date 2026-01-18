using Microsoft.Win32;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

#nullable disable
namespace InControl
{
  public static class Utility
  {
    public const float Epsilon = 1E-07f;
    private static Vector2[] circleVertexList = new Vector2[25]
    {
      new Vector2(0.0f, 1f),
      new Vector2(0.2588f, 0.9659f),
      new Vector2(0.5f, 0.866f),
      new Vector2(0.7071f, 0.7071f),
      new Vector2(0.866f, 0.5f),
      new Vector2(0.9659f, 0.2588f),
      new Vector2(1f, 0.0f),
      new Vector2(0.9659f, -0.2588f),
      new Vector2(0.866f, -0.5f),
      new Vector2(0.7071f, -0.7071f),
      new Vector2(0.5f, -0.866f),
      new Vector2(0.2588f, -0.9659f),
      new Vector2(0.0f, -1f),
      new Vector2(-0.2588f, -0.9659f),
      new Vector2(-0.5f, -0.866f),
      new Vector2(-0.7071f, -0.7071f),
      new Vector2(-0.866f, -0.5f),
      new Vector2(-0.9659f, -0.2588f),
      new Vector2(-1f, -0.0f),
      new Vector2(-0.9659f, 0.2588f),
      new Vector2(-0.866f, 0.5f),
      new Vector2(-0.7071f, 0.7071f),
      new Vector2(-0.5f, 0.866f),
      new Vector2(-0.2588f, 0.9659f),
      new Vector2(0.0f, 1f)
    };

    public static void DrawCircleGizmo(Vector2 center, float radius)
    {
      Vector2 from = Utility.circleVertexList[0] * radius + center;
      int length = Utility.circleVertexList.Length;
      for (int index = 1; index < length; ++index)
        Gizmos.DrawLine((Vector3) from, (Vector3) (from = Utility.circleVertexList[index] * radius + center));
    }

    public static void DrawCircleGizmo(Vector2 center, float radius, Color color)
    {
      Gizmos.color = color;
      Utility.DrawCircleGizmo(center, radius);
    }

    public static void DrawOvalGizmo(Vector2 center, Vector2 size)
    {
      Vector2 b = size / 2f;
      Vector2 from = Vector2.Scale(Utility.circleVertexList[0], b) + center;
      int length = Utility.circleVertexList.Length;
      for (int index = 1; index < length; ++index)
        Gizmos.DrawLine((Vector3) from, (Vector3) (from = Vector2.Scale(Utility.circleVertexList[index], b) + center));
    }

    public static void DrawOvalGizmo(Vector2 center, Vector2 size, Color color)
    {
      Gizmos.color = color;
      Utility.DrawOvalGizmo(center, size);
    }

    public static void DrawRectGizmo(Rect rect)
    {
      Vector3 vector3_1 = new Vector3(rect.xMin, rect.yMin);
      Vector3 vector3_2 = new Vector3(rect.xMax, rect.yMin);
      Vector3 vector3_3 = new Vector3(rect.xMax, rect.yMax);
      Vector3 vector3_4 = new Vector3(rect.xMin, rect.yMax);
      Gizmos.DrawLine(vector3_1, vector3_2);
      Gizmos.DrawLine(vector3_2, vector3_3);
      Gizmos.DrawLine(vector3_3, vector3_4);
      Gizmos.DrawLine(vector3_4, vector3_1);
    }

    public static void DrawRectGizmo(Rect rect, Color color)
    {
      Gizmos.color = color;
      Utility.DrawRectGizmo(rect);
    }

    public static void DrawRectGizmo(Vector2 center, Vector2 size)
    {
      float num1 = size.x / 2f;
      float num2 = size.y / 2f;
      Vector3 vector3_1 = new Vector3(center.x - num1, center.y - num2);
      Vector3 vector3_2 = new Vector3(center.x + num1, center.y - num2);
      Vector3 vector3_3 = new Vector3(center.x + num1, center.y + num2);
      Vector3 vector3_4 = new Vector3(center.x - num1, center.y + num2);
      Gizmos.DrawLine(vector3_1, vector3_2);
      Gizmos.DrawLine(vector3_2, vector3_3);
      Gizmos.DrawLine(vector3_3, vector3_4);
      Gizmos.DrawLine(vector3_4, vector3_1);
    }

    public static void DrawRectGizmo(Vector2 center, Vector2 size, Color color)
    {
      Gizmos.color = color;
      Utility.DrawRectGizmo(center, size);
    }

    public static bool GameObjectIsCulledOnCurrentCamera(GameObject gameObject)
    {
      return (Camera.current.cullingMask & 1 << gameObject.layer) == 0;
    }

    public static Color MoveColorTowards(Color color0, Color color1, float maxDelta)
    {
      return new Color(Mathf.MoveTowards(color0.r, color1.r, maxDelta), Mathf.MoveTowards(color0.g, color1.g, maxDelta), Mathf.MoveTowards(color0.b, color1.b, maxDelta), Mathf.MoveTowards(color0.a, color1.a, maxDelta));
    }

    public static float ApplyDeadZone(float value, float lowerDeadZone, float upperDeadZone)
    {
      if ((double) value < 0.0)
      {
        if ((double) value > -(double) lowerDeadZone)
          return 0.0f;
        return (double) value < -(double) upperDeadZone ? -1f : (float) (((double) value + (double) lowerDeadZone) / ((double) upperDeadZone - (double) lowerDeadZone));
      }
      if ((double) value < (double) lowerDeadZone)
        return 0.0f;
      return (double) value > (double) upperDeadZone ? 1f : (float) (((double) value - (double) lowerDeadZone) / ((double) upperDeadZone - (double) lowerDeadZone));
    }

    public static Vector2 ApplySeparateDeadZone(
      float x,
      float y,
      float lowerDeadZone,
      float upperDeadZone)
    {
      return new Vector2(Utility.ApplyDeadZone(x, lowerDeadZone, upperDeadZone), Utility.ApplyDeadZone(y, lowerDeadZone, upperDeadZone)).normalized;
    }

    public static Vector2 ApplyCircularDeadZone(Vector2 v, float lowerDeadZone, float upperDeadZone)
    {
      float magnitude = v.magnitude;
      if ((double) magnitude < (double) lowerDeadZone)
        return Vector2.zero;
      return (double) magnitude > (double) upperDeadZone ? v.normalized : v.normalized * (float) (((double) magnitude - (double) lowerDeadZone) / ((double) upperDeadZone - (double) lowerDeadZone));
    }

    public static Vector2 ApplyCircularDeadZone(
      float x,
      float y,
      float lowerDeadZone,
      float upperDeadZone)
    {
      return Utility.ApplyCircularDeadZone(new Vector2(x, y), lowerDeadZone, upperDeadZone);
    }

    public static float ApplySmoothing(
      float thisValue,
      float lastValue,
      float deltaTime,
      float sensitivity)
    {
      if (Utility.Approximately(sensitivity, 1f))
        return thisValue;
      float maxDelta = (float) ((double) deltaTime * (double) sensitivity * 100.0);
      if (Utility.IsNotZero(thisValue) && (double) Mathf.Sign(lastValue) != (double) Mathf.Sign(thisValue))
        lastValue = 0.0f;
      return Mathf.MoveTowards(lastValue, thisValue, maxDelta);
    }

    public static float ApplySnapping(float value, float threshold)
    {
      if ((double) value < -(double) threshold)
        return -1f;
      return (double) value > (double) threshold ? 1f : 0.0f;
    }

    internal static bool TargetIsButton(InputControlType target)
    {
      if (target >= InputControlType.Action1 && target <= InputControlType.Action12)
        return true;
      return target >= InputControlType.Button0 && target <= InputControlType.Button19;
    }

    internal static bool TargetIsStandard(InputControlType target)
    {
      if (target >= InputControlType.LeftStickUp && target <= InputControlType.Action12)
        return true;
      return target >= InputControlType.Command && target <= InputControlType.DPadY;
    }

    internal static bool TargetIsAlias(InputControlType target)
    {
      return target >= InputControlType.Command && target <= InputControlType.DPadY;
    }

    public static string ReadFromFile(string path)
    {
      StreamReader streamReader = new StreamReader(path);
      string end = streamReader.ReadToEnd();
      streamReader.Close();
      return end;
    }

    public static void WriteToFile(string path, string data)
    {
      StreamWriter streamWriter = new StreamWriter(path);
      streamWriter.Write(data);
      streamWriter.Flush();
      streamWriter.Close();
    }

    public static float Abs(float value) => (double) value < 0.0 ? -value : value;

    public static bool Approximately(float v1, float v2)
    {
      float num = v1 - v2;
      return (double) num >= -1.0000000116860974E-07 && (double) num <= 1.0000000116860974E-07;
    }

    public static bool Approximately(Vector2 v1, Vector2 v2)
    {
      return Utility.Approximately(v1.x, v2.x) && Utility.Approximately(v1.y, v2.y);
    }

    public static bool IsNotZero(float value)
    {
      return (double) value < -1.0000000116860974E-07 || (double) value > 1.0000000116860974E-07;
    }

    public static bool IsZero(float value)
    {
      return (double) value >= -1.0000000116860974E-07 && (double) value <= 1.0000000116860974E-07;
    }

    public static bool AbsoluteIsOverThreshold(float value, float threshold)
    {
      return (double) value < -(double) threshold || (double) value > (double) threshold;
    }

    public static float NormalizeAngle(float angle)
    {
      while ((double) angle < 0.0)
        angle += 360f;
      while ((double) angle > 360.0)
        angle -= 360f;
      return angle;
    }

    public static float VectorToAngle(Vector2 vector)
    {
      return Utility.IsZero(vector.x) && Utility.IsZero(vector.y) ? 0.0f : Utility.NormalizeAngle(Mathf.Atan2(vector.x, vector.y) * 57.29578f);
    }

    public static float Min(float v0, float v1) => (double) v0 >= (double) v1 ? v1 : v0;

    public static float Max(float v0, float v1) => (double) v0 <= (double) v1 ? v1 : v0;

    public static float Min(float v0, float v1, float v2, float v3)
    {
      float num1 = (double) v0 < (double) v1 ? v0 : v1;
      float num2 = (double) v2 < (double) v3 ? v2 : v3;
      return (double) num1 >= (double) num2 ? num2 : num1;
    }

    public static float Max(float v0, float v1, float v2, float v3)
    {
      float num1 = (double) v0 > (double) v1 ? v0 : v1;
      float num2 = (double) v2 > (double) v3 ? v2 : v3;
      return (double) num1 <= (double) num2 ? num2 : num1;
    }

    internal static float ValueFromSides(float negativeSide, float positiveSide)
    {
      float v1 = Utility.Abs(negativeSide);
      float v2 = Utility.Abs(positiveSide);
      if (Utility.Approximately(v1, v2))
        return 0.0f;
      return (double) v1 > (double) v2 ? -v1 : v2;
    }

    internal static float ValueFromSides(float negativeSide, float positiveSide, bool invertSides)
    {
      return invertSides ? Utility.ValueFromSides(positiveSide, negativeSide) : Utility.ValueFromSides(negativeSide, positiveSide);
    }

    public static void ArrayResize<T>(ref T[] array, int capacity)
    {
      if (array != null && capacity <= array.Length)
        return;
      Array.Resize<T>(ref array, Utility.NextPowerOfTwo(capacity));
    }

    public static void ArrayExpand<T>(ref T[] array, int capacity)
    {
      if (array != null && capacity <= array.Length)
        return;
      array = new T[Utility.NextPowerOfTwo(capacity)];
    }

    public static int NextPowerOfTwo(int value)
    {
      if (value <= 0)
        return 0;
      --value;
      value |= value >> 1;
      value |= value >> 2;
      value |= value >> 4;
      value |= value >> 8;
      value |= value >> 16 /*0x10*/;
      ++value;
      return value;
    }

    internal static bool Is32Bit => IntPtr.Size == 4;

    internal static bool Is64Bit => IntPtr.Size == 8;

    public static string HKLM_GetString(string path, string key)
    {
      try
      {
        RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(path);
        return registryKey == null ? string.Empty : (string) registryKey.GetValue(key);
      }
      catch
      {
        return (string) null;
      }
    }

    public static string GetWindowsVersion()
    {
      string str1 = Utility.HKLM_GetString("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion", "ProductName");
      if (str1 == null)
        return SystemInfo.operatingSystem;
      string str2 = Utility.HKLM_GetString("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion", "CSDVersion");
      string str3 = !Utility.Is32Bit ? "64Bit" : "32Bit";
      int systemBuildNumber = Utility.GetSystemBuildNumber();
      return $"{str1}{(str2 == null ? (object) string.Empty : (object) (" " + str2))} {str3} Build {(object) systemBuildNumber}";
    }

    public static int GetSystemBuildNumber() => Environment.OSVersion.Version.Build;

    internal static void LoadScene(string sceneName) => SceneManager.LoadScene(sceneName);

    internal static string PluginFileExtension() => ".dll";
  }
}
