using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;

public class ActiveSkillItemCreator : MonoBehaviour
{
    public int skillId;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            var pickUp = GameManager.Item.ActiveSkillPickUp.CreateNew(skillId);
            var pos = CameraManager.instance.MainCam.ScreenToWorldPoint(Input.mousePosition);
            pickUp.transform.position = new Vector3(pos.x, pos.y, 0);
        }
    }
}
