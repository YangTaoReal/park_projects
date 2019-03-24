// Advanced Dynamic Shaders
// Copyright Cristian Pop - https://boxophobic.com/

using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

[HelpURL("https://docs.google.com/document/d/13vul0zDF478he8hhteKjnxoLYgfW47G0Z9TSox21_J0/edit#heading=h.rp8ji698m9wz")]
[DisallowMultipleComponent]
[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer))]
public class ADSGlobals : MonoBehaviour {

    public enum DebugEnum
    {
        off = -1,
        vertexColorR = 1,
        vertexColorG = 2,
        vertexColorB = 3,
        vertexAlpha = 4,
        motionMask = 11,
        motionNoise = 12,
        grassTint = 21,
        grassSize = 22,
    };

    public DebugEnum debug = DebugEnum.off;

    [Space(20)]
    public float globalAmplitude = 0.5f;
	public float globalSpeed = 4.0f;
	public float globalScale = 0.5f;

    [Space(20)]
    public Texture2D noiseTexture = null;
    public float noiseContrast = 1.0f;
    public float noiseSpeed = 1.0f;
    public float noiseScale = 1.0f;


    public enum GrassTintModeEnum
    {
        texture = 0,
        colors = 1,
    };

    [Space(20)]
    public GrassTintModeEnum grassTintMode = GrassTintModeEnum.colors;
    public Texture2D grassTintTexture = null;
    public float grassTintIntensity = 1.0f;
    public Color grassTintColorOne = Color.white;
    public Color grassTintColorTwo = Color.white;
    public Vector4 grassTintScaleOffset = new Vector4(1.0f, 1.0f, 0.0f, 0.0f);

    [Space(20)]
    public Texture2D grassSizeTexture = null;
    public float grassSizeMin = 0.0f;
    public float grassSizeMax = 1.0f;
    public Vector4 grassSizeScaleOffset = new Vector4(1.0f, 1.0f, 0.0f, 0.0f);

    [Space(20)]
    public List<Mesh> ADSObjects = new List<Mesh>();

    private bool somethingChanged = false;

    private float old_globalAmplitude;
    private float old_globalSpeed;
    private float old_globalScale;
    private Vector3 old_globalDirection;

    private Texture2D old_noiseTexture;
    private float old_noiseContrast;
    private float old_noiseSpeed;
    private float old_noiseScale;

    private GrassTintModeEnum old_grassTintMode;
    private Texture2D old_grassTintTexture;
    private float old_grassTintIntensity;
    private Color old_grassTintColorOne;
    private Color old_grassTintColorTwo;
    private Vector4 old_grassTintScaleOffset;

    private Texture2D old_grassSizeTexture;
    private float old_grassSizeMin;
    private float old_grassSizeMax;
    private Vector4 old_grassSizeScaleOffset;

    private Shader debugShader;
    private bool debugShader_ON = false;

    void Awake(){

		// Set gameobject name to be searchable
		gameObject.name = "ADS Globals";

        ADSObjects = new List<Mesh>();

        // Send global information to shaders
        SetGlobalShaderProperties();

        // Disable Arrow in play mode
        if (Application.isPlaying == true)
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
        else
        {
            gameObject.GetComponent<MeshRenderer>().enabled = true;
        }

#if UNITY_EDITOR
        // Set Debug Shader
        debugShader = Shader.Find("Utils/ADS Debug");
#endif
    }

#if UNITY_EDITOR
    void Update ()
    {

        if (SceneView.lastActiveSceneView != null)
        {
            if (debug == DebugEnum.off)
            {
                if (debugShader_ON == true)
                {
                    SceneView.lastActiveSceneView.SetSceneViewShaderReplace(null, null);
                    SceneView.lastActiveSceneView.Repaint();

                    debugShader_ON = false;
                }
            }
            else
            {
                SceneView.lastActiveSceneView.SetSceneViewShaderReplace(debugShader, null);
                SceneView.lastActiveSceneView.Repaint();

                debugShader_ON = true;

                Shader.SetGlobalFloat("ADS_DebugMode", (int)debug);
            }
        }


        CheckSomethingChanged();

        if (somethingChanged == true)
        {
            SetGlobalShaderProperties();
            UpdateSomethingChanged();
        }
    }
#endif

    // Send global information to shaders
    void SetGlobalShaderProperties()
    {
        Shader.SetGlobalVector("ADS_GlobalDirection", gameObject.transform.forward);
        Shader.SetGlobalFloat("ADS_GlobalAmplitude", globalAmplitude);
        Shader.SetGlobalFloat("ADS_GlobalSpeed", globalSpeed);
        Shader.SetGlobalFloat("ADS_GlobalScale", globalScale);

        if (noiseTexture == null || noiseContrast <= 0)
        {
            Shader.SetGlobalFloat("ADS_NoiseTex_ON", 0.0f);
        }
        else
        {
            Shader.SetGlobalFloat("ADS_NoiseTex_ON", 1.0f);
            Shader.SetGlobalTexture("ADS_NoiseTex", noiseTexture);
            Shader.SetGlobalFloat("ADS_NoiseContrast", noiseContrast);
            Shader.SetGlobalFloat("ADS_NoiseSpeed", noiseSpeed * 0.1f);
            Shader.SetGlobalFloat("ADS_NoiseScale", noiseScale * 0.1f);
        }

        if (grassTintTexture == null || grassTintIntensity <= 0 || (grassTintColorOne == Color.white && grassTintColorTwo == Color.white))
        {
            Shader.SetGlobalFloat("ADS_GrassTintTex_ON", 0.0f);
        }
        else
        {
            if (grassTintMode == GrassTintModeEnum.texture)
            {
                Shader.SetGlobalFloat("ADS_GrassTintModeColors", 0.0f);
            }
            else
            {
                Shader.SetGlobalFloat("ADS_GrassTintModeColors", 1.0f);
            }

            Shader.SetGlobalFloat("ADS_GrassTintTex_ON", 1.0f);
            Shader.SetGlobalTexture("ADS_GrassTintTex", grassTintTexture);
            Shader.SetGlobalFloat("ADS_GrassTintIntensity", grassTintIntensity);
            Shader.SetGlobalColor("ADS_GrassTintColorOne", grassTintColorOne);
            Shader.SetGlobalColor("ADS_GrassTintColorTwo", grassTintColorTwo);
            Shader.SetGlobalVector("ADS_GrassTintScaleOffset", grassTintScaleOffset);
        }

        if (grassSizeTexture == null)
        {
            Shader.SetGlobalFloat("ADS_GrassSizeTex_ON", 0.0f);
        }
        else
        {
            Shader.SetGlobalFloat("ADS_GrassSizeTex_ON", 1.0f);
            Shader.SetGlobalTexture("ADS_GrassSizeTex", grassSizeTexture);
            Shader.SetGlobalFloat("ADS_GrassSizeMin", grassSizeMin - 1.0f);
            Shader.SetGlobalFloat("ADS_GrassSizeMax", grassSizeMax - 1.0f);
            Shader.SetGlobalVector("ADS_GrassSizeScaleOffset", grassSizeScaleOffset);
        }
    }

    void CheckSomethingChanged()
    {
        somethingChanged |= old_globalAmplitude != globalAmplitude;
        somethingChanged |= old_globalSpeed != globalSpeed;
        somethingChanged |= old_globalScale != globalScale;
        somethingChanged |= old_globalDirection != gameObject.transform.forward;

        somethingChanged |= old_noiseTexture != noiseTexture;
        somethingChanged |= old_noiseContrast != noiseContrast;
        somethingChanged |= old_noiseSpeed != noiseSpeed;
        somethingChanged |= old_noiseScale != noiseScale;

        somethingChanged |= old_grassTintMode != grassTintMode;
        somethingChanged |= old_grassTintTexture != grassTintTexture;
        somethingChanged |= old_grassTintIntensity != grassTintIntensity;
        somethingChanged |= old_grassTintColorOne != grassTintColorOne;
        somethingChanged |= old_grassTintColorTwo != grassTintColorTwo;
        somethingChanged |= old_grassTintScaleOffset != grassTintScaleOffset;

        somethingChanged |= old_grassSizeTexture != grassSizeTexture;
        somethingChanged |= old_grassSizeMin != grassSizeMin;
        somethingChanged |= old_grassSizeMax != grassSizeMax;
        somethingChanged |= old_grassSizeScaleOffset != grassSizeScaleOffset;
    }

    void UpdateSomethingChanged()
    {
        somethingChanged = false;

        old_globalAmplitude = globalAmplitude;
        old_globalSpeed = globalSpeed;
        old_globalScale = globalScale;
        old_globalDirection = gameObject.transform.forward;

        old_noiseTexture = noiseTexture;
        old_noiseContrast = noiseContrast;
        old_noiseSpeed = noiseSpeed;
        old_noiseScale = noiseScale;

        old_grassTintMode = grassTintMode;
        old_grassTintTexture = grassTintTexture;
        old_grassTintIntensity = grassTintIntensity;
        old_grassTintColorOne = grassTintColorOne;
        old_grassTintColorTwo = grassTintColorTwo;
        old_grassTintScaleOffset = grassTintScaleOffset;

        old_grassSizeTexture = grassSizeTexture;
        old_grassSizeMin = grassSizeMin;
        old_grassSizeMax = grassSizeMax;
        old_grassSizeScaleOffset = grassSizeScaleOffset;
    }
}
