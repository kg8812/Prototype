using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOnAwakeObject : MonoBehaviour
{
    public void Awake()
    {
        gameObject.SetActive(false);
    }
}
