using System;
using System.Collections;
using TMPro;
using UnityEngine;
public class Teacher : Enemy
{
    [SerializeField] private GameObject laser;
    [SerializeField] private TextMeshProUGUI mathText;
    private int number1;
    private int number2;
    private Action<bool, string> onTextCountDownShootEnemyTeacher;
    private Action onSetEnemyCurrent;
    public void Init(int number1, int number2, Action<bool, string> onTextCountDownShootEnemyTeacher, Action onSetEnemyCurrent)
    {
        this.number1 = number1;
        this.number2 = number2;
        this.onTextCountDownShootEnemyTeacher = onTextCountDownShootEnemyTeacher;
        this.onSetEnemyCurrent = onSetEnemyCurrent;
    }
    protected override void Awake()
    {
        base.type = EnemyType.Quizz;
        base.Awake();
    }
    protected override void Start()
    {
        mathText.text = $"{number1} + {number2} = ?";
        base.Start();
    }
    private void OnEnable()
    {
        laser.SetActive(false);
        base.type = EnemyType.Quizz;
        base.Awake();
    }
    private IEnumerator CountDownStartShoot()
    {
        Debug.Log("attack");
        yield return new WaitForSeconds(base.timeSpawn);
        int countDownTime = 3;
        onTextCountDownShootEnemyTeacher?.Invoke(true, countDownTime.ToString());
        while (countDownTime > 0)
        {
            onTextCountDownShootEnemyTeacher?.Invoke(true, countDownTime.ToString());
            yield return new WaitForSeconds(1.0f);
            countDownTime--;
        }
        onTextCountDownShootEnemyTeacher?.Invoke(false, String.Empty);
        laser.SetActive(true);
        base.isShoot = true;
        yield return new WaitForSeconds(base.timeAttack);
        base.isShoot = false;
        onSetEnemyCurrent?.Invoke();
        SimplePool.Despawn(gameObject);
    }
    public override void Attack()
    {
        StartCoroutine(CountDownStartShoot());
    }
}
