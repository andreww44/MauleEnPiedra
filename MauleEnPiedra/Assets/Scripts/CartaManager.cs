using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CartaManager : MonoBehaviour
{
    public SO_DeckCard coleccion;
    public GameObject cartaPrefab;
    public Transform[] slots;
    public RectTransform Reparto; 

    public float tiempoEntreCartas = 0.3f;
    public float duracionMovimiento = 0.4f;



    void Start()
    {
    }

    public IEnumerator RepartirCartas(int index, Sprite cardimage)
    {
        
        UnityEngine.Debug.Log("$<color=yellow>"+ index + "</color>");

        int cantidadCartas = coleccion.petroglyphsCards.Count();
        int cantidadSlots = slots.Length;
        int maxSlots = Mathf.Min(6, Mathf.Min(cantidadCartas, cantidadSlots));
        
        GameObject cartaGO = Instantiate(cartaPrefab, Reparto);
        RectTransform cartaRT = cartaGO.GetComponent<RectTransform>();
        cartaRT.localPosition = Vector3.zero;

        yield return new WaitForSeconds(tiempoEntreCartas);
        Transform destino = slots[index];
        yield return StartCoroutine(Suavizado(cartaRT, destino));
        yield return new WaitForSeconds(tiempoEntreCartas);

        cartaGO.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = cardimage;
    }

    IEnumerator Suavizado(RectTransform carta, Transform destino)
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
