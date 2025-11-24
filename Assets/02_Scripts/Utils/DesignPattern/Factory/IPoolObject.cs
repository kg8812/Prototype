using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 오브젝트 풀링에 넣을 오브젝트용 인터페이스

public interface IPoolObject
{
    public void OnGet(); // 오브젝트 가져올 때 호출되는 함수
    public void OnReturn(); // 오브젝트 되돌려보낼 때 호출되는 함수
}
