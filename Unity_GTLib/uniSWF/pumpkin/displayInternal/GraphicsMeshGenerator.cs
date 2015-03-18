namespace pumpkin.displayInternal
{
    using pumpkin.display;
    using pumpkin.swf;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// The graphics draw mesh generator class.
    /// </summary>
    public class GraphicsMeshGenerator : IGraphicsGenerator
    {
        private int ColourId = 0;
        private Material currentMaterial;
        private Vector2 lowerLeftUV;
        private List<Material> materialList;
        private Material[] sharedMaterials;
        private Mesh mesh;
        private Vector3[] normals = new Vector3[0];
        private int numIndex = 0;
        private int numVerts = 0;
        private int phaseId = 0;
        private int quadCount;
        private bool sameUpdateCounter = false;
        private bool simpleGeneration = true;
        private int subMeshId = 0;
        private List<Submesh> submeshIndices;
        private Vector2 tmpUv;
        private int triId = 0;
        private int updateCounter = -2;
        private Vector2 UVDimensions;
        private int UVId = 0;
        private Vector2[] UVs;
        private Vector3[] vertices;
        private Color[] colors;
        private int VertId = 0;
        private Color white = Color.white;
        private float zContainerSpace = 0f;
        private float zDrawOffset = 0f;
        private float zSpace = 0.0001f;

        /// <summary>
        /// 
        /// </summary>
        public GraphicsMeshGenerator()
        {
            mesh = newMesh();
            submeshIndices = new List<Submesh>();
            materialList = new List<Material>();
            normals = new Vector3[0];
            colors = new Color[0];
            UVs = new Vector2[0];
            vertices = new Vector3[0];
            sharedMaterials = new Material[0];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Mesh newMesh()
        {
            Mesh mesh = new Mesh();
            mesh.name = "MovieClip Mesh";
            mesh.hideFlags = HideFlags.HideAndDontSave;
            mesh.MarkDynamic();
            return mesh;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="meshRenderer"></param>
        /// <returns></returns>
        public Mesh applyToMeshRenderer(MeshRenderer meshRenderer)
        {
            mesh.vertices = vertices;
            mesh.uv = UVs;
            mesh.colors = colors;

            // Set materials.
            if (materialList.Count == sharedMaterials.Length)
                materialList.CopyTo(sharedMaterials);
            else
                sharedMaterials = materialList.ToArray();
            meshRenderer.sharedMaterials = sharedMaterials;
            int submeshCount = materialList.Count;

            mesh.subMeshCount = submeshCount;
            for (int i = 0; i < submeshCount; i++)
            {
                int[] triangles = submeshIndices[i].triangles;
                mesh.SetTriangles(triangles, i);
            }
            mesh.RecalculateBounds();
            return mesh;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="pcolor"></param>
        /// <param name="movieClip"></param>
        private void renderDisplayObjectContainer(DisplayObjectContainer parent, Color pcolor, bool movieClip)
        {
            if (phaseId == 0)
            {
                if (parent.updateCounter != updateCounter)
                {
                    sameUpdateCounter = false;
                }
            }
            else if (phaseId == 1)
            {
                parent.updateCounter = Time.frameCount;
            }

            for (int i = 0; i < parent.numChildren; i++)
            {
                if (movieClip)
                {
                    MovieClip obj2 = parent.getChildAt(i) as MovieClip;

                    if (obj2.visible)
                    {
                        BitmapAssetInfo info = obj2.bmpInfo;
                        renderGraphics(info, obj2, obj2.colorTransform);
                        renderDisplayObjectContainer(obj2, obj2.colorTransform, false);
                    }
                }
                else
                {
                    DisplayObjectContainer obj2 = parent.getChildAt(i) as DisplayObjectContainer;

                    BitmapAssetInfo info = obj2.bmpInfo;
                    renderGraphics(info, obj2, pcolor);
                    renderDisplayObjectContainer(obj2, pcolor, false);

                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="parent"></param>
        /// <param name="pcolor"></param>
        private void renderGraphics(BitmapAssetInfo info, DisplayObjectContainer parent, Color pcolor)
        {
            Submesh submesh = null;
            if (info != null)
            {
                Material material = info.material;
                if (material != null)
                {
                    if (phaseId == 0)
                    {
                        if (currentMaterial != material)
                        {
                            if (currentMaterial != null)
                            {
                                AppSubMesh();
                                triId = 0;
                                numIndex = 0;
                            }
                            currentMaterial = material;
                        }

                        numVerts += 4;
                        numIndex += 6;
                        quadCount++;
                        zDrawOffset += zSpace;
                    }
                    else if (phaseId == 1)
                    {
                        if (currentMaterial != material)
                        {
                            subMeshId++;
                            currentMaterial = material;
                            triId = 0;
                            submesh = submeshIndices[subMeshId];
                        }

                        float x = 0f;
                        float y = 0f;
                        float drawWidth = info.srcRect.width;
                        float drawHeight = info.srcRect.height;
                        Rect drawSrcRect = info.uvRect;
                        float zDrawOffset = this.zDrawOffset;
                        Matrix matrix = parent._fastGetFullMatrixRef();
                        lowerLeftUV.x = drawSrcRect.x;
                        lowerLeftUV.y = 1f - drawSrcRect.y;
                        UVDimensions.x = drawSrcRect.width;
                        UVDimensions.y = drawSrcRect.height;
                        tmpUv.x = lowerLeftUV.x + UVDimensions.x;
                        tmpUv.y = lowerLeftUV.y;
                        UVs[UVId++] = tmpUv;
                        tmpUv.x = lowerLeftUV.x + UVDimensions.x;
                        tmpUv.y = lowerLeftUV.y - UVDimensions.y;
                        UVs[UVId++] = tmpUv;
                        tmpUv.x = lowerLeftUV.x;
                        tmpUv.y = lowerLeftUV.y - UVDimensions.y;
                        UVs[UVId++] = tmpUv;
                        tmpUv.x = lowerLeftUV.x;
                        tmpUv.y = lowerLeftUV.y;
                        UVs[UVId++] = tmpUv;
                        if (!simpleGeneration)
                        {
                            colors[ColourId++] = white;
                            colors[ColourId++] = white;
                            colors[ColourId++] = white;
                            colors[ColourId++] = white;
                        }
                        else
                        {
                            float alpha = parent.getInheritedAlpha();
                            Color color = pcolor;
                            if (alpha <= 0.95f || alpha >= 0.05f)
                            {
                                Color tem = Color.white;
                                tem.a = alpha;
                                color = pcolor * tem;
                            }

                            colors[ColourId++] = color;
                            colors[ColourId++] = color;
                            colors[ColourId++] = color;
                            colors[ColourId++] = color;
                        }

                        int vertId = VertId;
                        vertices[VertId++] = matrix.transformVector3Static(x + drawWidth, y, zDrawOffset);
                        vertices[VertId++] = matrix.transformVector3Static(x + drawWidth, y + drawHeight, zDrawOffset);
                        vertices[VertId++] = matrix.transformVector3Static(x, y + drawHeight, zDrawOffset);
                        vertices[VertId++] = matrix.transformVector3Static(x, y, zDrawOffset);

                        if (submesh == null)
                        {
                            submesh = submeshIndices[subMeshId];
                        }
                        int[] triangles = submesh.triangles;
                        int num11 = quadCount * 4;

                        triangles[triId++] = num11;
                        triangles[triId++] = num11 + 1;
                        triangles[triId++] = num11 + 3;
                        triangles[triId++] = num11 + 3;
                        triangles[triId++] = num11 + 1;
                        triangles[triId++] = num11 + 2;
                        quadCount++;
                        zDrawOffset += zSpace;
                    }
                }

            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void AppSubMesh()
        {
            int submeshIndex = materialList.Count;
            materialList.Add(currentMaterial);
            if (submeshIndices.Count <= submeshIndex)
                submeshIndices.Add(new Submesh());

            Submesh submesh = submeshIndices[submeshIndex];
            int[] triangles = submesh.triangles;
            int triangleCount = numIndex;
            int trianglesCapacity = triangles.Length;
            if (trianglesCapacity > triangleCount)
            {
                // Last submesh may have more triangles than required, so zero triangles to the end.
                for (int k = triangleCount; k < trianglesCapacity; k++)
                    triangles[k] = 0;
            }
            else if (trianglesCapacity != triangleCount)
            {
                // Reallocate triangles when not the exact size needed.
                submesh.triangles = new int[triangleCount];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stage"></param>
        /// <param name="renderQueue"></param>
        /// <returns></returns>
        public bool renderStage(DisplayObjectContainer stage, int renderQueue)
        {
            materialList.Clear();
            currentMaterial = null;
            subMeshId = -1;
            quadCount = 0;
            zDrawOffset = 0f;
            phaseId = 0;
            sameUpdateCounter = true;
            numVerts = 0;
            numIndex = 0;

            renderDisplayObjectContainer(stage, white, true);
            if (sameUpdateCounter)
            {
                return false;
            }

            AppSubMesh();

            bool newTriangles = numVerts > vertices.Length;
            if (newTriangles)
            {
                // Not enough vertices, increase size.
                vertices = vertices = new Vector3[numVerts];
                colors = new Color[numVerts];
                UVs = new Vector2[numVerts];
                mesh.Clear();
            }
            else
            {
                // Too many vertices, zero the extra.
                Vector3 zero = Vector3.zero;
                for (int i = numVerts; i < vertices.Length; i++)
                    vertices[i] = zero;
            }

            currentMaterial = null;
            subMeshId = -1;
            quadCount = 0;
            zDrawOffset = 0f;
            phaseId = 1;
            VertId = 0;
            UVId = 0;
            ColourId = 0;
            triId = 0;
            renderDisplayObjectContainer(stage, white, true);
            updateCounter = Time.frameCount;
            return true;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    class Submesh
    {
        /// <summary>
        /// 
        /// </summary>
        public int[] triangles = new int[0];
    }

}