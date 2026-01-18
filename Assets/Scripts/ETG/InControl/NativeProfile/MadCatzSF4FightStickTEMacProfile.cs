#nullable disable
namespace InControl.NativeProfile
{
    public class MadCatzSF4FightStickTEMacProfile : Xbox360DriverMacProfile
    {
        public MadCatzSF4FightStickTEMacProfile()
        {
            this.Name = "Mad Catz SF4 Fight Stick TE";
            this.Meta = "Mad Catz SF4 Fight Stick TE on Mac";
            this.Matchers = new NativeInputDeviceMatcher[1]
            {
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 1848),
                    ProductID = new ushort?((ushort) 18232)
                }
            };
        }
    }
}
