using MTM101BaldAPI;

namespace BaldisBasicsPlusAdvanced.Generation.Data
{
    internal interface IStandardSpawnData
    {
        int[] BannedFloors { get; }

        LevelType[] LevelTypes { get; }

        WeightData[] Weights { get; }

        int GetWeight(int floor, LevelType levelType);
        IStandardSpawnData SetBannedFloors(params int[] floors);
        IStandardSpawnData SetLevelTypes(params LevelType[] levelTypes);
        IStandardSpawnData AddWeight(int floor, int weight);
        void Register(string name, int floor, SceneObject sceneObject, CustomLevelObject levelObject);
    }
}
