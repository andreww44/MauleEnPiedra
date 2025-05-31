using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//Esta clase se encarga de toda la logica de revisar si se cumples las reglas

public static class Scr_Rules
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public static void CompareCards()
    {

    }

    public static bool FullHand(int numberOfCards)
    {
        if(numberOfCards < 6 ){ return false; }
        return true;
    }

    public static bool FullHand(List<SO_Cards> list)
    {
        if (list == null) return false;

        // Si tiene menos de 6 cartas, no está llena
        if (list.Count < 6) return false;

        // Si alguna carta es null, tampoco está llena
        foreach (SO_Cards card in list)
        {
            if (card == null) return false;
        }

        return true; // Si tiene 6 cartas no nulas
    }

    public static bool FullGroup(List<SO_Cards> list)
    {
        if (list == null) return false;

        // Si tiene menos de 6 cartas, no está llena
        if (list.Count < 3) return false;

        // Si alguna carta es null, tampoco está llena
        foreach (SO_Cards card in list)
        {
            if (card == null) return false;
        }

        return true; // Si tiene 6 cartas no nulas
    }

    public static bool FullSpecial(List<SO_Cards> list)
    {
        if (list == null) return false;

        // Si tiene menos de 6 cartas, no está llena
        if (list.Count < 2) return false;

        // Si alguna carta es null, tampoco está llena
        foreach (SO_Cards card in list)
        {
            if (card == null) return false;
        }

        return true; // Si tiene 6 cartas no nulas
    }



    public static bool PetroComplete(List<SO_Cards> list)
    {
        if (list == null || list.Count != 3)
            return false;

        if (list.Any(card => card == null))
            return false;

        int code = list[0].Code;

        return list.All(card => card.Code == code);
    }

    public static bool PetroInComplete(List<SO_Cards> list)
    {
        if (list == null || list.Count != 3)
            return false;

        if (list.Any(card => card == null))
            return false;



        return true;
    }
}
