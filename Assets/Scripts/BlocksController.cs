using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Animations;

public class BlocksController : MonoBehaviour {

  public float MoveSpeed;
  [SerializeField]public bool hasBlockSelected;
  [SerializeField] private Camera camera;
  
  private GameManager gameManager;
  
  [SerializeField] private float hoverDistance = 1.0f;
  [SerializeField] private bool canMove = false;
  
  [Header("Block Components")]
  [SerializeField]private GameObject selectedBlock;
  [SerializeField]private Transform selectedTransform;
  [SerializeField]private Vector3 selectedPosition;

  [Header("Tower")] 
  [SerializeField] private string selectedTowerName;
  [SerializeField] private string comparedTowerName;
  [SerializeField] private int assignedIndex;

  [Header("Snap Positions")] 
  [SerializeField] private List<Transform> snapPositions;


  private void Awake() {
    camera = Camera.main;
  }

  private void Start() {
    gameManager = GameManager.instance;
  }
  private void Update() {
    if (Input.GetMouseButtonDown(0)) {
      Ray ray = camera.ScreenPointToRay(Input.mousePosition);
      RaycastHit hit;

      
      if(!hasBlockSelected){
        //checks if there is no block selected yet and takes that block to compare to the next clicked upon block
        if (Physics.Raycast(ray, out hit)) {
          selectedTowerName = hit.collider.name;

          if (gameManager.CheckStack(selectedTowerName) == false) return;

          selectedBlock = gameManager.GetTopBlock(selectedTowerName);
          selectedBlock.GetComponent<Rigidbody>().isKinematic = true;
          selectedBlock.GetComponent<Rigidbody>().useGravity = false;

          selectedTransform = selectedBlock.transform;
          selectedPosition = new Vector3(selectedTransform.position.x,
            selectedTransform.position.y + hoverDistance, selectedTransform.position.z);

          selectedBlock.transform.position = selectedPosition;

          Debug.Log(selectedBlock);
          hasBlockSelected = true;
        }
      }

      else if(hasBlockSelected) {
        //compares the selected block to this one
        if (Physics.Raycast(ray, out hit)) {
          if (hit.collider.name == selectedTowerName) {
            Debug.Log("Invalid Move");
            return;
          }

          comparedTowerName = hit.collider.name;
          
          if(gameManager.CheckStack(comparedTowerName)){
            if (!gameManager.CheckIfMoveIsValid(selectedTowerName, comparedTowerName)) {
              Debug.Log("Move is Invalid");
              return;
            }
          }

          
          assignedIndex = AssignIndex(comparedTowerName);
          print(assignedIndex);
          //selectedBlock.transform.position = snapPositions[assignedIndex];
          canMove = true;
          Debug.Log("Move is Valid");
        }
          
      }
    }
    
    if (canMove) {
      MoveBlock();
    }
  }

  public int AssignIndex(string towerName) {
    //assigns position index based on tower name
    if (towerName == "TowerA") return 0;
    if (towerName == "TowerB") return 1;
    else return 2;
  }

  public void MoveBlock() {
    selectedBlock.transform.position = Vector3.MoveTowards(selectedBlock.transform.position,
      snapPositions[assignedIndex].position, MoveSpeed * Time.deltaTime);

    if (Vector3.Distance(selectedBlock.transform.position, snapPositions[assignedIndex].position) <= 0) {
      hasBlockSelected = false;
      selectedBlock.GetComponent<Rigidbody>().isKinematic = false;
      selectedBlock.GetComponent<Rigidbody>().useGravity = true;
      canMove = false;
      gameManager.AfterSuccessfulMove(selectedTowerName, comparedTowerName, selectedBlock);
      selectedBlock = null;
      selectedTowerName = String.Empty;
      comparedTowerName = String.Empty;
    }
    
  }
}
