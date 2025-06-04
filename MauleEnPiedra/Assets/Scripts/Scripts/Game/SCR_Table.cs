using System.Collections.Generic;
using UnityEngine;

public class SCR_Table : MonoBehaviour
{
    public static SCR_Table Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    [Header("---- Jugadores ----")]
    public SCR_Player Player;
    public SCR_Player Ai;

    [Header("---- Mazo y Pozo ----")]
    public List<SO_Cards> DeckCard = new List<SO_Cards>();
    public List<SO_Cards> HoleDeck = new List<SO_Cards>();

    [Header("---- Estado de Partida ----")]
    public Turn currentTurn;
    public GameState gameState;
    private bool hasStartedTurn;

    [Header("---- UI Mulligan ----")]
    public GameObject UICardsMulligan;
    public GameObject UIWaitingOpponent;

    private System.Collections.IEnumerator Start()
    {
        gameState = GameState.Setup;
        UICardsMulligan.SetActive(true);
        UIWaitingOpponent.SetActive(false);

        ShuffleDeck();
        yield return StartCoroutine(GameSetup());
    }

    private void Update()
    {
        if (gameState == GameState.InGame)
        {
            if (!hasStartedTurn)
            {
                hasStartedTurn = true;
                SCR_Player actual = (currentTurn == Turn.Player) ? Player : Ai;

                if (actual.skipNextTurn)
                {
                    actual.skipNextTurn = false;
                    EndCurrentPlayerTurn();
                    return;
                }

                int draws = 1 + actual.extraDrawsNextTurn;
                for (int i = 0; i < draws; i++)
                {
                    DrawRandomCard(actual == Player);
                }
                actual.extraDrawsNextTurn = 0;

                if (currentTurn == Turn.AI)
                {
                    StartCoroutine(PetAI());
                }
            }
        }
        else if (gameState == GameState.EndGame)
        {
            // Juego terminado: aquí puedes mostrar pantalla de victoria, etc.
        }
    }

    private System.Collections.IEnumerator GameSetup()
    {
        currentTurn = (Turn)Random.Range(0, 2);
        Debug.Log("Quien inicia: " + currentTurn);

        UICardsMulligan.SetActive(true);
        yield return StartCoroutine(Player.DoMulligan(Turn.Player));

        yield return new WaitForSeconds(0.5f);
        Ai.CardsReady();

        UICardsMulligan.SetActive(false);

        for (int i = 0; i < 5; i++)
        {
            DrawRandomCard(true);
            DrawRandomCard(false);
            yield return new WaitForSeconds(0.1f);
        }

        gameState = GameState.InGame;
        hasStartedTurn = false;
    }

    public void ShuffleDeck()
    {
        for (int i = 0; i < DeckCard.Count; i++)
        {
            var temp = DeckCard[i];
            int rand = Random.Range(i, DeckCard.Count);
            DeckCard[i] = DeckCard[rand];
            DeckCard[rand] = temp;
        }
    }

    public void DrawRandomCard(bool playerIsHuman)
    {
        if (DeckCard.Count == 0)
        {
            DeckCard.AddRange(HoleDeck);
            HoleDeck.Clear();
            ShuffleDeck();
        }

        SO_Cards drawn = DeckCard[0];
        DeckCard.RemoveAt(0);

        if (playerIsHuman)
        {
            int destinoIndex = Player.HandCards.Count;
            Player.HandCards.Add(drawn);
            StartCoroutine(AnimateDrawToHand(drawn, true, destinoIndex));
        }
        else
        {
            int destinoIndex = Ai.HandCards.Count;
            Ai.HandCards.Add(drawn);
            StartCoroutine(AnimateDrawToHand(drawn, false, destinoIndex));
        }
    }

    public void DiscardToHole(SO_Cards carta)
    {
        HoleDeck.Add(carta);
    }

    private System.Collections.IEnumerator AnimateDrawToHand(SO_Cards drawn, bool playerIsHuman, int destinoIndex)
    {
        // yield return StartCoroutine(cartaManager.MoveCardFromDeckToHand(drawn, destinoIndex, playerIsHuman));
        yield return null;
    }

    public System.Collections.IEnumerator AnimateDrawFromHole(SO_Cards carta, bool playerIsHuman)
    {
        yield return null;
    }

    public void NotifyPetroComplete(SCR_Player who)
    {
        Debug.Log($"{(who == Player ? "Jugador" : "IA")} completó un petroglifo. Puntos: {who.Points}");
        // despues actualizar puntaje, reproducir un efecto o no se
    }

    public void NotifyPetroIncomplete(SCR_Player who)
    {
        Debug.Log($"{(who == Player ? "Jugador" : "IA")} tenía 3 fragmentos inválidos; se descartan sin puntuar.");
    }

    public void ShowOpponentHandTemporarily(SCR_Player opponent)
    {
        StartCoroutine(RutinaMostrarManoOponente(opponent));
    }

    private System.Collections.IEnumerator RutinaMostrarManoOponente(SCR_Player opponent)
    {
        UIWaitingOpponent.SetActive(true);
        yield return new WaitForSeconds(2f);
        UIWaitingOpponent.SetActive(false);
    }

    private System.Collections.IEnumerator PetAI()
    {
        yield return new WaitForSeconds(0.5f);

        if (Scr_Rules.PetroComplete(Ai.GroupCards) && !Ai.ProteccionActiva)
        {
            Ai.CheckCombineOrDiscard();
            yield return new WaitForSeconds(0.5f);
            EndCurrentPlayerTurn();
            yield break;
        }

        foreach (var carta in Ai.HandCards)
        {
            if (carta.type == Card.Petroglyph)
            {
                int idx = Ai.HandCards.IndexOf(carta);
                Ai.ClickHand(idx, Turn.AI);
                yield return new WaitForSeconds(0.5f);
                break;
            }
        }

        yield return new WaitForSeconds(0.5f);

        if (Player.GroupCards.Count == 2)
        {
            var amenaza = Ai.HandCards.Find(c => c.type == Card.Threath && Scr_Rules.CanPlayThreat(c, Player));
            if (amenaza != null)
            {
                int idxA = Ai.HandCards.IndexOf(amenaza);
                Ai.ClickHand(idxA, Turn.AI);
                yield return new WaitForSeconds(0.5f);
            }
        }

        if (Scr_Rules.PetroComplete(Ai.GroupCards) && !Ai.ProteccionActiva)
        {
            Ai.CheckCombineOrDiscard();
            yield return new WaitForSeconds(0.5f);
            EndCurrentPlayerTurn();
            yield break;
        }

        if (Ai.HandCards.Count > 0)
        {
            var descarta = Ai.HandCards[0];
            Ai.HandCards.RemoveAt(0);
            DiscardToHole(descarta);
        }

        yield return new WaitForSeconds(0.5f);
        EndCurrentPlayerTurn();
    }

    public void EndCurrentPlayerTurn()
    {
        SCR_Player actual = (currentTurn == Turn.Player) ? Player : Ai;
        actual.CheckCombineOrDiscard();

        Player.OnTurnEnd();
        Ai.OnTurnEnd();

        if (Player.Points >= 3 || Ai.Points >= 3)
        {
            gameState = GameState.EndGame;
            Debug.Log((Player.Points >= 3 ? "Player" : "AI") + " gana la partida!");
            return;
        }

        currentTurn = (currentTurn == Turn.Player) ? Turn.AI : Turn.Player;
        hasStartedTurn = false;
    }
    public void DrawSpecificCard(Turn playerTurn, SO_Cards soCard)
    {
        // Si la carta existe en el mazo la quita
        if (DeckCard.Contains(soCard))
        {
            DeckCard.Remove(soCard);
        }
        else
        {
            Debug.LogWarning($"DrawSpecificCard: La carta {soCard.name} no estaba en DeckCard.");
        }

        // La agrega a la mano del jugador correspondiente
        if (playerTurn == Turn.Player)
        {
            Player.HandCards.Add(soCard);
            // Aquí podrías instanciar el prefab de carta en la UI:
            // por ejemplo:
            // GameObject go = Instantiate(cardPrefab, HandAreaTransform);
            // go.GetComponent<UI_CardRepresentation>().SetCard(soCard);
        }
        else
        {
            Ai.HandCards.Add(soCard);
            // Si deseas mostrar la carta en pantalla para la IA, haz algo similar:
            // GameObject go = Instantiate(cardPrefab, AiHandAreaTransform);
            // go.GetComponent<UI_CardRepresentation>().SetCard(soCard);
        }
    }

}
