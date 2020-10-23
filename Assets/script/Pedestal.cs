using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pedestal : MonoBehaviour
{
    [ColorUsage(true, true)]
    public Color MOVE_HIGHLIGHT;

    [ColorUsage(true, true)]
    public Color ATTACK_HIGHLIGHT;

    [ColorUsage(true, true)]
    public Color NORMAL_COLOR;
    public const string ORANGE_HIGHLIGHT = "_orangeRange";
    public const string ALPHA = "_alpha";

    public enum Player
    {
        NONE = 0,
        Player,
        Enemies
    }
    public Material Mat { get; set; }
    public Vector2 Coordinate { get; set; }
    public Player PlayerO { get; set; }
    public bool IsInMoveRange { get; set; }
    public bool IsSelected { get; set; }
    public bool IsInAttackRange { get; set; }
    public bool IsHovering { get; set; }

    private Transform point;
    private GameManager GameManager { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Pedestal");
        PlayerO = Player.NONE;
        IsInAttackRange = false;
        IsInMoveRange = false;
        Mat = transform.GetChild(0).GetComponent<MeshRenderer>().material;
        GameManager = GameManager.Instance;
        Mat.SetColor("_color", NORMAL_COLOR);
        point = transform.GetChild(1).transform;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonUp(0))
        {
            //Debug.Log("Up");
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                if (hit.transform.tag.Equals("pedestal"))
                {
                    if (hit.transform == transform && PlayerO == Player.Player)
                    {
                        OnSelected();
                    }
                    else if (hit.transform == transform)
                    {
                        CheckMoveCharacter();
                        IsSelected = false;
                        transform.GetChild(2).GetComponent<ParticleSystem>().Stop();
                    }
                    else
                    {
                        IsSelected = false;
                        transform.GetChild(2).GetComponent<ParticleSystem>().Stop();
                    }
                }
            }
        }

        if (!IsInMoveRange && !IsInAttackRange)
        {
            EnablePedestalColor(NORMAL_COLOR);
        }

        if (IsInAttackRange)
        {
            EnablePedestalColor(ATTACK_HIGHLIGHT);
        }

        if (IsInMoveRange)
        {
            EnablePedestalColor(MOVE_HIGHLIGHT);
        }

        if (IsHovering)
        {
            EnablePedestalColor(MOVE_HIGHLIGHT + ATTACK_HIGHLIGHT);
        }



    }

    private void CheckMoveCharacter()
    {
        Debug.Log("hello");
        var chars = GameObject.FindGameObjectsWithTag("Player");
        Character c = null;

        foreach (var character in chars)
        {
            Character ct = character.GetComponent<Character>();
            if (ct.IsSelected)
            {
                c = ct;
            }
        }

        if (c != null && this.IsInMoveRange)
        {
            PlayerO = Player.Player;
            c.Run(point, Coordinate);
        }
    }

    public void OnSelected()
    {
        GameManager.ResetColorFightPlace();
        IsSelected = true;
        transform.GetChild(2).GetComponent<ParticleSystem>().Play();
        GameManager.SetActiveMoveRange(Coordinate.x, Coordinate.y);
    }



    public void EnablePedestalColor(Color name)
    {
        Mat.SetColor("_color", name);
    }
    public void DisablePedestalColor(string name)
    {
        Mat.SetFloat(name, 0);
    }
}
