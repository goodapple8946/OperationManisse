using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
    private ParticleSystem ParticleSystem;

    void Start()
    {
        ParticleSystem = gameObject.GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (!ParticleSystem.IsAlive())
        {
            Destroy(gameObject);
        }
    }
}
