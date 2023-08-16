using UnityEngine;

public class SewerController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer rd;
    private float distance;
    private float speed;
    private float worldWidth;
    private Vector3 startPos;

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
    private void Update()
    {
        BackgroundMovemoment();
    }
    //calculator width background
    private void SetWidthBackground()
    {
        float countClone = Mathf.Floor(distance / 16f) + 2;
        Vector2 newScale = new Vector2(countClone * rd.size.x, rd.size.y);
        rd.size = newScale;
    }
    public void ChangeSpeed(float speed)
    {
        this.speed = speed;
    }
    private void BackgroundMovemoment()
    {
        rd.transform.position += speed * Time.deltaTime * Vector3.left;
    }
    //Set position with any screen
    private void SetPossitionBackground()
    {
        Vector3 temp = new Vector3(-worldWidth, rd.transform.position.y, 0);
        rd.transform.position = temp;
        startPos = rd.transform.position;
    }
    public void Revival()
    {
        rd.transform.position = startPos;
    }
}
