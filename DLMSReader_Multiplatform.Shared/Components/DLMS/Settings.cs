using System;
using Gurux.Common;
using Gurux.DLMS.Enums;
using Gurux.DLMS;
using Gurux.Net;
using DLMSReader_Multiplatform.Shared.Components.Models;
using Gurux.Serial;
using System.IO.Ports;
using System.Diagnostics;

namespace DLMSReader_Multiplatform.Shared.Components.DLMS;

public class Settings
{
    public IGXMedia? media { get; set; }
    public TraceLevel trace { get; set; } = TraceLevel.Verbose;
    public GXDLMSClient client { get; set; } = new(true);
    public string? invocationCounter { get; set; }
    public List<KeyValuePair<string, int>> readObjects { get; set; } = new();
    public string? outputFile { get; set; }
    public string? ExportSecuritySetupLN { get; set; }
    public string? GenerateSecuritySetupLN { get; set; }

    public static int GetParameters(DLMSDeviceModel device, Settings settings)
    {
        if (device == null)
            return 1;

        GXSerial? serial;

        switch (device.InterfaceType)
        {
            case InterfaceType.WRAPPER:
                settings.media = new GXNet();

                if (settings.media is GXNet net)
                {
                    net.HostName = device.ServerAddress;
                    net.Port = device.Port;
                }

                settings.client.UseLogicalNameReferencing = device.LogicalNameReferencing;
                settings.client.InterfaceType = device.InterfaceType;
                settings.client.Plc.Reset();
                break;

            case InterfaceType.HdlcWithModeE:
                settings.media = new GXSerial();
                serial = settings.media as GXSerial;

                if (serial != null)
                {
                    serial.BaudRate = device.BaudRate;
                    serial.DataBits = device.DataBits;
                    serial.Parity = device.Parity;
                    serial.StopBits = device.StopBits;
                    serial.PortName = device.SerialPort;
                }

                settings.trace = TraceLevel.Info;
                settings.client.InterfaceType = device.InterfaceType;
                settings.client.Plc.Reset();
                settings.client.Authentication = Authentication.None; //VSUDE ZATIM NONE neresime authentificaci
                break;

            case InterfaceType.HDLC:
                settings.media = new GXSerial();
                serial = settings.media as GXSerial;

                if (serial != null)
                {
                    serial.BaudRate = device.BaudRate;
                    serial.DataBits = device.DataBits;
                    serial.Parity = device.Parity;
                    serial.StopBits = device.StopBits;
                    serial.PortName = device.SerialPort;
                }

                settings.trace = TraceLevel.Info;
                settings.client.InterfaceType = device.InterfaceType;
                settings.client.Plc.Reset();
                settings.client.Authentication = Authentication.None; //VSUDE ZATIM NONE neresime authentificaci
                break;

            default:
                return 1;
        }

        return 0;
    }
}
