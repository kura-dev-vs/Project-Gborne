using System.Collections;
using System.Collections.Generic;
using RK;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// タイトルスクリーンメニュー専用のplayercontrol管理
/// </summary>
public class TitleScreenLoadMenuInputManager : MonoBehaviour
{
    PlayerControls playerControls;
    [Header("Title Screen Inputs")]
    [SerializeField] bool deleteCharacterSlot = false;
    private void Update()
    {
        if (deleteCharacterSlot)
        {
            deleteCharacterSlot = false;
            TitleScreenManager.instance.AttempToDeleteCharacterSlot();
        }
    }
    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();
        }
        playerControls.Enable();
        playerControls.UI.WestORF.performed += i => deleteCharacterSlot = true;
    }
    private void OnDisable()
    {
        playerControls.Disable();
    }
}
