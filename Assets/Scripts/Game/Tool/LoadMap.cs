using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using NaughtyAttributes;
#endif
public class LoadMap : MonoBehaviour
{
    [Expandable]
    [SerializeField] private ScriptableObject mapSelect;
    [SerializeField] private Transform parent;
    [SerializeField] private PrefabOjectConfig prefabObjects;
    private List<ItemMap> itemMaps;
    private Dictionary<PrefabType, GameObject> dicPrefabs;


    [Button("Load Map")]
    public void Load()
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(parent.GetChild(i).gameObject);
        }
        Map mapTemp = (Map)mapSelect;
        itemMaps = mapTemp.Items.ToList();
        dicPrefabs = new();
        foreach (var item in prefabObjects.PrefabObjects)
        {
            dicPrefabs.Add(item.Type, item.Prefab);
        }
        Generate();
    }
    public void Generate()
    {
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
        mapSelect = null;
        itemMaps = null;
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(parent.GetChild(i).gameObject);
        }
    }
}
