using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using MC.Modelo;
using System.Linq;
using Microsoft.VisualBasic;
using System.Diagnostics;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;


public static class MonteCarlo
    {
        public static (int puntaje, int mejorJugada) MonteCarloTS(GameState estado, int simulaciones, int profundidadMax, int profundidadActual = 0)
        {
            if (estado.JuegoTerminado || profundidadActual >= profundidadMax)
            {
                int e = Evaluar(estado, false);
                return (e, -1);
            }

            var acciones = new List<Func<GameState, GameState>>();
            for (int i = 0; i < estado.Player2.Hand.Count; i++)
            {
                int index = i;
                acciones.Add(s => {
                    JugarCartaEn(s.Player2, s.Player1, s, index);
                    return s;
                });
            };

            int mejorPuntaje = int.MinValue;
            int mejorIndice = 0;

            for (int i = 0; i < acciones.Count; i++)
            {
                int puntajeTotal = 0;

                for (int j = 0; j < simulaciones; j++)
                {
                    var copia = estado.Clone();
                    var bot = copia.IsPlayer1Turn ? copia.Player2 : copia.Player1;
                    RobarCarta(bot, copia);
                    copia = acciones[i](copia);

                    copia = SimularTurno(copia);

                    // Llamada recursiva con profundidad controlada
                    puntajeTotal += MonteCarloTS(copia, simulaciones, profundidadMax, profundidadActual + 1).puntaje;
                }

                if (puntajeTotal > mejorPuntaje)
                {
                    mejorPuntaje = puntajeTotal;
                    mejorIndice = i;
                }
            }

            if (profundidadActual == 0)
                return (mejorPuntaje, mejorIndice);

            // En llamadas recursivas, solo interesa el puntaje
            return (mejorPuntaje, -1);
        }

        //Acciones que hacer La IA luego de evaluar esta parte es importante porque debe entender que accion es la mejor para hacer
        public static GameState AplicarAccion(int index, GameState estado)
        {

            JugarCartaEn(estado.Player2, estado.Player1, estado, index);
            return estado;
        }

        //Acciones y respuesta
        private static void RobarCarta(PlayerState jugador, GameState estado)
        {
            if (estado.Deck.Count == 0)
                estado.Deck = estado.DiscardPile.OrderBy(x => Guid.NewGuid()).ToList();
            if (estado.Deck.Count > 0)
                jugador.Hand.Add(estado.Deck[0]);
        }

        private static bool JugarCartaEn(PlayerState bot, PlayerState oponente, GameState estado, int posiciondelacarta)
        {
            if (bot.Hand.Count == 0) return false;
            if (posiciondelacarta >= bot.Hand.Count)
            {
                return false;
            }

            var carta = bot.Hand[posiciondelacarta];
            bot.Hand.RemoveAt(posiciondelacarta);

            //Aqui Estan las acciones realizadas
            if (carta.Type == Card.Threath)
            {
                if (oponente.MuseoVirtual == true)
                {
                    //El oponente tiene museo virtual
                    estado.DiscardPile.Add(carta);
                    return true;
                }
                if (carta.Id == 0)//Vandalismo
                {
                    //Intercambio de carta 
                    if (oponente.ZoneArmado.Count > 0 && estado.Deck.Count > 0)
                    {
                    // Console.WriteLine("Vandalismo");
                        var rng = new System.Random();
                        var aleatoria = rng.Next(oponente.ZoneArmado.Count);

                        
                        var reemplazada = oponente.ZoneArmado[aleatoria];
                        oponente.ZoneArmado.RemoveAt(aleatoria);
                        estado.DiscardPile.Add(reemplazada);

                        var nueva = estado.Deck[0];
                        estado.Deck.RemoveAt(0);
                        oponente.ZoneArmado.Add(nueva);
                        estado.DiscardPile.Add(carta);
                        return true;
                    }
                    estado.DiscardPile.Add(carta);
                    return false;
                }
                if (carta.Id == 1)//Erosion
                {
                    //Console.WriteLine("Eorsion");
                    //Elimina una carta de la mano al pozo
                    if (bot.Hand.Count > 0 && estado.Deck.Count > 0)
                    {
                        var rng = new System.Random();
                        var aleatoria = rng.Next(bot.Hand.Count);

                        var descartada = bot.Hand[aleatoria];
                        bot.Hand.RemoveAt(aleatoria);
                        estado.DiscardPile.Add(descartada);

                        var nueva = estado.Deck[0];
                        estado.Deck.RemoveAt(0);
                        bot.Hand.Add(nueva);
                        estado.DiscardPile.Add(carta);
                        return true;
                    }
                    estado.DiscardPile.Add(carta);
                    return false;
                }
                if (carta.Id == 2)//Abandono
                {
                    //Hace al oponente perder turno
                    oponente.PierdeTurno = true;
                    estado.DiscardPile.Add(carta);
                    return true;
                    //Console.WriteLine("Perdida de turno");
                }
                if (carta.Id == 3)//Desarrollo urbano
                {
                    //Elimina una carta aleatoria del mazo de armado del contrario
                    //Console.WriteLine("Desarrollo");
                    if (oponente.TurnosProtegido <= 0 && oponente.ZoneArmado.Count > 0)
                    {
                        
                        var rng = new System.Random();
                        var aleatoria = rng.Next(oponente.ZoneArmado.Count);

                        var eliminada = oponente.ZoneArmado[aleatoria];
                        oponente.ZoneArmado.RemoveAt(aleatoria);
                        estado.DiscardPile.Add(eliminada);
                        estado.DiscardPile.Add(carta);
                        return true;
                    }
                    estado.DiscardPile.Add(carta);
                    return false;
                }
                //
            }
            else if (carta.Type == Card.Protect)
            {
                if (carta.Id == 4) // Educacion patrimonial
                {
                    //Console.WriteLine("Edu Pat");
                    bot.TurnosProtegido = 1;
                    estado.DiscardPile.Add(carta);
                    return true;
                }
                if (carta.Id == 5) //Museo
                {
                    //Console.WriteLine("Museo");
                    bot.MuseoVirtual = true;
                    estado.DiscardPile.Add(carta);
                    return true;
                }
                if (carta.Id == 6)//Ley de patrimonio
                {
                    //Console.WriteLine("Ley Pat");
                    var ultima = estado.DiscardPile.LastOrDefault(c => c.Type == Card.Petroglyph);
                    if (ultima != null)
                    {
                        estado.DiscardPile.Remove(ultima);
                        bot.Hand.Add(ultima);
                        estado.DiscardPile.Add(carta);
                        return true;
                    }
                    estado.DiscardPile.Add(carta);
                    return false;
                }
            }
            else if (carta.Type == Card.Special)
            {
                if (carta.Id == 7)//Intecarmcio
                {
                    //Console.WriteLine("Intercambio");
                    

                    var rng = new System.Random();
                    var mia = rng.Next(bot.Hand.Count);
                    var suya = rng.Next(oponente.Hand.Count);

                    var temp = bot.Hand[mia];
                    bot.Hand[mia] = oponente.Hand[suya];
                    oponente.Hand[suya] = temp;
                    estado.DiscardPile.Add(carta);
                    return true;
                }
                if (carta.Id == 8)//Investigador
                {
                    //Console.WriteLine("Investigador");
                    if (estado.Deck.Count > 0)
                    {
                        var nueva = estado.Deck[0];
                        estado.Deck.RemoveAt(0);
                        bot.Hand.Add(nueva);
                        estado.DiscardPile.Add(carta);
                        return true;
                    }
                    estado.DiscardPile.Add(carta);
                    return false;
                }
                if (carta.Id == 9)//Ladron
                {
                    //Console.WriteLine("Ladron");
                    if (oponente.Hand.Count > 0)
                    {
                        var rand = new System.Random();
                        int index = rand.Next(oponente.Hand.Count);
                        var cartaRobada = oponente.Hand[index];
                        oponente.Hand.RemoveAt(index);
                        bot.Hand.Add(cartaRobada);
                        estado.DiscardPile.Add(carta);
                        return true;
                    }
                    estado.DiscardPile.Add(carta);
                    return false;
                }
            }
            else if (carta.Type == Card.Petroglyph)
            {
                //Console.WriteLine("Petro");
                bot.ZoneArmado.Add(carta);

                //Verifica que sean tres pero no estoy seguro de que hace
                var grupos = bot.ZoneArmado.GroupBy(c => c.Zona).Where(g => g.Select(c => c.Parte).Distinct().Count() == 3);

                foreach (var grupo in grupos)
                {
                    bot.Puntos++;
                    Console.WriteLine("Se añade punto");
                    foreach (var c in grupo.ToList()) bot.ZoneArmado.Remove(c);
                }
            }

            estado.DiscardPile.Add(carta);
            return true;
        }

        //Simula turno aleatorio aqui hay que hacer la accion aleatoria del player
        public static GameState SimularTurno(GameState estado)
        {
            //Aqui juega el player aleatoriamente
            //Se saca el quien es cada quien
            var jugador = estado.IsPlayer1Turn ? estado.Player1 : estado.Player2;
            var oponente = estado.IsPlayer1Turn ? estado.Player2 : estado.Player1;

            if (estado.Player1.Puntos >= 3 || estado.Player2.Puntos >= 3)
            {
                estado.JuegoTerminado = true;
                return estado;
            }


            //Si pierde turno se retoran el estado...
            if (jugador.PierdeTurno)
            {
                jugador.PierdeTurno = false;
                estado.IsPlayer1Turn = !estado.IsPlayer1Turn;
                return estado;
            }

            // Porque el jugador roba la carta

            RobarCarta(jugador, estado);

            // Si tiene cartas, jugar una aleatoria

            if (jugador.Hand.Count > 0)
            {
                var rnd = new System.Random();


                //Jugar cartas aleatorias
                int numerocartas = rnd.Next(jugador.Hand.Count);


                for (int i = 0; i < numerocartas; i++)
                {
                    //Console.WriteLine("Juega Random " + cartaIndex);
                    int cartaIndex = rnd.Next(jugador.Hand.Count);
                    bool jugadaExitosa = false;
                    int intentos = 0;

                    while (!jugadaExitosa && intentos < jugador.Hand.Count)
                    {
                        cartaIndex = rnd.Next(jugador.Hand.Count);
                        jugadaExitosa = JugarCartaEn(jugador, oponente, estado, cartaIndex);
                        intentos++;
                    }
                }
            }

            estado.IsPlayer1Turn = !estado.IsPlayer1Turn;
            return estado;
        }

        //La heurtistica del juego
        public static int Evaluar(GameState estado, bool paraPlayer1)
        {
            var jugadorActual = paraPlayer1 ? estado.Player1 : estado.Player2;
            var oponente = paraPlayer1 ? estado.Player2 : estado.Player1;

            int puntaje = 0;

            // Peso de los puntos
            puntaje += 10 * (jugadorActual.Puntos - oponente.Puntos);

            // Ejemplo: cantidad de cartas o piezas restantes
            puntaje += 2 * (jugadorActual.Hand.Count - oponente.Hand.Count);

            // Ejemplo: bonus si está por ganar
            if (jugadorActual.Puntos >= 3 - 1)
                puntaje += 100;  // casi gana

            // Penalización si el oponente está cerca de ganar
            if (oponente.Puntos >= 3 - 1)
                puntaje -= 100;

            // Puedes añadir más factores como control de tablero, acciones especiales, combos, etc.

            return puntaje;
        }
    }

