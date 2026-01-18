#nullable disable
namespace InControl.NativeProfile
{
    public class HoriBlueSoloControllerMacProfile : Xbox360DriverMacProfile
    {
        public HoriBlueSoloControllerMacProfile()
        {
            this.Name = "Hori Blue Solo Controller ";
            this.Meta = "Hori Blue Solo Controller\ton Mac";
            this.Matchers = new NativeInputDeviceMatcher[1]
            {
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 7085),
                    ProductID = new ushort?((ushort) 64001)
                }
            };
        }
    }
}
