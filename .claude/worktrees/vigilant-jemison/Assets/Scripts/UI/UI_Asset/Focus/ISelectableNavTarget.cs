using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISelectableNavTarget : IUI_Navigatable , IUI_NavigationManager
{
    void OnSelected(bool focus);      // Selector가 선택했을 때
    void OnDeselected();              // Selector가 다른 애 선택할 때/나갈 때
}
