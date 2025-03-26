using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    public class PlayerUIEquipmentManagerInputManager : MonoBehaviour
    {
        PlayerControls playercontrols;
        [Header("Inputs")]
        [SerializeField] bool unequipItemInput;
        private void Awake()
        {
        }
        private void OnEnable()
        {
            if (playercontrols == null)
            {
                playercontrols = new PlayerControls();

                playercontrols.PlayerActions.X.performed += i => unequipItemInput = true;
            }
            playercontrols.Enable();
        }
        private void OnDisable()
        {
            playercontrols.Disable();
        }
        private void Update()
        {
            HandlePlayerUIEquipmentManagerInputs();
        }
        private void HandlePlayerUIEquipmentManagerInputs()
        {
            if (unequipItemInput)
            {
                unequipItemInput = false;

            }
        }

    }
}
