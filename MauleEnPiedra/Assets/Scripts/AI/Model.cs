using System.Collections.Generic;
using UnityEngine;


namespace MC.Modelo
{
    
    
    //public enum
    public enum Zona { Zona1, Zona2, Zona3 }
    public enum PartePetroglifo { Superior = 1, Media = 2, Inferior = 3 }

    public class CardMC
    {
        public int Id;
        public string Name;
        public Card Type;
        public Zona Zona;
        public PartePetroglifo Parte;

        public CardMC(int id, string name, Card type, Zona zona = Zona.Zona1, PartePetroglifo parte = PartePetroglifo.Superior)
        {
            Id = id;
            Name = name;
            Type = type;
            Zona = zona;
            Parte = parte;
        }


    }

    public class PlayerState
    {
        public List<CardMC> Hand = new();
        public List<CardMC> ZoneArmado = new();
        public List<CardMC> ZoneAccion = new();
        public int Puntos = 0;
        public int TurnosProtegido = 0;
        public bool MuseoVirtual = false;
        public bool PierdeTurno = false;

        public PlayerState Clone()
        {
            return new PlayerState
            {
                Hand = new List<CardMC>(Hand),
                ZoneArmado = new List<CardMC>(ZoneArmado),
                ZoneAccion = new List<CardMC>(ZoneAccion),
                Puntos = Puntos,
                TurnosProtegido = TurnosProtegido,
                PierdeTurno = PierdeTurno
            };
        }
        public PlayerState()
        {

        }

        public PlayerState(List<CardMC> hand, List<CardMC> group, List<CardMC> special, int point, int protect, bool museo, bool lost) 
        {
            Hand = hand;
            ZoneArmado = group;
            ZoneAccion = special;
            Puntos = point;
            TurnosProtegido = protect;
            PierdeTurno = lost;
        }
    }

    //Estado del juego esto es lo que se simula
    public class GameState
    {
        public List<CardMC> Deck = new();
        public List<CardMC> DiscardPile = new();
        public PlayerState Player1 = new();
        public PlayerState Player2 = new();
        public bool IsPlayer1Turn = true;
        public bool JuegoTerminado = false;

        public GameState Clone()
        {
            return new GameState
            {
                Deck = new List<CardMC>(Deck),
                DiscardPile = new List<CardMC>(DiscardPile),
                Player1 = Player1.Clone(),
                Player2 = Player2.Clone(),
                IsPlayer1Turn = IsPlayer1Turn,
                JuegoTerminado = JuegoTerminado
            };
        }

        public GameState() { }

        public GameState(List<CardMC> Maze, List<CardMC> Discard, Turn currenTurn, PlayerState player1, PlayerState player2, bool ended)
        {

            Deck = new List<CardMC>(Maze);
            DiscardPile = new List<CardMC>(DiscardPile);
            Player1 = player1;
            Player2 = player2;
            IsPlayer1Turn = currenTurn == Turn.Player ? true : false;
            JuegoTerminado = ended;
        }

    }

    

}

