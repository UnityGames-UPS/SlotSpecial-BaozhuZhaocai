using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System;

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
    [SerializeField] private GameObject BlueRocket;       // rocket prefab
    [SerializeField] private Sprite[] BlueRocketAnimation;

    [SerializeField] private GameObject RedRocket;       // rocket prefab
    [SerializeField] private Sprite[] RedRocketAnimation;

    [SerializeField] private GameObject GreenRocket;       // rocket prefab
    [SerializeField] private Sprite[] GreenRocketAnimation;

    [Header("Symbol Animation Sprites")]
    [SerializeField] private Sprite[] BlueSymbolLoopSprites;
    [SerializeField] private Sprite[] RedSymbolLoopSprites;
    [SerializeField] private Sprite[] GreenSymbolLoopSprites;
    [SerializeField] private Sprite[] BlueSymbolBlastSprites;
    [SerializeField] private Sprite[] RedSymbolBlastSprites;
    [SerializeField] private Sprite[] GreenSymbolBlastSprites;

    [Header("Blast Animations")]
    [SerializeField] private Sprite[] BlueCrackerBlast;
    [SerializeField] private Sprite[] RedCrackerBlast;
    [SerializeField] private Sprite[] GreenCrackerBlast;

    [Header("Blue Crackers")]
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

    // Internal tracking
    private List<GameObject> activeRockets = new List<GameObject>();
    private List<Coroutine> activeAnimations = new List<Coroutine>();
    internal bool isRocketAnimationComplete = true;


    private void Start()
    {

    }


    internal void RocketAnimation(List<BonusSymbolData> bonusSymbols)
    {
        isRocketAnimationComplete = false;
        foreach (var bonusSymbol in bonusSymbols)
        {
            //GameObject rocket = InstantiateRocket(bonusSymbol.symbolId);
            //Debug.Log("rocket Animation Started");
            StartCoroutine(StartRocketAnimation(bonusSymbol));
        }
    }

    private IEnumerator StartRocketAnimation(BonusSymbolData bonusSymbol)
    {
        //foreach (var bonusSymbol in bonusSymbols)
        //Debug.Log("rocket Instantiate started");
        GameObject rocket = InstantiateRocket(bonusSymbol.symbolId);
        //Debug.Log("rocket Instantiate Finished");
        //Debug.Log("rocket path started");
        Transform pathRoot = GetPathForPosition(bonusSymbol.position[0], bonusSymbol.position[1], bonusSymbol.symbolId);
        //Debug.Log("rocket path finished");
        //Debug.Log("rocket animation started");
        RocketMovement rocketMovement = rocket.GetComponent<RocketMovement>();
        rocketMovement.pathRoot = pathRoot;
        rocketMovement.PlayRocket();
        yield return new WaitForSeconds(0.28f);
        rocketMovement.rocket.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        rocketMovement.StopRocket();
        RocketHitAnimation(bonusSymbol.symbolId);
        //Debug.Log("rocket animation finished");
        yield return new WaitUntil(() => rocketMovement.rocketAnimationFinished);
        isRocketAnimationComplete = true;
        BlueRocketHitObject.GetComponent<ImageAnimation>().ResetImageState();
        RedRocketHitObject.GetComponent<ImageAnimation>().ResetImageState();
        GreenRocketHitObject.GetComponent<ImageAnimation>().ResetImageState();
    }

    private GameObject InstantiateRocket(int symbolId) //,Vector3 position)
    {
        GameObject rocketPrefab = null;
        //Debug.Log(symbolId);
        switch (symbolId)
        {
            case 11: // Blue
                rocketPrefab = BlueRocket;
                break;
            case 12: // Red
                rocketPrefab = RedRocket;
                break;
            case 13: // Green
                rocketPrefab = GreenRocket;
                break;
        }

        if (rocketPrefab == null)
        {
            Debug.LogError($"No rocket prefab assigned for symbol ID: {symbolId}");
            return null;
        }

        GameObject rocket = Instantiate(rocketPrefab, RocketParent.transform);
        //rocket.transform.position = position;
        return rocket;
    }

    private void PopulateCrackerLoopAnimation(ImageAnimation animScript, int symbolId, int value)
    {
        animScript.textureArray.Clear();
        animScript.doLoopAnimation = true;
        animScript.AnimationSpeed = 10f;

        Sprite[] loopSprites = GetCrackerLoopSprites(symbolId, value);
        if (loopSprites != null)
        {
            foreach (var sprite in loopSprites)
            {
                animScript.textureArray.Add(sprite);
            }
        }
    }

    /// <summary>
    /// Populate cracker blast animation based on value
    /// </summary>
    private void PopulateCrackerBlastAnimation(ImageAnimation animScript, int symbolId, int value)
    {
        animScript.textureArray.Clear();
        animScript.doLoopAnimation = false;
        animScript.AnimationSpeed = 15f;

        Sprite[] blastSprites = GetCrackerBlastSprites(symbolId, value);
        if (blastSprites != null)
        {
            foreach (var sprite in blastSprites)
            {
                animScript.textureArray.Add(sprite);
            }
        }
    }

    /// <summary>
    /// Get the path for a specific reel position and symbol
    /// </summary>
    private Transform GetPathForPosition(int col, int row, int symbolId)
    {
        //if (col >= rocketPaths.Count) return null;
        //if (row >= rocketPaths[col].reel.Count) return null;

        ReelImage reelImage = rocketPaths[row].reel[col];

        switch (symbolId)
        {
            case 11: // Blue
                if (reelImage.bluePositions.positions.Count > 0)
                    return reelImage.bluePositions.positions[0].GetComponentInParent<RectTransform>();
                break;
            case 12: // Red
                if (reelImage.redPositions.positions.Count > 0)
                    return reelImage.redPositions.positions[0].GetComponentInParent<RectTransform>();
                break;
            case 13: // Green
                if (reelImage.greenpositions.positions.Count > 0)
                    return reelImage.greenpositions.positions[0].GetComponentInParent<RectTransform>();
                break;
        }

        return null;
    }

    /// <summary>
    /// Get symbol loop sprites based on symbol ID
    /// </summary>
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

    /// <summary>
    /// Get symbol blast sprites based on symbol ID
    /// </summary>
    private Sprite[] GetSymbolBlastSprites(int symbolId)
    {
        switch (symbolId)
        {
            case 11: return BlueSymbolBlastSprites;
            case 12: return RedSymbolBlastSprites;
            case 13: return GreenSymbolBlastSprites;
            default: return null;
        }
    }

    /// <summary>
    /// Get cracker loop sprites based on symbol and value
    /// </summary>
    private Sprite[] GetCrackerLoopSprites(int symbolId, int value)
    {
        switch (symbolId)
        {
            case 11: // Blue
                switch (value)
                {
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
                switch (value)
                {
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
                switch (value)
                {
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

    /// <summary>
    /// Get cracker blast sprites based on symbol and value
    /// </summary>
    private Sprite[] GetCrackerBlastSprites(int symbolId, int value)
    {
        switch (symbolId)
        {
            case 11: // Blue
                switch (value)
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
                switch (value)
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
                switch (value)
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

    private void RocketHitAnimation(int symbolId)
    {

        switch (symbolId)
        {
            case 11:
                ImageAnimation blueImage = BlueRocketHitObject.GetComponent<ImageAnimation>();
                if (blueImage.ImageAnimationPlaying())
                {
                    break;
                }
                else
                {
                    blueImage.ResetImageState();
                    blueImage.StartAnimation();
                    break;
                }
            case 12:
                ImageAnimation redImage = RedRocketHitObject.GetComponent<ImageAnimation>();
                if (redImage.ImageAnimationPlaying())
                {
                    break;
                }
                else
                {
                    redImage.ResetImageState();
                    redImage.StartAnimation();
                    break;
                }
            case 13:
                ImageAnimation greenImage = GreenRocketHitObject.GetComponent<ImageAnimation>();
                if (greenImage.ImageAnimationPlaying())
                {
                    break;
                }
                else
                {
                    greenImage.ResetImageState();
                    greenImage.StartAnimation();
                    break;
                }
        }
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
    public int[] position; // [col, row]
    public int symbolId;   // 11=Blue, 12=Red, 13=Green
    public int value;      // 1-8
}