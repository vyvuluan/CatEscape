using System;

public class Conveyor : ItemBase
{
    private bool isCheckReSpawn;
    private Action onReSpawn;
    private Action onChangeSpeedConveyor;
    private Action<bool> onSetCheckConveyor;
    private void Awake()
    {
        isCheckReSpawn = false;
    }
    private void OnEnable()
    {
        isCheckReSpawn = false;
    }
    public void Init(Action onChangeSpeedConveyor, Action<bool> onSetCheckConveyor, Action onReSpawn)
    {
        this.onChangeSpeedConveyor = onChangeSpeedConveyor;
        this.onSetCheckConveyor = onSetCheckConveyor;
        this.onReSpawn = onReSpawn;
    }
    public override void OnInDistance()
    {

    }

    public override void OnEnter()
    {
        onSetCheckConveyor?.Invoke(true);
    }

    public override void OnStay()
    {
        if (player.StatePlayer == StatePlayer.Sitting)
        {
            onChangeSpeedConveyor?.Invoke();
        }
    }

    public override void OnExit()
    {
        onSetCheckConveyor?.Invoke(false);
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
