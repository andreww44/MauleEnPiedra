using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//La logica que guarda por debajo el player para jugar, guarda una mano de cartas, etc.

public class SCR_Player : MonoBehaviour
{

    private List<SO_Cards> HandCards = new List<SO_Cards>();

    [SerializeField] bool readySetup = false;
    [SerializeField] bool endTurn = false;

    void Start()
    {

    }
    void Update()
    {
        
    }

    public IEnumerator DoMulligan(Turn turn)
    {
        if (turn==Turn.Player)
        {
            Debug.Log("Open UI");
        }
        
        Debug.Log(name + " está eligiendo cartas para cambiar...");

        yield return new WaitUntil(() => readySetup); // Aquí puedes mostrar UI real
        readySetup = false;
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
    

}
