using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float m_velocity;

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.Rotate(Vector3.up, m_velocity * Time.deltaTime);
    }
}
