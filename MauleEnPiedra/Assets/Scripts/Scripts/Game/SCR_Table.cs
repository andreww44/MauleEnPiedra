using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System;
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
    //[SerializeField] private SCR_Player Ai;
    [SerializeField] private SO_DeckCard _deckCard;
    [SerializeField] private List<SO_Cards> DeckCard;
    [SerializeField] private List<SO_Cards> _holeDeck = new List<SO_Cards>();
    [SerializeField] private List<int> indexCardUse = new List<int>();
    [SerializeField] private CartaManager CardManager;

    [SerializeField] private GameObject UICardsMulligan;

    [SerializeField] private bool drawCard = false;

    private Turn currentTurn;
    private GameState currentGameState;

    [SerializeField] private GameObject _player;
    [SerializeField] private GameObject _ai;

    [SerializeField] private GameObject[] slotspPlayers;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Player = _player.GetComponent<SCR_Player>();
        //Ai = _ai.GetComponent<SCR_Player>();
        LoadCards();
        StartCoroutine(GameSetup());
    }

    // Update is called once per frame
    void Update()
    {
        if (currentGameState == GameState.InGame)
        {
            if (currentTurn == Turn.Player)
            {

                //DrawCardTo(Turn.Player);
                UnityEngine.Debug.Log("Juega el humano");

                if (Player.IsEndTurn())
                {
                    EndTurn();
                }
            }
            else
            {

                /*/DrawCardTo(Turn.AI);

                UnityEngine.Debug.Log("Juega la IA");

                if (Ai.IsEndTurn())
                {
                    EndTurn();
                }*/
            }
        }

    }

    private void DrawRandomCard(Turn turn)
    {

        int index = UnityEngine.Random.Range(0, DeckCard.Count);
        bool fullHandPlayer = Scr_Rules.FullHand(Player.GetHand().Count);
        //bool fullHandAI = Scr_Rules.FullHand(Ai.GetHand().Count);

        if (turn == Turn.Player)
        {
            if (!fullHandPlayer && drawCard == false)
            {
                StartCoroutine(CardManager.MoveCard(0, Player.GetHand().Count, DeckCard[index], CardZone.Maze, CardZone.Hand));
                Player.DrawCard(DeckCard[index]);
                DeckCard.RemoveAt(index);
            }
        }
        drawCard = true;
    }

    public void DrawSpecificCard(Turn turn, SO_Cards card)
    {
        if (turn == Turn.Player)
        {
            if (!Scr_Rules.FullHand(Player.GetHand().Count))
            {
                StartCoroutine(CardManager.MoveCard(0, Player.GetHand().Count, card, CardZone.Maze, CardZone.Hand));
                Player.DrawCard(card);
                DeckCard.Remove(card);
            }
        }
    }

    private void EndTurn()
    {
        currentTurn = (currentTurn == Turn.Player) ? Turn.AI : Turn.Player;
        drawCard = false;
        //Ai.SetEndTurn(false);
        Player.SetEndTurn(false);
    }

    IEnumerator GameSetup()
    {
        //Paso 1: Sortear turno
        currentTurn = (Turn)UnityEngine.Random.Range(0, 2);
        UnityEngine.Debug.Log("El jugador que empieza es: " + currentTurn);


        //Paso 2: Esperar a que ambos hagan Mulligan
        UICardsMulligan.SetActive(true);
        yield return StartCoroutine(Player.DoMulligan(Turn.Player));
        //yield return StartCoroutine(Ai.DoMulligan(Turn.AI));
        UICardsMulligan.SetActive(false);

        //Paso 3: Dar cartas iniciales (ej. 3 para el que empieza, 4 para el otro)
        if (currentTurn == Turn.Player)
        {
            for (int i = 0; i < 5; i++)
            {
                DrawRandomCard(Turn.Player);
                drawCard = false;
            }
            /*for (int i = 0; i < 5; i++)
            {
                DrawCardTo(Turn.AI);
                drawCard = false;
            }*/
        }
        else
        {
            /*for (int i = 0; i < 5; i++)
            {
                DrawCardTo(Turn.AI);
                drawCard = false;
            }*/
            for (int i = 0; i < 5; i++)
            {
                DrawRandomCard(Turn.Player);
                drawCard = false;
            }
        }
        currentGameState = GameState.InGame;
    }

    private void LoadCards()
    {
        foreach (var card in _deckCard.petroglyphsCards)
        {
            DeckCard.Add(card);
        }
    }

    private void Testing(){

        //Scr_Rules.PetroComplete(slotspPlayers[]);
    }
}
