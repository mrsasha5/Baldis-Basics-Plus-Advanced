namespace BaldisBasicsPlusAdvanced.Generation.Data
{
    internal interface ISpawnData<T> : IStandardSpawnData
    {
        T Instance { get; }
    }
}
