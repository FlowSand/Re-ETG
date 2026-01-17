// Decompiled with JetBrains decompiler
// Type: Dungeonator.FastDungeonLayoutPathfinder
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

#nullable disable
namespace Dungeonator
{
  public class FastDungeonLayoutPathfinder
  {
    private byte[,] mGrid;
    private PriorityQueueB<int> mOpen;
    private List<PathFinderNode> mClose = new List<PathFinderNode>();
    private bool mStop;
    private bool mStopped = true;
    private int mHoriz;
    private HeuristicFormula mFormula = HeuristicFormula.Manhattan;
    private bool mDiagonals = true;
    private int mHEstimate = 2;
    private bool mPunishChangeDirection;
    private bool mTieBreaker;
    private bool mHeavyDiagonals;
    private int mSearchLimit = 2000;
    private double mCompletedTime;
    private bool mDebugProgress;
    private bool mDebugFoundPath;
    private FastDungeonLayoutPathfinder.PathFinderNodeFast[] mCalcGrid;
    private byte mOpenNodeValue = 1;
    private byte mCloseNodeValue = 2;
    private int mH;
    private int mLocation;
    private int mNewLocation;
    private ushort mLocationX;
    private ushort mLocationY;
    private ushort mNewLocationX;
    private ushort mNewLocationY;
    private int mCloseNodeCounter;
    private ushort mGridX;
    private ushort mGridY;
    private ushort mGridXMinus1;
    private ushort mGridYLog2;
    private bool mFound;
    private sbyte[,] mDirection = new sbyte[8, 2]
    {
      {
        (sbyte) 0,
        (sbyte) -1
      },
      {
        (sbyte) 1,
        (sbyte) 0
      },
      {
        (sbyte) 0,
        (sbyte) 1
      },
      {
        (sbyte) -1,
        (sbyte) 0
      },
      {
        (sbyte) 1,
        (sbyte) -1
      },
      {
        (sbyte) 1,
        (sbyte) 1
      },
      {
        (sbyte) -1,
        (sbyte) 1
      },
      {
        (sbyte) -1,
        (sbyte) -1
      }
    };
    private int mEndLocation;
    private int mNewG;

    public FastDungeonLayoutPathfinder(byte[,] grid)
    {
      this.mGrid = grid != null ? grid : throw new Exception("Grid cannot be null");
      this.mGridX = (ushort) (this.mGrid.GetUpperBound(0) + 1);
      this.mGridY = (ushort) (this.mGrid.GetUpperBound(1) + 1);
      this.mGridXMinus1 = (ushort) ((uint) this.mGridX - 1U);
      this.mGridYLog2 = (ushort) Math.Log((double) this.mGridY, 2.0);
      if (Math.Log((double) this.mGridX, 2.0) != (double) (int) Math.Log((double) this.mGridX, 2.0) || Math.Log((double) this.mGridY, 2.0) != (double) (int) Math.Log((double) this.mGridY, 2.0))
        throw new Exception("Invalid Grid, size in X and Y must be power of 2");
      if (this.mCalcGrid == null || this.mCalcGrid.Length != (int) this.mGridX * (int) this.mGridY)
        this.mCalcGrid = new FastDungeonLayoutPathfinder.PathFinderNodeFast[(int) this.mGridX * (int) this.mGridY];
      this.mOpen = new PriorityQueueB<int>((IComparer<int>) new FastDungeonLayoutPathfinder.ComparePFNodeMatrix(this.mCalcGrid));
    }

    public bool Stopped => this.mStopped;

    public HeuristicFormula Formula
    {
      get => this.mFormula;
      set => this.mFormula = value;
    }

    public bool Diagonals
    {
      get => this.mDiagonals;
      set
      {
        this.mDiagonals = value;
        if (this.mDiagonals)
          this.mDirection = new sbyte[8, 2]
          {
            {
              (sbyte) 0,
              (sbyte) -1
            },
            {
              (sbyte) 1,
              (sbyte) 0
            },
            {
              (sbyte) 0,
              (sbyte) 1
            },
            {
              (sbyte) -1,
              (sbyte) 0
            },
            {
              (sbyte) 1,
              (sbyte) -1
            },
            {
              (sbyte) 1,
              (sbyte) 1
            },
            {
              (sbyte) -1,
              (sbyte) 1
            },
            {
              (sbyte) -1,
              (sbyte) -1
            }
          };
        else
          this.mDirection = new sbyte[4, 2]
          {
            {
              (sbyte) 0,
              (sbyte) -1
            },
            {
              (sbyte) 1,
              (sbyte) 0
            },
            {
              (sbyte) 0,
              (sbyte) 1
            },
            {
              (sbyte) -1,
              (sbyte) 0
            }
          };
      }
    }

    public bool HeavyDiagonals
    {
      get => this.mHeavyDiagonals;
      set => this.mHeavyDiagonals = value;
    }

    public int HeuristicEstimate
    {
      get => this.mHEstimate;
      set => this.mHEstimate = value;
    }

    public bool PunishChangeDirection
    {
      get => this.mPunishChangeDirection;
      set => this.mPunishChangeDirection = value;
    }

    public bool TieBreaker
    {
      get => this.mTieBreaker;
      set => this.mTieBreaker = value;
    }

    public int SearchLimit
    {
      get => this.mSearchLimit;
      set => this.mSearchLimit = value;
    }

    public double CompletedTime
    {
      get => this.mCompletedTime;
      set => this.mCompletedTime = value;
    }

    public bool DebugProgress
    {
      get => this.mDebugProgress;
      set => this.mDebugProgress = value;
    }

    public bool DebugFoundPath
    {
      get => this.mDebugFoundPath;
      set => this.mDebugFoundPath = value;
    }

    public void FindPathStop() => this.mStop = true;

    public List<PathFinderNode> FindPath(IntVector2 start, IntVector2 end)
    {
      return this.FindPath(start, IntVector2.Zero, end);
    }

    public List<PathFinderNode> FindPath(IntVector2 start, IntVector2 startDirection, IntVector2 end)
    {
      lock ((object) this)
      {
        this.mFound = false;
        this.mStop = false;
        this.mStopped = false;
        this.mCloseNodeCounter = 0;
        this.mOpenNodeValue += (byte) 2;
        this.mCloseNodeValue += (byte) 2;
        this.mOpen.Clear();
        this.mClose.Clear();
        this.mLocation = (start.Y << (int) this.mGridYLog2) + start.X;
        this.mEndLocation = (end.Y << (int) this.mGridYLog2) + end.X;
        this.mCalcGrid[this.mLocation].G = 0;
        this.mCalcGrid[this.mLocation].F = this.mHEstimate;
        this.mCalcGrid[this.mLocation].PX = (ushort) start.X;
        this.mCalcGrid[this.mLocation].PY = (ushort) start.Y;
        this.mCalcGrid[this.mLocation].Status = this.mOpenNodeValue;
        this.mOpen.Push(this.mLocation);
        while (this.mOpen.Count > 0 && !this.mStop)
        {
          this.mLocation = this.mOpen.Pop();
          if ((int) this.mCalcGrid[this.mLocation].Status != (int) this.mCloseNodeValue)
          {
            this.mLocationX = (ushort) ((uint) this.mLocation & (uint) this.mGridXMinus1);
            this.mLocationY = (ushort) ((uint) this.mLocation >> (int) this.mGridYLog2);
            if (this.mLocation == this.mEndLocation)
            {
              this.mCalcGrid[this.mLocation].Status = this.mCloseNodeValue;
              this.mFound = true;
              break;
            }
            if (this.mCloseNodeCounter > this.mSearchLimit)
            {
              this.mStopped = true;
              return (List<PathFinderNode>) null;
            }
            if (this.mPunishChangeDirection)
            {
              this.mHoriz = (int) this.mLocationX - (int) this.mCalcGrid[this.mLocation].PX;
              if ((int) this.mLocationX == start.x && (int) this.mLocationY == start.y)
                this.mHoriz = startDirection.x;
            }
            for (int index = 0; index < (!this.mDiagonals ? 4 : 8); ++index)
            {
              this.mNewLocationX = (ushort) ((uint) this.mLocationX + (uint) this.mDirection[index, 0]);
              this.mNewLocationY = (ushort) ((uint) this.mLocationY + (uint) this.mDirection[index, 1]);
              this.mNewLocation = ((int) this.mNewLocationY << (int) this.mGridYLog2) + (int) this.mNewLocationX;
              if ((int) this.mNewLocationX < (int) this.mGridX && (int) this.mNewLocationY < (int) this.mGridY && this.mGrid[(int) this.mNewLocationX, (int) this.mNewLocationY] != (byte) 0)
              {
                this.mNewG = !this.mHeavyDiagonals || index <= 3 ? this.mCalcGrid[this.mLocation].G + (int) this.mGrid[(int) this.mNewLocationX, (int) this.mNewLocationY] : this.mCalcGrid[this.mLocation].G + (int) ((double) this.mGrid[(int) this.mNewLocationX, (int) this.mNewLocationY] * 2.41);
                if (this.mPunishChangeDirection)
                {
                  if ((int) this.mNewLocationX - (int) this.mLocationX != 0 && this.mHoriz == 0)
                    this.mNewG += Math.Abs((int) this.mNewLocationX - end.X) + Math.Abs((int) this.mNewLocationY - end.Y);
                  if ((int) this.mNewLocationY - (int) this.mLocationY != 0 && this.mHoriz != 0)
                    this.mNewG += Math.Abs((int) this.mNewLocationX - end.X) + Math.Abs((int) this.mNewLocationY - end.Y);
                }
                if ((int) this.mCalcGrid[this.mNewLocation].Status != (int) this.mOpenNodeValue && (int) this.mCalcGrid[this.mNewLocation].Status != (int) this.mCloseNodeValue || this.mCalcGrid[this.mNewLocation].G > this.mNewG)
                {
                  this.mCalcGrid[this.mNewLocation].PX = this.mLocationX;
                  this.mCalcGrid[this.mNewLocation].PY = this.mLocationY;
                  this.mCalcGrid[this.mNewLocation].G = this.mNewG;
                  switch (this.mFormula)
                  {
                    case HeuristicFormula.MaxDXDY:
                      this.mH = this.mHEstimate * Math.Max(Math.Abs((int) this.mNewLocationX - end.X), Math.Abs((int) this.mNewLocationY - end.Y));
                      break;
                    case HeuristicFormula.DiagonalShortCut:
                      int num1 = Math.Min(Math.Abs((int) this.mNewLocationX - end.X), Math.Abs((int) this.mNewLocationY - end.Y));
                      int num2 = Math.Abs((int) this.mNewLocationX - end.X) + Math.Abs((int) this.mNewLocationY - end.Y);
                      this.mH = this.mHEstimate * 2 * num1 + this.mHEstimate * (num2 - 2 * num1);
                      break;
                    case HeuristicFormula.Euclidean:
                      this.mH = (int) ((double) this.mHEstimate * Math.Sqrt(Math.Pow((double) ((int) this.mNewLocationY - end.X), 2.0) + Math.Pow((double) ((int) this.mNewLocationY - end.Y), 2.0)));
                      break;
                    case HeuristicFormula.EuclideanNoSQR:
                      this.mH = (int) ((double) this.mHEstimate * (Math.Pow((double) ((int) this.mNewLocationX - end.X), 2.0) + Math.Pow((double) ((int) this.mNewLocationY - end.Y), 2.0)));
                      break;
                    case HeuristicFormula.Custom1:
                      IntVector2 intVector2 = new IntVector2(Math.Abs(end.X - (int) this.mNewLocationX), Math.Abs(end.Y - (int) this.mNewLocationY));
                      int num3 = Math.Abs(intVector2.X - intVector2.Y);
                      this.mH = this.mHEstimate * (Math.Abs((intVector2.X + intVector2.Y - num3) / 2) + num3 + intVector2.X + intVector2.Y);
                      break;
                    default:
                      this.mH = this.mHEstimate * (Math.Abs((int) this.mNewLocationX - end.X) + Math.Abs((int) this.mNewLocationY - end.Y));
                      break;
                  }
                  if (this.mTieBreaker)
                  {
                    int num4 = (int) this.mLocationX - end.X;
                    int num5 = (int) this.mLocationY - end.Y;
                    int num6 = start.X - end.X;
                    int num7 = start.Y - end.Y;
                    this.mH = (int) ((double) this.mH + (double) Math.Abs(num4 * num7 - num6 * num5) * 0.001);
                  }
                  this.mCalcGrid[this.mNewLocation].F = this.mNewG + this.mH;
                  this.mOpen.Push(this.mNewLocation);
                  this.mCalcGrid[this.mNewLocation].Status = this.mOpenNodeValue;
                }
              }
            }
            ++this.mCloseNodeCounter;
            this.mCalcGrid[this.mLocation].Status = this.mCloseNodeValue;
          }
        }
        if (this.mFound)
        {
          this.mClose.Clear();
          int x = end.X;
          int y = end.Y;
          FastDungeonLayoutPathfinder.PathFinderNodeFast pathFinderNodeFast = this.mCalcGrid[(end.Y << (int) this.mGridYLog2) + end.X];
          PathFinderNode pathFinderNode;
          pathFinderNode.F = pathFinderNodeFast.F;
          pathFinderNode.G = pathFinderNodeFast.G;
          pathFinderNode.H = 0;
          pathFinderNode.PX = (int) pathFinderNodeFast.PX;
          pathFinderNode.PY = (int) pathFinderNodeFast.PY;
          pathFinderNode.X = end.X;
          int py;
          for (pathFinderNode.Y = end.Y; pathFinderNode.X != pathFinderNode.PX || pathFinderNode.Y != pathFinderNode.PY; pathFinderNode.Y = py)
          {
            this.mClose.Add(pathFinderNode);
            int px = pathFinderNode.PX;
            py = pathFinderNode.PY;
            pathFinderNodeFast = this.mCalcGrid[(py << (int) this.mGridYLog2) + px];
            pathFinderNode.F = pathFinderNodeFast.F;
            pathFinderNode.G = pathFinderNodeFast.G;
            pathFinderNode.H = 0;
            pathFinderNode.PX = (int) pathFinderNodeFast.PX;
            pathFinderNode.PY = (int) pathFinderNodeFast.PY;
            pathFinderNode.X = px;
          }
          this.mClose.Add(pathFinderNode);
          this.mStopped = true;
          return this.mClose;
        }
        this.mStopped = true;
        return (List<PathFinderNode>) null;
      }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct PathFinderNodeFast
    {
      public int F;
      public int G;
      public ushort PX;
      public ushort PY;
      public byte Status;
    }

    internal class ComparePFNodeMatrix : IComparer<int>
    {
      private FastDungeonLayoutPathfinder.PathFinderNodeFast[] mMatrix;

      public ComparePFNodeMatrix(
        FastDungeonLayoutPathfinder.PathFinderNodeFast[] matrix)
      {
        this.mMatrix = matrix;
      }

      public int Compare(int a, int b)
      {
        if (this.mMatrix[a].F > this.mMatrix[b].F)
          return 1;
        return this.mMatrix[a].F < this.mMatrix[b].F ? -1 : 0;
      }
    }
  }
}
