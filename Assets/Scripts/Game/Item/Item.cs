using Extensions;
using System;
using UnityEngine;
public enum ItemType
{
    Dither, Cake, Crystal, Finish, Plate, Car, TeacherTrue, TeacherFail, Start, StartPipe, Car1
}
public class Item : MonoBehaviour
{
    [SerializeField] protected BoxCollider2D colliderCheckBehind;
    [SerializeField] private ItemType itemType;
    protected float speed;
    private float worldWidth;
    private float posXLast;
    private bool isSpawnEnemy;
    private MapType mapType;
    private Transform road;
    private Action onIncreaseDistanceMapNormal;
    public ItemType ItemType { get => itemType; set => itemType = value; }
    public float Speed { get => speed; set => speed = value; }
    public BoxCollider2D ColliderCheckBehind { get => colliderCheckBehind; }
    public bool IsSpawnEnemy { get => isSpawnEnemy; }
    public float WorldWidth { get => worldWidth;}

    public void Init(bool isSpawnEnemy, float posXLast, MapType mapType, Transform road, Action onIncreaseDistanceMapNormal)
    {
        this.isSpawnEnemy = isSpawnEnemy;
        this.posXLast = posXLast;
        this.mapType = mapType;
        this.road = road;
        this.onIncreaseDistanceMapNormal = onIncreaseDistanceMapNormal;
    }
    protected virtual void Awake()
    {
        isSpawnEnemy = false;
        colliderCheckBehind.ThrowIfNull();
    }
    private void Start()
    {
        float worldHeight = Camera.main.orthographicSize * 2f;
        worldWidth = worldHeight * Screen.width / Screen.height;
        transform.SetParent(road);
        speed = 0;
    }
    protected virtual void Update()
    {
        if (itemType == ItemType.Car || itemType == ItemType.Car1)
            Move();
        if(transform.position.x <= -worldWidth && mapType == MapType.Endless && itemType != ItemType.Start && itemType != ItemType.Finish)
        {
            //endless map
            Vector3 temp = transform.position;
            temp.x = posXLast;
            if (itemType == ItemType.Car)
                temp.x += 5f;
            transform.position = temp;
            speed = 0;
            onIncreaseDistanceMapNormal?.Invoke();
        }    
    }
    public void Move()
    {
        if (transform.position.x > -worldWidth)
        {
            transform.position += Vector3.left * speed * Time.deltaTime;
        }
    }

}
