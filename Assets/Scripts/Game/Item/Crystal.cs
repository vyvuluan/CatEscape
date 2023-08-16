using System;

public class Crystal : ItemBase, IStateGame
{
    private bool isSpawn;
    private bool isCheckReSpawn;
    private Action onReSpawn;
    private Action<int> onSpawnEnemy;
    public void Init(Action<int> onSpawnEnemy, bool isSpawn, Action onReSpawn)
    {
        this.onSpawnEnemy = onSpawnEnemy;
        this.isSpawn = isSpawn;
        this.onReSpawn = onReSpawn;
    }
    private void Awake()
    {
        isCheckReSpawn = false;
    }
    private void OnEnable()
    {
        isCheckReSpawn = false;
    }
    public override void OnInDistance()
    {

    }

    public override void OnEnter()
    {
        if (isSpawn) onSpawnEnemy?.Invoke(1);
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

    public bool IsInsideItem()
    {

        return false;
    }
}
