
namespace pumpkin.swf
{
    using com.gt;
    using com.gt.assets;
    using com.gt.units;
    using pumpkin.display;
    using pumpkin.displayInternal;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using UnityEngine;

    /// <summary>
    /// 
    /// </summary>
    public class BuiltinResourceLoader : IAssetLoader
    {
        private static readonly Dictionary<string, IAssetBundle> bundles = new Dictionary<string, IAssetBundle>();
        private static readonly Dictionary<string, SwfAssetContext> contextCache = new Dictionary<string, SwfAssetContext>();
        private static Shader baseBitmapShader;
        private static string baseBitmapShaderName = "Shaders/uniSWF-Double-Alpha-Diffuse";
        private static BuiltinResourceLoader m_instance;
        /// <summary>
        /// 
        /// </summary>
        public static BuiltinResourceLoader instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new BuiltinResourceLoader();
                }
                return m_instance;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private BuiltinResourceLoader()
        {
            baseBitmapShader = (Shader) Resources.Load(baseBitmapShaderName, typeof(Shader));
            if (baseBitmapShader == null)
            {
                Debug.LogError("Failed to load uniSWF shader '" + baseBitmapShaderName + "' ");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="swfInfoAsset"></param>
        /// <param name="swfUri"></param>
        /// <param name="useAssetBundle"></param>
        /// <param name="texSize"></param>
        /// <returns></returns>
        private SwfAssetContext _loadFromTextAsset(TextAsset swfInfoAsset, string swfUri, IAssetBundle useAssetBundle,Vector2 texSize)
        {
            ByteArray b = new ByteArray(swfInfoAsset.bytes);
            MovieClipReader reader2 = new MovieClipReader();
            SwfAssetContext context = null;
            try
            {
                context = reader2.readSwfAssetContext(b, texSize);
            }
            catch (EndOfStreamException)
            {
                Debug.LogError("MovieClip() corrupt swf '" + swfUri + "'");
                return null;
            }
            catch (Exception exception)
            {
                Debug.LogError("MovieClip() corrupt swf '" + swfUri + "', " + exception.Message);
                return null;
            }
            if (context == null)
            {
                Debug.LogError("MovieClip() Invalid swf '" + swfUri + "'");
                return null;
            }
            context.swfPrefix = swfUri;
            contextCache[swfUri] = context;
            return context;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static string fixAssetBundleUri(string uri)
        {
            int num = uri.LastIndexOf("/");
            if (num != -1)
            {
                return uri.Substring(num + 1);
            }
            return uri;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="swfUri"></param>
        /// <param name="useAssetBundle"></param>
        /// <returns></returns>
        internal SwfAssetContext loadSWF(string swfUri, IAssetBundle useAssetBundle)
        {
            if (swfUri.EndsWith(".swf"))
            {
                swfUri = swfUri.Substring(0, swfUri.IndexOf(".swf"));
            }
            if (contextCache.ContainsKey(swfUri))
            {
                return contextCache[swfUri];
            }
            string uri = swfUri;
            if (useAssetBundle == null) return null;
            TextAsset swfInfoAsset = useAssetBundle.Load<TextAsset>(uri);

            if (swfInfoAsset == null)
            {
                Debug.LogError("MovieClip() Invalid asset url '" + swfUri + "' actual url loaded '" + uri + "'");
                return null;
            }
            string texuri = swfUri+"_tex";
            Texture texture = useAssetBundle.Load<Texture>(texuri);
            if (texture == null)
            {
                Debug.Log(string.Concat("Failed to load texture: ", texuri, " from bundle: ", useAssetBundle));
                return null;
            }
            Material material = new Material(baseBitmapShader)
            {
                color = Color.white,
                name = texuri
            };
            texture.name = texuri;
            material.mainTexture = texture;

            SwfAssetContext context = _loadFromTextAsset(swfInfoAsset, uri, useAssetBundle, new Vector2(texture.width, texture.height));
            if (context != null)
            {
                context.texture = texture;
                context.material = material;
            }

            useAssetBundle.Unload(uri);
            return context;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="swfUri"></param>
        private void removeAssetBundle(string swfUri)
        {
            bundles.Remove(swfUri);
            contextCache.Remove(swfUri);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="swfUri"></param>
        /// <returns></returns>
        public bool unloadSWF(string swfUri)
        {
            if (!bundles.ContainsKey(swfUri))
            {
                return false;
            }
            IAssetBundle bundle = bundles[swfUri];
            bundle.Unload(true);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bundle"></param>
        public void AddAsset(IAssetBundle bundle)
        {
            string name = fixAssetBundleUri(bundle.Name);
            if (bundles.ContainsKey(name))
            {
                unloadSWF(name);
            }
            bundles.Add(name, bundle);

            loadSWF(name, bundle);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bundle"></param>
        public void RemoveAsset(IAssetBundle bundle)
        {
            string name = fixAssetBundleUri(bundle.Name);
            removeAssetBundle(name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assetParameter"></param>
        /// <param name="loadType"></param>
        public void LoadAsset(AssetParameter assetParameter, string loadType)
        {
        }
    }
}

