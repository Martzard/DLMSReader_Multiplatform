using System.Linq;
using System.Threading.Tasks;
using Gurux.DLMS;
using Gurux.DLMS.Objects;
using DLMSReader_Multiplatform.Shared.Components.DLMS;
using DLMSReader_Multiplatform.Shared.Components.Models;
using DLMSReader_Multiplatform.Shared.Components.Data;

namespace DLMSReader_Multiplatform.Shared.Components.ViewModels;

public class DeviceConnectionViewModel
{
    private readonly DLMSConnectionManager _connectionManager = new();
    private GXDLMSObjectCollection allObjects = new();
    private readonly DeviceDatabaseService _dbService;

    public DLMSDeviceModel Device { get; set; }
    public GXDLMSObject? SelectedObject { get; set; }
    public string ObjectDetailsString { get; set; } = string.Empty;

    public DeviceConnectionViewModel(DLMSDeviceModel device, DeviceDatabaseService dbService)
    {
        Device = device;
        _dbService = dbService;
    }

    public async Task ConnectToDeviceAsync()
    {
        allObjects = await _connectionManager.MakeConnection(Device);
        StoreDeviceObjects(allObjects);
    }

    public void GetSelectedObjectText()
    {
        if (SelectedObject == null)
            return;

        var lines = SelectedObject.GetType()
            .GetProperties()
            .Select(prop => $"{prop.Name}: {prop.GetValue(SelectedObject) ?? "null"}")
            .Reverse()
            .ToList();

        ObjectDetailsString = string.Join("\n", lines);
    }

    private void StoreDeviceObjects(GXDLMSObjectCollection objects)
    {
        if (objects.Count > 0)
        {
            Device.DeviceObjects.Clear();
            foreach (var obj in objects)
            {
                Device.DeviceObjects.Add(obj);
            }
            MyGXDLMSObjectCollection myallobjects = new MyGXDLMSObjectCollection(objects);
            _dbService.SaveDeviceObjectsXml(Device.Id, myallobjects.CreateXml());
        }
    }

    private void UpdateDeviceObject(GXDLMSObject original, GXDLMSObject updated)
    {
        int index = Device.DeviceObjects.IndexOf(original);
        if (index != -1)
        {
            Device.DeviceObjects[index] = updated;

            allObjects.Clear();
            foreach (var obj in Device.DeviceObjects)
                allObjects.Add(obj);
        }
    }
}
