using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonData
{
    private string name;
    private PokemonSize size;
    private PokemonWeight weight;
}

public enum PokemonSize
{
    TINY, SMALL, MEDIUM, BIG, HUGE, GARGANTUAN
}

public enum PokemonWeight
{
    WEIGHTLESS, LIGHT, MEDIUM, HEAVY, CRUSHING, EXTREME
}

public enum PokemonBodyParth
{
    BODY,
    HEAD, EYES, MOUTH,
    ARMS, HANDS,
    LEGS, FEET, HOOVES
}