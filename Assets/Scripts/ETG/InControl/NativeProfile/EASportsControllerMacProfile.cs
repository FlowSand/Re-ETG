#nullable disable
namespace InControl.NativeProfile
{
    public class EASportsControllerMacProfile : Xbox360DriverMacProfile
    {
        public EASportsControllerMacProfile()
        {
            this.Name = "EA Sports Controller";
            this.Meta = "EA Sports Controller on Mac";
            this.Matchers = new NativeInputDeviceMatcher[1]
            {
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 3695),
                    ProductID = new ushort?((ushort) 305)
                }
            };
        }
    }
}
