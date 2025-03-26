using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using System.Data.Common;
using TMPro;

namespace RK
{
    /// <summary>
    /// タイトルスクリーン上での挙動, ボタンを押したときのイベント管理
    /// </summary>
    public class TitleScreenManager : MonoBehaviour
    {
        public static TitleScreenManager instance;
        [Header("Menus")]
        [SerializeField] GameObject titleScreenMainMenu;
        [SerializeField] GameObject titleScreenLoadMenu;

        [Header("Buttons")]
        [SerializeField] Button loadMenuReturnButton;
        [SerializeField] Button mainMenuLoadGameButton;
        [SerializeField] Button mainMenuNewGameButton;
        [SerializeField] Button deleteCharacterPopUpConfirmButton;
        [Header("Pop Ups")]
        [SerializeField] GameObject noCharacterSlotsPop;
        [SerializeField] Button noCharacterSlotsOkeyButton;
        [SerializeField] GameObject deleteCharacterSlotPopUp;
        [Header("Character Slots")]
        public CharacterSlot currentSelectedSlot = CharacterSlot.NO_SLOT;

        [Header("Title Screen Inputs")]
        [SerializeField] bool deleteCharacterSlot = false;

        [Header("Extra Scene")]
        [SerializeField] TextMeshProUGUI timeLimit;
        public Slider timeSlider;

        [SerializeField] TextMeshProUGUI waveEnemy;
        public Slider enemySlider;
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
        }
        private void Update()
        {
            timeLimit.SetText("{0}", timeSlider.value);
            waveEnemy.SetText("{0}", enemySlider.value);
        }
        public void StartNetworkAsHost()
        {
            // 一旦以下の行は消しているがNetcode関係でエラーが起きた場合はWorldSaveGameManagerのNewGame(), LoadGame(), ExtraGame()を直したあと以下を復活させる
            //NetworkManager.Singleton.StartHost();
            // 一旦以上の行は消しているがNetcode関係でエラーが起きた場合はWorldSaveGameManagerのNewGame(), LoadGame(), ExtraGame()を直したあと以上を復活させる
        }
        public void StartNewGame()
        {
            WorldSaveGameManager.instance.AttemptToCreateNewGame();
        }
        public void OpenLoadGameMenu()
        {
            titleScreenMainMenu.SetActive(false);

            titleScreenLoadMenu.SetActive(true);
            // 最初にリセッﾄボタンを選択
            loadMenuReturnButton.Select();
        }
        public void CloseLoadGameMenu()
        {
            titleScreenLoadMenu.SetActive(false);

            titleScreenMainMenu.SetActive(true);
            // 最初にロードゲームボタンを選択
            mainMenuLoadGameButton.Select();
        }
        public void DisplayNoFreeCharacterSlotsPopUp()
        {
            noCharacterSlotsPop.SetActive(true);
            noCharacterSlotsOkeyButton.Select();
        }
        public void CloseNoFreeCharacterSlotsPopUp()
        {
            noCharacterSlotsPop.SetActive(false);
            mainMenuNewGameButton.Select();
        }
        // character slots
        public void SelectCharacterSlot(CharacterSlot characterSlot)
        {
            currentSelectedSlot = characterSlot;
        }
        public void SelectNoSlot()
        {
            currentSelectedSlot = CharacterSlot.NO_SLOT;
        }
        public void AttempToDeleteCharacterSlot()
        {
            if (currentSelectedSlot != CharacterSlot.NO_SLOT)
            {
                deleteCharacterSlotPopUp.SetActive(true);
                deleteCharacterPopUpConfirmButton.Select();
            }
        }
        public void DeleteCharacterSlot()
        {
            deleteCharacterSlotPopUp.SetActive(false);
            WorldSaveGameManager.instance.DeleteGame(currentSelectedSlot);
            // ロードメニューを無効にしてから有効にし、スロットをリフレッシュする（削除されたスロットは無効になる)
            titleScreenLoadMenu.SetActive(false);
            titleScreenLoadMenu.SetActive(true);
            loadMenuReturnButton.Select();
        }
        public void CloseDeleteCharacterButton()
        {
            deleteCharacterSlotPopUp.SetActive(false);
            loadMenuReturnButton.Select();
        }

        public void StartExtraGame()
        {
            WorldSaveGameManager.instance.CreateExtraScene(timeSlider.value, enemySlider.value);
        }
        public void ExitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else
    Application.Quit();//ゲームプレイ終了
#endif
        }
    }
}
