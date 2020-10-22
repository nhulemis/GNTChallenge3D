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
    public GameObject PKnightPrefab;
    public GameObject PmagePrefab;

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
        PKnightPrefab.transform.position = localPlayer.transform.position;
        PKnightPrefab.GetComponent<Character>().CharacterParty = Character.Party.Player;
        PKnightPrefab.GetComponent<Character>().Coordinate = new Vector2(0, 0);
        localPlayer.transform.GetComponent<Pedestal>().PlayerO = Pedestal.Player.Player;

        var localPlayer2 = MatrixFightingPlace[2, 2];

        PmagePrefab.transform.position = localPlayer2.transform.position;
        PmagePrefab.GetComponent<Character>().CharacterParty = Character.Party.Player;
        PmagePrefab.GetComponent<Character>().Coordinate = new Vector2(2, 2);
        localPlayer2.transform.GetComponent<Pedestal>().PlayerO = Pedestal.Player.Player;

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
            for (int i = 1; i <= attackRange; i++)
            {
                // top
                int radius = (int)attackRange - i;
                int x = (int)coordinate.x + i;
                int y = (int)coordinate.y;
                ActivePedestalPerRadius(x, y, radius, false);

                // bottom
                x = (int)coordinate.x - i;
                y = (int)coordinate.y;
                ActivePedestalPerRadius(x, y, radius, false);

                //left
                x = (int)coordinate.x;
                y = (int)coordinate.y + i;
                ActivePedestalPerRadius(x, y, radius, true);

                //right
                x = (int)coordinate.x;
                y = (int)coordinate.y - i;
                ActivePedestalPerRadius(x, y, radius, true);

            }


        }
    }

    private void ActivePedestalPerRadius(int x, int y, int radius, bool vertical)
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
                    SetActivePedestal(tmpX, y);
                }
                // down
                ra = radius;
                while (ra > 0)
                {
                    var tmpX = x;
                    tmpX -= ra;
                    ra--;
                    SetActivePedestal(tmpX, y);
                }
                SetActivePedestal(x, y);
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
                    SetActivePedestal(x, tmpY);
                }
                // right
                ra = radius;
                while (ra > 0)
                {
                    var tmpY = y;
                    tmpY -= ra;
                    ra--;
                    SetActivePedestal(x, tmpY);
                }
                SetActivePedestal(x, y);
            }
        }
    }

    private void SetActivePedestal(int x, int y)
    {
        if (x >= 0 && x < ROW && y >= 0 && y < COLUMN)
        {
            var pedestal = MatrixFightingPlace[x, y].GetComponent<Pedestal>();
            if (!pedestal.IsHovering)
            {
                pedestal.EnablePedestalColor(Pedestal.ORANGE_HIGHLIGHT);
            }
            else
            {
                pedestal.DisablePedestalColor(Pedestal.ORANGE_HIGHLIGHT);
            }
        }
    }

    void OnHoverPedestal(Transform objTr)
    {
        if (oldHover != null && oldHover.name != objTr.name)
        {
            oldHover.GetComponent<Pedestal>().DisablePedestalColor(Pedestal.BLUE_HIGHLIGHT);
            //oldHover.GetChild(0).GetComponent<MeshRenderer>().material.SetFloat("_orangeRange", 1);
            oldHover.GetComponent<Pedestal>().IsHovering = false;
        }


        if (objTr.tag != "pedestal" && oldHover != null && oldHover.tag == "pedestal")
        {
            oldHover.GetComponent<Pedestal>().DisablePedestalColor(Pedestal.BLUE_HIGHLIGHT);
            oldHover.GetComponent<Pedestal>().IsHovering = false;
        }

        if (objTr.tag == "pedestal")
        {
            objTr.GetComponent<Pedestal>().EnablePedestalColor(Pedestal.BLUE_HIGHLIGHT);
            objTr.GetComponent<Pedestal>().IsHovering = true;
            oldHover = objTr;
        }
    }

    void ResetColorFightPlace()
    {
        foreach (var cell in MatrixFightingPlace)
        {
            var pedestal = cell.transform.GetComponent<Pedestal>();
            if (!pedestal.IsHovering)
            {
                pedestal.DisablePedestalColor(Pedestal.ORANGE_HIGHLIGHT);
                pedestal.DisablePedestalColor(Pedestal.BLUE_HIGHLIGHT);
            }
        }
    }

    public void SetActiveMoveRange(float x, float y)
    {
        var chars = GameObject.FindGameObjectsWithTag("Player");
        foreach (var character in chars)
        {
            Character c = character.GetComponent<Character>();
            if (c.Coordinate.x == x && c.Coordinate.y == y)
            {
                SetActiveMoveRange(x, y, c.RangeMove);

                break;
            }
        }
    }

    private void SetActiveMoveRange(float x, float y, uint rangeMove)
    {
        for (int i = 1; i <= rangeMove; i++)
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
