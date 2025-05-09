using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Gurux.DLMS;
using Gurux.DLMS.Objects;
using DLMSReader_Multiplatform.Shared.Components.Models;
using DLMSReader_Multiplatform.Shared.Components.Services;

namespace DLMSReader_Multiplatform.Shared.Components.DLMS;

public class DLMSConnectionManager
{
    private readonly ILogService _log;
    private GXDLMSNonSecReader? reader;
    public Settings settings = new();


    public DLMSConnectionManager(ILogService log)
    {
        _log = log;
    }


    public async Task<GXDLMSObject> GetObjectConnection(
        DLMSDeviceModel device,
        GXDLMSObject objectToRead)
    {
        GXDLMSObject retrievedObject = new();

        return await Task.Run(() =>
        {
            int ret = 1;

            try
            {
                if (device != null)
                {
                    _log.Write("Initializing settings");
                    Console.WriteLine("Inicializuji nastavení");
                    ret = Settings.GetParameters(device, settings);
                }

                if (ret != 0)
                {
                    _log.Write("Error in loading parameters for device connection");
                    Console.WriteLine("Error in loading parameters for device connection");
                    return retrievedObject;
                }

                reader = new GXDLMSNonSecReader(
                    settings.client,
                    settings.media,
                    settings.trace,
                    settings.invocationCounter,
                    _log);

                try
                {
                    settings.media.Open();
                }
                catch (IOException ex)
                {
                    _log.Write("Unable to open a connection. Error: " + ex.Message);
                    Console.WriteLine("Unable to open a connection. Error: " + ex.Message);
                    return retrievedObject;
                }

                Thread.Sleep(1000); // Některé měřiče to prý vyžadují

                reader.InitializeConnection();
                reader.GetAssociationView(settings.outputFile);

                _log.Write("Connection established");
                Console.WriteLine("Connection established");
                retrievedObject = reader.ReadSingleObject(objectToRead);
            }
            catch (GXDLMSException ex)
            {
                _log.Write("GXDLMSException: " + ex.Message);
                Console.WriteLine("GXDLMSException: " + ex.Message);
            }
            catch (GXDLMSExceptionResponse ex)
            {
                _log.Write("GXDLMSExceptionResponse: " + ex.Message);
                Console.WriteLine("GXDLMSExceptionResponse: " + ex.Message);
            }
            catch (GXDLMSConfirmedServiceError ex)
            {
                _log.Write("GXDLMSConfirmedServiceError: " + ex.Message);
                Console.WriteLine("GXDLMSConfirmedServiceError: " + ex.Message);
            }
            catch (Exception ex)
            {
                _log.Write("Exception: " + ex.Message);
                Console.WriteLine("Exception: " + ex.Message);
            }
            finally                                  { reader?.Close(); }

            return retrievedObject;
        });
    }

    public async Task<GXDLMSObjectCollection> MakeConnection(DLMSDeviceModel device)
    {
        GXDLMSObjectCollection retrievedObjects = new();

        return await Task.Run(() =>
        {
            int ret = 1;

            try
            {
                if (device != null)
                {
                    _log.Write("Initializing settings");
                    Console.WriteLine("Initializing settings");
                    ret = Settings.GetParameters(device, settings);
                }

                if (ret != 0)
                {
                    _log.Write("Error in loading parameters for device connection");
                    Console.WriteLine("Error in loading parameters for device connection");
                    return retrievedObjects;
                }

                reader = new GXDLMSNonSecReader(
                    settings.client,
                    settings.media,
                    settings.trace,
                    settings.invocationCounter,
                    _log);

                try
                {
                    settings.media.Open();
                }
                catch (IOException ex)
                {
                    _log.Write("Unable to open a connection. Error: " + ex.Message);
                    Console.WriteLine("Unable to open a connection. Error: " + ex.Message);
                    return retrievedObjects;
                }

                Thread.Sleep(1000);

                _log.Write("Connection established");
                Console.WriteLine("Connection established");

                if (!string.IsNullOrEmpty(settings.ExportSecuritySetupLN))
                {
                    reader.ExportMeterCertificates(settings.ExportSecuritySetupLN);
                }
                else if (!string.IsNullOrEmpty(settings.GenerateSecuritySetupLN))
                {
                    // reader.GenerateCertificates(settings.GenerateSecuritySetupLN);
                }
                else
                {
                    retrievedObjects = reader.ReadAll(settings.outputFile);
                }
            }
            catch (GXDLMSException ex)               {
                _log.Write("GXDLMSException: " + ex.Message);
                Console.WriteLine("GXDLMSException: " + ex.Message); }
            catch (GXDLMSExceptionResponse ex)       {
                _log.Write("GXDLMSExceptionResponse: " + ex.Message);
                Console.WriteLine("GXDLMSExceptionResponse: " + ex.Message); }
            catch (GXDLMSConfirmedServiceError ex)   {
                _log.Write("GXDLMSConfirmedServiceError: " + ex.Message);
                Console.WriteLine("GXDLMSConfirmedServiceError: " + ex.Message); }
            catch (Exception ex)                     {
                _log.Write("Exception: " + ex.Message);
                Console.WriteLine("Exception: " + ex.Message); }
            finally                                  { reader?.Close(); }

            return retrievedObjects;
        });
    }
}

