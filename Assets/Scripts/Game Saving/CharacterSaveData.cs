using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace RK
{

    [System.Serializable]
    /// <summary>
    /// 保存ファイルごとにこのデータを参照したいので、このスクリプトはMonoBehaviorではなく, serializable
    /// 
    /// </summary>
    public class CharacterSaveData
    {
        [Header("SCENE INDEX")]
        public int sceneIndex = 1;
        [Header("Character Name")]
        public string characterName = "Character";
        [Header("Time Played")]
        public float secondsPlayed;

        // monobehaiviorではないのでvector3が使えない
        [Header("World Coordinates")]
        public float xPosition;
        public float yPosition;
        public float zPosition;

        [Header("Resources")]
        public int currentHealth;
        public float currentStamina;
        [Header("Stats")]
        public int vitality, endurance;
        [Header("PT ID")]
        public int[] ptID = { 6, 5, 0, 0 };
        [Header("Rest Point")]
        public SerializableDictionary<int, bool> restPoint;    // int: 休憩ポイントのID, bool: 起動済か

        [Header("Bosses")]
        public SerializableDictionary<int, bool> bossesAwakened;    // int: ボスのID, bool: 起動済か
        public SerializableDictionary<int, bool> bossesDefeated;    // int: ボスのID, bool: 撃破済か


        [Header("World Items")]
        public SerializableDictionary<int, bool> worldItemsLooted;  // int: アイテムのID, bool: 所持済か
        public SerializableDictionary<int, int> itemsInInventory;   // int: 自身のinventoryの順番, int: その順番に入っているアイテムのid

        public SerializableDictionary<int, int> pcRightWeapon, pcLeftWeapon;
        public SerializableDictionary<int, int> pcHatOutfit, pcJacketOutfit, pcTopsOutfit, pcBottomsOutfit, pcShoesOutfit;

        public CharacterSaveData()
        {
            restPoint = new SerializableDictionary<int, bool>();
            bossesAwakened = new SerializableDictionary<int, bool>();
            bossesDefeated = new SerializableDictionary<int, bool>();
            //worldItemsLooted = new SerializableDictionary<int, bool>();

            pcRightWeapon = new SerializableDictionary<int, int>();
            pcLeftWeapon = new SerializableDictionary<int, int>();

            pcHatOutfit = new SerializableDictionary<int, int>();
            pcJacketOutfit = new SerializableDictionary<int, int>();
            pcTopsOutfit = new SerializableDictionary<int, int>();
            pcBottomsOutfit = new SerializableDictionary<int, int>();
            pcShoesOutfit = new SerializableDictionary<int, int>();
        }
    }
}
