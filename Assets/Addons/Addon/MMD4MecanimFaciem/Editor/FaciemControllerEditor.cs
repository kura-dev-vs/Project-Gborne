﻿using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Mebiustos.MMD4MecanimFaciem {
    [CustomEditor(typeof(FaciemController))]
    public class FaciemControllerEditor : Editor {

        static Color DefaultColor;
        static bool isClearSpeed = true;
        //static SerializedObject seriObj;

        protected virtual void OnEnable() {
            //DefaultColor = new Color(0.8f, 0.8f, 0.8f);
            DefaultColor = GUI.color;
        }

        public override void OnInspectorGUI() {
            //seriObj = serializedObject;
            var ctrl = (FaciemController)target;
            var isDirty = DrawGUI(ctrl);
            if (isDirty)
                EditorUtility.SetDirty(target);
        }

        /// <summary>
        /// DrawGUI
        /// </summary>
        /// <param name="ctrl"></param>
        public static bool DrawGUI(FaciemController ctrl) {
            bool isDirty = false;
            GUI.changed = false;

            //seriObj.Update();
            //EditorGUILayout.PropertyField(seriObj.FindProperty("OverideWeight"));
            //EditorGUILayout.PropertyField(seriObj.FindProperty("ClearSpeed"));

            ctrl.OverrideWeight = EditorGUILayout.Toggle("Override Weight", ctrl.OverrideWeight);
            ctrl.ClearSpeed = EditorGUILayout.FloatField("Clear Speed", ctrl.ClearSpeed);

            if (GUI.changed && !EditorApplication.isPlayingOrWillChangePlaymode)
                EditorApplication.MarkSceneDirty();

            isDirty |= GUI.changed;

            var isPlaying = EditorApplication.isPlayingOrWillChangePlaymode == true && EditorApplication.isPlaying == true;
            if (isPlaying && ctrl.CurrentFaceIndex > -1) {
                isDirty |= DrawGUI2(ctrl);
            }
            //seriObj.ApplyModifiedProperties();

            return isDirty;
        }

        /// <summary>
        /// DrawGUI
        /// </summary>
        /// <param name="ctrl"></param>
        public static bool DrawGUI2 (FaciemController ctrl) {
            GUI.changed = false;
            EditorGUILayout.Separator();
            GUILayout.Label("Debug Tool", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("以下で SetFace関数 を実行できます", MessageType.Info, true);
            isClearSpeed = EditorGUILayout.Toggle("Clear Speed (Unuse Morph)", isClearSpeed);
            EditorGUILayout.Separator();
            var database = ctrl.GetComponent<FaciemDatabase>();
            if (ctrl == null || database == null) return GUI.changed;
            var facelist = database.FaceDataList;
            int columnMax = 2;
            int nowColumn = 0;
            for (int i = 0; i < facelist.Length; i++) {
                if (nowColumn == 0)
                    GUILayout.BeginHorizontal();
                if (ctrl.CurrentFaceIndex == i) GUI.color = Color.cyan;
                if (GUILayout.Button(facelist[i].FaceName)) {
                    ctrl.SetFace(facelist[i].FaceName, isClearSpeed);
                }
                GUI.color = DefaultColor;
                nowColumn++;
                if (nowColumn == columnMax) {
                    GUILayout.EndHorizontal();
                    nowColumn = 0;
                }
            }
            if (nowColumn > 0 && nowColumn < columnMax) {
                //GUI.color = Color.gray;
                //for (int i = nowColumn; i < columnMax; i++)
                //    GUILayout.Button(" ", EditorStyles.miniButton);
                //GUI.color = DefaultColor;
                GUILayout.EndHorizontal();
            }

            return GUI.changed;
        }

        static string[] getFaceNames(FaciemController ctrl) {
            var database = ctrl.GetComponent<FaciemDatabase>();
            string[] keys = new string[database.FaceDataList.Length];
            for (int i = 0; i < database.FaceDataList.Length; i++) {
                var face = database.FaceDataList[i];
                keys[i] = face.FaceName;
            }
            return keys;
        }

    }
}
