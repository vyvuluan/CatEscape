using System;

public class Finish : ItemBase
{
    private Action onActionCollison;
    public void Init(Action onActionCollison)
    {
        this.onActionCollison = onActionCollison;
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

    }
}
