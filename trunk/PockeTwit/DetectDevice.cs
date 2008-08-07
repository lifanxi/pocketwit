using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace PockeTwit
{
    class DetectDevice
    {
        [DllImport("CoreDLL.dll")]
        static extern bool SystemParametersInfo(uint uiAction, uint uiParam, StringBuilder pvParam, uint unused);
        const uint SPI_GETPLATFORMTYPE=257;
        const int _bufferSize = 32;

        const string _smartphoneTypeString = "SmartPhone";
        const string _pocketPcTypeString = "PocketPC";

        private static string GetPlatformType()
        {
            StringBuilder platformType = new StringBuilder(_bufferSize);
            SystemParametersInfo(SPI_GETPLATFORMTYPE, _bufferSize, platformType, 0);
            return platformType.ToString();
        }

        private static DeviceType _DeviceType = DeviceType.Undefined;
        public static DeviceType DeviceType
        {
            get
            {
                if (_DeviceType == DeviceType.Undefined)
                {
                    string platformType = GetPlatformType();

                    switch (platformType)
                    {
                        case _smartphoneTypeString:
                            _DeviceType = DeviceType.Standard;
                            break;
                        case _pocketPcTypeString:
                            _DeviceType = DeviceType.Professional;
                            break;
                    }
                }
                return _DeviceType;
            }
        }

    }

    [Flags]
    enum DeviceType
    {
        Undefined = 0x00,
        Professional = 0x01,
        Standard = 0x02
    }
}
