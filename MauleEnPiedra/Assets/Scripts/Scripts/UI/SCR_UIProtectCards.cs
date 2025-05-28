using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SCR_UISpecialCards : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private SO_Cards card;
    [SerializeField] private SCR_Table table;
    [SerializeField] private SCR_Player player;

    private void Awake()
    {
        image.sprite = card.image;
    }

    public void SelectCard()
    {
        if (player.IsReadySetup() == false)
        {
            player.CardsReady();
            table.DrawSpecificCard(Turn.Player, card);
        }
        

    }
}
