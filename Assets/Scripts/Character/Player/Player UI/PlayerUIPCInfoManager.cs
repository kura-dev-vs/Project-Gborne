using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using Unity.Netcode;
using Unity.VisualScripting;
using TMPro;

namespace RK
{
    /// <summary>
    /// PCのステータス、装備等を表示するUI画面の管理マネージャー
    /// </summary>
    public class PlayerUIPCInfoManager : MonoBehaviour
    {
        float beforeFov;
        [HideInInspector] public int[] currentPTID = new int[4];
        public PlayableCharacter currentSelectingPC;
        public CanvasGroup pcInfoUIGroup;
        [SerializeField] CanvasGroup pcEquipmentUIGroup;
        public EquipmentWindow equipmentWindow;
        [Header("Upper UI")]
        [SerializeField] GameObject pcInfoIcon;
        [SerializeField] GameObject pcInfoIconContent;
        public GameObject pcInfoUI;
        GameObject tmpIcon;
        [Header("Left UI")]
        [SerializeField] ToggleGroup leftUIToggle;
        [SerializeField] Toggle[] leftUIToggleList;
        [Header("Details")]
        [SerializeField] GameObject detailsWindow;

        [Header("Weapons ")]
        [SerializeField] GameObject weaponWindow;
        [SerializeField] Image rightWeapon;
        [SerializeField] Image leftWeapon;
        [SerializeField] Image hatOutfit, jacketOutfit, topsOutfit, bottomsOutfit, shoesOutfit;
        [Header("Outfits")]
        [SerializeField] GameObject outfitsWindow;

        [Header("Profile")]
        [SerializeField] GameObject profileWindow;
        [SerializeField] Image pcImage;
        public void OpenUI()
        {
            pcInfoUI.SetActive(true);
            SetPCIcons();
            pcInfoUIGroup.interactable = true;
            pcInfoUIGroup.blocksRaycasts = true;
            PlayerUIManager.instance.playerUICurrentPTManager.ToggleCanvas(false);
            detailsWindow.GetComponent<PCDetailsManager>().RefreshRectTransform();
            beforeFov = PlayerCamera.instance.GetComponentInChildren<CinemachineUserInputZoom>().GetFov();
            PlayerCamera.instance.GetComponentInChildren<CinemachineUserInputZoom>().ChangeFov(-30);
            Debug.Log(beforeFov);
        }
        public void CloseUI()
        {
            leftUIToggleList[0].isOn = true;
            currentSelectingPC = null;
            detailsWindow.GetComponent<PCDetailsManager>().CloseUI();
            equipmentWindow.CloseUI();
            DestroyChildAll(pcInfoIconContent.transform);
            pcInfoUI.SetActive(false);
            PlayerUIManager.instance.playerUICurrentPTManager.ToggleCanvas(true);
            PlayerCamera.instance.GetComponentInChildren<CinemachineUserInputZoom>().ChangeFov(beforeFov);
        }
        private void DestroyChildAll(Transform parent)
        {
            foreach (Transform child in parent)
            {
                Destroy(child.gameObject);
            }
        }
        /// <summary>
        /// 所持しているPCを上部UI部分に横一列で表示する。
        /// 現在のPTメンバーを優先的に表示、 PTメンバーの表示後残りのメンバーをID順に表示する
        /// </summary>
        private void SetPCIcons()
        {
            if (pcInfoIconContent.transform == null)
                return;
            if (pcInfoIcon == null)
                return;

            EntryManager entry = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>().entry;

            for (int i = 0; i < entry.playableCharacterEntryNetworkManager.currentPTIDNetworkList.Count; i++)
                currentPTID[i] = entry.playableCharacterEntryNetworkManager.currentPTIDNetworkList[i];

            for (int i = 0; i < currentPTID.Length; i++)
            {
                if (currentPTID[i] == 0)
                {
                    continue;
                }
                tmpIcon = Instantiate(pcInfoIcon, pcInfoIconContent.transform, false);
                PCInfoIcon icon = tmpIcon.GetComponentInChildren<PCInfoIcon>();
                icon.pc = WorldPlayableCharacterDatabase.instance.GetPlayableCharacterByID(currentPTID[i]);
                icon.faceIcon.sprite = icon.pc.faceIcon;
                icon.ActiveJoinedSymbol();
                if (i == 0)
                {
                    currentSelectingPC = WorldPlayableCharacterDatabase.instance.GetPlayableCharacterByID(currentPTID[i]);
                    tmpIcon.GetComponent<Toggle>().isOn = true;
                    //ChangedMainDetails();
                }
            }

            for (int i = 1; i < WorldPlayableCharacterDatabase.instance.GetPlayableCharacterCount(); i++)
            {
                if (currentPTID.Contains(i))
                {
                    continue;
                }

                tmpIcon = Instantiate(pcInfoIcon, pcInfoIconContent.transform, false);
                PCInfoIcon icon = tmpIcon.GetComponentInChildren<PCInfoIcon>();
                icon.pc = WorldPlayableCharacterDatabase.instance.GetPlayableCharacterByID(i);
                icon.faceIcon.sprite = icon.pc.faceIcon;
            }

        }
        public void Back()
        {
            CloseUI();
        }

        /// <summary>
        /// 上部UIに表示されたPCアイコンをクリックした際のクリックイベント経由でこちらのメソッドが呼び出される
        /// 表示するPCを変更する
        /// </summary>
        /// <param name="changedPC"></param>
        public void SelectedCurrentPC(PlayableCharacter changedPC)
        {
            currentSelectingPC = changedPC;
            ChangedMainDetails();
        }

        /// <summary>
        /// 情報を表示する
        /// 左UIのトグルグループで選択されている情報を表示
        /// </summary>
        public void ChangedMainDetails()
        {
            int num = 0;
            foreach (Toggle tog in leftUIToggleList)
            {
                if (tog.isOn)
                    break;
                num++;
            }
            if (currentSelectingPC == null)
                return;

            detailsWindow.SetActive(false);
            weaponWindow.SetActive(false);
            profileWindow.SetActive(false);

            switch (num)
            {
                case 0:
                    detailsWindow.SetActive(true);
                    detailsWindow.GetComponent<PCDetailsManager>().SetDetails(currentSelectingPC);
                    break;
                case 1:
                    weaponWindow.SetActive(true);
                    Sprite transparent = WorldItemDatabase.instance.transparent;

                    rightWeapon.sprite = currentSelectingPC.rightWeapon.itemIcon;
                    leftWeapon.sprite = currentSelectingPC.leftWeapon.itemIcon;


                    hatOutfit.sprite = transparent;
                    jacketOutfit.sprite = transparent;
                    topsOutfit.sprite = transparent;
                    bottomsOutfit.sprite = transparent;
                    shoesOutfit.sprite = transparent;

                    if (currentSelectingPC.hatOutfit != null)
                        hatOutfit.sprite = currentSelectingPC.hatOutfit.itemIcon;
                    if (currentSelectingPC.jacketOutfit != null)
                        jacketOutfit.sprite = currentSelectingPC.jacketOutfit.itemIcon;
                    if (currentSelectingPC.topsOutfit != null)
                        topsOutfit.sprite = currentSelectingPC.topsOutfit.itemIcon;
                    if (currentSelectingPC.bottomsOutfit != null)
                        bottomsOutfit.sprite = currentSelectingPC.bottomsOutfit.itemIcon;
                    if (currentSelectingPC.shoesOutfit != null)
                        shoesOutfit.sprite = currentSelectingPC.shoesOutfit.itemIcon;
                    break;

                case 5:
                    profileWindow.SetActive(true);
                    pcImage.sprite = currentSelectingPC.fullBodyIcon;
                    break;
                default:
                    //detailsWindow.SetActive(true);
                    //detailsWindow.GetComponent<PCDetailsManager>().SetDetails(currentSelectingPC);
                    break;
            }
        }
        public void SelectedToggle(Toggle toggle)
        {
            if (toggle.isOn)
            {
                ChangedMainDetails();
            }
        }

        /// <summary>
        /// 装備UIで選択された部位の装備変更InventoryUIを表示する
        /// </summary>
        /// <param name="num"></param>
        public void CurrentOutfitSelected(int num)
        {
            equipmentWindow.OpenUI("outfit");
            switch (num)
            {
                case 0:
                    equipmentWindow.toggleEquipments[0].isOn = true;
                    equipmentWindow.ToggleHat();

                    break;
                case 1:
                    equipmentWindow.toggleEquipments[1].isOn = true;
                    equipmentWindow.ToggleJacket();
                    break;
                case 2:
                    equipmentWindow.toggleEquipments[2].isOn = true;
                    equipmentWindow.ToggleTops();
                    break;
                case 3:
                    equipmentWindow.toggleEquipments[3].isOn = true;
                    equipmentWindow.ToggleBottoms();
                    break;
                case 4:
                    equipmentWindow.toggleEquipments[4].isOn = true;
                    equipmentWindow.ToggleShoes();
                    break;
                default:
                    break;
            }
        }
        public void CurrentWeaponSelected(int num)
        {
            equipmentWindow.OpenUI("weapon");
            switch (num)
            {
                case 0:
                    equipmentWindow.toggleWeapons[0].isOn = true;
                    equipmentWindow.ToggleLeft();
                    break;
                case 1:
                    equipmentWindow.toggleWeapons[1].isOn = true;
                    equipmentWindow.ToggleRight();
                    break;
                default:
                    break;
            }
        }
    }
}
