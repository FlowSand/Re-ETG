#nullable disable
namespace InControl.NativeProfile
{
    public class BigBenControllerMacProfile : Xbox360DriverMacProfile
    {
        public BigBenControllerMacProfile()
        {
            this.Name = "Big Ben Controller";
            this.Meta = "Big Ben Controller on Mac";
            this.Matchers = new NativeInputDeviceMatcher[1]
            {
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 5227),
                    ProductID = new ushort?((ushort) 1537)
                }
            };
        }
    }
}
