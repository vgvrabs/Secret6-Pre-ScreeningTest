using System.Collections.Generic;
using UnityEngine;

public class AutoSolve : MonoBehaviour {

    public Queue<string[]> MovesQueue = new();

    public int CalculateTotalMoves(int blocks) {
        float totalMoves = Mathf.Pow(2, blocks) - 1;
        return (int) totalMoves;
    }

    public void GenerateAutoSolveMoves(int blocks) {
        MovesQueue.Clear();
        AutoSort(blocks, "TowerA", "TowerC", "TowerB");
        
        print("Moves Generated:" + MovesQueue.Count);
    }

    public int GetTotalMoves() {
        return MovesQueue.Count;
    }

    public string[] RemoveQueueMoves() {
        return MovesQueue.Dequeue();
    }

    public void AutoSort(int blocks, string source, string destination, string aux) {
        if (blocks == 1) {
            string[] moves = {source, destination};
            MovesQueue.Enqueue(moves);

            return;
        }
        
        AutoSort(blocks - 1, source, aux, destination);

        string[] secondMove = {source, destination};
        MovesQueue.Enqueue(secondMove);
        
        AutoSort(blocks - 1, aux, destination, source);
    }
}
