// Decompiled with JetBrains decompiler
// Type: Position
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    [Serializable]
    public struct Position
    {
      public IntVector2 m_position;
      public Vector2 m_remainder;

      public Position(int pixelX, int pixelY)
      {
        this.m_position.x = pixelX;
        this.m_position.y = pixelY;
        this.m_remainder = Vector2.zero;
      }

      public Position(float unitX, float unitY)
      {
        this.m_position.x = Mathf.RoundToInt(unitX * 16f);
        this.m_position.y = Mathf.RoundToInt(unitY * 16f);
        this.m_remainder.x = unitX - (float) this.m_position.x * (1f / 16f);
        this.m_remainder.y = unitY - (float) this.m_position.y * (1f / 16f);
      }

      public Position(IntVector2 pixelPosition, Vector2 remainder)
      {
        this.m_position = pixelPosition;
        this.m_remainder = remainder;
      }

      public Position(Position position)
        : this(position.m_position, position.m_remainder)
      {
      }

      public Position(Vector2 unitPosition)
        : this(unitPosition.x, unitPosition.y)
      {
      }

      public Position(Vector3 unitPosition)
        : this(unitPosition.x, unitPosition.y)
      {
      }

      public Position(IntVector2 pixelPosition)
        : this(pixelPosition.x, pixelPosition.y)
      {
      }

      public int X
      {
        get => this.m_position.x;
        set
        {
          this.m_position.x = value;
          this.m_remainder.x = 0.0f;
        }
      }

      public int Y
      {
        get => this.m_position.y;
        set
        {
          this.m_position.y = value;
          this.m_remainder.y = 0.0f;
        }
      }

      public float UnitX
      {
        get => (float) this.m_position.x * (1f / 16f) + this.m_remainder.x;
        set
        {
          this.m_position.x = Mathf.RoundToInt(value * 16f);
          this.m_remainder.x = value - (float) this.m_position.x * (1f / 16f);
        }
      }

      public float UnitY
      {
        get => (float) this.m_position.y * (1f / 16f) + this.m_remainder.y;
        set
        {
          this.m_position.y = Mathf.RoundToInt(value * 16f);
          this.m_remainder.y = value - (float) this.m_position.y * (1f / 16f);
        }
      }

      public IntVector2 PixelPosition
      {
        get => this.m_position;
        set
        {
          this.X = value.x;
          this.Y = value.y;
        }
      }

      public Vector2 UnitPosition
      {
        get
        {
          return new Vector2((float) this.m_position.x * (1f / 16f) + this.m_remainder.x, (float) this.m_position.y * (1f / 16f) + this.m_remainder.y);
        }
        set
        {
          this.UnitX = value.x;
          this.UnitY = value.y;
        }
      }

      public Vector2 Remainder
      {
        get => this.m_remainder;
        set => this.m_remainder = value;
      }

      public static Position operator +(Position lhs, Vector2 rhs)
      {
        return new Position(lhs.UnitPosition + rhs);
      }

      public static Position operator +(Position lhs, IntVector2 rhs)
      {
        return new Position(lhs.PixelPosition + rhs, lhs.Remainder);
      }

      public Vector2 GetPixelVector2() => (Vector2) this.m_position * (1f / 16f);

      public IntVector2 GetPixelDelta(Vector2 unitDelta)
      {
        return IntVector2.Zero with
        {
          x = Mathf.RoundToInt((float) (((double) this.UnitX + (double) unitDelta.x) * 16.0)) - this.m_position.x,
          y = Mathf.RoundToInt((float) (((double) this.UnitY + (double) unitDelta.y) * 16.0)) - this.m_position.y
        };
      }
    }

}
