using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RK
{
    /// <summary>
    /// CurrentPTUIを管理するコンポーネント
    /// </summary>
    /// @Var characterSprites: 現在のPTを表示する。初期値をInspecterで設定し、動的に変更される。
    /// @Var characterNames: PTのキャラクターの名前。初期値をInspecterで設定し、動的に変更される。
    /// @Var currentPTID: 現在のPTID
    /// @Var ptSetUI: Inspecterで子のUI部分を設定する。
    public class PlayerUICurrentPTManager : MonoBehaviour
    {
        public Image[] characterSprites = new Image[4];
        public TextMeshProUGUI[] characterNames = new TextMeshProUGUI[4];
        int[] currentPTID = new int[4];
        public GameObject ptSetUI;
        public bool uiActivity = false;
        EntryManager entry;
        [SerializeField] GameObject first_Slot;
        public Slider hpSlider;
        public Slider stSlider;

        public void OpenUI()
        {
            if (NetworkManager.Singleton.LocalClient.PlayerObject != null)
                if (entry == null)
                    entry = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>().entry;

            Time.timeScale = 0f;
            uiActivity = true;
            ptSetUI.SetActive(true);
            if (entry != null)
            {
                for (int i = 0; i < entry.playableCharacterEntryNetworkManager.currentPTIDNetworkList.Count; i++)
                    currentPTID[i] = entry.playableCharacterEntryNetworkManager.currentPTIDNetworkList[i];
                RefreshCurrentPTUI(currentPTID);
            }

            PlayerUIManager.instance.playerUIHudManager.ToggleHUD(false);
            EventSystemManager.instance.eventSystem.SetSelectedGameObject(first_Slot);

            PlayerUIManager.instance.multiplayerTentativeScript.GetLocalIPAddress();
            if (PlayerCamera.instance.player != null)
            {
                hpSlider.value = PlayerCamera.instance.player.playerNetworkManager.vitality.Value;
                stSlider.value = PlayerCamera.instance.player.playerNetworkManager.endurance.Value;
            }
        }
        public void CloseUI()
        {
            Time.timeScale = 1f;
            uiActivity = false;
            PlayerUIManager.instance.playerUIPCInfoManager.CloseUI();
            PlayerUIManager.instance.playerUISelectableCharacterManager.CloseUI();
            PlayerUIManager.instance.playerUIHudManager.ToggleHUD(true);

            if (!(NetworkManager.Singleton.LocalClient.PlayerObject == null))
            {
                PlayerCamera.instance.player.playerNetworkManager.vitality.Value = (int)hpSlider.value;
                PlayerCamera.instance.player.playerNetworkManager.endurance.Value = (int)stSlider.value;
            }
            ptSetUI.SetActive(false);
        }
        /// <summary>
        /// UI更新時に表示するキャラクターの情報を変更する。
        /// </summary>
        /// <param name="index">PT内のindex</param>
        /// <param name="characterID">キャラクターのID</param>
        public void SetCharacterInfo(int index, int characterID)
        {
            PlayableCharacter pc = WorldPlayableCharacterDatabase.instance.GetPlayableCharacterByID(characterID);
            characterSprites[index].sprite = pc.fullBodyIcon;
            currentPTID[index] = characterID;
        }
        /// <summary>
        /// UIの更新時の挙動
        /// </summary>
        /// <param name="refreshPTID">更新後のPTID</param>
        public void RefreshCurrentPTUI(int[] refreshPTID)
        {
            int teamLength = refreshPTID.Length;
            for (int i = 0; i < teamLength; i++)
            {
                int pcID = refreshPTID[i];
                PlayerUIManager.instance.playerUICurrentPTManager.SetCharacterInfo(i, pcID);
            }
        }
        /// <summary>
        /// PTのボタン (画像) を押した時の挙動
        /// </summary>
        public void ClickPTSlots()
        {
            PlayerUIManager.instance.playerUISelectableCharacterManager.OpenUI();
        }
        public void ClickPCInfo()
        {
            PlayerUIManager.instance.playerUIPCInfoManager.OpenUI();
        }
        public void Deployed()
        {
            //entry.RefreshDeployedPT();
        }
        public void Back()
        {
            CloseUI();
        }
        public void Restart()
        {
            PlayerManager player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();
            WorldAIManager.instance.DespawnALLCharacters();
            WorldSoundFXManager.instance.StopBossMusic();
            if (player.isDead.Value)
            {
                player.respawnCharacter = true;
            }

            if (SceneManager.GetActiveScene().buildIndex == 2)
            {
                // ExtraSceneの場合
                WorldSaveGameManager.instance.CreateExtraScene(WorldSaveGameManager.instance.timeLimit, WorldSaveGameManager.instance.waveEnemy);
                WorldExtraManager.instance.RestartExtraScene();
                entry.playableCharacterInventoryManager.RefreshDeployedPT();
            }
            else
            {
                player.playerNetworkManager.currentHealth.Value = player.playerNetworkManager.maxHealth.Value;
                player.playerNetworkManager.currentStamina.Value = player.playerNetworkManager.maxStamina.Value;

                CharacterSaveData data = WorldSaveGameManager.instance.currentCharacterData;
                string worldScene = SceneUtility.GetScenePathByBuildIndex(data.sceneIndex);
                NetworkManager.Singleton.SceneManager.LoadScene(worldScene, LoadSceneMode.Single);
                Vector3 myPosition = new Vector3(data.xPosition, data.yPosition, data.zPosition);
                player.transform.position = myPosition;
            }
            player.playerNetworkManager.currentScore.Value = 0;
        }
        public void BackMainMenu()
        {
            CloseUI();
            WorldAIManager.instance.DespawnALLCharacters();
            WorldSoundFXManager.instance.StopAudio();
            Destroy(EventSystemManager.instance.gameObject);
            WorldSaveGameManager.instance.CreateMainMenu();
            //NetworkManager.Singleton.Shutdown();
        }
        public void ToggleCanvas(bool isInteractable)
        {
            GetComponent<CanvasGroup>().interactable = isInteractable;
            if (isInteractable)
            {
                GetComponent<CanvasGroup>().alpha = 1;
            }
            else
            {
                GetComponent<CanvasGroup>().alpha = 0;
            }
        }

        public void StartClientButton()
        {
            PlayerUIManager.instance.StartClient();
            CloseUI();
        }
    }
}
