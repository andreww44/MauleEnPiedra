using UnityEngine;

[CreateAssetMenu(fileName = "NuevaColeccion", menuName = "Cartas/Colección de Cartas")]
public class ColeccionCartasSO : ScriptableObject
{
    public CartaData[] cartas;
}
