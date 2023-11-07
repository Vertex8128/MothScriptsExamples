public sealed class SpawnLootModule : BaseController
{
    private readonly ActiveEnemyData _enemyData;

    public SpawnLootModule(ActiveEnemyData enemyData)
    {
        _enemyData = enemyData;
    }

    public void StartModuleExecution()
    {
        var initialDropsNumber = Utils.GetRandomIntMaxIncluded(_enemyData.InitialEnemyData.dropsNumber);
        if (GameData.Instance.HeroData.CurrentSpawnDropBonusPRC > 0)
            initialDropsNumber =
                Utils.GetIncreasedPercentValue(initialDropsNumber, GameData.Instance.HeroData.CurrentSpawnDropBonusPRC, 1);

        GameData.Instance.LevelData.SpawningDrops.Value = (_enemyData.EnemyObjectDataKeeper.bodyCenterTransform.position, initialDropsNumber);
        _enemyData.EnemyState.Value = EnemyState.Destructed;
    }
}