// Decompiled with JetBrains decompiler
// Type: tk2dRuntime.ISpriteCollectionForceBuild
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace tk2dRuntime
{
  public interface ISpriteCollectionForceBuild
  {
    bool UsesSpriteCollection(tk2dSpriteCollectionData spriteCollection);

    void ForceBuild();
  }
}
