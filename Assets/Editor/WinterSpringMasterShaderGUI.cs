using System;
using UnityEditor;
using UnityEngine;

public class WinterSpringMasterShaderGUI : ShaderGUI
{
    public enum BlendMode
    {
        AlphaBlend,
        ParticleAdditive,
        Premultiplied,
        Additive,
        SoftAdditive,
        Multiplicative,
        Multiplicative2x,
        Custom
    }

    public static GUIContent[] blendNames =
        Array.ConvertAll(Enum.GetNames(typeof(BlendMode)), item => new GUIContent(item));

    private static readonly GUIContent staticLabel = new();

    private MaterialProperty BlendRGBDst;

    private MaterialProperty BlendRGBSrc;


    private MaterialProperty CullMode;

    private MaterialProperty DissolveMap;

    private MaterialProperty DissolvePolar;

    private MaterialProperty DissolveStepValue;

    private MaterialProperty DissolveUPanner;

    private MaterialProperty DissolveVPanner;
    private MaterialEditor editor;

    private MaterialProperty MainInsSlider;

    private MaterialProperty MainPannerPopupPr;

    private MaterialProperty MainPolar;

    private MaterialProperty MainPowerSlider;

    private MaterialProperty mainTex;

    private MaterialProperty MainUpanner;

    private MaterialProperty MainVpanner;

    private MaterialProperty MaskMap;

    private MaterialProperty MaskUPanner;

    private MaterialProperty MaskVPanner;

    private MaterialProperty NormalMap;

    private MaterialProperty NormalOption;

    private MaterialProperty NormalPolar;

    private MaterialProperty NormalPower;
    private MaterialProperty[] properties;

    private MaterialProperty StepEdgecolor;

    private MaterialProperty StepEdgeThickness;

    private MaterialProperty SubColor;

    private MaterialProperty SubColorHardness;

    private MaterialProperty SubColorOffSet;

    private MaterialProperty SubColorUV;

    private MaterialProperty SubDissolveMap;

    private Material target;

    private MaterialProperty UseCustomDataPr;

    private MaterialProperty UseStepPr;

    private MaterialProperty UseSubColorPr;

    private MaterialProperty UseSubDissolveMap;

    private MaterialProperty VertexNormalStr;

    public override void OnGUI(MaterialEditor editor, MaterialProperty[] properties)
    {
        FindProperties(properties);
        target = editor.target as Material;
        this.editor = editor;
        this.properties = properties;


        DoMain();
    }

    public void FindProperties(MaterialProperty[] props)
    {
        BlendRGBSrc = FindProperty("_BlendRGBSrc", props);
        BlendRGBDst = FindProperty("_BlendRGBDst", props);
        CullMode = FindProperty("_CustomCullMode", props);


        mainTex = FindProperty("_Main_Tex", props);
        MainInsSlider = FindProperty("_Main_Intensity", props);
        MainPowerSlider = FindProperty("_Main_Power", props);
        MainPannerPopupPr = FindProperty("_Main_Panner", props);
        MainUpanner = FindProperty("_Main_Upanner", props);
        MainVpanner = FindProperty("_Main_Vpanner", props);
        MainPolar = FindProperty("_Main_Polar", props);

        UseSubColorPr = FindProperty("_Use_Sub_Color", props);
        SubColorUV = FindProperty("_Sub_Color_UV", props);
        SubColor = FindProperty("_Sub_Color", props);
        SubColorOffSet = FindProperty("_Sub_Color_Offset", props);
        SubColorHardness = FindProperty("_Sub_Color_Hardness", props);

        NormalMap = FindProperty("_Normal_Tex", props);
        NormalPower = FindProperty("_Normal_Strength", props);
        NormalOption = FindProperty("_Normal_Panner_And_Offset", props);
        VertexNormalStr = FindProperty("_Vertex_Normal_Strength", props);
        NormalPolar = FindProperty("_Normal_Polar", props);

        MaskMap = FindProperty("_Mask_Tex", props);
        MaskUPanner = FindProperty("_Mask_Upanner", props);
        MaskVPanner = FindProperty("_Mask_Vpanner", props);
        UseCustomDataPr = FindProperty("_Mask_Custom", props);

        DissolveMap = FindProperty("_Dissolve_Tex", props);
        DissolveUPanner = FindProperty("_Dissolve_Upanner", props);
        DissolveVPanner = FindProperty("_Dissolve_Vpanner", props);
        UseStepPr = FindProperty("_Use_Step", props);
        StepEdgeThickness = FindProperty("_Step_Edge_Thickness", props);
        StepEdgecolor = FindProperty("_Edge_Color", props);
        DissolveStepValue = FindProperty("_Step_Value", props);
        DissolvePolar = FindProperty("_Dissolve_Polar", props);

        SubDissolveMap = FindProperty("_Dissolve_SubTex", props);
        UseSubDissolveMap = FindProperty("_Sub_Dissolve", props);
    }

    public override void ValidateMaterial(Material material)
    {
        SetMaterialKeywords(material);
    }

    private void SetMaterialKeywords(Material material)
    {
        SetKeyword(material, "_USE_NORMAL_ON", material.GetTexture("_Normal_Tex"));
        SetKeyword(material, "_USE_MASK_ON", material.GetTexture("_Mask_Tex"));
        SetKeyword(material, "_USE_DISSOLVE_ON", material.GetTexture("_Dissolve_Tex"));
        SetKeyword(material, "_SUB_DISSOLVE_ON", material.GetTexture("_Dissolve_SubTex"));
    }


    private void SetKeyword(string keyword, bool state)
    {
        if (state)
            foreach (Material m in editor.targets)
                m.EnableKeyword(keyword);
        else
            foreach (Material m in editor.targets)
                m.DisableKeyword(keyword);
    }

    private static void SetKeyword(Material m, string keyword, bool state)
    {
        if (state)
            m.EnableKeyword(keyword);
        else
            m.DisableKeyword(keyword);
    }

    private void SetBool(string keyword, bool state)
    {
        if (state)
            foreach (Material m in editor.targets)
                m.SetFloat(keyword, 1f);
        else
            foreach (Material m in editor.targets)
                m.SetFloat(keyword, 0f);
    }

    private void SetBool(string keyword, float state)
    {
        if (state != 0f)
            foreach (Material m in editor.targets)
                m.SetFloat(keyword, 1f);
        else
            foreach (Material m in editor.targets)
                m.SetFloat(keyword, 0f);
    }

    private void SetFloat(string keyword, float state)
    {
        foreach (Material m in editor.targets) m.SetFloat(keyword, state);
    }

    private void SetInt(string keyword, int state)
    {
        foreach (Material m in editor.targets) m.SetFloat(keyword, state);
    }

    private void SetVector(string keyword, Vector4 state)
    {
        foreach (Material m in editor.targets) m.SetVector(keyword, state);
    }

    private bool IsKeywordEnabled(string keyword)
    {
        return target.IsKeywordEnabled(keyword);
    }

    private MaterialProperty FindProperty(string name)
    {
        return FindProperty(name, properties);
    }

    private static GUIContent MakeLabel(MaterialProperty property, string tooltip = null)
    {
        staticLabel.text = property.displayName;
        staticLabel.tooltip = tooltip;
        return staticLabel;
    }

    private static GUIContent MakeLabel(string textName, string tooltip = null)
    {
        staticLabel.text = textName;
        staticLabel.tooltip = tooltip;
        return staticLabel;
    }

    private void DoOption()
    {
        GUILayout.Label("Setting", EditorStyles.boldLabel);
        EditorGUI.indentLevel += 1;
        editor.ShaderProperty(CullMode, MakeLabel("Cull Mode"));
        BlendModePopup();
        EditorGUI.indentLevel -= 1;
    }

    private BlendMode BlendModeInit()
    {
        var mode = BlendMode.Custom;


        if (BlendRGBSrc.floatValue == (int)UnityEngine.Rendering.BlendMode.SrcAlpha &&
            BlendRGBDst.floatValue == (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha)
            mode = BlendMode.AlphaBlend;
        else if (BlendRGBSrc.floatValue == (int)UnityEngine.Rendering.BlendMode.SrcAlpha &&
                 BlendRGBDst.floatValue == (int)UnityEngine.Rendering.BlendMode.One)
            mode = BlendMode.ParticleAdditive;
        else if (BlendRGBSrc.floatValue == (int)UnityEngine.Rendering.BlendMode.One &&
                 BlendRGBDst.floatValue == (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha)
            mode = BlendMode.Premultiplied;
        else if (BlendRGBSrc.floatValue == (int)UnityEngine.Rendering.BlendMode.One &&
                 BlendRGBDst.floatValue == (int)UnityEngine.Rendering.BlendMode.One)
            mode = BlendMode.Additive;
        else if (BlendRGBSrc.floatValue == (int)UnityEngine.Rendering.BlendMode.OneMinusDstColor &&
                 BlendRGBDst.floatValue == (int)UnityEngine.Rendering.BlendMode.One)
            mode = BlendMode.SoftAdditive;
        else if (BlendRGBSrc.floatValue == (int)UnityEngine.Rendering.BlendMode.DstColor &&
                 BlendRGBDst.floatValue == (int)UnityEngine.Rendering.BlendMode.Zero)
            mode = BlendMode.Multiplicative;
        else if (BlendRGBSrc.floatValue == (int)UnityEngine.Rendering.BlendMode.DstColor &&
                 BlendRGBDst.floatValue == (int)UnityEngine.Rendering.BlendMode.SrcColor)
            mode = BlendMode.Multiplicative2x;
        else
            mode = BlendMode.Custom;

        return mode;
    }

    private void BlendModePopup()
    {
        var mode = BlendModeInit();

        EditorGUI.BeginChangeCheck();
        mode = (BlendMode)EditorGUILayout.Popup(MakeLabel("Blend Mode"), (int)mode, blendNames);

        if (EditorGUI.EndChangeCheck())
            switch (mode)
            {
                case BlendMode.AlphaBlend:
                    SetInt("_BlendRGBSrc", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    SetInt("_BlendRGBDst", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);

                    break;
                case BlendMode.ParticleAdditive:
                    SetInt("_BlendRGBSrc", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    SetInt("_BlendRGBDst", (int)UnityEngine.Rendering.BlendMode.One);
                    break;
                case BlendMode.Premultiplied:
                    SetInt("_BlendRGBSrc", (int)UnityEngine.Rendering.BlendMode.One);
                    SetInt("_BlendRGBDst", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    break;
                case BlendMode.Additive:
                    SetInt("_BlendRGBSrc", (int)UnityEngine.Rendering.BlendMode.One);
                    SetInt("_BlendRGBDst", (int)UnityEngine.Rendering.BlendMode.One);
                    break;
                case BlendMode.SoftAdditive:
                    SetInt("_BlendRGBSrc", (int)UnityEngine.Rendering.BlendMode.OneMinusDstColor);
                    SetInt("_BlendRGBDst", (int)UnityEngine.Rendering.BlendMode.One);
                    break;
                case BlendMode.Multiplicative:
                    SetInt("_BlendRGBSrc", (int)UnityEngine.Rendering.BlendMode.DstColor);
                    SetInt("_BlendRGBDst", (int)UnityEngine.Rendering.BlendMode.Zero);
                    break;
                case BlendMode.Multiplicative2x:
                    SetInt("_BlendRGBSrc", (int)UnityEngine.Rendering.BlendMode.DstColor);
                    SetInt("_BlendRGBDst", (int)UnityEngine.Rendering.BlendMode.SrcColor);
                    break;
            }

        EditorGUI.indentLevel += 1;
        editor.ShaderProperty(BlendRGBSrc, MakeLabel("Blend Src"));
        editor.ShaderProperty(BlendRGBDst, MakeLabel("Blend Dst"));
        EditorGUI.indentLevel -= 1;
    }

    private void DoMain()
    {
        DoOption();

        GUILayout.Label("Main Maps", EditorStyles.boldLabel);

        editor.TexturePropertySingleLine(MakeLabel("Main Texture", "Albedo (RGB)"), mainTex,
            FindProperty("_Main_Color"));

        EditorGUI.indentLevel += 1;
        editor.TextureScaleOffsetProperty(mainTex);


        editor.ShaderProperty(MainInsSlider, MakeLabel("Intensity"));

        editor.ShaderProperty(MainPowerSlider, MakeLabel("Power"));
        EditorGUI.indentLevel -= 1;


        var MainPannerPopup = IsKeywordEnabled("_MAIN_PANNER_ON");

        EditorGUI.indentLevel += 1;
        EditorGUI.BeginChangeCheck();
        editor.ShaderProperty(MainPolar, MakeLabel("Polar Coordinates"));
        editor.ShaderProperty(MainPannerPopupPr, MakeLabel("Panner"));


        EditorGUI.indentLevel -= 1;
        if (EditorGUI.EndChangeCheck()) MainPannerPopup = IsKeywordEnabled("_MAIN_PANNER_ON");
        if (MainPannerPopup)
        {
            EditorGUI.indentLevel += 2;


            var MainPannerValue = new Vector2(MainUpanner.floatValue, MainVpanner.floatValue);
            MainPannerValue = EditorGUILayout.Vector2Field("Value", MainPannerValue);

            SetFloat("_Main_Upanner", MainPannerValue.x);
            SetFloat("_Main_Vpanner", MainPannerValue.y);

            EditorGUI.indentLevel -= 2;
        }

        DoSecondColor();
        DoNormal();
        DoMask();
        DoDissolve();
    }

    private void DoSecondColor()
    {
        GUILayout.Label("Sub Color", EditorStyles.boldLabel);

        var UseSubColor = IsKeywordEnabled("_USE_SUB_COLOR_ON");
        EditorGUI.indentLevel += 1;
        EditorGUI.BeginChangeCheck();

        editor.ShaderProperty(UseSubColorPr, MakeLabel("Use Sub Color"));

        if (EditorGUI.EndChangeCheck()) UseSubColor = IsKeywordEnabled("_USE_SUB_COLOR_ON");
        EditorGUI.indentLevel -= 1;

        if (UseSubColor)
        {
            EditorGUI.indentLevel += 1;

            editor.ShaderProperty(SubColor, MakeLabel("Sub Color"));

            editor.ShaderProperty(SubColorUV, MakeLabel("Direction"));

            editor.ShaderProperty(SubColorOffSet, MakeLabel("OffSet"));

            editor.ShaderProperty(SubColorHardness, MakeLabel("Hardness"));

            EditorGUI.indentLevel -= 1;
        }
    }

    private void DoNormal()
    {
        GUILayout.Label("Normal", EditorStyles.boldLabel);

        var tex = NormalMap.textureValue;
        SetKeyword("_USE_NORMAL_ON", NormalMap.textureValue ? true : false);

        EditorGUI.BeginChangeCheck();
        editor.TexturePropertySingleLine(
            MakeLabel("Normal Texture"), NormalMap,
            tex ? NormalPower : null
        );

        if (EditorGUI.EndChangeCheck() || tex != NormalMap.textureValue)
            SetKeyword("_USE_NORMAL_ON", NormalMap.textureValue ? true : false);

        EditorGUI.indentLevel += 1;

        if (IsKeywordEnabled("_USE_NORMAL_ON"))
        {
            editor.TextureScaleOffsetProperty(NormalMap);

            var NormalOptionValue = NormalOption.vectorValue;

            var NormalPanner = new Vector2(NormalOptionValue.x, NormalOptionValue.y);
            var NormalCustomOffset = new Vector2(NormalOptionValue.z, NormalOptionValue.w);

            editor.ShaderProperty(NormalPolar, MakeLabel("Polar Coordinates"));

            NormalPanner = EditorGUILayout.Vector2Field("Panner", NormalPanner);


            editor.ShaderProperty(VertexNormalStr, MakeLabel("Vertex Normal Strength"));


            var UseCustomDistortion = IsKeywordEnabled("_DISTORTION_CUSTOM_ON");

            EditorGUI.BeginChangeCheck();
            UseCustomDistortion = EditorGUILayout.Toggle("Use Custom Distortion", UseCustomDistortion);

            if (EditorGUI.EndChangeCheck()) SetKeyword("_DISTORTION_CUSTOM_ON", UseCustomDistortion);

            if (IsKeywordEnabled("_DISTORTION_CUSTOM_ON"))
                NormalCustomOffset = EditorGUILayout.Vector2Field("Custom Distortion Offset", NormalCustomOffset);

            SetVector("_Normal_Panner_And_Offset",
                new Vector4(NormalPanner.x, NormalPanner.y, NormalCustomOffset.x, NormalCustomOffset.y));
        }

        EditorGUI.indentLevel -= 1;
    }

    private void DoMask()
    {
        GUILayout.Label("Mask", EditorStyles.boldLabel);

        var tex = MaskMap.textureValue;

        SetKeyword("_USE_MASK_ON", MaskMap.textureValue ? true : false);

        EditorGUI.BeginChangeCheck();
        editor.TexturePropertySingleLine(MakeLabel("Mask Texture"), MaskMap);

        if (EditorGUI.EndChangeCheck() || tex != MaskMap.textureValue)
            SetKeyword("_USE_MASK_ON", MaskMap.textureValue ? true : false);

        EditorGUI.indentLevel += 1;

        if (IsKeywordEnabled("_USE_MASK_ON"))
        {
            editor.TextureScaleOffsetProperty(MaskMap);

            var MaskPannerValue = new Vector2(MaskUPanner.floatValue, MaskVPanner.floatValue);
            MaskPannerValue = EditorGUILayout.Vector2Field("Panner", MaskPannerValue);

            SetFloat("_Mask_Upanner", MaskPannerValue.x);
            SetFloat("_Mask_Vpanner", MaskPannerValue.y);

            var UseCustomDataValue = IsKeywordEnabled("_MASK_CUSTOM_ON");

            EditorGUI.BeginChangeCheck();
            editor.ShaderProperty(UseCustomDataPr, MakeLabel("Mask Use Custom Data(Y)"));
            //UseCustomDataValue = EditorGUILayout.Toggle("Mask Use Custom Data(Y)", UseCustomDataValue);

            if (EditorGUI.EndChangeCheck()) SetKeyword("_MASK_CUSTOM_ON", UseCustomDataValue);
        }

        EditorGUI.indentLevel -= 1;
    }

    private void DoDissolve()
    {
        GUILayout.Label("Dissolve", EditorStyles.boldLabel);

        var tex = DissolveMap.textureValue;

        SetKeyword("_USE_DISSOLVE_ON", DissolveMap.textureValue ? true : false);

        EditorGUI.BeginChangeCheck();
        editor.TexturePropertySingleLine(MakeLabel("Dissolve Texuture"), DissolveMap);
        if (EditorGUI.EndChangeCheck() || tex != DissolveMap.textureValue)
            SetKeyword("_USE_DISSOLVE_ON", DissolveMap.textureValue ? true : false);
        EditorGUI.indentLevel += 1;
        if (IsKeywordEnabled("_USE_DISSOLVE_ON"))
        {
            editor.TextureScaleOffsetProperty(DissolveMap);

            editor.ShaderProperty(DissolvePolar, MakeLabel("Polar Coordinates"));

            var DisslovePannerValue = new Vector2(DissolveUPanner.floatValue, DissolveVPanner.floatValue);
            DisslovePannerValue = EditorGUILayout.Vector2Field("Panner", DisslovePannerValue);


            SetFloat("_Dissolve_Upanner", DisslovePannerValue.x);
            SetFloat("_Dissolve_Vpanner", DisslovePannerValue.y);

            DoSubDissolve();

            var UseStep = IsKeywordEnabled("_USE_STEP_ON");
            EditorGUI.BeginChangeCheck();
            editor.ShaderProperty(UseStepPr, MakeLabel("Use Step"));

            if (EditorGUI.EndChangeCheck()) UseStep = IsKeywordEnabled("_USE_STEP_ON");

            if (UseStep)
            {
                EditorGUI.indentLevel += 1;

                editor.ShaderProperty(DissolveStepValue, MakeLabel("Step Value"));
                editor.ShaderProperty(StepEdgeThickness, MakeLabel("Edge Thickness"));
                editor.ShaderProperty(StepEdgecolor, MakeLabel("Edge Color"));


                EditorGUI.indentLevel -= 1;
            }
        }

        EditorGUI.indentLevel -= 1;
    }

    private void DoSubDissolve()
    {
        var tex = SubDissolveMap.textureValue;

        SetKeyword("_SUB_DISSOLVE_ON", SubDissolveMap.textureValue ? true : false);

        EditorGUI.BeginChangeCheck();
        editor.TexturePropertySingleLine(MakeLabel("Sub Dissolve Texuture"), SubDissolveMap);

        if (EditorGUI.EndChangeCheck() || tex != SubDissolveMap.textureValue)
            SetKeyword("_SUB_DISSOLVE_ON", SubDissolveMap.textureValue ? true : false);
        if (IsKeywordEnabled("_SUB_DISSOLVE_ON")) editor.TextureScaleOffsetProperty(SubDissolveMap);
    }


    private enum UV
    {
        U,
        V
    }
}