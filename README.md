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
