﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
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

    List<Pedestal> AttackRanges;

    private Animator Anim;

    public uint RangeAttack;
    public uint RangeMove;
    public Vector2 Coordinate { get; set; }
    public bool IsLocalPlayer { get; set; }
    public bool IsSelected { get; set; }
    public State StateC { get; set; }
    public enum Party
    {
        Player,
        Enemies
    }
    public Party CharacterParty;

    float deltaTimeWaitRotate;

    Transform targetTurn;
    // Start is called before the first frame update
    void Start()
    {
        Anim = gameObject.GetComponent<Animator>();
        StateC = State.Idle;
        //Anim.SetTrigger("I3");
        targetTurn = transform;
        AttackRanges = new List<Pedestal>();
        Idle();
        
    }

    // Update is called once per frame
    void Update()
    {
        //if (GameManager.Instance.currentTurn == GameManager.Turn.Player)
        {
            if (StateC == State.run)
            {
                var relativePos = targetTurn.position - transform.position;

                // The step size is equal to speed times frame time.
                float singleStep = 15f * Time.deltaTime;

                // Rotate the forward vector towards the target direction by one step
                Vector3 newDirection = Vector3.RotateTowards(transform.forward, relativePos, singleStep, 0.0f);

                // the second argument, upwards, defaults to Vector3.up
                Quaternion rotation = Quaternion.LookRotation(newDirection);
                transform.rotation = rotation;

                //if (deltaTimeWaitRotate <= Time.time)
                {
                    //move
                    float step = 3f * Time.deltaTime; // calculate distance to move
                    transform.position = Vector3.MoveTowards(transform.position, targetTurn.position, step);

                    // Check if the position of the cube and sphere are approximately equal.
                    var distance = Vector3.Distance(transform.position, targetTurn.position);
                    if (distance < 0.25f)
                    {
                        Idle();
                    }
                }
            }
        }

        if (StateC == State.Idle)
        {
            transform.position = targetTurn.position;
        }

    }

    public void Attack(float x, float y)
    {
        Anim.SetTrigger("Attack");
    }

    public void Run(Transform transf, Vector2 coordinate)
    {
        StateC = State.run;
        Anim.SetBool("Moving", true);

        GameManager.Instance.MatrixFightingPlace[(int)Coordinate.x, (int)Coordinate.y].GetComponent<Pedestal>().PlayerO = Pedestal.Player.NONE;

        var pedestals = GameObject.FindGameObjectsWithTag("pedestal");

        foreach (var item in pedestals)
        {
            var pe = item.GetComponent<Pedestal>();
            if (pe.IsInMoveRange)
            {
                pe.IsInMoveRange = false;
            }

            if (pe.IsInAttackRange)
            {
                pe.IsInAttackRange = false;
            }
        }

        Coordinate = coordinate;
        //GameManager.Instance.MatrixFightingPlace[(int)Coordinate.x, (int)Coordinate.y].GetComponent<Pedestal>().IsInAttackRange = true;
        targetTurn = transf;

        deltaTimeWaitRotate = Time.time + 0.5f;
    }

    public void Idle()
    {
        int rand = Random.Range(0, 3);
        StateC = State.Idle;
        Anim.SetBool("Moving", false);

    }

    public void AddAttackRange(Pedestal go)
    {
        if (!AttackRanges.Contains(go))
        {
            AttackRanges.Add(go);
        }
    }
}
