// Decompiled with JetBrains decompiler
// Type: AkUtilities
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Text;

#nullable disable

namespace ETG.Core.Audio.Integration
{
    public class AkUtilities
    {
      public class ShortIDGenerator
      {
        private const uint s_prime32 = 16777619;
        private const uint s_offsetBasis32 = 2166136261;
        private static byte s_hashSize;
        private static uint s_mask;

        static ShortIDGenerator() => AkUtilities.ShortIDGenerator.HashSize = (byte) 32 /*0x20*/;

        public static byte HashSize
        {
          get => AkUtilities.ShortIDGenerator.s_hashSize;
          set
          {
            AkUtilities.ShortIDGenerator.s_hashSize = value;
            AkUtilities.ShortIDGenerator.s_mask = (uint) ((1 << (int) AkUtilities.ShortIDGenerator.s_hashSize) - 1);
          }
        }

        public static uint Compute(string in_name)
        {
          byte[] bytes = Encoding.UTF8.GetBytes(in_name.ToLower());
          uint num = 2166136261;
          for (int index = 0; index < bytes.Length; ++index)
            num = num * 16777619U ^ (uint) bytes[index];
          return AkUtilities.ShortIDGenerator.s_hashSize == (byte) 32 /*0x20*/ ? num : num >> (int) AkUtilities.ShortIDGenerator.s_hashSize ^ num & AkUtilities.ShortIDGenerator.s_mask;
        }
      }
    }

}
