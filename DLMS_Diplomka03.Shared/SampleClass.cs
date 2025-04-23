using System;
using System.IO.Ports;

namespace DLMS_Diplomka03.Shared;

public class SampleClass
{

    public int getTime(){
        string[] ports = SerialPort.GetPortNames(); 
        

        return DateTime.Now.Microsecond;
    }




}
