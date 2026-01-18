#nullable disable
namespace InControl.NativeProfile
{
    public class ThrustmasterGPXControllerMacProfile : Xbox360DriverMacProfile
    {
        public ThrustmasterGPXControllerMacProfile()
        {
            this.Name = "Thrustmaster GPX Controller";
            this.Meta = "Thrustmaster GPX Controller on Mac";
            this.Matchers = new NativeInputDeviceMatcher[2]
            {
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 1103),
                    ProductID = new ushort?((ushort) 45862)
                },
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 9414),
                    ProductID = new ushort?((ushort) 23298)
                }
            };
        }
    }
}
