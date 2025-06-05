using MC.Modelo;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using UnityEngine;

public enum Turn
{
    Player,
    AI
}

public enum GameStateFlow
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

    [SerializeField] private int turns = 0;

    public SCR_CoroutineQueue coroutineQueue;


    //Player GameHand


    [SerializeField] private GameObject UICardsMulligan;

    private Turn currentTurn;
    private GameStateFlow currentGameState;


   
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
        if (currentGameState == GameStateFlow.InGame)
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
                currentGameState = GameStateFlow.EndGame;
            }
            if (!hasStartedTurn)
            {
                hasStartedTurn = true;
                if (currentTurn == Turn.Player) {StartPlayerTurn();}
                else { StartPlayerTurn();}
            }
            else
            {
                if(currentTurn == Turn.Player && Player.lostTurn == false) 
                {
                    //Permit
                    //UnityEngine.Debug.Log("Juega Player ");
                    Player.block = false;
                    Ai.block = true;
                    PlayerPet();
                } 
                else if(currentTurn == Turn.AI && Ai.lostTurn == false)
                {
                    //  UnityEngine.Debug.Log("Juega Ai");
                    Player.block = true;
                    Ai.block = false;
                    WaitAndPlayAI();
                    PetAI();
                    NextTurn();

                }
            }
        }

        if (currentGameState == GameStateFlow.EndGame)
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
        currentGameState = GameStateFlow.InGame;
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
        turns++;

        if (Player.isProtect == true)
        {
            Player.indexProtect--;
            if (Player.indexProtect == 0) {
                Player.isProtect = false;
                Player.indexProtect = 2;
            }
        }
        if (Ai.isProtect == true) {

            Ai.indexProtect--;
            if (Player.indexProtect == 0)
            {
                Player.isProtect = false;
                Player.indexProtect = 1;
            }
        }
        if (Player.isLock == true)
        {
            Player.indexLock--;
            if (Player.indexLock == 0)
            {
                Player.isLock = false;
                Player.indexLock = 2;
            }
        }
        if (Ai.isLock == true)
        {

            Ai.indexLock--;
            if (Player.indexLock == 0)
            {
                Player.isLock = false;
                Player.indexLock = 1;
            }
        }
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
            if (Player.isLock == false)
            {
                Player.AddPlayerPoints();
                UnityEngine.Debug.Log("Suma Puntos");
            }
            else
            {
                UnityEngine.Debug.Log("No suma puntos esta bloqueado");
            }
            
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
            
            if (Player.isLock == false) 
            {
                Ai.AddPlayerPoints();
                UnityEngine.Debug.Log("Suma Puntos");
            }
            else
            {
                UnityEngine.Debug.Log("No suma puntos esta bloqueado");
            }
            
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
        //Carta Especiales Activas
        switch (card.Code)
        {
            // Protege Turno Ready
            case 11:
                if (currentTurn == Turn.Player)
                {
                    Player.isProtect = true;
                }
                else
                {
                    Ai.isProtect = true; 
                }
                return true;
            // Bloquea Amenaza Esta carta es reactiva hacerla activa xdd Ets falta
            case 12: 
            // Saca Ultima Carta del pozo
            case 13:
                UnityEngine.Debug.Log("13 Card");
                if (currentTurn == Turn.Player)
                {
                    if (Scr_Rules.FullHand(Player.GetHand()))
                    {
                        return false;
                    }
                    DrawRandomCard(currentTurn);
                    return true;
                }
                else if (currentTurn == Turn.AI)
                {
                    if (Scr_Rules.FullHand(Ai.GetHand()))
                    {
                        return false;
                    }
                    var lastCard = _holeDeck[_holeDeck.Count - 1];
                    DrawSpecificCard(Turn.AI, lastCard);
                    _holeDeck.Remove(lastCard);
                    return Ai.HoleToHand(lastCard);
                }
                return false;
            //Cambia una carta con el contrincante. - Intercambio cultura;
            case 21: //
                UnityEngine.Debug.Log("21 Card");
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
                UnityEngine.Debug.Log("22 Card");
                if (currentTurn == Turn.Player)
                {
                    if (Scr_Rules.FullHand(Player.GetHand()))
                    {
                        return false;
                    }
                    DrawRandomCard(currentTurn);
                    return true;
                }
                else if (currentTurn == Turn.AI)
                {
                    if (Scr_Rules.FullHand(Ai.GetHand()))
                    {
                        return false;
                    }
                    DrawRandomCard(currentTurn);
                    return true;
                }
                return false;
            //Roba una carta aleatoria del la mano contraria.
            case 23:
                UnityEngine.Debug.Log("23 Card");
                if (currentTurn == Turn.Player)
                {
                    if (Scr_Rules.FullHand(Player.GetHand()))
                    {
                        return false;
                    }
                    var opoHand = Ai.GetHand();
                    var opovalidindex = Enumerable.Range(0, opoHand.Count).Where(i =>opoHand[i] != null).ToList();
                    if (opovalidindex.Count == 0)
                    { 
                        return false; 
                    }
                    int randhandop = opovalidindex[UnityEngine.Random.Range(0, opovalidindex.Count)];
                    var opocard = opoHand[randhandop];
                    opoHand.Remove(opocard);
                    return Player.OponentHand(opocard);
                }
                else if (currentTurn == Turn.AI)
                {
                    if (Scr_Rules.FullHand(Ai.GetHand()))
                    {
                        return false;
                    }
                    var opoHand = Player.GetHand();
                    var opovalidindex = Enumerable.Range(0, opoHand.Count).Where(i => opoHand[i] != null).ToList();
                    if (opovalidindex.Count == 0)
                    {
                        return false;
                    }
                    int randhandop = opovalidindex[UnityEngine.Random.Range(0, opovalidindex.Count)];
                    var opocard = opoHand[randhandop];
                    opoHand.Remove(opocard);
                    return Player.OponentHand(opocard);
                }
                return false;
            //Descarta una carta aleatoria del armado de petroglifo del contrincante 
            case 31: 
                //
                if (currentTurn == Turn.Player)
                {
                    if (Ai.isProtect)
                    {
                        return false;
                    }
                    var opoHand = Ai.GetGroup();
                    var opovalidindex = Enumerable.Range(0, opoHand.Count).Where(i => opoHand[i] != null).ToList();
                    if (opovalidindex.Count == 0)
                    {
                        return false;
                    }
                    int randhandop = opovalidindex[UnityEngine.Random.Range(0, opovalidindex.Count)];
                    var opocard = opoHand[randhandop];
                    opoHand.Remove(opocard);
                    return Ai.DiscardGroup(opocard, randhandop);
                    
                }
                else if (currentTurn == Turn.AI)
                {
                    if (Player.isProtect)
                    {
                        return false;
                    }
                    var opoHand = Player.GetGroup();
                    var opovalidindex = Enumerable.Range(0, opoHand.Count).Where(i => opoHand[i] != null).ToList();
                    if (opovalidindex.Count == 0)
                    {
                        return false;
                    }
                    int randhandop = opovalidindex[UnityEngine.Random.Range(0, opovalidindex.Count)];
                    var opocard = opoHand[randhandop];
                    opoHand.Remove(opocard);
                    return Player.DiscardGroup(opocard, randhandop);
                }
                return true;
            // Pierde una carta de la mano aleatoria 
            case 32:
                if (currentTurn == Turn.Player)
                {
                    if (Ai.isProtect)
                    {
                        return false;
                    }
                    var opoHand = Ai.GetHand();
                    var opovalidindex = Enumerable.Range(0, opoHand.Count).Where(i => opoHand[i] != null).ToList();
                    if (opovalidindex.Count == 0)
                    {
                        return false;
                    }
                    int randhandop = opovalidindex[UnityEngine.Random.Range(0, opovalidindex.Count)];
                    var opocard = opoHand[randhandop];
                    opoHand.Remove(opocard);
                    return Ai.DiscardCardHand(opocard, randhandop);

                    //var opoGro = Ai.GetGroup();
                }
                else if (currentTurn == Turn.AI)
                {
                    if (Player.isProtect)
                    {
                        return false;
                    }
                    var opoHand = Player.GetHand();
                    var opovalidindex = Enumerable.Range(0, opoHand.Count).Where(i => opoHand[i] != null).ToList();
                    if (opovalidindex.Count == 0)
                    {
                        return false;
                    }
                    int randhandop = opovalidindex[UnityEngine.Random.Range(0, opovalidindex.Count)];
                    var opocard = opoHand[randhandop];
                    opoHand.Remove(opocard);
                    return Player.DiscardCardHand(opocard, randhandop);
                }
                return true;
            // El contrincante pierde un turno
            case 33:
                if (currentTurn == Turn.Player)
                {
                    if (Ai.isProtect)
                    {
                        return false;
                    }
                    Ai.lostTurn = true;
                    return true;
                    //var opoGro = Ai.GetGroup();
                }
                else if (currentTurn == Turn.AI)
                {
                    if (Player.isProtect)
                    {
                        return false;
                    }
                    Player.lostTurn = true;
                    return true;
                }
                return false;
            // El contrincante no suma puntos por petroglifo por un turno
            case 34:
                if (currentTurn == Turn.Player)
                {
                    if (Ai.isProtect)
                    {
                        return false;
                    }
                    Ai.isLock = true;
                    return true;
                    //var opoGro = Ai.GetGroup();
                }
                else if (currentTurn == Turn.AI)
                {
                    if (Player.isProtect)
                    {
                        return false;
                    }
                    Ai.isLock = true;
                    return true;
                }
                return false;
            default:
                break;
        }
        return false;
    }

    public List<SO_Cards> GetHole()
    {
        return _holeDeck;
    }

    //public float iaDelay = 2.0f; // tiempo de espera en segundos

    void WaitAndPlayAI()
    {

        List<CardMC> _maze = new List<CardMC>();
        List<CardMC> _hole = new List<CardMC>();

        foreach (var card in DeckCard)
        {
            var _card = new CardMC(card.Code, card.name, card.type, card.zone, card.parte);
            _maze.Add(_card);
        }
        foreach (var card in _holeDeck)
        {
            var _card = new CardMC(card.Code, card.name, card.type, card.zone, card.parte);
            _hole.Add(_card);
        }
        
        List<CardMC> handp1 = new List<CardMC>();
        List<CardMC> handp2 = new List<CardMC>();

        foreach (var card in Player.GetHand())
        {
            if(card == null)
            {
                continue;
            }
            else
            {
                var _card = new CardMC(card.Code, card.name, card.type, card.zone, card.parte);
                handp1.Add(_card);
            }
            
        }

        foreach (var card in Ai.GetHand())
        {
            if (card == null)
            {
                continue;
            }
            else
            {
                var _card = new CardMC(card.Code, card.name, card.type, card.zone, card.parte);
                handp2.Add(_card);
            }

        }

        List<CardMC> handGroup1 = new List<CardMC>();
        List<CardMC> handGroup2 = new List<CardMC>();

        foreach (var card in Player.GetGroup())
        {
            if (card == null)
            {
                continue;
            }
            else
            {
                var _card = new CardMC(card.Code, card.name, card.type, card.zone, card.parte);
                handGroup1.Add(_card);
            }

        }

        foreach (var card in Ai.GetGroup())
        {
            if (card == null)
            {
                continue;
            }
            else
            {
                var _card = new CardMC(card.Code, card.name, card.type, card.zone, card.parte);
                handGroup2.Add(_card);
            }

        }

        List<CardMC> handSpecial1 = new List<CardMC>();
        List<CardMC> handSpecial2 = new List<CardMC>();

        foreach (var card in Player.GetSpe())
        {
            if (card == null)
            {
                continue;
            }
            else
            {
                var _card = new CardMC(card.Code, card.name, card.type, card.zone, card.parte);
                handSpecial1.Add(_card);
            }

        }

        foreach (var card in Ai.GetSpe())
        {
            if (card == null)
            {
                continue;
            }
            else
            {
                var _card = new CardMC(card.Code, card.name, card.type, card.zone, card.parte);
                handSpecial1.Add(_card);
            }

        }

        PlayerState p1 = new PlayerState(handp1, handGroup1, handSpecial1, Player.GetPoints(), Player.indexProtect, false, Player.lostTurn);
        PlayerState p2 = new PlayerState(handp2, handGroup2, handSpecial2, Ai.GetPoints(), Ai.indexProtect, false, Ai.lostTurn);

        bool end = currentGameState == GameStateFlow.EndGame ? true : false;

        //yield return new WaitForSeconds(0.1f); // primero esperamos

        GameState state = new GameState(_maze, _hole, Turn.AI, p1, p2, end);
        int index = MonteCarlo.MonteCarloTS(state, 2, 2).mejorJugada;

        Ai.ClickHand(index);

        //yield return new WaitForSeconds(1f);
        
    }
}


