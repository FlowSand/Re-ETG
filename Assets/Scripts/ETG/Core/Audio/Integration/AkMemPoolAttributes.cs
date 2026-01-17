// Decompiled with JetBrains decompiler
// Type: AkMemPoolAttributes
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable

namespace ETG.Core.Audio.Integration
{
    public enum AkMemPoolAttributes
    {
      AkNoAlloc = 0,
      AkMalloc = 1,
      AkVirtualAlloc = 2,
      AkAllocMask = 3,
      AkBlockMgmtMask = 8,
      AkFixedSizeBlocksMode = 8,
    }

}
