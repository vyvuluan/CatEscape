using DG.Tweening;
using UnityEngine;
public enum StatePlayer
{
    Running, Sitting, Jumpping
}
public class Player : MonoBehaviour
{
    [SerializeField] private GameObject playerRun;
    [SerializeField] private GameObject playerSit;
    [SerializeField] private GameObject playerSkillBehind;
    [SerializeField] private BoxCollider2D boxCollider;
    private bool isMovePipeOut;
    private bool isMovePipeIn;
    private float timeJump;
    private float timeDrop;
    private float jumpHeight;
    private Vector3 startPos;
    private StatePlayer statePlayer;
    public bool IsMovePipeOut { get => isMovePipeOut; set => isMovePipeOut = value; }
    public bool IsMovePipeIn { get => isMovePipeIn; set => isMovePipeIn = value; }
    public Vector3 StartPos { get => startPos; }
    public StatePlayer StatePlayer { get => statePlayer; }

    private void Awake()
    {
        isMovePipeOut = false;
        startPos = transform.position;
        PlayerDropStart();

    }
    private void Update()
    {
        if (isMovePipeOut)
        {
            transform.DOMoveY(transform.position.y + 2f, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
            {
                isMovePipeOut = false;
            });
        }
        if (isMovePipeIn)
        {
            transform.DOMoveY(startPos.y, timeDrop).SetEase(Ease.OutBounce).OnComplete(() =>
            {
                isMovePipeIn = false;
            });
        }
    }
    public void Init(float timeJump, float timeDrop, float jumpHeight)
    {
        this.timeJump = timeJump;
        this.timeDrop = timeDrop;
        this.jumpHeight = jumpHeight;
    }
    public void Sit()
    {
        playerRun.SetActive(false);
        playerSit.SetActive(true);
        statePlayer = StatePlayer.Sitting;
    }
    public void Run()
    {
        playerRun.SetActive(true);
        playerSit.SetActive(false);
        statePlayer = StatePlayer.Running;
    }
    public void Jump()
    {
        statePlayer = StatePlayer.Jumpping;
        Vector3 temp = transform.position;
        temp.y += jumpHeight;

        transform.DOMove(temp, timeJump).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            DOVirtual.DelayedCall(0.1f, Drop);

        });
    }
    public void Drop()
    {
        Vector3 temp = transform.position;
        temp.y -= jumpHeight;
        transform.DOMove(startPos, timeDrop).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            statePlayer = StatePlayer.Running;
        });
    }
    public void SetSkillBehind(bool status)
    {
        playerSkillBehind.SetActive(status);
    }
    public bool IsPointPlayerInsideCollider(Collider2D collider)
    {
        Bounds bounds = collider.bounds;
        return bounds.Contains(new Vector3(transform.position.x, bounds.center.y, 0f));
    }
    public bool IsCollisonCollider(BoxCollider2D collider)
    {
        Bounds bounds1 = boxCollider.bounds;
        Bounds bounds2 = collider.bounds;
        return bounds1.Intersects(bounds2);
    }
    public void SetPosWhenEnterPipe()
    {
        transform.position += Vector3.up * 8f;
        isMovePipeIn = true;
    }
    public void PlayerDropStart()
    {
        transform.DOMove(new Vector3(0, -0.36f, 0), 0.7f).SetEase(Ease.OutBounce);
    }
    public void SelectSkin()
    {
        Vector3 temp = new Vector3(0, -5f, 0);
        transform.DOMove(temp, 0.7f).SetEase(Ease.OutBounce).OnComplete(() =>
        {
            startPos = temp;
        });
    }
}
