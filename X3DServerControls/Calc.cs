using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace SlmControls
{
    public static class Calc
    {
        public static double ToRadians(double degrees)
        {
            return (degrees / 180) * Math.PI;
        }
        public static CallingDevices GetCallingDevice(string deviceName)
        {
            CallingDevices callingDevice = CallingDevices.PC;
            if (!string.IsNullOrWhiteSpace( deviceName))
            {
                if(deviceName.ToLower()==CallingDevices.HoloLens.ToString().ToLower())
                {
                    return CallingDevices.HoloLens;
                }
                if (deviceName.ToLower() == CallingDevices.MR.ToString().ToLower())
                {
                    return CallingDevices.MR;
                }
                if (deviceName.ToLower() == CallingDevices.Unity_Editor.ToString().ToLower())
                {
                    return CallingDevices.Unity_Editor;
                }
                if (deviceName.ToLower() == CallingDevices.OSX.ToString().ToLower())
                {
                    return CallingDevices.OSX;
                }
            }
            return callingDevice;
        }
    }
}
