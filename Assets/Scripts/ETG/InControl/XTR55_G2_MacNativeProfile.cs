#nullable disable
namespace InControl
{
    [AutoDiscover]
    public class XTR55_G2_MacNativeProfile : NativeInputDeviceProfile
    {
        public XTR55_G2_MacNativeProfile()
        {
            this.Name = "SAILI Simulator XTR5.5 G2 FMS Controller";
            this.Meta = "SAILI Simulator XTR5.5 G2 FMS Controller on OS X";
            this.DeviceClass = InputDeviceClass.Controller;
            this.IncludePlatforms = new string[1]{ "OS X" };
            this.Matchers = new NativeInputDeviceMatcher[1]
            {
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 2971),
                    ProductID = new ushort?((ushort) 16402),
                    NameLiterals = new string[1]
                    {
                        "SAILI Simulator --- XTR5.5+G2+FMS Controller"
                    }
                }
            };
        }
    }
}
