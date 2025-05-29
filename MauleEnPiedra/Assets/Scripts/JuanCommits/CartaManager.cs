using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum CardZone
{
    Hand,
    Group,
    Special,
    Maze
}

public class CartaManager : MonoBehaviour
{
    public SO_DeckCard coleccion;
    public GameObject cartaPrefab;
    public Transform[] slots;
    public Transform[] reparto;

    //Slots Petrogliphs
    public Transform[] slotsPetro;
    //Slots Specials
    public Transform[] slotsSpecial;

    public float tiempoEntreCartas = 0.4f;
    public float duracionMovimiento = 0.4f;

    public static CartaManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance);
        }
    }
    void Start()
    {
    }

    

   
    public IEnumerator MoveCard(int fromIndex, int toIndex, SO_Cards card, CardZone fromZone, CardZone toZone)
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

        yield return new WaitForSeconds(tiempoEntreCartas);

        Transform destino = toSlots[toIndex];
        yield return StartCoroutine(Suavizado(cartaRT, destino));
        cartaGO.transform.GetChild(0).GetComponent<Image>().sprite = card.image;
        destino.GetComponent<CartaSlot>().setSoCard(card);
        yield return new WaitForSeconds(tiempoEntreCartas);

        
        

        
    }

    private Transform[] GetSlotsByZone(CardZone zone)
    {
        switch (zone)
        {
            case CardZone.Hand: return slots;
            case CardZone.Group: return slotsPetro;
            case CardZone.Special: return slotsSpecial;
            case CardZone.Maze: return reparto;
            default: return null;
        }
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
