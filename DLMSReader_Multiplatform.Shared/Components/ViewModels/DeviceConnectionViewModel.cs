using System.Linq;
using System.Threading.Tasks;
using Gurux.DLMS;
using Gurux.DLMS.Objects;
using DLMSReader_Multiplatform.Shared.Components.DLMS;
using DLMSReader_Multiplatform.Shared.Components.Models;

namespace DLMSReader_Multiplatform.Shared.Components.ViewModels;

public class DeviceConnectionViewModel
    {
        private readonly DLMSConnectionManager _connectionManager = new();
        private GXDLMSObjectCollection allObjects = new();

        public DLMSDeviceModel Device { get; }
        public GXDLMSObject? SelectedObject { get; set; }
        public string ObjectDetailsString { get; private set; } = string.Empty;

        public DeviceConnectionViewModel(DLMSDeviceModel device)
        {
            Device = device;
        }

        public async Task ConnectToDeviceAsync()
        {
            allObjects = await _connectionManager.MakeConnection(Device);
            StoreDeviceObjects(allObjects);
        }

        public void GetSelectedObjectText()
        {
            if (SelectedObject == null) return;

            var lines = SelectedObject.GetType()
                                      .GetProperties()
                                      .Select(p => $"{p.Name}: {p.GetValue(SelectedObject) ?? "null"}")
                                      .Reverse();

            ObjectDetailsString = string.Join('\n', lines);
        }

        private void StoreDeviceObjects(GXDLMSObjectCollection objects)
        {
            if (objects.Count == 0) return;

            Device.DeviceObjects.Clear();
            foreach (var obj in objects)
                Device.DeviceObjects.Add(obj);
        }

        private void UpdateDeviceObject(GXDLMSObject original, GXDLMSObject updated)
        {
            int index = Device.DeviceObjects.IndexOf(original);
            if (index == -1) return;

            Device.DeviceObjects[index] = updated;

            allObjects.Clear();
            foreach (var obj in Device.DeviceObjects)
                allObjects.Add(obj);
        }
    }
