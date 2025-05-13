using UnityEngine;

//Se guarda la informacion de lo que deberia contener el mazo de cartas

[CreateAssetMenu(fileName = "Deck", menuName = "SO/Deck")]
public class SO_DeckCard : ScriptableObject
{
    public SO_Cards[] petroglyphsCards;
}
