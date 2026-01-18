#nullable disable
namespace InControl.NativeProfile
{
    public class HoriFightingStickVXMacProfile : Xbox360DriverMacProfile
    {
        public HoriFightingStickVXMacProfile()
        {
            this.Name = "Hori Fighting Stick VX";
            this.Meta = "Hori Fighting Stick VX on Mac";
            this.Matchers = new NativeInputDeviceMatcher[2]
            {
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 7085),
                    ProductID = new ushort?((ushort) 62723)
                },
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 9414),
                    ProductID = new ushort?((ushort) 21762)
                }
            };
        }
    }
}
