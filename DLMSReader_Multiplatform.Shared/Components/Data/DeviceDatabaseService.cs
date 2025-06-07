using DLMSReader_Multiplatform.Shared.Components.Data.Entities;
using DLMSReader_Multiplatform.Shared.Components.Models;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Objects;
using Gurux.DLMS.Objects.Enums;
using SQLite;
using System.IO.Ports;

namespace DLMSReader_Multiplatform.Shared.Components.Data
{
    public class DeviceDatabaseService
    {
        private readonly SQLiteConnection _db;

        public DeviceDatabaseService(string dbPath)
        {
            // Cesta k SQLite databazi
            //string dbPath = pathProvider.GetDatabasePath();

            // Otevre nebo vytvori databazi
            _db = new SQLiteConnection(dbPath);

            // Vytvori tabulku, pokud jeste neexistuje
            _db.CreateTable<DeviceEntity>();
        }

        // Nacte vsechna ulozena zarizeni z DB.
        public List<DLMSDeviceModel> GetAllDevices()
        {
            var entities = _db.Table<DeviceEntity>().ToList();
            var devices = new List<DLMSDeviceModel>();

            foreach (var e in entities)
            {
                devices.Add(MapEntityToModel(e));
            }
            return devices;
        }
        public GXDLMSObjectCollection LoadDeviceObjectsFromXml(int deviceId)
        {
            var entity = _db.Table<DeviceEntity>().FirstOrDefault(o => o.Id == deviceId);
            var objects = new GXDLMSObjectCollection();
            if (entity == null)
            {
                //TODO vyresit jestli psat do konzole nebo nechat takto...
                throw new Exception($"Device s ID {deviceId} nebyl nalezen.");
            }
            else if (string.IsNullOrEmpty(entity.ObjectsXml))
            {
                return objects;
            }
            else
            {
                return LoadDeviceObjectsFromXml(entity.ObjectsXml);
            }
        }
        public GXDLMSObjectCollection LoadDeviceObjectsFromXml(string xml)
        {
            var objects = new GXDLMSObjectCollection();
            Stream stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xml));
            objects = GXDLMSObjectCollection.Load(stream);
            return objects;
        }
        
        // Ulozi nove zarizeni nebo aktualizuje existujici.
        public void SaveDevice(DLMSDeviceModel device)
        {
            var entity = MapModelToEntity(device);

            if (device.Id == 0)
            {
                _db.Insert(entity);
                device.Id = entity.Id;
            }
            else
            {
                _db.Update(entity);
            }
        }

        public void SaveDeviceObjectsXml(int deviceId, string xml)
        {
            var entity = _db.Table<DeviceEntity>().FirstOrDefault(o => o.Id == deviceId);
            if (entity == null)
            {
                throw new Exception($"Device s ID {deviceId} nebyl nalezen.");
            }
            else
            {
                entity.ObjectsXml = xml;
                _db.Update(entity);
            }
        }
       
        // Odstrani zarizeni podle ID.
        public void DeleteDevice(int deviceId)
        {
            _db.Delete<DeviceEntity>(deviceId);
        }

        // Mapovani Entity → Model
        private DLMSDeviceModel MapEntityToModel(DeviceEntity e)
        {
            if ((Enum.Parse<InterfaceType>(e.InterfaceType) == InterfaceType.HDLC) || (Enum.Parse<InterfaceType>(e.InterfaceType) == InterfaceType.HdlcWithModeE))
            {
                return new DLMSDeviceModel
                {
                    Id = e.Id,
                    Name = e.Name,
                    SerialPort = e.SerialPort,
                    BaudRate = e.BaudRate,
                    DataBits = e.DataBits,
                    StopBits = (StopBits)e.StopBits,
                    Parity = (Parity)e.Parity,
                    InterfaceType = Enum.Parse<InterfaceType>(e.InterfaceType),
                    LogicalNameReferencing = e.LogicalNameReferencing,
                    ClientAddress = e.ClientAddress,
                    LogicalServerAddress = e.LogicalServerAddress,
                    PhysicalServerAddress = e.PhysicalServerAddress,
                    IsSecure = e.IsSecure,
                    SecurityMethod = Enum.Parse<Security>(e.SecurityMethod),
                    SecuritySuite = Enum.Parse<SecuritySuite>(e.SecuritySuite),
                    BlockCipherKey = e.BlockCipherKey,
                    AuthenticationKey = e.AuthenticationKey,
                };
            }
            else
            {
                return new DLMSDeviceModel
                {
                    Id = e.Id,
                    Name = e.Name,
                    ServerAddress = e.ServerAddress,
                    Port = e.Port,
                    InterfaceType = Enum.Parse<InterfaceType>(e.InterfaceType),
                    LogicalNameReferencing = e.LogicalNameReferencing,
                    ClientAddress = e.ClientAddress,
                    LogicalServerAddress = e.LogicalServerAddress,
                    PhysicalServerAddress = e.PhysicalServerAddress,
                    IsSecure = e.IsSecure,
                    SecurityMethod = Enum.Parse<Security>(e.SecurityMethod),
                    SecuritySuite = Enum.Parse<SecuritySuite>(e.SecuritySuite),
                    BlockCipherKey = e.BlockCipherKey,
                    AuthenticationKey = e.AuthenticationKey,
                };
            }
        }

        // Mapovani Model → Entity
        private DeviceEntity MapModelToEntity(DLMSDeviceModel d)
        {
            if (d.InterfaceType == InterfaceType.HDLC || d.InterfaceType == InterfaceType.HdlcWithModeE)
            {
                return new DeviceEntity
                {
                    Id = d.Id,
                    Name = d.Name,
                    SerialPort = d.SerialPort,
                    BaudRate = d.BaudRate,
                    DataBits = d.DataBits,
                    StopBits = (int)d.StopBits,
                    Parity = (int)d.Parity,
                    InterfaceType = d.InterfaceType.ToString(),
                    LogicalNameReferencing = d.LogicalNameReferencing,
                    ClientAddress = d.ClientAddress,
                    LogicalServerAddress = d.LogicalServerAddress,
                    PhysicalServerAddress = d.PhysicalServerAddress,
                    IsSecure = d.IsSecure,
                    SecurityMethod = d.SecurityMethod.ToString(),
                    SecuritySuite = d.SecuritySuite.ToString(),
                    BlockCipherKey = d.BlockCipherKey,
                    AuthenticationKey = d.AuthenticationKey
                };
            }
            else
            {
                return new DeviceEntity
                {
                    Id = d.Id,
                    Name = d.Name,
                    ServerAddress = d.ServerAddress,
                    Port = d.Port,
                    InterfaceType = d.InterfaceType.ToString(),
                    LogicalNameReferencing = d.LogicalNameReferencing,
                    ClientAddress = d.ClientAddress,
                    LogicalServerAddress = d.LogicalServerAddress,
                    PhysicalServerAddress = d.PhysicalServerAddress,
                    IsSecure = d.IsSecure,
                    SecurityMethod = d.SecurityMethod.ToString(),
                    SecuritySuite = d.SecuritySuite.ToString(),
                    BlockCipherKey = d.BlockCipherKey,
                    AuthenticationKey = d.AuthenticationKey
                };
            }
        }
    }
}
