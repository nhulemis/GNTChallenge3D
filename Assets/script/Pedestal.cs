using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pedestal : MonoBehaviour
{
    public const string BLUE_HIGHLIGHT = "_blueRange";
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
                    else
                    {
                        IsSelected = false;
                        transform.GetChild(2).GetComponent<ParticleSystem>().Stop();
                    }
                }
            }
        }
        if (GameManager.currentTurn == GameManager.Turn.Player)
        {
            Mat.SetFloat(ALPHA, 0.4f);
        }
        else
        {
            Mat.SetFloat(ALPHA, 0.1f);
        }

        if (IsInMoveRange)
        {
            DisablePedestalColor(ORANGE_HIGHLIGHT);
            EnablePedestalColor(BLUE_HIGHLIGHT);
        }
        
    }

    public void OnSelected()
    {
        IsSelected = true;
        IsInMoveRange = true;
        transform.GetChild(2).GetComponent<ParticleSystem>().Play();
        GameManager.SetActiveMoveRange(Coordinate.x, Coordinate.y);
    }

    public void EnablePedestalColor(string name)
    {
        Mat.SetFloat(name, 1);
    }
    public void DisablePedestalColor(string name)
    {
        Mat.SetFloat(name, 0);
    }
}
