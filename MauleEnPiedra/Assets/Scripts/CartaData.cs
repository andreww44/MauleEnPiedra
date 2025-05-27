using UnityEngine;

[CreateAssetMenu(fileName = "NuevaCarta", menuName = "Cartas/Carta")]
public class CartaData : ScriptableObject
{
    public string nombre;
    [TextArea]
    public string descripcion;
    public int poder;
    public Sprite imagen;
}
