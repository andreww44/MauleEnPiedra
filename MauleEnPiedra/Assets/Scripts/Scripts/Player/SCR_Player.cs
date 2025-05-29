using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

//La logica que guarda por debajo el player para jugar, guarda una mano de cartas, etc.

public class SCR_Player : MonoBehaviour
{

    [SerializeField] private List<SO_Cards> HandCards = new List<SO_Cards>();
    [SerializeField] private List<SO_Cards> SpecialCards = new List<SO_Cards>();
    [SerializeField] private List<SO_Cards> GroupCards = new List<SO_Cards>();
    [SerializeField] bool readySetup = false;
    [SerializeField] bool endTurn = false;
    
    public bool IsReadySetup()
    {
        return readySetup;
    }

    void Start()
    {
    }
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Q))
        {
            PlayCardToPet(0);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            PlayCardToPet(1);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            PlayCardToPet(2);
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            PlayCardToPet(3);
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            PlayCardToPet(4);
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            PlayCardToPet(5);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            PlayPetToHand(0);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            PlayPetToHand(1);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            PlayPetToHand(2);
        }

    }

    public IEnumerator DoMulligan(Turn turn)
    {
        if (turn==Turn.Player)
        {
            Debug.Log("Open UI");
        }
        
        Debug.Log(name + " está eligiendo cartas para cambiar...");

        yield return new WaitUntil(() => readySetup); // Aquí puedes mostrar UI real
        //readySetup = false;
    }

    public bool IsEndTurn() { 
        return endTurn;
    }

    public void SetEndTurn(bool _endTurn) {
        endTurn = _endTurn;
    }

    //RobarCarta
    public void DrawCard(SO_Cards card)
    {
        Debug.Log(name + " Robo Carta " + card.name);
        HandCards.Add(card);
    }

    public void CardsReady()
    {

        readySetup = true;
    }


    public List<SO_Cards> GetHand()
    {
        return HandCards;
    }

    private void PlayCardToPet(int indexcard)
    {
        if (indexcard < 0 || indexcard >= HandCards.Count)
            return;

        if(GroupCards.Count >= 3)
        {
            return;
        }
        var card = HandCards[indexcard];

        if (card != null && card.type == Card.Petroglyph)
        {
            int insertIndex = -1;
            for (int i = 0; i < GroupCards.Count; i++)
            {
                if (GroupCards[i] == null)
                {
                    insertIndex = i;
                    break;
                }
            }

            if (insertIndex == -1)
            {
                insertIndex = GroupCards.Count;
                GroupCards.Add(null); // Expandimos lista si no hay espacio
            }

            StartCoroutine(CartaManager.Instance.MoveCard(indexcard, insertIndex, card, CardZone.Hand, CardZone.Group));
            GroupCards[insertIndex] = card;
            HandCards[indexcard] = null;
        }
    }

    private void PlayPetToHand(int indexcard)
    {
        if (indexcard < 0 || indexcard >= GroupCards.Count)
            return;
       
        var card = GroupCards[indexcard];

        if (card != null && card.type == Card.Petroglyph)
        {
            int insertIndex = -1;
            for (int i = 0; i < HandCards.Count; i++)
            {
                if (HandCards[i] == null)
                {
                    insertIndex = i;
                    break;
                }
            }

            if (insertIndex == -1)
            {
                insertIndex = HandCards.Count;
                HandCards.Add(null); // Expandimos lista si no hay espacio
            }

            StartCoroutine(CartaManager.Instance.MoveCard(indexcard, insertIndex, card, CardZone.Group, CardZone.Hand));
            HandCards[insertIndex] = card;
            GroupCards[indexcard] = null;
        }
    }

    



}
