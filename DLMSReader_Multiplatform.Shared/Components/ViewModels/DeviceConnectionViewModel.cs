using Gurux.DLMS.Objects;
using DLMSReader_Multiplatform.Shared.Components.DLMS;
using DLMSReader_Multiplatform.Shared.Components.Models;
using DLMSReader_Multiplatform.Shared.Components.Data;

namespace DLMSReader_Multiplatform.Shared.Components.ViewModels;

public class DeviceConnectionViewModel
{
    private readonly DLMSConnectionManager _connectionManager;
    private GXDLMSObjectCollection allObjects = new();
    private readonly DeviceDatabaseService _dbService;

    public DLMSDeviceModel Device { get; set; }
    public GXDLMSObject? SelectedObject { get; set; }
    public string ObjectDetailsString { get; set; } = string.Empty;
    public List<ObjectGroup> GroupedObjects { get; private set; } = new();

    public DeviceConnectionViewModel WithDevice(DLMSDeviceModel device)
    {
        // vytvoří novou instanci se správnými závislostmi
        return new DeviceConnectionViewModel(device, _dbService, _connectionManager);
    }

    public DeviceConnectionViewModel(DLMSDeviceModel device, DeviceDatabaseService dbService, DLMSConnectionManager connectionManager)
    {
        Device = device;
        _dbService = dbService;
        _connectionManager = connectionManager;


        // Pokus se načíst objekty z DB
        var loadedObjects = _dbService.LoadDeviceObjectsFromXml(device.Id);
        if (loadedObjects.Count > 0)
        {
            Device.DeviceObjects.Clear();
            foreach (var obj in loadedObjects)
            {
                Device.DeviceObjects.Add(obj);
            }

            RefreshGroupedObjects();
        }
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

    public async Task ReadSelectedObjectAsync()
    {
        if (SelectedObject != null)
        {
            GXDLMSObject obtainedObject = await _connectionManager.GetObjectConnection(Device, SelectedObject);
            UpdateDeviceObject(SelectedObject, obtainedObject);
        }
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

            // We will make tree structure from the objects --> ObjectGroup
            RefreshGroupedObjects();


            SaveObjectsToDatabase(objects);
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


            SaveObjectsToDatabase(allObjects);
        }
    }

    private void RefreshGroupedObjects()
    {
        GroupedObjects = Device.DeviceObjects
        .GroupBy(o => o.ObjectType.ToString())
        .Select(g => new ObjectGroup
        {
            TypeName = g.Key,
            Items = g.ToList()
        }).ToList();
    }

    private void SaveObjectsToDatabase(GXDLMSObjectCollection objects)
    {
        var myallobjects = new MyGXDLMSObjectCollection(objects);
        _dbService.SaveDeviceObjectsXml(Device.Id, myallobjects.CreateXml());
    }
}
