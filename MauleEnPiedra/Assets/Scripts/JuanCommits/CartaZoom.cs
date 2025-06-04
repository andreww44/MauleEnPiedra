using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public class CartaZoom : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public SO_Cards cardData;
    public GameObject previewUI;
    public float previewScale = 1.2f;

    private Image _previewImage;
    private Vector2 _originalSize;

    private void Awake()
    {
        if (previewUI != null)
        {
            _previewImage = previewUI.GetComponent<Image>();
            if (_previewImage != null)
            {
                _originalSize = _previewImage.rectTransform.sizeDelta;
                previewUI.SetActive(false);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Image currentImage = GetComponent<Image>();
        if (currentImage == null || cardData == null || previewUI == null) return;
        if (currentImage.sprite != cardData.image) return;

        if (_previewImage == null)
        {
            _previewImage = previewUI.GetComponent<Image>();
            if (_previewImage == null) return;
            _originalSize = _previewImage.rectTransform.sizeDelta;
        }

        previewUI.SetActive(true);
        _previewImage.sprite = cardData.image;
        _previewImage.preserveAspect = true;
        _previewImage.rectTransform.sizeDelta = _originalSize * previewScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_previewImage != null)
        {
            _previewImage.sprite = null;
            _previewImage.rectTransform.sizeDelta = _originalSize;
        }
        if (previewUI != null)
            previewUI.SetActive(false);
    }
}
