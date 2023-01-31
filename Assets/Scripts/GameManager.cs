using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    //initializes how many blocks for the playthrough
    public int BlockCount = 1;

    public int MoveCount = 0;

    public bool GameHasEnded;
    public bool AutoSolve;

    private UIManager uiManager;
    [SerializeField]private AutoSolve autoSolve;

    //initializes a dictionary for the towers to implement a FIFO order
    //string refer to unique towers
    private Dictionary<string, Stack> towers = new Dictionary<string, Stack> {
        {"TowerA", new Stack()},
        {"TowerB", new Stack()},
        {"TowerC", new Stack()}
    };

    [SerializeField] 
    //total number of blocks available
    private List<GameObject> blocks;

    private void OnEnable() {
       SingletonManager.Register(this);
    }

    private void OnDisable() {
        SingletonManager.Remove<GameManager>();
    }

    private void Start() {
        uiManager = SingletonManager.Get<UIManager>();
        GameHasEnded = false;
        InitializeBlocks();
        
        if (AutoSolve) {
            //autoSolve.CalculateTotalMoves(BlockCount);
           // print(autoSolve.GetTotalMoves());
           autoSolve.GenerateAutoSolveMoves(BlockCount);
        }
    }

    private void InitializeBlocks() {
        //sets up number of blocks based BlockCount
        for(int i = 0; i < blocks.Count; i++)
        {
            if (i < BlockCount) {
                blocks[i].GetComponent<Rigidbody>().isKinematic = false;
                blocks[i].GetComponent<Rigidbody>().useGravity = true;
               
                towers["TowerA"].Push(blocks[i]);
            }
            else {
                blocks[i].SetActive(false);
            }
        }
    }

    public GameObject GetTopBlock(string towerName) {
        //returns the top element of the stack
        return (GameObject)towers[towerName].Peek();
    }

    public int GetStackCount(string towerName) {
        return towers[towerName].Count;
    }
    public bool CheckStack(string towerName) {
        //checks if the stack has elements on it
        if (towers[towerName].Count > 0)
            return true;
        else return false;
    }

    public bool CheckIfMoveIsValid(string selectedTower, string comparedTower) {
        //initializes value for the selected and block to be compared 
        //compare their blocks together
        GameObject selectedBlock = (GameObject)towers[selectedTower].Peek();
        int selectedBlockRank = selectedBlock.GetComponent<BlockOrder>().Order;
        
        GameObject comparedBlock = (GameObject) towers[comparedTower].Peek();
        int comparedBlockRank = comparedBlock.GetComponent<BlockOrder>().Order;

        if (selectedBlockRank < comparedBlockRank) return true;
        
        else return false;
    }

    public void CheckForWinCondition() {
        //checks the number of elements of the target tower
        if (towers["TowerC"].Count >= BlockCount) {
            Debug.Log("Completed!");
            GameHasEnded = true;
            //Time.timeScale = 0;
        }
    }

    public void AfterSuccessfulMove(string currentTowerName,string comparedTowerName, GameObject currentBlock) {
        towers[currentTowerName].Pop();
        towers[comparedTowerName].Push(currentBlock);

        MoveCount++;
        uiManager.SetMoveCountText(MoveCount);
    }
}
