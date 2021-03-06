﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ProjectileSettings : MonoBehaviour
{
    //rotation controls
    public float x_rotaspeed;
    public float y_rotaspeed;
    public float z_rotaspeed;

    public float m_Speed = 10f;   // default speed of projectile
    public float m_Lifespan = 3f; // Lifespan per second

    private Rigidbody m_Rigidbody;

    void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }
    void Start()
    {
        m_Rigidbody.AddForce(transform.forward * m_Speed);
        Destroy(gameObject, m_Lifespan);
    }
}