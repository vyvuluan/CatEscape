using DG.Tweening;
using System;
using UnityEngine;

public class Remote : ItemBase
{
    [SerializeField] private Renderer rd;
    private Material material;
    private Color originalColor;
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
        material = rd.material;
        originalColor = material.color;

        material.DOFade(0f, 1f).OnComplete(ResetAlpha); ;
        Vector3 temp = transform.position;
        temp.x -= 3f;
        temp.y += 3f;
        transform.DOMove(temp, 1f).OnComplete(() =>
        {
            SimplePool.Despawn(gameObject);
            Vector3 temp1 = new Vector3(transform.position.x + 3f, transform.position.y - 3f, 0);
            transform.position = temp1;
        });
        //SimplePool.Despawn(gameObject);
        isColliding = true;
    }
    private void ResetAlpha()
    {
        material.color = originalColor;
    }
    private void OnEnable()
    {
        //ResetAlpha();
        //Vector3 temp = new Vector3(transform.position.x + 3f, transform.position.y - 3f, 0);
        //transform.position = temp;
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
