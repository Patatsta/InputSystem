using System;
using UnityEngine;
using Game.Scripts.UI;

namespace Game.Scripts.LiveObjects
{
    public class InteractableZone : MonoBehaviour
    { 
        private enum ZoneType
        {
            Collectable,
            Action,
            HoldAction,
            BreakWall
        }   

        [SerializeField]
        private ZoneType _zoneType;
        [SerializeField]
        private int _zoneID;
        [SerializeField]
        private int _requiredID;
        [SerializeField]
        [Tooltip("Press the (---) Key to .....")]
        private string _displayMessage;
        [SerializeField]
        private GameObject[] _zoneItems;
     
        private bool _itemsCollected = false;
        private bool _actionPerformed = false;
        private bool _isHacked = false;
        private bool _isBroken = false;
        [SerializeField]
        private Sprite _inventoryIcon;
        [SerializeField]
        private KeyCode _zoneKeyInput;
        [SerializeField]
        private GameObject _marker;
        
        private static int _currentZoneID = 0;
        public static int CurrentZoneID
        { 
            get 
            {
                print(_currentZoneID);
               return _currentZoneID; 
            }
            set
            {
                _currentZoneID = value;
                print(_currentZoneID);

            }
        }


        public static event Action<InteractableZone> onZoneInteractionComplete;
        public static event Action<int> onHoldStarted;
        public static event Action<int> onHoldEnded;
        public static event Action<InteractableZone> onZoneEnter;
        public static event Action onZoneExit;
        public static event Action<InteractableZone> OnWallBreak;

        private void OnEnable()
        {
            InteractableZone.onZoneInteractionComplete += SetMarker;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && _currentZoneID > _requiredID)
            {
                
                onZoneEnter?.Invoke(this);

                switch (_zoneType)
                {
                    case ZoneType.Collectable:
                        if (_itemsCollected == false)
                        {

                            if (_displayMessage != null)
                            {
                                string message = $"Press the {_zoneKeyInput.ToString()} key to {_displayMessage}.";
                                UIManager.Instance.DisplayInteractableZoneMessage(true, message);
                            }
                            else
                                UIManager.Instance.DisplayInteractableZoneMessage(true, $"Press the {_zoneKeyInput.ToString()} key to collect");
                        }
                        break;

                    case ZoneType.Action:
                        if (_actionPerformed == false)
                        {

                            if (_displayMessage != null)
                            {
                                string message = $"Press the {_zoneKeyInput.ToString()} key to {_displayMessage}.";
                                UIManager.Instance.DisplayInteractableZoneMessage(true, message);
                            }
                            else
                                UIManager.Instance.DisplayInteractableZoneMessage(true, $"Press the {_zoneKeyInput.ToString()} key to perform action");
                        }
                        break;

                    case ZoneType.HoldAction:

                        if (_displayMessage != null)
                        {
                            if(_isHacked == false)
                            {
                                string message = $"Press the {_zoneKeyInput.ToString()} key to {_displayMessage}.";
                                UIManager.Instance.DisplayInteractableZoneMessage(true, message);
                            }              
                        }
                        else
                            UIManager.Instance.DisplayInteractableZoneMessage(true, $"Hold the {_zoneKeyInput.ToString()} key to perform action");
                        break;
                
                    case ZoneType.BreakWall:
                        if (_isBroken == false)
                        {

                            if (_displayMessage != null)
                            {
                                string message = $"Press the {_zoneKeyInput} key to {_displayMessage}.\nHold to perform a stronger punch.";

                                UIManager.Instance.DisplayInteractableZoneMessage(true, message);
                            }
                            else
                                UIManager.Instance.DisplayInteractableZoneMessage(true, $"Press the {_zoneKeyInput.ToString()} key to perform action");
                        }
                        break;

                }
            }
        }

        public void Interact_Performed()
        {
            print("pressed");
            switch (_zoneType)
            {
                case ZoneType.Collectable:
                  
                    if (_itemsCollected == false)
                    {
                        CollectItems();
                        _itemsCollected = true;
                        UIManager.Instance.DisplayInteractableZoneMessage(false);
                    }
                    break;

                case ZoneType.Action:
                    if (_actionPerformed == false)
                    {
                        PerformAction();
                        _actionPerformed = true;
                        UIManager.Instance.DisplayInteractableZoneMessage(false);
                    }
                    break;
                
                case ZoneType.HoldAction:
                    if(_isHacked == false)
                        PerformHoldAction();
                        break;
                case ZoneType.BreakWall:
                    if(_isBroken == false)  
                        PerformBreakAction();
                        break;
            }
        }
 
        private void PerformBreakAction()
        {
            foreach (var item in _zoneItems)
            {
                item.SetActive(true);
            }

            if (_inventoryIcon != null)
                UIManager.Instance.UpdateInventoryDisplay(_inventoryIcon);

            OnWallBreak?.Invoke(this);
        }
        
        public void Interact_Canceled()
        {
            print("canceled");
           
               
                onHoldEnded?.Invoke(_zoneID);
            
        }
       
        private void CollectItems()
        {
            foreach (var item in _zoneItems)
            {
                item.SetActive(false);
            }

            UIManager.Instance.UpdateInventoryDisplay(_inventoryIcon);

            CompleteTask(_zoneID);

            onZoneInteractionComplete?.Invoke(this);

        }

        private void PerformAction()
        {
            foreach (var item in _zoneItems)
            {
                item.SetActive(true);
            }

            if (_inventoryIcon != null)
                UIManager.Instance.UpdateInventoryDisplay(_inventoryIcon);

            onZoneInteractionComplete?.Invoke(this);
        }

        private void PerformHoldAction()
        {
            UIManager.Instance.DisplayInteractableZoneMessage(false);
            onHoldStarted?.Invoke(_zoneID);
        }

        public GameObject[] GetItems()
        {
            return _zoneItems;
        }

        public int GetZoneID()
        {
            return _zoneID;
        }

        public void CompleteTask(int zoneID)
        {
            if(zoneID == 3)
            {
                _isHacked = true;
            }else if (zoneID == 6)
            {
                _isBroken = true;
            }
            if (zoneID == _zoneID)
            {
                print(_currentZoneID);
                _currentZoneID++;

                print(_currentZoneID);
                onZoneInteractionComplete?.Invoke(this);
            }
        }

        public void ResetAction(int zoneID)
        {
            if (zoneID == _zoneID)
                _actionPerformed = false;
        }

        public void SetMarker(InteractableZone zone)
        {
            if (_zoneID == _currentZoneID)
                _marker.SetActive(true);
            else
                _marker.SetActive(false);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                onZoneExit?.Invoke();
               
                UIManager.Instance.DisplayInteractableZoneMessage(false);
            }
        }

        private void OnDisable()
        {
            InteractableZone.onZoneInteractionComplete -= SetMarker;
        }       
        
    }
}