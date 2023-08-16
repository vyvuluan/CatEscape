using DG.Tweening;
using System;
using UnityEngine;

public class SkillBehind : ItemBase
{
    [SerializeField] private Renderer rd;
    private bool isCheckReSpawn;
    private Action onReSpawn;
    private Action onActionCollison;
    private Material material;
    private Color originalColor;
    public void Init(Action onActionCollison, Action onReSpawn)
    {
        this.onActionCollison = onActionCollison;
        this.onReSpawn = onReSpawn;
    }
    public override void OnInDistance()
    {

    }
    private void Awake()
    {
        isCheckReSpawn = false;
    }
    private void OnEnable()
    {
        isCheckReSpawn = false;
    }

    public override void OnEnter()
    {
        onActionCollison?.Invoke();
        material = rd.material;
        originalColor = material.color;

        material.DOFade(0f, 1f).OnComplete(ResetAlpha);
        Vector3 temp = transform.position;
        temp.x -= 3f;
        temp.y += 3f;
        transform.DOMove(temp, 1f).OnComplete(() =>
        {
            SimplePool.Despawn(gameObject);
        });
        isColliding = true;
    }
    private void ResetAlpha()
    {
        material.color = originalColor;
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
            onReSpawn?.Invoke();
            isCheckReSpawn = true;
            SimplePool.Despawn(gameObject);

        }
    }
}
