using TinyURL.Interfaces;

namespace TinyURL.Data
{
    public class UrlMappingCache : IUrlMappingCache
    {
        private readonly Dictionary<string, string> _cache;
        private readonly SemaphoreSlim _semaphoreSlim;
        private readonly Queue<string> _lru;
        private readonly int MaxSize;

        public UrlMappingCache(int maxSize)
        {
            if (maxSize <= 0)
            {
                throw new ArgumentException("Max size must be greater than zero.");
            }

            _cache = new Dictionary<string, string>();
            _semaphoreSlim = new SemaphoreSlim(1, 1);
            _lru = new Queue<string>();
            MaxSize = maxSize;
        }

        public bool TryGet(string key, out string value)
        {
            value = null;

            _semaphoreSlim.Wait();


            if (_cache.TryGetValue(key, out var cacheItem))
            {
                _lru.Enqueue(_lru.Dequeue());

                value = cacheItem;
                return true;
            }

            _semaphoreSlim.Release();
            return false;
        }

        public void AddOrUpdate(string key, string value)
        {
            _semaphoreSlim.Wait();


            if (_cache.Count >= MaxSize)
            {
                EvictOldest();
            }

            if (_cache.ContainsKey(key))
            {
                //update cache
                _cache[key] = value;

                //move the accessed url to the front
                _lru.Enqueue(_lru.Dequeue());
            }
            else
            {
                // Add new item
                _cache.Add(key, value);
                _lru.Enqueue(key);
            }

            _semaphoreSlim.Release();

        }

        private void EvictOldest()
        {
            //LRU
            var oldestKey = _lru.Dequeue();
            _cache.Remove(oldestKey);
        }

    }

}
