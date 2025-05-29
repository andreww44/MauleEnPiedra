using UnityEngine;
using UnityEngine.EventSystems;

public class CartaSlot : MonoBehaviour, IDropHandler
{

    //sprivate GameObject card;
    [SerializeField] private SO_Cards cardData;

    public void OnDrop(PointerEventData eventData)
    {
        if(transform.childCount > 0)
        {
            return;
        }

        GameObject dropped = eventData.pointerDrag;

        if (dropped != null)
        {
            CartaArrastre cartaArrastre = dropped.GetComponent<CartaArrastre>();

            if (cartaArrastre != null)
            {
                cartaArrastre.parentToReturnTo = this.transform;

                dropped.transform.SetParent(this.transform);
                dropped.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

            }
        }
    }

    public void Update()
    {
        if (transform.childCount == 0)
        {
            RetireCard();
        }
    }

    public void setSoCard(SO_Cards card)
    {
        cardData = card;
    }

    public void RetireCard()
    {
        cardData = null;
    }

    
}
