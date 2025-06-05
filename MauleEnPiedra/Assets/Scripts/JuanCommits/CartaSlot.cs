using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class CartaSlot : MonoBehaviour
{

    //sprivate GameObject card;
    [SerializeField] private SO_Cards cardData;
    private Button myButton;

    void Start()
    {
        myButton = GetComponent<Button>();
    }

    public void Update()
    {
        if (myButton != null)
        {
            if (transform.childCount == 0)
            {
                RetireCard();
            }

            if (gameObject.transform.childCount > 0)
            {
                myButton.interactable = true;

            }
            else
            {
                myButton.interactable = false;
            }
        }
        
    }

    public void setSoCard(SO_Cards card)
    {
        cardData = card;
    }

    public void RetireCard()
    {
        cardData = null;
    } 
    

    
}
