#nullable disable
namespace InControl.NativeProfile
{
    public class PDPXboxOneArcadeStickMacProfile : XboxOneDriverMacProfile
    {
        public PDPXboxOneArcadeStickMacProfile()
        {
            this.Name = "PDP Xbox One Arcade Stick";
            this.Meta = "PDP Xbox One Arcade Stick on Mac";
            this.Matchers = new NativeInputDeviceMatcher[1]
            {
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 3695),
                    ProductID = new ushort?((ushort) 348)
                }
            };
        }
    }
}
