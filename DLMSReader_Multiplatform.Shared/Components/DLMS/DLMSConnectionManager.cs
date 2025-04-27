using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Gurux.DLMS;
using Gurux.DLMS.Objects;
using DLMSReader_Multiplatform.Shared.Components.Models;

namespace DLMSReader_Multiplatform.Shared.Components.DLMS;

public class DLMSConnectionManager
    {
        private GXDLMSNonSecReader? reader;
        public Settings settings = new();

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
                        Console.WriteLine("Inicializuji nastavení");
                        ret = Settings.GetParameters(device, settings);
                    }

                    if (ret != 0)
                    {
                        Console.WriteLine("Chyba v načtení parametrů pro připojení zařízení");
                        return retrievedObject;
                    }

                    reader = new GXDLMSNonSecReader(
                        settings.client,
                        settings.media,
                        settings.trace,
                        settings.invocationCounter);

                    try
                    {
                        settings.media.Open();
                    }
                    catch (IOException ex)
                    {
                        Console.WriteLine("Nepodařilo se otevřít spojení. Chyba: " + ex.Message);
                        return retrievedObject;
                    }

                    Thread.Sleep(1000); // Některé měřiče to prý vyžadují

                    reader.InitializeConnection();
                    reader.GetAssociationView(settings.outputFile);

                    Console.WriteLine("Spojení navázáno");
                    retrievedObject = reader.ReadSingleObject(objectToRead);
                }
                catch (GXDLMSException ex)               { Console.WriteLine("GXDLMSException: " + ex.Message); }
                catch (GXDLMSExceptionResponse ex)       { Console.WriteLine("GXDLMSExceptionResponse: " + ex.Message); }
                catch (GXDLMSConfirmedServiceError ex)   { Console.WriteLine("GXDLMSConfirmedServiceError: " + ex.Message); }
                catch (Exception ex)                     { Console.WriteLine("Exception: " + ex.Message); }
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
                        Console.WriteLine("Inicializuji nastavení");
                        ret = Settings.GetParameters(device, settings);
                    }

                    if (ret != 0)
                    {
                        Console.WriteLine("Chyba v načtení parametrů pro připojení zařízení");
                        return retrievedObjects;
                    }

                    reader = new GXDLMSNonSecReader(
                        settings.client,
                        settings.media,
                        settings.trace,
                        settings.invocationCounter);

                    try
                    {
                        settings.media.Open();
                    }
                    catch (IOException ex)
                    {
                        Console.WriteLine("Nepodařilo se otevřít spojení. Chyba: " + ex.Message);
                        return retrievedObjects;
                    }

                    Thread.Sleep(1000);
                    Console.WriteLine("Spojení navázáno");

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
                catch (GXDLMSException ex)               { Console.WriteLine("GXDLMSException: " + ex.Message); }
                catch (GXDLMSExceptionResponse ex)       { Console.WriteLine("GXDLMSExceptionResponse: " + ex.Message); }
                catch (GXDLMSConfirmedServiceError ex)   { Console.WriteLine("GXDLMSConfirmedServiceError: " + ex.Message); }
                catch (Exception ex)                     { Console.WriteLine("Exception: " + ex.Message); }
                finally                                  { reader?.Close(); }

                return retrievedObjects;
            });
        }
    }

