#nullable disable
namespace InControl.NativeProfile
{
    public class BatarangControllerMacProfile : Xbox360DriverMacProfile
    {
        public BatarangControllerMacProfile()
        {
            this.Name = "Batarang Controller";
            this.Meta = "Batarang Controller on Mac";
            this.Matchers = new NativeInputDeviceMatcher[1]
            {
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 5604),
                    ProductID = new ushort?((ushort) 16144)
                }
            };
        }
    }
}
