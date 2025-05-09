using System;
using UnityEngine;
using Cinemachine;
using Game.Scripts.UI;

namespace Game.Scripts.LiveObjects
{
    public class Drone : MonoBehaviour
    {
        private enum Tilt
        {
            NoTilt, Forward, Back, Left, Right
        }

        [SerializeField]
        private Rigidbody _rigidbody;
        [SerializeField]
        private float _speed = 5f;
        public bool _inFlightMode = false;
        [SerializeField]
        private Animator _propAnim;
        [SerializeField]
        private CinemachineVirtualCamera _droneCam;
        [SerializeField]
        private InteractableZone _interactableZone;
        [SerializeField]
        private GameObject _drone_UI;
        public static event Action OnEnterFlightMode;
        public static event Action OnExitFlightMode;

        private void Start()
        {
            _drone_UI.SetActive(false);
        }
        private void OnEnable()
        {
            InteractableZone.onZoneInteractionComplete += EnterFlightMode;
        }

        private void EnterFlightMode(InteractableZone zone)
        {
            if (_inFlightMode != true && zone.GetZoneID() == 4) // drone Scene
            {
                _drone_UI.SetActive(true);
                _propAnim.SetTrigger("StartProps");
                _droneCam.Priority = 11;
                _inFlightMode = true;
                OnEnterFlightMode?.Invoke();
                UIManager.Instance.DroneView(true);
                _interactableZone.CompleteTask(4);
            }
        }

        public void ExitFlightMode()
        {
            _drone_UI.SetActive(false);
            OnExitFlightMode?.Invoke(); 
            _droneCam.Priority = 9;
            _inFlightMode = false;
            UIManager.Instance.DroneView(false);            
        }

        private void FixedUpdate()
        {
            _rigidbody.AddForce(transform.up * (9.81f), ForceMode.Acceleration);

        }

    
        public void CalculateMovementUpdate(float move)
        {
            var tempRot = transform.localRotation.eulerAngles;
            tempRot.y += move *_speed / 3;
            transform.localRotation = Quaternion.Euler(tempRot);
        }

  
        public void CalculateMovementFixedUpdate(float direction)
        {
            _rigidbody.AddForce(transform.up * direction * _speed, ForceMode.Acceleration);
        }


        public void CalculateTilt(Vector2 tiltInput)
        {
            if (tiltInput == Vector2.zero)
            {
                transform.rotation = Quaternion.Euler(0, transform.localRotation.eulerAngles.y, 0);
            }
            else if (tiltInput.x < 0)
            {
                transform.rotation = Quaternion.Euler(0, transform.localRotation.eulerAngles.y, 30);
            }
            else if (tiltInput.x > 0)
            {
                transform.rotation = Quaternion.Euler(0, transform.localRotation.eulerAngles.y, -30);
            }
            else if (tiltInput.y > 0)
            {
                transform.rotation = Quaternion.Euler(30, transform.localRotation.eulerAngles.y, 0);
            }
            else if (tiltInput.y < 0)
            {
                transform.rotation = Quaternion.Euler(-30, transform.localRotation.eulerAngles.y, 0);
            }
        }
       


        private void OnDisable()
        {
            InteractableZone.onZoneInteractionComplete -= EnterFlightMode;
        }
    }
}

