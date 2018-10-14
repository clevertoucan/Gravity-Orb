using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityControl : AbilityWithCooldown {
    static Matrix4x4 transformMatrix;
    Vector3 fillA, fillB;
    float gravityMagnitude = 9.81f;
    Vector3 rotation;
    Persistance persistance;

    // Use this for initialization
	protected override void Start () {
        base.Start();
        Input.gyro.enabled = true;
        persistance = Persistance.instance;
        transformMatrix = persistance.ReadData("GravityControl.transformMatrix", ZeroRotation(Vector3.back));
	}

    public void UpdateRotation() {
        transformMatrix = ZeroRotation();
        persistance.WriteData("GravityControl.transformMatrix", transformMatrix);
    }
    
    public Matrix4x4 ZeroRotation() {
        return ZeroRotation(Input.acceleration);
    }

    //https://en.wikipedia.org/wiki/Transformation_matrix
    public Matrix4x4 ZeroRotation(Vector3 zeroVector) {
        Matrix4x4 matrix;
        Vector3 zeroV = zeroVector, basis = Vector3.down, rotationAxis = new Vector3();
        Vector3.OrthoNormalize(ref zeroV, ref basis, ref rotationAxis);
        float angle = Vector3.SignedAngle(zeroVector, Vector3.down, rotationAxis) * Mathf.PI / 180;
        float cos = Mathf.Cos(angle), sin = Mathf.Sin(angle);
        float l = rotationAxis.x, m = rotationAxis.y, n = rotationAxis.z;
        matrix = new Matrix4x4();
        Vector3 row1 = new Vector3(
            l * l * ( 1 - cos ) + cos, 
            m * l * ( 1 - cos ) - n * sin, 
            n * l * ( 1 - cos ) + m * sin);
        Vector3 row2 = new Vector3(
            l * m * ( 1 - cos ) + n * sin, 
            m * m * ( 1 - cos + cos ), 
            n * m * ( 1 - cos ) - l * sin);
        Vector3 row3 = new Vector3(
            l * n * ( 1 - cos ) - m * sin,
            m * n * ( 1 - cos ) + l * sin,
            n * n * (1 - cos) + cos);
        matrix.SetRow(0, row1);
        matrix.SetRow(1, row2);
        matrix.SetRow(2, row3);
        return matrix;
    }

    public override void StartAbility() {
        ZeroRotation();
    }

    public override void StopAbility() {
        
    }


    // Update is called once per frame
    void Update () {
        if (Abilities.instance.CurrentEvasionAbility == evasionAbility) {
            Vector3 acc = transformMatrix * Input.acceleration.normalized;
            Physics.gravity = new Vector3(acc.x, acc.y, -acc.z) * gravityMagnitude;
        }
	}
}
