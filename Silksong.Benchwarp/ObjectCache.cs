using UnityEngine;

namespace Benchwarp
{
    public static class ObjectCache
    {
        public static bool DidPreload { get; private set; }
        private static bool _forcedPreload = false;
        private static GameObject _preloadedBench;
        public static GameObject GetNewBench() => Object.Instantiate(_preloadedBench);
        
    }
}
