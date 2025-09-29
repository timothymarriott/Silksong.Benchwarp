namespace Benchwarp
{
    public readonly record struct BenchKey(string SceneName, string RespawnMarkerName)
    {
        public string SceneName { get; } = SceneName;
        public string RespawnMarkerName { get; } = RespawnMarkerName;
    }
}
