using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum CardZone
{
    Hand,
    Group,
    Special,
    Maze,
    HoleMaze,
    ToG
}

public class CartaManager : MonoBehaviour
{
    public GameObject cartaPrefab;
    
    public Transform[] slots;
    public Transform[] slotsPetro;
    public Transform[] slotsSpecial;
    public Transform[] slotsToG;

    public Transform[] Aislots;
    public Transform[] AislotsPetro;
    public Transform[] AislotsSpecial;
    public Transform[] AislotsToG;

    public Transform[] reparto;
    public Transform[] holemaze;

    public float tiempoEntreCartas = 0.1f;
    public float duracionMovimiento = 0.1f;

    private void Awake()
    {
    }
    void Start()
    {
    }

    

   
    public IEnumerator MoveCard(int fromIndex, int toIndex, SO_Cards card, CardZone fromZone, CardZone toZone, bool isShowImage, Turn turn)
    {
        Transform[] fromSlots = GetSlotsByZone(fromZone, turn);
        Transform[] toSlots = GetSlotsByZone(toZone, turn);

        if (fromSlots[fromIndex].childCount > 0)
        {
            Destroy(fromSlots[fromIndex].GetChild(0).gameObject);
        }

        GameObject cartaGO = Instantiate(cartaPrefab, fromSlots[fromIndex]);
        RectTransform cartaRT = cartaGO.GetComponent<RectTransform>();
        cartaRT.localPosition = Vector3.zero;

        yield return new WaitForSeconds(tiempoEntreCartas);

        Transform destino = toSlots[toIndex];
        yield return StartCoroutine(Suavizado(cartaRT, destino));
        
        destino.GetComponent<CartaSlot>().setSoCard(card);
        yield return new WaitForSeconds(tiempoEntreCartas);

        if (cartaGO.transform.childCount > 0)
        {
            if (isShowImage)
            {
                cartaGO.transform.GetChild(0).GetComponent<Image>().sprite = card.image;
            }
            
        }

        if (fromSlots[fromIndex].childCount > 0)
        {
            
            Destroy(fromSlots[fromIndex].GetChild(0).gameObject);
        }
    }

    private Transform[] GetSlotsByZone(CardZone zone, Turn turn)
    {
        Debug.Log(turn);
        switch (turn)
        {
            case Turn.Player:
                switch (zone)
                {
                    case CardZone.Hand: return slots;
                    case CardZone.Group: return slotsPetro;
                    case CardZone.Special: return slotsSpecial;
                    case CardZone.ToG: return AislotsToG;
                    case CardZone.Maze: return reparto;
                    case CardZone.HoleMaze: return holemaze;
                    default: return null;
                }
            case Turn.AI:
                switch (zone)
                {
                    case CardZone.Hand: return Aislots;
                    case CardZone.Group: return AislotsPetro;
                    case CardZone.Special: return AislotsSpecial;
                    case CardZone.ToG: return slotsToG;
                    case CardZone.Maze: return reparto;
                    case CardZone.HoleMaze: return holemaze;
                    default: return null;
                }
        }
        return null;
    }

    public IEnumerator Suavizado(RectTransform carta, Transform destino)
    {
        Vector3 inicio = carta.position;
        Vector3 final = destino.position;
        Transform parentOriginal = carta.parent;

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
