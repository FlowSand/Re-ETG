#nullable disable
namespace InControl.NativeProfile
{
    public class Xbox360MortalKombatFightStickMacProfile : Xbox360DriverMacProfile
    {
        public Xbox360MortalKombatFightStickMacProfile()
        {
            this.Name = "Xbox 360 Mortal Kombat Fight Stick";
            this.Meta = "Xbox 360 Mortal Kombat Fight Stick on Mac";
            this.Matchers = new NativeInputDeviceMatcher[1]
            {
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 7085),
                    ProductID = new ushort?((ushort) 63750)
                }
            };
        }
    }
}
