using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CartaUI : MonoBehaviour
{
    public Image imagenCarta;
    public TextMeshProUGUI nombreTexto;
    public TextMeshProUGUI descripcionTexto;
    public TextMeshProUGUI poderTexto;

    public void MostrarCarta(CartaData data)
    {
        Debug.Log($"Mostrando carta: {data.nombre}");
        imagenCarta.sprite = data.imagen;
        nombreTexto.text = data.nombre;
        descripcionTexto.text = data.descripcion;
        poderTexto.text = data.poder.ToString();
    }

}
