using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class Species
{
    #region Data
    private string name;
    private bool intelligent;
    private int health;
    private float energy;
    private float speed;
    private float hungerRate;
    private Attack unarmedAttack;
    //private int unarmedDamage;
    private List<MovementType> movementTypes;

    private int meatAmount;
    private int leatherAmount;

    private Color colour;
    private Tile tile;
    #endregion Data

    #region Properties
    public string Name { get => name; }
    public bool Intelligent { get => intelligent; }
    public int Health { get => health; }
    public float Energy { get => energy; }
    public float Speed { get => speed; }
    public float HungerRate { get => hungerRate; }
    public Attack UnarmedAttack { get => unarmedAttack; }
    //public int UnarmedDamage { get => unarmedDamage; }
    public List<MovementType> MovementTypes { get => movementTypes; }

    public int MeatAmount { get => meatAmount; }
    public int LeatherAmount { get => leatherAmount; }

    public Color Colour { get => colour; }
    public Tile Tile { get => tile; }
    #endregion Properties


    #region Methods
    public Species(string name, bool intelligent, int health, float energy, float speed, float hungerRate, Attack unarmedAttack, List<MovementType> movementTypes, int meatAmount, int leatherAmount,
                    Color colour, Tile tile)
    {
        this.name = name;
        this.intelligent = intelligent;
        this.health = health;
        this.energy = energy;
        this.speed = speed;
        this.hungerRate = hungerRate;
        this.unarmedAttack = unarmedAttack;
        //this.unarmedDamage = unarmedDamage;
        this.movementTypes = movementTypes;

        this.meatAmount = meatAmount;
        this.leatherAmount = leatherAmount;

        this.colour = colour;
        this.tile = tile;
    }
    #endregion Methods
}



public enum MovementType : byte { Land, Water, Air }