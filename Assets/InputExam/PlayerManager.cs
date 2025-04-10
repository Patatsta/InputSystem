using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Scripts.Player;
using System;
using Game.Scripts.LiveObjects;
public class PlayerManager : MonoBehaviour
{
    private ExamPlayerActions _input;
    [SerializeField] Player _player;
    private InteractableZone _interactableZone;
    [SerializeField] Laptop _laptop;
    private bool _hacked = false;
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
        var move = _input.Player.Walk.ReadValue<Vector2>();
        _player.CalcutateMovement(move);
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
