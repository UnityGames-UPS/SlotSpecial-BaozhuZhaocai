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
    private int remainingSpins = 0;
    Vector2 slotInitialPos = new Vector2(0, -1250f);
    private void Start()
    {
        if (GreenSpinStartButton) GreenSpinStartButton.onClick.AddListener(() => StartCoroutine(GreenSpin()));
        if (BlueSpinStartButton) BlueSpinStartButton.onClick.AddListener(() => StartCoroutine(BlueSpin()));
        if (RedSpinStartButton) RedSpinStartButton.onClick.AddListener(() => StartCoroutine(RedSpin()));
        //MultiplierAnimation(GreeSpinMultiplierPanel);
    }

    internal void BonusStarted()
    {
        tweenHeight = slotManager.tweenHeight;
        isBonusComplete = false;
        
        // Clear any leftover coins from previous bonus rounds
        multiplierObjects.Clear();
        
        GreenSpinPanel.SetActive(false);
        RedSpinPanel.SetActive(false);
        BlueSpinPanel.SetActive(false);
        BonusPanel.SetActive(true);
        if (socketManager.resultData.payload.bonusGame.features.extraSpins)
        {
            //StartCoroutine(GreenSpin());
            GreenSpinReset();
            GreenSpinPanel.SetActive(true);
            //yield return new WaitForSeconds(0.1f);
            // GreenSymbolAnimationObject.GetComponent<ImageAnimation>().textureArray = null;
            // GreenSymbolAnimationObject.GetComponent<ImageAnimation>().textureArray = GreenSpinDoorSymbolSprites;
            // GreenSpinDoor.GetComponent<ImageAnimation>().StartAnimation();
            StartCoroutine(DoorAnimation(GreenSpinDoor, GreenSymbolAnimationObject, GreenSpinDoorSymbolSprites, GreenSpinLoopSprites));
            GreenSpinStartButton.gameObject.SetActive(true);
        }
        if (socketManager.resultData.payload.bonusGame.features.ultraSpins)
        {
            BlueSpinReset();
            BlueSpinPanel.SetActive(true);
            //BlueSpinDoor.GetComponent<ImageAnimation>().StartAnimation();
            StartCoroutine(DoorAnimation(BlueSpinDoor, BlueSymbolAnimationObject, BlueSpinDoorSymbolSprites, BlueSpinLoopSprites));
            BlueSpinStartButton.gameObject.SetActive(true);
        }
        if (socketManager.resultData.payload.bonusGame.features.doubleReel)
        {
            RedSpinReset();
            RedSpinPanel.SetActive(true);
            //RedSpinDoor.GetComponent<ImageAnimation>().StartAnimation();
            StartCoroutine(DoorAnimation(RedSpinDoor, RedSymbolAnimationObject, RedSpinDoorSymbolSprites, RedSpinLoopSprites));
            RedSpinStartButton.gameObject.SetActive(true);
        }
        // else
        // {
        //     isBonusComplete = true;
        // }
    }

    private IEnumerator GreenSpin()
    {
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

            // Activate slots
            foreach (var slot in GreenSpinSlots)
            {
                slot.SetActive(true);
            }
            yield return new WaitForSeconds(0.4f);

            // Start tweening
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

            // Get result from server
            socketManager.AccumulateResult(uiManager.betCounter);
            yield return new WaitUntil(() => socketManager.isResultdone);

            // Populate results
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

            // Stop tweening
            for (int i = 0; i < GreenSpinSlots.Count; i++)
            {
                yield return StopTweening(GreenSpinSlots[i].transform, i, StopSpinToggle);
            }
            StopSpinToggle = false;

            int spinsAfter = socketManager.resultData.payload.bonusGame.reselectSpinsRemaining;

            // If spins increased, activate new indicators
            if (spinsAfter > currentSpinCount)
            {
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

            // Display all current bonus symbols (just display, don't add to multiplierObjects yet)
            foreach (var bonusSymbol in socketManager.resultData.payload.bonusGame.bonusSymbols)
            {
                int col = bonusSymbol.position[0];
                int row = bonusSymbol.position[1];

                var slotGO = GreenSpinCoinImages[row].slotImages[col];
                slotGO.sprite = BrightCoinSprite;
                slotGO.gameObject.SetActive(true);
                var imageText = slotGO.GetComponentInChildren<TMP_Text>();
                imageText.text = bonusSymbol.value.ToString();
            }

            yield return alltweens[^1].WaitForCompletion();
            KillAllTweens();
        }

        // ALL SPINS COMPLETE - Now collect all the coins for multiplier animation
        foreach (var bonusSymbol in socketManager.resultData.payload.bonusGame.bonusSymbols)
        {
            int col = bonusSymbol.position[0];
            int row = bonusSymbol.position[1];
            var slotGO = GreenSpinCoinImages[row].slotImages[col];
            
            // Add to multiplier list (no duplicates since we're adding from final server state)
            if (!multiplierObjects.Contains(slotGO.gameObject))
            {
                multiplierObjects.Add(slotGO.gameObject);
            }
        }

        // Hide all indicators
        for (int i = 0; i < GreenSpinIndicators.Count; i++)
        {
            GreenSpinIndicators[i].SetActive(false);
            GreenSpinIndicators[i].GetComponent<ImageAnimation>().InverseAimationDirection();
            GreenSpinIndicators[i].GetComponent<ImageAnimation>().ResetImageState();
        }

        yield return new WaitForSeconds(1f);
        StartCoroutine(MultiplierAnimation(GreeSpinMultiplierPanel));
        yield return new WaitUntil(() => isPayoutDone);

        isBonusComplete = true;
        yield return new WaitForSeconds(1f);
        BonusPanel.SetActive(false);
        GreenSpinPanel.SetActive(false);
    }

    private IEnumerator BlueSpin()
    {
        yield return new WaitForSeconds(1f);
        BlueSpinStartButton.gameObject.SetActive(false);
        SlotAnimation(BlueSpinSlotPanel, -400f);
        InitializeBonusSlots(BlueSpinSlotImages, false, false, true);
        yield return new WaitForSeconds(0.5f);

        // Show initial indicators
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
            // Activate slots
            foreach (var slot in BlueSpinSlots)
            {
                slot.SetActive(true);
            }
            yield return new WaitForSeconds(0.4f);

            // Start tweening
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

            // Get result from server
            socketManager.AccumulateResult(uiManager.betCounter);
            yield return new WaitUntil(() => socketManager.isResultdone);

            // Populate results
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

            // Stop tweening
            for (int i = 0; i < BlueSpinSlots.Count; i++)
            {
                yield return StopTweening(BlueSpinSlots[i].transform, i, StopSpinToggle);
            }
            StopSpinToggle = false;

            int spinsAfter = socketManager.resultData.payload.bonusGame.reselectSpinsRemaining;

            // If spins increased, activate new indicators
            if (spinsAfter > currentSpinCount)
            {
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

            // Display all current bonus symbols (just display, don't add to multiplierObjects yet)
            foreach (var bonusSymbol in socketManager.resultData.payload.bonusGame.bonusSymbols)
            {
                int col = bonusSymbol.position[0];
                int row = bonusSymbol.position[1];

                var slotGO = BlueSpinCoinImages[row].slotImages[col];
                slotGO.sprite = BrightCoinSprite;
                slotGO.gameObject.SetActive(true);
                var imageText = slotGO.GetComponentInChildren<TMP_Text>();
                imageText.text = bonusSymbol.value.ToString();
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
                    var birdAnim = BlueBirdAnimationObject.GetComponent<ImageAnimation>();
                    birdAnim.StartAnimation();
                    yield return new WaitForSeconds(1f);
                    birdAnim.ResetImageState();
                    yield return new WaitForSeconds(0.3f);

                    // Show numAdded animation
                    TMP_Text winText = slotGO.transform.GetChild(2).GetComponent<TMP_Text>();
                    RectTransform winRect = winText.GetComponent<RectTransform>();

                    winText.text = bonusSymbol.numAdded.ToString();
                    winText.alpha = 1f;

                    Vector2 startPos = winRect.anchoredPosition;
                    winRect.anchoredPosition = startPos;
                    winText.gameObject.SetActive(true);

                    // Pop animation
                    Sequence seq = DOTween.Sequence();
                    seq.Insert(0f, winRect.DOPunchScale(Vector3.one * 0.2f, 0.3f));
                    seq.Append(winRect.DOAnchorPosY(startPos.y + 100f, 0.8f).SetEase(Ease.OutCubic));
                    seq.Join(winText.DOFade(0f, 0.8f));

                    yield return seq.WaitForCompletion();

                    // Cleanup
                    winText.gameObject.SetActive(false);
                    winText.alpha = 1f;
                    winRect.anchoredPosition = startPos;
                }
            }

            yield return alltweens[^1].WaitForCompletion();
            KillAllTweens();
        }

        // ALL SPINS COMPLETE - Now collect all the coins for multiplier animation
        foreach (var bonusSymbol in socketManager.resultData.payload.bonusGame.bonusSymbols)
        {
            int col = bonusSymbol.position[0];
            int row = bonusSymbol.position[1];
            var slotGO = BlueSpinCoinImages[row].slotImages[col];
            
            // Add to multiplier list (no duplicates since we're adding from final server state)
            if (!multiplierObjects.Contains(slotGO.gameObject))
            {
                multiplierObjects.Add(slotGO.gameObject);
            }
        }

        // Hide all indicators
        for (int i = 0; i < BlueSpinIndicators.Count; i++)
        {
            BlueSpinIndicators[i].SetActive(false);
            BlueSpinIndicators[i].GetComponent<ImageAnimation>().InverseAimationDirection();
            BlueSpinIndicators[i].GetComponent<ImageAnimation>().ResetImageState();
        }

        yield return new WaitForSeconds(1f);
        StartCoroutine(MultiplierAnimation(BlueSpinMultiplierPanel));
        yield return new WaitUntil(() => isPayoutDone);

        isBonusComplete = true;
        yield return new WaitForSeconds(1f);
        BonusPanel.SetActive(false);
        BlueSpinPanel.SetActive(false);
    }

    private IEnumerator RedSpin()
    {
        yield return new WaitForSeconds(1f);
        RedSpinStartButton.gameObject.SetActive(false);

        // Animate both slot panels simultaneously
        SlotAnimation(RedTopSpinSlotPanel, 200f);
        SlotAnimation(RedBottomSpinSlotPanel, -450f);

        // Initialize both sets of bonus slots with random sprites
        InitializeBonusSlots(RedTopSpinSlotImages, false, true, false);
        InitializeBonusSlots(RedBottomSpinSlotImages, false, true, false);

        //yield return new WaitForSeconds(0.5f);

        yield return new WaitForSeconds(0.5f);

        // Start both reel coroutines simultaneously
        Coroutine topReelCoroutine = StartCoroutine(RedTopReelSpin());
        Coroutine bottomReelCoroutine = StartCoroutine(RedBottomReelSpin());

        // Wait for both reels to complete
        yield return topReelCoroutine;
        yield return bottomReelCoroutine;

        for (int i = 0; i < RedTopSpinIndicators.Count; i++)
        {
            RedTopSpinIndicators[i].SetActive(false);
            RedTopSpinIndicators[i].GetComponent<ImageAnimation>().InverseAimationDirection();
            RedTopSpinIndicators[i].GetComponent<ImageAnimation>().ResetImageState();
            //yield return new WaitForSeconds(0.5f);
        }

        for (int i = 0; i < RedBottomSpinIndicators.Count; i++)
        {
            RedBottomSpinIndicators[i].SetActive(false);
            RedBottomSpinIndicators[i].GetComponent<ImageAnimation>().InverseAimationDirection();
            RedBottomSpinIndicators[i].GetComponent<ImageAnimation>().ResetImageState();
            //yield return new WaitForSeconds(0.5f);
        }

        // Both reels are complete, play the multiplier animation (same as green/blue)
        yield return new WaitForSeconds(1f);
        StartCoroutine(MultiplierAnimation(RedSpinMultiplierPanel));
        yield return new WaitUntil(() => isPayoutDone);

        isBonusComplete = true;
        yield return new WaitForSeconds(1f);
        //uiManager.TotalWinPopup();
        RedSpinPanel.SetActive(false);
        BonusPanel.SetActive(false);
    }

    private IEnumerator RedTopReelSpin()
    {
        for (int i = 0; i < RedTopSpinIndicators.Count; i++)
        {
            RedTopSpinIndicators[i].SetActive(true);
            RedTopSpinIndicators[i].GetComponent<ImageAnimation>().StartAnimation();
            yield return new WaitForSeconds(1.5f);
        }

        // Track which symbols we've already added to multiplierObjects
        int currentSpinCount = socketManager.resultData.payload.bonusGame.doubleReel.topReel.reselectSpinsRemaining;
        while (socketManager.resultData.payload.bonusGame.doubleReel.topReel.reselectSpinsRemaining > 0)
        {

            // Activate top reel slots
            foreach (var slot in RedTopSpinSlots)
            {
                slot.SetActive(true);
            }

            yield return new WaitForSeconds(0.4f);

            // Initialize tweening for top reel
            List<Tweener> topTweens = new List<Tweener>();
            for (int i = 0; i < RedTopSpinSlots.Count; i++)
            {
                RedTopSpinSlots[i].transform.localPosition = new Vector2(RedTopSpinSlots[i].transform.localPosition.x, 0);
                Tweener tweener = RedTopSpinSlots[i].transform.DOLocalMoveY(-tweenHeight, 0.2f).SetLoops(-1, LoopType.Restart).SetDelay(0);
                tweener.Play();
                topTweens.Add(tweener);
            }

            // Animate the current indicator
            if (currentSpinCount > 0)
            {
                RedTopSpinIndicators[currentSpinCount - 1].GetComponent<ImageAnimation>().InverseAimationDirection();
                RedTopSpinIndicators[currentSpinCount - 1].GetComponent<ImageAnimation>().ResetImageState();
                RedTopSpinIndicators[currentSpinCount - 1].GetComponent<ImageAnimation>().StartAnimation();
                currentSpinCount--;
            }

            // Request result from server
            socketManager.AccumulateResult(uiManager.betCounter);
            yield return new WaitUntil(() => socketManager.isResultdone);

            // Populate top reel results
            if (socketManager.resultData.payload.bonusGame.doubleReel.topReel.reels != null)
            {
                for (int j = 0; j < socketManager.resultData.payload.bonusGame.doubleReel.topReel.reels.Count; j++)
                {
                    for (int i = 0; i < socketManager.resultData.payload.bonusGame.doubleReel.topReel.reels[j].Count; i++)
                    {
                        if (int.TryParse(socketManager.resultData.payload.bonusGame.doubleReel.topReel.reels[j][i], out int symbolId))
                        {
                            RedTopSpinResultSlotImages[i].slotImages[j].sprite = TotalBonusSprites[symbolId];
                            RedTopSpinResultSlotImages[i].slotImages[j].GetComponentInChildren<TMP_Text>().text = "";
                        }
                    }
                }
            }

            // Stop tweening for top reel
            for (int i = 0; i < RedTopSpinSlots.Count; i++)
            {
                bool isComplete = false;
                topTweens[i].OnStepComplete(() => isComplete = true);
                yield return new WaitUntil(() => isComplete);

                topTweens[i].Kill();
                RedTopSpinSlots[i].transform.localPosition = new Vector2(RedTopSpinSlots[i].transform.localPosition.x, -329f);
                topTweens[i] = RedTopSpinSlots[i].transform.DOLocalMoveY(-250f, 0.5f).SetEase(Ease.OutElastic);

                yield return new WaitForSeconds(0.2f);
            }

            int spinsAfter = socketManager.resultData.payload.bonusGame.doubleReel.topReel.reselectSpinsRemaining;

            // If spins increased, activate new indicators
            if (spinsAfter > currentSpinCount)
            {
                for (int i = currentSpinCount; i < spinsAfter + 1 && i < RedTopSpinIndicators.Count; i++)
                {
                    //RedTopSpinIndicators[i].SetActive(true);
                    RedTopSpinIndicators[i].GetComponent<ImageAnimation>().InverseAimationDirection();
                    RedTopSpinIndicators[i].GetComponent<ImageAnimation>().ResetImageState();
                    RedTopSpinIndicators[i].GetComponent<ImageAnimation>().StartAnimation();
                    yield return new WaitForSeconds(0.5f);
                }
                currentSpinCount = spinsAfter;
            }

            // Display bonus symbols for top reel (just display, don't add to multiplierObjects yet)
            foreach (var bonusSymbol in socketManager.resultData.payload.bonusGame.doubleReel.topReel.bonusSymbols)
            {
                int col = bonusSymbol.position[0];
                int row = bonusSymbol.position[1];

                var slotGO = RedTopSpinCoinImages[row].slotImages[col];
                slotGO.sprite = BrightCoinSprite;
                slotGO.gameObject.SetActive(true);
                var imageText = slotGO.GetComponentInChildren<TMP_Text>();
                imageText.text = bonusSymbol.value.ToString();
            }

            // Wait for the last tween to complete and clean up
            yield return topTweens[^1].WaitForCompletion();
            foreach (var tween in topTweens)
            {
                if (tween != null && tween.IsActive())
                {
                    tween.Kill();
                }
            }
            topTweens.Clear();
        }
        
        // ALL TOP REEL SPINS COMPLETE - Collect all coins for multiplier animation
        foreach (var bonusSymbol in socketManager.resultData.payload.bonusGame.doubleReel.topReel.bonusSymbols)
        {
            int col = bonusSymbol.position[0];
            int row = bonusSymbol.position[1];
            var slotGO = RedTopSpinCoinImages[row].slotImages[col];
            
            // Add to multiplier list
            if (!multiplierObjects.Contains(slotGO.gameObject))
            {
                multiplierObjects.Add(slotGO.gameObject);
            }
        }
    }

    private IEnumerator RedBottomReelSpin()
    {
        for (int i = 0; i < RedBottomSpinIndicators.Count; i++)
        {
            RedBottomSpinIndicators[i].SetActive(true);
            RedBottomSpinIndicators[i].GetComponent<ImageAnimation>().StartAnimation();
            yield return new WaitForSeconds(1.5f);
        }

        // Track which symbols we've already added to multiplierObjects
        int currentSpinCount = socketManager.resultData.payload.bonusGame.doubleReel.bottomReel.reselectSpinsRemaining;
        while (socketManager.resultData.payload.bonusGame.doubleReel.bottomReel.reselectSpinsRemaining > 0)
        {

            // Activate bottom reel slots
            foreach (var slot in RedBottomSpinSlots)
            {
                slot.SetActive(true);
            }

            yield return new WaitForSeconds(0.4f);

            // Initialize tweening for bottom reel
            List<Tweener> bottomTweens = new List<Tweener>();
            for (int i = 0; i < RedBottomSpinSlots.Count; i++)
            {
                RedBottomSpinSlots[i].transform.localPosition = new Vector2(RedBottomSpinSlots[i].transform.localPosition.x, 0);
                Tweener tweener = RedBottomSpinSlots[i].transform.DOLocalMoveY(-tweenHeight, 0.2f).SetLoops(-1, LoopType.Restart).SetDelay(0);
                tweener.Play();
                bottomTweens.Add(tweener);
            }

            // Animate the current indicator
            if (currentSpinCount > 0)
            {
                RedBottomSpinIndicators[currentSpinCount - 1].GetComponent<ImageAnimation>().InverseAimationDirection();
                RedBottomSpinIndicators[currentSpinCount - 1].GetComponent<ImageAnimation>().ResetImageState();
                RedBottomSpinIndicators[currentSpinCount - 1].GetComponent<ImageAnimation>().StartAnimation();
                currentSpinCount--;
            }

            // Request result from server (bottom reel has its own call)
            socketManager.AccumulateResult(uiManager.betCounter);
            yield return new WaitUntil(() => socketManager.isResultdone);

            // Populate bottom reel results
            if (socketManager.resultData.payload.bonusGame.doubleReel.bottomReel.reels != null)
            {
                for (int j = 0; j < socketManager.resultData.payload.bonusGame.doubleReel.bottomReel.reels.Count; j++)
                {
                    for (int i = 0; i < socketManager.resultData.payload.bonusGame.doubleReel.bottomReel.reels[j].Count; i++)
                    {
                        if (int.TryParse(socketManager.resultData.payload.bonusGame.doubleReel.bottomReel.reels[j][i], out int symbolId))
                        {
                            RedBottomSpinResultSlotImages[i].slotImages[j].sprite = TotalBonusSprites[symbolId];
                            RedBottomSpinResultSlotImages[i].slotImages[j].GetComponentInChildren<TMP_Text>().text = "";
                        }
                    }
                }
            }

            // Stop tweening for bottom reel
            for (int i = 0; i < RedBottomSpinSlots.Count; i++)
            {
                bool isComplete = false;
                bottomTweens[i].OnStepComplete(() => isComplete = true);
                yield return new WaitUntil(() => isComplete);

                bottomTweens[i].Kill();
                RedBottomSpinSlots[i].transform.localPosition = new Vector2(RedBottomSpinSlots[i].transform.localPosition.x, -329f);
                bottomTweens[i] = RedBottomSpinSlots[i].transform.DOLocalMoveY(-250f, 0.5f).SetEase(Ease.OutElastic);

                yield return new WaitForSeconds(0.2f);
            }

            int spinsAfter = socketManager.resultData.payload.bonusGame.doubleReel.bottomReel.reselectSpinsRemaining;

            // If spins increased, activate new indicators
            if (spinsAfter > currentSpinCount)
            {
                for (int i = currentSpinCount; i < spinsAfter + 1 && i < RedBottomSpinIndicators.Count; i++)
                {
                    //RedBottomSpinIndicators[i].SetActive(true);
                    RedBottomSpinIndicators[i].GetComponent<ImageAnimation>().InverseAimationDirection();
                    RedBottomSpinIndicators[i].GetComponent<ImageAnimation>().ResetImageState();
                    RedBottomSpinIndicators[i].GetComponent<ImageAnimation>().StartAnimation();
                    yield return new WaitForSeconds(0.5f);
                }
                currentSpinCount = spinsAfter;
            }

            // Display bonus symbols for bottom reel (just display, don't add to multiplierObjects yet)
            foreach (var bonusSymbol in socketManager.resultData.payload.bonusGame.doubleReel.bottomReel.bonusSymbols)
            {
                int col = bonusSymbol.position[0];
                int row = bonusSymbol.position[1];

                var slotGO = RedBottomSpinCoinImages[row].slotImages[col];
                slotGO.sprite = BrightCoinSprite;
                slotGO.gameObject.SetActive(true);
                var imageText = slotGO.GetComponentInChildren<TMP_Text>();
                imageText.text = bonusSymbol.value.ToString();
            }

            // Wait for the last tween to complete and clean up
            yield return bottomTweens[^1].WaitForCompletion();
            foreach (var tween in bottomTweens)
            {
                if (tween != null && tween.IsActive())
                {
                    tween.Kill();
                }
            }
            bottomTweens.Clear();
        }
        
        // ALL BOTTOM REEL SPINS COMPLETE - Collect all coins for multiplier animation
        foreach (var bonusSymbol in socketManager.resultData.payload.bonusGame.doubleReel.bottomReel.bonusSymbols)
        {
            int col = bonusSymbol.position[0];
            int row = bonusSymbol.position[1];
            var slotGO = RedBottomSpinCoinImages[row].slotImages[col];
            
            // Add to multiplier list
            if (!multiplierObjects.Contains(slotGO.gameObject))
            {
                multiplierObjects.Add(slotGO.gameObject);
            }
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

            Tweener moveTween = lightRect.DOLocalMove(targetRect.localPosition, 1.7f).SetEase(Ease.Linear);

            yield return moveTween.WaitForCompletion();
            light.SetActive(false);

            // Reset light position after animation
            lightRect.localPosition = Vector3.zero;

            // Trigger payout flash
            payoutAnim.ResetImageState();
            payoutAnim.StartAnimation();

            // Add value with safety
            TMP_Text coinText = coin.GetComponentInChildren<TMP_Text>();
            if (coinText != null && !string.IsNullOrEmpty(coinText.text))
            {
                if (float.TryParse(coinText.text, out float coinValue))
                {
                    float fromValue = totalPayout;
                    totalPayout += coinValue;

                    uiManager.StartTextAnim(fromValue, totalPayout, payoutText, 0.5f);

                    yield return new WaitForSeconds(0.55f);
                }
                else
                {
                    Debug.LogWarning($"Failed to parse coin value: {coinText.text}");
                }
            }
            else
            {
                Debug.LogWarning($"Coin {coin.name} has no valid text");
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
        symbolAnimationObject.GetComponent<ImageAnimation>().doLoopAnimation = false;
        symbolAnimationObject.GetComponent<ImageAnimation>().ResetImageState();
        doorObject.GetComponent<ImageAnimation>().StartAnimation();
        symbolAnimationObject.GetComponent<ImageAnimation>().StartAnimation();
        yield return new WaitUntil(() => symbolAnimationObject.GetComponent<ImageAnimation>().IsComplete);
        symbolAnimationObject.GetComponent<ImageAnimation>().textureArray = null;
        symbolAnimationObject.GetComponent<ImageAnimation>().IsComplete = false;
        symbolAnimationObject.GetComponent<ImageAnimation>().textureArray = loopSprites;
        symbolAnimationObject.GetComponent<ImageAnimation>().ResetImageState();
        symbolAnimationObject.GetComponent<ImageAnimation>().doLoopAnimation = true;
        symbolAnimationObject.GetComponent<ImageAnimation>().AnimationSpeed = 35f;
        symbolAnimationObject.GetComponent<ImageAnimation>().StartAnimation();
        yield return new WaitForSeconds(0.1f);
    }

    private void SpinCountDecreaseAnimation(List<GameObject> indicators)
    {

    }

}