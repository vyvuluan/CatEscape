using DG.Tweening;
using Extensions;
using Parameters;
using Services;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;
public class MainSceneController : MonoBehaviour
{
    [Header("MVC")]
    [SerializeField] private MainSceneModel model;
    [SerializeField] private MainSceneView view;

    [Header("Preference")]
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private GameObject coin;
    [SerializeField] private MapController mapController;
    [SerializeField] private SewerController sewerController;
    [SerializeField] private Camera cameraObject;
    [SerializeField] private Transform sewerRoad;
    [SerializeField] private Transform normalRoad;
    [SerializeField] private LayerMask layerItem;
    [SerializeField] private List<Player> characters;

    private int coinCurrent;
    private int playerCurrent;
    private bool isPause;
    private bool isCheckShootTeacher;
    private bool isContinueGame;
    private bool isGameOver;
    private bool isPipe;
    private bool isCheckConveyor;
    private int mapNumber;
    private float elapsedTime;
    private float worldWidth;
    private float mapPrecent;
    private RaycastHit2D hitItem;
    private MouseEvent mouseEventCurrent;
    private IStateGame item;
    private Finish finishItem;
    private PipeOut pipeOutItem;
    private GameServices gameServices;
    private Player player;
    private PlayerService playerService;
    private AdsService adsService;
    private DisplayService displayService;
    private IAPService iapService;
    private AudioService audioService;
    private InputService inputService;
    private Map mapCurrent;
    private PopUpParameter popUpParameter;
    private List<SkinItem> skinItems;
    private Dictionary<PrefabType, GameObject> dicPrefabs;
    private void Awake()
    {
        if (GameObject.FindGameObjectWithTag(Constanst.ServicesTag) == null)
        {
            SceneManager.LoadScene(Constanst.EntryScene);
        }
        else
        {
            GameObject gameServiecObject = GameObject.FindGameObjectWithTag(Constanst.ServicesTag);
            gameServices = gameServiecObject.GetComponent<GameServices>();
        }
        playerService = gameServices.GetService<PlayerService>();
        adsService = gameServices.GetService<AdsService>();
        displayService = gameServices.GetService<DisplayService>();
        iapService = gameServices.GetService<IAPService>();
        audioService = gameServices.GetService<AudioService>();
        inputService = gameServices.GetService<InputService>();
        InitDicPrefab();
        mapNumber = playerService.GetLevel();
        mapCurrent = model.Maps[mapNumber - 1];
        view.SetLevelText(mapNumber);
        enemySpawner.Init(mapCurrent.Enemies, view.TextCountDownShootEnemyTeacher, dicPrefabs, GameOver, null);
        foreach (var item in characters)
        {
            item.Init(model.JumpTime, model.DropTime, model.JumpHeight);
        }

        playerService.SetSkill(0);
        sewerController.Init(mapCurrent.GetPosByItemType(PrefabType.PipeOut), model.SpeedNormal);
        mapController.Init(mapCurrent.Distance, model.SpeedNormal);
        elapsedTime = 0;
        isPause = true;
        isCheckShootTeacher = false;
        isContinueGame = false;
        isGameOver = false;
        isCheckConveyor = false;
        isPipe = false;
        playerCurrent = 0;
        player = characters[0];
        mouseEventCurrent = MouseEvent.None;
        coinCurrent = playerService.GetScore();

        if (iapService.IsRemoveAds() == false)
        {
            if (mapNumber >= 2)
                adsService.ShowBannerAds();
        }
        else
        {
            adsService.HideBannerAds();
            view.ShowRemoveAdsButton(false);
        }
        view.SetCoin(coinCurrent);
        InitActionToPopup();
        ResetMapHandle();
        InitListSkinItem();
        if (!ShowSkinReward())
        {
            if (playerService.GetInterstitialAds() == 1)
            {
                InterstitialAds();
                playerService.SetInterstitialAds(0);
            }
        }
        CheckUnlockEndlessMode();
        enemySpawner.ThrowIfNull();
        model.ThrowIfNull();
        view.ThrowIfNull();
        player.ThrowIfNull();
    }
    private void ResetMapHandle()
    {
        ResetMapParameters resetMapParameters = popUpParameter.gameObject.GetComponent<ResetMapParameters>();
        if (resetMapParameters == null) return;
        if (resetMapParameters.IsReset)
        {
            Time.timeScale = 1;
            view.SetActivePlateCharater(false);
            characters[playerCurrent].gameObject.SetActive(false);
            player = characters[resetMapParameters.IndexPlayer];
            characters[resetMapParameters.IndexPlayer].gameObject.SetActive(true);
            cameraObject.orthographicSize = 5f;
            StartGame();
            DestroyImmediate(popUpParameter.gameObject.GetComponent<ResetMapParameters>());
        }
    }
    private void Start()
    {
        float worldHeight = Camera.main.orthographicSize * 2f;
        worldWidth = worldHeight * Screen.width / Screen.height;
        if (mapNumber >= 2) view.AvoidBanner(adsService.GetHightBanner());
    }
    public void NextAndPreBtnSelectChar(int type)
    {
        characters[playerCurrent].gameObject.SetActive(false);
        switch ((SelectCharacterType)type)
        {
            case SelectCharacterType.Increase:
                playerCurrent++;
                break;
            case SelectCharacterType.Decrease:
                playerCurrent--;
                break;
        }
        if (playerCurrent == -1 || playerCurrent >= characters.Count)
        {
            playerCurrent = 0;
        }
        characters[playerCurrent].gameObject.SetActive(true);
        player = characters[playerCurrent];
    }
    #region Init
    private void InitDicPrefab()
    {
        dicPrefabs = new();
        foreach (var item in model.PrefabObjects.PrefabObjects)
        {
            dicPrefabs.Add(item.Type, item.Prefab);
        }
    }

    public void InitListSkinItem()
    {
        skinItems = model.SkinItems.Select(n => new SkinItem(n.Id, n.State, n.SkinType, n.Image, n.LvlUnlock, n.IsSeen)).Where(n => n.LvlUnlock != -1 && n.LvlUnlock >= mapNumber).ToList();
        skinItems = skinItems.OrderBy(x => x.LvlUnlock).ToList();
        List<int> skinOwned = playerService.GetSkinOwned();
        for (int i = skinItems.Count - 1; i >= 0; i--)
        {
            if (skinOwned.Contains(skinItems[i].Id))
            {
                skinItems.Remove(skinItems[i]);
            }
        }
    }
    private void CheckUnlockEndlessMode()
    {
        if (mapNumber >= 3)
        {
            view.SetActiveLockEndless(false);
        }
        else
        {
            view.SetActiveLockEndless(true);
        }
    }
    public void InitActionToPopup()
    {
        popUpParameter = PopupHelpers.PassParamPopup();
        popUpParameter.AddAction(ActionType.CloseGameOver, ResetWhenGameOver);
        popUpParameter.AddAction(ActionType.ContinueGameOver, OnContinue);
        popUpParameter.AddAction(ActionType.AdsRewardSkin, GetSkin);
        popUpParameter.AddAction(ActionType.NoThanksSkin, ClosePanelGetSkin);
        popUpParameter.AddAction(ActionType.CloseSetting, CloseSetting);
        popUpParameter.AddAction(ActionType.ResetMap, ResetMap);
    }
    #endregion
    private bool ShowSkinReward()
    {
        List<int> skinAds = playerService.GetSkinAds();
        if (skinItems.Count > 0 && mapNumber == skinItems[0].LvlUnlock && skinItems[0].State == StatusState.Lock && !skinAds.Contains(skinItems[0].Id))
        {
            popUpParameter.SaveObject(Constanst.SkinItemKey, skinItems[0]);
            PopupHelpers.Show(Constanst.RewardSkinScene);
            return true;
        }
        return false;
    }
    private void InterstitialAds()
    {
        if (adsService.IsInterstitialReady() == true && iapService.IsRemoveAds() == false)
        {
            adsService.OnInterstitialClose = () =>
            {

            };
            adsService.ShowLimitInterstitialAd();
        }
        else
        {
        }
    }
    public void GameOver()
    {
        StartCoroutine(CouGameOver());
    }
    private IEnumerator CouGameOver()
    {
        isPause = true;
        isGameOver = true;
        if (!isPipe)
        {
            player.Sit();
            if (enemySpawner.EnemyCurrent.Type == EnemyType.Normal)
            {
                Cat cat = (Cat)enemySpawner.EnemyCurrent;
                cat.StartAnimDetect();
                yield return new WaitForSeconds(1f);
                characters[playerCurrent].GameOver();
                SimplePool.Despawn(enemySpawner.EnemyCurrent.gameObject);
                enemySpawner.EnemyCurrent = null;

            }
        }
        else
        {
            sewerController.ChangeSpeed(model.SpeedStop);
        }
        popUpParameter.SaveObject(Constanst.MapPercentKey, mapPrecent);
        PopupHelpers.Show(Constanst.GameOverScene);
    }
    private void PercentMapCurrent(float posXFinishCurrent, float posXFinishMap, float posXPipeOutMap)
    {

        if (isPause) return;
        if (posXFinishCurrent < 0f)
        {
            return;
        }
        float posXPipeOutCurrent = 0f;
        if (pipeOutItem != null)
        {
            posXPipeOutCurrent = pipeOutItem.transform.position.x;
        }
        float temp = posXFinishCurrent + posXPipeOutCurrent;
        float total = posXFinishMap + posXPipeOutMap;
        mapPrecent = 1 - (temp / total);
        view.SetSilderValue(mapPrecent);

    }
    public void IncreaseCoin()
    {
        int coinCurrent = playerService.GetScore();
        playerService.SetScore(coinCurrent + 1);
        view.SetCoin(coinCurrent + 1);
    }
    #region ACTION POPUP
    public void ResetWhenGameOver()
    {
        playerService.SetInterstitialAds(1);
        PopupHelpers.Close();
        SceneManager.LoadScene(Constanst.MainScene);
    }
    //use to button continue when game over
    public void OnContinue()
    {
        adsService.InitRewardedAd(() =>
        {
            PopupHelpers.Close(Constanst.GameOverScene);
            isContinueGame = true;
            if (isPipe)
            {
                sewerController.ChangeSpeed(model.SpeedStop);
                sewerController.Revival();
                enemySpawner.ResetAttackEnemySewer();
            }
        }, () =>
        {
            Debug.Log("fail");
        });
        adsService.ShowRewardedAd();
    }
    //use for get skin button in reward popup
    public void GetSkin()
    {
        adsService.InitRewardedAd(() =>
        {
            List<int> skinOwned = playerService.GetSkinOwned();
            skinOwned.Add(skinItems[0].Id);
            playerService.SetSkinOwned(skinOwned);
            PopupHelpers.Close(Constanst.RewardSkinScene);
        }, () =>
        {
        });
        adsService.ShowRewardedAd();

    }
    //use for no thanks button in reward popup
    public void ClosePanelGetSkin()
    {
        List<int> skinAds = playerService.GetSkinAds();
        skinAds.Add(skinItems[0].Id);
        playerService.SetSkinAds(skinAds);
        PopupHelpers.Close(Constanst.RewardSkinScene);
        playerService.SetInterstitialAds(0);
        Invoke(nameof(InterstitialAds), 0.1f);
    }
    public void CloseSetting()
    {
        //Time.timeScale = 1;
        //PopupHelpers.Close(Constanst.SettingScene);
        if (isPause)
        {
            PopupHelpers.Close(Constanst.SettingScene);
            Time.timeScale = 1;
        }
        else StartCoroutine(CouCloseSetting());
    }
    private IEnumerator CouCloseSetting()
    {
        PopupHelpers.Close(Constanst.SettingScene);
        int countDownTime = 3;
        while (countDownTime > 0)
        {
            view.TextCountDownShootEnemyTeacher(true, countDownTime.ToString());
            yield return new WaitForSecondsRealtime(1.0f);
            countDownTime--;
        }
        view.TextCountDownShootEnemyTeacher(false, countDownTime.ToString());
        Time.timeScale = 1;
    }
    public void ResetMap()
    {
        ResetMapParameters parameters = popUpParameter.gameObject.AddComponent<ResetMapParameters>();
        parameters.IsReset = true;
        parameters.IndexPlayer = playerCurrent;
        PopupHelpers.Close(Constanst.SettingScene);
        SceneManager.LoadScene(Constanst.MainScene);
    }
    #endregion
    #region FINISH
    public void FinishMap()
    {
        characters[playerCurrent].SetAnimation(Constanst.HappyAnim, true);
        StartCoroutine(CouFinishMap());
    }
    private IEnumerator CouFinishMap()
    {
        view.SetOrderLayoutCanvas(100);
        view.SetActiveFinishText(true);
        mapController.ChangeSpeed(model.SpeedStop);
        yield return new WaitForSecondsRealtime(1.0f);
        view.SetActiveFinishText(false);
        view.OnPanelFinishMap(true);
        //show MREC ads
        if (!iapService.IsRemoveAds())
        {
            adsService.ShowMRECAd();
            adsService.HideBannerAds();
        }
        //calculator percent receive skin
        if (skinItems.Count > 0)
        {
            if (skinItems[0].LvlUnlock == mapNumber)
            {
                if (skinItems.Count > 1)
                {
                    //view.SetWinPanel((mapNumber + 1) / (float)skinItems[1].LvlUnlock, skinItems[1].Id);
                    view.SetParamaterWinPopup((mapNumber + 1) / (float)skinItems[1].LvlUnlock, skinItems[1].Image);
                    view.SetLvlUnlockSkin(true);
                }
            }
            else
            {
                //view.SetWinPanel((mapNumber + 1) / (float)skinItems[0].LvlUnlock, skinItems[0].Id);
                view.SetParamaterWinPopup((mapNumber + 1) / (float)skinItems[0].LvlUnlock, skinItems[0].Image);
                view.SetLvlUnlockSkin(true);
            }
        }
        playerService.SetLevel(mapNumber + 1);
        isPause = true;
    }
    //user for get button in popup finish
    public void OnFinishMap()
    {
        ActionIncreaseCoin(10);
    }
    //user for ads button in popup finish
    public void OnFinishMapAds()
    {
        adsService.InitRewardedAd(() =>
        {
            ActionIncreaseCoin(20);
        }, () =>
        {
            Debug.Log("fail");
        });
        adsService.ShowRewardedAd();
    }
    public void ActionIncreaseCoin(int coinQuantity)
    {
        int coinPlus = coinQuantity;
        playerService.SetInterstitialAds(1);
        adsService.HideMRECAd();
        view.SetStatusBtnGetCoin(false);
        StartCoroutine(CouFinishMapAds(coinPlus, playerService.GetScore()));
        playerService.SetScore(playerService.GetScore() + coinPlus);
    }

    //use for ads button in popup finish
    private IEnumerator CouFinishMapAds(int coinPlus, int coinCur)
    {
        StartCoroutine(IncrementCoin(coinPlus, coinCur));
        for (int i = 0; i < 50; i++)
        {
            Vector3 temp = new Vector3(UnityEngine.Random.Range(-4f, 4f), UnityEngine.Random.Range(-12f, -11f), 0);
            GameObject go = SimplePool.Spawn(coin, temp, Quaternion.identity);
            go.transform.DOMoveY(go.transform.position.y + 15f, 0.7f).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                go.transform.DOMoveY(go.transform.position.y - 3f, 0.3f).SetEase(Ease.InQuad).OnComplete(() =>
                {
                    SimplePool.Despawn(go);
                });
            });
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(Constanst.MainScene);
    }
    private IEnumerator IncrementCoin(int coin, int totalCoin)
    {
        float increaseTime = 2f;
        int targetCoin = totalCoin + coin;
        float elapsedTime = 0.0f;
        while (totalCoin < targetCoin)
        {
            elapsedTime += Time.deltaTime;
            float incrementalValue = Mathf.Lerp(0, targetCoin - totalCoin, elapsedTime / increaseTime);
            totalCoin += Mathf.FloorToInt(incrementalValue);
            view.SetCoin(totalCoin);
            yield return null;
        }
    }
    #endregion
    #region INPUT
    private void InputManager()
    {
        foreach (var touch in inputService.GetTouches())
        {
            switch (touch.Phase)
            {
                case Services.TouchPhase.Down:
                    if (isContinueGame)
                    {
                        if (isPause)
                            mouseEventCurrent = MouseEvent.Continue;
                    }
                    else
                    {
                        if (isPipe)
                        {
                            mouseEventCurrent = MouseEvent.Jump;
                        }
                    }
                    break;
                case Services.TouchPhase.Move:
                    if (!isPipe)
                    {
                        mouseEventCurrent = MouseEvent.Sitting;
                    }
                    break;
                case Services.TouchPhase.Up:
                    if (!isPipe)
                    {
                        mouseEventCurrent = MouseEvent.Run;
                    }
                    break;
                default:
                    break;
            }
        }


    }
    private void HandleContinueGame()
    {
        if (mouseEventCurrent == MouseEvent.Continue && isPause)
        {
            if (isPipe)
            {
                player.Run();
                sewerController.ChangeSpeed(model.SpeedNormal);
            }
            isPause = false;
            isGameOver = false;
            isContinueGame = false;
            mouseEventCurrent = MouseEvent.None;
        }
    }
    private void HandleInput()
    {
        switch (mouseEventCurrent)
        {
            case MouseEvent.Jump:
                if (player.transform.position.y <= player.StartPos.y + 1.0f)
                {
                    player.Jump();
                    mouseEventCurrent = MouseEvent.None;
                }
                break;
            case MouseEvent.Sitting:
                if (!isCheckConveyor)
                {
                    mapController.ChangeSpeed(model.SpeedStop);
                }
                player.Sit();
                break;
            case MouseEvent.Run:
                mapController.ChangeSpeed(model.SpeedNormal);
                sewerController.ChangeSpeed(model.SpeedStop);
                player.Run();
                mouseEventCurrent = MouseEvent.None;
                break;
            case MouseEvent.None:
                if (isPipe)
                {
                    mapController.ChangeSpeed(model.SpeedStop);
                    sewerController.ChangeSpeed(model.SpeedNormal);
                }
                //else
                //{
                //    mapController.ChangeSpeed(model.SpeedNormal);
                //    sewerController.ChangeSpeed(model.SpeedStop);
                //}
                break;
            default:
                break;
        }
    }

    #endregion
    #region BUTTON
    public void BuyRemoveAd()
    {
        iapService.GetPrice();
        iapService.PurchaseRemoveAds(OnPurchaseCompleted);
    }
    public void OnPurchaseCompleted(bool isSuccess)
    {
        if (isSuccess == true)
        {
            SceneManager.LoadScene(Constanst.MainScene);
        }
    }
    public void StartGame()
    {
        SpawnItem();
        RandomCoin(20);
        StartCoroutine(CouStartGame());
    }
    private IEnumerator CouStartGame()
    {
        view.SetStatusListButton(false);
        float currentZoom = cameraObject.orthographicSize;
        float t = 0f;
        view.SelectCharacterHandle(-worldWidth - 2f);
        player.SelectSkin();
        while (t < 1f)
        {
            t += Time.deltaTime;
            float newZoom = Mathf.Lerp(currentZoom, 9.5f, t);
            cameraObject.orthographicSize = newZoom;
            yield return null;
        }
        isPause = false;
    }
    public void Shop()
    {
        SceneManager.LoadScene(Constanst.ShopScene);
    }
    public void Skin()
    {
        SceneManager.LoadScene(Constanst.SkinScene);
    }
    public void EndLessMode()
    {
        SceneManager.LoadScene(Constanst.MainSceneEndless);
    }
    public void Setting()
    {
        Time.timeScale = 0;
        PopupHelpers.Show(Constanst.SettingScene);
    }
    #endregion
    public void CheckAdsThreeMin()
    {
        if (Input.anyKeyDown)
        {
            elapsedTime = 0f;
        }
        else
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= model.InactivityThreshold)
            {
                InterstitialAds();
                elapsedTime = 0f;
            }
        }
    }
    //use when player enter pipe or out pipe
    private IEnumerator Loading(float time, PrefabType prefabType)
    {
        float elapsedTimeLoading = 0f;
        yield return new WaitForSeconds(0.5f);
        while (elapsedTimeLoading < time)
        {
            float opacityCurrent = elapsedTimeLoading / time;
            view.SetOpacityImageLoading(opacityCurrent);
            elapsedTimeLoading += Time.deltaTime;
            yield return null;
        }
        if (prefabType == PrefabType.PipeIn)
        {
            view.SetGroundBackGround(false);
            player.SetPosWhenEnterPipe();

        }
        else if (prefabType == PrefabType.PipeOut)
        {
            view.SetGroundBackGround(true);
            player.IsMovePipeIn = true;
            mapController.ChangeSpeed(model.SpeedNormal);
        }
        view.SetOpacityImageLoading(0);
        isPause = false;
    }
    public void CheckDie()
    {
        hitItem = Physics2D.Raycast(player.transform.position, Vector2.right, mapController.WorldWidth / 2, layerItem);
        //Debug.Log(hitItem.collider.gameObject.name);
        if (hitItem.collider != null)
        {
            item = hitItem.collider.gameObject.GetComponent<IStateGame>();
        }
        if (item == null || enemySpawner.EnemyCurrent == null) return;
        if (enemySpawner.EnemyCurrent.IsShoot)
        {
            //if (item == null)
            //{
            //    GameOver();
            //}
            //else
            //{
            if (item.IsInsideItem())
            {

            }
            else
            {
                if (playerService.GetSkill() == 1)
                {
                    player.SetSkillBehind(true);
                    view.SetImageSkill(false);
                }
                else
                {
                    GameOver();
                }
            }
            //}
        }
    }
    private void Update()
    {
        InputManager();
        HandleContinueGame();
        if (!isPause)
        {
            PercentMapCurrent(finishItem.transform.position.x, mapCurrent.GetPosByItemType(PrefabType.Finish), mapCurrent.GetPosByItemType(PrefabType.PipeOut));
            HandleInput();
            CheckDie();
        }
        else
        {
            mapController.ChangeSpeed(model.SpeedStop);
            sewerController.ChangeSpeed(model.SpeedStop);
        }
        if (isGameOver)
        {
            playerService.SetInterstitialAds(1);
        }
        //enable interstitial ads when dont touch screen
        CheckAdsThreeMin();
    }
    public void SpeedDash()
    {
        mapController.ChangeSpeed(model.SpeedWhenDash);
    }
    public void SpeedSlow()
    {
        mapController.ChangeSpeed(model.SpeedWhenSlow);
    }
    public void SpeedConveyor()
    {
        mapController.ChangeSpeed(model.SpeedWhenConveyor);
    }
    public void SpeedNormal()
    {
        mapController.ChangeSpeed(model.SpeedNormal);
    }
    public void ActionSetCheckConveyor(bool status)
    {
        isCheckConveyor = status;
    }
    public void ActionPipeIn()
    {
        StartCoroutine(Loading(0.5f, PrefabType.PipeIn));
        mapController.IsMovePipeIn = true;
        mapController.ChangeSpeed(model.SpeedStop);
        isPause = true;
        isPipe = true;
        //pipeIn = boosterTemp;
    }
    public void ActionPipeOut()
    {
        if (playerService.GetRemote() == 1)
        {
            StartCoroutine(Loading(1f, PrefabType.PipeOut));
            player.IsMovePipeOut = true;
            sewerController.ChangeSpeed(model.SpeedStop);
            isPause = true;
            isPipe = false;
            //pipeOut = boosterTemp;
            playerService.SetRemote(0);

        }
        else GameOver();
    }
    public void ActionShootEnemyTeacher()
    {
        if (!isCheckShootTeacher)
        {
            enemySpawner.EnemyCurrent.Attack();
            isCheckShootTeacher = true;
        }
    }
    public void ActionRemote()
    {
        playerService.SetRemote(1);
    }
    public void ActionSkillBehind()
    {
        playerService.SetSkill(1);
    }
    public void ActionRotation()
    {
        if (cameraObject.transform.rotation.z == 0)
        {
            Quaternion quaternion = Quaternion.Euler(0, 0, -90);
            cameraObject.transform.rotation = quaternion;
        }
        else
        {
            Quaternion quaternion = Quaternion.Euler(0, 0, 0);
            cameraObject.transform.rotation = quaternion;
        }
    }
    public void ActionSpawnEnemy(int index)
    {
        enemySpawner.SpawnEnemyWithIndex(index);
    }
    public void SpawnItem()
    {
        EnemyMap teacherEnemy = mapCurrent.FindEnemyMapByType(PrefabType.Teacher, mapCurrent.Enemies);
        foreach (var item in mapCurrent.Items)
        {
            Vector3 temp = dicPrefabs[item.Type].transform.position;
            temp.x = item.Pos;
            ItemBase stateItem = null;
            if (item.Type == PrefabType.TeacherTrue)
            {
                SpawnQuizz(teacherEnemy.Number1, teacherEnemy.Number2, item, temp);
            }
            else
            {
                stateItem = SimplePool.Spawn(dicPrefabs[item.Type], temp, Quaternion.identity).GetComponent<ItemBase>();
                stateItem.Init(player, worldWidth, MapType.Normal);
            }
            HandleItem(item, stateItem);
        }

    }
    private void HandleItem(ItemMap item, ItemBase stateItem)
    {
        switch (item.Type)
        {
            case PrefabType.Dash:
                Dash dash = (Dash)stateItem;
                dash.Init(SpeedDash, SpeedNormal, null);
                dash.transform.SetParent(normalRoad);
                break;
            case PrefabType.Slow:
                Slow slow = (Slow)stateItem;
                slow.Init(SpeedSlow, SpeedNormal, null);
                slow.transform.SetParent(normalRoad);
                break;
            case PrefabType.Conveyor:
                Conveyor conveyor = (Conveyor)stateItem;
                conveyor.Init(SpeedConveyor, ActionSetCheckConveyor, null);
                conveyor.transform.SetParent(normalRoad);
                break;
            case PrefabType.PipeIn:
                PipeIn pipeIn = (PipeIn)stateItem;
                pipeIn.Init(ActionPipeIn, null);
                pipeIn.transform.SetParent(normalRoad);
                break;
            case PrefabType.PipeOut:
                PipeOut pipeOut = (PipeOut)stateItem;
                pipeOut.Init(ActionPipeOut);
                pipeOut.transform.SetParent(sewerRoad);
                pipeOutItem = pipeOut;
                break;
            case PrefabType.Jump:
                Jump jump = (Jump)stateItem;
                jump.Init(GameOver);
                jump.transform.SetParent(sewerRoad);
                break;
            case PrefabType.Remote:
                Remote remote = (Remote)stateItem;
                remote.Init(ActionRemote);
                remote.transform.SetParent(sewerRoad);
                break;
            case PrefabType.SkillBehind:
                SkillBehind skillBehind = (SkillBehind)stateItem;
                skillBehind.Init(ActionSkillBehind, null);
                skillBehind.transform.SetParent(normalRoad);
                break;
            case PrefabType.Rotation:
                Rotation rotation = (Rotation)stateItem;
                rotation.Init(ActionRotation, null);
                rotation.transform.SetParent(normalRoad);
                break;
            case PrefabType.Cake:
            case PrefabType.Dither:
            case PrefabType.EleRed1:
            case PrefabType.EleRed2:
            case PrefabType.EleRed3:
            case PrefabType.Trash1:
            case PrefabType.Trash2:
            case PrefabType.Trash3:
                Normal normal = (Normal)stateItem;
                normal.Init(ActionSpawnEnemy, item.IsSpawn, null);
                normal.transform.SetParent(normalRoad);
                break;
            case PrefabType.Crystal:
                Crystal crystal = (Crystal)stateItem;
                crystal.Init(ActionSpawnEnemy, item.IsSpawn, null);
                crystal.transform.SetParent(normalRoad);
                break;
            case PrefabType.Car:
                Car car = (Car)stateItem;
                car.Init(ActionSpawnEnemy, item.IsSpawn, null);
                car.transform.SetParent(normalRoad);
                break;
            case PrefabType.Car1:
                CarBack carBack = (CarBack)stateItem;
                carBack.Init(ActionSpawnEnemy, item.IsSpawn, null);
                carBack.transform.SetParent(normalRoad);
                break;
            case PrefabType.Finish:
                Finish finish = (Finish)stateItem;
                finish.Init(FinishMap);
                finish.transform.SetParent(normalRoad);
                finishItem = finish;
                break;
            case PrefabType.Plate:
                SwitchBtn switchBtn = (SwitchBtn)stateItem;
                switchBtn.Init(ActionSpawnEnemy, item.IsSpawn);
                switchBtn.transform.SetParent(normalRoad);
                break;
            case PrefabType.Coin:
                Coin coin = (Coin)stateItem;
                coin.Init(IncreaseCoin);
                coin.transform.SetParent(normalRoad);
                break;
            default:
                break;
        }
    }
    private void SpawnQuizz(int number1, int number2, ItemMap itemMap, Vector3 pos)
    {
        int numberCorrect = number1 + number2;
        int indexCorrect = PosRandomCorrect();
        for (int i = 0; i < 3; i++)
        {
            ItemBase stateItem = SimplePool.Spawn(dicPrefabs[itemMap.Type], pos, Quaternion.identity).GetComponent<ItemBase>();
            stateItem.Init(player, worldWidth, MapType.Normal);
            Quizz quizz = (Quizz)stateItem;
            if (i == indexCorrect)
            {
                quizz.Init(ActionSpawnEnemy, ActionShootEnemyTeacher, itemMap.IsSpawn, numberCorrect, true, null);
            }
            else
            {
                int numberTemp = GetRandomValueExcluding(0, 9, numberCorrect);
                quizz.Init(ActionSpawnEnemy, ActionShootEnemyTeacher, itemMap.IsSpawn, numberTemp, false, null);
            }
            if (i == 0) quizz.SetIsHead(true);
            pos.x += 5f;
            quizz.transform.SetParent(normalRoad);
        }

    }
    private int GetRandomValueExcluding(int min, int max, int excludedValue)
    {
        int randomValue;
        do
        {
            randomValue = Random.Range(min, max + 1);
        } while (randomValue == excludedValue);
        return randomValue;
    }
    private int PosRandomCorrect()
    {
        return Random.Range(0, 3);
    }
    private void RandomCoin(int quantity)
    {
        for (int i = 0; i < quantity; i++)
        {
            int indexMap = Random.Range(0, 2);
            Vector3 temp = dicPrefabs[PrefabType.Coin].transform.position;
            ItemBase stateItem = SimplePool.Spawn(dicPrefabs[PrefabType.Coin], temp, Quaternion.identity).GetComponent<ItemBase>();
            stateItem.Init(player, worldWidth, MapType.Normal);
            Coin coin = (Coin)stateItem;
            coin.Init(IncreaseCoin);
            if (indexMap == 0)
            {
                temp.x = Random.Range(10, mapCurrent.GetPosByItemType(PrefabType.Finish));
                coin.transform.position = temp;
                coin.transform.SetParent(normalRoad);

            }
            else
            {
                temp.x = Random.Range(10, mapCurrent.GetPosByItemType(PrefabType.PipeOut));
                coin.transform.position = temp;
                coin.transform.SetParent(sewerRoad);
            }
        }
    }
}
