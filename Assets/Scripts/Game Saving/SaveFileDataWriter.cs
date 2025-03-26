using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace RK
{
    /// <summary>
    /// セーブデータの保存時
    /// </summary>
    public class SaveFileDataWriter
    {
        public string saveDataDirectoryPath = "";
        public string saveFileName = "";

        // セーブファイルを作成する前にスロットに空きがあるか確認
        public bool CheckToSeeIfFileExists()
        {
            if (File.Exists(Path.Combine(saveDataDirectoryPath, saveFileName)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        // セーブデータの削除
        public void DeleteSaveFile()
        {
            File.Delete(Path.Combine(saveDataDirectoryPath, saveFileName));
        }
        // ニューゲームを行った際にセーブデータを作成
        public void CreateNewCharacterSaveFile(CharacterSaveData characterData)
        {
            //  (Application.persistentDataPath) で得た永続データ ディレクトリパスにファイル名を加えたパスを作成
            string savePath = Path.Combine(saveDataDirectoryPath, saveFileName);

            try
            {
                // ファイルが書き込まれるディレクトリが存在しない場合、そのディレクトリを作成する
                Directory.CreateDirectory(Path.GetDirectoryName(savePath));
                Debug.Log("CREATING SAVE FILE, AT SAVE PATH: " + savePath);

                // c#のゲーム・データ・オブジェクトをjsonにシリアライズする。
                string dataToStore = JsonUtility.ToJson(characterData, true);

                // ファイルをシステムに書き込む
                using (FileStream stream = new FileStream(savePath, FileMode.Create))
                {
                    using (StreamWriter fileWriter = new StreamWriter(stream))
                    {
                        fileWriter.Write(dataToStore);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("ERROR WHILST TRYING TO SAVE CHARACTER DATA, GAME NOT SAVED" + savePath + "\n" + ex);
            }
        }

        // セーブデータのロード
        public CharacterSaveData LoadSaveFile()
        {
            CharacterSaveData characterData = null;
            //  (Application.persistentDataPath) で得た永続データ ディレクトリパスにファイル名を加えたパスを作成
            string loadPath = Path.Combine(saveDataDirectoryPath, saveFileName);

            if (File.Exists(loadPath))
            {
                try
                {
                    string dataToLoad = "";

                    using (FileStream stream = new FileStream(loadPath, FileMode.Open))
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            dataToLoad = reader.ReadToEnd();
                        }
                    }

                    // データをjsonからunityにデシリアライズする。
                    characterData = JsonUtility.FromJson<CharacterSaveData>(dataToLoad);
                }
                catch (Exception)
                {
                    Debug.Log("FILE IS BLANK");
                }


            }
            return characterData;

        }
    }
}
