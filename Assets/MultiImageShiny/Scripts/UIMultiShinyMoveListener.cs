using System;
using UnityEngine;

namespace Coffee.UIEffects
{
    public class UIMultiShinyMoveListener : MonoBehaviour
    {
        public Action<Vector3> OnMove;
        private Vector3 lastPosition;
        void Start()
        {
            // 在开始的时候记录初始位置
            lastPosition = transform.position;
        }
        public void Update()
        {
            // 如果位置有变化
            if (transform.position != lastPosition)
            {
                // 更新最后的位置到当前位置
                var position = transform.position;
                lastPosition = position;
                OnMove?.Invoke(position);
            }
        }
    }
}