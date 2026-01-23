using UnityEngine;
using DG.Tweening;


public class ZZzzZZ : MonoBehaviour
{
    public GameObject monkey;
    private void Update() 
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("1");
            monkey.transform.DORotate(new Vector3(-90, -90, 0), 1.5f);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log("2");
            monkey.transform.DORotate(new Vector3(-90, 90, 0), 1.5f);
        }
    }
}