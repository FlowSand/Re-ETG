#nullable disable
namespace InControl.NativeProfile
{
    public class APlayControllerMacProfile : Xbox360DriverMacProfile
    {
        public APlayControllerMacProfile()
        {
            this.Name = "A Play Controller";
            this.Meta = "A Play Controller on Mac";
            this.Matchers = new NativeInputDeviceMatcher[1]
            {
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 9414),
                    ProductID = new ushort?((ushort) 64251)
                }
            };
        }
    }
}
