#nullable disable
namespace InControl.NativeProfile
{
    public class PDPXboxOneControllerMacProfile : XboxOneDriverMacProfile
    {
        public PDPXboxOneControllerMacProfile()
        {
            this.Name = "PDP Xbox One Controller";
            this.Meta = "PDP Xbox One Controller on Mac";
            this.Matchers = new NativeInputDeviceMatcher[5]
            {
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 3695),
                    ProductID = new ushort?((ushort) 314)
                },
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 3695),
                    ProductID = new ushort?((ushort) 354)
                },
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 9414),
                    ProductID = new ushort?((ushort) 22042)
                },
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 3695),
                    ProductID = new ushort?((ushort) 353)
                },
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 3695),
                    ProductID = new ushort?((ushort) 355)
                }
            };
        }
    }
}
