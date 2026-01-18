#nullable disable
namespace InControl.NativeProfile
{
    public class NaconGC100XFControllerMacProfile : Xbox360DriverMacProfile
    {
        public NaconGC100XFControllerMacProfile()
        {
            this.Name = "Nacon GC-100XF Controller";
            this.Meta = "Nacon GC-100XF Controller on Mac";
            this.Matchers = new NativeInputDeviceMatcher[1]
            {
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 4553),
                    ProductID = new ushort?((ushort) 22000)
                }
            };
        }
    }
}
