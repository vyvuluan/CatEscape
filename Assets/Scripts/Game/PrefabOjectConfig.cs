using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PrefabObject
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private PrefabType type;
    [SerializeField] private ObjectType objectType;

    public GameObject Prefab { get => prefab; set => prefab = value; }
    public PrefabType Type { get => type; set => type = value; }
    public ObjectType ObjectType { get => objectType; set => objectType = value; }
}
[CreateAssetMenu(fileName = "PrefabObject", menuName = "ScriptableObjects/PrefabObject")]
public class PrefabOjectConfig : ScriptableObject
{
    [SerializeField] private List<PrefabObject> prefabObjects;

    public List<PrefabObject> PrefabObjects { get => prefabObjects; }
}
