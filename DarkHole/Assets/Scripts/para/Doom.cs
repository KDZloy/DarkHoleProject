using UnityEngine;
using DG.Tweening;


public class Doom : MonoBehaviour
{
    void Start()
    {

        transform.DOMove(new Vector3(5, 0, 0), 2f);
    

        transform.DOScale(2f, 1f);
    

        transform.DORotate(new Vector3(0, 90, 0), 1.5f);
    }
}
