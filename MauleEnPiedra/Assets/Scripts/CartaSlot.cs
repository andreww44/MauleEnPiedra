using UnityEngine;
using UnityEngine.EventSystems;

public class CartaSlot : MonoBehaviour, IDropHandler
{
    public bool ocupado = false;

    public void OnDrop(PointerEventData eventData)
    {
        if (ocupado) return;

        GameObject dropped = eventData.pointerDrag;

        if (dropped != null)
        {
            CartaArrastre cartaArrastre = dropped.GetComponent<CartaArrastre>();

            if (cartaArrastre != null)
            {
                cartaArrastre.parentToReturnTo = this.transform;

                dropped.transform.SetParent(this.transform);
                dropped.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

                ocupado = true;
            }
        }
    }
}
