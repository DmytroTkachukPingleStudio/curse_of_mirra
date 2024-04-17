using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeSize : MonoBehaviour
{
    public float scaleFactor = 1.0f; // Factor by which to scale the shape size

    private ParticleSystem particleSystem;

    void Start()
    {
        // Get the ParticleSystem component
        particleSystem = GetComponent<ParticleSystem>();

        // Check if particle system exists
        if (particleSystem == null)
        {
            Debug.LogError("No ParticleSystem component found!");
            return;
        }

        // Scale the initial shape size
        ScaleParticleSystemShape();
    }

    // Function to scale the particle system shape size
    private void ScaleParticleSystemShape()
    {
        var shapeModule = particleSystem.shape;
        shapeModule.scale *= scaleFactor;
    }
}