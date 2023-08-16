using DG.Tweening;
using Extensions;
using Parameters;
using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;

public class EndlessController : MonoBehaviour
{
    [Header("MVC")]
    [SerializeField] private EndlessModel model;
    [SerializeField] private EndlessView view;

    [Header("Preference")]
    [SerializeField] private Player player;
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private GameObject coin;
    [SerializeField] private MapController mapController;
    [SerializeField] private SewerController sewerController;
    [SerializeField] private Transform sewerRoad;
    [SerializeField] private Transform normalRoad;
    [SerializeField] private Camera cameraObject;
    [SerializeField] private LayerMask layerItem;
    [SerializeField] private List<ItemBase> randomPrefabs;

    private int coinCurrent;
    private int coinReward;
    private int scoreCurrent;
    private bool isPause;
    private bool isContinueGame;
    private bool isGameOver;
    private bool isPipe;
    private bool isCheckConveyor;
    private float elapsedTime;
    private float worldWidth;
    private RaycastHit2D hitItem;
    private GameObject lastItem;
    private GameObject lastBooster;
    private GameObject romoteItem;
    private MouseEvent mouseEventCurrent;
    private IStateGame item;
    private GameServices gameServices;
    private PlayerService playerService;
    private AdsService adsService;
    private DisplayService displayService;
    private IAPService iapService;
    private AudioService audioService;
    private InputService inputService;
    private Map mapCurrent;
    private PopUpParameter popUpParameter;
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
        mapCurrent = model.MapEndless;
        enemySpawner.Init(mapCurrent.Enemies, view.TextCountDownShootEnemyTeacher, dicPrefabs, GameOver, ActionPlusScore);
        player.Init(model.JumpTime, model.DropTime, model.JumpHeight);
        playerService.SetSkill(0);
        playerService.SetRemote(0);
        sewerController.Init(mapCurrent.GetPosByItemType(PrefabType.PipeOut), model.SpeedNormal);
        mapController.Init(mapCurrent.Distance, model.SpeedNormal);
        isPause = true;
        isContinueGame = false;
        isGameOver = false;
        isPipe = false;
        isCheckConveyor = false;
        mouseEventCurrent = MouseEvent.None;
        scoreCurrent = 0;
        coinReward = 0;
        coinCurrent = playerService.GetScore();

        if (playerService.GetLevel() >= 2)
        {
            if (iapService.IsRemoveAds() == false)
            {
                adsService.ShowBannerAds();
            }
        }
        view.SetCoin(coinCurrent);
        InitActionToPopup();
        if (playerService.GetInterstitialAds() == 1)
        {
            InterstitialAds();
            playerService.SetInterstitialAds(0);
        }
        enemySpawner.ThrowIfNull();
        model.ThrowIfNull();
        view.ThrowIfNull();
        player.ThrowIfNull();
    }

    private void Start()
    {
        float worldHeight = Camera.main.orthographicSize * 2f;
        worldWidth = worldHeight * Screen.width / Screen.height;
        if (playerService.GetLevel() >= 2) view.AvoidBanner(adsService.GetHightBanner());
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

    public void InitActionToPopup()
    {
        popUpParameter = PopupHelpers.PassParamPopup();
        popUpParameter.AddAction(ActionType.CloseGameOver, ResetWhenGameOver);
        popUpParameter.AddAction(ActionType.ContinueGameOver, OnContinue);
        popUpParameter.AddAction(ActionType.CloseSetting, CloseSetting);
        popUpParameter.AddAction(ActionType.ResetMap, ResetMap);
    }
    #endregion
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
                SimplePool.Despawn(enemySpawner.EnemyCurrent.gameObject);
                enemySpawner.EnemyCurrent = null;
            }
        }
        else
        {
            sewerController.ChangeSpeed(model.SpeedStop);
        }
        PopupHelpers.Show(Constanst.GameOverScene);
    }
    #region ACTION POPUP
    public void ResetWhenGameOver()
    {
        playerService.SetInterstitialAds(1);
        PopupHelpers.Close();
        ShowReport();

        //SceneManager.LoadScene(Constanst.MainScene);
    }
    private void ShowReport()
    {
        if (scoreCurrent > playerService.GetHighScore())
        {
            playerService.SetHighScore(scoreCurrent);
        }
        view.SetActivePanelReport(true);
        view.SetParameterPanelReport(playerService.GetHighScore(), scoreCurrent, coinReward);
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
    public void CloseSetting()
    {
        //Time.timeScale = 1;
        //PopupHelpers.Close(Constanst.SettingScene);
        if (isPause)
        {
            PopupHelpers.Close(Constanst.SettingScene);
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
        StartGame();
        PopupHelpers.Close(Constanst.SettingScene);
        SceneManager.LoadScene(Constanst.MainScene);
    }
    #endregion
    #region FINISH
    //public void FinishMap()
    //{
    //    view.OnPanelFinishMap(true);
    //    //show MREC ads
    //    if (!iapService.IsRemoveAds())
    //    {
    //        adsService.ShowMRECAd();
    //        adsService.HideBannerAds();
    //    }
    //    mapController.ChangeSpeed(model.SpeedStop);
    //    //calculator percent receive skin
    //    if (skinItems.Count > 0)
    //    {
    //        if (skinItems[0].LvlUnlock == mapNumber)
    //        {
    //            if (skinItems.Count > 1)
    //            {
    //                view.SetWinPanel((mapNumber + 1) / (float)skinItems[1].LvlUnlock, skinItems[1].Id);
    //                view.SetLvlUnlockSkin(true);
    //            }
    //        }
    //        else
    //        {
    //            view.SetWinPanel((mapNumber + 1) / (float)skinItems[0].LvlUnlock, skinItems[0].Id);
    //            view.SetLvlUnlockSkin(true);
    //        }
    //    }
    //    playerService.SetLevel(mapNumber + 1);
    //    isPause = true;
    //}
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
        //view.SetStatusBtnGetCoin(false);
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
            //view.SetCoin(totalCoin);
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
    public void StartGame()
    {
        SpawnItem();
        //RandomCoin(20);
        player.SelectSkin();
        view.SetStatusListButton(false);
        isPause = false;
    }
    public void Back()
    {
        SceneManager.LoadScene(Constanst.MainScene);
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
            sewerController.Revival();
            enemySpawner.ResetAttackEnemySewer();
        }
        view.SetOpacityImageLoading(0);
        isPause = false;
    }
    public void CheckDie()
    {
        hitItem = Physics2D.Raycast(player.transform.position, Vector2.right, mapController.WorldWidth / 2, layerItem);
        if (hitItem.collider != null)
            item = hitItem.collider.gameObject.GetComponent<IStateGame>();
        if (item == null || enemySpawner.EnemyCurrent == null) return;
        if (enemySpawner.EnemyCurrent.IsShoot)
        {
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
        }
    }
    private void Update()
    {
        Debug.Log($"score: {scoreCurrent}");
        InputManager();
        HandleContinueGame();
        if (!isPause)
        {
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
            romoteItem.SetActive(true);
        }
        else GameOver();
    }
    public void ActionShootEnemyTeacher()
    {
        enemySpawner.EnemyCurrent.Attack();
        scoreCurrent++;
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
    public void ActionPlusScore()
    {
        scoreCurrent++;
    }
    public void ActionSpawnEnemy(int index)
    {
        enemySpawner.SpawnEnemyWithIndex(index);
    }

    public void SpawnItem()
    {
        EnemyMap teacherEnemy = mapCurrent.FindEnemyMapByType(PrefabType.Teacher, mapCurrent.Enemies);
        for (int i = 0; i < 2; i++)
        {
            Vector3 temp = dicPrefabs[mapCurrent.Items[i].Type].transform.position;
            temp.x = mapCurrent.Items[i].Pos;
            ItemBase stateItem = null;
            if (mapCurrent.Items[i].Type == PrefabType.TeacherTrue)
            {
                SpawnQuizz(teacherEnemy.Number1, teacherEnemy.Number2, mapCurrent.Items[i], temp);
            }
            else
            {
                stateItem = SimplePool.Spawn(dicPrefabs[mapCurrent.Items[i].Type], temp, Quaternion.identity).GetComponent<ItemBase>();
                stateItem.Init(player, worldWidth, MapType.Endless);
            }
            if (i == 0)
                lastItem = stateItem.gameObject;
            else lastBooster = stateItem.gameObject;
            HandleItem(mapCurrent.Items[i], stateItem);
        }
        for (int i = 18; i <= 21; i++)
        {
            Vector3 temp = dicPrefabs[mapCurrent.Items[i].Type].transform.position;
            temp.x = mapCurrent.Items[i].Pos;
            ItemBase stateItem = SimplePool.Spawn(dicPrefabs[mapCurrent.Items[i].Type], temp, Quaternion.identity).GetComponent<ItemBase>();
            stateItem.Init(player, worldWidth, MapType.Endless);

            HandleItem(mapCurrent.Items[i], stateItem);
        }

    }
    private void HandleItem(ItemMap item, ItemBase stateItem)
    {
        switch (item.Type)
        {
            case PrefabType.Dash:
                Dash dash = (Dash)stateItem;
                if (dash == null) return;
                dash.Init(SpeedDash, SpeedNormal, RandomBooster);
                dash.transform.SetParent(normalRoad);
                break;
            case PrefabType.Slow:
                Slow slow = (Slow)stateItem;
                slow.Init(SpeedSlow, SpeedNormal, RandomBooster);
                slow.transform.SetParent(normalRoad);
                break;
            case PrefabType.Conveyor:
                Conveyor conveyor = (Conveyor)stateItem;
                if (conveyor == null) return;
                conveyor.Init(SpeedConveyor, ActionSetCheckConveyor, RandomBooster);
                conveyor.transform.SetParent(normalRoad);
                break;
            case PrefabType.PipeIn:
                PipeIn pipeIn = (PipeIn)stateItem;
                pipeIn.Init(ActionPipeIn, RandomElement);
                pipeIn.transform.SetParent(normalRoad);
                break;
            case PrefabType.PipeOut:
                PipeOut pipeOut = (PipeOut)stateItem;
                if (pipeOut == null) return;
                pipeOut.Init(ActionPipeOut);
                pipeOut.transform.SetParent(sewerRoad);
                //pipeOutItem = pipeOut;
                break;
            case PrefabType.Jump:
                Jump jump = (Jump)stateItem;
                if (jump == null) return;
                jump.Init(GameOver);
                jump.transform.SetParent(sewerRoad);
                break;
            case PrefabType.Remote:
                Remote remote = (Remote)stateItem;
                if (remote == null) return;
                remote.Init(ActionRemote);
                remote.transform.SetParent(sewerRoad);
                romoteItem = remote.gameObject;
                break;
            case PrefabType.SkillBehind:
                SkillBehind skillBehind = (SkillBehind)stateItem;
                if (skillBehind == null) return;
                skillBehind.Init(ActionSkillBehind, RandomBooster);
                skillBehind.transform.SetParent(normalRoad);
                break;
            case PrefabType.Rotation:
                Rotation rotation = (Rotation)stateItem;
                if (rotation == null) return;
                rotation.Init(ActionRotation, RandomBooster);
                rotation.transform.SetParent(normalRoad);
                break;
            case PrefabType.Cake:
            case PrefabType.EleRed1:
            case PrefabType.EleRed2:
            case PrefabType.EleRed3:
            case PrefabType.Dither:
            case PrefabType.Trash1:
            case PrefabType.Trash2:
                Normal normal = (Normal)stateItem;
                if (normal == null) return;
                normal.Init(ActionSpawnEnemy, item.IsSpawn, RandomElement);
                normal.transform.SetParent(normalRoad);
                break;
            case PrefabType.Crystal:
                Crystal crystal = (Crystal)stateItem;
                if (crystal == null) return;
                crystal.Init(ActionSpawnEnemy, item.IsSpawn, RandomElement);
                crystal.transform.SetParent(normalRoad);
                break;
            case PrefabType.Car:
                Car car = (Car)stateItem;
                if (car == null) return;
                car.Init(ActionSpawnEnemy, item.IsSpawn, RandomElement);
                car.transform.SetParent(normalRoad);
                break;
            case PrefabType.Car1:
                CarBack carBack = (CarBack)stateItem;
                if (carBack == null) return;
                carBack.Init(ActionSpawnEnemy, item.IsSpawn, RandomElement);
                carBack.transform.SetParent(normalRoad);
                break;
            case PrefabType.Plate:
                SwitchBtn switchBtn = (SwitchBtn)stateItem;
                if (switchBtn == null) return;
                switchBtn.Init(ActionSpawnEnemy, item.IsSpawn);
                switchBtn.transform.SetParent(normalRoad);
                break;
            case PrefabType.Coin:
                Coin coin = (Coin)stateItem;
                if (coin == null) return;
                coin.Init(IncreaseCoin);
                coin.transform.SetParent(normalRoad);
                break;
            default:
                break;
        }
    }
    public void IncreaseCoin()
    {
        int coinCurrent = playerService.GetScore();
        playerService.SetScore(coinCurrent + 1);
        coinReward++;
        view.SetCoin(coinCurrent + 1);
    }
    private void SpawnQuizz(int number1, int number2, ItemMap itemMap, Vector3 pos)
    {
        int numberCorrect = number1 + number2;
        int indexCorrect = PosRandomCorrect();
        for (int i = 0; i < 3; i++)
        {
            ItemBase stateItem = SimplePool.Spawn(dicPrefabs[itemMap.Type], pos, Quaternion.identity).GetComponent<ItemBase>();
            stateItem.Init(player, worldWidth, MapType.Endless);
            Quizz quizz = (Quizz)stateItem;
            if (i == indexCorrect)
            {
                quizz.Init(ActionSpawnEnemy, ActionShootEnemyTeacher, itemMap.IsSpawn, numberCorrect, true, RandomElement);
            }
            else
            {
                int numberTemp = GetRandomValueExcluding(0, 9, numberCorrect);
                quizz.Init(ActionSpawnEnemy, ActionShootEnemyTeacher, itemMap.IsSpawn, numberTemp, false, RandomElement);
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
        Vector3 temp = dicPrefabs[PrefabType.Coin].transform.position;
        coin.transform.position = temp;
        temp.x = Random.Range(5, lastItem.transform.position.x);


        for (int i = 0; i < quantity; i++)
        {
            ItemBase stateItem = SimplePool.Spawn(dicPrefabs[PrefabType.Coin], temp, Quaternion.identity).GetComponent<ItemBase>();
            stateItem.Init(player, worldWidth, MapType.Endless);
            Coin coin = (Coin)stateItem;
            coin.Init(IncreaseCoin);

            coin.transform.SetParent(normalRoad);
            temp.x += 3f;

        }
    }
    public void RandomElement()
    {
        int index = Random.Range(2, 14);
        EnemyMap teacherEnemy = mapCurrent.FindEnemyMapByType(PrefabType.Teacher, mapCurrent.Enemies);
        ItemBase stateItem = null;
        Vector3 tempItem = dicPrefabs[mapCurrent.Items[index].Type].transform.position;
        tempItem.x = lastItem.transform.position.x + 25f;
        if (mapCurrent.Items[index].Type == PrefabType.TeacherTrue)
        {
            SpawnQuizz(teacherEnemy.Number1, teacherEnemy.Number2, mapCurrent.Items[index], dicPrefabs[mapCurrent.Items[index].Type].transform.position);
        }
        else
        {
            stateItem = SimplePool.Spawn(dicPrefabs[mapCurrent.Items[index].Type], tempItem, Quaternion.identity).GetComponent<ItemBase>();
            stateItem.Init(player, worldWidth, MapType.Endless);

        }
        HandleItem(mapCurrent.Items[index], stateItem);
        if (stateItem != null)
            lastItem = stateItem.gameObject;
        mapController.IncreaseDistanceMap();
        RandomCoin(2);
    }
    public void RandomBooster()
    {
        int indexBooster = Random.Range(14, 18);
        ItemBase stateBooster;
        Vector3 tempBooster = dicPrefabs[mapCurrent.Items[indexBooster].Type].transform.position;
        tempBooster.x = lastBooster.transform.position.x + 20f;
        stateBooster = SimplePool.Spawn(dicPrefabs[mapCurrent.Items[indexBooster].Type], tempBooster, Quaternion.identity).GetComponent<ItemBase>();
        stateBooster.Init(player, worldWidth, MapType.Endless);
        HandleItem(mapCurrent.Items[indexBooster], stateBooster);
        if (stateBooster != null)
            lastBooster = stateBooster.gameObject;

    }
}
