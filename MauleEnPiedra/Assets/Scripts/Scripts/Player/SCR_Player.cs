using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SCR_Player : MonoBehaviour
{
    [Header("**** Listas internas de cartas ****")]
    [Tooltip("Cartas que el jugador tiene en mano.")]
    [SerializeField] public List<SO_Cards> HandCards = new List<SO_Cards>();

    [Tooltip("Cartas de tipo especial (no petroglifo ni amenaza ni protección).")]
    [SerializeField] public List<SO_Cards> SpecialCards = new List<SO_Cards>();

    [Tooltip("Fragmentos de petroglifo que el jugador ha colocado en mesa.")]
    [SerializeField] public List<SO_Cards> GroupCards = new List<SO_Cards>();

    [Header("**** Referencias a otros componentes ****")]
    [Tooltip("Referencia al componente que mueve/animar cartas en UI.")]
    [SerializeField] private CartaManager cardManager;

    [Tooltip("Cantidad de puntos (petroglifos completos) que lleva el jugador.")]
    [SerializeField] private int Poinst = 0;

    [Header("**** Estados especiales del jugador ****")]
    [Tooltip("¿Está activo el efecto de Educación Patrimonial (protege 2 turnos)?")]
    [SerializeField] private bool proteccionActiva = false;

    [Tooltip("Cuántos turnos de protección le restan.")]
    [SerializeField] private int turnosProteccionRestantes = 0;

    [Tooltip("¿Tiene activo Museo Virtual (anula la próxima amenaza)?")]
    [SerializeField] private bool museoVirtualActiva = false;

    [Tooltip("Cantidad de cartas extra que robará en su próximo turno por Ladrón de Antigüedades.")]
    [SerializeField] private int _extraDrawsNextTurn = 0;
    public int extraDrawsNextTurn
    {
        get { return _extraDrawsNextTurn; }
        set { _extraDrawsNextTurn = value; }
    }

    [Tooltip("Flag: si el jugador perdió el turno (por carta Abandono), salta su siguiente turno.")]
    [SerializeField] public bool skipNextTurn = false;

    [Header("**** Estado de Preparación (Mulligan) ****")]
    [Tooltip("Indica si el jugador ya terminó su fase de mulligan en Setup.")]
    [SerializeField] private bool readySetup = false;

    public bool ProteccionActiva => proteccionActiva;
    public bool MuseoVirtualActiva => museoVirtualActiva;
    public int ExtraDrawsNextTurn => extraDrawsNextTurn;
    public int Points => Poinst;
    public bool IsReadySetup() => readySetup;
    public IEnumerator DoMulligan(Turn who)
    {
        yield return null;
    }

    public void CardsReady()
    {
        readySetup = true;
    }

    public void ResetSetupState()
    {
        readySetup = false;
    }

    public void ClickHand(int indexcard, Turn currentTurn)
    {
        if (indexcard < 0 || indexcard >= HandCards.Count)
            return;

        var card = HandCards[indexcard];
        if (card == null)
            return;

        switch (card.type)
        {
            case Card.Petroglyph:

                PlayPetroglyph(indexcard, currentTurn);
                break;

            case Card.Threath:

                var opponent = FindOpponent(currentTurn);
                if (Scr_Rules.CanPlayThreat(card, opponent))
                    PlayThreat(indexcard, currentTurn);
                break;

            case Card.Protect:

                if (Scr_Rules.CanPlayProtection(card, this))
                    PlayProtection(indexcard, currentTurn);
                break;

            case Card.Special:

                PlaySpecial(indexcard, currentTurn);
                break;
        }
    }


    public void ClickPet(int indexcard, Turn currentTurn)
    {
        if (indexcard < 0 || indexcard >= GroupCards.Count)
            return;

        var card = GroupCards[indexcard];
        if (card == null)
            return;

        GroupCards.RemoveAt(indexcard);

        bool isPlayer = (currentTurn == Turn.Player);
        StartCoroutine(cardManager.MoveCard(indexcard, HandCards.Count, card, CardZone.Group, CardZone.Hand, isPlayer));
        HandCards.Add(card);
    }

    private void PlayPetroglyph(int indexcard, Turn currentTurn)
    {
        var card = HandCards[indexcard];
        if (card == null || card.type != Card.Petroglyph)
            return;

        bool isPlayer = (currentTurn == Turn.Player);
        StartCoroutine(cardManager.MoveCard(indexcard, GroupCards.Count, card, CardZone.Hand, CardZone.Group, isPlayer));

        HandCards[indexcard] = null;
        HandCards.RemoveAll(c => c == null);
        GroupCards.Add(card);
    }
    private void PlayThreat(int indexcard, Turn currentTurn)
    {
        var card = HandCards[indexcard];
        if (card == null || card.type != Card.Threath)
            return;

        var opponent = FindOpponent(currentTurn);

        bool isPlayer = (currentTurn == Turn.Player);
        StartCoroutine(cardManager.MoveCard(indexcard, 0, card, CardZone.Hand, CardZone.HoleMaze, isPlayer));

        HandCards[indexcard] = null;
        HandCards.RemoveAll(c => c == null);

        ApplyThreatEffect(card, opponent);
    }

    private void ApplyThreatEffect(SO_Cards threatCard, SCR_Player target)
    {

        if (target.museoVirtualActiva)
        {
            target.museoVirtualActiva = false;
            return;
        }

        switch (threatCard.Code)
        {
            case 201: // Vandalismo: Cambia una carta en zona de armado del contrario por una del mazo
                if (target.GroupCards.Count > 0)
                {
                    int rand = Random.Range(0, target.GroupCards.Count);
                    var toRemove = target.GroupCards[rand];
                    target.GroupCards.RemoveAt(rand);
                    SCR_Table.Instance.DiscardToHole(toRemove); 
                    SCR_Table.Instance.DrawRandomCard(target == SCR_Table.Instance.Player);
                }
                break;

            case 202: // Erosión: El oponente debe descartar una carta de la mano y robar otra
                if (target.HandCards.Count > 0)
                {
                    int rand2 = Random.Range(0, target.HandCards.Count);
                    var toRemoveHand = target.HandCards[rand2];
                    target.HandCards.RemoveAt(rand2);
                    SCR_Table.Instance.DiscardToHole(toRemoveHand);
                    SCR_Table.Instance.DrawRandomCard(target == SCR_Table.Instance.Player);
                }
                break;

            case 203: // Abandono: El oponente pierde el siguiente turno
                target.skipNextTurn = true;
                break;

            case 204: // Desarrollo Urbano: Elimina un fragmento en mesa si no está protegido
                if (target.GroupCards.Count > 0 && !target.proteccionActiva)
                {
                    int idx = Random.Range(0, target.GroupCards.Count);
                    var toRemove2 = target.GroupCards[idx];
                    target.GroupCards.RemoveAt(idx);
                    SCR_Table.Instance.DiscardToHole(toRemove2);
                }
                break;
        }
    }

    private void PlayProtection(int indexcard, Turn currentTurn)
    {
        var card = HandCards[indexcard];
        if (card == null || card.type != Card.Protect)
            return;

        bool isPlayer = (currentTurn == Turn.Player);
        StartCoroutine(cardManager.MoveCard(indexcard, 0, card, CardZone.Hand, CardZone.HoleMaze, isPlayer));

        HandCards[indexcard] = null;
        HandCards.RemoveAll(c => c == null);

        ApplyProtectionEffect(card);
    }

    private void ApplyProtectionEffect(SO_Cards protectCard)
    {
        switch (protectCard.Code)
        {
            case 301: // Educación Patrimonial
                proteccionActiva = true;
                turnosProteccionRestantes = 2;
                break;

            case 302: // Museo Virtual
                museoVirtualActiva = true;
                break;

            case 303: // Ley de Patrimonio
                if (SCR_Table.Instance.HoleDeck.Count > 0)
                {
                    int rand = Random.Range(0, SCR_Table.Instance.HoleDeck.Count);
                    var rec = SCR_Table.Instance.HoleDeck[rand];
                    SCR_Table.Instance.HoleDeck.RemoveAt(rand);
                    HandCards.Add(rec);
                    SCR_Table.Instance.AnimateDrawFromHole(rec, this == SCR_Table.Instance.Player);
                }
                break;
        }
    }
    private void PlaySpecial(int indexcard, Turn currentTurn)
    {
        var card = HandCards[indexcard];
        if (card == null || card.type != Card.Special)
            return;

        var opponent = FindOpponent(currentTurn);

        bool isPlayer = (currentTurn == Turn.Player);
        StartCoroutine(cardManager.MoveCard(indexcard, 0, card, CardZone.Hand, CardZone.HoleMaze, isPlayer));

        HandCards[indexcard] = null;
        HandCards.RemoveAll(c => c == null);

        ApplySpecialEffect(card, opponent);
    }

    private void ApplySpecialEffect(SO_Cards specialCard, SCR_Player opponent)
    {
        switch (specialCard.Code)
        {
            case 401: // Intercambio Cultural
                if (HandCards.Count > 0 && opponent.HandCards.Count > 0)
                {
                    int i1 = Random.Range(0, HandCards.Count);
                    int i2 = Random.Range(0, opponent.HandCards.Count);
                    var mine = HandCards[i1];
                    var theirs = opponent.HandCards[i2];
                    HandCards[i1] = theirs;
                    opponent.HandCards[i2] = mine;
                }
                break;

            case 402: // Investigador
                SCR_Table.Instance.ShowOpponentHandTemporarily(opponent);
                break;

            case 403: // Ladrón de Antigüedades
                extraDrawsNextTurn += 2;
                break;
        }
    }
    public void CheckCombineOrDiscard()
    {

        if (GroupCards.Count == 3 && !proteccionActiva)
        {
            if (Scr_Rules.PetroComplete(GroupCards))
            {
                foreach (var carta in GroupCards)
                    SCR_Table.Instance.DiscardToHole(carta);
                GroupCards.Clear();

                AddPlayerPoints();
                SCR_Table.Instance.NotifyPetroComplete(this);

                StartCoroutine(RobUntilHandFull());
            }
            else if (Scr_Rules.PetroInComplete(GroupCards))
            {
                foreach (var carta in GroupCards)
                    SCR_Table.Instance.DiscardToHole(carta);
                GroupCards.Clear();
                SCR_Table.Instance.NotifyPetroIncomplete(this);

                StartCoroutine(RobUntilHandFull());
            }
        }
    }

    public void AddPlayerPoints()
    {
        Poinst += 1;
    }


    private IEnumerator RobUntilHandFull()
    {

        yield return null;

        int meta = 5 + extraDrawsNextTurn;
        while (HandCards.Count < meta)
        {
            SCR_Table.Instance.DrawRandomCard(this == SCR_Table.Instance.Player);
            yield return new WaitForSeconds(0.1f);
        }
        extraDrawsNextTurn = 0;
    }
    public void OnTurnEnd()
    {
        if (turnosProteccionRestantes > 0)
        {
            turnosProteccionRestantes--;
            if (turnosProteccionRestantes == 0)
                proteccionActiva = false;
        }
    }

    private SCR_Player FindOpponent(Turn currentTurn)
    {
        if (currentTurn == Turn.Player)
            return SCR_Table.Instance.Ai;
        else
            return SCR_Table.Instance.Player;
    }
    public void OnClickHand_Player(int indexcard)
    {
        ClickHand(indexcard, Turn.Player);
    }
}
