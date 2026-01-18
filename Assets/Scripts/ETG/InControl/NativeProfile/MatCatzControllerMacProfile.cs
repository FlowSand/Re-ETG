#nullable disable
namespace InControl.NativeProfile
{
    public class MatCatzControllerMacProfile : Xbox360DriverMacProfile
    {
        public MatCatzControllerMacProfile()
        {
            this.Name = "Mat Catz Controller";
            this.Meta = "Mat Catz Controller on Mac";
            this.Matchers = new NativeInputDeviceMatcher[1]
            {
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 7085),
                    ProductID = new ushort?((ushort) 61462)
                }
            };
        }
    }
}
