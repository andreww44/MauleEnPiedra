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
    [SerializeField] private SCR_Player Ai;
    [SerializeField] private SO_DeckCard _deckCard;
    [SerializeField] private List<SO_Cards> DeckCard = new List<SO_Cards>();
    [SerializeField] private List<SO_Cards> _holeDeck = new List<SO_Cards>();
    [SerializeField] private CartaManager CardManagerPlayer;
    [SerializeField] private CartaManager CardManagerAI;


    //Player GameHand


    [SerializeField] private GameObject UICardsMulligan;

    private Turn currentTurn;
    private GameState currentGameState;


   
    [SerializeField] private bool hasStartedTurn = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadCards();
        StartCoroutine(GameSetup());
    }

    // Update is called once per frame
    void Update()
    {
        if (currentGameState == GameState.InGame)
        {
            if (DeckCard.Count == 0)
            {
                foreach (var card in _holeDeck)
                {
                    DeckCard.Add(card);
                    StartCoroutine(CardManagerPlayer.MoveCard(0, 0, card, CardZone.HoleMaze, CardZone.Maze, false));
                }
                _holeDeck.Clear();
            }
            if(Player.GetPoints() >= 3)
            {
                currentGameState = GameState.EndGame;
            }
            if (!hasStartedTurn)
            {
                hasStartedTurn = true;
                if (currentTurn == Turn.Player) {StartPlayerTurn();}
                else { StartPlayerTurn();}
            }
            else
            {
                if(currentTurn == Turn.Player) { PlayerPet();}
                else
                {
                    PetAI();
                }
            }
        }

        if (currentGameState == GameState.EndGame)
        {
            UnityEngine.Debug.Log("Carga la siguiente pantalla");
        }

    }

    public void DrawRandomCard(Turn turn)
    {

        int index = UnityEngine.Random.Range(0, DeckCard.Count);
       

        if (turn == Turn.Player)
        {
            if (!Scr_Rules.FullHand(Player.GetHand()))
            {
                SO_Cards drawnCard = DeckCard[index];
                DeckCard.RemoveAt(index);

                List<SO_Cards> hand = Player.GetHand();
                int insertIndex = -1;

                for (int i = 0; i < hand.Count; i++)
                {
                    if (hand[i] == null)
                    {
                        insertIndex = i;
                        break;
                    }
                }

                if (insertIndex == -1)
                {
                    insertIndex = hand.Count;
                    hand.Add(null); 
                }

                StartCoroutine(CardManagerPlayer.MoveCard(0, insertIndex, drawnCard, CardZone.Maze, CardZone.Hand, true));

                hand[insertIndex] = drawnCard;
            }
        }
        if (turn == Turn.AI)
        {
            if (!Scr_Rules.FullHand(Ai.GetHand()))
            {
                SO_Cards drawnCard = DeckCard[index];
                DeckCard.RemoveAt(index);

                List<SO_Cards> hand = Ai.GetHand();
                int insertIndex = -1;

                for (int i = 0; i < hand.Count; i++)
                {
                    if (hand[i] == null)
                    {
                        insertIndex = i;
                        break;
                    }
                }

                if (insertIndex == -1)
                {
                    insertIndex = hand.Count;
                    hand.Add(null);
                }

                StartCoroutine(CardManagerAI.MoveCard(0, insertIndex, drawnCard, CardZone.Maze, CardZone.Hand, false));

                hand[insertIndex] = drawnCard;
            }
        }
    }

    public void DrawSpecificCard(Turn turn, SO_Cards card)
    {
        if (turn == Turn.Player)
        {
            if (!Scr_Rules.FullHand(Player.GetHand().Count))
            {
                StartCoroutine(CardManagerPlayer.MoveCard(0, Player.GetHand().Count, card, CardZone.Maze, CardZone.Hand, true));
                Player.DrawCard(card);
                DeckCard.Remove(card);
            }
        }
        if (turn == Turn.AI)
        {
            if (!Scr_Rules.FullHand(Ai.GetHand().Count))
            {
                StartCoroutine(CardManagerPlayer.MoveCard(0, Ai.GetHand().Count, card, CardZone.Maze, CardZone.Hand, false));
                Ai.DrawCard(card);
                DeckCard.Remove(card);
            }
        }
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
            for (int i = 0; i < 4; i++)
            {
                DrawRandomCard(Turn.Player);
                //DrawSpecificCard(Turn.Player, DeckCard[0]);
            }
            for (int i = 0; i < 5; i++)
            {
                DrawRandomCard(Turn.AI);
            }
        }
        else
        {
            for (int i = 0; i < 5; i++)
            {
                DrawRandomCard(Turn.AI);
            }
            for (int i = 0; i < 4; i++)
            {
                DrawRandomCard(Turn.Player);
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

    //Button Next 
    public void NextTurn()
    {
        currentTurn = (currentTurn == Turn.Player) ? Turn.AI : Turn.Player;
        hasStartedTurn = false;
    }

    private void StartPlayerTurn()
    {
        if (currentTurn == Turn.Player)
        {
            if (!Scr_Rules.FullHand(Player.GetHand()))
            {
                DrawRandomCard(Turn.Player);
                //DrawSpecificCard(Turn.Player, DeckCard[0]);
            }
        }
        else 
        {
            if (!Scr_Rules.FullHand(Ai.GetHand()))
            {
                DrawRandomCard(Turn.AI);

            }
        }

        

        // Aquí puedes añadir otras lógicas si quieres que el jugador pueda realizar acciones, etc.
        UnityEngine.Debug.Log("Juega el humano");
    }

    private void PlayerPet()
    {
        if (Scr_Rules.PetroComplete(Player.GetGroup()))
        {
            foreach (var card in Player.GetGroup())
            {
                _holeDeck.Add(card);
            }
           
            Player.PetroComplete();
            Player.AddPlayerPoints();
            UnityEngine.Debug.Log("Suma Puntos");
        }

        if (Scr_Rules.PetroInComplete(Player.GetGroup()))
        {
            foreach (var card in Player.GetGroup())
            {
                _holeDeck.Add(card);
            }
            Player.PetroComplete();
            UnityEngine.Debug.Log("No suma puntos");

        }
    }

    private void PetAI()
    {
        if (Scr_Rules.PetroComplete(Ai.GetGroup()))
        {
            foreach (var card in Ai.GetGroup())
            {
                _holeDeck.Add(card);
            }

            Ai.PetroComplete();
            Ai.AddPlayerPoints();
            UnityEngine.Debug.Log("Suma Puntos");
        }

        if (Scr_Rules.PetroInComplete(Ai.GetGroup()))
        {
            foreach (var card in Ai.GetGroup())
            {
                _holeDeck.Add(card);
            }
            Ai.PetroComplete();
            UnityEngine.Debug.Log("No suma puntos");

        }
    }
}
