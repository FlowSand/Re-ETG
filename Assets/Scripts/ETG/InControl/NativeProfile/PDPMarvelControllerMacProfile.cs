#nullable disable
namespace InControl.NativeProfile
{
    public class PDPMarvelControllerMacProfile : Xbox360DriverMacProfile
    {
        public PDPMarvelControllerMacProfile()
        {
            this.Name = "PDP Marvel Controller";
            this.Meta = "PDP Marvel Controller on Mac";
            this.Matchers = new NativeInputDeviceMatcher[1]
            {
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 3695),
                    ProductID = new ushort?((ushort) 327)
                }
            };
        }
    }
}
