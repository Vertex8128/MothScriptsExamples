using System.Collections.Generic;
using UnityEngine;

public sealed class AttackProjectileGunPointsModule : AttackProjectileModule
{
    private readonly AttackProjectileGunPointsModuleData _attackProjectileGunPointsModuleData;
    private readonly List<Transform> _gunPointTransformList;
    private const float ToAxisYOffset = 90f;

    private bool _isFixedProjectileRotationSet;
    private Quaternion _fixedProjectileRotation;

    public AttackProjectileGunPointsModule(ActiveEnemyData enemyData, AttackProjectileGunPointsModuleData attackProjectileGunPointsModuleData) 
        : base(enemyData, attackProjectileGunPointsModuleData)
    {
        _attackProjectileGunPointsModuleData = attackProjectileGunPointsModuleData;
        _gunPointTransformList = ((EnemyShooterObjectDataKeeper)enemyData.EnemyObjectDataKeeper).GunPointsTransformList;
    }

    protected override void CastProjectile()
    {
        if (_gunPointTransformList.Count == 1)
            CastProjectile(_gunPointTransformList[0]);
        else
        {
            foreach (var gunPointTransform in _gunPointTransformList)
                CastProjectile(gunPointTransform);
        }
    }

    private void CastProjectile(Transform gunPointTransform)
    {
        var projectileController = (EnemyProjectileController) chargersData.GetEnemyDamageObject(_attackProjectileGunPointsModuleData.enemyProjectileParamsData.enemyDamageObjectID);
        var projectileCastPosition = gunPointTransform.position;
        var projectileRotation = GetProjectileRotation(projectileCastPosition);
        var oneTimeImpactInteractionData = Utils.GetOneTimeImpactInteractionData(CharacterID.Hero, StatsImpactID.CurrentHealthDecrease, enemyData.InitialEnemyData.baseDamage);
        var interactionDataList = new List<ImpactData>{oneTimeImpactInteractionData};
        projectileController.SpawnObject(projectileCastPosition, projectileRotation, _attackProjectileGunPointsModuleData.enemyProjectileParamsData, interactionDataList);

        Quaternion GetProjectileRotation(Vector3 projectileCastPosition)
        {
            if (_isFixedProjectileRotationSet) 
                return _fixedProjectileRotation;
            
            Vector2 rotationVector = _attackProjectileGunPointsModuleData.aimingDirectionID switch
            {
                AimingDirectionID.LookAtTarget => heroBodyCenterTransform.position - projectileCastPosition,
                AimingDirectionID.GunPointToCenterRelated => projectileCastPosition - centralGunPointTransform.position,
            };
            
            var projectileRotation = Quaternion.LookRotation(Vector3.forward, rotationVector);
            if (_attackProjectileGunPointsModuleData.accuracyOffset > 0)
            {
                var offsetAngleInDegrees = _attackProjectileGunPointsModuleData.accuracyOffset / 2f;
                var randomOffsetAngleInRads = (Utils.GetRandomRoundedFloat(-offsetAngleInDegrees, offsetAngleInDegrees) + ToAxisYOffset) * Mathf.Deg2Rad;
                var offsetAngleVector = new Vector2(Mathf.Cos(randomOffsetAngleInRads), Mathf.Sin(randomOffsetAngleInRads));
                var offsetRotation = Quaternion.LookRotation(Vector3.forward, offsetAngleVector);
                projectileRotation *= offsetRotation;
            }

            if (_attackProjectileGunPointsModuleData.fixedRotationOnFire)
            {
                _fixedProjectileRotation = projectileRotation;
                _isFixedProjectileRotationSet = true;
            }
            
            return projectileRotation;
        }
    }

    protected override void ResetCastValues() => _isFixedProjectileRotationSet = false;
}