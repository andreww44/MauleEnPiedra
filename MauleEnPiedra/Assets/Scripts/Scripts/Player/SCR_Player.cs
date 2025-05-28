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
            if (PlayCard(0))
            {
                UnityEngine.Debug.Log("<color=red>¡Carta Retirada!</color>");
            }
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (PlayCard(1))
            {
                UnityEngine.Debug.Log("<color=red>¡Carta Retirada!</color>");
            }
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (PlayCard(2))
            {
                UnityEngine.Debug.Log("<color=red>¡Carta Retirada!</color>");
            }
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (PlayCard(3))
            {
                UnityEngine.Debug.Log("<color=red>¡Carta Retirada!</color>");
            };
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (PlayCard(4))
            {
                UnityEngine.Debug.Log("<color=red>¡Carta Retirada!</color>");
            }
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            if (PlayCard(5))
            {
                UnityEngine.Debug.Log("<color=red>¡Carta Retirada!</color>");
            }

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

    public bool PlayCard(int index)
    {
        if (index < 0 || index >= HandCards.Count) { return false; }
        HandCards.RemoveAt(index);
        return true;
    }

    public List<SO_Cards> GetHand()
    {
        return HandCards;
    }


    

}
