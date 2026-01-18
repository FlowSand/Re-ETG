#nullable disable
namespace InControl.NativeProfile
{
    public class RockCandyControllerMacProfile : Xbox360DriverMacProfile
    {
        public RockCandyControllerMacProfile()
        {
            this.Name = "Rock Candy Controller";
            this.Meta = "Rock Candy Controller on Mac";
            this.Matchers = new NativeInputDeviceMatcher[1]
            {
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 3695),
                    ProductID = new ushort?((ushort) 287)
                }
            };
        }
    }
}
