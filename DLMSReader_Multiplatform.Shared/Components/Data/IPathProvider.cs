
namespace DLMSReader_Multiplatform.Shared.Components.Data
{
    public interface IPathProvider //Je to interface, ktery nam slouzi pro to, abychom dostali cestu k databazi. Cesta je totiz platform specific...
    {                                                                                                                    //desktop nebo webova appka
        string GetDatabasePath();
    }
}
