using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class RocketMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] internal RectTransform rocket;
    [SerializeField] internal Transform pathRoot; // parent containing path points

    [Header("Movement")]
    [SerializeField] private float travelTime = 2.8f;
    [SerializeField] private Ease movementEase = Ease.InOutSine;
    [SerializeField] private float rotationLerp = 12f;

    [Header("Visual Polish")]
    private float scalePunch = 1.02f;
    [SerializeField] private float wobbleStrength = 3f;

    internal bool rocketAnimationFinished = true;
    private Vector3[] pathPoints;
    private Vector3 lastPosition;
    private Tweener pathTween;
    private Tweener scaleTween;

    private void Awake()
    {
        //CachePathPoints();
    }

    private void CachePathPoints()
    {
        if (pathRoot == null)
        {
            Debug.LogError("RocketMovement: Path Root is not assigned!");
            return;
        }

        List<Vector3> points = new List<Vector3>();
        foreach (Transform t in pathRoot)
        {
            points.Add(t.position);
        }

        pathPoints = points.ToArray();
        
        if (pathPoints.Length < 2)
        {
            Debug.LogError("RocketMovement: Need at least 2 path points!");
        }
    }

    /// <summary>
    /// Play rocket animation from current position through path
    /// </summary>
    internal void PlayRocket()
    {
        CachePathPoints();
        if (pathPoints == null || pathPoints.Length < 2)
        {
            Debug.LogError("RocketMovement: Path points not set up correctly!");
            return;
        }

        rocketAnimationFinished = false;
        rocket.position = pathPoints[0];
        lastPosition = rocket.position;
        UpdateRotation();
        rocket.gameObject.SetActive(false);
        // rocket.localScale = Vector3.one * 0.7f;
        
        // rocket.DOScale(Vector3.one,0.5f);

        // Kill any existing tweens
        if (pathTween != null && pathTween.IsActive())
        {
            pathTween.Kill();
        }
        if (scaleTween != null && scaleTween.IsActive())
        {
            scaleTween.Kill();
        }
        ImageAnimation rocketAnimation = rocket.GetComponent<ImageAnimation>();
        rocketAnimation.ResetImageState();
        rocketAnimation.StartAnimation();

        // Subtle scale pulse (BaoZhuZhaocai feel)
        scaleTween = rocket.DOPunchScale(Vector3.one * scalePunch, 2f, 1);

        // Path movement
        pathTween = rocket.DOPath(pathPoints, travelTime, PathType.CatmullRom, PathMode.Full3D)
            .SetEase(movementEase)
            .OnUpdate(UpdateRotation)
            .OnComplete(OnRocketEnd);
    }

    /// <summary>
    /// Play rocket from a specific start position
    /// </summary>
    public void PlayRocketFromPosition(Vector3 startPosition)
    {
        if (pathPoints == null || pathPoints.Length < 2)
        {
            Debug.LogError("RocketMovement: Path points not set up correctly!");
            return;
        }

        rocket.gameObject.SetActive(true);
        rocket.position = startPosition;
        rocket.localScale = Vector3.one;

        lastPosition = rocket.position;

        // Kill any existing tweens
        if (pathTween != null && pathTween.IsActive())
        {
            pathTween.Kill();
        }
        if (scaleTween != null && scaleTween.IsActive())
        {
            scaleTween.Kill();
        }

        // Create path starting from custom position
        Vector3[] customPath = new Vector3[pathPoints.Length + 1];
        customPath[0] = startPosition;
        for (int i = 0; i < pathPoints.Length; i++)
        {
            customPath[i + 1] = pathPoints[i];
        }

        // Subtle scale pulse
        scaleTween = rocket.DOPunchScale(Vector3.one * scalePunch, 0.6f, 1)
            .SetLoops(-1, LoopType.Restart);

        // Path movement
        pathTween = rocket.DOPath(customPath, travelTime, PathType.CatmullRom, PathMode.Full3D)
            .SetEase(movementEase)
            .OnUpdate(UpdateRotation)
            .OnComplete(OnRocketEnd);
    }

    private void UpdateRotation()
    {
        Vector3 currentPosition = rocket.position;
        Vector3 velocity = (currentPosition - lastPosition) / Time.deltaTime;
        lastPosition = currentPosition;

        if (velocity.sqrMagnitude < 0.001f)
            return;

        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        Quaternion targetRot = Quaternion.Euler(0, 0, angle - 90f);

        rocket.rotation = Quaternion.Lerp(rocket.rotation, targetRot, Time.deltaTime * rotationLerp);
    }

    private void OnRocketEnd()
    {
        pathTween?.Kill();
        scaleTween?.Kill();
        rocket.DOKill();
        rocket.gameObject.SetActive(false);
        rocketAnimationFinished = true;
    }
    
    internal void StopRocket()
    {
        if (pathTween != null && pathTween.IsActive())
        {
            pathTween.Kill();
        }
        if (scaleTween != null && scaleTween.IsActive())
        {
            scaleTween.Kill();
        }
        rocket.DOKill();
        rocketAnimationFinished = true;
        rocket.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        StopRocket();
    }
}