#nullable disable
namespace InControl.NativeProfile
{
    public class RazerStrikeControllerMacProfile : Xbox360DriverMacProfile
    {
        public RazerStrikeControllerMacProfile()
        {
            this.Name = "Razer Strike Controller";
            this.Meta = "Razer Strike Controller on Mac";
            this.Matchers = new NativeInputDeviceMatcher[1]
            {
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 5769),
                    ProductID = new ushort?((ushort) 1)
                }
            };
        }
    }
}
