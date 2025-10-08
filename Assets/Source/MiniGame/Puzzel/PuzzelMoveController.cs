using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Source.MiniGame.Puzzel
{
    public class DragUIManager : MonoBehaviour
    {
        [Header("Перетаскиваемые объекты")]
        [SerializeField] private List<Image> draggableImages;

        [Header("Все слоты для магнита в одном списке (в инспекторе)")]
        [SerializeField] private List<Transform> allTargets;

        [Header("Количество слотов на каждый Image")]
        [SerializeField] private int slotsPerImage = 1;

        [Header("Радиус притягивания (в пикселях или единицах Canvas)")]
        [SerializeField] private float snapRadius = 50f;

        private List<List<Transform>> targetsForImage = new List<List<Transform>>();

        private void Start()
        {
            targetsForImage.Clear();
            int index = 0;
            for (int i = 0; i < draggableImages.Count; i++)
            {
                var sublist = new List<Transform>();
                for (int j = 0; j < slotsPerImage && index < allTargets.Count; j++, index++)
                {
                    sublist.Add(allTargets[index]);
                }
                targetsForImage.Add(sublist);

                var img = draggableImages[i];
                if (img != null && img.gameObject.GetComponent<DragUI>() == null)
                {
                    var drag = img.gameObject.AddComponent<DragUI>();
                    drag.manager = this;
                    drag.imageIndex = i;
                }
            }
        }

        public Transform GetSnapTarget(Vector3 position, int imageIndex)
        {
            if (imageIndex < 0 || imageIndex >= targetsForImage.Count) return null;

            Transform closest = null;
            float minDist = snapRadius; // используем поле из инспектора

            foreach (var target in targetsForImage[imageIndex])
            {
                float dist = Vector3.Distance(position, target.position);
                if (dist <= minDist)
                {
                    minDist = dist;
                    closest = target;
                }
            }

            return closest;
        }
    }

    public class DragUI : MonoBehaviour, IPointerDownHandler, IDragHandler, IEndDragHandler
    {
        [HideInInspector] public DragUIManager manager;
        [HideInInspector] public int imageIndex = 0;

        private RectTransform rectTransform;
        private Canvas canvas;
        private bool isLocked = false;

        private void Start()
        {
            rectTransform = GetComponent<RectTransform>();
            canvas = GetComponentInParent<Canvas>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (isLocked) return;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (isLocked) return;

            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
                canvas.transform as RectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out Vector3 worldPos))
            {
                rectTransform.position = worldPos;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (isLocked) return;

            Transform snapTarget = manager.GetSnapTarget(rectTransform.position, imageIndex);
            if (snapTarget != null)
            {
                rectTransform.position = snapTarget.position;
                isLocked = true;
            }
        }
    }
}
