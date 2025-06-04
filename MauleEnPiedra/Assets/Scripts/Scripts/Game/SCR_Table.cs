using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
    [SerializeField] private CartaManager CardManager;

    public SCR_CoroutineQueue coroutineQueue;


    //Player GameHand


    [SerializeField] private GameObject UICardsMulligan;

    private Turn currentTurn;
    private GameState currentGameState;


   
    [SerializeField] private bool hasStartedTurn = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        //LoadCards();
        GameSetup();
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
                    coroutineQueue.Enqueue(CardManager.MoveCard(0, 0, card, CardZone.HoleMaze, CardZone.Maze, false, Turn.Player));
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
                DeckCard.Remove(drawnCard);

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

                coroutineQueue.Enqueue(CardManager.MoveCard(0, insertIndex, drawnCard, CardZone.Maze, CardZone.Hand, true, Turn.Player));

                hand[insertIndex] = drawnCard;
            }
        }
        if (turn == Turn.AI)
        {
            if (!Scr_Rules.FullHand(Ai.GetHand()))
            {
                SO_Cards drawnCard = DeckCard[index];
                DeckCard.Remove(drawnCard);

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

                coroutineQueue.Enqueue(CardManager.MoveCard(0, insertIndex, drawnCard, CardZone.Maze, CardZone.Hand, false, Turn.AI));

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
                coroutineQueue.Enqueue(CardManager.MoveCard(0, Player.GetHand().Count, card, CardZone.Maze, CardZone.Hand, true, Turn.Player));
                Player.DrawCard(card);
                DeckCard.Remove(card);
            }
        }
        if (turn == Turn.AI)
        {
            if (!Scr_Rules.FullHand(Ai.GetHand().Count))
            {
                coroutineQueue.Enqueue(CardManager.MoveCard(0, Ai.GetHand().Count, card, CardZone.Maze, CardZone.Hand, false, Turn.AI));
                Ai.DrawCard(card);
                DeckCard.Remove(card);
            }
        }
    }

    public void GameSetup()
    {
        //Paso 1: Sortear turno
        currentTurn = (Turn)UnityEngine.Random.Range(0, 2);
        UnityEngine.Debug.Log("El jugador que empieza es: " + currentTurn);

        //Paso 3: Dar cartas iniciales (ej. 3 para el que empieza, 4 para el otro)
        if (currentTurn == Turn.Player)
        {
            for (int i = 0; i < 5; i++)
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
            for (int i = 0; i < 5; i++)
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

    public bool StartCard(SO_Cards card)
    {
        //Carta Especiales
        switch (card.Code)
        {
            //Cambia una carta con el contrincante. - Intercambio cultura;
            case 21: //
                //Esta bug pero funcionando
                var playerHand = Player.GetHand();
                var aiHand = Ai.GetHand();

                if (playerHand.Count == 0 || aiHand.Count == 0)
                {
                    return false;
                }

                // Filtrar solo cartas no nulas
                var playerValidIndexes = Enumerable.Range(0, playerHand.Count).Where(i => playerHand[i] != null).ToList();
                var aiValidIndexes = Enumerable.Range(0, aiHand.Count).Where(i => aiHand[i] != null).ToList();

                // Verificar si hay al menos una carta válida
                if (playerValidIndexes.Count == 0 || aiValidIndexes.Count == 0)
                {
                    return false;
                }

                // Elegir una carta al azar de las válidas
                int randhandplayer = playerValidIndexes[UnityEngine.Random.Range(0, playerValidIndexes.Count)];
                int randhandAI = aiValidIndexes[UnityEngine.Random.Range(0, aiValidIndexes.Count)];

                if (Player.GetHand()[randhandplayer].Code == 21 || Ai.GetHand()[randhandAI].Code== 21)
                {
                    return false;
                }

                var cardPlayer = playerHand[randhandplayer];
                var cardAI = aiHand[randhandAI];

                // Remover las cartas
                playerHand.Remove(cardPlayer);
                aiHand.Remove(cardAI);

                // Jugar
                return Player.OponentHand(cardAI) && Ai.OponentHand(cardPlayer);

            //Roba una carta del deck.
            case 22:
                if (currentTurn == Turn.Player)
                {
                    if (Scr_Rules.FullHand(Player.GetHand().Count))
                    {
                        return false;
                    }
                    DrawRandomCard(currentTurn);
                    return true;
                }
                else if (currentTurn == Turn.AI)
                {
                    if (Scr_Rules.FullHand(Ai.GetHand().Count))
                    {
                        return false;
                    }
                    DrawRandomCard(currentTurn);
                    return true;
                }
                return false;
            //Roba una carta aleatoria del mazo contrario.
            case 23:
                if (currentTurn == Turn.Player)
                {
                    if (Scr_Rules.FullHand(Player.GetHand().Count))
                    {
                        return false;
                    }
                    
                    return true;
                }
                else if (currentTurn == Turn.AI)
                {
                    if (Scr_Rules.FullHand(Ai.GetHand().Count))
                    {
                        return false;
                    }
                    
                    return true;
                }
                return false;
                break;
            default:
                break;
        }
        return false;
    }

    public List<SO_Cards> GetHole()
    {
        return _holeDeck;
    }

    private void StealCard()
    {

    }
}


