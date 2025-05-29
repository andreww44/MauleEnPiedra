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
        if(numberOfCards < 6){ return false; }
        return true;
    }

    public static bool PetroComplete()
    {
        return false;
    }
}
