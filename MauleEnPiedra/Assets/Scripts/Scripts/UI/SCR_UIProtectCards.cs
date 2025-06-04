using UnityEngine;
using UnityEngine.UI;

public class SCR_UIProtectCards : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private SO_Cards card;         // La carta que representa este botón
    [SerializeField] private SCR_Table table;        // Referencia al SCR_Table (Tablero)
    [SerializeField] private SCR_Player player;      // Referencia al jugador humano

    private void Awake()
    {
        image.sprite = card.image; // Mostrar la imagen en el botón
    }

    public void SelectCard()
    {
        // Si no está readySetup, significa que estamos en fase de Setup (Mulligan):
        if (!player.IsReadySetup())
        {
            player.CardsReady();
            table.DrawSpecificCard(Turn.Player, card);
            table.DrawRandomCard(false); // IA roba
            return;
        }

        // Si ya estamos en GAME:
        // Dependiendo del tipo de carta, invocar al SCR_Player:
        if (card.type == Card.Protect)
        {
            // Buscar en la mano de player y obtener el índice de ‘card’ en HandCards
            int idx = player.HandCards.IndexOf(card);
            if (idx >= 0)
            {
                player.ClickHand(idx, Turn.Player);
                table.EndCurrentPlayerTurn();
            }
        }
        else if (card.type == Card.Special)
        {
            int idx = player.HandCards.IndexOf(card);
            if (idx >= 0)
            {
                player.ClickHand(idx, Turn.Player);
                table.EndCurrentPlayerTurn();
            }
        }
        // NOTA: no se contemplan aquí amenazas, ya que las amenazas suelen estar en otra zona de UI.
    }
}
