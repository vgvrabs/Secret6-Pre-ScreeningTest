using System;
using System.Collections.Generic;
using UnityEngine;


public class BlocksController : MonoBehaviour {

  public float MoveSpeed;
  public bool IsBlockMoving = false;
  [SerializeField] public bool hasBlockSelected;
  [SerializeField] private Camera camera;

  private GameManager gameManager;

  [SerializeField] private float hoverDistance = 1.0f;
  [SerializeField] private bool canMove = false;

  [Header("Block Components")] 
  [SerializeField] private GameObject selectedBlock;
  [SerializeField] private Transform selectedTransform;
  [SerializeField] private Vector3 selectedPosition;
  [SerializeField] private Vector3 previousPosition;

  [Header("Tower")] [SerializeField] private string selectedTowerName;
  [SerializeField] private string comparedTowerName;
  [SerializeField] private int assignedIndex;

  [Header("Snap Positions")] [SerializeField]
  private List<Transform> snapPositions;


  private void Awake() {
    camera = Camera.main;
  }

  private void Start() {
    gameManager = GameManager.instance;
  }

  private void Update() {
    if (canMove) {
      MoveBlock();
    }

    if (IsBlockMoving) return;

    if (Input.GetMouseButtonDown(0)) {
      Ray ray = camera.ScreenPointToRay(Input.mousePosition);
      RaycastHit hit;
      
      if (!hasBlockSelected) {
        //checks if there is no block selected yet and takes that block to compare to the next clicked upon block
        if (Physics.Raycast(ray, out hit)) {
          selectedTowerName = hit.collider.name;

          if (gameManager.CheckStack(selectedTowerName) == false) return;

          selectedBlock = gameManager.GetTopBlock(selectedTowerName);
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
          if (hit.collider.name == selectedTowerName) {
            Debug.Log("Invalid Move");
            ResetBlock();
            return;
          }

          comparedTowerName = hit.collider.name;
          
          //checks if the second selected tower has stack on them
          if (gameManager.CheckStack(comparedTowerName)) {
            if (!gameManager.CheckIfMoveIsValid(selectedTowerName, comparedTowerName)) {
              Debug.Log("Move is Invalid");
              ResetBlock();
              return;
            }
          }
          
          assignedIndex = AssignIndex(comparedTowerName);
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
      gameManager.AfterSuccessfulMove(selectedTowerName, comparedTowerName, selectedBlock);
      gameManager.CheckForWinCondition();
      
      //resets all variables
      selectedBlock = null;
      selectedTowerName = String.Empty;
      comparedTowerName = String.Empty;
      
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
    selectedTowerName = String.Empty;
    comparedTowerName = String.Empty;
    
    //enables player put again
    canMove = false;
    IsBlockMoving = false;
    hasBlockSelected = false;
  }

}
