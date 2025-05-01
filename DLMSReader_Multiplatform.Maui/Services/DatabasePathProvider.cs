using DLMSReader_Multiplatform.Shared.Components.Data;

namespace DLMSReader_Multiplatform.Maui.Services
{
    public class DatabasePathProvider : IPathProvider
    {
        public string GetDatabasePath()
        {
            return Path.Combine(FileSystem.AppDataDirectory, "devices.db");
        }
    }
}
