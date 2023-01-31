using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


public class BlocksController : MonoBehaviour {

  public float MoveSpeed;
  public bool IsBlockMoving = false;
  public AutoSolve AutoSolve;
  [SerializeField] public bool hasBlockSelected;
  [SerializeField] private Camera camera;
  [SerializeField] private float waitTime = 0f;

  private GameManager gameManager;

  [SerializeField] private float hoverDistance = 1.0f;
  [SerializeField] private bool canMove = false;

  [Header("Block Components")] 
  [SerializeField] private GameObject selectedBlock;
  [SerializeField] private Transform selectedTransform;
  [SerializeField] private Vector3 selectedPosition;
  [SerializeField] private Vector3 previousPosition;

  [Header("Tower")] 
  [SerializeField] private string sourceTowerName;
  [SerializeField] private string destinationTowerName;
  [SerializeField] private string auxTowerName;
  [SerializeField] private int assignedIndex;
  [SerializeField] private int tempBlockCount;

  [Header("Snap Positions")] [SerializeField]
  private List<Transform> snapPositions;


  private void Awake() {
    camera = Camera.main;
  }

  private void Start() {
    gameManager = SingletonManager.Get<GameManager>();

    /*if (gameManager.AutoSolve) {
      //selectedTowerName = "TowerA";
      StartCoroutine(Sort(3, "TowerA", "TowerB", "TowerC"));
    }*/
  }

  public IEnumerator WaitAndCallAutoSolve(int blockCount, string from, string aux, string to) {
    yield return new WaitForSeconds(2f);
    
    //Sort(blockCount, from, aux, to);
  }

  private void Update() {
    if (canMove) {
      MoveBlock();
    }

    if (gameManager.AutoSolve) {
      if (AutoSolve.GetTotalMoves() > 0 && Time.time >= waitTime) {
        print("Auto Moves:" + AutoSolve.GetTotalMoves());
        string[] currentMoves = AutoSolve.RemoveQueueMoves();
        sourceTowerName = currentMoves[0];
        destinationTowerName = currentMoves[1];

        canMove = true;

        selectedBlock = gameManager.GetTopBlock(sourceTowerName);
        selectedBlock.GetComponent<Rigidbody>().isKinematic = true;
        selectedBlock.GetComponent<Rigidbody>().useGravity = false;
        
        selectedTransform = selectedBlock.transform;
        previousPosition = selectedBlock.transform.position;
        //makes the selected block float for feedback
        selectedPosition = new Vector3(selectedTransform.position.x,
          selectedTransform.position.y + hoverDistance, selectedTransform.position.z);

        selectedBlock.transform.position = selectedPosition;

        assignedIndex = AssignIndex(destinationTowerName);
        hasBlockSelected = true;

        waitTime = Time.time + 2f;
      }
    }
    
    if (IsBlockMoving || gameManager.GameHasEnded) return;

    if (Input.GetMouseButtonDown(0) && !gameManager.AutoSolve) {
      Ray ray = camera.ScreenPointToRay(Input.mousePosition);
      RaycastHit hit;
      
      if (!hasBlockSelected) {
        //checks if there is no block selected yet and takes that block to compare to the next clicked upon block
        if (Physics.Raycast(ray, out hit)) {
          sourceTowerName = hit.collider.name;

          if (gameManager.CheckStack(sourceTowerName) == false) return;

          selectedBlock = gameManager.GetTopBlock(sourceTowerName);
          selectedBlock.GetComponent<Rigidbody>().isKinematic = true;
          selectedBlock.GetComponent<Rigidbody>().useGravity = false;

          selectedTransform = selectedBlock.transform;
          previousPosition = selectedBlock.transform.position;
          //makes the selected block float for feedback
          selectedPosition = new Vector3(selectedTransform.position.x,
            selectedTransform.position.y + hoverDistance, selectedTransform.position.z);

          selectedBlock.transform.position = selectedPosition;

          Debug.Log(selectedBlock);
          hasBlockSelected = true;
        }
      }

      else if (hasBlockSelected) {
        //compares the selected block to this one
        if (Physics.Raycast(ray, out hit)) {
          if (hit.collider.name == sourceTowerName) {
            Debug.Log("Invalid Move");
            ResetBlock();
            return;
          }

          destinationTowerName = hit.collider.name;
          
          //checks if the second selected tower has stack on them
          if (gameManager.CheckStack(destinationTowerName)) {
            if (!gameManager.CheckIfMoveIsValid(sourceTowerName, destinationTowerName)) {
              Debug.Log("Move is Invalid");
              ResetBlock();
              return;
            }
          }
          
          assignedIndex = AssignIndex(destinationTowerName);
          print(assignedIndex);
          //selectedBlock.transform.position = snapPositions[assignedIndex];
          canMove = true;
          IsBlockMoving = true;
          Debug.Log("Move is Valid");
        }

      }
    }
  }

  public int AssignIndex(string towerName) {
    //assigns position index based on tower name
    if (towerName == "TowerA") return 0;
    if (towerName == "TowerB") return 1;
    else return 2;
  }

  public void MoveBlock() {
    //moves the block towards the other tower
    selectedBlock.transform.position = Vector3.MoveTowards(selectedBlock.transform.position,
      snapPositions[assignedIndex].position, MoveSpeed * Time.deltaTime);

    //checks if the block has arrived in the target position
    if (Vector3.Distance(selectedBlock.transform.position, snapPositions[assignedIndex].position) <= 0) {
      //disables movement for the block
      selectedBlock.GetComponent<Rigidbody>().isKinematic = false;
      selectedBlock.GetComponent<Rigidbody>().useGravity = true;
      
      //allows player input again 
      canMove = false;
      IsBlockMoving = false;
      hasBlockSelected = false;
      
      //resets all variables and checks for win condition
      gameManager.AfterSuccessfulMove(sourceTowerName, destinationTowerName, selectedBlock);
      gameManager.CheckForWinCondition();

      if (gameManager.AutoSolve) {
        //Sort(tempBlockCount,selectedTowerName, comparedTowerName, auxTowerName);
        return;
      }
      //resets all variables

      if (gameManager.AutoSolve) return;
      selectedBlock = null;
      sourceTowerName = String.Empty;
      destinationTowerName = String.Empty;
      
    }
  }
  
  private void ResetBlock() {
    //resets all values of the block
    selectedBlock.GetComponent<Rigidbody>().isKinematic = false;
    selectedBlock.GetComponent<Rigidbody>().useGravity = true;
    
    //resets the block to its original position
    selectedPosition = previousPosition;
    selectedBlock.transform.position = selectedPosition;
    selectedBlock = null;
    
    //clears the tower names for comparison
    sourceTowerName = String.Empty;
    destinationTowerName = String.Empty;
    
    //enables player input again
    canMove = false;
    IsBlockMoving = false;
    hasBlockSelected = false;
  }

  /*public void Sort(int blockNumber, string from, string aux, string to) {
    if (!IsBlockMoving) {
      if (blockNumber == 1) {
        canMove = true;
        IsBlockMoving = true;
        selectedTowerName = from;
        comparedTowerName = to;
        auxTowerName = aux;
        tempBlockCount = blockNumber;
        assignedIndex = AssignIndex(to);
        selectedBlock = gameManager.GetTopBlock(selectedTowerName);
        selectedBlock.GetComponent<Rigidbody>().isKinematic = true;
        selectedBlock.GetComponent<Rigidbody>().useGravity = false;
        return;
      }
      else {
        Sort(blockNumber - 1, from, to, aux);
        //StartCoroutine(WaitAndCall(blockNumber - 1, from, to, aux));
        canMove = true;
        IsBlockMoving = true;
        selectedTowerName = from;
        comparedTowerName = to;
        auxTowerName = aux;
        tempBlockCount = blockNumber;
        assignedIndex = AssignIndex(to);
        selectedBlock = gameManager.GetTopBlock(selectedTowerName);
        selectedBlock.GetComponent<Rigidbody>().isKinematic = true;
        selectedBlock.GetComponent<Rigidbody>().useGravity = false;
        //print("block from " + from + " moved to " + to);
        //Sort(blockNumber - 1, aux, from, to);
        //StartCoroutine(WaitAndCall(blockNumber - 1, aux, from, to));

      }
    }

    waitTime = Time.time + 1.1f;
  }*/

  /*public void WaitAndCall() {
    Sort(gameManager.BlockCount - 1, "TowerA", "TowerB", "TowerC");
  }*/
}
