using System;

#nullable disable
namespace Dungeonator
{
    [Serializable]
    public class RuntimeInjectionFlags
    {
        public bool ShopAnnexed;
        public bool CastleFireplace;

        public void Clear()
        {
            this.ShopAnnexed = false;
            this.CastleFireplace = false;
        }

        public bool Merge(RuntimeInjectionFlags flags)
        {
            bool flag = false;
            if (!this.CastleFireplace && flags.CastleFireplace)
                flag = true;
            this.ShopAnnexed |= flags.ShopAnnexed;
            this.CastleFireplace |= flags.CastleFireplace;
            return flag;
        }

        public bool IsValid(RuntimeInjectionFlags other)
        {
            bool flag = true;
            if (this.ShopAnnexed && other.ShopAnnexed)
                flag = false;
            if (this.CastleFireplace && other.CastleFireplace)
                flag = false;
            return flag;
        }
    }
}
