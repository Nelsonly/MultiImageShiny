using System;
using UnityEditor;
using UnityEngine;

namespace Coffee.UIEffects
{
    /// <summary>
    /// UIEffect editor.
    /// </summary>
    [CustomEditor(typeof(UIMultiShiny))]
    [CanEditMultipleObjects]
    public class UIMultiShinyEditor : Editor
    {
        private Vector3 _lastPos;

        private bool hasSetMethod = false;
       
        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        protected void OnEnable()
        {
            _spWidth = serializedObject.FindProperty("m_Width");
            _spRotation = serializedObject.FindProperty("m_Rotation");
            _spSoftness = serializedObject.FindProperty("m_Softness");
            _spBrightness = serializedObject.FindProperty("m_Brightness");
            _spGloss = serializedObject.FindProperty("m_Gloss");
            _worldPositionListener = serializedObject.FindProperty("worldPositionListener");
            _spEffectImages = serializedObject.FindProperty("m_EffectImages");
            _lastPos = ((UIMultiShiny)target).transform.position;
            if (!EditorApplication.isPlaying && !hasSetMethod)
            {
                EditorApplication.update += MyUpdateMethod;
                hasSetMethod = true;
            }
        }
        
        /// <summary>
        /// Implement this function to make a custom inspector.
        /// </summary>
        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(_spWidth);
            EditorGUILayout.PropertyField(_spRotation);
            EditorGUILayout.PropertyField(_spSoftness);
            EditorGUILayout.PropertyField(_spBrightness);
            EditorGUILayout.PropertyField(_spGloss);
            EditorGUILayout.PropertyField(_worldPositionListener);
            EditorGUILayout.BeginHorizontal();

            // 如果数组是可见的，显示一个折叠的三角形
            bool isExpanded = EditorGUILayout.PropertyField(_spEffectImages, true);

            // 绘制刷新按钮
            if (GUILayout.Button("刷新", GUILayout.Width(50))) // 你可以设置按钮的宽度
            {
                // 刷新按钮被点击时执行的操作
                ((UIMultiShiny)target).RefreshImages();
            }

            // 结束水平布局
            EditorGUILayout.EndHorizontal();

            // 如果数组或列表是展开的，绘制子属性
            if (isExpanded)
            {
                EditorGUI.indentLevel++;
                for (int i = 0; i < _spEffectImages.arraySize; i++)
                {
                    EditorGUILayout.PropertyField(_spEffectImages.GetArrayElementAtIndex(i), true);
                }
                EditorGUI.indentLevel--;
            }
            
            serializedObject.ApplyModifiedProperties();
        }
        

        private void MyUpdateMethod()
        {
            if (!target)
            {
                EditorApplication.update -= MyUpdateMethod;
                return;
            }
            UIMultiShinyMoveListener t = ((UIMultiShiny)target).worldPositionListener;

            if (_lastPos != t.transform.position)
            {
                _lastPos = t.transform.position;
                ((UIMultiShiny)target).SetDirty();
            }
        }
        
        //################################
        // Private Members.
        //################################
        SerializedProperty _spMaterial;
        SerializedProperty _spWidth;
        SerializedProperty _spRotation;
        SerializedProperty _spSoftness;
        SerializedProperty _spBrightness;
        SerializedProperty _spGloss;
        SerializedProperty _worldPositionListener;
        SerializedProperty _spEffectImages;
    }
}