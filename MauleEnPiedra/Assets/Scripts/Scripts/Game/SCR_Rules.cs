using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Esta clase estática contiene todas las funciones de validación de reglas.
// Se encarga de determinar si la mano está llena, si hay petroglifo completo/incompleto,
// y si se pueden jugar amenazas o protecciones en cada contexto.
public static class Scr_Rules
{
    /// <summary>
    /// Verifica si la mano del jugador está “llena” (es decir, si tiene 6 o más cartas).
    /// </summary>
    /// <param name="numberOfCards">Cantidad de cartas que tiene actualmente el jugador.</param>
    /// <returns>True si numberOfCards >= 6, false en caso contrario.</returns>
    public static bool FullHand(int numberOfCards)
    {
        return numberOfCards >= 6;
    }

    /// <summary>
    /// Verifica si las 3 cartas en lista forman un petroglifo completo:
    /// - La lista no es nula y tiene exactamente 3 cartas.
    /// - Ninguna carta es null.
    /// - Las tres cartas comparten el mismo “prefijo” de código (Code/10), 
    ///   de modo que están en la misma región (= región * 10 + parte).
    /// - Y, además, están las 3 partes distintas (parte 1 = Superior, 2 = Medio, 3 = Inferior).
    /// 
    /// NOTA: Se asume que cada `SO_Cards` de petroglifo tiene:
    ///   - `Code` diseñado como “(IdRegión)*10 + (IdParte)”, donde IdParte ∈ {1,2,3}.
    ///   - `type == Card.Petroglyph`.
    /// </summary>
    public static bool PetroComplete(List<SO_Cards> list)
    {
        if (list == null || list.Count != 3)
            return false;
        if (list.Any(card => card == null))
            return false;
        // Todas deben ser de tipo Petroglyph
        if (list.Any(c => c.type != Card.Petroglyph))
            return false;

        // Verificar que comparten el mismo “prefijo de código” (región):
        int codigoBase = list[0].Code / 10;
        foreach (var carta in list)
        {
            if (carta.Code / 10 != codigoBase)
                return false;
        }

        // Verificar que están las tres partes: 1 (Superior), 2 (Medio), 3 (Inferior)
        var partes = new HashSet<int>(list.Select(c => c.Code % 10));
        if (partes.Contains(1) && partes.Contains(2) && partes.Contains(3))
            return true;

        return false;
    }

    /// <summary>
    /// Verifica si hay 3 cartas en mesa pero NO forman un petroglifo válido.
    /// Sólo comprueba que la lista tenga exactamente 3 cartas no-nulas.  
    /// </summary>
    public static bool PetroInComplete(List<SO_Cards> list)
    {
        if (list == null || list.Count != 3)
            return false;
        if (list.Any(card => card == null))
            return false;

        return true;
    }

    public static bool CanPlayThreat(SO_Cards threatCard, SCR_Player target)
    {
        if (threatCard == null || threatCard.type != Card.Threath)
            return false;

        switch (threatCard.Code)
        {
            case 201: // Vandalismo
                return target.GroupCards.Count > 0;

            case 202: // Erosión
                return target.HandCards.Count > 0;

            case 203: // Abandono
                return true;

            case 204: // Desarrollo Urbano
                return target.GroupCards.Count > 0 && !target.ProteccionActiva;

            default:
                return false;
        }
    }

    public static bool CanPlayProtection(SO_Cards protectCard, SCR_Player self)
    {
        if (protectCard == null || protectCard.type != Card.Protect)
            return false;

        switch (protectCard.Code)
        {
            case 301: // Educación Patrimonial
                return !self.ProteccionActiva;

            case 302: // Museo Virtual
                return !self.MuseoVirtualActiva;

            case 303: // Ley de Patrimonio
                return true;

            default:
                return false;
        }
    }
}
