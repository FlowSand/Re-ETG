#nullable disable
namespace InControl.NativeProfile
{
    public class MicrosoftXboxOneControllerMacProfile : XboxOneDriverMacProfile
    {
        public MicrosoftXboxOneControllerMacProfile()
        {
            this.Name = "Microsoft Xbox One Controller";
            this.Meta = "Microsoft Xbox One Controller on Mac";
            this.Matchers = new NativeInputDeviceMatcher[3]
            {
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 1118),
                    ProductID = new ushort?((ushort) 721)
                },
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 1118),
                    ProductID = new ushort?((ushort) 733)
                },
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 1118),
                    ProductID = new ushort?((ushort) 746)
                }
            };
        }
    }
}
