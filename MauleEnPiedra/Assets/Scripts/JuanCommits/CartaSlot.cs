using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class CartaSlot : MonoBehaviour, IDropHandler
{
    [SerializeField] private SO_Cards cardData;
    private Image imgSlot;
    private Button myButton;
    [Tooltip("Referencia a la imagen hija donde se mostrará el sprite")]
    [SerializeField] private Image imageChild;

    void Awake()
    {
        imgSlot = GetComponent<Image>();
        myButton = GetComponent<Button>();
        UpdateVisual();
    }

    public void setSoCard(SO_Cards soCard)
    {
        cardData = soCard;
        UpdateVisual();
    }

    public void OnDrop(PointerEventData eventData)
    {
        var dragged = eventData.pointerDrag;
        if (dragged == null) return;

        var cartaArrastre = dragged.GetComponent<CartaArrastre>();
        if (cartaArrastre == null) return;

        var repr = dragged.GetComponent<UI_CardRepresentation>();
        if (repr == null) return;

        SO_Cards soCard = repr.CardData;

        bool vinoDeMano = cartaArrastre.parentToReturnTo.CompareTag("HandArea");
        bool vinoDeMesa = cartaArrastre.parentToReturnTo.CompareTag("GroupArea");

        if (vinoDeMano)
        {
            SCR_Player player = SCR_Table.Instance.Player;
            int idx = player.HandCards.IndexOf(soCard);
            if (idx >= 0)
                player.ClickHand(idx, Turn.Player);
        }

        cartaArrastre.parentToReturnTo = this.transform;
        dragged.transform.SetParent(this.transform, false);
    }

    private void UpdateVisual()
    {
        if (cardData != null)
        {
            if (imageChild != null)
            {
                imageChild.sprite = cardData.image;
                imageChild.enabled = true;
            }
            imgSlot.enabled = true;
            myButton.interactable = true;
        }
        else
        {
            if (imageChild != null) imageChild.enabled = false;
            imgSlot.enabled = false;
            myButton.interactable = false;
        }
    }
    public void ClearSlot()
    {
        cardData = null;
        UpdateVisual();
    }
}
