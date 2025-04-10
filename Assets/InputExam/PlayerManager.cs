using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Scripts.Player;
using System;
using Game.Scripts.LiveObjects;
using System.Runtime.InteropServices;
public class PlayerManager : MonoBehaviour
{
    private ExamPlayerActions _input;
    [SerializeField] Player _player;
    private InteractableZone _interactableZone;
    [SerializeField] Laptop _laptop;
    [SerializeField] Drone _drone;
    private bool _hacked = false;
    [SerializeField]
    private Forklift _forklift;
    public static PlayerManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }else if (Instance != this)
        {
            Destroy(this);
        }
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        InteractableZone.onZoneEnter += ZoneEnter;
        InteractableZone.onZoneExit += ZoneExit;
        _input = new ExamPlayerActions();
        _input.Player.Enable();
        Drone.OnEnterFlightMode += Drone_OnEnterFlightMode;
        Drone.OnExitFlightMode += Drone_onExitFlightmode;
        Forklift.OnDriveModeEntered += EnterDriveMode;
        Forklift.OnDriveModeExited += ExitDriveMode;

    }

    private void Drone_OnEnterFlightMode()
    {
        _input.Player.Disable();
        _input.Drone.Enable();
        _input.Drone.Escape.performed += Escape_performed1;

    }

    private void Drone_onExitFlightmode()
    {
        _input.Player.Enable();
        _input.Drone.Disable();
        _input.Drone.Escape.performed -= Escape_performed1;
        
    }


    private void Escape_performed1(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        _drone.ExitFlightMode();
    }

    private void EnterDriveMode()
    {
        _input.Player.Disable();
        _input.Forklift.Enable();
        _input.Forklift.Escape.performed += Escape_performed2;
    }

    private void Escape_performed2(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        _forklift.ExitDriveMode();
    }

    private void ExitDriveMode()
    {
        _input.Player.Enable();
        _input.Forklift.Disable();
        _input.Forklift.Escape.performed -= Escape_performed2;
    }
    private void ZoneEnter(InteractableZone zone)
    {
        print("Zone");
        _interactableZone = zone;
        _input.Player.Interact.started += Interact_performed;
        _input.Player.Interact.canceled += Interact_canceled;
    }
    private void ZoneExit()
    {
        _interactableZone = null;
        _input.Player.Interact.started -= Interact_performed;
        _input.Player.Interact.canceled -= Interact_canceled;
    }
    private void Interact_canceled(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (_interactableZone != null)
        {

            _interactableZone.Interact_Canceled();
          
        }     
    }

    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        print("interact");
        if (_interactableZone != null && !_hacked)
        {
          _interactableZone.Interact_Performed(); 
        }
        else
        {
            _laptop.CameraSwitch();
        }
    }

    private void Update()
    {
        if (_input.Player.enabled)
        {
            var move = _input.Player.Walk.ReadValue<Vector2>();
            _player.CalcutateMovement(move);
        }
        else if (_input.Drone.enabled && _drone._inFlightMode)
        {
            
            var tilt = _input.Drone.Tilt.ReadValue<Vector2>();
            _drone.CalculateTilt(tilt);
            var move = _input.Drone.Move.ReadValue<float>();
            _drone.CalculateMovementUpdate(move);
        }else if (_input.Forklift.enabled && _forklift._inDriveMode)
        {
         
            var drive = _input.Forklift.Drive.ReadValue<Vector2>();
            _forklift.CalcutateMovement(drive);
            var lift = _input.Forklift.Lift.ReadValue<float>();
            _forklift.LiftControls(lift);
        }
      
    }
    private void FixedUpdate()
    {
        if (_input.Drone.enabled && _drone._inFlightMode)
        {
            var _direction = _input.Drone.Direction.ReadValue<float>();
            _drone.CalculateMovementFixedUpdate(_direction);
        }
    }
    public void OnHacked()
    {
        _hacked = true;
        _input.Player.Escape.performed += Escape_performed; ;
    }

    public void OnNotHacked()
    {
        _hacked = false;
    }
    private void Escape_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        _laptop.ExitCamera();
    }
}
