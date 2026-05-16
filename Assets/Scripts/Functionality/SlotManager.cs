using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using System;

public class SlotManager : MonoBehaviour
{

    [Header("Sprites")]
    [SerializeField]
    private Sprite[] symbolImages;  //images taken initially

    [Header("Slot Images")]
    [SerializeField] private List<SlotImage> totalImages;     //class to store total images
    [SerializeField] private List<SlotImage> resultImages;     //class to store the result matrix
    [SerializeField] private List<ImageAnimation> TempList;  //stores the sprites whose animation is running at present 

    [Header("Slots Transforms")]
    [SerializeField] private Transform[] slotTransforms;

    [Header("Animated Sprites")]
    [SerializeField]
    internal Sprite[] Ten_Sprite;
    [SerializeField]
    private Sprite[] A_Sprite;
    [SerializeField]
    private Sprite[] BlueCoin_Sprite;
    [SerializeField]
    private Sprite[] ChineseKing_Sprite;
    [SerializeField]
    private Sprite[] Coin_Sprite;
    [SerializeField]
    private Sprite[] CoinPlant_Sprite;
    [SerializeField]
    private Sprite[] Drum_Sprite;
    [SerializeField]
    private Sprite[] GoldenCard_Sprite;
    [SerializeField]
    private Sprite[] GoldenShip_Sprite;
    [SerializeField]
    private Sprite[] GreenCoin_Sprite;
    [SerializeField]
    private Sprite[] J_Sprite;
    [SerializeField]
    private Sprite[] K_Sprite;
    [SerializeField]
    private Sprite[] Q_Sprite;
    [SerializeField]
    private Sprite[] RedCoin_Sprite;
    [SerializeField]
    private Sprite[] RedPacket_Sprite;


    [Header("Managers")]
    [SerializeField]
    private UIManager uiManager;
    [SerializeField]
    private SocketIOManager SocketManager;
    [SerializeField] private BonusManager bonusManager;
    [SerializeField] private RocketManager rocketManager;
    [SerializeField] private AudioController audioController;

    private List<Tweener> alltweens = new List<Tweener>();

    private Coroutine AutoSpinRoutine = null;
    private Coroutine tweenroutine;
    internal bool IsAutoSpin = false;
    private bool IsSpinning = false;
    //internal bool wasAutoSpin = false;
    private float _spinDelay = 0.3f;
    internal bool CheckPopups = false;
    private bool wasBonusActive = false;
    internal double currentBalance = 0;
    [SerializeField]
    private int IconSizeFactor = 100;       //set this parameter according to the size of the icon and spacing
    private int numberOfSlots = 5;          //number of columns
    private bool StopSpinToggle;
    private bool _stopAutoSpinPending;  // NEW: true when auto-spin is being cancelled via TriggerStopSpin
    internal int tweenHeight = 0;  //calculate the height at which tweening is done

    private void Start()
    {
        IsAutoSpin = false;

        tweenHeight = (15 * IconSizeFactor) - 280;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // NEW: Called by UIManager when the Stop Spin button is pressed.
    // Sets the flag that makes all reels stop simultaneously (same pattern as
    // LifeofLuxury's _stopSpinToggle).
    // ─────────────────────────────────────────────────────────────────────────
    internal void TriggerStopSpin()
    {
        StopSpinToggle = true;
        uiManager.StopSpinButton.interactable = false;

        // NEW: if Stop is pressed during auto-spin, cancel the auto-spin loop
        // after the current spin finishes landing (StopAutoSpin sets IsAutoSpin=false,
        // which makes AutoSpinCoroutine exit after yielding tweenroutine).
        if (IsAutoSpin)
        {
            _stopAutoSpinPending = true;
            StopAutoSpin();
        }
    }

    #region InitialFunctions
    internal void shuffleSlotImages(bool midTween = false)
    {
        for (int i = 0; i < totalImages.Count; i++)
        {
            for (int j = 0; j < totalImages[i].slotImages.Count; j++)
            {
                Sprite image = symbolImages[UnityEngine.Random.Range(0, 10)];
                if (!midTween)
                {
                    totalImages[i].slotImages[j].sprite = image;
                }
            }
        }
    }


    //function to populate animation sprites accordingly
    private void PopulateAnimationSprites(ImageAnimation animScript, int val)
    {

        if (animScript == null) return;
        animScript.textureArray.Clear();
        animScript.textureArray.TrimExcess();
        //animScript.AnimationSpeed = 100f;
        switch (val)
        {
            case 0:
                for (int i = 0; i < ChineseKing_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(ChineseKing_Sprite[i]);
                }
                //animScript.AnimationSpeed = 10f;
                break;
            case 9:
                for (int i = 0; i < Ten_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(Ten_Sprite[i]);
                }
                //animScript.AnimationSpeed = 10f;
                break;
            case 5:
                for (int i = 0; i < A_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(A_Sprite[i]);
                }
                //animScript.AnimationSpeed = 10f;
                break;
            case 11:
                for (int i = 0; i < BlueCoin_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(BlueCoin_Sprite[i]);
                }
                //animScript.AnimationSpeed = 10f;
                break;
            case 14:
                for (int i = 0; i < Coin_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(Coin_Sprite[i]);
                }
                //animScript.AnimationSpeed = 10f;
                break;
            case 2:
                for (int i = 0; i < CoinPlant_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(CoinPlant_Sprite[i]);
                }
                //animScript.AnimationSpeed = 10f;
                break;
            case 3:
                for (int i = 0; i < Drum_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(Drum_Sprite[i]);
                }
                //animScript.AnimationSpeed = 10f;
                break;
            case 10:
                for (int i = 0; i < GoldenCard_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(GoldenCard_Sprite[i]);
                }
                //animScript.AnimationSpeed = 10f;
                break;
            case 1:
                for (int i = 0; i < GoldenShip_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(GoldenShip_Sprite[i]);
                }
                //animScript.AnimationSpeed = 10f;
                break;
            case 13:
                for (int i = 0; i < GreenCoin_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(GreenCoin_Sprite[i]);
                }
                //animScript.AnimationSpeed = 10f;
                break;
            case 8:
                for (int i = 0; i < J_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(J_Sprite[i]);
                }
                //animScript.AnimationSpeed = 10f;
                break;
            case 6:
                for (int i = 0; i < K_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(K_Sprite[i]);
                }
                //animScript.AnimationSpeed = 10f;
                break;
            case 7:
                for (int i = 0; i < Q_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(Q_Sprite[i]);
                }
                //animScript.AnimationSpeed = 10f;
                break;
            case 12:
                for (int i = 0; i < RedCoin_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(RedCoin_Sprite[i]);
                }
                //animScript.AnimationSpeed = 10f;
                break;
            case 4:
                for (int i = 0; i < RedPacket_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(RedPacket_Sprite[i]);
                }
                //animScript.AnimationSpeed = 10f;
                break;
        }
    }
    #endregion

    #region SlotSpin

    internal void AutoSpin()
    {
        if (!IsAutoSpin)
        {
            IsAutoSpin = true;

            if (AutoSpinRoutine != null)
            {
                StopCoroutine(AutoSpinRoutine);
                AutoSpinRoutine = null;
            }
            AutoSpinRoutine = StartCoroutine(AutoSpinCoroutine());
        }
    }

    internal void StopAutoSpin()
    {
        //_audioController.PlayButtonAudio();
        if (IsAutoSpin)
        {
            IsAutoSpin = false;
            StartCoroutine(StopAutoSpinCoroutine());
        }
    }

    private IEnumerator AutoSpinCoroutine()
    {
        while (IsAutoSpin)
        {
            StartSlots(IsAutoSpin);
            yield return tweenroutine;
            yield return new WaitForSeconds(_spinDelay);
        }

    }

    private IEnumerator StopAutoSpinCoroutine()
    {
        yield return new WaitUntil(() => !IsSpinning);
        if (AutoSpinRoutine != null)
        {
            StopCoroutine(AutoSpinRoutine);
            AutoSpinRoutine = null;
        }
        if (tweenroutine != null)
        {
            StopCoroutine(tweenroutine);
            tweenroutine = null;
        }
        StopCoroutine(StopAutoSpinCoroutine());
        uiManager.AutoSpinButton.gameObject.SetActive(true);
        uiManager.AutoSpinButton.interactable = true;
        uiManager.StopAutoSpinButton.gameObject.SetActive(false);
        uiManager.SpinButton.interactable = true;
        uiManager.SpinButton.gameObject.SetActive(true);
        uiManager.StopSpinButton.gameObject.SetActive(false);
        uiManager.SetBetButtonsInteractable(true);   // re-enable bet buttons when auto-spin fully stops
        _stopAutoSpinPending = false;  // NEW: clear flag once UI is fully restored
    }
    //starts the spin process
    internal void StartSlots(bool autoSpin = false)
    {

        if (!autoSpin)
        {
            if (AutoSpinRoutine != null)
            {
                StopCoroutine(AutoSpinRoutine);
                StopCoroutine(tweenroutine);
                tweenroutine = null;
                AutoSpinRoutine = null;
            }
        }

        if (TempList.Count > 0)
        {
            StopGameAnimation();
        }

        tweenroutine = StartCoroutine(TweenRoutine());
    }

    //manage the Routine for spinning of the slots
    private IEnumerator TweenRoutine()
    {
        if (currentBalance < uiManager.currentTotalBet)
        {
            uiManager.LowBalPopup();
            StopAutoSpin();
            yield return new WaitForSeconds(1f);
            yield break;
        }
        else
        {
            currentBalance -= uiManager.currentTotalBet;
            uiManager.BalanceText.text = currentBalance.ToString();
            yield return new WaitForSeconds(0.2f);
        }

        audioController.PlaySpinStarts();

        IsSpinning = true;

        TempList.Clear();
        TempList.TrimExcess();

        for (int i = 0; i < numberOfSlots; i++)
        {
            InitializeTweening(slotTransforms[i]);
            yield return new WaitForSeconds(0.1f);
        }

        SocketManager.AccumulateResult(uiManager.betCounter);
        yield return new WaitUntil(() => SocketManager.isResultdone);

        List<BonusSymbolData> bonusSymbolsData = new List<BonusSymbolData>();

        for (int j = 0; j < SocketManager.resultData.payload.reels.Count; j++)
        {
            for (int i = 0; i < SocketManager.resultData.payload.reels[j].Count; i++)
            {
                if (int.TryParse(SocketManager.resultData.payload.reels[j][i], out int symbolId))
                {
                    resultImages[i].slotImages[j].sprite = symbolImages[symbolId];
                    resultImages[i].slotImages[j].GetComponentInChildren<TMP_Text>().text = "";
                    if (symbolId == 11 || symbolId == 12 || symbolId == 13)
                    {
                        ImageAnimation animScript = resultImages[i].slotImages[j].GetComponent<ImageAnimation>();
                        animScript.AnimationSpeed = 5f;
                        PopulateAnimationSprites(animScript, symbolId);
                        StartGameAnimation(resultImages[i].slotImages[j].gameObject);
                    }
                }
            }
        }

        //if (SocketManager.resultData.payload.bonusSymbolsInMatrix != null)
        {
            foreach (var bonusSymbol in SocketManager.resultData.payload.bonusSymbolsInMatrix)
            {
                int col = bonusSymbol.position[0]; // backend: [row, col]
                int row = bonusSymbol.position[1];

                var slotGO = resultImages[row].slotImages[col];
                var imageText = slotGO.GetComponentInChildren<TMP_Text>();
                imageText.text = bonusSymbol.value.ToString();

                int symbolId = int.Parse(SocketManager.resultData.payload.reels[col][row]);

                // Add to bonus symbols list
                bonusSymbolsData.Add(new BonusSymbolData
                {
                    position = new int[] { col, row },
                    symbolId = symbolId,
                    value = bonusSymbol.value
                });
                Debug.Log(bonusSymbolsData);
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // CHANGED: Wait a short grace period for the player to press Stop,
        // then hide the stop button — mirroring LifeofLuxury's behaviour.
        // If StopSpinToggle was already set (button pressed during server wait),
        // we skip straight to simultaneous stop.
        // ─────────────────────────────────────────────────────────────────────
        if (!StopSpinToggle)
        {
            // Give the player a short window to press Stop before reels snap
            for (int i = 0; i < 5; i++)
            {
                yield return new WaitForSeconds(0.1f);
                if (StopSpinToggle) break;
            }
            //uiManager.StopSpinButton.gameObject.SetActive(false);
        }

        for (int i = 0; i < numberOfSlots; i++)
        {
            yield return StopTweening(slotTransforms[i], i, StopSpinToggle);
        }
        StopSpinToggle = false;

        yield return alltweens[^1].WaitForCompletion();
        KillAllTweens();
        audioController.PlaySpinStops();

        var payload = SocketManager.resultData.payload;
        foreach (var win in payload.winningCombinations)
        {
            foreach (var pos in win.positions)
            {
                int col = pos[0]; // backend: [row, col]
                int row = pos[1];

                var slotGO = resultImages[row].slotImages[col];
                var animScript = slotGO.GetComponent<ImageAnimation>();
                animScript.AnimationSpeed = 10f;
                int symbolId = int.Parse(payload.reels[col][row]);

                if (symbolId == 11 || symbolId == 12 || symbolId == 13)
                {
                    continue;
                }
                else
                {
                    PopulateAnimationSprites(animScript, symbolId);
                    StartGameAnimation(slotGO.gameObject);
                }
            }
        }

        uiManager.WinAmountText.text = SocketManager.resultData.payload.win.ToString();

        if (SocketManager.resultData.payload.win > SocketManager.initialData.bets[uiManager.betCounter] * 7 && SocketManager.resultData.payload.win < SocketManager.initialData.bets[uiManager.betCounter] * 15)
        {
            uiManager.PopulateWin(1, SocketManager.resultData.payload.win);
            yield return new WaitUntil(() => !CheckPopups);
        }

        if (SocketManager.resultData.payload.win > SocketManager.initialData.bets[uiManager.betCounter] * 15)
        {
            uiManager.PopulateWin(2, SocketManager.resultData.payload.win);
            yield return new WaitUntil(() => !CheckPopups);
        }

        uiManager.StartTextAnim(currentBalance, SocketManager.resultData.payload.balance, uiManager.BalanceText, 1f);
        currentBalance = SocketManager.resultData.payload.balance;
        uiManager.BalanceText.text = SocketManager.resultData.payload.balance.ToString();

        // TRIGGER ROCKET SEQUENCE if bonus symbols exist
        if (bonusSymbolsData.Count > 0)
        {
            //Debug.Log("rocket Animation Started");
            rocketManager.RocketAnimation(bonusSymbolsData);
            yield return new WaitUntil(() => rocketManager.isRocketAnimationComplete);
            rocketManager.CrackerAnimation(bonusSymbolsData);
            //yield return new WaitUntil(() => rocketManager.isCrackerAnimationComplete);
            yield return new WaitUntil(() => rocketManager.blueCrackerAnimationComplete && rocketManager.redCrackerAnimationComplete && rocketManager.greenCrackerAnimationComplete);
            yield return new WaitForSeconds(1f);
        }
        // Then handle bonus game
        if (SocketManager.resultData.payload.bonusGame.isActive)
        {
            bonusManager.BonusStarted();
            wasBonusActive = true;
        }
        yield return new WaitUntil(() => bonusManager.isBonusComplete);
        yield return new WaitForSeconds(0.7f);

        if (wasBonusActive)
        {
            uiManager.PopulateWin(3, SocketManager.resultData.payload.totalWin);
            yield return new WaitUntil(() => !CheckPopups);
            wasBonusActive = false;
        }

        uiManager.WinAmountText.text = SocketManager.resultData.payload.totalWin.ToString();
        uiManager.StartTextAnim(currentBalance, SocketManager.resultData.payload.balance, uiManager.BalanceText, 1f);
        currentBalance = SocketManager.resultData.payload.balance;
        uiManager.BalanceText.text = SocketManager.resultData.payload.balance.ToString();

        IsSpinning = false;
        // NEW: if auto-spin was cancelled via TriggerStopSpin, StopAutoSpinCoroutine
        // is already waiting on IsSpinning and will restore the full UI (including
        // AutoSpin buttons). Don't call OnSpinEnd here to avoid conflicting UI state.
        if (!IsAutoSpin && !_stopAutoSpinPending)
        {
            uiManager.OnSpinEnd();
        }
    }


    #endregion

    //start the icons animation
    private void StartGameAnimation(GameObject animObjects)
    {
        ImageAnimation temp = animObjects.GetComponent<ImageAnimation>();

        temp.StartAnimation();
        TempList.Add(temp);
    }

    //stop the icons animation
    private void StopGameAnimation(bool WithSun = true)
    {
        for (int i = 0; i < TempList.Count; i++)
        {
            TempList[i].StopAnimation();
        }
        TempList.Clear();
        TempList.TrimExcess();

    }


    #region TweeningCode
    private void InitializeTweening(Transform slotTransform)
    {
        slotTransform.localPosition = new Vector2(slotTransform.localPosition.x, 0);
        Tweener tweener = slotTransform.DOLocalMoveY(-tweenHeight, 0.2f).SetLoops(-1, LoopType.Restart).SetDelay(0);
        tweener.Play();
        alltweens.Add(tweener);
    }

    private IEnumerator StopTweening(Transform slotTransform, int index, bool isStop)
    {
        if (!isStop)
        {
            bool isComplete = false;
            alltweens[index].OnStepComplete(() => isComplete = true);
            yield return new WaitUntil(() => isComplete);
        }
        alltweens[index].Kill();
        slotTransform.localPosition = new Vector2(slotTransform.localPosition.x, -329f);
        alltweens[index] = slotTransform.DOLocalMoveY(-250f, 0.5f).SetEase(Ease.OutElastic);
        if (!isStop)
        {
            yield return new WaitForSeconds(0.2f);
        }
        else
        {
            yield return null;
        }
    }


    private void KillAllTweens()
    {
        for (int i = 0; i < numberOfSlots; i++)
        {
            alltweens[i].Kill();
        }
        alltweens.Clear();

    }
    #endregion

}

[Serializable]
public class SlotImage
{
    public List<Image> slotImages = new List<Image>(10);
}

[Serializable]
public class Slottext
{
    public List<TMP_Text> slotImages = new List<TMP_Text>(10);
}