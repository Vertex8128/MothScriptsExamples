using System;
using UnityEngine;
using Random = UnityEngine.Random;

public static class Utils
{
    #region Random

    public static int GetRandomIntMaxIncluded(Vector2Int nums) => nums == Vector2Int.zero ? 0 : Random.Range(nums.x, nums.y + 1);
    public static int GetRandomIntMaxIncluded(int min, int max) => Random.Range(min, max + 1);
    
    public static int GetRandomIntMaxExcluded(Vector2Int nums) => nums == Vector2Int.zero ? 0 : Random.Range(nums.x, nums.y);
    public static int GetRandomIntMaxExcluded(int min, int max) => Random.Range(min, max);

    public static float GetRandomRoundedFloat(Vector2 nums) => nums == Vector2.zero ? 0 : RoundFloat(Random.Range(nums.x, nums.y), 2);
    public static float GetRandomRoundedFloat(float min, float max) => RoundFloat(Random.Range(min, max), 2);

    public static int GetRandomPercent() => (int) (Random.value * 100f);

    public static int GetRandomListIndex(int count) => Random.Range(0, count);

    public static Vector2 GetRandomNormalizedVector2() => Random.insideUnitCircle.normalized;
    
    public static Quaternion GetRandomQuaternionRotationZ() => Quaternion.Euler(new Vector3(0, 0, Random.Range(0, 360)));

    public static bool CheckChanceIsTrue(float percent) => Random.value <= percent * 0.01f;

    #endregion
    
    
    #region Math
    
    public static int GetIncreasedPercentValue(int baseValue, float stepPercent, int stepNumbers)
    {
        float newValue = baseValue;
        for (var i = 0; i < stepNumbers; i++)
        {
            newValue += newValue * stepPercent * 0.01f;
        }
        return Mathf.CeilToInt(newValue);
    }
    
    public static float GetIncreasedPercentValue(float baseValue, float stepPercent, int stepNumbers)
    {
        var newValue = baseValue;
        for (var i = 0; i < stepNumbers; i++)
        {
            newValue += newValue * stepPercent * 0.01f;
        }
        return RoundFloat(newValue, 1);
    }

    public static int GetDecreasedPercentValue(int baseValue, float stepPercent, int stepNumbers)     
    {
        float newValue = baseValue;
        for (var i = 0; i < stepNumbers; i++)
        {
            newValue -= newValue * stepPercent * 0.01f;
        }
        return Mathf.RoundToInt(newValue);
    }
    
    public static float GetDecreasedPercentValue(float baseValue, float stepPercent, int stepNumbers)     
    {
        var newValue = baseValue;
        for (var i = 0; i < stepNumbers; i++)
        {
            newValue -= newValue * stepPercent * 0.01f;
        }
        return newValue;
    }

    private static float RoundFloat(float num, int symbolsAfterDot)
    {
        var modifier = symbolsAfterDot switch
        {
            1 => 10f,
            2 => 100f,
            3 => 1000f,
            4 => 10000f,
            5 => 100000f,
            _ => throw new Exception("Modifier wasn't set")
        };
        return Mathf.Round(num * modifier) / modifier;
    }
    
    public static float GetAngleFromVector2Float(Vector2 direction) 
    {
        var normDirection = direction.normalized;
        var angle = Mathf.Atan2(normDirection.y, normDirection.x) * Mathf.Rad2Deg;
        if (angle < 0) 
            angle += 360;
        return angle;
    }
    
    public static int GetAngleFromVector2Int(Vector2 direction) 
    {
        var normDirection = direction.normalized;
        var angle = Mathf.Atan2(normDirection.y, normDirection.x) * Mathf.Rad2Deg;
        if (angle < 0) 
            angle += 360;
        return Mathf.RoundToInt(angle);
    }
    #endregion
    

    #region Spawn

    public static bool IsPositionOccupied(Vector3 position, int layerMask, float radius = 0f)
    {
        var hitColliders = radius == 0f 
            ? Physics2D.OverlapPointAll(position, layerMask)
            : Physics2D.OverlapCircleAll(position, radius, layerMask);
        return hitColliders.Length != 0;
    }

    public static Vector2? GetSpawnPositionWithinSquare(Transform transform, int layerMask, bool isYRoundedToInt)
    {
        var spawnPosition = Vector2.zero;
        var safeCounter = 0;
        do 
        {
            var xHalfSide = transform.localScale.x * 0.5f;
            var yHalfSide = transform.localScale.y * 0.5f;
            var xPosition = GetRandomRoundedFloat(-xHalfSide, xHalfSide);
            var yPosition = GetRandomRoundedFloat(-yHalfSide, yHalfSide);
            if (isYRoundedToInt)
                yPosition = Mathf.RoundToInt(yPosition);
            spawnPosition = new Vector2(xPosition, yPosition) + (Vector2)transform.position;
        } 
        while (IsPositionOccupied(spawnPosition, layerMask, 1f));
        return spawnPosition;
    }

    public static Vector2? GetSpawnPositionWithinCircle(Vector2 originPoint, float radius, int layerMask)
    {
        var spawnPosition = Vector2.zero;
        var safeCounter = 0;
        do 
        {
            spawnPosition = originPoint + (Random.insideUnitCircle.normalized * radius);
        } 
        while (IsPositionOccupied(spawnPosition, layerMask, 1f));
        return spawnPosition;
    }

    #endregion

    #region Inspector

    public static string GetObjectNameForEditor(Sprite sprite) => sprite == null ? "Null" : sprite.name;
    
    public static string GetObjectNameForEditor(GameObject gameObject) => gameObject == null ? "Null" : gameObject.name;

    #endregion
}