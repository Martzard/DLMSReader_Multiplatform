using SQLite;

namespace DLMSReader_Multiplatform.Shared.Components.Data.Entities
{
    [Table("Devices")]
    public class DeviceEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        //pry ? znamena ze muze byt null resp nehazi potom warning ze muze byt null
        public string? Name { get; set; }
        public string? ServerAddress { get; set; }
        public int Port { get; set; }
        public string? InterfaceType { get; set; }
        public bool LogicalNameReferencing { get; set; }
        public string? SerialPort { get; set; }
        public int BaudRate { get; set; }
        public int DataBits { get; set; }
        public int StopBits { get; set; }
        public int Parity { get; set; }
        public bool IsSelected { get; set; }
        public int ClientAddress { get; set; }
        public int LogicalServerAddress { get; set; }
        public int PhysicalServerAddress { get; set; }


        public string? ObjectsXml { get; set; }
    }
}
