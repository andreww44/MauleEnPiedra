using UnityEngine;

public class MazoManager : MonoBehaviour
{
    public CartaData[] cartasDisponibles;
    public GameObject cartaPrefab;
    public Transform contenedor;

    void Start()
    {
        foreach (CartaData carta in cartasDisponibles)
        {
            GameObject nuevaCarta = Instantiate(cartaPrefab, contenedor);
            CartaUI cartaUI = nuevaCarta.GetComponent<CartaUI>();
            cartaUI.MostrarCarta(carta);
        }
    }
}
