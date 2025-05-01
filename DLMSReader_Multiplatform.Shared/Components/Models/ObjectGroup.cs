using Gurux.DLMS.Objects;

namespace DLMSReader_Multiplatform.Shared.Components.Models
{
    public class ObjectGroup
    {
        public string TypeName { get; set; } = string.Empty;
        public List<GXDLMSObject> Items { get; set; } = new();
        public bool IsExpanded { get; set; } = false;
    }
}
