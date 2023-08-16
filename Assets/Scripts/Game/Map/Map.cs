using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyMap
{
    [SerializeField] private PrefabType type;
    [SerializeField] private float timeSpawn;
    [SerializeField] private float timeAttack;
    [SerializeField] private float pos;
    [SerializeField] private int number1;
    [SerializeField] private int number2;
    public float TimeSpawn { get => timeSpawn; set => timeSpawn = value; }
    public int Number1 { get => number1; set => number1 = value; }
    public int Number2 { get => number2; set => number2 = value; }
    public PrefabType Type { get => type; set => type = value; }
    public float TimeAttack { get => timeAttack; set => timeAttack = value; }
    public float Pos { get => pos; set => pos = value; }
}
[Serializable]
public class ItemMap
{
    [SerializeField] private PrefabType type;
    [SerializeField] private float pos;
    [SerializeField] private bool isSpawn;
    public float Pos { get => pos; set => pos = value; }
    public PrefabType Type { get => type; set => type = value; }
    public bool IsSpawn { get => isSpawn; set => isSpawn = value; }
}
[CreateAssetMenu(fileName = "Map", menuName = "ScriptableObjects/Map")]
public class Map : ScriptableObject
{
    [SerializeField] private float mapId;
    [SerializeField] private float distance;
    [SerializeField] private MapType type;
    [SerializeField] private List<ItemMap> items;
    [SerializeField] private List<EnemyMap> enemies;
    public MapType Type { get => type; set => type = value; }
    public List<ItemMap> Items { get => items; set => items = value; }
    public float MapId { get => mapId; set => mapId = value; }
    public List<EnemyMap> Enemies { get => enemies; set => enemies = value; }
    public float Distance { get => distance; set => distance = value; }
    public EnemyMap FindEnemyMapByType(PrefabType enemyType, List<EnemyMap> enemyMaps)
    {
        foreach (var map in enemyMaps)
        {
            if (map.Type == enemyType)
                return map;
        }
        return null;
    }
    public float GetPosByItemType(PrefabType prefabType)
    {
        foreach (var item in items)
        {
            if (item.Type == prefabType)
                return item.Pos;
        }
        return 0f;
    }
}
