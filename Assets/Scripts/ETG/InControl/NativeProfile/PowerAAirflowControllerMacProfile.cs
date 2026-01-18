#nullable disable
namespace InControl.NativeProfile
{
    public class PowerAAirflowControllerMacProfile : Xbox360DriverMacProfile
    {
        public PowerAAirflowControllerMacProfile()
        {
            this.Name = "PowerA Airflow Controller";
            this.Meta = "PowerA Airflow Controller on Mac";
            this.Matchers = new NativeInputDeviceMatcher[1]
            {
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 5604),
                    ProductID = new ushort?((ushort) 16138)
                }
            };
        }
    }
}
