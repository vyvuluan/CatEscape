using Extensions;
using System;
using System.Collections;
using UnityEngine;

public class Sewer : Enemy
{
    [SerializeField] private GameObject outAttack;
    [SerializeField] private GameObject inAttack;
    [SerializeField] private BoxCollider2D leftHitBox;
    [SerializeField] private BoxCollider2D rightHitBox;
    [SerializeField] private BoxCollider2D middleHitBox;
    private bool isAttack;
    private bool isGameOver;
    private Transform roadSewer;
    private Player player;
    private Action onGameOver;


    protected override void Awake()
    {
        isAttack = false;
        isGameOver = false;
        base.Awake();
        roadSewer.ThrowIfNull();
        outAttack.ThrowIfNull();
        inAttack.ThrowIfNull();
        leftHitBox.ThrowIfNull();
        rightHitBox.ThrowIfNull();
        middleHitBox.ThrowIfNull();
    }
    protected override void Start()
    {
        transform.SetParent(roadSewer);
    }
    public void Init(Player player, Action onGameOver, Transform roadSewer)
    {
        this.player = player;
        this.onGameOver = onGameOver;
        this.roadSewer = roadSewer;
    }

    public override void Attack()
    {
        if (transform.position.x < 1.5f)
        {
            StartCoroutine(ActionAttack());
            isAttack = true;
        }
    }
    private IEnumerator ActionAttack()
    {
        outAttack.SetActive(true);
        base.isShoot = true;
        yield return new WaitForSeconds(0.8f);
        outAttack.SetActive(false);
        inAttack.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        base.isShoot = false;
        onPlusScore?.Invoke();
    }
    private void Update()
    {
        if (!isAttack)
        {
            Attack();
        }
        if (base.isShoot)
        {
            CheckCollsion(player);
        }
    }
    public void CheckCollsion(Player player)
    {
        if (player.IsCollisonCollider(leftHitBox) || player.IsCollisonCollider(rightHitBox) || player.IsCollisonCollider(middleHitBox))
        {
            if (!isGameOver)
            {
                onGameOver?.Invoke();
                isGameOver = true;
            }
        }
    }
    public void SetIsAttack()
    {
        isAttack = false;
        isGameOver = false;
        outAttack.SetActive(false);
        inAttack.SetActive(false);

    }

}
