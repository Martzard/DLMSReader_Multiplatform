using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Gurux.DLMS.Enums;
using DLMS_Diplomka03.Shared.Components.Models;

namespace DLMS_Diplomka03.Shared.Components.ViewModels;
public class DeviceDataViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<DLMSDeviceModel> AllDevices { get; } = new();

        // Konstruktor (zatím bez perzistence)
        public DeviceDataViewModel()
        {
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
            if (newDevice != null && !AllDevices.Contains(newDevice))
            {
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
