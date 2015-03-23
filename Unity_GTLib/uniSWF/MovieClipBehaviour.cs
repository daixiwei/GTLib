using com.platform.unity;
using pumpkin.display;
using pumpkin.displayInternal;
using pumpkin.swf;
using System;
using UnityEngine;
using com.gt;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MovieClipBehaviour : MonoBehaviour
{
    public Color colorTransform = Color.white;
    public static Vector2 defaultDrawScale = new Vector2(1f, 1f);
    protected Vector2 drawScale = Vector2.zero;
    public bool flipY = false;
    private int fps = 24;
    private float frameDrift = 0.0f;
    internal IGraphicsGenerator gfxGenerator;

    public bool loop = true;
    protected Vector3 m_TmpVector = new Vector3();
    protected MeshFilter meshFilter;
    protected MeshRenderer meshRenderer;
    public MovieClip movieClip;
    public DisplayObjectContainer stage;

    private float t;
    protected float updateInterval;
    private int m_RenderQueue = 2000;
    public bool FastMode = true;
    /// <summary>
    /// 
    /// </summary>
    public int RenderQueue
    {
        set { this.m_RenderQueue = value; }
        get { return m_RenderQueue; }
    }


    /// <summary>
    /// 
    /// </summary>
    void Awake()
    {
        stage = new DisplayObjectContainer();
        meshFilter = gameObject.GetComponent<MeshFilter>();
        if (meshFilter == null)
            meshFilter = (MeshFilter)gameObject.AddComponent(typeof(MeshFilter));
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        if (meshRenderer == null)
            meshRenderer = (MeshRenderer)gameObject.AddComponent(typeof(MeshRenderer));

        meshRenderer.castShadows = false;
        meshRenderer.receiveShadows = false;

        gfxGenerator = instanceGfxGenerator();
        if (drawScale == Vector2.zero)
        {
            drawScale = defaultDrawScale;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected virtual IGraphicsGenerator instanceGfxGenerator()
    {
        IGraphicsGenerator generator = null;
        if (FastMode)
        {
            generator = new FastGraphicsDrawMeshGenerator();
        }
        else
        {
            generator = new GraphicsMeshGenerator();
        }

        return generator;
    }

    /// <summary>
    /// 
    /// </summary>
    void Update()
    {
        RenderFrame();
    }

    /// <summary>
    /// 
    /// </summary>
    public void RenderFrame()
    {
        if (stage != null)
        {
            t += GTLib.GameManager.DeltaTime;
            if (t >= updateInterval)
            {
                frameDrift = t - updateInterval;
                t = frameDrift;
                if (t > updateInterval)
                {
                    t = updateInterval;
                }
                stage.updateFrame();
                if ((meshFilter != null) && ((gfxGenerator != null) && gfxGenerator.renderStage(stage, m_RenderQueue)))
                {
                    meshFilter.sharedMesh = gfxGenerator.applyToMeshRenderer(meshRenderer);
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fps"></param>
    public void SetFps(float fps)
    {
        this.fps = (int)fps;
        updateInterval = 1f / fps;
        frameDrift = 0.0f;
        t = 0.0f;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="uri"></param>
    public void SetUri(string uri)
    {
        movieClip = null;
        meshFilter.mesh.Clear();
        stage.removeAllChildren();
        SwfURI furi = new SwfURI(uri);
        movieClip = new MovieClip(furi);
        movieClip.looping = loop;
        movieClip.colorTransform = colorTransform;
        movieClip.alpha = colorTransform.a;
        movieClip.scaleX = drawScale.x;
        if (flipY)
        {
            movieClip.scaleY = drawScale.y;
        }
        else
        {
            movieClip.scaleY = -drawScale.y;
        }
        stage.addChild(movieClip);
        RenderFrame();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="symbolName"></param>
    public void SetSymbolName(string symbolName)
    {
        movieClip.setSymbolName(symbolName);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="uri"></param>
    public MovieClip AddMovieClip(string uri)
    {
        SwfURI furi = new SwfURI(uri);
        MovieClip movieClip = new MovieClip(furi);
        movieClip.looping = loop;
        movieClip.colorTransform = colorTransform;
        movieClip.alpha = colorTransform.a;
        movieClip.scaleX = drawScale.x;
        if (flipY)
        {
            movieClip.scaleY = drawScale.y;
        }
        else
        {
            movieClip.scaleY = -drawScale.y;
        }
        stage.addChild(movieClip);
        return movieClip;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="index"></param>
    public MovieClip AddMovieClip(string uri, int index)
    {
        SwfURI furi = new SwfURI(uri);
        MovieClip movieClip = new MovieClip(furi);
        movieClip.looping = loop;
        movieClip.colorTransform = colorTransform;
        movieClip.alpha = colorTransform.a;
        movieClip.scaleX = drawScale.x;
        if (flipY)
        {
            movieClip.scaleY = drawScale.y;
        }
        else
        {
            movieClip.scaleY = -drawScale.y;
        }
        stage.addChildAt(movieClip, index);
        return movieClip;
    }



    /// <summary>
    /// 
    /// </summary>
    public int CurrentIndex
    {
        get
        {
            return stage.getChildIndex(movieClip);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public int CurrentFrame
    {
        get
        {
            return movieClip.getCurrentFrame();
        }
        set
        {
            movieClip.setFrame(value);
        }
    }
}

