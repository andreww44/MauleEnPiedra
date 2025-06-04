using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UI_CardRepresentation : MonoBehaviour
{
    [HideInInspector] public SO_Cards CardData;
    private Image _img;

    private void Awake()
    {
        _img = GetComponent<Image>();
    }

    private void Start()
    {
        if (CardData != null)
            _img.sprite = CardData.image;
        else
            _img.enabled = false;
    }

    public void SetCard(SO_Cards data)
    {
        CardData = data;
        if (_img == null) _img = GetComponent<Image>();
        _img.sprite = CardData.image;
        _img.enabled = true;
    }
}
