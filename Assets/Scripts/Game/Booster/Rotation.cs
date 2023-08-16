using System;

public class Rotation : ItemBase
{
    private bool isCheckReSpawn;
    private Action onReSpawn;
    private Action onActionCollison;
    public void Init(Action onActionCollison, Action onReSpawn)
    {
        this.onActionCollison = onActionCollison;
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
        onActionCollison?.Invoke();
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
