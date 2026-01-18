#nullable disable
namespace InControl.NativeProfile
{
    public class PDPBattlefieldXBoxOneControllerMacProfile : XboxOneDriverMacProfile
    {
        public PDPBattlefieldXBoxOneControllerMacProfile()
        {
            this.Name = "PDP Battlefield XBox One Controller";
            this.Meta = "PDP Battlefield XBox One Controller on Mac";
            this.Matchers = new NativeInputDeviceMatcher[1]
            {
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 3695),
                    ProductID = new ushort?((ushort) 356)
                }
            };
        }
    }
}
