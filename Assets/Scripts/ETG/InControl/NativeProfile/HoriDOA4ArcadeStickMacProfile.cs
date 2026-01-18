#nullable disable
namespace InControl.NativeProfile
{
    public class HoriDOA4ArcadeStickMacProfile : Xbox360DriverMacProfile
    {
        public HoriDOA4ArcadeStickMacProfile()
        {
            this.Name = "Hori DOA4 Arcade Stick";
            this.Meta = "Hori DOA4 Arcade Stick on Mac";
            this.Matchers = new NativeInputDeviceMatcher[1]
            {
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 3853),
                    ProductID = new ushort?((ushort) 10)
                }
            };
        }
    }
}
