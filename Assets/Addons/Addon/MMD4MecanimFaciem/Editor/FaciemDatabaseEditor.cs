using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Text;
using System.Reflection;

namespace Mebiustos.MMD4MecanimFaciem {
    [CustomEditor(typeof(FaciemDatabase))]
    public class FaciemDatabaseEditor : Editor {

        const string BATCH_VALUE_STRING = "Batch Value";
        const float SPEED_MAX = 10f;
        public const int DIRTY_EXEC_COUNT = 5;

        FaciemDatabaseEditor() {
            EditorApplication.playmodeStateChanged += StateChange;
        }

        void StateChange() {
            var isStop = EditorApplication.isPlayingOrWillChangePlaymode == false && EditorApplication.isPlaying == false;
            if (isStop) {
                refleshFace = true;
            }
        }
 
        void OnEnable() {
            //Debug.Log("OnEnable");
            isDirty = true; // Inspector変更対応　&　コンパイル時の表情初期化対応

            // Wireframe表示／非表示 処理
            SetWireframeHidden((FaciemDatabase)target);
        }

        public bool refleshFace = false;
        bool isDirty = false;
        int dirtyCount = DIRTY_EXEC_COUNT;

        public override void OnInspectorGUI() {
            var database = (FaciemDatabase)target;


            //EditorGUILayout.LabelField("Mouse Position: ", Event.current.mousePosition.ToString());
            //if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            //    GUILayout.Label("Mouse over!");
            //else
            //    GUILayout.Label("Mouse somewhere else");
            //Repaint();


            //if (PrefabUtility.GetPrefabType(target) == PrefabType.None) {
            //} else {
            //    // In Project View
            //}

            // Hierarchyのオブジェクトかチェック
            if (!isHeerarchyObject(database)) {
                DispProjecObjectWarnMsg();
                return;
            }

            // MMDモデルオブジェクトかチェック
            var model = database.gameObject.GetComponent<MMD4MecanimModel>();
            if (model == null) return;

            // コンパイル時の表情初期化対応
            var isPlaying = !(EditorApplication.isPlayingOrWillChangePlaymode == false && EditorApplication.isPlaying == false);
            if (isDirty && !isPlaying) { // この時点でのisDirty==trueはOnEnableにて設定されたもの
                // もし表情が初期顔に戻ってたら表情再描画フラグON
                refleshFace |= isAllWeightZeroModel(database);
                MMD4MecanimAPI.SuperForceUpdateMorph(model);
            }

            // 表情再描画処理
            if (refleshFace && !isPlaying) {
                refleshFace = false;
                MfaceToMMD4M(database);
                isDirty = true;
            }

            // GUI描画
            isDirty |= DrawGUI(database);
            if (isDirty) {
                EditorUtility.SetDirty(model);
                EditorUtility.SetDirty(target);
                RefleshInspector(model);
                isDirty = false;
                dirtyCount = DIRTY_EXEC_COUNT;
            }
            if (dirtyCount > 0) {
                //Debug.Log("DirtyCount:" + dirtyCount);
                EditorUtility.SetDirty(model);
                dirtyCount--;
            }
        }

        void RefleshInspector(MMD4MecanimModel model) {
            var expressionInspectors = (FaciemDatabaseInspector[])Resources.FindObjectsOfTypeAll(typeof(FaciemDatabaseInspector));
            foreach (var inspector in expressionInspectors) {
                //inspector.SetModelObject(model);
                inspector.Repaint();
            }
        }

        public static bool DrawGUI(FaciemDatabase database) {
            GUI.changed = false;

            if (isHeerarchyObject(database) == false) {
                DispProjecObjectWarnMsg();
                return GUI.changed;
            }

            var isPlaying = !(EditorApplication.isPlayingOrWillChangePlaymode == false && EditorApplication.isPlaying == false);

            if (KeepMMD4MMorphList(database) && isPlaying == false) {
                MfaceToMMD4M(database);
            }

            DrawHideWireframeButton(database);

            EditorGUILayout.Separator();
            if (isPlaying) {
                var sb = new StringBuilder()
                    .Append("シーン再生中に'ADD FACE'を行っても、停止と共に再生前の状態に戻されます。")
                    .Append(Environment.NewLine)
                    .Append("下記の方法で対応して下さい。")
                    .Append(Environment.NewLine)
                    .Append(Environment.NewLine)
                    .Append("1) ADD FACEを実行する")
                    .Append(Environment.NewLine)
                    .Append("2) Componentタイトルを右クリック")
                    .Append(Environment.NewLine)
                    .Append("3) Copy Componentを選択")
                    .Append(Environment.NewLine)
                    .Append("4) 停止")
                    .Append(Environment.NewLine)
                    .Append("5) Componentタイトルを右クリック")
                    .Append(Environment.NewLine)
                    .Append("6) Paste Component Values");

                EditorGUILayout.HelpBox(sb.ToString(), MessageType.Warning, true);
            }

            //EditorGUILayout.ToggleLeft("Helper Prepare Advance", false);
            //EditorGUILayout.Separator();
            GUILayout.BeginHorizontal();
            database._newFaceName = EditorGUILayout.TextField("FaceName", database._newFaceName);
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("ADD FACE", GUILayout.ExpandWidth(false))) {
                AddFace(database);
            }
            if (isPlaying == false) {
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("CLEAR FACE", GUILayout.ExpandWidth(false))) {
                    ClearFace(database);
                }
            }
            GUI.backgroundColor = Color.white;
            GUILayout.EndHorizontal();

            if (isPlaying) {
                EditorGUILayout.Separator();
                StringBuilder sb = new StringBuilder();
                foreach (var facedata in database.FaceDataList) {
                    sb.Append("'").Append(facedata.FaceName).Append("'").Append(", ");
                }
                GUILayout.Label("FaceName List", EditorStyles.boldLabel);
                EditorGUILayout.HelpBox(sb.ToString(), MessageType.None, true);
                return GUI.changed;
            }

            EditorGUILayout.Separator();

            if (database.FaceDataList.Length > 0) {
                GUI.color = Color.cyan;
                GUILayout.Label("--- SELECT FACE", EditorStyles.boldLabel);
                GUI.color = Color.white;
                GUILayout.BeginHorizontal();
                if (database._selectedIdx == 0) GUI.backgroundColor = Color.grey;
                if (GUILayout.Button("<<", GUILayout.Width(50f)) && database._selectedIdx > 0) {
                    ChangeSelect(database, database._selectedIdx, database._selectedIdx - 1);
                }
                GUI.backgroundColor = Color.white;
                if (!(database._selectedIdx < database.FaceDataList.Length - 1)) GUI.backgroundColor = Color.grey;
                if (GUILayout.Button(">>", GUILayout.Width(50f)) && database._selectedIdx < database.FaceDataList.Length - 1) {
                    ChangeSelect(database, database._selectedIdx, database._selectedIdx + 1);
                }
                GUI.backgroundColor = Color.white;
                var beforeIdx = database._selectedIdx;
                database._selectedIdx = EditorGUILayout.Popup(database._selectedIdx, getFaceNames(database));
                ChangeSelect(database, beforeIdx, database._selectedIdx);
                FaceSelect(database, beforeIdx, database._selectedIdx);
                if (GUILayout.Button("RENAME", GUILayout.ExpandWidth(false))) {
                    Rename(database);
                }
                GUILayout.EndHorizontal();

                EditorGUILayout.Separator();
                GUILayout.BeginHorizontal();
                {
                    GUI.backgroundColor = Color.yellow;
                    if (GUILayout.Button("SAVE", GUILayout.Width(50f))) {
                        SaveFace(database);
                    }
                    //GUI.backgroundColor = Color.gray;
                    if (GUILayout.Button("LOAD", GUILayout.Width(50f))) {
                        LoadFace(database);
                    }
                    GUI.backgroundColor = Color.white;
                    GUILayout.FlexibleSpace();
                    if (database._selectedGuid == database.DefaultGuid) {
                        GUI.color = Color.cyan;
                        GUILayout.Label("DEFAULT", GUILayout.ExpandWidth(false));
                        GUI.color = Color.white;
                    } else {
                        if (GUILayout.Button("DEFAULT", GUILayout.ExpandWidth(false)))
                            SetDefaultFace(database);
                    }
                    GUILayout.FlexibleSpace();
                    GUI.backgroundColor = Color.red;
                    if (GUILayout.Button("DELETE", GUILayout.ExpandWidth(false)))
                        DeleteFace(database);
                    GUI.backgroundColor = Color.white;
                }
                GUILayout.EndHorizontal();

                EditorGUILayout.Separator();
                GUI.color = Color.cyan;
                GUILayout.Label("--- SPEED PARAM", EditorStyles.boldLabel);
                GUI.color = Color.white;
                if (database.FaceDataList.Length > 0) {
                    GUI.color = Color.cyan;
                    var fdata = database.FaceDataList[database._selectedIdx];
                    fdata._editorParam.BatchInputSpeedAll = EditorGUILayout.Slider(BATCH_VALUE_STRING, fdata._editorParam.BatchInputSpeedAll, 0, SPEED_MAX);
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    {
                        if (GUILayout.Button("BATCH", GUILayout.ExpandWidth(false))) {
                            BatchInputSpeed(database);
                        }
                    }
                    GUILayout.EndHorizontal();
                    GUI.color = Color.white;

                    var model = database.gameObject.GetComponent<MMD4MecanimModel>();
                    for (int catIndex = MMD4MecanimAPI.MorphCategoryIndexMin; catIndex < MMD4MecanimAPI.MorphCategoryIndexMax; ++catIndex) {
                        EditorGUILayout.Separator();
                        MMD4MecanimData.MorphCategory morphCategory = (MMD4MecanimData.MorphCategory)catIndex;
                        //GUI.color = Color.cyan;
                        //GUILayout.Label(morphCategory.ToString());
                        //GUI.color = Color.white;

                        bool isFound = false;
                        foreach (var morph in fdata.MMD4Morphs) {
                            if (model.modelData.morphDataList[morph.morphIndex].morphCategory == morphCategory) {
                                if (model.morphList != null && (uint)morph.morphIndex < model.morphList.Length) {
                                    if (!isFound) {
                                        GUI.color = Color.cyan;
                                        GUILayout.Label(morphCategory.ToString());
                                        fdata._editorParam.BatchInputSpeedCategory[catIndex - 1] = EditorGUILayout.Slider(BATCH_VALUE_STRING, fdata._editorParam.BatchInputSpeedCategory[catIndex - 1], 0, SPEED_MAX);
                                        GUILayout.BeginHorizontal();
                                        GUILayout.FlexibleSpace();
                                        {
                                            if (GUILayout.Button("BATCH", GUILayout.ExpandWidth(false))) {
                                                BatchInputSpeed(database, catIndex);
                                            }
                                        }
                                        GUILayout.EndHorizontal();
                                        GUI.color = Color.white;
                                    }
                                    var name = morph.GetName(model);
                                    name = name.Length == 0 ? " " : name;
                                    var beforespeed = morph.speed;
                                    morph.speed = EditorGUILayout.Slider(name, morph.speed, 0, SPEED_MAX);
                                    if (beforespeed != morph.speed) EditorApplication.MarkSceneDirty();
                                    isFound = true;
                                }
                            }
                        }
                        //if (!isFound) GUILayout.Label("(none)");
                    }
                }
            }
            return GUI.changed;
        }

        /// <summary>
        /// ワイヤーフレーム表示・非表示ボタンの描画
        /// </summary>
        /// <param name="database"></param>
        /// <returns></returns>
        static bool DrawHideWireframeButton(FaciemDatabase database) {
            bool isDirty = false;

            var b_contentColor = GUI.contentColor;
            var b_backgroundColor = GUI.backgroundColor;
            if (FaciemDatabase.hideWireframe) {
                GUI.contentColor = Color.yellow;
                GUI.backgroundColor = Color.gray;
            }
            if (GUILayout.Button("Hide Wireframe")) {
                isDirty = true;
                FaciemDatabase.hideWireframe = !FaciemDatabase.hideWireframe;
                FaciemDatabaseEditor.SetWireframeHidden(database);
            }
            GUI.contentColor = b_contentColor;
            GUI.backgroundColor = b_backgroundColor;

            return isDirty;
        }

        /// <summary>
        /// ワイヤーフレーム表示・非表示処理
        /// </summary>
        /// <param name="database"></param>
        static void SetWireframeHidden(FaciemDatabase database) {
            if (!database) return;
            var rends = database.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < rends.Length; i++) {
                EditorUtility.SetSelectedWireframeHidden(rends[i], FaciemDatabase.hideWireframe);
            }
        }

        /// <summary>
        /// プロジェクト内オブジェクト選択時の警告メッセージ表示
        /// </summary>
        static void DispProjecObjectWarnMsg() {
            EditorGUILayout.HelpBox("Activeでないモデル、Project内のモデルはDatabaseGUIを表示できません。", MessageType.Warning);
        }

        /// <summary>
        /// Hierarchy内のオブジェクトか判定します。
        /// </summary>
        /// <returns></returns>
        public static bool isHeerarchyObject(FaciemDatabase database) {
            bool isHierarchy = false;
            var objs = UnityEngine.Object.FindObjectsOfType<FaciemDatabase>();
            foreach (var obj in objs) {
                if (obj == database) {
                    isHierarchy = true;
                    break;
                }
            }
            return isHierarchy;
        }

        /// <summary>
        /// 現在選択中の表情をデフォルトに設定します。
        /// </summary>
        /// <param name="database"></param>
        static void SetDefaultFace(FaciemDatabase database) {
            database.DefaultGuid = database._selectedGuid;
            EditorApplication.MarkSceneDirty();
        }

        /// <summary>
        /// モデルの表情をクリアします。（all weight 0）
        /// </summary>
        /// <param name="database"></param>
        static void ClearFace(FaciemDatabase database) {
            bool result = EditorUtility.DisplayDialog(
                "CLEAR FACE",
                "現在のMMD4MecanimModelに設定されている表情をクリアします。(All weight 0.)",
                "OK",
                "CANCEL"
                );
            if (result) {
                var model = database.GetComponent<MMD4MecanimModel>();
                foreach (var morph in model.morphList) {
                    morph.weight = 0;
                }
                MMD4MecanimAPI.SuperForceUpdateMorph(model);
            }
        }

        /// <summary>
        /// 別のFaceDataを選択した際の、インデックス移動チェック処理
        /// </summary>
        /// <param name="database"></param>
        /// <param name="beforeIdx"></param>
        /// <param name="afterIdx"></param>
        static void ChangeSelect(FaciemDatabase database, int beforeIdx, int afterIdx) {
            if (beforeIdx == afterIdx) {
                database._selectedIdx = afterIdx;
                return;
            }
            
            database._selectedIdx = beforeIdx;

            if (isDiffFaceParam(database)) {
                bool result = EditorUtility.DisplayDialog(
                    "確認",
                    database.FaceDataList[beforeIdx].FaceName + " は変更されています。現在のMorph値で更新しますか？",
                    "YES（更新）",
                    "NO（無視）"
                    );
                if (result) {
                    database._selectedIdx = beforeIdx;
                    __SaveFaceMarge(database);

                    database._selectedIdx = beforeIdx;
                } else {
                    database._selectedIdx = afterIdx;
                }
            } else {
                database._selectedIdx = afterIdx;
            }
        }

        /// <summary>
        /// 現在の表情でFaceDataを保存します。
        /// </summary>
        static void SaveFace(FaciemDatabase database) {
            if (!isDiffFaceParam(database)) {
                EditorUtility.DisplayDialog("SAVE", database.FaceDataList[database._selectedIdx].FaceName + " は現在のMorph値と同一のため、保存の必要はありません。", "OK");
                return;
            }

            bool result = EditorUtility.DisplayDialog(
                "SAVE",
                "現在の表情を " + database.FaceDataList[database._selectedIdx].FaceName + " に保存します。",
                "OK",
                "CANCEL"
                );
            if (result) {
                __SaveFaceMarge(database);
            }
        }

        /// <summary>
        /// FaceDataを現在の表情で更新します。(Speedマージ考慮あり)
        /// </summary>
        /// <param name="database"></param>
        static void __SaveFaceMarge(FaciemDatabase database) {
            var fdata = database.FaceDataList[database._selectedIdx];
            var model = database.gameObject.GetComponent<MMD4MecanimModel>();
            var newMMD4Morphs = FaciemDatabase.GetFaceMorphsByMMD4Model(model, fdata._editorParam.BatchInputSpeedAll);
            foreach (var newPara in newMMD4Morphs) {
                foreach (var oldPara in fdata.MMD4Morphs) {
                    if (newPara.morphIndex == oldPara.morphIndex) {
                        newPara.speed = oldPara.speed;
                        break;
                    }
                }
            }
            fdata.MMD4Morphs = newMMD4Morphs;
            EditorApplication.MarkSceneDirty();
        }

        /// <summary>
        /// FaceDataを現在のMorphに設定します。
        /// </summary>
        static void LoadFace(FaciemDatabase database) {
            bool result = EditorUtility.DisplayDialog(
                "LOAD",
                database.FaceDataList[database._selectedIdx].FaceName + " をモデルに読み込みます。",
                "OK",
                "CANCEL"
                );
            if (result) {
                MfaceToMMD4M(database);
            }
        }
        
        /// <summary>
        /// Morphスピード一括設定(全て)
        /// </summary>
        /// <param name="database"></param>
        static void BatchInputSpeed(FaciemDatabase database) {
            bool result = EditorUtility.DisplayDialog(
                "スピード一括設定",
                "MorphSpeed : " + database.FaceDataList[database._selectedIdx]._editorParam.BatchInputSpeedAll + " で一括設定します。",
                "OK",
                "CANCEL"
                );
            if (result) {
                var speed = database.FaceDataList[database._selectedIdx]._editorParam.BatchInputSpeedAll;
                foreach (var morph in database.FaceDataList[database._selectedIdx].MMD4Morphs) {
                    morph.speed = speed;
                }

                for (int i = 0; i < 4; i++ ) {
                    database.FaceDataList[database._selectedIdx]._editorParam.BatchInputSpeedCategory[i] = speed;
                }
                EditorApplication.MarkSceneDirty();
            }
        }

        /// <summary>
        /// Morphスピード一括設定（カテゴリ）
        /// </summary>
        /// <param name="database"></param>
        /// <param name="catIndex"></param>
        static void BatchInputSpeed(FaciemDatabase database, int catIndex) {
            var mcate = (MMD4MecanimData.MorphCategory)catIndex;
            bool result = EditorUtility.DisplayDialog(
                "スピード一括設定",
                "MorphSpeed : " + database.FaceDataList[database._selectedIdx]._editorParam.BatchInputSpeedCategory[catIndex - 1] + " で " + mcate.ToString() + " を一括設定します。",
                "OK",
                "CANCEL"
                );
            if (result) {
                var model = database.gameObject.GetComponent<MMD4MecanimModel>();
                var speed = database.FaceDataList[database._selectedIdx]._editorParam.BatchInputSpeedCategory[catIndex - 1];
                foreach (var morph in database.FaceDataList[database._selectedIdx].MMD4Morphs) {
                    if (model.modelData.morphDataList[morph.morphIndex].morphCategory == mcate)
                        morph.speed = speed;
                }
                EditorApplication.MarkSceneDirty();
            }
        }

        /// <summary>
        /// 表情選択プルダウン値に基づく処理(MMD4Model表情の変更処理)
        /// </summary>
        static void FaceSelect(FaciemDatabase database, int beforeIdx, int afterIdx) {
            bool isDiffIdx = beforeIdx != afterIdx;
            bool isDiffGuid = database._selectedGuid != database.FaceDataList[database._selectedIdx].FaceGuid;
            if (isDiffIdx || isDiffGuid) {
                if (isDiffIdx) {
                    database._selectedGuid = database.FaceDataList[database._selectedIdx].FaceGuid;
                    isDiffGuid = false;
                }
                if (isDiffGuid) {
                    //Debug.Log("DIFF FACE-GUID. Force update face.");
                    var guididx = getFaceIndexByGuid(database, database.FaceDataList[database._selectedIdx].FaceGuid);
                    database._selectedIdx = guididx == -1 ? database._selectedIdx : guididx;
                    database._selectedGuid = database.FaceDataList[database._selectedIdx].FaceGuid;
                }
                MfaceToMMD4M(database);
            }
        }

        /// <summary>
        /// 選択中のFaceDataを削除します。
        /// </summary>
        /// <param name="database"></param>
        static void DeleteFace(FaciemDatabase database) {
            bool result = EditorUtility.DisplayDialog(
                "DELETE",
                database.FaceDataList[database._selectedIdx].FaceName + " を削除します",
                "OK",
                "CANCEL"
                );
            if (result) {
                database.DeleteFace(database._selectedIdx);
                if (database.FaceDataList.Length == 0) {
                    // 表情が空の場合はMMD4Modelの表情を初期化する
                    var model = database.GetComponent<MMD4MecanimModel>();
                    foreach (var morph in model.morphList) {
                        morph.weight = 0;
                    }
                    MMD4MecanimAPI.SuperForceUpdateMorph(model);
                    //model.ForceUpdateMorph();
                }
                EditorApplication.MarkSceneDirty();
            }
        }


        /// <summary>
        /// 表情名の変更
        /// </summary>
        /// <param name="database"></param>
        static void Rename(FaciemDatabase database) {
            //ShowPopupEx window = (ShowPopupEx)EditorWindow.GetWindow(typeof(ShowPopupEx));
            //var mainWindowType = System.Type.GetType("UnityEditor.MainWindow, UnityEditor");
            //UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll(mainWindowType);
            //if (array.Length == 0) {
            //    UnityEngine.Debug.LogError("No Main Window found!");
            //} else {
            //    var view = array[0];
            //    var screenPosition = mainWindowType.GetProperty("screenPosition");

            //    window.position = (Rect)screenPosition.GetValue(view, null);
            //    window.ShowPopup();
            //}

            if (database._newFaceName == null || database._newFaceName.Length == 0) {
                EditorUtility.DisplayDialog("RENAME", "FaceName欄が空です。", "OK");
                return;
            }

            foreach (var data in database.FaceDataList) {
                if (data.FaceName == database._newFaceName) {
                    EditorUtility.DisplayDialog("RENAME", database._newFaceName + " と同じ名前がすでに存在します。", "OK");
                    return;
                }
            }

            bool result = EditorUtility.DisplayDialog(
                "RENAME",
                database.FaceDataList[database._selectedIdx].FaceName + " を " + database._newFaceName + " にリネームします。",
                "OK",
                "CANCEL"
                );
            if (result) {
                database.RenameFace(database._selectedIdx, database._newFaceName);
                database._newFaceName = "";
                EditorGUI.FocusTextInControl(null);
                EditorApplication.MarkSceneDirty();
            }
        }

        /// <summary>
        /// 新規にFaceDataを作成します
        /// </summary>
        /// <param name="database"></param>
        static void AddFace(FaciemDatabase database) {
            KeepMMD4MMorphList(database);
            //var mmd4m = database.gameObject.GetComponent<MMD4MecanimModel>();
            //mmd4m.InitializeOnEditor();

            if (database._newFaceName == null || database._newFaceName.Length == 0) {
                EditorUtility.DisplayDialog("ADD FACE", "FaceName欄が空です。", "OK");
                return;
            }

            var newface = database.AddNewFace(database._newFaceName, database._newMorphSpeed);
            if (newface == null) {
                EditorUtility.DisplayDialog("ADD FACE", database._newFaceName + " と同じ名前がすでに存在します。", "OK");
                return;
            }

            database._newFaceName = "";
            EditorGUI.FocusTextInControl(null);
            EditorApplication.MarkSceneDirty();
        }

        /// <summary>
        /// 選択中のFaceDataをMMD4Modelに反映させます
        /// </summary>
        /// <param name="database"></param>
        public static void MfaceToMMD4M(FaciemDatabase database) {
            //Debug.Log("MfaceToMMD4M");
            if (database.FaceDataList == null || database.FaceDataList.Length == 0) return;
            KeepMMD4MMorphList(database);
            //mmd4m.InitializeOnEditor();

            var mmd4m = database.gameObject.GetComponent<MMD4MecanimModel>();
            if (mmd4m == null || mmd4m.modelData == null || mmd4m.modelData.morphDataList == null)
                return;

            for (int morphIndex = 0; morphIndex < mmd4m.modelData.morphDataList.Length; ++morphIndex) {
                MMD4MecanimModel.Morph morph = mmd4m.morphList[morphIndex];
                bool isMatch = false;
                foreach (var faceMorph in database.FaceDataList[database._selectedIdx].MMD4Morphs) {
                    if (faceMorph.morphIndex == morphIndex) {
                        morph.weight = faceMorph.weight;
                        isMatch = true;
                        break;
                    }
                }
                if (!isMatch)
                    morph.weight = 0f;
            }

            MMD4MecanimAPI.SuperForceUpdateMorph(mmd4m);
            //mmd4m.ForceUpdateMorph();
            //EditorUtility.SetDirty(mmd4m.gameObject);
            //HandleUtility.Repaint();
            //SceneView.RepaintAll();
        }

        /// <summary>
        /// 選択中のFaceDataと現在のモデルの表情を比較します。
        /// </summary>
        /// <param name="database"></param>
        /// <returns>true: 違う false: 同じ</returns>
        static bool isDiffFaceParam(FaciemDatabase database) {
            bool isDiff = false;
            var model = database.GetComponent<MMD4MecanimModel>();
            foreach (var morph in model.morphList) {
                if (morph.weight > 0) {
                    bool isFound = false;
                    foreach (var faceMorph in database.FaceDataList[database._selectedIdx].MMD4Morphs) {
                        if (faceMorph.GetName(model) == morph.name) {
                            isFound = true;
                            isDiff = faceMorph.weight != morph.weight;
                            break;
                        }
                    }
                    if (isFound == false)
                        isDiff = true;
                    if (isDiff)
                        break;
                } else {
                    foreach (var faceMorph in database.FaceDataList[database._selectedIdx].MMD4Morphs) {
                        if (faceMorph.GetName(model) == morph.name) {
                            isDiff = true;
                            break;
                        }
                    }
                    if (isDiff)
                        break;
                }
            }

            return isDiff;
        }

        /// <summary>
        /// MMD4ModelのMorphListを有効な状態に保ちます。
        /// </summary>
        /// <param name="database"></param>
        /// <returns>true: 初期化実施 false: 現状維持</returns>
        public static bool KeepMMD4MMorphList(FaciemDatabase database) {
            var mmd4m = database.gameObject.GetComponent<MMD4MecanimModel>();
            if (mmd4m.morphList == null) {
                mmd4m.InitializeOnEditor();
                return true;
            } else {
                return false;
            }
        }

        /// <summary>
        /// Databaseに保存されているFace情報のなまえ一覧を取得する。
        /// </summary>
        /// <param name="database"></param>
        /// <returns></returns>
        static string[] getFaceNames(FaciemDatabase database) {
            string[] keys = new string[database.FaceDataList.Length];
            for (int i = 0; i < database.FaceDataList.Length; i++) {
                var face = database.FaceDataList[i];
                keys[i] = face.FaceName;
                if (database.DefaultGuid == face.FaceGuid) {
                    keys[i] += " [Default]";
                }
            }
            return keys;
        }

        /// <summary>
        /// 指定したGUIDを持つFaceDataのインデックス番号を返却する
        /// </summary>
        /// <param name="database"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        static int getFaceIndexByGuid(FaciemDatabase database, string guid) {
            //return database.getFaceIndexByGuid(guid);
            int foundIdx = -1;
            for (int i = 0; i < database.FaceDataList.Length; i++) {
                if (database.FaceDataList[i].FaceGuid == guid) {
                    foundIdx = i;
                    break;
                }
            }
            return foundIdx;
        }

        /// <summary>
        /// モデルに設定されている表情が全てWeight0か判定します。
        /// </summary>
        /// <returns></returns>
        public static bool isAllWeightZeroModel(FaciemDatabase database) {
            KeepMMD4MMorphList(database);
            var model = database.GetComponent<MMD4MecanimModel>();
            if (model == null || model.morphList == null)
                return true;
            foreach (var morph in model.morphList) {
                if (morph.weight > 0) return false;
            }
            return true;
        }
    }
}
