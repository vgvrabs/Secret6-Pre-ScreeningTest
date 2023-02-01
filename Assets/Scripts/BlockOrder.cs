using UnityEngine;

public class BlockOrder : MonoBehaviour {
    public int Order;
    
    public void OnCollisionEnter(Collision collision) {
        GameObject other = collision.gameObject;
        
        /*if (other.CompareTag("Block")) {
            Physics.IgnoreCollision(GetComponent<Collider>(), other.GetComponent<Collider>());
        }*/
    }
}
