using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { private set; get; }
    public GameObject PedestalPrefab;
    public int ROW = 8;
    public int COLUMN = 6;
    public GameObject[,] MatrixFightingPlace;
    public bool IsEditingPTeam { get; set; }
    public bool IsEndGame { get; set; }
    //===========================Player Model======================
    public GameObject char1;
    public GameObject char2;
    public GameObject char3;

    //===========================Enemies Model======================
    public GameObject EKnightPrefab;

    private Transform oldHover;

    public enum Turn
    {
        None,
        Player,
        Enemies
    }

    public Turn currentTurn { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        IsEndGame = true;
        CreateFightingPlace();
        currentTurn = Turn.None;
    }

    public void InitCharacter()
    {
        var localPlayer = MatrixFightingPlace[0, 0];
        char1.transform.position = localPlayer.transform.position;
        char1.GetComponent<Character>().Coordinate = new Vector2(0, 0);
        localPlayer.transform.GetComponent<Pedestal>().PlayerO = Pedestal.Player.Player;

        var localPlayer2 = MatrixFightingPlace[2, 2];

        char2.transform.position = localPlayer2.transform.position;
        char2.GetComponent<Character>().CharacterParty = Character.Party.Player;
        char2.GetComponent<Character>().Coordinate = new Vector2(2, 2);
        localPlayer2.transform.GetComponent<Pedestal>().PlayerO = Pedestal.Player.Player;

        var localPlayer3 = MatrixFightingPlace[0, 1];

        char3.transform.position = localPlayer3.transform.position;
        char3.GetComponent<Character>().CharacterParty = Character.Party.Player;
        char3.GetComponent<Character>().Coordinate = new Vector2(0, 1);
        localPlayer3.transform.GetComponent<Pedestal>().PlayerO = Pedestal.Player.Player;

        var bot = MatrixFightingPlace[ROW - 1, COLUMN - 1];
        EKnightPrefab.transform.position = bot.transform.position;
        bot.GetComponent<Pedestal>().PlayerO = Pedestal.Player.Enemies;

    }

    private void CreateFightingPlace()
    {
        MatrixFightingPlace = new GameObject[ROW, COLUMN];

        Vector3 currentRow = GameObject.FindGameObjectWithTag("SpawnPedestal").transform.position;
        float firstCoordinate = currentRow.x;
        for (int row = 0; row < ROW; row++)
        {
            for (int col = 0; col < COLUMN; col++)
            {
                GameObject pedestal = Instantiate(PedestalPrefab);
                pedestal.transform.position = currentRow;
                pedestal.GetComponent<Pedestal>().Coordinate = new Vector2(row, col);
                pedestal.transform.name = "pedestal" + row + col;
                MatrixFightingPlace[row, col] = pedestal;

                currentRow.x -= 1;
            }
            currentRow.x = firstCoordinate;
            currentRow.z += 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (IsEndGame)
        {
            return;
        }
        //if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                OnHoverPedestal(hit.transform);
                //Debug.Log("You selected the " + hit.transform.name); // ensure you picked right object
            }
        }
        if (currentTurn == Turn.Player)
        {
            SetActiveAttackRange();
        }
        else
        {
            ResetColorFightPlace();
        }

        if (GameManager.Instance.currentTurn == GameManager.Turn.None)
        {
            var chars = GameObject.FindGameObjectsWithTag("Player");
            foreach (var item in chars)
            {
                item.GetComponent<Character>().Attack(0, 0);
            }
            GameManager.Instance.currentTurn = Turn.Enemies;
        }
    }

    private void SetActiveAttackRange()
    {
        var chars = GameObject.FindGameObjectsWithTag("Player");

        foreach (var character in chars)
        {
            var characterTmp = character.GetComponent<Character>();
            Vector2 coordinate = characterTmp.Coordinate;
            uint attackRange = characterTmp.RangeAttack;
            var pedestal0 = MatrixFightingPlace[(int)coordinate.x, (int)coordinate.y];
            pedestal0.GetComponent<Pedestal>().Mat.SetFloat("_orangeRange", 1);

            List<Pedestal> attR = new List<Pedestal>();

            for (int i = 0; i <= attackRange; i++)
            {
                // top
                int radius = (int)attackRange - i;
                int x = (int)coordinate.x + i;
                int y = (int)coordinate.y;
                ActivePedestalAttackPerRadius(x, y, radius, false, ref attR);

                // bottom
                x = (int)coordinate.x - i;
                y = (int)coordinate.y;
                ActivePedestalAttackPerRadius(x, y, radius, false, ref attR);

                //left
                x = (int)coordinate.x;
                y = (int)coordinate.y + i;
                ActivePedestalAttackPerRadius(x, y, radius, true, ref attR);

                //right
                x = (int)coordinate.x;
                y = (int)coordinate.y - i;
                ActivePedestalAttackPerRadius(x, y, radius, true, ref attR);

            }
            foreach (var item in attR)
            {
                characterTmp.AddAttackRange(item);
            }
        }
    }

    private void ActivePedestalAttackPerRadius(int x, int y, int radius, bool vertical, ref List<Pedestal> attR)
    {
        if (x >= 0 && x < ROW && y >= 0 && y < COLUMN)
        {
            if (vertical)
            {
                //up
                int ra = radius;
                while (ra > 0)
                {
                    var tmpX = x;
                    tmpX += ra;
                    ra--;
                    SetActivePedestal(tmpX, y, ref attR);
                }
                // down
                ra = radius;
                while (ra > 0)
                {
                    var tmpX = x;
                    tmpX -= ra;
                    ra--;
                    SetActivePedestal(tmpX, y, ref attR);
                }
                SetActivePedestal(x, y, ref attR);
            }
            else
            {
                //left
                int ra = radius;
                while (ra > 0)
                {
                    var tmpY = y;
                    tmpY += ra;
                    ra--;
                    SetActivePedestal(x, tmpY, ref attR);
                }
                // right
                ra = radius;
                while (ra > 0)
                {
                    var tmpY = y;
                    tmpY -= ra;
                    ra--;
                    SetActivePedestal(x, tmpY, ref attR);
                }
                SetActivePedestal(x, y, ref attR);
            }
        }
    }

    private void SetActivePedestal(int x, int y, ref List<Pedestal> attR)
    {
        if (x >= 0 && x < ROW && y >= 0 && y < COLUMN)
        {
            var pedestal = MatrixFightingPlace[x, y].GetComponent<Pedestal>();
            if (!pedestal.IsHovering)
            {
                pedestal.IsInAttackRange = true;
                attR.Add(pedestal);
            }
            else
            {
                pedestal.IsInAttackRange = false;
            }
        }
    }

    void OnHoverPedestal(Transform objTr)
    {
        if (oldHover != null && oldHover.name != objTr.name)
        {
            //oldHover.GetComponent<Pedestal>().DisablePedestalColor(Pedestal.BLUE_HIGHLIGHT);
            //oldHover.GetChild(0).GetComponent<MeshRenderer>().material.SetFloat("_orangeRange", 1);
            oldHover.GetComponent<Pedestal>().IsHovering = false;
        }


        if (objTr.tag != "pedestal" && oldHover != null && oldHover.tag == "pedestal")
        {
            //oldHover.GetComponent<Pedestal>().DisablePedestalColor(Pedestal.BLUE_HIGHLIGHT);
            oldHover.GetComponent<Pedestal>().IsHovering = false;
        }

        if (objTr.tag == "pedestal")
        {
            //objTr.GetComponent<Pedestal>().EnablePedestalColor(Pedestal.BLUE_HIGHLIGHT);
            objTr.GetComponent<Pedestal>().IsHovering = true;
            oldHover = objTr;
        }
    }

    public void ResetColorFightPlace()
    {
        foreach (var cell in MatrixFightingPlace)
        {
            var pedestal = cell.transform.GetComponent<Pedestal>();
            if (!pedestal.IsHovering)
            {
                pedestal.IsInAttackRange = false;
                pedestal.IsInMoveRange = false;
            }
        }
    }

    public void SetActiveMoveRange(float x, float y)
    {
        var chars = GameObject.FindGameObjectsWithTag("Player");

        foreach (var cell in MatrixFightingPlace)
        {
            var pedestal = cell.transform.GetComponent<Pedestal>();
            if (pedestal.IsInMoveRange)
            {

                pedestal.IsInMoveRange = false;
            }
        }

        foreach (var character in chars)
        {
            Character c = character.GetComponent<Character>();
            if (c.Coordinate.x == x && c.Coordinate.y == y)
            {
                c.IsSelected = true;
                SetActiveMoveRange(x, y, c.RangeMove);
            }
            else
            {
                c.IsSelected = false;
            }
        }
    }

    private void SetActiveMoveRange(float x, float y, uint rangeMove)
    {
        for (int i = 0; i <= rangeMove; i++)
        {
            float curX = x;
            float curY = y;
            //UP
            curX += i;
            SetActiveMoveRange(curX, curY, rangeMove, true);

            curX = x;
            curX -= i;
            SetActiveMoveRange(curX, curY, rangeMove, true);

            curX = x;
            curY = y;
            curY -= i;
            SetActiveMoveRange(curX, curY, rangeMove, false);

            curY = y;
            curY += i;
            SetActiveMoveRange(curX, curY, rangeMove, false);
        }
    }

    private void SetActiveMoveRange(float curX, float curY, uint rangeMove, bool vertical)
    {
        if (vertical)
        {
            for (int i = 0; i <= rangeMove; i++)
            {
                var tempY = curY;
                tempY = curY;
                tempY -= i;
                SetActiveMoveRangeByCoordinate(curX, tempY);

                tempY = curY;
                tempY += i;
                SetActiveMoveRangeByCoordinate(curX, tempY);
            }
        }
        else
        {
            for (int i = 0; i <= rangeMove; i++)
            {
                var tempX = curX;
                tempX += i;
                SetActiveMoveRangeByCoordinate(tempX, curY);

                tempX = curX;
                tempX -= i;
                SetActiveMoveRangeByCoordinate(tempX, curY);

            }
        }
    }

    private void SetActiveMoveRangeByCoordinate(float x, float y)
    {
        if (x >= 0 && x < ROW && y >= 0 && y < COLUMN)
        {
            var pedestal = MatrixFightingPlace[(int)x, (int)y].GetComponent<Pedestal>();
            pedestal.IsInMoveRange = true;
        }

    }
}
