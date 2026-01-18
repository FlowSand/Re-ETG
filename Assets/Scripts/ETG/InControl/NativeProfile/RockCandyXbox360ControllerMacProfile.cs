#nullable disable
namespace InControl.NativeProfile
{
    public class RockCandyXbox360ControllerMacProfile : Xbox360DriverMacProfile
    {
        public RockCandyXbox360ControllerMacProfile()
        {
            this.Name = "Rock Candy Xbox 360 Controller";
            this.Meta = "Rock Candy Xbox 360 Controller on Mac";
            this.Matchers = new NativeInputDeviceMatcher[2]
            {
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 3695),
                    ProductID = new ushort?((ushort) 543)
                },
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 9414),
                    ProductID = new ushort?((ushort) 64254)
                }
            };
        }
    }
}
