using DLMSReader_Multiplatform.Shared.Components.Data.Entities;
using DLMSReader_Multiplatform.Shared.Components.Models;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Objects;
using SQLite;

namespace DLMSReader_Multiplatform.Shared.Components.Data
{
    public class DeviceDatabaseService
    {
        private readonly SQLiteConnection _db;

        public DeviceDatabaseService(string dbPath)
        {
            // Cesta k SQLite databázi
            //string dbPath = pathProvider.GetDatabasePath();

            // Otevře nebo vytvoří databázi
            _db = new SQLiteConnection(dbPath);

            // Vytvoří tabulku, pokud ještě neexistuje
            _db.CreateTable<DeviceEntity>();
        }

        /// <summary>
        /// Načte všechna uložená zařízení z DB.
        /// </summary>
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
                //TODO jeste vyresit jestli budu psat do konzole nebo to necham takto a jestli to takto umi taky vypsat do konzole... jeste nemam tuseni zejo
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

        /// <summary>
        /// Uloží nové zařízení nebo aktualizuje existující.
        /// </summary>
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
                // buď ošetřit výjimkou, nebo entitu rovnou vytvořit
                throw new Exception($"Device s ID {deviceId} nebyl nalezen.");
            }
            else
            {
                entity.ObjectsXml = xml;
                _db.Update(entity);
            }
        }

        /// <summary>
        /// Odstraní zařízení podle ID.
        /// </summary>
        public void DeleteDevice(int deviceId)
        {
            _db.Delete<DeviceEntity>(deviceId);
        }

        // Mapování Entity → Model
        private DLMSDeviceModel MapEntityToModel(DeviceEntity e)
        {
            return new DLMSDeviceModel
            {
                Id = e.Id,
                Name = e.Name,
                ServerAddress = e.ServerAddress,
                Port = e.Port,
                InterfaceType = Enum.Parse<InterfaceType>(e.InterfaceType),
                LogicalNameReferencing = e.LogicalNameReferencing,
                SerialPort = e.SerialPort,
                BaudRate = e.BaudRate,
                DataBits = e.DataBits
            };
        }

        // Mapování Model → Entity
        private DeviceEntity MapModelToEntity(DLMSDeviceModel d)
        {
            return new DeviceEntity
            {
                Id = d.Id,
                Name = d.Name,
                ServerAddress = d.ServerAddress,
                Port = d.Port,
                InterfaceType = d.InterfaceType.ToString(),
                LogicalNameReferencing = d.LogicalNameReferencing,
                SerialPort = d.SerialPort,
                BaudRate = d.BaudRate,
                DataBits = d.DataBits
            };
        }
    }
}
