using DG.Tweening;
using System;
using UnityEngine;
public class SwitchBtn : ItemBase, IStateGame
{
    [SerializeField] private GameObject btnClick;
    [SerializeField] private GameObject btnClicked;
    [SerializeField] private GameObject plate;
    [SerializeField] private GameObject mask;
    [SerializeField] private Transform platePos;
    [SerializeField] private BoxCollider2D colPlate;
    private bool isDrop;
    private bool isSpawn;
    private bool isCheckSpawn;
    private Action<int> onSpawnEnemy;
    protected void Awake()
    {
        isDrop = true;
        isCheckSpawn = false;
    }
    public void OnClickBtn()
    {
        btnClick.SetActive(false);
        btnClicked.SetActive(true);
    }
    public void Init(Action<int> onSpawnEnemy, bool isSpawn)
    {
        this.onSpawnEnemy = onSpawnEnemy;
        this.isSpawn = isSpawn;
    }
    public override void OnDestroyItem()
    {
        SimplePool.Despawn(gameObject);
    }

    public override void OnEnter()
    {
        //if (isSpawn) onSpawnEnemy?.Invoke(1);
    }

    public override void OnExit()
    {

    }

    public override void OnInDistance()
    {
        float distance = Mathf.Abs(player.transform.position.x - transform.position.x);

        if (distance <= 5f && isSpawn && !isCheckSpawn)
        {
            mask.SetActive(true);
            onSpawnEnemy?.Invoke(1);
            isCheckSpawn = true;
        }
    }

    public override void OnStay()
    {
        if (player.StatePlayer == StatePlayer.Sitting)
        {
            //onActionCollison?.Invoke();
            OnClickBtn();
            if (isDrop)
            {
                plate.transform.DOMoveY(platePos.transform.position.y - 1f, 0.5f).SetEase(Ease.OutBounce).OnComplete(() => isDrop = false);
            }
        }

    }

    public bool IsInsideItem()
    {
        if (colPlate.transform.position.y > 15f)
        {
            return false;
        }
        else return player.IsPointPlayerInsideCollider(colPlate);
    }
}
