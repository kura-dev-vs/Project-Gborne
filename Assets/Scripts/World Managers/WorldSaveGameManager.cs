using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using UnityEngine.TextCore.Text;
using System.Linq;
using System;

namespace RK
{
    /// <summary>
    /// 
    /// </summary> 
    public class WorldSaveGameManager : MonoBehaviour
    {
        public static WorldSaveGameManager instance;
        public PlayerManager player;
        [Header("SAVE/LOAD")]
        [SerializeField] bool saveGame;
        [SerializeField] bool loadGame;

        [Header("World Scene Index")]
        [SerializeField] int[] battleSceneIndex;
        [SerializeField] int worldSceneIndex = 1;
        [SerializeField] int extraSceneIndex = 2;
        [Header("Save Data Writer")]
        private SaveFileDataWriter saveFileDataWriter;
        [Header("Current Character Data")]
        public CharacterSlot currentCharacterSlotBeingUsed;
        public CharacterSaveData currentCharacterData;
        private string saveFileName;

        [Header("Character Slots")]
        public CharacterSaveData
        characterSlots01, characterSlots02, characterSlots03;
        [Header("Extra Scene")]
        [HideInInspector] public float timeLimit = 60.0f;
        [HideInInspector] public int waveEnemy = 10;
        private void Awake()
        {
            // このスクリプトのオブジェクトは一度に一つのみ存在
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            LoadAllCharacterProfiles();
        }
        private void Update()
        {
            if (saveGame)
            {
                saveGame = false;
                SaveGame();
            }
            if (loadGame)
            {
                loadGame = false;
                LoadGame();
            }
        }
        // ファイル名をスロットによって変更する
        public string DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot characterSlot)
        {
            string fileName = "";
            switch (characterSlot)
            {
                case CharacterSlot.CharacterSlot_01:
                    fileName = "characterSlot_01";
                    break;
                case CharacterSlot.CharacterSlot_02:
                    fileName = "characterSlot_02";
                    break;
                case CharacterSlot.CharacterSlot_03:
                    fileName = "characterSlot_03";
                    break;
                default:
                    break;
            }
            return fileName;
        }

        /// <summary>
        /// new gameを始める際にセーブデータスロットのチェックを行う
        /// スロットの使用状況を上から確認し、空いている場合は始めることができる
        /// </summary>
        public void AttemptToCreateNewGame()
        {
            saveFileDataWriter = new SaveFileDataWriter();
            saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;

            // 新しいセーブファイルを作成できるかチェックする（まず他のファイルが存在しないかチェックする）
            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_01);

            if (!saveFileDataWriter.CheckToSeeIfFileExists())
            {
                // このスロットが使用されていない場合は使用できる
                currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_01;
                currentCharacterData = new CharacterSaveData();
                NewGame();
                return;
            }

            // 新しいセーブファイルを作成できるかチェックする（まず他のファイルが存在しないかチェックする）
            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_02);

            if (!saveFileDataWriter.CheckToSeeIfFileExists())
            {
                // このスロットが使用されていない場合は使用できる
                currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_02;
                currentCharacterData = new CharacterSaveData();
                NewGame();
                return;
            }

            // 新しいセーブファイルを作成できるかチェックする（まず他のファイルが存在しないかチェックする）
            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_03);

            if (!saveFileDataWriter.CheckToSeeIfFileExists())
            {
                // このスロットが使用されていない場合は使用できる
                currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_03;
                currentCharacterData = new CharacterSaveData();
                NewGame();
                return;
            }
            // 空き枠がない場合、プレーヤーに通知する
            TitleScreenManager.instance.DisplayNoFreeCharacterSlotsPopUp();
        }
        /// <summary>
        /// 新規に作成したキャラクターのステータスやアイテムを設定してから作成する
        /// </summary>
        public void NewGame()
        {
            // もしNetcode関係でエラーが起きたら以下の行を消す
            NetworkManager.Singleton.StartHost();
            // もしNetcode関係でエラーが起きたら以上の行を消す

            player.playerNetworkManager.vitality.Value = 15;
            player.playerNetworkManager.endurance.Value = 10;

            SaveGame();
            //StartCoroutine(LoadWorldSceneCoroutine(worldSceneIndex));
            LoadWorldScene(worldSceneIndex);
        }
        /// <summary>
        /// ゲームのロード
        /// Application.persistentDataPathで得たパス下に保存したセーブデータをファイル名で特定してロード
        /// </summary>
        public void LoadGame()
        {
            // もしNetcode関係でエラーが起きたら以下の行を消す
            NetworkManager.Singleton.StartHost();
            // もしNetcode関係でエラーが起きたら以上の行を消す

            // どのスロットを使うかによってファイル名を変えて、前のファイルをロードする。
            saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(currentCharacterSlotBeingUsed);

            saveFileDataWriter = new SaveFileDataWriter();
            // 複数のマシンタイプで動作するライブラリ (Application.persistentDataPath)
            // セーブデータの保存に適した読み取り専用の永続データ ディレクトリパスらしい
            saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
            saveFileDataWriter.saveFileName = saveFileName;
            currentCharacterData = saveFileDataWriter.LoadSaveFile();

            //StartCoroutine(LoadWorldSceneCoroutine(worldSceneIndex));
            LoadWorldScene(worldSceneIndex);
        }
        /// <summary>
        /// ゲームのセーブ
        /// </summary>
        public void SaveGame()
        {
            if (SceneManager.GetActiveScene().buildIndex == extraSceneIndex)
                return;

            // どのスロットを使用しているかに応じて、現在のファイルをファイル名で保存する。
            saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(currentCharacterSlotBeingUsed);

            saveFileDataWriter = new SaveFileDataWriter();
            // 複数のマシンタイプで動作するライブラリ (Application.persistentDataPath)
            // セーブデータの保存に適した読み取り専用の永続データ ディレクトリパスらしい
            saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
            saveFileDataWriter.saveFileName = saveFileName;

            // プレイヤー情報をゲームからセーブファイルに渡す
            player.SaveGameDataToCurrentCharacterData(ref currentCharacterData);

            // その情報をjsonファイルに書き出し、このマシンに保存する
            saveFileDataWriter.CreateNewCharacterSaveFile(currentCharacterData);


        }
        /// <summary>
        /// 引数で与えられたセーブスロット上のセーブデータの削除
        /// </summary>
        /// <param name="characterSlot"></param>
        public void DeleteGame(CharacterSlot characterSlot)
        {
            saveFileDataWriter = new SaveFileDataWriter();
            saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

            saveFileDataWriter.DeleteSaveFile();
        }
        // ゲーム起動時にキャラクターのステータス等をロード
        private void LoadAllCharacterProfiles()
        {
            saveFileDataWriter = new SaveFileDataWriter();
            saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;

            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_01);
            characterSlots01 = saveFileDataWriter.LoadSaveFile();

            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_02);
            characterSlots02 = saveFileDataWriter.LoadSaveFile();

            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_03);
            characterSlots03 = saveFileDataWriter.LoadSaveFile();
        }

        public IEnumerator LoadWorldSceneCoroutine(int buildIndex)
        {
            // ワールドシーンを1つだけ作りたい場合
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(buildIndex);

            // sセーブデータ毎に異なるシーンを読み込む場合
            // AsyncOperation loadOperation = SceneManager.LoadSceneAsync(currentCharacterData.sceneIndex);
            if (buildIndex != 0)
            {
                PlayerUIManager.instance.ActivateHUD();
                if (player == null)
                {
                    PlayerUIManager.instance.playerUICurrentPTManager.OpenUI();
                }
                else
                {
                    player.LoadGameDataFromCurrentCharacterData(ref currentCharacterData);
                }
            }
            else
            {
                // Netcode関係でもしエラーが起きた場合は以下の行を消してみる
                PlayerUIManager.instance.DestroyChildAll(PlayerUIManager.instance.playerUIHudManager.characterSlotParent);
                NetworkManager.Singleton.Shutdown();
                // もしエラーが起きた場合は以上の行を消す
                PlayerUIManager.instance.InActivateHUD();
            }
            yield return null;
        }


        public void LoadWorldScene(int buildIndex)
        {
            string worldScene = SceneUtility.GetScenePathByBuildIndex(buildIndex);
            Debug.Log(NetworkManager.Singleton.SceneManager);
            if (NetworkManager.Singleton.SceneManager == null)
            {
                AsyncOperation loadOperation = SceneManager.LoadSceneAsync(buildIndex);
            }
            else
            {
                if (!player.IsServer)
                {
                    AsyncOperation loadOperation = SceneManager.LoadSceneAsync(buildIndex);
                }
                else
                {
                    NetworkManager.Singleton.SceneManager.LoadScene(worldScene, LoadSceneMode.Single);
                }
            }

            if (buildIndex != 0)
            {
                PlayerUIManager.instance.ActivateHUD();
                if (player == null)
                {
                    PlayerUIManager.instance.playerUICurrentPTManager.OpenUI();
                }
                else
                {
                    player.LoadGameDataFromCurrentCharacterData(ref currentCharacterData);
                }
            }
            else
            {
                // Netcode関係でもしエラーが起きた場合は以下の行を消してみる
                PlayerUIManager.instance.DestroyChildAll(PlayerUIManager.instance.playerUIHudManager.characterSlotParent);
                NetworkManager.Singleton.Shutdown();
                // もしエラーが起きた場合は以上の行を消す
                PlayerUIManager.instance.InActivateHUD();
            }
        }


        public int GetWorldSceneIndex()
        {
            return worldSceneIndex;
        }
        public bool CheckBattleScene(int scene)
        {
            bool result = battleSceneIndex.Contains(scene);
            return result;
        }

        // extra sceneの作成
        public void CreateExtraScene(float timeLimit, float waveEnemy)
        {
            // もしNetcode関係でエラーが起きたら以下の行を消す
            NetworkManager.Singleton.StartHost();
            // もしNetcode関係でエラーが起きたら以上の行を消す
            this.timeLimit = timeLimit;
            this.waveEnemy = (int)waveEnemy;
            currentCharacterData = new CharacterSaveData();
            player.transform.position = new Vector3(0, 0, 0);
            player.playerNetworkManager.vitality.Value = 20;
            player.playerNetworkManager.endurance.Value = 20;
            player.playerNetworkManager.currentHealth.Value = player.playerNetworkManager.maxHealth.Value;
            player.playerNetworkManager.currentStamina.Value = player.playerNetworkManager.maxStamina.Value;
            player.inventory.itemsInInventory.Clear();
            player.SaveGameDataToCurrentCharacterData(ref currentCharacterData);

            //StartCoroutine(LoadWorldSceneCoroutine(extraSceneIndex));
            LoadWorldScene(extraSceneIndex);
        }
        public void CreateMainMenu()
        {
            //StartCoroutine(LoadWorldSceneCoroutine(0));
            LoadWorldScene(0);
        }
    }
}