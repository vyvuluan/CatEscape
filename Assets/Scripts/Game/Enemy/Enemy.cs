using System;
using UnityEngine;
public abstract class Enemy : MonoBehaviour
{
    [SerializeField] protected EnemyType type;
    protected bool isShoot;
    protected float timeSpawn;
    protected float timeAttack;
    protected Action onPlusScore;
    private Transform envir2D;
    public bool IsShoot { get => isShoot; }
    public EnemyType Type { get => type; }
    public void Init(float timeSpawn, float timeAttack, Transform envir2D, Action onPlusScore)
    {
        this.timeSpawn = timeSpawn;
        this.timeAttack = timeAttack;
        this.envir2D = envir2D;
        this.onPlusScore = onPlusScore;
    }
    protected virtual void Awake()
    {
        isShoot = false;
    }
    protected virtual void Start()
    {
        transform.SetParent(envir2D);
    }
    public abstract void Attack();

}
