// Decompiled with JetBrains decompiler
// Type: InControl.KeyCombo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.IO;

#nullable disable
namespace InControl;

public struct KeyCombo
{
  private int includeSize;
  private ulong includeData;
  private int excludeSize;
  private ulong excludeData;
  private static Dictionary<ulong, string> cachedStrings = new Dictionary<ulong, string>();

  public KeyCombo(params Key[] keys)
  {
    this.includeData = 0UL;
    this.includeSize = 0;
    this.excludeData = 0UL;
    this.excludeSize = 0;
    for (int index = 0; index < keys.Length; ++index)
      this.AddInclude(keys[index]);
  }

  private void AddIncludeInt(int key)
  {
    if (this.includeSize == 8)
      return;
    this.includeData |= (ulong) (((long) key & (long) byte.MaxValue) << this.includeSize * 8);
    ++this.includeSize;
  }

  private int GetIncludeInt(int index)
  {
    return (int) ((long) (this.includeData >> index * 8) & (long) byte.MaxValue);
  }

  [Obsolete("Use KeyCombo.AddInclude instead.")]
  public void Add(Key key) => this.AddInclude(key);

  [Obsolete("Use KeyCombo.GetInclude instead.")]
  public Key Get(int index) => this.GetInclude(index);

  public void AddInclude(Key key) => this.AddIncludeInt((int) key);

  public Key GetInclude(int index)
  {
    if (index < 0 || index >= this.includeSize)
      throw new IndexOutOfRangeException($"Index {(object) index} is out of the range 0..{(object) this.includeSize}");
    return (Key) this.GetIncludeInt(index);
  }

  private void AddExcludeInt(int key)
  {
    if (this.excludeSize == 8)
      return;
    this.excludeData |= (ulong) (((long) key & (long) byte.MaxValue) << this.excludeSize * 8);
    ++this.excludeSize;
  }

  private int GetExcludeInt(int index)
  {
    return (int) ((long) (this.excludeData >> index * 8) & (long) byte.MaxValue);
  }

  public void AddExclude(Key key) => this.AddExcludeInt((int) key);

  public Key GetExclude(int index)
  {
    if (index < 0 || index >= this.excludeSize)
      throw new IndexOutOfRangeException($"Index {(object) index} is out of the range 0..{(object) this.excludeSize}");
    return (Key) this.GetExcludeInt(index);
  }

  public static KeyCombo With(params Key[] keys) => new KeyCombo(keys);

  public KeyCombo AndNot(params Key[] keys)
  {
    for (int index = 0; index < keys.Length; ++index)
      this.AddExclude(keys[index]);
    return this;
  }

  public void Clear()
  {
    this.includeData = 0UL;
    this.includeSize = 0;
    this.excludeData = 0UL;
    this.excludeSize = 0;
  }

  [Obsolete("Use KeyCombo.IncludeCount instead.")]
  public int Count => this.includeSize;

  public int IncludeCount => this.includeSize;

  public int ExcludeCount => this.excludeSize;

  public bool IsPressed
  {
    get
    {
      if (this.includeSize == 0)
        return false;
      bool isPressed = true;
      for (int index = 0; index < this.includeSize; ++index)
      {
        int includeInt = this.GetIncludeInt(index);
        isPressed = isPressed && KeyInfo.KeyList[includeInt].IsPressed;
      }
      for (int index = 0; index < this.excludeSize; ++index)
      {
        int excludeInt = this.GetExcludeInt(index);
        if (KeyInfo.KeyList[excludeInt].IsPressed)
          return false;
      }
      return isPressed;
    }
  }

  public static KeyCombo Detect(bool modifiersAsKeys)
  {
    KeyCombo keyCombo = new KeyCombo();
    if (modifiersAsKeys)
    {
      for (int key = 5; key < 13; ++key)
      {
        if (KeyInfo.KeyList[key].IsPressed)
        {
          keyCombo.AddIncludeInt(key);
          return keyCombo;
        }
      }
    }
    else
    {
      for (int key = 1; key < 5; ++key)
      {
        if (KeyInfo.KeyList[key].IsPressed)
          keyCombo.AddIncludeInt(key);
      }
    }
    for (int key = 13; key < KeyInfo.KeyList.Length; ++key)
    {
      if (KeyInfo.KeyList[key].IsPressed)
      {
        keyCombo.AddIncludeInt(key);
        return keyCombo;
      }
    }
    keyCombo.Clear();
    return keyCombo;
  }

  public override string ToString()
  {
    string empty;
    if (!KeyCombo.cachedStrings.TryGetValue(this.includeData, out empty))
    {
      empty = string.Empty;
      for (int index = 0; index < this.includeSize; ++index)
      {
        if (index != 0)
          empty += " ";
        int includeInt = this.GetIncludeInt(index);
        empty += KeyInfo.KeyList[includeInt].Name;
      }
    }
    return empty;
  }

  public static bool operator ==(KeyCombo a, KeyCombo b)
  {
    return (long) a.includeData == (long) b.includeData && (long) a.excludeData == (long) b.excludeData;
  }

  public static bool operator !=(KeyCombo a, KeyCombo b)
  {
    return (long) a.includeData != (long) b.includeData || (long) a.excludeData != (long) b.excludeData;
  }

  public override bool Equals(object other)
  {
    return other is KeyCombo keyCombo && (long) this.includeData == (long) keyCombo.includeData && (long) this.excludeData == (long) keyCombo.excludeData;
  }

  public override int GetHashCode()
  {
    return (17 * 31 /*0x1F*/ + this.includeData.GetHashCode()) * 31 /*0x1F*/ + this.excludeData.GetHashCode();
  }

  internal void Load(BinaryReader reader, ushort dataFormatVersion, bool upgrade)
  {
    if (dataFormatVersion == (ushort) 1)
    {
      this.includeSize = reader.ReadInt32();
      this.includeData = reader.ReadUInt64();
    }
    else
    {
      if (dataFormatVersion != (ushort) 2)
        throw new InControlException("Unknown data format version: " + (object) dataFormatVersion);
      this.includeSize = reader.ReadInt32();
      this.includeData = reader.ReadUInt64();
      this.excludeSize = reader.ReadInt32();
      this.excludeData = reader.ReadUInt64();
    }
  }

  internal void Save(BinaryWriter writer)
  {
    writer.Write(this.includeSize);
    writer.Write(this.includeData);
    writer.Write(this.excludeSize);
    writer.Write(this.excludeData);
  }
}
