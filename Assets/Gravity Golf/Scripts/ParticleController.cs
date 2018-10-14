using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour {
    /*
    ParticleSystem system;
    ParticleSystem.ForceOverLifetimeModule force;
    ParticleSystem.EmissionModule emission;
    ParticleSystem.MainModule main;
    GameController game;
    float baseEmission, magnitude;
    public float baseGravity;
    public float emissionConstant = 1f;

	// Use this for initialization
	void Start () {
        game = GameController.Game;
        system = GetComponent<ParticleSystem>();
        force = system.forceOverLifetime;
        force.space = ParticleSystemSimulationSpace.World;
        emission = system.emission;
        baseEmission = emission.rateOverTime.constant; 

        main = system.main;
    }
	
	// Update is called once per frame
	void Update () {
        
        if (game.Orientation.sqrMagnitude > .1) {
            magnitude = game.Orientation.magnitude;
            float value = GameController.gravityConstant / (magnitude);
            force.x = game.Orientation.x * value;
            force.z = game.Orientation.z * value;
            emission.rateOverTime = Mathf.Clamp(baseEmission * magnitude * emissionConstant, baseEmission, 200);
        } else {
            float forceMagnitude = Mathf.Sqrt((force.x.constant * force.x.constant) + (force.z.constant * force.z.constant));
            if (forceMagnitude > baseGravity) {
                force.x = force.x.constant * .9f;
                force.z = force.z.constant * .9f;
                emission.rateOverTime = Mathf.Clamp(emission.rateOverTime.constant * .9f, baseEmission, 200);
                main.startSpeed = forceMagnitude;
            } else {
                main.startSpeed = 0;
            }
        }
        
	}
    */
}
