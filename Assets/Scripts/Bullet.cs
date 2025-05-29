using UnityEngine;

public class Bullet : MonoBehaviour
{
[SerializeField] float speed = 10f;
[SerializeField] float lifetime = 2f;

    void Start()
    {
        Destroy(gameObject, lifetime); // Destruye la bala después de un tiempo
    }

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }
}
