// Decompiled with JetBrains decompiler
// Type: Dungeonator.PathFinderDebugHandler
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace Dungeonator
{
  public delegate void PathFinderDebugHandler(
    int fromX,
    int fromY,
    int x,
    int y,
    PathFinderNodeType type,
    int totalCost,
    int cost);
}
