// Decompiled with JetBrains decompiler
// Type: Dungeonator.HeuristicFormula
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace Dungeonator;

public enum HeuristicFormula
{
  Manhattan = 1,
  MaxDXDY = 2,
  DiagonalShortCut = 3,
  Euclidean = 4,
  EuclideanNoSQR = 5,
  Custom1 = 6,
}
