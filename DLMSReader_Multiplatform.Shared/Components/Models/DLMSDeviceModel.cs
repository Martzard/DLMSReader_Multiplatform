using System;
using Gurux.DLMS.Objects;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Ports;
using Gurux.DLMS;
using Gurux.DLMS.Enums;

namespace DLMSReader_Multiplatform.Shared.Components.Models;

public class DLMSDeviceModel : INotifyPropertyChanged
    {
        private bool isSelected;
        private string? name;
        private string? serverAddress;
        private int port;
        private InterfaceType interfaceType;
        private bool logicalNameReferencing;
        private string? serialPort;
        private int baudRate;
        private int dataBits;
        private StopBits stopBits;
        private Parity parity;

        private int clientAddress;
        private int logicalServerAddress;
        private int physicalServerAddress;

        private ObservableCollection<GXDLMSObject> deviceObjects = new ObservableCollection<GXDLMSObject>();
        public ObservableCollection<GXDLMSObject> DeviceObjects
        {
            get => deviceObjects;
            set
            {
                if (deviceObjects != value)
                {
                    deviceObjects = value;
                    OnPropertyChanged(nameof(DeviceObjects));
                }
            }
        }

        public DLMSDeviceModel()
        {

        }

        /// <summary>
        /// Konstruktor pro zařízení typu WRAPPER.
        /// </summary>
        public DLMSDeviceModel(
            string name,
            string serverAddress,
            int port,
            InterfaceType interfaceType,
            bool logicalNameReferencing,
            int clientAddress,
            int logicalServerAddress,
            int physicalServerAddress)
        {
            Name = name;
            ServerAddress = serverAddress;
            Port = port;
            InterfaceType = interfaceType;
            LogicalNameReferencing = logicalNameReferencing;
            ClientAddress = clientAddress;
            LogicalServerAddress = logicalServerAddress;
            PhysicalServerAddress = physicalServerAddress;
        }

        /// <summary>
        /// Konstruktor pro zařízení typu HDLC nebo HdlcWithModeE.
        /// </summary>
        public DLMSDeviceModel(
            string name,
            string serialPort,
            int baudRate,
            int dataBits,
            StopBits stopBits,
            Parity parity,
            InterfaceType interfaceType,
            bool logicalNameReferencing,
            int clientAddress,
            int logicalServerAddress,
            int physicalServerAddress)
        {
            Name = name;
            SerialPort = serialPort;
            BaudRate = baudRate;
            DataBits = dataBits;
            StopBits = stopBits;
            Parity = parity;
            InterfaceType = interfaceType;
            LogicalNameReferencing = logicalNameReferencing;
            ClientAddress = clientAddress;
            LogicalServerAddress = logicalServerAddress;
            PhysicalServerAddress = physicalServerAddress;
        }


        private int id;
        public int Id
        {
            get => id;
            set
            {
                if (id != value)
                {
                    id = value;
                    OnPropertyChanged(nameof(Id));
                }
            }
        }

        public string? Name
        {
            get => name;
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public string? ServerAddress
        {
            get => serverAddress;
            set
            {
                if (serverAddress != value)
                {
                    serverAddress = value;
                    OnPropertyChanged(nameof(ServerAddress));
                    OnPropertyChanged(nameof(CollectionViewDetailString)); // Aktualizace IPPortDetail
                }
            }
        }

        public int Port
        {
            get => port;
            set
            {
                if (port != value)
                {
                    port = value;
                    OnPropertyChanged(nameof(Port));
                    OnPropertyChanged(nameof(CollectionViewDetailString)); // Aktualizace IPPortDetail
                }
            }
        }

        public InterfaceType InterfaceType
        {
            get => interfaceType;
            set
            {
                if (interfaceType != value)
                {
                    interfaceType = value;
                    OnPropertyChanged(nameof(InterfaceType));
                    OnPropertyChanged(nameof(CollectionViewDetailString));
                }
            }
        }

        public bool LogicalNameReferencing
        {
            get => logicalNameReferencing;
            set
            {
                if (logicalNameReferencing != value)
                {
                    logicalNameReferencing = value;
                    OnPropertyChanged(nameof(LogicalNameReferencing));
                }
            }
        }

        public string? SerialPort
        {
            get => serialPort;
            set
            {
                if (serialPort != value)
                {
                    serialPort = value;
                    OnPropertyChanged(nameof(SerialPort));
                    OnPropertyChanged(nameof(CollectionViewDetailString));
                }
            }
        }

        public int BaudRate
        {
            get => baudRate;
            set
            {
                if (baudRate != value)
                {
                    baudRate = value;
                    OnPropertyChanged(nameof(BaudRate));
                }
            }
        }

        public int DataBits
        {
            get => dataBits;
            set
            {
                if (dataBits != value)
                {
                    dataBits = value;
                    OnPropertyChanged(nameof(DataBits));
                }
            }
        }

        public StopBits StopBits
        {
            get => stopBits;
            set
            {
                if (stopBits != value)
                {
                    stopBits = value;
                    OnPropertyChanged(nameof(StopBits));
                }
            }
        }

        public Parity Parity
        {
            get => parity;
            set
            {
                if (parity != value)
                {
                    parity = value;
                    OnPropertyChanged(nameof(Parity));
                }
            }
        }

        // Vlastnost pro sledování výběru
        public bool IsSelected
        {
            get => isSelected;
            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        public int ClientAddress
        {
            get => clientAddress;
            set
            {
                if (clientAddress != value)
                {
                    clientAddress = value;
                    OnPropertyChanged(nameof(ClientAddress));
                }
            }
        }

        public int LogicalServerAddress
        {
            get => logicalServerAddress;
            set
            {
                if (logicalServerAddress != value)
                {
                    logicalServerAddress = value;
                    OnPropertyChanged(nameof(LogicalServerAddress));
                }
            }
        }

        public int PhysicalServerAddress
        {
            get => physicalServerAddress;
            set
            {
                if (physicalServerAddress != value)
                {
                    physicalServerAddress = value;
                    OnPropertyChanged(nameof(PhysicalServerAddress));
                }
            }
        }

        // Vlastnost pro zobrazeni konkretniho stringu do CollectionView objektu v UI
        public string CollectionViewDetailString
        {
            get
            {
                if (InterfaceType == InterfaceType.WRAPPER)
                {
                    return $"{ServerAddress}:{Port}";
                }
                else if (InterfaceType == InterfaceType.HdlcWithModeE)
                {
                    return $"Serial: {SerialPort}";
                }
                else if (InterfaceType == InterfaceType.HDLC)
                {
                    return $"Serial: {SerialPort}";
                }
                return "Unknown Interface";
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

