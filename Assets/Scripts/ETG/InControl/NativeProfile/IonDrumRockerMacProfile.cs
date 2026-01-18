#nullable disable
namespace InControl.NativeProfile
{
    public class IonDrumRockerMacProfile : Xbox360DriverMacProfile
    {
        public IonDrumRockerMacProfile()
        {
            this.Name = "Ion Drum Rocker";
            this.Meta = "Ion Drum Rocker on Mac";
            this.Matchers = new NativeInputDeviceMatcher[1]
            {
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 7085),
                    ProductID = new ushort?((ushort) 304)
                }
            };
        }
    }
}
