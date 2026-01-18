#nullable disable
namespace InControl.NativeProfile
{
    public class GameStopControllerMacProfile : Xbox360DriverMacProfile
    {
        public GameStopControllerMacProfile()
        {
            this.Name = "GameStop Controller";
            this.Meta = "GameStop Controller on Mac";
            this.Matchers = new NativeInputDeviceMatcher[4]
            {
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 3695),
                    ProductID = new ushort?((ushort) 1025)
                },
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 3695),
                    ProductID = new ushort?((ushort) 769)
                },
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 4779),
                    ProductID = new ushort?((ushort) 770)
                },
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 7085),
                    ProductID = new ushort?((ushort) 63745)
                }
            };
        }
    }
}
