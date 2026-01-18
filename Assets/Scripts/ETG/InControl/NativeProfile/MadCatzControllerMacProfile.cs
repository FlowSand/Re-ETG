#nullable disable
namespace InControl.NativeProfile
{
    public class MadCatzControllerMacProfile : Xbox360DriverMacProfile
    {
        public MadCatzControllerMacProfile()
        {
            this.Name = "Mad Catz Controller";
            this.Meta = "Mad Catz Controller on Mac";
            this.Matchers = new NativeInputDeviceMatcher[3]
            {
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 1848),
                    ProductID = new ushort?((ushort) 18198)
                },
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 7085),
                    ProductID = new ushort?((ushort) 63746)
                },
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 7085),
                    ProductID = new ushort?((ushort) 61642)
                }
            };
        }
    }
}
