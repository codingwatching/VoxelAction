using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCS : MonoBehaviour
{
    public enum Type { Ammo, Coin, Grenade, Heart, Weapon };
    public Type type;
    public int value;

    Rigidbody rigidbody;
    SphereCollider sphereCollider;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        // ����
        transform.Rotate(Vector3.up * 20 * Time.deltaTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            rigidbody.isKinematic = true; // ����ȿ���� ������� �ʵ��� ����
            sphereCollider.enabled = false;
        }
    }
}
