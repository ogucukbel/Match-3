using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    public enum GameState
    {
        wait,
        move
    }
    [Header ("Column To Stop")]
    [Tooltip("eğer sütunların hiç birinde spaw'ın durmasını istemiyorsanız değeri 9 girebilirsiniz.")]
    [SerializeField] private int columnToStop;

    [Header ("Board Settings")]
    public GameState currentState = GameState.move;
    public int boardWidth;
    public int boardHeight;
    [SerializeField] private GameObject tilePrefab;
    private GameObject[,] allTiles;
    private GameObject backgroundTile;

    [Header ("Drop Settings")]
    [SerializeField] private GameObject[] drops;
    public GameObject[,] allDrops;
    private GameObject drop;
    private int dropPoolType;
    [SerializeField] private ObjectPool objectPool;
    [SerializeField] private MatchFinder matchFinder;

    private void OnEnable() 
    {
        allTiles = new GameObject[boardWidth, boardHeight];
        allDrops = new GameObject[boardWidth, boardHeight];
        SetUp();
    }

    private void SetUp()
    {
        for(int i = 0; i < boardWidth; i++)
        {
            for(int j = 0; j < boardHeight; j++)
            {
                Vector2 tempPosition = new Vector2(i, j);
                backgroundTile = Instantiate(tilePrefab, tempPosition, Quaternion.identity) as GameObject;
                backgroundTile.transform.parent = this.transform;
                backgroundTile.name = i + " , " + j + " tile";
                dropPoolType = Random.Range(0, 4);

                int maxIterations = 0;
                while(PreMatches(i, j, drops[dropPoolType]) && maxIterations < 100)
                {
                    dropPoolType = Random.Range(0, 4);
                    maxIterations ++;
                }

                maxIterations = 0;

                drop = objectPool.SpawnFromPool(tempPosition, dropPoolType);
                allDrops[i,j] = drop;
            }
        }
    }

    private bool PreMatches(int column, int row, GameObject drop)
    {
        if(column > 1 && row >1)
        {
            if((allDrops[column -1, row].GetComponent<DropController>().dropType == drop.GetComponent<DropController>().dropType)
            && (allDrops[column -2, row].GetComponent<DropController>().dropType == drop.GetComponent<DropController>().dropType))
            {
                return true;
            }
            if((allDrops[column, row -1].GetComponent<DropController>().dropType == drop.GetComponent<DropController>().dropType)
            && (allDrops[column, row -2].GetComponent<DropController>().dropType == drop.GetComponent<DropController>().dropType))
            {
                return true;
            }
        }
        else if(column <= 1 || row <= 1)
        {
            if(row > 1)
            {
                if((allDrops[column, row-1].GetComponent<DropController>().dropType == drop.GetComponent<DropController>().dropType)
                && (allDrops[column, row-2].GetComponent<DropController>().dropType == drop.GetComponent<DropController>().dropType))
                {
                    return true;
                }
            }
            if(column > 1)
            {
                if((allDrops[column -1, row].GetComponent<DropController>().dropType == drop.GetComponent<DropController>().dropType)
                && (allDrops[column -2, row].GetComponent<DropController>().dropType == drop.GetComponent<DropController>().dropType))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void ClearMatchesAt(int column, int row)
    {
        if(allDrops[column, row].GetComponent<DropController>().isMatched)
        {
            matchFinder.currentMatches.Remove(allDrops[column,row]);
            objectPool.ReturnToPool(allDrops[column , row]);
            allDrops[column, row] = null;
        }
    }

    public void ClearMatches()
    {
        for(int i =0; i<boardWidth; i++)
        {
            for(int j = 0; j< boardHeight; j++)
            {
                if(allDrops[i, j] != null)
                {
                    ClearMatchesAt(i,j);
                }
            }
        }
        StartCoroutine(DecreaseRowCount());
    }

    private IEnumerator DecreaseRowCount()
    { 
        int emptyTileCount = 0;
        for(int i = 0; i< boardWidth; i++)
        {
            for(int j = 0; j <boardHeight; j++)
            {
                if(allDrops[i, j] == null)
                {
                    emptyTileCount++;
                }
                else if(emptyTileCount > 0)
                {
                    allDrops[i, j].GetComponent<DropController>().dropRow -= 1;
                    allDrops[i, j] = null;
                }
            }
            emptyTileCount = 0;
        }
        yield return new WaitForSeconds( .25f);
        StartCoroutine(FillEmptySpaces());
    }

    private void SpawnNewDrops()
    {
        for(int i=0; i<boardHeight; i++)
        {
            for(int j=0; j<boardWidth; j++)
            {
                if(allDrops[i, j] == null && i != columnToStop)
                {
                    int dropPoolType = Random.Range(0, 4);
                    Vector2 tempPosition = new Vector2(i, j);
                    //GameObject drop = Instantiate(drops[dropPoolType], tempPosition, Quaternion.identity);
                    GameObject drop = objectPool.RespawnFromPool(tempPosition, dropPoolType);
                    allDrops[i, j] = drop;
                }
            }
        }
    }

    private bool MatchesOnBoard()
    {
        for(int i=0; i<boardWidth; i++)
        {
            for(int j=0; j<boardHeight; j++)
            {
                if(allDrops[i, j] != null)
                {
                    if(allDrops[i, j].GetComponent<DropController>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private IEnumerator FillEmptySpaces()
    {
        SpawnNewDrops();
        yield return new WaitForSeconds(.25f);

        while(MatchesOnBoard())
        {
            yield return new WaitForSeconds(.25f);
            ClearMatches();
        }
        yield return new WaitForSeconds(.4f);
        currentState = GameState.move;
    }
}
