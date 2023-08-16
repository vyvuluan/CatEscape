using System;

public class PipeOut : ItemBase
{
    private bool isOutPipe;
    private Action onActionCollison;

    public void Init(Action onActionCollison)
    {
        this.onActionCollison = onActionCollison;
    }
    private void Awake()
    {
        isOutPipe = false;
    }

    public override void OnInDistance()
    {

    }

    public override void OnEnter()
    {
        if (!isOutPipe)
        {
            onActionCollison?.Invoke();
            isOutPipe = true;
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
        //SimplePool.Despawn(gameObject);
    }
}
