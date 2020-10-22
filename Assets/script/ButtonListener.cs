using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonListener : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void On_EndTurn_Click()
    {
        //GameObject.FindGameObjectWithTag("CameraHolder").GetComponent<CameraControl>()._state = CameraControl.State.Turn;
        if (GameManager.Instance.currentTurn == GameManager.Turn.Enemies)
        {
            GameManager.Instance.currentTurn = GameManager.Turn.Player;
        }
        else
        {
            GameManager.Instance.currentTurn = GameManager.Turn.Enemies;
        }
    }

    public void On_Play_Click() 
    {
        GameManager.Instance.IsEndGame = false;
        GameManager.Instance.InitCharacter();
        GameManager.Instance.currentTurn = GameManager.Turn.Player;
       
    }
}
