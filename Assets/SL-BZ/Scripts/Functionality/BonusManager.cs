using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class BonusManager : MonoBehaviour
{

    [Header("UI Elements")]
    [SerializeField] private GameObject BonusPanel;
    [SerializeField] private GameObject GreenSpinPanel;
    [SerializeField] private GameObject BlueSpinPanel;
    [SerializeField] private GameObject RedSpinPanel;
    [SerializeField] private Sprite[] TotalBonusSprites;
    [SerializeField] private Sprite BrightCoinSprite;
    [SerializeField] private Sprite DullCoinSprite;

    [SerializeField] private Sprite MiniTextImage;
    [SerializeField] private Sprite MinorTextImage;
    [SerializeField] private Sprite MajorTextImage;

    [Header("GreeSpin Elements")]
    [SerializeField] private GameObject GreenSpinDoor;
    [SerializeField] private GameObject GreenSymbolAnimationObject;
    [SerializeField] private List<Sprite> GreenSpinDoorSymbolSprites;
    [SerializeField] private List<Sprite> GreenSpinLoopSprites;
    [SerializeField] private Button GreenSpinStartButton;
    [SerializeField] private GameObject GreenSpinSlotPanel;
    [SerializeField] private List<GameObject> GreenSpinIndicators;
    [SerializeField] private List<GameObject> GreenSpinSlots;
    [SerializeField] private List<SlotImage> GreenSpinSlotImages;
    [SerializeField] private List<SlotImage> GreenSpinResultSlotImages;
    [SerializeField] private List<SlotImage> GreenSpinCoinImages;
    [SerializeField] private GameObject GreeSpinMultiplierPanel;

    [Header("BlueSpin Elements")]
    [SerializeField] private GameObject BlueSpinDoor;
    [SerializeField] private GameObject BlueSymbolAnimationObject;
    [SerializeField] private List<Sprite> BlueSpinDoorSymbolSprites;
    [SerializeField] private List<Sprite> BlueSpinLoopSprites;
    [SerializeField] private Button BlueSpinStartButton;
    [SerializeField] private GameObject BlueSpinSlotPanel;
    [SerializeField] private List<GameObject> BlueSpinIndicators;
    [SerializeField] private List<GameObject> BlueSpinSlots;
    [SerializeField] private List<SlotImage> BlueSpinSlotImages;
    [SerializeField] private List<SlotImage> BlueSpinResultSlotImages;
    [SerializeField] private List<SlotImage> BlueSpinCoinImages;
    [SerializeField] private GameObject BlueSpinMultiplierPanel;
    [SerializeField] private GameObject BlueBirdAnimationObject;

    [Header("RedSpin Elements")]
    [SerializeField] private GameObject RedSpinDoor;
    [SerializeField] private GameObject RedSymbolAnimationObject;
    [SerializeField] private List<Sprite> RedSpinDoorSymbolSprites;
    [SerializeField] private List<Sprite> RedSpinLoopSprites;
    [SerializeField] private Button RedSpinStartButton;
    [SerializeField] private GameObject RedTopSpinSlotPanel;
    [SerializeField] private GameObject RedBottomSpinSlotPanel;
    [SerializeField] private List<GameObject> RedTopSpinIndicators;
    [SerializeField] private List<GameObject> RedBottomSpinIndicators;
    [SerializeField] private List<GameObject> RedTopSpinSlots;
    [SerializeField] private List<GameObject> RedBottomSpinSlots;
    [SerializeField] private List<SlotImage> RedTopSpinSlotImages;
    [SerializeField] private List<SlotImage> RedBottomSpinSlotImages;
    [SerializeField] private List<SlotImage> RedTopSpinResultSlotImages;
    [SerializeField] private List<SlotImage> RedBottomSpinResultSlotImages;
    [SerializeField] private List<SlotImage> RedTopSpinCoinImages;
    [SerializeField] private List<SlotImage> RedBottomSpinCoinImages;
    [SerializeField] private GameObject RedSpinMultiplierPanel;

    [Header("Managers")]
    [SerializeField] private SlotManager slotManager;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private SocketIOManager socketManager;
    [SerializeField] private AudioController audioManager;


    private List<Tweener> alltweens = new List<Tweener>();
    [SerializeField] private List<GameObject> multiplierObjects = new List<GameObject>();
    internal bool isBonusComplete = true;
    private bool isPayoutDone = true;
    private bool StopSpinToggle;
    private int tweenHeight = 0;
    Vector2 slotInitialPos = new Vector2(0, -1250f);
    private void Start()
    {
        if (GreenSpinStartButton) GreenSpinStartButton.onClick.AddListener(() => StartCoroutine(GreenSpin()));
        if (BlueSpinStartButton) BlueSpinStartButton.onClick.AddListener(() => StartCoroutine(BlueSpin()));
        if (RedSpinStartButton) RedSpinStartButton.onClick.AddListener(() => StartCoroutine(RedSpin()));
    }

    internal void BonusStarted()
    {
        audioManager.PlayBonusStarted();
        tweenHeight = slotManager.tweenHeight;
        isBonusComplete = false;

        multiplierObjects.Clear();

        GreenSpinPanel.SetActive(false);
        RedSpinPanel.SetActive(false);
        BlueSpinPanel.SetActive(false);
        BonusPanel.SetActive(true);
        if (socketManager.resultData.payload.bonusGame.features.extraSpins)
        {
            GreenSpinReset();
            GreenSpinPanel.SetActive(true);
            StartCoroutine(DoorAnimation(GreenSpinDoor, GreenSymbolAnimationObject, GreenSpinDoorSymbolSprites, GreenSpinLoopSprites));
            GreenSpinStartButton.gameObject.SetActive(true);
        }
        if (socketManager.resultData.payload.bonusGame.features.ultraSpins)
        {
            BlueSpinReset();
            BlueSpinPanel.SetActive(true);
            StartCoroutine(DoorAnimation(BlueSpinDoor, BlueSymbolAnimationObject, BlueSpinDoorSymbolSprites, BlueSpinLoopSprites));
            BlueSpinStartButton.gameObject.SetActive(true);
        }
        if (socketManager.resultData.payload.bonusGame.features.doubleReel)
        {
            RedSpinReset();
            RedSpinPanel.SetActive(true);
            StartCoroutine(DoorAnimation(RedSpinDoor, RedSymbolAnimationObject, RedSpinDoorSymbolSprites, RedSpinLoopSprites));
            RedSpinStartButton.gameObject.SetActive(true);
        }
    }

    private IEnumerator GreenSpin()
    {
        GreenSpinStartButton.interactable = false;
        yield return new WaitForSeconds(1f);
        GreenSpinStartButton.gameObject.SetActive(false);
        SlotAnimation(GreenSpinSlotPanel, -400f);
        InitializeBonusSlots(GreenSpinSlotImages, true, false, false);
        yield return new WaitForSeconds(0.5f);

        // Show initial indicators
        for (int i = 0; i < GreenSpinIndicators.Count; i++)
        {
            GreenSpinIndicators[i].SetActive(true);
            GreenSpinIndicators[i].GetComponent<ImageAnimation>().StartAnimation();
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(1f);

        int currentSpinCount = socketManager.resultData.payload.bonusGame.reselectSpinsRemaining;
        while (socketManager.resultData.payload.bonusGame.reselectSpinsRemaining > 0)
        {
            audioManager.PlaySpinStarts();

            foreach (var slot in GreenSpinSlots)
            {
                slot.SetActive(true);
            }
            yield return new WaitForSeconds(0.4f);

            for (int i = 0; i < GreenSpinSlots.Count; i++)
            {
                InitializeTweening(GreenSpinSlots[i].transform);
            }

            // Animate the current indicator (reverse animation as it's being used)
            if (currentSpinCount > 0)
            {
                GreenSpinIndicators[currentSpinCount - 1].GetComponent<ImageAnimation>().InverseAimationDirection();
                GreenSpinIndicators[currentSpinCount - 1].GetComponent<ImageAnimation>().ResetImageState();
                GreenSpinIndicators[currentSpinCount - 1].GetComponent<ImageAnimation>().StartAnimation();
                currentSpinCount--;
            }

            socketManager.AccumulateResult(uiManager.betCounter);
            yield return new WaitUntil(() => socketManager.isResultdone);

            for (int j = 0; j < socketManager.resultData.payload.reels.Count; j++)
            {
                for (int i = 0; i < socketManager.resultData.payload.reels[j].Count; i++)
                {
                    if (int.TryParse(socketManager.resultData.payload.reels[j][i], out int symbolId))
                    {
                        GreenSpinResultSlotImages[i].slotImages[j].sprite = TotalBonusSprites[symbolId];
                        GreenSpinResultSlotImages[i].slotImages[j].GetComponentInChildren<TMP_Text>().text = "";
                    }
                }
            }

            for (int i = 0; i < GreenSpinSlots.Count; i++)
            {
                yield return StopTweening(GreenSpinSlots[i].transform, i, StopSpinToggle);
            }
            StopSpinToggle = false;
            audioManager.PlaySpinStops();

            int spinsAfter = socketManager.resultData.payload.bonusGame.reselectSpinsRemaining;

            // If spins increased, activate new indicators
            if (spinsAfter > currentSpinCount)
            {
                audioManager.PlayRepeatSlotWin();
                for (int i = currentSpinCount; i < spinsAfter + 1 && i < GreenSpinIndicators.Count; i++)
                {
                    //GreenSpinIndicators[i].SetActive(true);
                    GreenSpinIndicators[i].GetComponent<ImageAnimation>().InverseAimationDirection();
                    GreenSpinIndicators[i].GetComponent<ImageAnimation>().ResetImageState();
                    GreenSpinIndicators[i].GetComponent<ImageAnimation>().StartAnimation();
                    yield return new WaitForSeconds(0.5f);
                }
                currentSpinCount = spinsAfter;
            }

            foreach (var bonusSymbol in socketManager.resultData.payload.bonusGame.bonusSymbols)
            {
                int col = bonusSymbol.position[0];
                int row = bonusSymbol.position[1];

                var slotGO = GreenSpinCoinImages[row].slotImages[col];
                slotGO.sprite = BrightCoinSprite;
                if (!slotGO.gameObject.activeSelf)
                {
                    audioManager.PlayGoldenCoin();
                }
                slotGO.gameObject.SetActive(true);
                var imageText = slotGO.GetComponentInChildren<TMP_Text>();
                if (bonusSymbol.jackpotType != null)
                {
                    imageText.text = "";
                    imageText.GetComponentInChildren<Image>(true).gameObject.SetActive(true);
                    switch (bonusSymbol.jackpotType)
                    {
                        case "MINI":
                            imageText.GetComponentInChildren<Image>(true).sprite = MiniTextImage;
                            break;
                        case "MINOR":
                            imageText.GetComponentInChildren<Image>(true).sprite = MinorTextImage;
                            break;
                        case "MAJOR":
                            imageText.GetComponentInChildren<Image>(true).sprite = MajorTextImage;
                            break;
                    }
                }
                else
                {
                    imageText.text = bonusSymbol.value.ToString();
                    imageText.GetComponentInChildren<Image>(true).gameObject.SetActive(false);
                }
            }

            yield return alltweens[^1].WaitForCompletion();
            KillAllTweens();

            foreach (var bonusSymbol in socketManager.resultData.payload.bonusGame.bonusSymbols)
            {
                int col = bonusSymbol.position[0];
                int row = bonusSymbol.position[1];
                var slotGO = GreenSpinCoinImages[row].slotImages[col];

                if (!multiplierObjects.Contains(slotGO.gameObject))
                {
                    multiplierObjects.Add(slotGO.gameObject);
                }
            }
        }

        for (int i = 0; i < GreenSpinIndicators.Count; i++)
        {
            GreenSpinIndicators[i].SetActive(false);
            GreenSpinIndicators[i].GetComponent<ImageAnimation>().InverseAimationDirection();
            GreenSpinIndicators[i].GetComponent<ImageAnimation>().ResetImageState();
        }

        yield return new WaitForSeconds(1f);
        StartCoroutine(MultiplierAnimation(GreeSpinMultiplierPanel));
        yield return new WaitUntil(() => isPayoutDone);

        if (socketManager.resultData.payload.win > socketManager.initialData.bets[uiManager.betCounter] * 15)
        {
            uiManager.PopulateWin(2, socketManager.resultData.payload.win);
            yield return new WaitUntil(() => !slotManager.CheckPopups);
        }
        else
        {
            uiManager.PopulateWin(1, socketManager.resultData.payload.win);
            yield return new WaitUntil(() => !slotManager.CheckPopups);
        }

        isBonusComplete = true;
        yield return new WaitForSeconds(1f);
        BonusPanel.SetActive(false);
        GreenSpinPanel.SetActive(false);
    }

    private IEnumerator BlueSpin()
    {
        BlueSpinStartButton.interactable = false;
        yield return new WaitForSeconds(1f);
        BlueSpinStartButton.gameObject.SetActive(false);
        SlotAnimation(BlueSpinSlotPanel, -400f);
        InitializeBonusSlots(BlueSpinSlotImages, false, false, true);
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < BlueSpinIndicators.Count; i++)
        {
            BlueSpinIndicators[i].SetActive(true);
            BlueSpinIndicators[i].GetComponent<ImageAnimation>().StartAnimation();
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(1f);

        int currentSpinCount = socketManager.resultData.payload.bonusGame.reselectSpinsRemaining;
        while (socketManager.resultData.payload.bonusGame.reselectSpinsRemaining > 0)
        {
            foreach (var slot in BlueSpinSlots)
            {
                slot.SetActive(true);
            }
            yield return new WaitForSeconds(0.4f);

            for (int i = 0; i < BlueSpinSlots.Count; i++)
            {
                InitializeTweening(BlueSpinSlots[i].transform);
            }

            // Animate the current indicator (reverse animation as it's being used)
            if (currentSpinCount > 0)
            {
                BlueSpinIndicators[currentSpinCount - 1].GetComponent<ImageAnimation>().InverseAimationDirection();
                BlueSpinIndicators[currentSpinCount - 1].GetComponent<ImageAnimation>().ResetImageState();
                BlueSpinIndicators[currentSpinCount - 1].GetComponent<ImageAnimation>().StartAnimation();
                currentSpinCount--;
            }

            socketManager.AccumulateResult(uiManager.betCounter);
            yield return new WaitUntil(() => socketManager.isResultdone);

            for (int j = 0; j < socketManager.resultData.payload.reels.Count; j++)
            {
                for (int i = 0; i < socketManager.resultData.payload.reels[j].Count; i++)
                {
                    if (int.TryParse(socketManager.resultData.payload.reels[j][i], out int symbolId))
                    {
                        BlueSpinResultSlotImages[i].slotImages[j].sprite = TotalBonusSprites[symbolId];
                        BlueSpinResultSlotImages[i].slotImages[j].GetComponentInChildren<TMP_Text>().text = "";
                    }
                }
            }

            for (int i = 0; i < BlueSpinSlots.Count; i++)
            {
                yield return StopTweening(BlueSpinSlots[i].transform, i, StopSpinToggle);
            }
            StopSpinToggle = false;

            int spinsAfter = socketManager.resultData.payload.bonusGame.reselectSpinsRemaining;

            // If spins increased, activate new indicators
            if (spinsAfter > currentSpinCount)
            {
                audioManager.PlayRepeatSlotWin();
                for (int i = currentSpinCount; i < spinsAfter + 1 && i < BlueSpinIndicators.Count; i++)
                {
                    //BlueSpinIndicators[i].SetActive(true);
                    BlueSpinIndicators[i].GetComponent<ImageAnimation>().InverseAimationDirection();
                    BlueSpinIndicators[i].GetComponent<ImageAnimation>().ResetImageState();
                    BlueSpinIndicators[i].GetComponent<ImageAnimation>().StartAnimation();
                    yield return new WaitForSeconds(0.5f);
                }
                currentSpinCount = spinsAfter;
            }

            foreach (var bonusSymbol in socketManager.resultData.payload.bonusGame.bonusSymbols)
            {
                int col = bonusSymbol.position[0];
                int row = bonusSymbol.position[1];

                var slotGO = BlueSpinCoinImages[row].slotImages[col];
                slotGO.sprite = BrightCoinSprite;
                if (!slotGO.gameObject.activeSelf)
                {
                    audioManager.PlayGoldenCoin();
                }
                slotGO.gameObject.SetActive(true);
                var imageText = slotGO.GetComponentInChildren<TMP_Text>();
                if (!bonusSymbol.added)
                {
                    if (bonusSymbol.jackpotType != null)
                    {
                        imageText.text = "";
                        imageText.GetComponentInChildren<Image>(true).gameObject.SetActive(true);
                        switch (bonusSymbol.jackpotType)
                        {
                            case "MINI":
                                imageText.GetComponentInChildren<Image>(true).sprite = MiniTextImage;
                                break;
                            case "MINOR":
                                imageText.GetComponentInChildren<Image>(true).sprite = MinorTextImage;
                                break;
                            case "MAJOR":
                                imageText.GetComponentInChildren<Image>(true).sprite = MajorTextImage;
                                break;
                        }
                    }
                    else
                    {
                        imageText.GetComponentInChildren<Image>(true).gameObject.SetActive(false);
                        imageText.text = bonusSymbol.value.ToString();
                    }
                }
            }

            for (int i = 0; i < socketManager.resultData.payload.bonusGame.bonusSymbols.Count; i++)
            {
                var bonusSymbol = socketManager.resultData.payload.bonusGame.bonusSymbols[i];
                if (bonusSymbol.added)
                {
                    var birdAnim = BlueBirdAnimationObject.GetComponent<ImageAnimation>();
                    birdAnim.StartAnimation();
                    yield return new WaitForSeconds(1f);
                    birdAnim.ResetImageState();
                    yield return new WaitForSeconds(0.3f);
                    break;
                }
            }

            // Show bird animation and numAdded for newly added symbols
            foreach (var bonusSymbol in socketManager.resultData.payload.bonusGame.bonusSymbols)
            {
                if (bonusSymbol.added)
                {
                    int col = bonusSymbol.position[0];
                    int row = bonusSymbol.position[1];

                    var slotGO = BlueSpinCoinImages[row].slotImages[col];

                    // Bird animation
                    // var birdAnim = BlueBirdAnimationObject.GetComponent<ImageAnimation>();
                    // birdAnim.StartAnimation();
                    // yield return new WaitForSeconds(1f);
                    // birdAnim.ResetImageState();
                    // yield return new WaitForSeconds(0.3f);

                    var imageText = slotGO.transform.GetChild(0).GetComponent<TMP_Text>();
                    imageText.text = bonusSymbol.value.ToString();

                    // Show numAdded animation
                    TMP_Text winText = slotGO.transform.GetChild(3).GetComponent<TMP_Text>();
                    RectTransform winRect = winText.GetComponent<RectTransform>();

                    winText.text = bonusSymbol.numAdded.ToString();
                    winText.alpha = 1f;

                    Vector2 startPos = winRect.anchoredPosition;
                    winRect.anchoredPosition = startPos;
                    winText.gameObject.SetActive(true);

                    // Pop animation
                    Sequence seq = DOTween.Sequence();
                    seq.Insert(0f, winRect.DOPunchScale(Vector3.one * 0.2f, 0.3f));
                    seq.Append(winRect.DOAnchorPosY(startPos.y + 170f, 0.8f).SetEase(Ease.OutCubic));
                    seq.Join(winText.DOFade(0f, 0.8f));

                    yield return seq.WaitForCompletion();

                    winText.gameObject.SetActive(false);
                    winText.alpha = 1f;
                    winRect.anchoredPosition = startPos;
                }
            }

            yield return alltweens[^1].WaitForCompletion();
            KillAllTweens();
            foreach (var bonusSymbol in socketManager.resultData.payload.bonusGame.bonusSymbols)
            {
                int col = bonusSymbol.position[0];
                int row = bonusSymbol.position[1];
                var slotGO = BlueSpinCoinImages[row].slotImages[col];

                Debug.Log("before check");

                if (!multiplierObjects.Contains(slotGO.gameObject))
                {
                    multiplierObjects.Add(slotGO.gameObject);
                    Debug.Log("Multiplier Object Added");
                }
            }
        }


        for (int i = 0; i < BlueSpinIndicators.Count; i++)
        {
            BlueSpinIndicators[i].SetActive(false);
            BlueSpinIndicators[i].GetComponent<ImageAnimation>().InverseAimationDirection();
            BlueSpinIndicators[i].GetComponent<ImageAnimation>().ResetImageState();
        }

        yield return new WaitForSeconds(1f);
        StartCoroutine(MultiplierAnimation(BlueSpinMultiplierPanel));
        yield return new WaitUntil(() => isPayoutDone);

        if (socketManager.resultData.payload.win > socketManager.initialData.bets[uiManager.betCounter] * 15)
        {
            uiManager.PopulateWin(2, socketManager.resultData.payload.win);
            yield return new WaitUntil(() => !slotManager.CheckPopups);
        }
        else
        {
            uiManager.PopulateWin(1, socketManager.resultData.payload.win);
            yield return new WaitUntil(() => !slotManager.CheckPopups);
        }

        isBonusComplete = true;
        yield return new WaitForSeconds(1f);
        BonusPanel.SetActive(false);
        BlueSpinPanel.SetActive(false);
    }

    // ── Red spin shared synchronisation state ──────────────────────────────────
    // Each reel coroutine sets its "round done" flag when it has finished ALL
    // animations for one round (spin stop + coin reveal + indicator update).
    // The coordinator waits for both, fires one backend request, writes the
    // result sprites, then raises _redRoundProceed so both reels loop again.
    private bool _redTopRoundDone;
    private bool _redBottomRoundDone;
    private bool _redRoundProceed;   // coordinator → reels: "result is ready, go"

    // ── Coordinator ────────────────────────────────────────────────────────────
    private IEnumerator RedSpin()
    {
        RedSpinStartButton.interactable = false;
        yield return new WaitForSeconds(1f);
        RedSpinStartButton.gameObject.SetActive(false);

        SlotAnimation(RedTopSpinSlotPanel, 200f);
        SlotAnimation(RedBottomSpinSlotPanel, -450f);

        InitializeBonusSlots(RedTopSpinSlotImages, false, true, false);
        InitializeBonusSlots(RedBottomSpinSlotImages, false, true, false);

        yield return new WaitForSeconds(0.5f);

        // Reveal all indicators for both reels before spinning begins
        for (int i = 0; i < RedTopSpinIndicators.Count; i++)
        {
            RedTopSpinIndicators[i].SetActive(true);
            RedTopSpinIndicators[i].GetComponent<ImageAnimation>().StartAnimation();
            yield return new WaitForSeconds(0.5f);
        }
        for (int i = 0; i < RedBottomSpinIndicators.Count; i++)
        {
            RedBottomSpinIndicators[i].SetActive(true);
            RedBottomSpinIndicators[i].GetComponent<ImageAnimation>().StartAnimation();
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(1f);

        // Reset sync flags before launching reel coroutines
        _redTopRoundDone    = false;
        _redBottomRoundDone = false;
        _redRoundProceed    = false;

        bool topHasSpins    = socketManager.resultData.payload.bonusGame.doubleReel.topReel.reselectSpinsRemaining > 0;
        bool bottomHasSpins = socketManager.resultData.payload.bonusGame.doubleReel.bottomReel.reselectSpinsRemaining > 0;

        // Launch both reel coroutines — they run independently and signal back
        Coroutine topCo    = topHasSpins    ? StartCoroutine(RedTopReelLoop())    : null;
        Coroutine bottomCo = bottomHasSpins ? StartCoroutine(RedBottomReelLoop()) : null;

        // If one reel has no spins at all, pre-mark it as always done
        if (!topHasSpins)    _redTopRoundDone    = true;
        if (!bottomHasSpins) _redBottomRoundDone = true;

        // ── Main coordinator loop ──────────────────────────────────────────────
        // Continues as long as either reel coroutine is still running
        while (topCo != null || bottomCo != null)
        {
            // Wait until every still-running reel has finished its round
            yield return new WaitUntil(() => _redTopRoundDone && _redBottomRoundDone);

            // Check whether any reel actually needs a result (might both be done)
            bool topActive    = socketManager.resultData.payload.bonusGame.doubleReel.topReel.reselectSpinsRemaining > 0;
            bool bottomActive = socketManager.resultData.payload.bonusGame.doubleReel.bottomReel.reselectSpinsRemaining > 0;

            if (!topActive && !bottomActive)
                break; // both reels exhausted their spins; skip the final request

            // Send ONE request for this round
            audioManager.PlaySpinStarts();
            socketManager.AccumulateResult(uiManager.betCounter);
            yield return new WaitUntil(() => socketManager.isResultdone);

            // Write result sprites into the hidden result layer for top reel
            if (topActive && socketManager.resultData.payload.bonusGame.doubleReel.topReel.reels != null)
            {
                for (int j = 0; j < socketManager.resultData.payload.bonusGame.doubleReel.topReel.reels.Count; j++)
                    for (int i = 0; i < socketManager.resultData.payload.bonusGame.doubleReel.topReel.reels[j].Count; i++)
                        if (int.TryParse(socketManager.resultData.payload.bonusGame.doubleReel.topReel.reels[j][i], out int sid))
                        {
                            RedTopSpinResultSlotImages[i].slotImages[j].sprite = TotalBonusSprites[sid];
                            RedTopSpinResultSlotImages[i].slotImages[j].GetComponentInChildren<TMP_Text>().text = "";
                        }
            }

            // Write result sprites for bottom reel
            if (bottomActive && socketManager.resultData.payload.bonusGame.doubleReel.bottomReel.reels != null)
            {
                for (int j = 0; j < socketManager.resultData.payload.bonusGame.doubleReel.bottomReel.reels.Count; j++)
                    for (int i = 0; i < socketManager.resultData.payload.bonusGame.doubleReel.bottomReel.reels[j].Count; i++)
                        if (int.TryParse(socketManager.resultData.payload.bonusGame.doubleReel.bottomReel.reels[j][i], out int sid))
                        {
                            RedBottomSpinResultSlotImages[i].slotImages[j].sprite = TotalBonusSprites[sid];
                            RedBottomSpinResultSlotImages[i].slotImages[j].GetComponentInChildren<TMP_Text>().text = "";
                        }
            }

            // Reset per-round flags and release both reel coroutines to proceed
            _redTopRoundDone    = false;
            _redBottomRoundDone = false;
            _redRoundProceed    = true;

            yield return null; // one frame so WaitUntil in reels can catch the flag

            _redRoundProceed = false;

            // Mark inactive reels as permanently done for future coordinator loops
            if (socketManager.resultData.payload.bonusGame.doubleReel.topReel.reselectSpinsRemaining <= 0)
            {
                topCo = null;
                _redTopRoundDone = true;
            }
            if (socketManager.resultData.payload.bonusGame.doubleReel.bottomReel.reselectSpinsRemaining <= 0)
            {
                bottomCo = null;
                _redBottomRoundDone = true;
            }
        }
        
        yield return new WaitForSeconds(1f);

        // ── Collect all coins into the multiplier list ─────────────────────────
        foreach (var bonusSymbol in socketManager.resultData.payload.bonusGame.doubleReel.topReel.bonusSymbols)
        {
            var slotGO = RedTopSpinCoinImages[bonusSymbol.position[1]].slotImages[bonusSymbol.position[0]];
            if (!multiplierObjects.Contains(slotGO.gameObject))
                multiplierObjects.Add(slotGO.gameObject);
        }
        foreach (var bonusSymbol in socketManager.resultData.payload.bonusGame.doubleReel.bottomReel.bonusSymbols)
        {
            var slotGO = RedBottomSpinCoinImages[bonusSymbol.position[1]].slotImages[bonusSymbol.position[0]];
            if (!multiplierObjects.Contains(slotGO.gameObject))
                multiplierObjects.Add(slotGO.gameObject);
        }

        // Hide indicators
        for (int i = 0; i < RedTopSpinIndicators.Count; i++)
        {
            RedTopSpinIndicators[i].SetActive(false);
            RedTopSpinIndicators[i].GetComponent<ImageAnimation>().InverseAimationDirection();
            RedTopSpinIndicators[i].GetComponent<ImageAnimation>().ResetImageState();
        }
        for (int i = 0; i < RedBottomSpinIndicators.Count; i++)
        {
            RedBottomSpinIndicators[i].SetActive(false);
            RedBottomSpinIndicators[i].GetComponent<ImageAnimation>().InverseAimationDirection();
            RedBottomSpinIndicators[i].GetComponent<ImageAnimation>().ResetImageState();
        }

        yield return new WaitForSeconds(2f);
        StartCoroutine(MultiplierAnimation(RedSpinMultiplierPanel));
        yield return new WaitUntil(() => isPayoutDone);

        if (socketManager.resultData.payload.win > socketManager.initialData.bets[uiManager.betCounter] * 15)
        {
            uiManager.PopulateWin(2, socketManager.resultData.payload.win);
            yield return new WaitUntil(() => !slotManager.CheckPopups);
        }
        else
        {
            uiManager.PopulateWin(1, socketManager.resultData.payload.win);
            yield return new WaitUntil(() => !slotManager.CheckPopups);
        }

        isBonusComplete = true;
        yield return new WaitForSeconds(1f);
        RedSpinPanel.SetActive(false);
        BonusPanel.SetActive(false);
    }

    // ── TOP reel independent loop ───────────────────────────────────────────────
    // Runs its own spin → stop → coin reveal → indicator update each round,
    // then signals the coordinator and waits for the next result before looping.
    private IEnumerator RedTopReelLoop()
    {
        int spinCount = socketManager.resultData.payload.bonusGame.doubleReel.topReel.reselectSpinsRemaining;

        while (true)
        {
            // ── Spin phase ─────────────────────────────────────────────────────
            foreach (var slot in RedTopSpinSlots) slot.SetActive(true);
            yield return new WaitForSeconds(0.4f);

            // Consume one indicator
            if (spinCount > 0)
            {
                RedTopSpinIndicators[spinCount - 1].GetComponent<ImageAnimation>().InverseAimationDirection();
                RedTopSpinIndicators[spinCount - 1].GetComponent<ImageAnimation>().ResetImageState();
                RedTopSpinIndicators[spinCount - 1].GetComponent<ImageAnimation>().StartAnimation();
                spinCount--;
            }

            // Start spinning tweens
            var tweens = new List<Tweener>();
            for (int i = 0; i < RedTopSpinSlots.Count; i++)
            {
                RedTopSpinSlots[i].transform.localPosition = new Vector2(RedTopSpinSlots[i].transform.localPosition.x, 0);
                Tweener tw = RedTopSpinSlots[i].transform.DOLocalMoveY(-tweenHeight, 0.2f).SetLoops(-1, LoopType.Restart).SetDelay(0);
                tw.Play();
                tweens.Add(tw);
            }

            // ── Signal coordinator that we are spinning (wait for result) ──────
            // The coordinator fires the request once BOTH reels reach this point.
            // We mark ourselves "done-spinning" by signalling ready here.
            // But we can't stop yet — we need the result sprites first.
            // So: signal, wait for coordinator to deliver result, then stop.
            _redTopRoundDone = true;
            yield return new WaitUntil(() => _redRoundProceed);

            // ── Stop phase — result sprites already written by coordinator ──────
            for (int i = 0; i < tweens.Count; i++)
            {
                bool stepDone = false;
                tweens[i].OnStepComplete(() => stepDone = true);
                yield return new WaitUntil(() => stepDone);
                tweens[i].Kill();
                RedTopSpinSlots[i].transform.localPosition = new Vector2(RedTopSpinSlots[i].transform.localPosition.x, -329f);
                tweens[i] = RedTopSpinSlots[i].transform.DOLocalMoveY(-250f, 0.5f).SetEase(Ease.OutElastic);
                yield return new WaitForSeconds(0.2f);
            }
            if (tweens.Count > 0) yield return tweens[^1].WaitForCompletion();
            foreach (var tw in tweens) { if (tw != null && tw.IsActive()) tw.Kill(); }
            audioManager.PlaySpinStops();

            // ── Coin reveal + indicator update ─────────────────────────────────
            int spinsAfter = socketManager.resultData.payload.bonusGame.doubleReel.topReel.reselectSpinsRemaining;
            if (spinsAfter > spinCount)
            {
                audioManager.PlayRepeatSlotWin();
                for (int i = spinCount; i < spinsAfter + 1 && i < RedTopSpinIndicators.Count; i++)
                {
                    RedTopSpinIndicators[i].GetComponent<ImageAnimation>().InverseAimationDirection();
                    RedTopSpinIndicators[i].GetComponent<ImageAnimation>().ResetImageState();
                    RedTopSpinIndicators[i].GetComponent<ImageAnimation>().StartAnimation();
                    yield return new WaitForSeconds(0.5f);
                }
                spinCount = spinsAfter;
            }

            foreach (var bonusSymbol in socketManager.resultData.payload.bonusGame.doubleReel.topReel.bonusSymbols)
            {
                var slotGO = RedTopSpinCoinImages[bonusSymbol.position[1]].slotImages[bonusSymbol.position[0]];
                slotGO.sprite = BrightCoinSprite;
                if (!slotGO.gameObject.activeSelf) audioManager.PlayGoldenCoin();
                slotGO.gameObject.SetActive(true);
                var imageText = slotGO.GetComponentInChildren<TMP_Text>();
                if (bonusSymbol.jackpotType != null)
                {
                    imageText.text = "";
                    imageText.GetComponentInChildren<Image>(true).gameObject.SetActive(true);
                    switch (bonusSymbol.jackpotType)
                    {
                        case "MINI":  imageText.GetComponentInChildren<Image>(true).sprite = MiniTextImage;  break;
                        case "MINOR": imageText.GetComponentInChildren<Image>(true).sprite = MinorTextImage; break;
                        case "MAJOR": imageText.GetComponentInChildren<Image>(true).sprite = MajorTextImage; break;
                    }
                }
                else
                {
                    imageText.GetComponentInChildren<Image>(true).gameObject.SetActive(false);
                    imageText.text = bonusSymbol.value.ToString();
                }
            }

            // ── Check if this reel has more spins ──────────────────────────────
            if (socketManager.resultData.payload.bonusGame.doubleReel.topReel.reselectSpinsRemaining <= 0)
                yield break; // no more spins; exit so coordinator can null out topCo

            // Ready for next round — wait until coordinator resets _redRoundProceed
            yield return new WaitUntil(() => !_redRoundProceed);
            _redTopRoundDone = false;
        }
    }

    // ── BOTTOM reel independent loop ────────────────────────────────────────────
    // Exact mirror of RedTopReelLoop but for the bottom reel data and GameObjects.
    private IEnumerator RedBottomReelLoop()
    {
        int spinCount = socketManager.resultData.payload.bonusGame.doubleReel.bottomReel.reselectSpinsRemaining;

        while (true)
        {
            // ── Spin phase ─────────────────────────────────────────────────────
            foreach (var slot in RedBottomSpinSlots) slot.SetActive(true);
            yield return new WaitForSeconds(0.4f);

            // Consume one indicator
            if (spinCount > 0)
            {
                RedBottomSpinIndicators[spinCount - 1].GetComponent<ImageAnimation>().InverseAimationDirection();
                RedBottomSpinIndicators[spinCount - 1].GetComponent<ImageAnimation>().ResetImageState();
                RedBottomSpinIndicators[spinCount - 1].GetComponent<ImageAnimation>().StartAnimation();
                spinCount--;
            }

            // Start spinning tweens
            var tweens = new List<Tweener>();
            for (int i = 0; i < RedBottomSpinSlots.Count; i++)
            {
                RedBottomSpinSlots[i].transform.localPosition = new Vector2(RedBottomSpinSlots[i].transform.localPosition.x, 0);
                Tweener tw = RedBottomSpinSlots[i].transform.DOLocalMoveY(-tweenHeight, 0.2f).SetLoops(-1, LoopType.Restart).SetDelay(0);
                tw.Play();
                tweens.Add(tw);
            }

            // ── Signal coordinator that we are ready for the result ────────────
            _redBottomRoundDone = true;
            yield return new WaitUntil(() => _redRoundProceed);

            // ── Stop phase ─────────────────────────────────────────────────────
            for (int i = 0; i < tweens.Count; i++)
            {
                bool stepDone = false;
                tweens[i].OnStepComplete(() => stepDone = true);
                yield return new WaitUntil(() => stepDone);
                tweens[i].Kill();
                RedBottomSpinSlots[i].transform.localPosition = new Vector2(RedBottomSpinSlots[i].transform.localPosition.x, -329f);
                tweens[i] = RedBottomSpinSlots[i].transform.DOLocalMoveY(-250f, 0.5f).SetEase(Ease.OutElastic);
                yield return new WaitForSeconds(0.2f);
            }
            if (tweens.Count > 0) yield return tweens[^1].WaitForCompletion();
            foreach (var tw in tweens) { if (tw != null && tw.IsActive()) tw.Kill(); }
            audioManager.PlaySpinStops();

            // ── Coin reveal + indicator update ─────────────────────────────────
            int spinsAfter = socketManager.resultData.payload.bonusGame.doubleReel.bottomReel.reselectSpinsRemaining;
            if (spinsAfter > spinCount)
            {
                audioManager.PlayRepeatSlotWin();
                for (int i = spinCount; i < spinsAfter + 1 && i < RedBottomSpinIndicators.Count; i++)
                {
                    RedBottomSpinIndicators[i].GetComponent<ImageAnimation>().InverseAimationDirection();
                    RedBottomSpinIndicators[i].GetComponent<ImageAnimation>().ResetImageState();
                    RedBottomSpinIndicators[i].GetComponent<ImageAnimation>().StartAnimation();
                    yield return new WaitForSeconds(0.5f);
                }
                spinCount = spinsAfter;
            }

            foreach (var bonusSymbol in socketManager.resultData.payload.bonusGame.doubleReel.bottomReel.bonusSymbols)
            {
                var slotGO = RedBottomSpinCoinImages[bonusSymbol.position[1]].slotImages[bonusSymbol.position[0]];
                slotGO.sprite = BrightCoinSprite;
                if (!slotGO.gameObject.activeSelf) audioManager.PlayGoldenCoin();
                slotGO.gameObject.SetActive(true);
                var imageText = slotGO.GetComponentInChildren<TMP_Text>();
                if (bonusSymbol.jackpotType != null)
                {
                    imageText.text = "";
                    imageText.GetComponentInChildren<Image>(true).gameObject.SetActive(true);
                    switch (bonusSymbol.jackpotType)
                    {
                        case "MINI":  imageText.GetComponentInChildren<Image>(true).sprite = MiniTextImage;  break;
                        case "MINOR": imageText.GetComponentInChildren<Image>(true).sprite = MinorTextImage; break;
                        case "MAJOR": imageText.GetComponentInChildren<Image>(true).sprite = MajorTextImage; break;
                    }
                }
                else
                {
                    imageText.GetComponentInChildren<Image>(true).gameObject.SetActive(false);
                    imageText.text = bonusSymbol.value.ToString();
                }
            }

            // ── Check if this reel has more spins ──────────────────────────────
            if (socketManager.resultData.payload.bonusGame.doubleReel.bottomReel.reselectSpinsRemaining <= 0)
                yield break;

            // Ready for next round
            yield return new WaitUntil(() => !_redRoundProceed);
            _redBottomRoundDone = false;
        }
    }
    private void SlotAnimation(GameObject slotPanel, float positionY)
    {
        slotInitialPos = slotPanel.GetComponent<RectTransform>().anchoredPosition;
        slotPanel.GetComponent<RectTransform>().DOAnchorPosY(positionY, 1f).SetEase(Ease.Linear);
    }

    private void InitializeBonusSlots(List<SlotImage> slotImage, bool isGreenSpin, bool isRedSpin, bool isBlueSpin)
    {
        for (int i = 0; i < slotImage.Count; i++)
        {
            for (int j = 0; j < slotImage[i].slotImages.Count; j++)
            {
                Sprite image = TotalBonusSprites[UnityEngine.Random.Range(0, 10)];
                slotImage[i].slotImages[j].sprite = image;
            }
        }

        // Just display initial bonus symbols, don't add to multiplierObjects
        // They will be collected at the end of all spins
        if (isGreenSpin)
        {
            foreach (var bonusSymbol in socketManager.resultData.payload.bonusSymbolsInMatrix)
            {
                if (bonusSymbol.name == "BonusGreen")
                {
                    int col = bonusSymbol.position[0];
                    int row = bonusSymbol.position[1];

                    var slotGO = GreenSpinCoinImages[row].slotImages[col];

                    slotGO.sprite = BrightCoinSprite;
                    slotGO.gameObject.SetActive(true);
                    var imageText = slotGO.GetComponentInChildren<TMP_Text>();

                    imageText.text = bonusSymbol.value.ToString();
                }
            }
        }
        if (isBlueSpin)
        {
            foreach (var bonusSymbol in socketManager.resultData.payload.bonusSymbolsInMatrix)
            {
                if (bonusSymbol.name == "BonusBlue")
                {
                    int col = bonusSymbol.position[0];
                    int row = bonusSymbol.position[1];

                    var slotGO = BlueSpinCoinImages[row].slotImages[col];

                    slotGO.sprite = BrightCoinSprite;
                    slotGO.gameObject.SetActive(true);
                    var imageText = slotGO.GetComponentInChildren<TMP_Text>();

                    imageText.text = bonusSymbol.value.ToString();
                }
            }
        }
        if (isRedSpin)
        {
            foreach (var bonusSymbol in socketManager.resultData.payload.bonusSymbolsInMatrix)
            {
                if (bonusSymbol.name == "BonusRed")
                {
                    int col = bonusSymbol.position[0];
                    int row = bonusSymbol.position[1];

                    var slotGOTop = RedTopSpinCoinImages[row].slotImages[col];

                    slotGOTop.sprite = BrightCoinSprite;
                    slotGOTop.gameObject.SetActive(true);
                    var imageTextTop = slotGOTop.GetComponentInChildren<TMP_Text>();

                    imageTextTop.text = bonusSymbol.value.ToString();

                    var slotGOBottom = RedBottomSpinCoinImages[row].slotImages[col];

                    slotGOBottom.sprite = BrightCoinSprite;
                    slotGOBottom.gameObject.SetActive(true);
                    var imageTextBottom = slotGOBottom.GetComponentInChildren<TMP_Text>();

                    imageTextBottom.text = bonusSymbol.value.ToString();
                }
            }
        }
    }

    private IEnumerator MultiplierAnimation(GameObject payoutPanel)
    {
        isPayoutDone = false;
        Debug.Log($"Multiplier Animation Started with {multiplierObjects.Count} coins");

        payoutPanel.SetActive(true);

        TMP_Text payoutText = payoutPanel.transform.GetChild(0).GetComponent<TMP_Text>();
        ImageAnimation payoutAnim = payoutPanel.transform.GetChild(1).GetComponent<ImageAnimation>();

        float totalPayout = 0f;
        payoutText.text = "0";

        // Work on a copy and clear original
        List<GameObject> coins = new List<GameObject>(multiplierObjects);
        multiplierObjects.Clear();

        foreach (GameObject coin in coins)
        {
            // Safety checks
            if (coin == null)
            {
                Debug.LogWarning("Skipping null coin");
                continue;
            }

            if (coin.transform.childCount < 3)
            {
                Debug.LogWarning($"Coin {coin.name} missing children (has {coin.transform.childCount}, needs 3)");
                continue;
            }

            Debug.Log($"Animating coin: {coin.name}");

            GameObject light = coin.transform.GetChild(1).gameObject;
            RectTransform lightRect = light.GetComponent<RectTransform>();
            RectTransform targetRect = coin.transform.GetChild(2).GetComponent<RectTransform>();

            // Reset light position before animation
            lightRect.localPosition = Vector3.zero;

            coin.GetComponent<Image>().sprite = DullCoinSprite;
            light.SetActive(true);

            Tweener moveTween = lightRect.DOLocalMove(targetRect.localPosition, 1f).SetEase(Ease.Linear);

            yield return moveTween.WaitForCompletion();
            audioManager.PlayLightSound();
            light.SetActive(false);

            // Reset light position after animation
            lightRect.localPosition = Vector3.zero;

            // Trigger payout flash
            payoutAnim.ResetImageState();
            payoutAnim.StartAnimation();

            // Add value with safety.
            // Coins with a jackpotType (Mini / Minor / Major) have coinText.text == ""
            // and show a sprite Image instead. In that case we read the payout value
            // from the matching UIManager jackpot text label.
            TMP_Text coinText = coin.GetComponentInChildren<TMP_Text>();
            float coinValue = 0f;
            bool valueResolved = false;

            if (coinText != null && !string.IsNullOrEmpty(coinText.text))
            {
                // Normal numeric coin
                if (float.TryParse(coinText.text, out coinValue))
                {
                    valueResolved = true;
                }
                else
                {
                    Debug.LogWarning($"Failed to parse coin value: {coinText.text}");
                }
            }
            else
            {
                // Jackpot coin — text is blank; sprite Image child is active.
                // Identify which jackpot it is by comparing its sprite to the
                // known jackpot sprites, then read the value from UIManager.
                Image jackpotImage = coinText != null
                    ? coinText.GetComponentInChildren<Image>(true)
                    : coin.GetComponentInChildren<Image>(true);

                if (jackpotImage != null && jackpotImage.gameObject.activeSelf)
                {
                    string jackpotValueText = null;
                    if (jackpotImage.sprite == MiniTextImage)
                        jackpotValueText = uiManager.MiniText.text;
                    else if (jackpotImage.sprite == MinorTextImage)
                        jackpotValueText = uiManager.MinorText.text;
                    else if (jackpotImage.sprite == MajorTextImage)
                        jackpotValueText = uiManager.MajorText.text;

                    if (jackpotValueText != null && float.TryParse(jackpotValueText, out coinValue))
                    {
                        valueResolved = true;
                    }
                    else
                    {
                        Debug.LogWarning($"Coin {coin.name} jackpot image found but value could not be resolved (sprite={jackpotImage.sprite?.name}, text={jackpotValueText})");
                    }
                }
                else
                {
                    Debug.LogWarning($"Coin {coin.name} has no valid text and no active jackpot image");
                }
            }

            if (valueResolved)
            {
                float fromValue = totalPayout;
                totalPayout += coinValue;

                uiManager.StartTextAnim(fromValue, totalPayout, payoutText, 0.5f);

                yield return new WaitForSeconds(0.55f);
            }

            payoutAnim.RevertToInitialState();
        }

        Debug.Log("Multiplier Animation Ended");
        isPayoutDone = true;
    }


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
        for (int i = 0; i < GreenSpinSlots.Count; i++)
        {
            alltweens[i].Kill();
        }
        alltweens.Clear();
    }

    private void GreenSpinReset()
    {
        GreenSpinSlotPanel.GetComponent<RectTransform>().anchoredPosition = slotInitialPos;
        GreeSpinMultiplierPanel.SetActive(false);
        for (int i = 0; i < GreenSpinIndicators.Count; i++)
        {
            GreenSpinIndicators[i].SetActive(false);
        }
        for (int i = 0; i < GreenSpinSlots.Count; i++)
        {
            GreenSpinSlots[i].SetActive(false);
        }
        for (int i = 0; i < GreenSpinCoinImages.Count; i++)
        {
            for (int j = 0; j < GreenSpinCoinImages[i].slotImages.Count; j++)
            {
                GreenSpinCoinImages[i].slotImages[j].gameObject.SetActive(false);
            }
        }
    }

    private void BlueSpinReset()
    {
        BlueSpinSlotPanel.GetComponent<RectTransform>().anchoredPosition = slotInitialPos;
        BlueSpinMultiplierPanel.SetActive(false);
        for (int i = 0; i < BlueSpinIndicators.Count; i++)
        {
            BlueSpinIndicators[i].SetActive(false);
        }
        for (int i = 0; i < BlueSpinSlots.Count; i++)
        {
            BlueSpinSlots[i].SetActive(false);
        }
        for (int i = 0; i < BlueSpinCoinImages.Count; i++)
        {
            for (int j = 0; j < BlueSpinCoinImages[i].slotImages.Count; j++)
            {
                BlueSpinCoinImages[i].slotImages[j].gameObject.SetActive(false);
            }
        }
    }

    private void RedSpinReset()
    {
        RedTopSpinSlotPanel.GetComponent<RectTransform>().anchoredPosition = slotInitialPos;
        RedBottomSpinSlotPanel.GetComponent<RectTransform>().anchoredPosition = slotInitialPos;
        RedSpinMultiplierPanel.SetActive(false);
        for (int i = 0; i < RedTopSpinIndicators.Count; i++)
        {
            RedTopSpinIndicators[i].SetActive(false);
        }
        for (int i = 0; i < RedBottomSpinIndicators.Count; i++)
        {
            RedBottomSpinIndicators[i].SetActive(false);
        }
        for (int i = 0; i < RedTopSpinSlots.Count; i++)
        {
            RedTopSpinSlots[i].SetActive(false);
        }
        for (int i = 0; i < RedBottomSpinSlots.Count; i++)
        {
            RedBottomSpinSlots[i].SetActive(false);
        }
        for (int i = 0; i < RedTopSpinCoinImages.Count; i++)
        {
            for (int j = 0; j < RedTopSpinCoinImages[i].slotImages.Count; j++)
            {
                RedTopSpinCoinImages[i].slotImages[j].gameObject.SetActive(false);
            }
        }
        for (int i = 0; i < RedBottomSpinCoinImages.Count; i++)
        {
            for (int j = 0; j < RedBottomSpinCoinImages[i].slotImages.Count; j++)
            {
                RedBottomSpinCoinImages[i].slotImages[j].gameObject.SetActive(false);
            }
        }
    }

    private IEnumerator DoorAnimation(GameObject doorObject, GameObject symbolAnimationObject, List<Sprite> doorSymbolSprites, List<Sprite> loopSprites)
    {
        //yield return new WaitForSeconds(2f);
        doorObject.GetComponent<ImageAnimation>().ResetImageState();
        symbolAnimationObject.GetComponent<ImageAnimation>().textureArray = null;
        symbolAnimationObject.GetComponent<ImageAnimation>().textureArray = doorSymbolSprites;
        symbolAnimationObject.GetComponent<ImageAnimation>().ResetImageState();
        symbolAnimationObject.GetComponent<ImageAnimation>().doLoopAnimation = false;
        symbolAnimationObject.GetComponent<ImageAnimation>().AnimationSpeed = 85f;
        symbolAnimationObject.GetComponent<Image>().preserveAspect = true;
        doorObject.GetComponent<ImageAnimation>().StartAnimation();
        symbolAnimationObject.GetComponent<ImageAnimation>().StartAnimation();
        yield return new WaitUntil(() => symbolAnimationObject.GetComponent<ImageAnimation>().IsComplete);
        symbolAnimationObject.GetComponent<ImageAnimation>().textureArray = null;
        symbolAnimationObject.GetComponent<ImageAnimation>().IsComplete = false;
        symbolAnimationObject.GetComponent<ImageAnimation>().textureArray = loopSprites;
        symbolAnimationObject.GetComponent<ImageAnimation>().ResetImageState();
        symbolAnimationObject.GetComponent<ImageAnimation>().doLoopAnimation = true;
        symbolAnimationObject.GetComponent<ImageAnimation>().AnimationSpeed = 47f;
        symbolAnimationObject.GetComponent<Image>().preserveAspect = true;
        symbolAnimationObject.GetComponent<ImageAnimation>().StartAnimation();
        yield return new WaitForSeconds(0.1f);
    }

}