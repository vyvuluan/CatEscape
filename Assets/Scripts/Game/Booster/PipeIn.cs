using System;
public class PipeIn : ItemBase
{
    private bool isCheckReSpawn;
    private bool isEnterPipe;
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
        isEnterPipe = false;
    }
    private void OnEnable()
    {
        isCheckReSpawn = false;
        isEnterPipe = false;
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

    public override void OnEnter()
    {
        if (!isEnterPipe)
        {
            onActionCollison?.Invoke();
            isEnterPipe = true;
        }
    }


    public override void OnExit()
    {
        base.boxCollider.enabled = false;
    }
    public override void OnInDistance()
    {

    }


    public override void OnStay()
    {

    }
}
