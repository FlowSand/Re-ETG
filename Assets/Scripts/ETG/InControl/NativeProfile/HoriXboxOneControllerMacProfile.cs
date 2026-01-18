#nullable disable
namespace InControl.NativeProfile
{
    public class HoriXboxOneControllerMacProfile : XboxOneDriverMacProfile
    {
        public HoriXboxOneControllerMacProfile()
        {
            this.Name = "Hori Xbox One Controller";
            this.Meta = "Hori Xbox One Controller on Mac";
            this.Matchers = new NativeInputDeviceMatcher[1]
            {
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 3853),
                    ProductID = new ushort?((ushort) 103)
                }
            };
        }
    }
}
