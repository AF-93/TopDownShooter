using UnityEngine;

public class Item : MonoBehaviour
{   [SerializeField] private int HpUp;
    void OnTriggerEnter(Collider other){
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerController player = other.gameObject.GetComponent<PlayerController>();
            player.LPPlayer(HpUp);
        }
    }
}
