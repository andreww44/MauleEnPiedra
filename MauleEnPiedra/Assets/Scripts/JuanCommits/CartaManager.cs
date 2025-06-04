using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum CardZone
{
    Hand,
    Group,
    Special,
    Maze,
    HoleMaze,
}

public class CartaManager : MonoBehaviour
{
    public GameObject cartaPrefab;
    public Transform[] slots;
    public Transform[] reparto;
    public Transform[] holemaze;
    public Transform[] slotsPetro;
    public Transform[] slotsSpecial;
    public GameObject previewUI;
    public float tiempoEntreCartas = 0.4f;
    public float duracionMovimiento = 0.4f;

    public IEnumerator MoveCard(int fromIndex, int toIndex, SO_Cards card, CardZone fromZone, CardZone toZone, bool isShowImage)
    {
        Transform[] fromSlots = GetSlotsByZone(fromZone);
        Transform[] toSlots = GetSlotsByZone(toZone);

        if (fromSlots[fromIndex].childCount > 0)
        {
            Destroy(fromSlots[fromIndex].GetChild(0).gameObject);
        }

        GameObject cartaGO = Instantiate(cartaPrefab, fromSlots[fromIndex]);
        RectTransform cartaRT = cartaGO.GetComponent<RectTransform>();
        cartaRT.localPosition = Vector3.zero;

        CartaZoom cz = cartaGO.GetComponentInChildren<CartaZoom>();
        if (cz != null)
        {
            cz.cardData = card;
            cz.previewUI = previewUI;
        }

        yield return new WaitForSeconds(tiempoEntreCartas);

        Transform destino = toSlots[toIndex];
        yield return StartCoroutine(Suavizado(cartaRT, destino));

        CartaSlot slotScript = destino.GetComponent<CartaSlot>();
        if (slotScript != null)
        {
            slotScript.setSoCard(card);
        }

        yield return new WaitForSeconds(tiempoEntreCartas);

        Image imgComponent = cartaGO.GetComponentInChildren<Image>();
        if (imgComponent != null && isShowImage)
        {
            imgComponent.sprite = card.image;
            imgComponent.preserveAspect = true;
        }

        if (fromSlots[fromIndex].childCount > 0)
        {
            Destroy(fromSlots[fromIndex].GetChild(0).gameObject);
        }
    }

    private Transform[] GetSlotsByZone(CardZone zone)
    {
        switch (zone)
        {
            case CardZone.Hand: return slots;
            case CardZone.Group: return slotsPetro;
            case CardZone.Special: return slotsSpecial;
            case CardZone.Maze: return reparto;
            case CardZone.HoleMaze: return holemaze;
            default: return null;
        }
    }

    private IEnumerator Suavizado(RectTransform carta, Transform destino)
    {
        Vector3 inicio = carta.position;
        Vector3 final = destino.position;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duracionMovimiento;
            carta.position = Vector3.Lerp(inicio, final, t);
            yield return null;
        }

        carta.position = final;
        carta.SetParent(destino, worldPositionStays: false);
        carta.localPosition = Vector3.zero;
    }
}
