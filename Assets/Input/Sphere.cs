using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sphere : MonoBehaviour
{
    private SphereInputAction _input;
    private bool _bounce= false;
    private Rigidbody _rb;
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _input = new SphereInputAction();
        _input.Enable();
        _input.Sphere.Bounce.performed += Bounce_performed;
        _input.Sphere.Bounce.canceled += Bounce_canceled;
    }

    private void Bounce_canceled(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        var duration = context.duration;
        
        
            _rb.AddForce(Vector3.up * (30f * (float)duration), ForceMode.Impulse);
        
    }

    private void Bounce_performed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        _bounce = true;
        _rb.AddForce(Vector3.up * 30f, ForceMode.Impulse);
    }
}
