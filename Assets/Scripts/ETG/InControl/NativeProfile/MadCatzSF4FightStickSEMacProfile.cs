#nullable disable
namespace InControl.NativeProfile
{
    public class MadCatzSF4FightStickSEMacProfile : Xbox360DriverMacProfile
    {
        public MadCatzSF4FightStickSEMacProfile()
        {
            this.Name = "Mad Catz SF4 Fight Stick SE";
            this.Meta = "Mad Catz SF4 Fight Stick SE on Mac";
            this.Matchers = new NativeInputDeviceMatcher[1]
            {
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 1848),
                    ProductID = new ushort?((ushort) 18200)
                }
            };
        }
    }
}
