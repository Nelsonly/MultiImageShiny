using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEditor;
#endif

namespace Coffee.UIEffects
{
    /// <summary>
    /// UIEffect.
    /// </summary>
    [AddComponentMenu("UI/UIEffects/UIMultiShiny", 4)]
    public class UIMultiShiny : UIBehaviour
    {
        private const string shaderName = "UI/Hidden/UI-Effect-Multi-Shiny";
        
        [Tooltip("Width for shiny effect.")] [SerializeField] [Range(0, 2)]
        float m_Width = 0.25f;

        [Tooltip("Rotation for shiny effect.")] [SerializeField] [Range(-180, 180)]
        float m_Rotation;

        [Tooltip("Softness for shiny effect.")] [SerializeField] [Range(0.01f, 1)]
        float m_Softness = 1f;

        [Tooltip("Brightness for shiny effect.")] [FormerlySerializedAs("m_Alpha")] [SerializeField] [Range(0, 1)]
        float m_Brightness = 1f;

        [Tooltip("Gloss factor for shiny effect.")] [FormerlySerializedAs("m_Highlight")] [SerializeField] [Range(0, 1)]
        float m_Gloss = 1;

        // 高度用到的地方很少，目前先不用
        // [Tooltip("扫光高度")] [SerializeField] [Range(0, 10)] private float m_High = 1;

        [SerializeField] [Tooltip("跟随的Obj")]
        public UIMultiShinyMoveListener worldPositionListener;
        private Material CacheMaterial;

        [SerializeField] [Tooltip("影响的Image组件")] 
        Image[] m_EffectImages;
        
        /// <summary>
        /// Width for shiny effect.
        /// </summary>
        public float width
        {
            get { return m_Width; }
            set
            {
                value = Mathf.Clamp(value, 0, 1);
                if (!Mathf.Approximately(m_Width, value))
                {
                    m_Width = value;
                    SetDirty();
                }
            }
        }

        /// <summary>
        /// Softness for shiny effect.
        /// </summary>
        public float softness
        {
            get { return m_Softness; }
            set
            {
                value = Mathf.Clamp(value, 0.01f, 1);
                if (!Mathf.Approximately(m_Softness, value))
                {
                    m_Softness = value;
                    SetDirty();
                }
            }
        }


        /// <summary>
        /// Brightness for shiny effect.
        /// </summary>
        public float brightness
        {
            get { return m_Brightness; }
            set
            {
                value = Mathf.Clamp(value, 0, 1);
                if (!Mathf.Approximately(m_Brightness, value))
                {
                    m_Brightness = value;
                    SetDirty();
                }
            }
        }

        /// <summary>
        /// Gloss factor for shiny effect.
        /// </summary>
        [System.Obsolete("Use gloss instead (UnityUpgradable) -> gloss")]
        public float highlight
        {
            get { return m_Gloss; }
            set
            {
                value = Mathf.Clamp(value, 0, 1);
                if (!Mathf.Approximately(m_Gloss, value))
                {
                    m_Gloss = value;
                    SetDirty();
                }
            }
        }

        /// <summary>
        /// Gloss factor for shiny effect.
        /// </summary>
        public float gloss
        {
            get { return m_Gloss; }
            set
            {
                value = Mathf.Clamp(value, 0, 1);
                if (!Mathf.Approximately(m_Gloss, value))
                {
                    m_Gloss = value;
                    SetDirty();
                }
            }
        }

        // 高度用到的地方很少，目前先不用
        // public float high
        // {
        //     get { return m_High; }
        //     set
        //     {
        //         value = Mathf.Clamp(value, 0, 10);
        //         if (!Mathf.Approximately(m_High, value))
        //         {
        //             m_High = value;
        //             SetDirty();
        //         }
        //     }
        // }
        /// <summary>
        /// Rotation for shiny effect.
        /// </summary>
        public float rotation
        {
            get { return m_Rotation; }
            set
            {
                if (!Mathf.Approximately(m_Rotation, value))
                {
                    m_Rotation  = value;
                  SetDirty();
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();
            if (m_EffectImages == null || m_EffectImages.Length == 0)
            {
                RefreshImages();
            }
        }

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        protected override void OnEnable()
        {
            CacheMaterial = new Material(Shader.Find(shaderName));
            CacheMaterial.name += "_" + gameObject.name;

            RefreshImageMaterial();
            base.OnEnable();
#if UNITY_EDITOR
            EditorApplication.update += MyUpdateMethod;
#endif
            worldPositionListener.OnMove += OnNavigatorPositionChanged;
        }

        public void RefreshImages()
        {
            m_EffectImages = transform.GetComponentsInChildren<Image>();
        }

        private void RefreshImageMaterial()
        {
            foreach (var image in m_EffectImages)
            {
                image.material = CacheMaterial;
            }
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled () or inactive.
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            worldPositionListener.OnMove -= OnNavigatorPositionChanged;
#if UNITY_EDITOR
            EditorApplication.update -= MyUpdateMethod;
#endif
            CacheMaterial = null;
            foreach (var image in m_EffectImages)
            {
                image.material = null;
            }
        }

        private void OnNavigatorPositionChanged(Vector3 position)
        {
            if (!isActiveAndEnabled) return;
            SetDirty();
        }

      
#if UNITY_EDITOR
        public void MyUpdateMethod()
        {
            worldPositionListener.Update();
        }
        
        protected override void OnValidate()
        {
            if(!isActiveAndEnabled) return;
            base.OnValidate ();
            if (CacheMaterial == null)
            {
                OnEnable();
            }
            SetDirty ();
        }

#endif
        
        public void SetDirty()
        {
            var material1 = CacheMaterial;
            material1.SetVector(WorldSpaceUVs, worldPositionListener.transform.position);
            material1.SetFloat(Width, width);
            material1.SetFloat(Soft, softness);
            material1.SetFloat(Brightness, brightness);
            material1.SetFloat(Gloss, gloss);
            material1.SetFloat(MyRotate, rotation);
            // 高度用到的地方很少，目前先不用
            // material1.SetFloat(High, m_High);
            
            foreach (var image in m_EffectImages)
            {
                image.SetAllDirty();
            }
        }
        
        private static readonly int WorldSpaceUVs = Shader.PropertyToID("_WorldSpaceUVs");
        private static readonly int Width = Shader.PropertyToID("_Width");
        private static readonly int Soft = Shader.PropertyToID("_Soft");
        private static readonly int Brightness = Shader.PropertyToID("_Brightness");
        private static readonly int Gloss = Shader.PropertyToID("_Gloss");
        private static readonly int MyRotate = Shader.PropertyToID("_MyRotate");
        private static readonly int High = Shader.PropertyToID("_High");
    }
}