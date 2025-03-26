using System.Collections;
using System.Collections.Generic;
using RK;
using TMPro;
using UnityEngine;

/// <summary>
/// キャラクターのセーブスロットUIで用いられるコンポーネント
/// </summary> 
public class UI_Character_Save_Slot : MonoBehaviour
{
    SaveFileDataWriter saveFileWriter;
    [Header("Game Slot")]
    public CharacterSlot characterSlot;
    [Header("Character Info")]
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI timedPlayed;

    private void OnEnable()
    {
        LoadSaveSlots();
    }
    private void LoadSaveSlots()
    {
        saveFileWriter = new SaveFileDataWriter();
        saveFileWriter.saveDataDirectoryPath = Application.persistentDataPath;

        // save slot 01
        if (characterSlot == CharacterSlot.CharacterSlot_01)
        {
            saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);
            // セーブファイルが存在する場合情報取得
            if (saveFileWriter.CheckToSeeIfFileExists())
            {
                characterName.text = WorldSaveGameManager.instance.characterSlots01.characterName;
            }
            // 存在しない場合非表示
            else
            {
                gameObject.SetActive(false);
            }
        }
        // save slot 02
        else if (characterSlot == CharacterSlot.CharacterSlot_02)
        {
            saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);
            // セーブファイルが存在する場合情報取得
            if (saveFileWriter.CheckToSeeIfFileExists())
            {
                characterName.text = WorldSaveGameManager.instance.characterSlots02.characterName;
            }
            // 存在しない場合非表示
            else
            {
                gameObject.SetActive(false);
            }
        }
        // save slot 03
        else if (characterSlot == CharacterSlot.CharacterSlot_03)
        {
            saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);
            // セーブファイルが存在する場合情報取得
            if (saveFileWriter.CheckToSeeIfFileExists())
            {
                characterName.text = WorldSaveGameManager.instance.characterSlots03.characterName;
            }
            // 存在しない場合非表示
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
    public void LoadGameFromCharacterSlot()
    {
        WorldSaveGameManager.instance.currentCharacterSlotBeingUsed = characterSlot;
        WorldSaveGameManager.instance.LoadGame();
    }
    public void SelectCurrentSlot()
    {
        TitleScreenManager.instance.SelectCharacterSlot(characterSlot);
    }
}
