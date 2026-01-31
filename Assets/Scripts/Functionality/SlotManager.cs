using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Linq;
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

    private List<Tweener> alltweens = new List<Tweener>();

    private Coroutine AutoSpinRoutine = null;
    private Coroutine tweenroutine;
    internal bool IsAutoSpin = false;
    private bool IsSpinning = false;
    internal bool CheckPopups = false;
    internal double currentBalance = 0;
    [SerializeField]
    private int IconSizeFactor = 100;       //set this parameter according to the size of the icon and spacing
    private int numberOfSlots = 5;          //number of columns
    private bool StopSpinToggle;
    internal int tweenHeight = 0;  //calculate the height at which tweening is done

    private void Start()
    {
        IsAutoSpin = false;

        tweenHeight = (15 * IconSizeFactor) - 280;
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

        if (SlotAnimRoutine != null)
        {
            StopCoroutine(SlotAnimRoutine);
            SlotAnimRoutine = null;
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
            //uiManager.LowBalPopup();
            yield break;
        }
        else
        {
            currentBalance -= uiManager.currentTotalBet;
            uiManager.BalanceText.text = currentBalance.ToString();
            yield return new WaitForSeconds(0.2f);
        }

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
            }
        }

        for (int i = 0; i < numberOfSlots; i++)
        {
            yield return StopTweening(slotTransforms[i], i, StopSpinToggle);
        }
        StopSpinToggle = false;

        yield return alltweens[^1].WaitForCompletion();
        KillAllTweens();

        var payload = SocketManager.resultData.payload;
        foreach (var win in payload.winningCombinations)
        {
            foreach (var pos in win.positions)
            {
                int col = pos[0]; // backend: [row, col]
                int row = pos[1];

                // ---- FETCH OBJECTS ----
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
        if (SocketManager.resultData.payload.bonusGame.isActive)
        {
            bonusManager.BonusStarted();
        }
        yield return new WaitUntil(() => bonusManager.isBonusComplete);
        yield return new WaitUntil(() => !CheckPopups);

        IsSpinning = false;
        uiManager.OnSpinEnd();

    }

    private Coroutine SlotAnimRoutine = null;

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