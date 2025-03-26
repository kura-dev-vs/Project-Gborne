using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Collections;
using System;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;

namespace Mebiustos.MMD4MecanimFaciem {
    public class FaciemDatabaseInspector : EditorWindow {

        private const string ITEM_NAME = "MMD4Mecanim/MMD4MecanimFaciem/Faciem Inspector";  // コマンド名
        const string CONFIG_PATH = "Temp/MMD4MecanimFaciemInspector.xml";

        public bool isRequestLoad = false;

        FaciemDatabaseInspector() {
            EditorApplication.playmodeStateChanged += StateChange;
        }

        void StateChange() {
            if (EditorApplication.isPlayingOrWillChangePlaymode == true && EditorApplication.isPlaying == false) {
                isPlaying = true;
                this.Repaint();
            }
            if (EditorApplication.isPlayingOrWillChangePlaymode == false && EditorApplication.isPlaying == false) {
                //LoadConfig();
                isPlaying = false;
                this.isRequestLoad = true;
                this.Repaint();
            }
            if (EditorApplication.isPlayingOrWillChangePlaymode == true && EditorApplication.isPlaying == true) {
                this.Repaint();
            }
        }

        [MenuItem(ITEM_NAME)]
        static void ShowWindow() {
            if (!CanCreate()) {
                return;
            }
            EditorWindow.GetWindow<FaciemDatabaseInspector>("Faciem Inspector");
            //win.autoRepaintOnSceneChange = true;
        }

        [MenuItem(ITEM_NAME, true)]
        static bool CanCreate() {
            return !EditorApplication.isPlaying && !Application.isPlaying && !EditorApplication.isCompiling;
        }

        void OnLostFocus() {
            if (config != null)
                config.Save(CONFIG_PATH);
        }

        void OnDestroy() {
            if (config != null)
                config.Save(CONFIG_PATH);
        }

        void OnFocus() {
            //Debug.Log("OnFocus:" + Selection.activeObject);
            ChangeModelBySelectionActiveObject();
        }

        void OnEnable() {
            //Debug.Log("OnEnable:" + Selection.activeObject);
            this.isRequestLoad = true;
            //LoadConfig();
            //if (!isPlaying) {
            //    RepaintZeroFaceAll(); // コンパイル対応
            //}
        }

        void OnSelectionChange() {
            ChangeModelBySelectionActiveObject();
            if (config != null && config.model != null) {
                MMD4MecanimAPI.SuperForceUpdateMorph(config.model);
            }
            this.Repaint();
        }

        /// <summary>
        /// 選択モデルを現在選択中のモデルに変更
        /// </summary>
        void ChangeModelBySelectionActiveObject() {
            if (Selection.activeObject == null)
                return;
            if (Selection.activeObject.GetType() != typeof(GameObject))
                return;
            if (config == null)
                return;

            var model = (Selection.activeObject as GameObject).GetComponent<MMD4MecanimModel>();
            if (model != null) {
                isChangeObject |= true;
                config.model = model;
            }
        }
 
        /// <summary>
        /// Inspector保存情報をロード
        /// </summary>
        void LoadConfig() {
            if (config == null || config.model == null) {
                if (!File.Exists(CONFIG_PATH)) {
                    config = new Config();
                } else {
                    config = Config.Load(CONFIG_PATH);
                }

                if (config.model != null) {
                    var database = config.model.GetComponent<FaciemDatabase>();
                    if (database != null) {
                        EditorUtility.SetDirty(database);
                        FaciemDatabaseEditor.MfaceToMMD4M(database);
                    }
                }

                this.Repaint();
            }
        }

        Config config;
        bool isChangeObject = true;
        bool isPlaying = false;
        int dirtyCount = FaciemDatabaseEditor.DIRTY_EXEC_COUNT;
        Vector2 scrollPosition;
        String defaultGuid; // シーン再生中の対象モデル検索に使用


        void OnGUI() {
            bool isDirty = false;
            //isDirty |= OnGUIByController();
            //EditorGUILayout.Separator();
            isDirty |= OnGUIByDatabase();
            if (isDirty) {
                dirtyCount = FaciemDatabaseEditor.DIRTY_EXEC_COUNT;
            }
        }

        void Update() {
            if (this.isRequestLoad) {
                this.isRequestLoad = false;
                LoadConfig();
                if (!isPlaying) {
                    RepaintZeroFaceAll(); // コンパイル対応
                    if (config != null && config.model != null) {
                        MMD4MecanimAPI.SuperForceUpdateMorph(config.model);
                    }
                    dirtyCount = FaciemDatabaseEditor.DIRTY_EXEC_COUNT;
                }
            }
            if (dirtyCount > 0) {
                dirtyCount--;
                if (config != null && config.model != null) {
                    EditorUtility.SetDirty(config.model);
                }
            }
        }

        /// <summary>
        /// MMD4Model選択フィールド表示処理
        /// </summary>
        bool OnGUIGetMMD4Model() {
            bool isDirty = false;
            if (isPlaying) {
            } else {
                int before = config.model == null ? -1 : config.model.GetInstanceID();
                //EditorGUILayout.LabelField("SELECT MMD4MecanimModel", EditorStyles.boldLabel);
                config.model = EditorGUILayout.ObjectField("MMD4Model", config.model, typeof(MMD4MecanimModel), true) as MMD4MecanimModel;
                int after = config.model == null ? -1 : config.model.GetInstanceID();
                if (before != after) {
                    isChangeObject = true;
                    isDirty = true; 
                }
            }
            return isDirty;
        }

        bool OnGUIByController() {
            bool isDirty = false;
            if (isPlaying) {
                //EditorGUILayout.HelpBox("再生中は表示できません。", MessageType.Info, true);
                if (EditorApplication.isPlaying) {
                    // ExpressionDatabase取得処理
                    var models = (MMD4MecanimModel[])GameObject.FindObjectsOfType(typeof(MMD4MecanimModel));
                    FaciemController ctrl = null;
                    foreach (var model in models) {
                        if (model.GetComponent<FaciemDatabase>().DefaultGuid == defaultGuid) {
                            ctrl = model.gameObject.GetComponent<FaciemController>();
                            break;
                        }
                    }
                    if (ctrl != null) {
                        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                        isDirty |= FaciemControllerEditor.DrawGUI2(ctrl);
                        EditorGUILayout.EndScrollView();
                        if (isDirty)
                          EditorUtility.SetDirty(ctrl);
                    }
// REPAINT
                    //EditorUtility.SetDirty(this);
                    //this.Repaint();
                }
            } else {
            }
            return false;
        }

        bool OnGUIByDatabase() {
            bool isDirty = false;
            if (isPlaying) {
                EditorGUILayout.HelpBox("再生中は Faciem Inspector を利用できません。", MessageType.Info, true);
            } else {
                if (config == null) config = new Config();

                // MMD4Model選択フィールド表示処理
                isDirty |= OnGUIGetMMD4Model();

                // ExpressionDatabase取得処理
                var database = config.model == null ? null : config.model.gameObject.GetComponent<FaciemDatabase>();
                if (database != null) {
                    if (isChangeObject) {
                        //ExpressionDatabaseEditor.MfaceToMMD4M(database);
                        isChangeObject = false;
                        defaultGuid = database.DefaultGuid;
                        Selection.activeGameObject = config.model.gameObject;
                    }
                    //scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUI.skin.box);
                    scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                    isDirty |= FaciemDatabaseEditor.DrawGUI(database);
                    EditorGUILayout.EndScrollView();
                    if (isDirty)
                        EditorUtility.SetDirty(database);
                } else {
                    if (config.model != null)
                        EditorGUILayout.HelpBox("対象モデルは Faciem Database コンポーネントを持っていません。", MessageType.Warning, true);
                }
// REPAINT
                //EditorUtility.SetDirty(this);
            }
            return isDirty;
        }

        ///// <summary>
        ///// 外部からの選択モデルオブジェクト変更
        ///// </summary>
        ///// <param name="model"></param>
        //public void SetModelObject(MMD4MecanimModel model) {
        //    config.model = model;
        //    isChangeObject |= true;
        //}

        /// <summary>
        /// 初期表情の全てのモデルを表情データに従い表情再描画させる
        /// </summary>
        void RepaintZeroFaceAll() {
            var databases = (FaciemDatabase[])Resources.FindObjectsOfTypeAll(typeof(FaciemDatabase));
            foreach (var database in databases) {
                if (FaciemDatabaseEditor.isAllWeightZeroModel(database)) {
                    FaciemDatabaseEditor.MfaceToMMD4M(database);
                }
            }
        }
        
        [System.Serializable]
        public class Config {
            public int modelObject_InstanceID;
            [XmlIgnore]
            public MMD4MecanimModel model;

            public void Save(string path) {
                //Debug.Log("### save");
                var serializer = new XmlSerializer(typeof(Config));
                if (model != null)
                    modelObject_InstanceID = model.gameObject.GetInstanceID();
                using (var stream = new FileStream(path, FileMode.Create)) {
                    serializer.Serialize(stream, this);
                }
            }

            public static Config Load(string path) {
                //Debug.Log("--- load");
                var serializer = new XmlSerializer(typeof(Config));
                using (var stream = new FileStream(path, FileMode.Open)) {
                    var conf = serializer.Deserialize(stream) as Config;
                    var obj = EditorUtility.InstanceIDToObject(conf.modelObject_InstanceID) as GameObject;
                    if (obj != null) {
                        conf.model = obj.GetComponent<MMD4MecanimModel>();
                    }
                    return conf;
                }
            }
        }
    }
}
