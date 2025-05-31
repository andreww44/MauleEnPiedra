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
    [SerializeField] private CartaManager cardManager;
    [SerializeField] private int Poinst = 0;
    [SerializeField] bool readySetup = false;

    public bool IsReadySetup()
    {
        return readySetup;
    }

    void Start()
    {
    }
    void Update()
    {

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

    private void PlayCardToFromHand(int indexcard)
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

            StartCoroutine(cardManager.MoveCard(indexcard, insertIndex, card, CardZone.Hand, CardZone.Group));
            GroupCards[insertIndex] = card;
        }
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

            StartCoroutine(cardManager.MoveCard(indexcard, insertIndex, card, CardZone.Hand, CardZone.Special));
            SpecialCards[insertIndex] = card;
        }

        HandCards[indexcard] = null;
    }
    private void PlayPetToHand(int indexcard)
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

            StartCoroutine(cardManager.MoveCard(indexcard, insertIndex, card, CardZone.Group, CardZone.Hand));
            HandCards[insertIndex] = card;
            GroupCards[indexcard] = null;
        }
    }
    private void PlaySpeToHand(int indexcard)
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

            StartCoroutine(cardManager.MoveCard(indexcard, insertIndex, card, CardZone.Special, CardZone.Hand));
            HandCards[insertIndex] = card;
            SpecialCards[indexcard] = null;
        }
    }

    public void ClickHand(int index){
        PlayCardToFromHand(index);
    }

    public void ClickPet(int index)
    {
        PlayPetToHand(index);
    }

    public void ClickSpecial(int index)
    {
        PlaySpeToHand(index);
    }

    public void PetroComplete()
    {
        
        int indexcard = 0;
        foreach (var card in GroupCards)
        {
            StartCoroutine(cardManager.MoveCard(indexcard, 0, card, CardZone.Group, CardZone.HoleMaze));
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

}
