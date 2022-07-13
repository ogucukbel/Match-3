using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchFinder : MonoBehaviour
{
    [SerializeField] BoardController board;
    public List<GameObject> currentMatches = new List<GameObject>();


    public void FindAllMatches()
    {
        StartCoroutine(FindAllMatchesCoroutine());
    }

    private IEnumerator FindAllMatchesCoroutine()
    {
        yield return new WaitForSeconds(.1f);
        for(int i = 0; i < board.boardWidth; i++)
        {
            for(int j = 0; j < board.boardHeight; j++)
            {
                GameObject currentDrop = board.allDrops[i, j];
                if(currentDrop != null)
                {
                    if(i > 0 && i < board.boardWidth -1)
                    {
                        GameObject leftDrop = board.allDrops[i-1, j]; 
                        GameObject rightDrop = board.allDrops[i+1, j];
                        if(leftDrop != null && rightDrop != null)
                        {
                            if(leftDrop.GetComponent<DropController>().dropType == currentDrop.GetComponent<DropController>().dropType
                            && rightDrop.GetComponent<DropController>().dropType == currentDrop.GetComponent<DropController>().dropType)
                            {
                                if(!currentMatches.Contains(leftDrop))
                                {
                                    currentMatches.Add(leftDrop);
                                }
                                leftDrop.GetComponent<DropController>().isMatched = true;
                                if(!currentMatches.Contains(rightDrop))
                                {
                                    currentMatches.Add(rightDrop);
                                }     
                                rightDrop.GetComponent<DropController>().isMatched = true;      
                                if(!currentMatches.Contains(currentDrop))
                                {
                                    currentMatches.Add(currentDrop);
                                } 
                                currentDrop.GetComponent<DropController>().isMatched = true;
                            }
                        }
                    }
                    if(j > 0 && j < board.boardHeight -1)
                    {
                        GameObject upDrop = board.allDrops[i, j+1]; 
                        GameObject downDrop = board.allDrops[i, j-1];
                        if(upDrop != null && downDrop != null)
                        {
                            if(upDrop.GetComponent<DropController>().dropType == currentDrop.GetComponent<DropController>().dropType
                            && downDrop.GetComponent<DropController>().dropType == currentDrop.GetComponent<DropController>().dropType)
                            {
                                if(!currentMatches.Contains(upDrop))
                                {
                                    currentMatches.Add(upDrop);
                                }
                                upDrop.GetComponent<DropController>().isMatched = true;   
                                if(!currentMatches.Contains(downDrop))
                                {
                                    currentMatches.Add(downDrop);
                                }  
                                downDrop.GetComponent<DropController>().isMatched = true;
                                if(!currentMatches.Contains(currentDrop))
                                {
                                    currentMatches.Add(currentDrop);
                                }      
                                currentDrop.GetComponent<DropController>().isMatched = true;
                            }
                        }
                    }
                }
            }
        }
    }
}
