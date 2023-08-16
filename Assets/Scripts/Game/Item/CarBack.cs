using System;
using UnityEngine;

public class CarBack : ItemBase, IStateGame
{
    private bool isSpawn;
    private bool isStartPos;
    private bool isCheckSpawn;
    private bool isCheckReSpawn;
    private float speed;
    private float startX;
    private Action onReSpawn;
    private Action<int> onSpawnEnemy;
    protected void Awake()
    {
        speed = 0;
        isStartPos = false;
        isCheckSpawn = false;
        isCheckReSpawn = false;
    }
    private void OnEnable()
    {
        speed = 0;
        isStartPos = false;
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
        if (distance < 3f)
        {
            speed = 1f;
            if (!isStartPos)
            {
                startX = transform.position.x;
                isStartPos = true;
            }

        }
        else speed = 0f;
        if (Mathf.Abs(startX - transform.position.x) > 1f)
            speed = -1f;
        transform.position += speed * Vector3.right * Time.deltaTime;
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
