using System;

public class Dash : ItemBase
{
    private bool isCheckReSpawn;
    private Action onReSpawn;
    private Action onChangeSpeedDash;
    private Action onChangeSpeedNormal;
    public void Init(Action onChangeSpeedDash, Action onChangeSpeedNormal, Action onReSpawn)
    {
        this.onChangeSpeedDash = onChangeSpeedDash;
        this.onChangeSpeedNormal = onChangeSpeedNormal;
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
    }

    public override void OnStay()
    {
        if (base.player.StatePlayer != StatePlayer.Sitting)
        {
            onChangeSpeedDash?.Invoke();
        }
    }

    public override void OnExit()
    {
        onChangeSpeedNormal?.Invoke();
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
