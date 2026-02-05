using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class OrientationChange : MonoBehaviour
{
  [SerializeField] private RectTransform UIWrapper;
  [SerializeField] private CanvasScaler CanvasScaler;
  [SerializeField] private float MatchWidth = 0f;
  [SerializeField] private float MatchHeight = 1f;
  [SerializeField] private float transitionDuration = 0.2f;
  [SerializeField] private float waitForResize = 0.2f;

  private Vector2 ReferenceAspect;
  private Tween matchTween;
  private Coroutine resizeRoutine;

  private void Awake()
  {
    ReferenceAspect = CanvasScaler.referenceResolution;
  }

  void SwitchDisplay(string dimensions)
  {
    if (resizeRoutine != null) StopCoroutine(resizeRoutine);
    resizeRoutine = StartCoroutine(ResizeCoroutine(dimensions));
  }

  IEnumerator ResizeCoroutine(string dimensions)
  {
    yield return new WaitForSecondsRealtime(waitForResize);
    string[] parts = dimensions.Split(',');
    if (parts.Length == 2 && int.TryParse(parts[0], out int width) && int.TryParse(parts[1], out int height) && width > 0 && height > 0)
    {
      Debug.LogWarning($"Unity: Received Dimensions - Width: {width}, Height: {height}");

      float currentAspectRatio = (float)height / width; // Portrait mode: height/width
      float referenceAspectRatio = ReferenceAspect.y / ReferenceAspect.x; // Portrait reference
      Debug.LogWarning("Current Aspect Ratio: " + currentAspectRatio);
      
      float targetMatch;

      // Determine match value based on aspect ratio
      // Higher values = more height matching, lower values = more width matching
      if (currentAspectRatio < 1.3f)
        targetMatch = MatchWidth; // Very wide screens (almost square)
      else if (currentAspectRatio >= 1.3f && currentAspectRatio < 1.4f)
        targetMatch = 1f;   // ~1.33 (iPad Pro 1024x1366)
      else if (currentAspectRatio >= 1.4f && currentAspectRatio < 1.5f)
        targetMatch = 0.65f;   // ~1.4
      else if (currentAspectRatio >= 1.5f && currentAspectRatio < 1.6f)
        targetMatch = 0.7f;   // ~1.5
      else if (currentAspectRatio >= 1.6f && currentAspectRatio < 1.85f)
        targetMatch = 0.75f;   // ~1.6-1.85 range
      else if (currentAspectRatio >= 1.85 && currentAspectRatio < 2.4)
        targetMatch = 0.85f;    // ~1.85-2.4 range (taller phones)
      else
        targetMatch = MatchHeight; // Very tall screens (2.4+)

      if (matchTween != null && matchTween.IsActive()) matchTween.Kill();
      matchTween = DOTween.To(() => CanvasScaler.matchWidthOrHeight, x => CanvasScaler.matchWidthOrHeight = x, targetMatch, transitionDuration).SetEase(Ease.InOutQuad);

      Debug.LogWarning($"matchWidthOrHeight set to: {targetMatch}");
    }
    else
    {
      Debug.LogWarning("Unity: Invalid format received in SwitchDisplay");
    }
  }

#if UNITY_EDITOR
  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.Space))
    {
      SwitchDisplay(Screen.width + "," + Screen.height);
    }
  }
#endif
}