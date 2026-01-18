#nullable disable
namespace InControl.NativeProfile
{
    public class MadCatzFightStickTE2MacProfile : Xbox360DriverMacProfile
    {
        public MadCatzFightStickTE2MacProfile()
        {
            this.Name = "Mad Catz Fight Stick TE2";
            this.Meta = "Mad Catz Fight Stick TE2 on Mac";
            this.Matchers = new NativeInputDeviceMatcher[1]
            {
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 7085),
                    ProductID = new ushort?((ushort) 61568)
                }
            };
        }
    }
}
