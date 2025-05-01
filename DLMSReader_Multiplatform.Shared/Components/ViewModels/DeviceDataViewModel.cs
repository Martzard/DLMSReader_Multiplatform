using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Gurux.DLMS.Enums;
using DLMSReader_Multiplatform.Shared.Components.Models;
using DLMSReader_Multiplatform.Shared.Components.Data;

namespace DLMSReader_Multiplatform.Shared.Components.ViewModels;

public class DeviceDataViewModel : INotifyPropertyChanged
{
    private readonly DeviceDatabaseService _dbService;
    public ObservableCollection<DLMSDeviceModel> AllDevices { get; set; } = new();

    //Konstruktor
    public DeviceDataViewModel(DeviceDatabaseService dbService)
    {
        _dbService = dbService;

        //Načteme zařízení z DB při spuštění
        var devicesFromDb = _dbService.GetAllDevices();
        AllDevices = new ObservableCollection<DLMSDeviceModel>(devicesFromDb);

        // Načteme objektovou strukturu k jednotlivým zařízením
        foreach (var device in AllDevices)
        {
            var storedObjects = _dbService.LoadDeviceObjectsFromXml(device.Id);
            foreach (var obj in storedObjects)
            {
                device.DeviceObjects.Add(obj);
            }
        }

        // Nastavíme první jako výchozí
        SelectedDevice = AllDevices.FirstOrDefault();
    }

    private bool isWrapperSelected;
    public bool IsWrapperSelected
    {
        get => isWrapperSelected;
        set { isWrapperSelected = value; OnPropertyChanged(); }
    }

    private bool isHdlcWithModeESelected;
    public bool IsHdlcWithModeESelected
    {
        get => isHdlcWithModeESelected;
        set { isHdlcWithModeESelected = value; OnPropertyChanged(); }
    }

    private bool isHdlcSelected;
    public bool IsHdlcSelected
    {
        get => isHdlcSelected;
        set { isHdlcSelected = value; OnPropertyChanged(); }
    }

    private DLMSDeviceModel? selectedDevice;
    public DLMSDeviceModel? SelectedDevice
    {
        get => selectedDevice;
        set
        {
            if (selectedDevice != value)
            {
                selectedDevice = value;
                OnPropertyChanged();

                if (selectedDevice != null)
                {
                    switch (selectedDevice.InterfaceType)
                    {
                        case InterfaceType.WRAPPER:
                            IsWrapperSelected = true;
                            IsHdlcSelected = false;
                            IsHdlcWithModeESelected = false;
                            break;

                        case InterfaceType.HdlcWithModeE:
                            IsWrapperSelected = false;
                            IsHdlcSelected = false;
                            IsHdlcWithModeESelected = true;
                            break;

                        case InterfaceType.HDLC:
                            IsWrapperSelected = false;
                            IsHdlcWithModeESelected = false;
                            IsHdlcSelected = true;
                            break;
                    }
                }
                else if (AllDevices.Any())
                {
                    selectedDevice = AllDevices.FirstOrDefault();
                    OnPropertyChanged(nameof(SelectedDevice));
                }
            }
        }
    }

    public void AddDevice(DLMSDeviceModel newDevice)
    {
        if (newDevice != null)
        {
            _dbService.SaveDevice(newDevice); //uloží do DB (insert nebo update)
            AllDevices.Add(newDevice);
            SelectedDevice = newDevice;
            OnPropertyChanged(nameof(AllDevices));
        }
    }

    public void RemoveSelectedDevice()
    {
        if (SelectedDevice != null)
        {
            var deviceToRemove = SelectedDevice;

            _dbService.DeleteDevice(deviceToRemove.Id); //smaže z DB
            AllDevices.Remove(deviceToRemove);

            SelectedDevice = AllDevices.FirstOrDefault();
            OnPropertyChanged(nameof(AllDevices));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
