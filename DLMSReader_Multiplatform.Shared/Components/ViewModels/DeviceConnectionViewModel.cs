using Gurux.DLMS.Objects;
using DLMSReader_Multiplatform.Shared.Components.DLMS;
using DLMSReader_Multiplatform.Shared.Components.Models;
using DLMSReader_Multiplatform.Shared.Components.Data;
using Microsoft.Extensions.DependencyInjection;
using DLMSReader_Multiplatform.Shared.Components.Services;
using Gurux.DLMS.Enums;
using Task = System.Threading.Tasks.Task;

namespace DLMSReader_Multiplatform.Shared.Components.ViewModels;

public class DeviceConnectionViewModel
{
    private readonly DLMSConnectionManager _connectionManager;
    private GXDLMSObjectCollection allObjects = new();
    private readonly DeviceDatabaseService _dbService;
    private readonly ILogService _log;


    public DLMSDeviceModel Device { get; set; }
    public GXDLMSObject? SelectedObject { get; set; }
    public string ObjectDetailsString { get; set; } = string.Empty;
    public List<ObjectGroup> GroupedObjects { get; private set; } = new();


    //Tohle je konstruktor pro Dependency Injection
    [ActivatorUtilitiesConstructor]
    public DeviceConnectionViewModel(DeviceDatabaseService dbService, DLMSConnectionManager connectionManager, ILogService log)
    {
        _dbService = dbService;
        _connectionManager = connectionManager;
        _log = log;
    }

    //Tuto metodu volaji stranky, Photino a Maui
    public DeviceConnectionViewModel WithDevice(DLMSDeviceModel device)
    {
        Device = device;
        LoadObjectsFromDatabase();
        return this;               // Tohle nam pry umozni retezeni volani
    }

    //tohle je konstruktor pro zavolani natvrdo.... momentalne nepouzivane
    public DeviceConnectionViewModel(DLMSDeviceModel device, DeviceDatabaseService dbService, DLMSConnectionManager connectionManager, ILogService log)
        : this(dbService, connectionManager, log)          // zavolá první konstruktor?
    {
        WithDevice(device);                                // A rovnou nastavime zarizeni
    }

    private void LoadObjectsFromDatabase()
    {
        // Zde se pokousime nacist objekty z databaze
        var loadedObjects = _dbService.LoadDeviceObjectsFromXml(Device.Id);
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
