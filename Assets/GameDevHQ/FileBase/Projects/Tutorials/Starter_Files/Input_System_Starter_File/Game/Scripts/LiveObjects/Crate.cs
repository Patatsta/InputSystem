
using Game.Scripts.UI;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Game.Scripts.LiveObjects
{
    public class Crate : MonoBehaviour
    {
        [SerializeField] private float _punchDelay;
        [SerializeField] private GameObject _wholeCrate, _brokenCrate;
        [SerializeField] private Rigidbody[] _pieces;
        [SerializeField] private BoxCollider _crateCollider;
        [SerializeField] private InteractableZone _interactableZone;
        private bool _isReadyToBreak = false;

        private List<Rigidbody> _brakeOff = new List<Rigidbody>();

        private void OnEnable()
        {
            InteractableZone.OnWallBreak += InteractableZone_OnWallBreak;
            InteractableZone.onHoldEnded += InteractableZone_onHoldEnded;
        }

        private float _holdStartTime;

        private void InteractableZone_OnWallBreak(InteractableZone zone)
        {
            if (_isReadyToBreak == false && _brakeOff.Count > 0)
            {
               
                _wholeCrate.SetActive(false);
                _brokenCrate.SetActive(true);
                _isReadyToBreak = true;
            }
            else if (zone.GetZoneID() == 6 && _isReadyToBreak)
            {
             
                _holdStartTime = Time.time;
            }
        }

        private void InteractableZone_onHoldEnded(int zoneID)
        {      
            if (zoneID == 6 && _isReadyToBreak)
            {          
                float heldTime = Mathf.Clamp(Time.time - _holdStartTime, 0f, 1f);
                int blocksToBreak = Mathf.Max(1, Mathf.CeilToInt(heldTime / (1f / 3f)));
                int iterations = Mathf.Min(blocksToBreak, _brakeOff.Count);
             
                for (int i = 0; i < iterations; i++)
                {
                    Debug.Log("break " + i);
                    BreakPart();
                }

                StartCoroutine(PunchDelay());

                if (_brakeOff.Count <= 0)
                {
                    UIManager.Instance.DisplayInteractableZoneMessage(false);
                    _isReadyToBreak = false;
                    _crateCollider.enabled = false;
                    _interactableZone.CompleteTask(6);
                    Debug.Log("Completely Busted");
                }
            }
        }

        private void Start()
        {
            _brakeOff.AddRange(_pieces);     
        }

        public void BreakPart()
        {
            int rng = Random.Range(0, _brakeOff.Count);
            _brakeOff[rng].constraints = RigidbodyConstraints.None;
            _brakeOff[rng].AddForce(new Vector3(1f, 1f, 1f), ForceMode.Force);
            _brakeOff.Remove(_brakeOff[rng]);            
        }

        IEnumerator PunchDelay()
        {
            float delayTimer = 0;
            while (delayTimer < _punchDelay)
            {
                yield return new WaitForEndOfFrame();
                delayTimer += Time.deltaTime;
            }

            _interactableZone.ResetAction(6);
        }

        private void OnDisable()
        {
            InteractableZone.OnWallBreak -= InteractableZone_OnWallBreak;
        }
    }
}
