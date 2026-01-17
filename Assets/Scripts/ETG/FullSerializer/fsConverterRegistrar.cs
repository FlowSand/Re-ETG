// Decompiled with JetBrains decompiler
// Type: FullSerializer.fsConverterRegistrar
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using FullSerializer.Internal;
using FullSerializer.Internal.DirectConverters;
using System;
using System.Collections.Generic;
using System.Reflection;

#nullable disable
namespace FullSerializer
{
  public class fsConverterRegistrar
  {
    public static AnimationCurve_DirectConverter Register_AnimationCurve_DirectConverter;
    public static Bounds_DirectConverter Register_Bounds_DirectConverter;
    public static Gradient_DirectConverter Register_Gradient_DirectConverter;
    public static Keyframe_DirectConverter Register_Keyframe_DirectConverter;
    public static LayerMask_DirectConverter Register_LayerMask_DirectConverter;
    public static Rect_DirectConverter Register_Rect_DirectConverter;
    public static List<Type> Converters = new List<Type>();

    static fsConverterRegistrar()
    {
      foreach (FieldInfo declaredField in typeof (fsConverterRegistrar).GetDeclaredFields())
      {
        if (declaredField.Name.StartsWith("Register_"))
          fsConverterRegistrar.Converters.Add(declaredField.FieldType);
      }
      foreach (MethodInfo declaredMethod in typeof (fsConverterRegistrar).GetDeclaredMethods())
      {
        if (declaredMethod.Name.StartsWith("Register_"))
          declaredMethod.Invoke((object) null, (object[]) null);
      }
    }
  }
}
