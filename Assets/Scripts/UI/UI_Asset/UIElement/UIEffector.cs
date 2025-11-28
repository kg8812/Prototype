using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Apis.UI
{
    public enum PropertyNames
    {
        _IsHovering,
        _IsSelect,
        _UnScaledTime
    }

    [Serializable]
    public struct UIElStateEffector
    {
        public UIElementState onStates;
        public bool isSetActive;
        public bool isSetMat;
        public bool isLoop;
        public bool isText;
        public bool isSetColor;
        public PropertyNames matProperty;
        public PropertyNames loopProperty;
        public GameObject[] objs;
        public Graphic[] graphics;
        public TextMeshProUGUI text;
        public Color textColor;
        public Color imgColor;

        [HideInInspector] public Material[] matForCode;
        [HideInInspector] public int matPropInt;
        [HideInInspector] public int looopPropInt;
    }

    public class UIEffector : UIElement
    {
        private const float ColorChangeTime = 0.2f;
        public UIElStateEffector[] Effectors;

        private UIElStateEffector _effector;

        private void Update()
        {
            for (var i = 0; i < Effectors.Length; i++)
            {
                _effector = Effectors[i];
                if (_effector.isLoop && (_effector.onStates & ElState) != 0)
                    // Debug.Log("checking");
                    for (var j = 0; j < _effector.matForCode.Length; j++)
                        _effector.matForCode[j].SetFloat(_effector.looopPropInt, Time.unscaledTime);
            }
        }


        public override void Init()
        {
            base.Init();
            for (var i = 0; i < Effectors.Length; i++)
            {
                Effectors[i].matPropInt = Shader.PropertyToID(Effectors[i].matProperty.ToString());
                Effectors[i].looopPropInt = Shader.PropertyToID(Effectors[i].loopProperty.ToString());

                Effectors[i].matForCode = new Material[Effectors[i].graphics.Length];
                for (var j = 0; j < Effectors[i].graphics.Length; j++)
                {
                    var targetMat = Instantiate(Effectors[i].graphics[j].material);
                    Effectors[i].graphics[j].material = targetMat;
                    Effectors[i].matForCode[j] = targetMat;
                }
            }

            ElStateChanged(ElState);
        }


        protected override void ElStateChanged(UIElementState elState)
        {
            base.ElStateChanged(elState);
            for (var i = 0; i < Effectors.Length; i++)
            {
                _effector = Effectors[i];
                if (_effector.isSetMat)
                    // Debug.Log($"isTrue {(_effector.onStates & elState) != 0}");
                    for (var j = 0; j < _effector.matForCode.Length; j++)
                        _effector.matForCode[j]
                            .SetInt(_effector.matPropInt, (_effector.onStates & elState) != 0 ? 1 : 0);

                if (_effector.isSetActive)
                    for (var j = 0; j < _effector.objs.Length; j++)
                        _effector.objs[j].SetActive((_effector.onStates & elState) != 0);

                if (_effector.isText && (elState & _effector.onStates) != 0) _effector.text.color = _effector.textColor;

                if (_effector.isSetColor && (elState & _effector.onStates) != 0)
                    for (var j = 0; j < _effector.graphics.Length; j++)
                        _effector.graphics[j].DOColor(_effector.imgColor, ColorChangeTime).SetUpdate(true);
            }
        }
    }
}