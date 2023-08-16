using System;
using TMPro;
using UnityEngine;

public class Quizz : ItemBase, IStateGame
{
    [SerializeField] private TextMeshProUGUI numberText;
    private bool isCorrect;
    private bool isSpawn;
    private bool isCheckReSpawn;
    private bool isCheckSpawn;
    private bool isCheckAttack;
    private bool isHead;
    private Action onReSpawn;
    private Action<int> onSpawnEnemy;
    private Action onEnemyAttack;
    private void Awake()
    {
        isCheckReSpawn = false;
        isCheckSpawn = false;
        isCheckAttack = false;
        isHead = false;
    }
    private void OnEnable()
    {
        isCheckReSpawn = false;
        isCheckSpawn = false;
        isCheckAttack = false;
        isHead = false;
    }
    public void Init(Action<int> onSpawnEnemy, Action onEnemyAttack, bool isSpawn, int number, bool isCorrect, Action onReSpawn)
    {
        this.onSpawnEnemy = onSpawnEnemy;
        this.isSpawn = isSpawn;
        numberText.text = number.ToString();
        this.onEnemyAttack = onEnemyAttack;
        this.isCorrect = isCorrect;
        this.onReSpawn = onReSpawn;
    }
    public void SetIsHead(bool status)
    {
        isHead = status;
    }
    public bool IsInsideItem()
    {
        return player.IsPointPlayerInsideCollider(boxCollider) && isCorrect;
    }

    public override void OnInDistance()
    {
        float distance = Mathf.Abs(player.transform.position.x - transform.position.x);
        if (distance <= 5f && isSpawn && !isCheckSpawn && isHead)
        {
            onSpawnEnemy?.Invoke(2);
            isCheckSpawn = true;
        }
    }

    public override void OnEnter()
    {
        if (!isCheckAttack && isHead)
        {
            Debug.Log("111");
            onEnemyAttack?.Invoke();
            isCheckAttack = true;
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
        if (!isCheckReSpawn)
        {
            if (isCorrect)
            {
                onReSpawn?.Invoke();
            }
            SimplePool.Despawn(gameObject);
            isCheckReSpawn = true;
        }
    }
}
