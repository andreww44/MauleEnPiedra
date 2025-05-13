using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SCR_UIProtectCards : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI descrition;
    [SerializeField] private TextMeshProUGUI region;
    [SerializeField] private TextMeshProUGUI part;
    [SerializeField] private SO_Cards card;

    private void Awake()
    {
        image = card.image;
        _name.text = card.name;
        descrition.text = card.description;
        region.text = "";
        part.text = "";
    }

    public void SelectCard()
    {
        Debug.Log("Carta Escogida");

    }
}
