#nullable disable
namespace InControl.NativeProfile
{
    public class MadCatzPortableDrumMacProfile : Xbox360DriverMacProfile
    {
        public MadCatzPortableDrumMacProfile()
        {
            this.Name = "Mad Catz Portable Drum";
            this.Meta = "Mad Catz Portable Drum on Mac";
            this.Matchers = new NativeInputDeviceMatcher[1]
            {
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 1848),
                    ProductID = new ushort?((ushort) 39025)
                }
            };
        }
    }
}
