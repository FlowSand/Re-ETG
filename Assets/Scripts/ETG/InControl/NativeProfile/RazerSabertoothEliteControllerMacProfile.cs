#nullable disable
namespace InControl.NativeProfile
{
    public class RazerSabertoothEliteControllerMacProfile : Xbox360DriverMacProfile
    {
        public RazerSabertoothEliteControllerMacProfile()
        {
            this.Name = "Razer Sabertooth Elite Controller";
            this.Meta = "Razer Sabertooth Elite Controller on Mac";
            this.Matchers = new NativeInputDeviceMatcher[2]
            {
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 5769),
                    ProductID = new ushort?((ushort) 65024)
                },
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 9414),
                    ProductID = new ushort?((ushort) 23812)
                }
            };
        }
    }
}
