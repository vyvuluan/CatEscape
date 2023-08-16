using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    [SerializeField] private List<SpriteRenderer> renderers;
    private bool isMovePipeIn;
    private float distance;
    private float speed;
    private float worldWidth;
    private Vector3 startPos;
    public bool IsMovePipeIn { get => isMovePipeIn; set => isMovePipeIn = value; }
    public float WorldWidth { get => worldWidth; }

    private void Awake()
    {
        isMovePipeIn = false;
        startPos = transform.position;

    }
    private void Update()
    {
        if (isMovePipeIn)
        {
            transform.DOMoveY(startPos.y + 8f, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
            {
                EnterPipe();
            });
        }
        else
        {
            BackgroundMovemoment();
        }
    }
    public void Init(float distance, float speed)
    {
        this.distance = distance;
        this.speed = speed;
    }
    private void Start()
    {
        SetWidthBackground();
        float worldHeight = Camera.main.orthographicSize * 2f;
        worldWidth = worldHeight * Screen.width / Screen.height;
        SetPossitionBackground();
    }
    public void EnterPipe()
    {
        transform.localPosition = startPos;
        isMovePipeIn = false;
    }
    //calculator width background
    private void SetWidthBackground()
    {
        //16 is width 
        float countClone = Mathf.Floor(distance / 16f) + 1;
        foreach (SpriteRenderer renderer in renderers)
        {
            Vector2 newScale = new Vector2(countClone * renderer.size.x, renderer.size.y);
            renderer.size = newScale;
        }
    }
    public void ChangeSpeed(float speed)
    {
        this.speed = speed;
    }
    private void BackgroundMovemoment()
    {
        foreach (SpriteRenderer renderer in renderers)
        {
            renderer.transform.position += speed * Time.deltaTime * Vector3.left;
        }
    }
    //Set position with any screen
    private void SetPossitionBackground()
    {
        foreach (SpriteRenderer renderer in renderers)
        {
            Vector3 temp = new Vector3(-worldWidth, renderer.transform.position.y, 0);
            renderer.transform.position = temp;
        }
    }
    public void IncreaseDistanceMap()
    {
        foreach (SpriteRenderer renderer in renderers)
        {
            Vector2 newScale = new Vector2(64f + renderer.size.x, renderer.size.y);
            renderer.size = newScale;
        }
    }
}
