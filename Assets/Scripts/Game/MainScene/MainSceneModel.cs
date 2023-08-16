using System.Collections.Generic;
using UnityEngine;

public class MainSceneModel : MonoBehaviour
{
    #region Gameplay
    [Header("Item")]
    [Space(8.0f)]
    [Tooltip("Speed normal")]
    [SerializeField] private float speedNormal = 6f;
    [Tooltip("Speed slow car")]
    [SerializeField] private float speedSlowCar = 1f;
    [Tooltip("Speed stop")]
    [SerializeField] private float speedStop = 0f;
    [Tooltip("Speed when dash")]
    [SerializeField] private float speedWhenDash = 12f;
    [Tooltip("Speed when slow")]
    [SerializeField] private float speedWhenSlow = 3f;
    [Tooltip("Speed when conveyor")]
    [SerializeField] private float speedWhenConveyor = -2f;
    [Tooltip("Time show ads if no tap screen")]
    [SerializeField] private float inactivityThreshold = 180f;
    [Tooltip("List prefab")]
    [SerializeField] private PrefabOjectConfig prefabObjects;
    public float SpeedNormal { get => speedNormal; }
    public float SpeedSlowCar { get => speedSlowCar; }
    public float SpeedStop { get => speedStop; }
    public float InactivityThreshold { get => inactivityThreshold; }
    public PrefabOjectConfig PrefabObjects { get => prefabObjects; }
    public float SpeedWhenDash { get => speedWhenDash; }
    public float SpeedWhenSlow { get => speedWhenSlow; }
    public float SpeedWhenConveyor { get => speedWhenConveyor; }
    #endregion
    #region PLAYER
    [Header("Player")]
    [Space(8.0f)]
    [Tooltip("Time jump")]
    [SerializeField] private float jumpTime = 0.5f;
    [Tooltip("Time drop")]
    [SerializeField] private float dropTime = 0.3f;
    [Tooltip("Jump height")]
    [SerializeField] private float jumpHeight = 5f;
    public float JumpTime { get => jumpTime; }
    public float DropTime { get => dropTime; }
    public float JumpHeight { get => jumpHeight; }
    #endregion
    #region MAP
    [Header("Map")]
    [Space(8.0f)]
    [Tooltip("List Map")]
    [SerializeField] private List<Map> maps;
    [Tooltip("Map Endless")]
    [SerializeField] private Map mapEndless;
    public Map MapEndless { get => mapEndless; }
    public List<Map> Maps { get => maps; }
    #endregion
    #region SKIN
    [Header("SKIN")]
    [Space(8.0f)]
    [Tooltip("list skin")]
    [SerializeField] private List<SkinItem> skinItems;
    public List<SkinItem> SkinItems { get => skinItems; }
    #endregion
}
