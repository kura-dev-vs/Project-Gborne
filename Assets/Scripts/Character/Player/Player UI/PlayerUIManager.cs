using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace RK
{
    /// <summary>
    /// playerそれぞれのUIを管理する大元のコンポーネント
    /// ここでプレイヤー自身のinstanceを指定し、各UIのManagerへアクセス
    /// </summary>
    public class PlayerUIManager : MonoBehaviour
    {
        public static PlayerUIManager instance;
        [Header("NETWORK JOIN")]
        [SerializeField] bool startGameAsClient;
        public PlayerUIHudManager playerUIHudManager;
        [HideInInspector] public PlayerUIPopUpManager playerUIPopUpManager;
        [HideInInspector] public PlayerUICurrentPTManager playerUICurrentPTManager;
        [HideInInspector] public PlayerUISelectablePCManager playerUISelectableCharacterManager;
        [HideInInspector] public PlayerUIPCInfoManager playerUIPCInfoManager;
        [HideInInspector] public PlayerUICharacterMenuManager playerUICharacterMenuManager;

        public MultiplayerTentativeScript multiplayerTentativeScript;

        [Header("UI Flags")]
        public bool menuWindowIsOpen = false; // inventory screen, equipment menu, blacksmith menu ect
        public bool popUpWindowIsOpen = false; // item pick up, dialogue pop up ect
        Canvas myCanvas;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            playerUIHudManager = GetComponentInChildren<PlayerUIHudManager>();
            playerUIPopUpManager = GetComponentInChildren<PlayerUIPopUpManager>();
            playerUICurrentPTManager = GetComponentInChildren<PlayerUICurrentPTManager>();
            playerUISelectableCharacterManager = GetComponentInChildren<PlayerUISelectablePCManager>();
            playerUIPCInfoManager = GetComponentInChildren<PlayerUIPCInfoManager>();
            multiplayerTentativeScript = GetComponent<MultiplayerTentativeScript>();
            playerUICharacterMenuManager = GetComponentInChildren<PlayerUICharacterMenuManager>();
            myCanvas = GetComponent<Canvas>();
            myCanvas.enabled = false;
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }
        private void Update()
        {
            if (startGameAsClient)
            {
                startGameAsClient = false;

                NetworkManager.Singleton.Shutdown();

                NetworkManager.Singleton.StartClient();
            }
        }
        public void DestroyChildAll(Transform parent)
        {
            foreach (Transform child in parent)
            {
                Destroy(child.gameObject);
            }
        }
        public void OpenMenuUI()
        {
            if (playerUICurrentPTManager.uiActivity)
            {
                playerUICurrentPTManager.CloseUI();
            }
            else
            {
                playerUICurrentPTManager.OpenUI();
            }
        }
        public void StartClient()
        {
            if (!startGameAsClient)
            {
                if (PlayerCamera.instance.player != null)
                {
                    if (PlayerCamera.instance.player.IsServer)
                    {
                        startGameAsClient = false;
                    }
                }
                else
                {
                    startGameAsClient = true;
                }
            }
        }
        public void ActivateHUD()
        {
            myCanvas.enabled = true;
        }
        public void InActivateHUD()
        {
            myCanvas.enabled = false;
        }
        public void CloseAllMenuWindows()
        {
            playerUICharacterMenuManager.CloseCharacterMenu();
        }
        public void ToggleHUD(bool status)
        {

        }
    }
}
