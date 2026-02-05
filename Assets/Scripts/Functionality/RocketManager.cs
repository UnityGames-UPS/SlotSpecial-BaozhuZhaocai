using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System;
using NUnit.Framework;

public class RocketManager : MonoBehaviour
{
    [Header("Symbol & Cracker Objects")]
    [SerializeField] private GameObject BlueSymbolObject;
    [SerializeField] private GameObject RedSymbolObject;
    [SerializeField] private GameObject GreenSymbolObject;
    [SerializeField] private GameObject BlueCrackerObject;
    [SerializeField] private GameObject RedCrackerObject;
    [SerializeField] private GameObject GreenCrackerObject;

    [Header("Rocket Hit Object")]
    [SerializeField] private GameObject BlueRocketHitObject;
    [SerializeField] private GameObject RedRocketHitObject;
    [SerializeField] private GameObject GreenRocketHitObject;

    [Header("Rocket Paths")]
    [SerializeField] private List<Reel> rocketPaths = new List<Reel>();

    [Header("Rocket Prefabs & Animation")]
    [SerializeField] private GameObject RocketParent;
    [SerializeField] private GameObject BlueRocket;
    [SerializeField] private GameObject RedRocket;
    [SerializeField] private GameObject GreenRocket;

    [Header("Symbol Animation Sprites")]
    [SerializeField] private Sprite[] BlueSymbolLoopSprites;
    [SerializeField] private Sprite[] RedSymbolLoopSprites;
    [SerializeField] private Sprite[] GreenSymbolLoopSprites;
    [SerializeField] private Sprite[] BlueSymbolShootUpSprites;
    [SerializeField] private Sprite[] RedSymbolShootUpSprites;
    [SerializeField] private Sprite[] GreenSymbolShootUpSprites;

    [Header("Full Burst Animations")]
    [SerializeField] private Sprite[] BlueCrackerBlast;
    [SerializeField] private Sprite[] RedCrackerBlast;
    [SerializeField] private Sprite[] GreenCrackerBlast;

    [Header("Blue Crackers")]
    [SerializeField] private Sprite[] BlueEmptyLoop;
    [SerializeField] private Sprite[] BlueOneLoop;
    [SerializeField] private Sprite[] BlueTwoLoop;
    [SerializeField] private Sprite[] BlueThreeLoop;
    [SerializeField] private Sprite[] BlueFourLoop;
    [SerializeField] private Sprite[] BlueFiveLoop;
    [SerializeField] private Sprite[] BlueSixLoop;
    [SerializeField] private Sprite[] BlueSevenLoop;
    [SerializeField] private Sprite[] BlueEightLoop;

    [SerializeField] private Sprite[] BlueOneBlast;
    [SerializeField] private Sprite[] BlueTwoBlast;
    [SerializeField] private Sprite[] BlueThreeBlast;
    [SerializeField] private Sprite[] BlueFourBlast;
    [SerializeField] private Sprite[] BlueFiveBlast;
    [SerializeField] private Sprite[] BlueSixBlast;
    [SerializeField] private Sprite[] BlueSevenBlast;
    [SerializeField] private Sprite[] BlueEightBlast;

    [Header("Red Crackers")]
    [SerializeField] private Sprite[] RedEmptyLoop;
    [SerializeField] private Sprite[] RedOneLoop;
    [SerializeField] private Sprite[] RedTwoLoop;
    [SerializeField] private Sprite[] RedThreeLoop;
    [SerializeField] private Sprite[] RedFourLoop;
    [SerializeField] private Sprite[] RedFiveLoop;
    [SerializeField] private Sprite[] RedSixLoop;
    [SerializeField] private Sprite[] RedSevenLoop;
    [SerializeField] private Sprite[] RedEightLoop;

    [SerializeField] private Sprite[] RedOneBlast;
    [SerializeField] private Sprite[] RedTwoBlast;
    [SerializeField] private Sprite[] RedThreeBlast;
    [SerializeField] private Sprite[] RedFourBlast;
    [SerializeField] private Sprite[] RedFiveBlast;
    [SerializeField] private Sprite[] RedSixBlast;
    [SerializeField] private Sprite[] RedSevenBlast;
    [SerializeField] private Sprite[] RedEightBlast;

    [Header("Green Crackers")]
    [SerializeField] private Sprite[] GreenEmptyLoop;
    [SerializeField] private Sprite[] GreenOneLoop;
    [SerializeField] private Sprite[] GreenTwoLoop;
    [SerializeField] private Sprite[] GreenThreeLoop;
    [SerializeField] private Sprite[] GreenFourLoop;
    [SerializeField] private Sprite[] GreenFiveLoop;
    [SerializeField] private Sprite[] GreenSixLoop;
    [SerializeField] private Sprite[] GreenSevenLoop;
    [SerializeField] private Sprite[] GreenEightLoop;

    [SerializeField] private Sprite[] GreenOneBlast;
    [SerializeField] private Sprite[] GreenTwoBlast;
    [SerializeField] private Sprite[] GreenThreeBlast;
    [SerializeField] private Sprite[] GreenFourBlast;
    [SerializeField] private Sprite[] GreenFiveBlast;
    [SerializeField] private Sprite[] GreenSixBlast;
    [SerializeField] private Sprite[] GreenSevenBlast;
    [SerializeField] private Sprite[] GreenEightBlast;

    [Header("Timing Settings")]
    [SerializeField] private float symbolLoopDuration = 1.0f;
    [SerializeField] private float rocketTravelTime = 2.8f;
    [SerializeField] private float blastAnimationDuration = 0.8f;
    [SerializeField] private float delayBetweenRockets = 0.3f;
    [SerializeField] private float delayBetweenBlasts = 0.15f;     // small gap between consecutive blast stages
    [SerializeField] private float delayBeforeFullBurst = 0.3f;    // pause before the final full-burst animation
    [SerializeField] private float delayAfterShootUp = 0.5f;       // pause after shootUp before resetting

    internal bool isRocketAnimationComplete = true;
    //internal bool isCrackerAnimationComplete = true;

    internal bool blueCrackerAnimationComplete = true;
    internal bool redCrackerAnimationComplete = true;
    internal bool greenCrackerAnimationComplete = true;

    private int blueCurrentStage = 0;
    private int redCurrentStage = 0;
    private int greenCurrentStage = 0;

    private Coroutine blueLoopCoroutine = null;
    private Coroutine redLoopCoroutine = null;
    private Coroutine greenLoopCoroutine = null;

    [SerializeField] private SocketIOManager socketManager;
    [SerializeField] private AudioController audioController;


    private void Start()
    {
        InitializeCrackerLoops();
    }

    internal void InitializeCrackerLoops()
    {
        blueCurrentStage = 0;
        redCurrentStage = 0;
        greenCurrentStage = 0;

        StopLoopCoroutine(ref blueLoopCoroutine);
        StopLoopCoroutine(ref redLoopCoroutine);
        StopLoopCoroutine(ref greenLoopCoroutine);

        blueLoopCoroutine = StartCoroutine(PlayLoopAnimation(BlueCrackerObject, BlueEmptyLoop));
        redLoopCoroutine = StartCoroutine(PlayLoopAnimation(RedCrackerObject, RedEmptyLoop));
        greenLoopCoroutine = StartCoroutine(PlayLoopAnimation(GreenCrackerObject, GreenEmptyLoop));
    }

    internal void RocketAnimation(List<BonusSymbolData> bonusSymbols)
    {
        isRocketAnimationComplete = false;
        foreach (var bonusSymbol in bonusSymbols)
        {
            StartCoroutine(StartRocketAnimation(bonusSymbol));
        }
        audioController.PlayRocket();
    }

    private IEnumerator StartRocketAnimation(BonusSymbolData bonusSymbol)
    {
        GameObject rocket = InstantiateRocket(bonusSymbol.symbolId);
        Transform pathRoot = GetPathForPosition(bonusSymbol.position[0], bonusSymbol.position[1], bonusSymbol.symbolId);

        RocketMovement rocketMovement = rocket.GetComponent<RocketMovement>();
        rocketMovement.pathRoot = pathRoot;
        rocketMovement.PlayRocket();
        yield return new WaitForSeconds(0.28f);
        rocketMovement.rocket.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.8f);
        rocketMovement.StopRocket();
        RocketHitAnimation(bonusSymbol.symbolId);
        yield return new WaitForSeconds(0.5f);
        yield return new WaitUntil(() => rocketMovement.rocketAnimationFinished);

        BlueRocketHitObject.GetComponent<ImageAnimation>().ResetImageState();
        RedRocketHitObject.GetComponent<ImageAnimation>().ResetImageState();
        GreenRocketHitObject.GetComponent<ImageAnimation>().ResetImageState();
        isRocketAnimationComplete = true;
    }

    internal void CrackerAnimation(List<BonusSymbolData> bonusSymbols)
    {
        //isCrackerAnimationComplete = false;

        int blueValue = 0;
        int redValue = 0;
        int greenValue = 0;

        foreach (var b in bonusSymbols)
        {
            switch (b.symbolId)
            {
                case 11: blueValue += b.value; break;
                case 12: redValue += b.value; break;
                case 13: greenValue += b.value; break;
            }
        }

        bool blueTrigger = false;
        bool redTrigger = false;
        bool greenTrigger = false;

        //if (socketManager?.resultData?.payload?.bonusGame?.features != null)
        {
            var feat = socketManager.resultData.payload.bonusGame.features;
            blueTrigger = feat.ultraSpins;
            redTrigger = feat.doubleReel;
            greenTrigger = feat.extraSpins;
        }

        if (blueValue > 0)
        {
            blueCrackerAnimationComplete = false;
            StartCoroutine(RunCrackerSequence(11, blueValue, blueTrigger));
        }
        if (redValue > 0)
        {
            redCrackerAnimationComplete = false;
            StartCoroutine(RunCrackerSequence(12, redValue, redTrigger));
        }
        if (greenValue > 0)
        {
            greenCrackerAnimationComplete = false;
            StartCoroutine(RunCrackerSequence(13, greenValue, greenTrigger));
        }

    }
    private IEnumerator RunCrackerSequence(int symbolId, int addedValue, bool isTriggerActive)
    {
        int currentStage = GetCurrentStage(symbolId);
        GameObject crackerObj = GetCrackerObject(symbolId);
        GameObject symbolObj = GetSymbolObject(symbolId);

        StopCrackerLoop(symbolId);

        int targetStage;
        if (isTriggerActive)
        {
            targetStage = 8;
        }
        else
        {
            targetStage = Mathf.Min(currentStage + addedValue, 8);
        }

        for (int stage = currentStage + 1; stage <= targetStage; stage++)
        {
            yield return StartCoroutine(PlayOneBlast(crackerObj, symbolId, stage));
            yield return new WaitForSeconds(delayBetweenBlasts);
        }

        // update persisted stage
        SetCurrentStage(symbolId, targetStage);

        if (isTriggerActive)
        {
            yield return new WaitForSeconds(delayBeforeFullBurst);

            yield return StartCoroutine(PlayFullBurst(crackerObj, symbolId));

            yield return StartCoroutine(PlaySymbolShootUp(symbolObj, symbolId));

            if (symbolId == 11)
            {
                blueCrackerAnimationComplete = true;
            }
            if (symbolId == 12)
            {
                redCrackerAnimationComplete = true;
            }
            if (symbolId == 13)
            {
                greenCrackerAnimationComplete = true;
            }

            yield return new WaitForSeconds(delayAfterShootUp);

            SetCurrentStage(symbolId, 0);
            Sprite[] emptyLoop = GetCrackerLoopSprites(symbolId, 0);
            SetCrackerLoopCoroutine(symbolId, StartCoroutine(PlayLoopAnimation(crackerObj, emptyLoop)));

            StartCoroutine(PlaySymbolLoop(symbolObj, symbolId));
        }
        else
        {
            Sprite[] loopSprites = GetCrackerLoopSprites(symbolId, targetStage);
            SetCrackerLoopCoroutine(symbolId, StartCoroutine(PlayLoopAnimation(crackerObj, loopSprites)));
        }
        //isCrackerAnimationComplete = true;
        if (symbolId == 11)
        {
            blueCrackerAnimationComplete = true;
        }
        if (symbolId == 12)
        {
            redCrackerAnimationComplete = true;
        }
        if (symbolId == 13)
        {
            greenCrackerAnimationComplete = true;
        }
    }

    private IEnumerator PlayOneBlast(GameObject crackerObj, int symbolId, int stage)
    {
        ImageAnimation anim = crackerObj.GetComponent<ImageAnimation>();
        Sprite[] blastSprites = GetCrackerBlastSprites(symbolId, stage);

        if (blastSprites == null || blastSprites.Length == 0)
        {
            Debug.LogWarning($"[CrackerAnim] No blast sprites for symbolId={symbolId} stage={stage}");
            yield break;
        }

        anim.StopAnimation();
        anim.textureArray.Clear();
        anim.textureArray.TrimExcess();
        foreach (var s in blastSprites) anim.textureArray.Add(s);
        anim.doLoopAnimation = false;
        anim.AnimationSpeed = 50f;
        anim.ResetImageState();
        anim.IsComplete = false;
        anim.StartAnimation();

        // Wait until this one-shot finishes
        yield return new WaitUntil(() => anim.IsComplete);
    }

    private IEnumerator PlayFullBurst(GameObject crackerObj, int symbolId)
    {
        ImageAnimation anim = crackerObj.GetComponent<ImageAnimation>();
        Sprite[] burstSprites = GetFullBurstSprites(symbolId);

        if (burstSprites == null || burstSprites.Length == 0)
        {
            Debug.LogWarning($"[CrackerAnim] No full-burst sprites for symbolId={symbolId}");
            yield break;
        }

        anim.StopAnimation();
        anim.textureArray.Clear();
        anim.textureArray.TrimExcess();
        foreach (var s in burstSprites) anim.textureArray.Add(s);
        anim.doLoopAnimation = false;
        anim.AnimationSpeed = 15f;
        anim.ResetImageState();
        anim.IsComplete = false;
        anim.StartAnimation();

        yield return new WaitUntil(() => anim.IsComplete);
    }

    private IEnumerator PlaySymbolShootUp(GameObject symbolObj, int symbolId)
    {
        ImageAnimation anim = symbolObj.GetComponent<ImageAnimation>();
        Sprite[] shootSprites = GetSymbolBlastSprites(symbolId);   // these are the ShootUp sprite arrays

        if (shootSprites == null || shootSprites.Length == 0)
        {
            Debug.LogWarning($"[CrackerAnim] No shoot-up sprites for symbolId={symbolId}");
            yield break;
        }

        anim.StopAnimation();
        anim.textureArray.Clear();
        anim.textureArray.TrimExcess();
        foreach (var s in shootSprites) anim.textureArray.Add(s);
        anim.doLoopAnimation = false;
        anim.AnimationSpeed = 10f;
        anim.ResetImageState();
        anim.IsComplete = false;
        anim.StartAnimation();

        yield return new WaitUntil(() => anim.IsComplete);
    }

    private IEnumerator PlaySymbolLoop(GameObject symbolObj, int symbolId)
    {
        ImageAnimation anim = symbolObj.GetComponent<ImageAnimation>();
        Sprite[] loopSprites = GetSymbolLoopSprites(symbolId);

        if (loopSprites == null || loopSprites.Length == 0) yield break;

        anim.StopAnimation();
        anim.textureArray.Clear();
        anim.textureArray.TrimExcess();
        foreach (var s in loopSprites) anim.textureArray.Add(s);
        anim.doLoopAnimation = true;
        anim.AnimationSpeed = 47f;
        anim.ResetImageState();
        anim.IsComplete = false;
        anim.StartAnimation();

        yield return null;
    }

    private IEnumerator PlayLoopAnimation(GameObject crackerObj, Sprite[] loopSprites)
    {
        if (loopSprites == null || loopSprites.Length == 0)
        {
            Debug.LogWarning($"[CrackerAnim] PlayLoopAnimation called with null/empty sprites on {crackerObj.name}");
            yield break;
        }

        ImageAnimation anim = crackerObj.GetComponent<ImageAnimation>();
        anim.StopAnimation();
        anim.textureArray.Clear();
        anim.textureArray.TrimExcess();
        foreach (var s in loopSprites) anim.textureArray.Add(s);
        anim.doLoopAnimation = true;
        anim.AnimationSpeed = 10f;
        anim.ResetImageState();
        anim.IsComplete = false;
        anim.StartAnimation();

        while (true) yield return null;
    }

    private int GetCurrentStage(int symbolId)
    {
        switch (symbolId)
        {
            case 11: return blueCurrentStage;
            case 12: return redCurrentStage;
            case 13: return greenCurrentStage;
            default: return 0;
        }
    }

    private void SetCurrentStage(int symbolId, int value)
    {
        switch (symbolId)
        {
            case 11: blueCurrentStage = value; break;
            case 12: redCurrentStage = value; break;
            case 13: greenCurrentStage = value; break;
        }
    }

    private GameObject GetCrackerObject(int symbolId)
    {
        switch (symbolId)
        {
            case 11: return BlueCrackerObject;
            case 12: return RedCrackerObject;
            case 13: return GreenCrackerObject;
            default: return null;
        }
    }

    private GameObject GetSymbolObject(int symbolId)
    {
        switch (symbolId)
        {
            case 11: return BlueSymbolObject;
            case 12: return RedSymbolObject;
            case 13: return GreenSymbolObject;
            default: return null;
        }
    }

    private void StopCrackerLoop(int symbolId)
    {
        switch (symbolId)
        {
            case 11: StopLoopCoroutine(ref blueLoopCoroutine); break;
            case 12: StopLoopCoroutine(ref redLoopCoroutine); break;
            case 13: StopLoopCoroutine(ref greenLoopCoroutine); break;
        }
    }

    private void SetCrackerLoopCoroutine(int symbolId, Coroutine co)
    {
        switch (symbolId)
        {
            case 11: blueLoopCoroutine = co; break;
            case 12: redLoopCoroutine = co; break;
            case 13: greenLoopCoroutine = co; break;
        }
    }

    private void StopLoopCoroutine(ref Coroutine co)
    {
        if (co != null)
        {
            StopCoroutine(co);
            co = null;
        }
    }

    private Sprite[] GetFullBurstSprites(int symbolId)
    {
        switch (symbolId)
        {
            case 11: return BlueCrackerBlast;
            case 12: return RedCrackerBlast;
            case 13: return GreenCrackerBlast;
            default: return null;
        }
    }

    private Sprite[] GetSymbolLoopSprites(int symbolId)
    {
        switch (symbolId)
        {
            case 11: return BlueSymbolLoopSprites;
            case 12: return RedSymbolLoopSprites;
            case 13: return GreenSymbolLoopSprites;
            default: return null;
        }
    }

    private Sprite[] GetSymbolBlastSprites(int symbolId)
    {
        switch (symbolId)
        {
            case 11: return BlueSymbolShootUpSprites;
            case 12: return RedSymbolShootUpSprites;
            case 13: return GreenSymbolShootUpSprites;
            default: return null;
        }
    }

    private Sprite[] GetCrackerLoopSprites(int symbolId, int stage)
    {
        switch (symbolId)
        {
            case 11: // Blue
                switch (stage)
                {
                    case 0: return BlueEmptyLoop;
                    case 1: return BlueOneLoop;
                    case 2: return BlueTwoLoop;
                    case 3: return BlueThreeLoop;
                    case 4: return BlueFourLoop;
                    case 5: return BlueFiveLoop;
                    case 6: return BlueSixLoop;
                    case 7: return BlueSevenLoop;
                    case 8: return BlueEightLoop;
                }
                break;
            case 12: // Red
                switch (stage)
                {
                    case 0: return RedEmptyLoop;
                    case 1: return RedOneLoop;
                    case 2: return RedTwoLoop;
                    case 3: return RedThreeLoop;
                    case 4: return RedFourLoop;
                    case 5: return RedFiveLoop;
                    case 6: return RedSixLoop;
                    case 7: return RedSevenLoop;
                    case 8: return RedEightLoop;
                }
                break;
            case 13: // Green
                switch (stage)
                {
                    case 0: return GreenEmptyLoop;
                    case 1: return GreenOneLoop;
                    case 2: return GreenTwoLoop;
                    case 3: return GreenThreeLoop;
                    case 4: return GreenFourLoop;
                    case 5: return GreenFiveLoop;
                    case 6: return GreenSixLoop;
                    case 7: return GreenSevenLoop;
                    case 8: return GreenEightLoop;
                }
                break;
        }
        return null;
    }

    private Sprite[] GetCrackerBlastSprites(int symbolId, int stage)
    {
        switch (symbolId)
        {
            case 11: // Blue
                switch (stage)
                {
                    case 1: return BlueOneBlast;
                    case 2: return BlueTwoBlast;
                    case 3: return BlueThreeBlast;
                    case 4: return BlueFourBlast;
                    case 5: return BlueFiveBlast;
                    case 6: return BlueSixBlast;
                    case 7: return BlueSevenBlast;
                    case 8: return BlueEightBlast;
                }
                break;
            case 12: // Red
                switch (stage)
                {
                    case 1: return RedOneBlast;
                    case 2: return RedTwoBlast;
                    case 3: return RedThreeBlast;
                    case 4: return RedFourBlast;
                    case 5: return RedFiveBlast;
                    case 6: return RedSixBlast;
                    case 7: return RedSevenBlast;
                    case 8: return RedEightBlast;
                }
                break;
            case 13: // Green
                switch (stage)
                {
                    case 1: return GreenOneBlast;
                    case 2: return GreenTwoBlast;
                    case 3: return GreenThreeBlast;
                    case 4: return GreenFourBlast;
                    case 5: return GreenFiveBlast;
                    case 6: return GreenSixBlast;
                    case 7: return GreenSevenBlast;
                    case 8: return GreenEightBlast;
                }
                break;
        }
        return null;
    }

    private GameObject InstantiateRocket(int symbolId)
    {
        GameObject rocketPrefab = null;
        switch (symbolId)
        {
            case 11: rocketPrefab = BlueRocket; break;
            case 12: rocketPrefab = RedRocket; break;
            case 13: rocketPrefab = GreenRocket; break;
        }

        if (rocketPrefab == null)
        {
            Debug.LogError($"No rocket prefab assigned for symbol ID: {symbolId}");
            return null;
        }

        return Instantiate(rocketPrefab, RocketParent.transform);
    }

    private Transform GetPathForPosition(int col, int row, int symbolId)
    {
        ReelImage reelImage = rocketPaths[row].reel[col];

        switch (symbolId)
        {
            case 11:
                if (reelImage.bluePositions.positions.Count > 0)
                    return reelImage.bluePositions.positions[0].GetComponentInParent<RectTransform>();
                break;
            case 12:
                if (reelImage.redPositions.positions.Count > 0)
                    return reelImage.redPositions.positions[0].GetComponentInParent<RectTransform>();
                break;
            case 13:
                if (reelImage.greenpositions.positions.Count > 0)
                    return reelImage.greenpositions.positions[0].GetComponentInParent<RectTransform>();
                break;
        }

        return null;
    }

    private void RocketHitAnimation(int symbolId)
    {
        GameObject hitObj = null;
        switch (symbolId)
        {
            case 11: hitObj = BlueRocketHitObject; break;
            case 12: hitObj = RedRocketHitObject; break;
            case 13: hitObj = GreenRocketHitObject; break;
        }
        if (hitObj == null) return;

        ImageAnimation img = hitObj.GetComponent<ImageAnimation>();
        if (img.ImageAnimationPlaying()) return;

        audioController.PlayRocketBlast();
        img.AnimationSpeed = 17f;
        img.ResetImageState();
        img.StartAnimation();
    }
}

[Serializable]
public class Reel
{
    public List<ReelImage> reel = new List<ReelImage>(3);
}

[Serializable]
public class ReelImage
{
    public Blue bluePositions;
    public Red redPositions;
    public Green greenpositions;
}

[Serializable]
public class Blue
{
    public List<GameObject> positions = new List<GameObject>();
}

[Serializable]
public class Red
{
    public List<GameObject> positions = new List<GameObject>();
}

[Serializable]
public class Green
{
    public List<GameObject> positions = new List<GameObject>();
}

[Serializable]
public class BonusSymbolData
{
    public int[] position;  // [col, row]
    public int symbolId;    // 11=Blue, 12=Red, 13=Green
    public int value;       // blast value from backend
}