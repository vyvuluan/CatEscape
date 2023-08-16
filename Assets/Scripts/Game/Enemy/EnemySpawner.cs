using Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;
public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Transform envir;
    [SerializeField] private Transform roadSewer;
    [SerializeField] private Player player;
    [SerializeField] private List<Enemy> listEmemies;
    private Enemy enemyCurrent;
    private Action onGameOver;
    private Action onPlusScore;
    private Action<bool, string> onTextCountDownShootEnemyTeacher;
    private List<EnemyMap> enemieMaps;
    private List<EnemyMap> normalEnemies;
    private List<EnemyMap> movementEnemies;
    private List<Sewer> sewers;
    private Dictionary<PrefabType, GameObject> dicPrefabs;
    public Enemy EnemyCurrent { get => enemyCurrent; set => enemyCurrent = value; }
    public void Init(List<EnemyMap> enemieMaps, Action<bool, string> onTextCountDownShootEnemyTeacher, Dictionary<PrefabType, GameObject> dicPrefabs, Action onGameOver, Action onPlusScore)
    {
        this.enemieMaps = enemieMaps;
        this.onTextCountDownShootEnemyTeacher = onTextCountDownShootEnemyTeacher;
        this.dicPrefabs = dicPrefabs;
        this.onGameOver = onGameOver;
        this.onPlusScore = onPlusScore;
    }
    private void Awake()
    {
        sewers = new List<Sewer>();
        envir.ThrowIfNull();
        player.ThrowIfNull();
        roadSewer.ThrowIfNull();
    }
    private void Start()
    {
        InitList();
        SpawnEnemyMovement();
    }
    private void InitList()
    {
        normalEnemies = new List<EnemyMap>();
        movementEnemies = new List<EnemyMap>();
        foreach (var item in enemieMaps)
        {
            if (item.Type == PrefabType.Sewer)
            {
                movementEnemies.Add(item);
            }
            else normalEnemies.Add(item);
        }
    }
    public void SpawnEnemyWithIndex(int indexEnemy)
    {
        if (enemyCurrent == null)
        {
            Enemy enemy = SimplePool.Spawn(dicPrefabs[normalEnemies[indexEnemy].Type], dicPrefabs[normalEnemies[indexEnemy].Type].transform.position, Quaternion.identity).GetComponent<Enemy>();
            enemy.Init(normalEnemies[indexEnemy].TimeSpawn, normalEnemies[indexEnemy].TimeAttack, envir, onPlusScore);
            switch (enemy.Type)
            {
                case EnemyType.Normal:

                    enemy.Attack();
                    Cat cat = (Cat)enemy;
                    cat.Init(ResetEnemyCurrent);

                    break;
                case EnemyType.Quizz:
                    Teacher teacher = (Teacher)enemy;
                    teacher.Init(normalEnemies[indexEnemy].Number1, normalEnemies[indexEnemy].Number2, onTextCountDownShootEnemyTeacher, ResetEnemyCurrent);
                    break;
                default:
                    break;
            }
            enemyCurrent = enemy;
        }

    }
    public void ResetEnemyCurrent()
    {
        enemyCurrent = null;
    }
    private void SpawnEnemyMovement()
    {
        if (movementEnemies.Count <= 0) return;
        foreach (var item in movementEnemies)
        {
            Enemy enemy = SimplePool.Spawn(dicPrefabs[item.Type], dicPrefabs[item.Type].transform.position, Quaternion.identity).GetComponent<Enemy>();
            enemy.Init(item.TimeSpawn, item.TimeAttack, envir, onPlusScore);
            Vector3 temp = dicPrefabs[item.Type].transform.position;
            temp.x = item.Pos;
            enemy.transform.position = temp;
            Sewer sewer = (Sewer)enemy;
            sewer.Init(player, onGameOver, roadSewer);
            sewers.Add(sewer);
        }
    }
    public void ResetAttackEnemySewer()
    {
        foreach (var item in sewers)
        {
            item.SetIsAttack();
        }
    }

    public int GetCountListEnemyNormal()
    {
        return normalEnemies.Count;
    }
}
