using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
#if UNITY_EDITOR
using NaughtyAttributes;
#endif

public class Tool : MonoBehaviour
{
    [SerializeField] private float id;
    [SerializeField] private float distance;
    [SerializeField] private MapType type;
    [SerializeField] private Transform parent;
    [SerializeField] private PrefabOjectConfig prefabOject;
    [SerializeField] private List<ItemMap> itemMaps;
    [SerializeField] private List<EnemyMap> enemyMaps;

    private Dictionary<PrefabType, GameObject> dicPrefabs;
    [Button("Generate Map")]
    public void Generate()
    {
        //init dictionary prefab
        dicPrefabs = new();
        foreach (var item in prefabOject.PrefabObjects)
        {
            dicPrefabs.Add(item.Type, item.Prefab);
        }
        //Generate map 
        ItemMap startItem = new ItemMap();
        startItem.Type = PrefabType.Start;
        startItem.Pos = 0;
        itemMaps.Insert(0, startItem);
        foreach (ItemMap item in itemMaps)
        {
            GameObject prefab = dicPrefabs[item.Type];
            GameObject go = Instantiate(prefab, Vector2.right * item.Pos, Quaternion.identity);
            go.transform.SetParent(parent);
        }
    }
    [Button("Clear Map")]
    public void ClearMap()
    {
        itemMaps.RemoveAt(0);
        itemMaps.Clear();
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(parent.GetChild(i).gameObject);
        }
    }
    [Button("Save Map")]
    public void SaveMap()
    {
        Map map = ScriptableObject.CreateInstance<Map>();
        map.MapId = id;
        map.Distance = distance;
        map.Type = type;
        map.Items = itemMaps.ToList();
        map.Enemies = enemyMaps;
        string filePath = "Assets/Scripts/Game/Map/Map_" + id + ".asset";
        AssetDatabase.CreateAsset(map, filePath);
        AssetDatabase.SaveAssets();

    }

}
