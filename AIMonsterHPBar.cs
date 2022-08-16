using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMonsterHPBar : MonoBehaviour
{
    private Camera UICamera;
    private Canvas canvas;
    private RectTransform rectParent;
    private RectTransform rectHP;

    [HideInInspector] public Vector3 offset = Vector3.zero; //몬스터 머리에서 얼마만큼 떨어져서 생성할 것인지
    [HideInInspector] public Transform MonsterTr;

    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        UICamera = canvas.worldCamera;
        rectParent = canvas.GetComponent<RectTransform>();
        rectHP = this.gameObject.GetComponent<RectTransform>();
    }

    // 몬스터의 HP 계산이 끝난 후 업데이트 되어야 하니까 LateUpdate 사용
    private void LateUpdate()
    {
        if(MonsterTr != null)
        {
            var screenPos = Camera.main.WorldToScreenPoint(MonsterTr.position + offset);

            if (screenPos.z < 0.0f) // 2d는 x, y 만 필요, 3d라 z도 반환, 메인카메라에서 XY평면까지의 거리, 뒤를 돌면 안보이게
            {
                screenPos *= -1.0f;
            }
            var localPos = Vector2.zero;

            // UI 캔버스에서 사용하기 위해
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectParent, screenPos, UICamera, out localPos);

            rectHP.localPosition = localPos;
        }
    }
}
