using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public Vector3 MoveAdd;
    public float Speed;
    int Dir = 1;

    void Update()
    {
        transform.position += MoveAdd * Speed * Dir * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) Debug.Log("Touched!");
    }

    public void ChangeDir(bool left)
    {
        Dir = left ? -1 : 1;
    }
}
