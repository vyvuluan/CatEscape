using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
using NaughtyAttributes;
#endif

public class ToolRandom : MonoBehaviour
{
    [SerializeField] private float id;
    [SerializeField] private float distance;
    [SerializeField] private MapType type;
    [SerializeField] private List<ItemMap> itemMaps;
    [SerializeField] private List<EnemyMap> enemyMaps;
    [SerializeField] private List<PrefabObject> itemPrefabs;
    [SerializeField] private List<PrefabObject> boosterPrefabs;
    [SerializeField] private Transform parent;
    [SerializeField] private GameObject end;
    [SerializeField] private int quantityBooster;
    [SerializeField] private int quantityItem;


    [Button("Generate Map")]
    public void Generate()
    {
        float posItemCurrent = 0f;
        float posBoosterCurrent = 0f;

        //Generate map
        SpawnItem(ref posItemCurrent);
        SpawnBooster(posBoosterCurrent);
        CreateItemFinish(posItemCurrent);
    }

    [Button("Clear Map")]
    public void ClearMap()
    {
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
        map.Items = itemMaps;
        map.Enemies = enemyMaps;
        string filePath = "Assets/Scripts/Game/Map/Map_" + id + ".asset";
        AssetDatabase.CreateAsset(map, filePath);
        AssetDatabase.SaveAssets();

    }
    private void SpawnItem(ref float posValue)
    {
        for (int i = 0; i < quantityItem; i++)
        {
            int indexItemPrefab = Random.Range(0, itemPrefabs.Count);
            PrefabObject prefabObject = itemPrefabs[indexItemPrefab];
            switch (prefabObject.Type)
            {
                case PrefabType.Crystal:
                case PrefabType.Dither:
                    CreateCrystalOrDither(ref posValue);
                    break;
                case PrefabType.TeacherTrue:
                    posValue += 10f;
                    ItemMap itemQuizz = new ItemMap();
                    itemQuizz.Type = PrefabType.TeacherTrue;
                    itemQuizz.Pos = posValue;
                    itemQuizz.IsSpawn = true;
                    itemMaps.Add(itemQuizz);
                    posValue += 30f;
                    break;
                default:
                    GameObject go = Instantiate(itemPrefabs[indexItemPrefab].Prefab, Vector2.right * posValue, Quaternion.identity);
                    go.transform.SetParent(parent);
                    ItemMap itemMap = new ItemMap();
                    itemMap.Type = prefabObject.Type;
                    itemMap.Pos = posValue;
                    itemMap.IsSpawn = true;
                    itemMaps.Add(itemMap);
                    posValue += 20f;
                    break;
            }
        }
    }
    private void CreateCrystalOrDither(ref float posValue)
    {
        ItemMap itemTemp = new ItemMap();
        GameObject go = null;
        for (int j = 0; j < 4; j++)
        {
            if (j % 2 == 0)
            {
                go = Instantiate(FindPrefabByEnum(PrefabType.Crystal, itemPrefabs), Vector2.right * posValue, Quaternion.identity);
                itemTemp.Type = PrefabType.Crystal;
                itemTemp.Pos = posValue;
            }
            else
            {
                go = Instantiate(FindPrefabByEnum(PrefabType.Dither, itemPrefabs), Vector2.right * posValue, Quaternion.identity);
                itemTemp.Type = PrefabType.Dither;
                itemTemp.Pos = posValue;
            }

            go.transform.SetParent(parent);
            itemMaps.Add(itemTemp);
            posValue += 5f;
        }
    }
    private void SpawnBooster(float posValue)
    {
        for (int i = 0; i < quantityBooster; i++)
        {
            int indexBoosterPrefab = Random.Range(0, boosterPrefabs.Count);
            PrefabObject prefabObject = boosterPrefabs[indexBoosterPrefab];
            GameObject go = Instantiate(prefabObject.Prefab, Vector2.right * posValue, Quaternion.identity);
            go.transform.SetParent(parent);
            ItemMap itemMap = new ItemMap();
            itemMap.Type = prefabObject.Type;
            itemMap.Pos = posValue;
            itemMaps.Add(itemMap);
            float distanceRandom = Random.Range(10f, 25f);
            posValue += distanceRandom;
            //if (prefab.BoosterType == BoosterType.Coin)
            //{
            //    for (int j = 0; j < 4; j++)
            //    {
            //        GameObject go = Instantiate(prefab.gameObject, Vector2.right * posValue, Quaternion.identity);
            //        go.transform.SetParent(parent);

            //        boosterMapTemp = new BoosterMap();
            //        boosterMapTemp.Type = FindPrefabType(prefab.gameObject);
            //        boosterMapTemp.Pos = posValue;
            //        boosterMaps.Add(boosterMapTemp);
            //        posValue += 1f;
            //    }
            //}
            //else
            //{
            //    GameObject go = Instantiate(prefab.gameObject, Vector2.right * posValue, Quaternion.identity);
            //    go.transform.SetParent(parent);
            //    boosterMapTemp = new BoosterMap();
            //    boosterMapTemp.Type = FindPrefabType(prefab.gameObject);
            //    boosterMapTemp.Pos = posValue;
            //    boosterMaps.Add(boosterMapTemp);
            //}

        }
    }
    private void CreateItemFinish(float pos)
    {
        ItemMap endItem = new ItemMap();
        endItem.Type = PrefabType.Finish;
        endItem.Pos = pos + 10f;
        distance = endItem.Pos;
        GameObject goEndItem = Instantiate(end, Vector2.right * distance, Quaternion.identity);
        goEndItem.transform.SetParent(parent);
        itemMaps.Add(endItem);
    }
    public GameObject FindPrefabByEnum(PrefabType prefabType, List<PrefabObject> list)
    {
        foreach (var item in list)
        {
            if (item.Type == prefabType)
                return item.Prefab;
        }
        return null;
    }
}
