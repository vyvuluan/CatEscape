using System;
using UnityEngine;

public class Car : ItemBase, IStateGame
{
    private bool isSpawn;
    private bool isCheckSpawn;
    private bool isCheckReSpawn;
    private float speed;
    private Action onReSpawn;
    private Action<int> onSpawnEnemy;
    protected void Awake()
    {
        speed = 0;
        isCheckSpawn = false;
        isCheckReSpawn = false;
    }
    private void OnEnable()
    {
        speed = 0;
        isCheckSpawn = false;
        isCheckReSpawn = false;
    }
    public void Init(Action<int> onSpawnEnemy, bool isSpawn, Action onReSpawn)
    {
        this.onSpawnEnemy = onSpawnEnemy;
        this.isSpawn = isSpawn;
        this.onReSpawn = onReSpawn;
    }

    public bool IsInsideItem()
    {
        return player.IsPointPlayerInsideCollider(boxCollider);
    }

    public override void OnInDistance()
    {
        float distance = Mathf.Abs(player.transform.position.x - transform.position.x);
        if (distance < 2f)
            speed = 1f;
        else speed = 0f;
        transform.position += speed * Time.deltaTime * Vector3.right;
    }

    public override void OnEnter()
    {
        if (isSpawn && !isCheckSpawn)
        {
            onSpawnEnemy?.Invoke(1);
            isCheckSpawn = true;
        }
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
