#nullable disable
namespace InControl.NativeProfile
{
    public class MadCatzMicroControllerMacProfile : Xbox360DriverMacProfile
    {
        public MadCatzMicroControllerMacProfile()
        {
            this.Name = "Mad Catz Micro Controller";
            this.Meta = "Mad Catz Micro Controller on Mac";
            this.Matchers = new NativeInputDeviceMatcher[1]
            {
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 1848),
                    ProductID = new ushort?((ushort) 18230)
                }
            };
        }
    }
}
