using UnityEngine;

public class CollisionDetector : MonoBehaviour
{
    public LogicScript logic;
    void Start()
    {
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Touched: " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("TriggerWall"))
        {
            logic.addScore();
        }
    }
}
