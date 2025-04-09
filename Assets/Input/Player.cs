using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;
using UnityEngine;

public class PlayerMe : MonoBehaviour
{
    private PlayerInputAction _input;
    private MeshRenderer _renderer;

    private void Start()
    {
        _renderer = GetComponent<MeshRenderer>();
        _input = new PlayerInputAction();
      
        _input.Player.Enable();
    
      
        _input.Player.Color.performed += Color_performed;
        _input.Player.DrivingState.performed += DrivingState_performed;
    }

    private void DrivingState_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        _input.Player.Disable();
        _input.Car.Enable();
    }


    private void Color_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
     
        _renderer.material.color = Random.ColorHSV();
    }

    private void Update()
    {
        Move();   
    }

    private void Move()
    {
      
        var rot = _input.Player.Rotate.ReadValue<float>();

        transform.Rotate(new Vector3(0,rot,0) * Time.deltaTime * 30);

        var move = _input.Car.Move.ReadValue<Vector2>();

        transform.Translate(new Vector3(move.x, 0 , move.y) * Time.deltaTime * 5);
    }
}
