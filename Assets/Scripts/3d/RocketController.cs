using System.Collections;
using UnityEngine;

public class RocketController : MonoBehaviour
{
    [SerializeField] private Transform StartPos;
    [SerializeField] private Transform EndPos;
    [SerializeField] private Camera UICamera;

    void Start()
    {
        // Convert UI positions to world space
        Vector3 startWorld = ConvertUIToWorldPoint(StartPos);
        Vector3 endWorld = ConvertUIToWorldPoint(EndPos);

        MoveRocket(StartPos.position, EndPos.position, true, 1.5f); // Use reasonable curve distance
    }

    private Vector3 ConvertUIToWorldPoint(Transform uiTransform)
    {
        Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(UICamera, uiTransform.position);
        Ray ray = UICamera.ScreenPointToRay(screenPos);
        Plane plane = new Plane(Vector3.forward, Vector3.zero); // XY plane

        if (plane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }

        return Vector3.zero;
    }

    public void MoveRocket(Vector3 startPos, Vector3 endPos, bool left, float curveDistance)
    {
        StartCoroutine(MoveAlongCurve(startPos, endPos, left, curveDistance));
    }

    private IEnumerator MoveAlongCurve(Vector3 start, Vector3 end, bool left, float curveDistance)
    {
        float duration = 2f;
        float elapsed = 0f;

        Vector3 direction = end - start;
        Vector3 midPoint = start + direction / 2f;

        // ✅ Corrected for 2D: use Vector3.forward instead of Vector3.up
        Vector3 perpendicular = Vector3.Cross(direction.normalized, Vector3.forward) * (left ? 1 : -1);
        Vector3 controlPoint = midPoint + perpendicular * curveDistance;

        Debug.Log("Start: " + start);
        Debug.Log("Control: " + controlPoint);
        Debug.Log("End: " + end);

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            Vector3 position = Mathf.Pow(1 - t, 2) * start +
                               2 * (1 - t) * t * controlPoint +
                               Mathf.Pow(t, 2) * end;

            transform.position = position;

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = end;
    }
}
