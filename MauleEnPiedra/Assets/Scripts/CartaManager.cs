using UnityEngine;

public class CartaManager : MonoBehaviour
{
    public ColeccionCartasSO coleccion;

    void Start()
    {
        if (coleccion != null)
        {
            foreach (CartaData carta in coleccion.cartas)
            {
                Debug.Log($"Carta: {carta.nombre}, Poder: {carta.poder}");
            }
        }
    }
}
