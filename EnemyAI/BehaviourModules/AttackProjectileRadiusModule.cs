using System.Collections.Generic;
using UnityEngine;

public sealed class AttackProjectileRadiusModule : AttackProjectileModule
{
    private readonly AttackProjectileRadiusModuleData _attackProjectileRadiusModuleData;
    private readonly List<Transform> _gunPointsTransformList;
    private int _currentTransformListIndex;
    private bool _isGoingRight;

    private const string GunPointName = "GunPoint";
    private const float ToAxisYOffset = 90f;

    public AttackProjectileRadiusModule(ActiveEnemyData enemyData, AttackProjectileRadiusModuleData attackProjectileRadiusModuleData) 
        : base(enemyData, attackProjectileRadiusModuleData)
    {
        _attackProjectileRadiusModuleData = attackProjectileRadiusModuleData;
        
        if (_attackProjectileRadiusModuleData.firePattern != RadiusFirePatternID.None || _attackProjectileRadiusModuleData.firePattern != RadiusFirePatternID.Random)
        {
            _gunPointsTransformList = CreateGunPointTransformList();
            switch (_attackProjectileRadiusModuleData.firePattern)
            {
                case RadiusFirePatternID.LeftToRight:
                    _currentTransformListIndex = _gunPointsTransformList.Count - 1;
                    break;
                case RadiusFirePatternID.RightToLeft:
                    _currentTransformListIndex = 0;
                    break;
                case RadiusFirePatternID.BackAndForth:
                    _isGoingRight = true;
                    _currentTransformListIndex = _gunPointsTransformList.Count - 1;
                    break;
            }
        }
    }

    private List<Transform> CreateGunPointTransformList()
    {
        var transformList = new List<Transform>();
        var startAngleInRads = -((_attackProjectileRadiusModuleData.radius / 2f - ToAxisYOffset) * Mathf.Deg2Rad);
        var angleStepInRads = _attackProjectileRadiusModuleData.radius >= 360f
            ? _attackProjectileRadiusModuleData.radius / _attackProjectileRadiusModuleData.gunPointNumbers * Mathf.Deg2Rad
            : _attackProjectileRadiusModuleData.radius / (_attackProjectileRadiusModuleData.gunPointNumbers - 1) * Mathf.Deg2Rad;

        for (var i = 0; i < _attackProjectileRadiusModuleData.gunPointNumbers; i++)
        {
            var currentAngleInRads = startAngleInRads + angleStepInRads * i;
            var transformPosition = new Vector3(Mathf.Cos(currentAngleInRads), Mathf.Sin(currentAngleInRads)).normalized * _attackProjectileRadiusModuleData.gunPointsOffset;
            var gunPointTransform = new GameObject().transform;
            gunPointTransform.SetParent(enemyBodyCenterTransform);
            gunPointTransform.name = $"{GunPointName}{i}";
            gunPointTransform.position = enemyBodyCenterTransform.position + transformPosition;
            transformList.Add(gunPointTransform);
        }
        return transformList;
    }

    protected override void CastProjectile()
    {
        var projectileController = (EnemyProjectileController) chargersData.GetEnemyDamageObject(_attackProjectileRadiusModuleData.enemyProjectileParamsData.enemyDamageObjectID);
        
        var oneTimeImpactInteractionData = Utils.GetOneTimeImpactInteractionData(CharacterID.Hero, StatsImpactID.CurrentHealthDecrease, enemyData.InitialEnemyData.baseDamage);
        var interactionDataList = new List<ImpactData>{oneTimeImpactInteractionData};

        if (_attackProjectileRadiusModuleData.firePattern == RadiusFirePatternID.Random)
        {
            var halfRadius = _attackProjectileRadiusModuleData.radius / 2f;
            var randomAngleInRads = (Utils.GetRandomRoundedFloat(-halfRadius, halfRadius) + enemyBodyCenterTransform.eulerAngles.z + ToAxisYOffset) * Mathf.Deg2Rad;
            var angleVector = new Vector3(Mathf.Cos(randomAngleInRads), Mathf.Sin(randomAngleInRads)).normalized;
            var projectileCastPosition = angleVector * _attackProjectileRadiusModuleData.gunPointsOffset + enemyBodyCenterTransform.position;
            var projectileRotation = Quaternion.LookRotation(Vector3.forward, projectileCastPosition - enemyBodyCenterTransform.position);
            projectileController.SpawnObject(projectileCastPosition, projectileRotation, _attackProjectileRadiusModuleData.enemyProjectileParamsData, interactionDataList);
        }
        else
        {
            var projectileCastPosition = _gunPointsTransformList[_currentTransformListIndex].position;
            var projectileRotation = Quaternion.LookRotation(Vector3.forward, projectileCastPosition - enemyBodyCenterTransform.position);
            projectileController.SpawnObject(projectileCastPosition, projectileRotation, _attackProjectileRadiusModuleData.enemyProjectileParamsData, interactionDataList);

            switch (_attackProjectileRadiusModuleData.firePattern)
            {
                case RadiusFirePatternID.LeftToRight:
                    _currentTransformListIndex--;
                    if (_currentTransformListIndex < 0)
                        _currentTransformListIndex = _gunPointsTransformList.Count - 1;
                    break;
                case RadiusFirePatternID.RightToLeft:
                    _currentTransformListIndex++;
                    if (_currentTransformListIndex > _gunPointsTransformList.Count - 1)
                        _currentTransformListIndex = 0;
                    break;
                case RadiusFirePatternID.BackAndForth:
                    if (_isGoingRight)
                    {
                        _currentTransformListIndex--;
                        if (_currentTransformListIndex >= 0) return;
                        _currentTransformListIndex = 0;
                        _isGoingRight = false;
                    }
                    else
                    {
                        _currentTransformListIndex++;
                        if (_currentTransformListIndex <= _gunPointsTransformList.Count - 1) return;
                        _currentTransformListIndex = _gunPointsTransformList.Count - 1;
                        _isGoingRight = true;
                    }
                    break;
            }
        }
    }
}