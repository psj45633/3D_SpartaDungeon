using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperJump : MonoBehaviour
{
    public float jumpForce;

    private void Start()
    {

    }
    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            Rigidbody _rb = col.GetComponent<Rigidbody>();
            if (_rb != null)
            {
                _rb.velocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z); // 점프하는 높이 항상 같음
                _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                Debug.Log("점프");
            }
        }
    }
}
