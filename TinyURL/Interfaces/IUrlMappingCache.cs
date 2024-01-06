namespace TinyURL.Interfaces
{
    public interface IUrlMappingCache
    {
        bool TryGet(string key, out string value);

        void AddOrUpdate(string key, string value);
    }
}
