using System;
using UnityEngine;

public class Normal : ItemBase, IStateGame
{
    private bool isSpawn;
    private bool isCheckSpawn;
    private bool isCheckReSpawn;
    private Action onReSpawn;
    private Action<int> onSpawnEnemy;

    public void Init(Action<int> onSpawnEnemy, bool isSpawn, Action onReSpawn)
    {
        this.onSpawnEnemy = onSpawnEnemy;
        this.isSpawn = isSpawn;
        this.onReSpawn = onReSpawn;
    }
    protected void Awake()
    {
        isCheckSpawn = false;
        isCheckReSpawn = false;
    }

    private void OnEnable()
    {
        isCheckSpawn = false;
        isCheckReSpawn = false;
    }
    public bool IsInsideItem()
    {
        return player.IsPointPlayerInsideCollider(boxCollider);
    }

    public override void OnInDistance()
    {
        float distance = Mathf.Abs(player.transform.position.x - transform.position.x);
        if (distance <= 5f)
        {
            if (isSpawn && !isCheckSpawn)
            {
                onSpawnEnemy?.Invoke(0);
                isCheckSpawn = true;
            }
        }
    }

    public override void OnEnter()
    {

    }

    public override void OnStay()
    {

    }

    public override void OnExit()
    {

    }

    public override void OnDestroyItem()
    {
        if (!isCheckReSpawn)
        {
            onReSpawn?.Invoke();
            isCheckReSpawn = true;
            SimplePool.Despawn(gameObject);

        }
    }
}
