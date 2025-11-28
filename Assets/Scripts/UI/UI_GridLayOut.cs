using UnityEngine;
using UnityEngine.UI;

namespace Apis
{
    [RequireComponent(typeof(GridLayoutGroup))]
    public class UI_GridLayOut : MonoBehaviour
    {
        // Grid LayOut Group, 슬롯배치가 필요한 UI에 추가
        // 아니면 따로 클래스 생성해서 상속받아도 됩니다.

        [Header("슬롯 배치 가로 세로")] [Range(1, 10)] [SerializeField]
        private int horizontal; // 가로

        [Range(1, 10)] [SerializeField] private int vertical; // 세로

        [Header("슬롯 패딩 비율")] [Range(0, 0.5f)] [SerializeField] [Tooltip("가로, 0.1 = 전체 10%")]
        private float left;

        [Range(0, 0.5f)] [SerializeField] [Tooltip("가로, 0.1 = 전체 10%")]
        private float right;

        [Range(0, 0.5f)] [SerializeField] [Tooltip("세로, 0.1 = 전체 10%")]
        private float top;

        [Range(0, 0.5f)] [SerializeField] [Tooltip("세로, 0.1 = 전체 10%")]
        private float bottom;

        [Header("슬롯 간 간격 크기")] [Range(0, 1)] [SerializeField] [Tooltip("1 = 슬롯 1개 크기")]
        private float spaceX;

        [Range(0, 1)] [SerializeField] [Tooltip("1 = 슬롯 1개 크기")]
        private float spaceY;

        private GridLayoutGroup grid;
        private RectTransform rect;

        public int Horizontal => horizontal;
        public int Vertical => vertical;

        private void Awake()
        {
            grid = GetComponent<GridLayoutGroup>();
            rect = GetComponent<RectTransform>();
        }

        private void Start()
        {
            // 가로 세로 개수에 맞춰서 슬롯 크기 및 사이 공간 조절

            // grid 패딩값 UI크기 맞춰서 조절
            grid.padding.left = (int)(rect.rect.width * left);
            grid.padding.right = (int)(rect.rect.width * right);
            grid.padding.top = (int)(rect.rect.height * top);
            grid.padding.bottom = (int)(rect.rect.height * bottom);

            // width,height 패딩 값 제외하여 계산
            var width = rect.rect.width - grid.padding.left - grid.padding.right;
            var height = rect.rect.height - grid.padding.top - grid.padding.bottom;

            // space(간격)의 총합
            var sumX = (horizontal - 1) * spaceX;
            var sumY = (vertical - 1) * spaceY;

            // 셀 크기와 간격 알맞게 조절
            grid.spacing = new Vector2(spaceX * (width / (horizontal + sumX)), spaceY * (height / (vertical + sumY)));
            grid.cellSize = new Vector2(width / (horizontal + sumX), height / (vertical + sumY));
        }
    }
}