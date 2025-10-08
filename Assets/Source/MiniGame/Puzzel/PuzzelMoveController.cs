using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Source.MiniGame.Puzzel
{
    public class DragUIManager : MonoBehaviour
    {
        [SerializeField] private GameObject puzzle;
        [SerializeField] private GameObject exitButton;
        
        [Header("Перетаскиваемые объекты")]
        [SerializeField] private List<Image> draggableImages;

        [Header("Все слоты для магнита в одном списке (в инспекторе)")]
        [SerializeField] private List<Transform> allTargets;

        [Header("Количество слотов на каждый Image")]
        [SerializeField] private int slotsPerImage = 1;

        [Header("Радиус притягивания (в пикселях или единицах Canvas)")]
        [SerializeField] private float snapRadius;
        

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
        
        public bool AreAllTargetsFilled()
        {
            float threshold = 0.1f; // погрешность для сравнения позиций

            for (int i = 0; i < targetsForImage.Count; i++)
            {
                foreach (var target in targetsForImage[i])
                {
                    bool filled = false;
                    foreach (var img in draggableImages)
                    {
                        if (Vector3.Distance(img.transform.position, target.position) <= threshold)
                        {
                            filled = true;
                            break;
                        }
                    }

                    if (!filled)
                        return false;
                }
            }
            puzzle.SetActive(false);
            exitButton.SetActive(true);
            return true;
        }

    }
    public class DragUI : MonoBehaviour, IPointerDownHandler, IDragHandler, IEndDragHandler
    {
        [HideInInspector] public DragUIManager manager;
        [HideInInspector] public int imageIndex = 0;

        private RectTransform rectTransform;
        private Canvas canvas;
        private bool isLocked = false;
        private int originalSiblingIndex;
        private Transform originalParent;

        private void Start()
        {
            rectTransform = GetComponent<RectTransform>();
            canvas = GetComponentInParent<Canvas>();
            originalParent = transform.parent;
            originalSiblingIndex = transform.GetSiblingIndex(); // запоминаем исходное место
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (isLocked) return;

            // Поднимаем объект на передний план
            transform.SetAsLastSibling();

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

        // Внутри DragUI
        public void OnEndDrag(PointerEventData eventData)
        {
            if (isLocked) return;

            Transform snapTarget = manager.GetSnapTarget(rectTransform.position, imageIndex);

            if (snapTarget != null)
            {
                rectTransform.position = snapTarget.position;
                isLocked = true;
                transform.SetSiblingIndex(originalSiblingIndex);
            }

            // Сразу проверяем после дропа, все ли слоты заняты
            manager.AreAllTargetsFilled();
        }


    }
}
