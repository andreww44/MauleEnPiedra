using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class CartaZoom : MonoBehaviour, IPointerClickHandler
{
    private RectTransform rectTransform;
    private Vector3 originalScale;
    private Vector3 zoomScale = new Vector3(2f, 2f, 2f);
    private float zoomDuration = 0.3f;
    private bool isZoomed = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isZoomed)
            StartCoroutine(ZoomTo(originalScale));
        else
            StartCoroutine(ZoomTo(zoomScale));

        isZoomed = !isZoomed;
    }

    IEnumerator ZoomTo(Vector3 targetScale)
    {
        float time = 0f;
        Vector3 startScale = rectTransform.localScale;

        while (time < zoomDuration)
        {
            rectTransform.localScale = Vector3.Lerp(startScale, targetScale, time / zoomDuration);
            time += Time.unscaledDeltaTime;
            yield return null;
        }

        rectTransform.localScale = targetScale;
    }
}
