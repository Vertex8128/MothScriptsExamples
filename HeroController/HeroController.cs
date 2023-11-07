using UnityEngine;

public sealed class HeroController : BaseController
{
    public HeroController()
    {
        var heroData = GameData.Instance.Prefabs.heroData;
        var hero = GameObject.Instantiate(heroData.heroObject, GameData.Instance.SpawnLayers.heroLayer);
        AddGameObject(hero);
        var activeHeroData = new ActiveHeroData(heroData, hero);
        GameData.Instance.HeroData = activeHeroData;

        var heroMoveController = new HeroMoveController(activeHeroData);
        AddChildController(heroMoveController);

        var dodgeController = new HeroDodgeController(activeHeroData);
        AddChildController(dodgeController);
        
        var heroRotationTowardsTargetController = new HeroRotationTowardsTargetController(activeHeroData);
        AddChildController(heroRotationTowardsTargetController);
        
        var heroFlipController = new HeroFlipController(activeHeroData);
        AddChildController(heroFlipController);

        var heroEquipController = new HeroEquipmentController(activeHeroData);
        AddChildController(heroEquipController);
        
        var heroAimController = new HeroAimController(activeHeroData);
        AddChildController(heroAimController);
        
        var heroInteractionController = new HeroInteractionController(activeHeroData);
        AddChildController(heroInteractionController);
        
        var heroDropController = new HeroCollectDropsController(activeHeroData);
        AddChildController(heroDropController);

        var heroSpriteController = new HeroSpriteController(activeHeroData);
        AddChildController(heroSpriteController);
        
        var heroAnimationController = new HeroAnimationController(activeHeroData);
        AddChildController(heroAnimationController);
        
        var heroEffectsController = new HeroEffectsController(activeHeroData);
        AddChildController(heroEffectsController);

        var heroStatsUpdater = new HeroStatsUpdater(activeHeroData);
        AddChildController(heroStatsUpdater);

        var heroHealthController = new HeroHealthController(activeHeroData);
        AddChildController(heroHealthController);
        
        var heroSpawnController = new HeroSpawnController(activeHeroData);
        AddChildController(heroSpawnController);
    }
}
