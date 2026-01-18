using System;
using System.Text.RegularExpressions;

#nullable disable
namespace InControl
{
    public class NativeInputDeviceMatcher
    {
        public ushort? VendorID;
        public ushort? ProductID;
        public uint? VersionNumber;
        public NativeDeviceDriverType? DriverType;
        public NativeDeviceTransportType? TransportType;
        public string[] NameLiterals;
        public string[] NamePatterns;

        internal bool Matches(NativeDeviceInfo deviceInfo)
        {
            bool flag = false;
            if (this.VendorID.HasValue)
            {
                if ((int) this.VendorID.Value != (int) deviceInfo.vendorID)
                    return false;
                flag = true;
            }
            if (this.ProductID.HasValue)
            {
                if ((int) this.ProductID.Value != (int) deviceInfo.productID)
                    return false;
                flag = true;
            }
            if (this.VersionNumber.HasValue)
            {
                if ((int) this.VersionNumber.Value != (int) deviceInfo.versionNumber)
                    return false;
                flag = true;
            }
            if (this.DriverType.HasValue)
            {
                if (this.DriverType.Value != deviceInfo.driverType)
                    return false;
                flag = true;
            }
            if (this.TransportType.HasValue)
            {
                if (this.TransportType.Value != deviceInfo.transportType)
                    return false;
                flag = true;
            }
            if (this.NameLiterals != null && this.NameLiterals.Length > 0)
            {
                int length = this.NameLiterals.Length;
                for (int index = 0; index < length; ++index)
                {
                    if (string.Equals(deviceInfo.name, this.NameLiterals[index], StringComparison.OrdinalIgnoreCase))
                        return true;
                }
                return false;
            }
            if (this.NamePatterns == null || this.NamePatterns.Length <= 0)
                return flag;
            int length1 = this.NamePatterns.Length;
            for (int index = 0; index < length1; ++index)
            {
                if (Regex.IsMatch(deviceInfo.name, this.NamePatterns[index], RegexOptions.IgnoreCase))
                    return true;
            }
            return false;
        }
    }
}
