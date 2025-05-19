using System.Collections;
using System.Linq;
using UnityEngine;

public class CartaManager : MonoBehaviour
{
    public ColeccionCartasSO coleccion;
    public GameObject cartaPrefab;
    public Transform[] slots;
    public RectTransform Reparto; 

    public float tiempoEntreCartas = 0.3f;
    public float duracionMovimiento = 0.4f;

    void Start()
    {
        if (coleccion != null && cartaPrefab != null && slots != null && Reparto != null)
        {
            StartCoroutine(RepartirCartas());
        }
    }

    IEnumerator RepartirCartas()
    {
        int cantidadCartas = coleccion.cartas.Count();
        int cantidadSlots = slots.Length;
        int maxSlots = Mathf.Min(6, Mathf.Min(cantidadCartas, cantidadSlots));

        for (int i = 0; i < maxSlots; i++)
        {
            GameObject cartaGO = Instantiate(cartaPrefab, Reparto);
            RectTransform cartaRT = cartaGO.GetComponent<RectTransform>();
            cartaRT.localPosition = Vector3.zero;

            CartaUI cartaUI = cartaGO.GetComponent<CartaUI>();
            if (cartaUI != null)
            {
                cartaUI.MostrarCarta(coleccion.cartas[i]);
            }

            yield return new WaitForSeconds(tiempoEntreCartas);

            Transform destino = slots[i];

            yield return StartCoroutine(Suavizado(cartaRT, destino));

            yield return new WaitForSeconds(tiempoEntreCartas);
        }
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
