using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderClass : MonoBehaviour
{
    private Slider _slider;
    private SphereInputAction _input;
    private bool _isCharging = false;
    private void Start()
    {
        _input = new SphereInputAction();
        _input.Enable();
        _slider = GetComponent<Slider>();

        _input.Sphere.Slider.started += Slider_performed;
        _input.Sphere.Slider.canceled += Slider_canceled;
    }

    private void Slider_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
     
        _isCharging = false;
    }

    private void Slider_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
       
        _isCharging = true; 
        StartCoroutine(ChargeBarRoutine());
    }

    IEnumerator ChargeBarRoutine()
    {
        while (_isCharging == true)
        {
            _slider.value += 1.0f * Time.deltaTime;
            yield return null;
        }

        while(_slider.value > 0.0f)
        {
         
            _slider.value -= 1.0f * Time.deltaTime;
            yield return null;
        }
            
    }
}
