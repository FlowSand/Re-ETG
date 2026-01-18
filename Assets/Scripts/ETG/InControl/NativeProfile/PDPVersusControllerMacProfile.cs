#nullable disable
namespace InControl.NativeProfile
{
    public class PDPVersusControllerMacProfile : Xbox360DriverMacProfile
    {
        public PDPVersusControllerMacProfile()
        {
            this.Name = "PDP Versus Controller";
            this.Meta = "PDP Versus Controller on Mac";
            this.Matchers = new NativeInputDeviceMatcher[1]
            {
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 7085),
                    ProductID = new ushort?((ushort) 63748)
                }
            };
        }
    }
}
