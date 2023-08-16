using System;

public class Jump : ItemBase
{
    private bool isCheckGameOver;
    private Action onActionCollison;
    protected void Awake()
    {
        //isCheckGameOver = false;
    }
    public void Init(Action onActionCollison)
    {
        this.onActionCollison = onActionCollison;
    }

    public override void OnInDistance()
    {

    }

    public override void OnEnter()
    {
        //if (!isCheckGameOver)
        //{
        onActionCollison?.Invoke();
        isCheckGameOver = true;
        //}
    }

    public override void OnStay()
    {

    }

    public override void OnExit()
    {
        isCheckGameOver = false;
    }

    public override void OnDestroyItem()
    {

    }
}
