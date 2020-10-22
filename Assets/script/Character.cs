using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public enum State
    {
        Idle = 0,
        Hit,
        BeDamaged,
        run,
        die
    }

    public uint RangeAttack;
    public uint RangeMove;
    public Vector2 Coordinate { get; set; }
    public bool IsLocalPlayer { get; set; }
    public bool IsSelected { get; set; }
    public State StateC{ get; set; }
    public enum Party
    {
        Player,
        Enemies
    }
    public Party CharacterParty { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        StateC = State.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
