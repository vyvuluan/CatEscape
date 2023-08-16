using DG.Tweening;
using Extensions;
using Spine.Unity;
using System;
using System.Collections;
using UnityEngine;

public class Cat : Enemy
{
    [SerializeField] private GameObject laser;
    [SerializeField] private SkeletonAnimation skeletonAnimation;
    [SerializeField] private GameObject maskLaser;
    private Vector3 startPosLaser;
    private Tween myTween;
    private Action onSetEnemyCurrent;
    protected override void Awake()
    {
        base.type = EnemyType.Normal;
        laser.ThrowIfNull();
        skeletonAnimation.ThrowIfNull();
        base.Awake();
    }
    protected override void Start()
    {
        startPosLaser = maskLaser.transform.position;
        SetAnimation(Constanst.ToiletJump, false);
        base.Start();
    }
    public void Init(Action onSetEnemyCurrent)
    {
        this.onSetEnemyCurrent = onSetEnemyCurrent;
    }
    private IEnumerator Shoot()
    {
        yield return new WaitForSeconds(base.timeSpawn);
        SetAnimation(Constanst.ToiletScanStart, false);
        yield return new WaitForSeconds(0.4f);
        Shooting();
        SetAnimation(Constanst.ToiletScanIdle, true);
        myTween = maskLaser.transform.DOMoveY(startPosLaser.y - 23f, base.timeAttack).OnComplete(() =>
        {
            //yield return new WaitForSeconds(base.timeAttack);
            SetAnimation(Constanst.ToiletScanEnd, false);
            DontShooting();

        });
        yield return new WaitForSeconds(base.timeAttack);
        SetAnimation(Constanst.ToiletDown, false);
        yield return new WaitForSeconds(1f);
        onPlusScore?.Invoke();
        onSetEnemyCurrent?.Invoke();
        SimplePool.Despawn(gameObject);

    }
    private void OnEnable()
    {
        maskLaser.transform.position = startPosLaser;
        SetAnimation(Constanst.ToiletJump, false);
    }
    public void Shooting()
    {
        base.isShoot = true;
        laser.SetActive(true);
    }
    public void DontShooting()
    {
        base.isShoot = false;
        laser.SetActive(false);
    }
    public override void Attack()
    {
        StartCoroutine(Shoot());
    }
    private void SetAnimation(string animName, bool isLoop)
    {
        skeletonAnimation.AnimationState.SetAnimation(0, animName, isLoop);
    }
    public void StartAnimDetect()
    {
        laser.SetActive(false);
        skeletonAnimation.AnimationState.ClearTracks();
        skeletonAnimation.skeleton.SetToSetupPose();
        skeletonAnimation.Update(0f);
        myTween.Kill();
        SetAnimation(Constanst.ToiletDetect, false);
        base.isShoot = false;
    }
}
