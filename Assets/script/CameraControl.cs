using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public enum State : int
    {
        Turn = 0,
        EndTurn,
    }

    public State _state { get; set; }

    public Transform targetTurn;
    public Transform targetEndTurn;

    // Start is called before the first frame update
    void Start()
    {
        var s = transform.position;
        _state = State.EndTurn;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 relativePos;
        if (_state == State.EndTurn)
        {
            relativePos = targetEndTurn.position - transform.position;
        }
        else
        {
            relativePos = targetTurn.position - transform.position;
        }

        // The step size is equal to speed times frame time.
        float singleStep = 2f * Time.deltaTime;

        // Rotate the forward vector towards the target direction by one step
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, relativePos, singleStep, 0.0f);

        // the second argument, upwards, defaults to Vector3.up
        Quaternion rotation = Quaternion.LookRotation(newDirection, Vector3.up);
        transform.rotation = rotation;
    }
}
