using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public partial class Player
{
    [TabGroup("기획쪽 수정 변수들/group1", "특수 액션")]
    public ActionInfo actionInfo;
    private ActionController _actionController;
    public ActionController ActionController => _actionController ??= new(this, actionInfo);
}
