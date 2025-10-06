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

        private List<List<Transform>> targetsForImage = new List<List<Transform>>();

        private void Start()
        {
            // разбиваем слоты на подсписки для каждого Image
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
            float minDist = 50f;

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
        private Vector2 offset;
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

            // ничего не сохраняем, просто фиксируем что начали перетаскивать
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (isLocked) return;

            Vector3 worldPos;
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
                    canvas.transform as RectTransform,
                    eventData.position,
                    eventData.pressEventCamera,
                    out worldPos))
            {
                rectTransform.position = worldPos;
            }
        }


        public void OnEndDrag(PointerEventData eventData)
        {
            if (isLocked) return;

            // проверяем ближайший слот для магнитного эффекта
            Transform snapTarget = manager.GetSnapTarget(rectTransform.position, imageIndex);
            if (snapTarget != null)
            {
                rectTransform.position = snapTarget.position;
                isLocked = true; // больше нельзя двигать
            }
        }
    }
}