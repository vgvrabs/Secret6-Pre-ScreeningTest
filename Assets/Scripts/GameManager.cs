using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager gameManager;
    
    //initializes how many blocks for the playthrough
    public int BlockCount = 1;

    //initializes a dictionary for the towers to implement a FIFO order
    //chars refer to unique towers
    private Dictionary<char, Stack> towers = new Dictionary<char, Stack> {
        {'A', new Stack()},
        {'B', new Stack()},
        {'C', new Stack()}
    };

    [SerializeField] 
    //total number of blocks available
    private List<GameObject> blocks;

    private void Awake() {
        
        if (gameManager ==  null) {
            gameManager = GetComponent<GameManager>();
        }
    }

    private void Start() {
        InitializeBlocks();
    }

    private void InitializeBlocks() {
        
        //sets up number of blocks based BlockCount
        for(int i = 0; i < blocks.Count; i++)
        {
            if (i < BlockCount) {
                towers['A'].Push(blocks[i]);
            }
            else {
                blocks[i].SetActive(false);
            }
        }
    }


}
