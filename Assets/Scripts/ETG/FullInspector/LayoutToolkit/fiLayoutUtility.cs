// Decompiled with JetBrains decompiler
// Type: FullInspector.LayoutToolkit.fiLayoutUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace FullInspector.LayoutToolkit
{
  public static class fiLayoutUtility
  {
    public static fiLayout Margin(float margin, fiLayout layout)
    {
      return (fiLayout) new fiHorizontalLayout()
      {
        margin,
        (fiLayout) new fiVerticalLayout()
        {
          margin,
          layout,
          margin
        },
        margin
      };
    }
  }
}
