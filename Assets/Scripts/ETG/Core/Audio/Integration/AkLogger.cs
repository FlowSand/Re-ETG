// Decompiled with JetBrains decompiler
// Type: AkLogger
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Runtime.InteropServices;
using UnityEngine;

#nullable disable

namespace ETG.Core.Audio.Integration
{
    public class AkLogger
    {
      private static AkLogger ms_Instance = new AkLogger();
      private AkLogger.ErrorLoggerInteropDelegate errorLoggerDelegate;

      private AkLogger()
      {
        // ISSUE: reference to a compiler-generated field
        if (AkLogger._f__mg_cache0 == null)
        {
          // ISSUE: reference to a compiler-generated field
          AkLogger._f__mg_cache0 = new AkLogger.ErrorLoggerInteropDelegate(AkLogger.WwiseInternalLogError);
        }
        // ISSUE: reference to a compiler-generated field
        this.errorLoggerDelegate = AkLogger._f__mg_cache0;
        // ISSUE: explicit constructor call
        // base..ctor(); // Removed: explicit base constructor calls not allowed in C#
        if (AkLogger.ms_Instance != null)
          return;
        AkLogger.ms_Instance = this;
        AkSoundEngine.SetErrorLogger(this.errorLoggerDelegate);
      }

      public static AkLogger Instance => AkLogger.ms_Instance;

      ~AkLogger()
      {
        if (AkLogger.ms_Instance != this)
          return;
        AkLogger.ms_Instance = (AkLogger) null;
        this.errorLoggerDelegate = (AkLogger.ErrorLoggerInteropDelegate) null;
        AkSoundEngine.SetErrorLogger();
      }

      public void Init()
      {
      }

      [AOT.MonoPInvokeCallback(typeof (AkLogger.ErrorLoggerInteropDelegate))]
      public static void WwiseInternalLogError(string message)
      {
        Debug.LogError((object) ("Wwise: " + message));
      }

      public static void Message(string message) => Debug.Log((object) ("WwiseUnity: " + message));

      public static void Warning(string message)
      {
        Debug.LogWarning((object) ("WwiseUnity: " + message));
      }

      public static void Error(string message) => Debug.LogError((object) ("WwiseUnity: " + message));

      [UnmanagedFunctionPointer(CallingConvention.StdCall)]
      public delegate void ErrorLoggerInteropDelegate([MarshalAs(UnmanagedType.LPStr)] string message);
    }

}
