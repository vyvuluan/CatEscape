using UnityEngine;

public abstract class ItemBase : MonoBehaviour, IStateItem
{
    [SerializeField] protected BoxCollider2D boxCollider;
    protected bool isColliding;
    protected float worldWidth;
    protected MapType mapType;
    protected Player player;
    public void Init(Player player, float worldWidth, MapType mapType)
    {
        this.player = player;
        this.worldWidth = worldWidth;
        this.mapType = mapType;
    }
    private void Update()
    {
        if (player == null) return;
        float distance = Mathf.Abs(player.transform.position.x - transform.position.x);
        if (distance <= 15f)
        {
            OnInDistance();
        }

        bool isCollison = player.IsCollisonCollider(boxCollider);
        if (isCollison)
        {
            if (!isColliding)
            {
                isColliding = true;
                OnEnter();
            }
            else
            {
                OnStay();
            }
        }
        else
        {
            if (isColliding)
            {

                isColliding = false;
                OnExit();
            }
        }
        if (transform.position.x < -worldWidth && mapType == MapType.Endless)
        {
            OnDestroyItem();
        }

    }

    public abstract void OnInDistance();
    public abstract void OnEnter();
    public abstract void OnStay();
    public abstract void OnExit();
    public abstract void OnDestroyItem();
}
