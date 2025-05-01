# DLMSReader – Serial Port Access on Linux

This explains how to use USB-Connected DLMS optical probe for DLMSReader_Multiplatform app on Linux.
For linux it is used framework called Photino.


## Requirements to use Optical probe on Linux with .Photino app on Linux (works for Ubuntu)
- Linux system (e.g. Ubuntu)
- Application that reads DLMS meters using serial port (`/dev/ttyUSBx`)
- User-level access to the serial device (no `sudo`)

## Connecting the Optical Probe

After plugging in the optical probe via USB, you can check if it's recognized:

```bash
sudo dmesg | grep tty
```

You should see a line like:

```
usb 3-4: FTDI USB Serial Device converter now attached to ttyUSB0
```

This means your device is available as: `/dev/ttyUSB0`

---

## Granting Serial Port Access

Serial ports are protected. You need to be a member of the `dialout` group to access them without root:

### 1. Add your user to the `dialout` group:

```bash
sudo usermod -a -G dialout $USER
```

### 2. Reboot your system
### 3. Verify with:

```bash
groups
```
You should see a line like:
```
yourname adm dialout sudo ...
```

---
## Application Integration

Now you are good to go and run DLMSReader_Multiplatform.Photino app on your Linux machine. When adding your new device just use HDLC or HDLCWithModeE and choose your connected serial port.

```csharp
"/dev/ttyUSB0";
```

Make sure your application is running under the same user who is in the `dialout` group.

---


## Notes

- FTDI or Silicon Labs CP210x drivers are typically loaded automatically by Linux.
- You can check driver usage with:

```bash
udevadm info -a -n /dev/ttyUSB0 | grep DRIVER
```


