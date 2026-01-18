using System;

#nullable disable
namespace AK.Wwise
{
    [Serializable]
    public class BaseGroupType : BaseType
    {
        public int groupID;

        protected uint GetGroupID() => (uint) this.groupID;

        public override bool IsValid() => base.IsValid() && this.groupID != 0;
    }
}
