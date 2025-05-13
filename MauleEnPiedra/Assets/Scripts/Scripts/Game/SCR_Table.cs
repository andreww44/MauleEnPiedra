using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public enum Turn
{
    Player,
    AI
}

public enum GameState
{
    Setup,
    InGame,
    EndGame
}


public class SCR_Table : MonoBehaviour
{

    //Lógica detras del juego
    [SerializeField] private SCR_Player Player;
    [SerializeField] private SCR_Player Ai;
    [SerializeField] private SO_DeckCard _deckCard;
    [SerializeField] private List<SO_Cards> DeckCard;
    [SerializeField] private List<SO_Cards> _holeDeck = new List<SO_Cards>();
    [SerializeField] private List<int> indexCardUse = new List<int>();

    [SerializeField] private GameObject UICardsMulligan;

    private Turn currentTurn;
    private GameState currentGameState;

    [SerializeField] private GameObject _player;
    [SerializeField] private GameObject _ai;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        Player = _player.GetComponent<SCR_Player>();
        Ai = _ai.GetComponent<SCR_Player>();
        LoadCards();
        StartCoroutine(GameSetup());
    }

    // Update is called once per frame
    void Update()
    {
        if(currentGameState == GameState.InGame)
        {
            if (currentTurn == Turn.Player)
            {
                UnityEngine.Debug.Log("Juega el humano");
                if (Player.IsEndTurn())
                {
                    EndTurn();
                }
            }
            else
            {
                UnityEngine.Debug.Log("Juega la IA");
                Ai.SetEndTurn(true);
                if (Ai.IsEndTurn())
                {
                    EndTurn();
                }
            }
        }
        
    }

    private void EndTurn()
    {
        currentTurn = (currentTurn == Turn.Player) ? Turn.AI : Turn.Player;
        Ai.SetEndTurn(false);
        Player.SetEndTurn(false);
    }

    IEnumerator GameSetup()
    {
        //Paso 1: Sortear turno
        currentTurn = (Turn)Random.Range(0, 2);
        UnityEngine.Debug.Log("El jugador que empieza es: " + currentTurn);

        //Paso 2: Dar cartas iniciales (ej. 3 para el que empieza, 4 para el otro)
        if (currentTurn == Turn.Player)
        {
            int index = 0;
            for (int i = 0; i < 5; i++){
                index = Random.Range(0, DeckCard.Count);
                Player.DrawCard(DeckCard[index]);
                DeckCard.RemoveAt(index);
            }
            for (int i = 0; i < 5; i++)
            {
                index = Random.Range(0, DeckCard.Count);
                Ai.DrawCard(DeckCard[index]);
                DeckCard.RemoveAt(index);
            }
        }
        else
        {
            int index = 0;
            for (int i = 0; i < 5; i++)
            {
                index = Random.Range(0, DeckCard.Count);
                Ai.DrawCard(DeckCard[index]);
                DeckCard.RemoveAt(index);
            }
            for (int i = 0; i < 5; i++)
            {
                index = Random.Range(0, DeckCard.Count);
                Player.DrawCard(DeckCard[index]);
                DeckCard.RemoveAt(index);
            }
        }
        // Esperar a que ambos hagan Mulligan
        UICardsMulligan.SetActive(true);
        yield return StartCoroutine(Player.DoMulligan(Turn.Player));
        yield return StartCoroutine(Ai.DoMulligan(Turn.AI));
        UICardsMulligan.SetActive(false);
        currentGameState = GameState.InGame;
        
    }

    private void LoadCards()
    {
        foreach(var card in _deckCard.petroglyphsCards)
        {
            DeckCard.Add(card);
        }
    }
}
