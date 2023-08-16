using System;

public class Slow : ItemBase
{
    private bool isCheckReSpawn;
    private Action onReSpawn;
    private Action onChangeSpeedSlow;
    private Action onChangeSpeedNormal;
    public void Init(Action onChangeSpeedSlow, Action onChangeSpeedNormal, Action onReSpawn)
    {
        this.onChangeSpeedSlow = onChangeSpeedSlow;
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
        if (player.StatePlayer != StatePlayer.Sitting)
            onChangeSpeedSlow?.Invoke();
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
