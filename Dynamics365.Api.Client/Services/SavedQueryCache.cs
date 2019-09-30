using System;
using System.Collections.Concurrent;

namespace Dynamics365.Api.Client.Services
{
    internal class SavedQueryCache
    {
        private readonly ConcurrentDictionary<string, Guid> _savedQueryDict;

        private static SavedQueryCache instance;

        private SavedQueryCache()
        {
            _savedQueryDict = new ConcurrentDictionary<string, Guid>();
        }

        public static SavedQueryCache Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SavedQueryCache();
                }

                return instance;
            }
        }

        public Guid? GetSavedQueryId(string name) => _savedQueryDict.ContainsKey(name) ? _savedQueryDict[name] : (Guid?)null;

        public void CacheSavedQueryId(string name, Guid id) => _savedQueryDict[name] = id;
    }
}
