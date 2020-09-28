using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
	// 粒子所在系统
    private ParticleSystem _particleSystem;

    void Start()
    {
		_particleSystem = gameObject.GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (!_particleSystem.IsAlive())
        {
            Destroy(gameObject);
        }
    }
}
