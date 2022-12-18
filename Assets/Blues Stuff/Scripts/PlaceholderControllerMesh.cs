using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceholderControllerMesh : MonoBehaviour
{
    public CharacterController Controller;
    public Transform CylinderMesh;
    private void OnDrawGizmosSelected()
    {
        Vector3 scale = CylinderMesh.localScale;
        scale.x = (Controller.radius + Controller.skinWidth) * 2;
        scale.y = Controller.height / 2;
        scale.z = (Controller.radius + Controller.skinWidth) * 2;
        CylinderMesh.localScale = scale;
    }
}
