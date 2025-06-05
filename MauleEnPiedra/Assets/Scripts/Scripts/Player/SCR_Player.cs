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
    [SerializeField] private SCR_Table table;
    [SerializeField] private CartaManager cardManager;
    [SerializeField] private int Poinst = 0;
    [SerializeField] bool readySetup = false;
    
    [SerializeField] Turn MyTurn;

    [SerializeField] public bool block { get; set; }

    [SerializeField] private bool _isprtect = false;
    [SerializeField] private bool _islockt = false;
    [SerializeField] private bool _islostturn = false;

    [SerializeField] private int _indexprtect;
    [SerializeField] private int _indexlock;
    [SerializeField] private int _indexlosttiur;
    //Rules
    [SerializeField] public bool isProtect { get; set; }
    [SerializeField] public int indexProtect { get; set; }
    [SerializeField] public bool lostTurn { get; set; }
    [SerializeField] public int indexTurn { get; set; }
    [SerializeField] public bool isLock { get; set; }
    [SerializeField] public int indexLock { get; set; }
    public bool IsReadySetup()
    {
        return readySetup;
    }

    void Start()
    {
        block = false;
        isProtect = false;
        indexProtect = 2;
        indexLock = 1;
        indexTurn = 1;
        lostTurn = false;
        isLock = false;
    }
    void Update()
    {
        _isprtect = isProtect;
        _islockt = isLock;
        _islostturn = lostTurn;

        _indexlosttiur = indexTurn;
        _indexlock = indexLock;
        _indexprtect = indexTurn;
    }

    public IEnumerator DoMulligan(Turn turn)
    {
        if (turn == Turn.Player)
        {
            Debug.Log("Open UI");
        }

        Debug.Log(name + " está eligiendo cartas para cambiar...");

        yield return new WaitUntil(() => readySetup); // Aquí puedes mostrar UI real
        //readySetup = false;
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

    public List<SO_Cards> GetGroup()
    {
        return GroupCards;
    }

    public List<SO_Cards> GetSpe()
    {
        return SpecialCards;
    }

    public void PlayCardToFromHand(int indexcard)
    {
        if (indexcard < 0 || indexcard >= HandCards.Count)
            return;

        var card = HandCards[indexcard];

        if (card == null)
            return;

        if (card.type == Card.Petroglyph)
        {
            if (Scr_Rules.FullGroup(GroupCards))
                return;
            
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
                GroupCards.Add(null);
            }
            if(MyTurn == Turn.Player)
            {
                table.coroutineQueue.Enqueue(cardManager.MoveCard(indexcard, insertIndex, card, CardZone.Hand, CardZone.Group, true, Turn.Player));
            }
            else
            {
                table.coroutineQueue.Enqueue(cardManager.MoveCard(indexcard, insertIndex, card, CardZone.Hand, CardZone.Group, true, Turn.AI));
            }
            
            GroupCards[insertIndex] = card;
            HandCards[indexcard] = null;
        }
        //Play Special Cards
        else
        {
            if (Scr_Rules.FullSpecial(SpecialCards))
                return;
            int insertIndex = -1;
            for (int i = 0; i < SpecialCards.Count; i++)
            {
                if (SpecialCards[i] == null)
                {
                    insertIndex = i;
                    break;
                }
            }

            if (insertIndex == -1)
            {
                insertIndex = SpecialCards.Count;
                SpecialCards.Add(null);
            }

            if (MyTurn == Turn.Player)
            {
                table.coroutineQueue.Enqueue(cardManager.MoveCard(indexcard, insertIndex, card, CardZone.Hand, CardZone.Special, true, Turn.Player));
            }
            else 
            {
                table.coroutineQueue.Enqueue(cardManager.MoveCard(indexcard, insertIndex, card, CardZone.Hand, CardZone.Special, true, Turn.AI));
            }


            HandCards[indexcard] = null;
            SpecialCards[insertIndex] = card;
            if (table.StartCard(card))
            {
                SpecialToD(card);
            }
            else
            {
                Debug.Log("No se activo la carta " + card.name);
            }
        }
        
        //HandCards[indexcard] = null;
        
    }
    public void PlayPetToHand(int indexcard)
    {
        if (indexcard < 0 || indexcard >= GroupCards.Count)
            return;

        if (Scr_Rules.FullHand(HandCards))
        {
            return;
        }

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

            if (MyTurn == Turn.Player)
            {
                table.coroutineQueue.Enqueue(cardManager.MoveCard(indexcard, insertIndex, card, CardZone.Group, CardZone.Hand, true, Turn.Player));
            }
            else
            {
                table.coroutineQueue.Enqueue(cardManager.MoveCard(indexcard, insertIndex, card, CardZone.Group, CardZone.Hand, false, Turn.AI));
            }
            
            HandCards[insertIndex] = card;
            GroupCards[indexcard] = null;
        }
    }
    
    public void SpecialToD(SO_Cards card)
    {
        for(int i = 0;i < SpecialCards.Count; i++)
        {
            if(SpecialCards[i] == card)
            {
                table.coroutineQueue.Enqueue(cardManager.MoveCard(i, 0, card, CardZone.Special, CardZone.HoleMaze, false, MyTurn));
                break;
            }
        }
        table.GetHole().Add(card);
        SpecialCards.Remove(card);

        
    }

    public bool DiscardGroup(SO_Cards card, int index)
    {
        table.coroutineQueue.Enqueue(cardManager.MoveCard(index, 0, card, CardZone.Group, CardZone.HoleMaze, false, MyTurn));
        table.GetHole().Add(card);
        GroupCards.Remove(card);
        return true;
    }

    public bool DiscardCardHand(SO_Cards card, int index)
    {
        table.coroutineQueue.Enqueue(cardManager.MoveCard(index, 0, card, CardZone.Hand, CardZone.HoleMaze, false, MyTurn));
        table.GetHole().Add(card);
        HandCards.Remove(card);
        return true;
    }

    public void PlaySpeToHand(int indexcard)
    {
        if (indexcard < 0 || indexcard >= SpecialCards.Count)
            return;

        if (Scr_Rules.FullHand(HandCards))
        {
            return;
        }
        var card = SpecialCards[indexcard];

        if (card != null && card.type != Card.Petroglyph)
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

            if (MyTurn == Turn.Player)
            {
                table.coroutineQueue.Enqueue(cardManager.MoveCard(indexcard, insertIndex, card, CardZone.Special, CardZone.Hand, true, Turn.Player));
            }
            else 
            {
                table.coroutineQueue.Enqueue(cardManager.MoveCard(indexcard, insertIndex, card, CardZone.Special, CardZone.Hand, false, Turn.AI));
            }
            
            HandCards[insertIndex] = card;
            SpecialCards[indexcard] = null;
        }
    }

    public bool OponentHand(SO_Cards card)
    {
        //No se mueve la carta
        if (Scr_Rules.FullHand(HandCards))
        {
            return false;
        }

        if (card != null)
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

            if (MyTurn == Turn.Player)
            {
                table.coroutineQueue.Enqueue(cardManager.MoveCard(0, insertIndex, card, CardZone.ToG, CardZone.Hand, true, Turn.Player));
            }
            else
            {
                table.coroutineQueue.Enqueue(cardManager.MoveCard(0, insertIndex, card, CardZone.ToG, CardZone.Hand, false, Turn.AI));
            }

            HandCards[insertIndex] = card;
            
        }
        return true;
    }

    public bool HoleToHand(SO_Cards card)
    {
        //No se mueve la carta
        if (Scr_Rules.FullHand(HandCards))
        {
            return false;
        }

        if (card != null)
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

            if (MyTurn == Turn.Player)
            {
                table.coroutineQueue.Enqueue(cardManager.MoveCard(0, insertIndex, card, CardZone.HoleMaze, CardZone.Hand, true, Turn.Player));
            }
            else
            {
                table.coroutineQueue.Enqueue(cardManager.MoveCard(0, insertIndex, card, CardZone.HoleMaze, CardZone.Hand, false, Turn.AI));
            }

            HandCards[insertIndex] = card;

        }
        return true;
    }

    //Apretar Cartas
    public void ClickHand(int index){
        if(block == false)
        {
            PlayCardToFromHand(index);
        }
        
    }

    public void ClickPet(int index)
    {
        if (block == false)
        {
            PlayPetToHand(index);

        }
    }

    public void ClickSpecial(int index)
    {
        if (block == false)
        {
            PlaySpeToHand(index);
        }
    }



    public void PetroComplete()
    {
        
        int indexcard = 0;
        foreach (var card in GroupCards)
        {
            if (MyTurn == Turn.Player)
            {
                table.coroutineQueue.Enqueue(cardManager.MoveCard(indexcard, 0, card, CardZone.Group, CardZone.HoleMaze, true, Turn.Player));
            }
            else
            {
                table.coroutineQueue.Enqueue(cardManager.MoveCard(indexcard, 0, card, CardZone.Group, CardZone.HoleMaze, false, Turn.AI));
            }
            
            indexcard++;
        }
        GroupCards.Clear();
    }

    public void AddPlayerPoints()
    {
        Poinst++;
    }

    public int GetPoints()
    {
        return Poinst;
    }

    
    //Faltan las 10 reglas de las cartas
    //Necesito las condiciones, que se muevan no es necesario
}
