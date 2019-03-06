using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace Slam
{
    public static class MeshLoadHandler 
    {
 
        #region PointObject

        public static Mesh HandleVertexShapeNode(IndexedFaceSet indexedFaceSet)
        {
            StartMeshParsing();
            Mesh mesh = new Mesh();

            try
            {
                mesh.vertices = getCoordinates(indexedFaceSet.Coordinate);
                mesh.triangles = getCoordIndex(indexedFaceSet);
                if (indexedFaceSet.Normal != null)
                {
                    mesh.normals = getNormals(indexedFaceSet.Normal);
                }
                // mesh.SetIndices()

                // Vector3[] x3dMaterial = getX3DMatValues(ref xpni);
                Vector2[][] maps = getUVMaps(indexedFaceSet);
                if (maps[0].Length > 0)
                {
                    mesh.uv = maps[0];
                }
                if (maps[1] != null && maps[1].Length > 0) mesh.uv2 = maps[1];
                if (maps[2] != null && maps[2].Length > 0) mesh.uv3 = maps[2];

            }
            catch (Exception x)
            {
                var a = x.Message;
                Debug.LogWarning(a);
            }
            finally
            {
                FinishMeshParsing();
            }
            return mesh;
        }
        #region Internal

        private static bool meshParsingRunning = false;
        private static System.Object meshParsingLock = new System.Object();
        private static void StartMeshParsing()
        {
            lock (meshParsingLock)
            {
                if (meshParsingRunning)
                {
                    throw new Exception("MeshParsing is already running. You can not call these APIs from multiple threads.");
                }
                meshParsingRunning = true;
            }
        }
        private static void FinishMeshParsing()
        {
            meshParsingRunning = false;
        }
        #endregion
        static char[] fChop = { ' ', '\t', ',', '\n' };
        //        private char[] cChop = { '\'', ' ' };
        //        private char[] urlChop = { ' ', '\t', '\'', '\"' };

        static Vector2[][] getUVMaps(IndexedFaceSet indexedFaceCet)
        {
            string[] textureCoords = { "", "", "" };
            var n = 0;
            if (indexedFaceCet.MultiTextureCoordinate.Count > 0)
                foreach (var texturecord in indexedFaceCet.MultiTextureCoordinate)
                {
                    if (n < 3)
                    {
                        textureCoords[n] = texturecord.Point;
                        n = n + 1;
                    }
                }
            else if (indexedFaceCet.TextureCoordinate != null && !string.IsNullOrEmpty(indexedFaceCet.TextureCoordinate.Point))
            {
                textureCoords[0] = indexedFaceCet.TextureCoordinate.Point;
            }

            Vector2[][] maps = new Vector2[3][];
            if (indexedFaceCet.TextCoordIndex == null)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (textureCoords[i].Length > 0)
                    {
                        string[] vecs = textureCoords[i].Split(fChop, StringSplitOptions.RemoveEmptyEntries);
                        maps[i] = new Vector2[(int)Math.Floor((double)vecs.Length / 2.0)];
                        int j = 0;
                        int m = 0;
                        while (j < vecs.Length && m < maps[i].Length)
                        {
                            int k = j + 1;
                            maps[i][m].x = (float)Double.Parse(vecs[j]);
                            maps[i][m].y = (float)Double.Parse(vecs[k]);
                            j = j + 2;
                            m = m + 1;
                        }
                    }
                    else maps[i] = new Vector2[0];
                }
            }
            else
            {
                //               string[] vecs = indexedFaceCet.TextCoordIndex.Split(fChop, StringSplitOptions.RemoveEmptyEntries);
                int[] textIndices = ToIntArray(indexedFaceCet.TextCoordIndex);
                //int[] faces = ToIntArray(indexedFaceCet.CoordIndex);
                float[] textureCoordFloat = ToFloatArray(indexedFaceCet.TextureCoordinate.Point);
                maps[0] = new Vector2[(int)Math.Floor((double)textureCoordFloat.Length / 2.0)];
                int j = 0;
                int m = 0;
                while (j < textureCoordFloat.Length && m < maps[0].Length)
                {
                    int k = j + 1;
                    maps[0][m].x = textureCoordFloat[textIndices[j]];
                    maps[0][m].y = textureCoordFloat[textIndices[k]];
                    j = j + 2;
                    m = m + 1;

                }
            }
            return maps;
        }

        static float[] ToFloatArray(string intString)
        {
            if (string.IsNullOrEmpty(intString)) { return new float[0]; }
            string[] vecs = intString.Split(fChop, StringSplitOptions.RemoveEmptyEntries);
            float[] floatArray = new float[vecs.Length];
            int count = 0;
            for (int nn = 0; nn < vecs.Length; nn++)
            {
                var v = float.Parse(vecs[nn]);
                //if (v != -1)
                //{
                floatArray[nn] = v;
                count++;
                //}
            }
            //float[] newArray = new float[count];
            //for (int nn = 0; nn < count; nn++)
            //{
            //    newArray[nn] = floatArray[nn];
            //}
            return floatArray;
        }

        static int[] ToIntArray(string intString)
        {
            if (string.IsNullOrEmpty(intString)) { return new int[0]; }
            string[] vecs = intString.Split(fChop, StringSplitOptions.RemoveEmptyEntries);
            int[] intArray = new int[vecs.Length];
            int count = 0;
            for (int nn = 0; nn < vecs.Length; nn++)
            {
                var v = int.Parse(vecs[nn]);
                if (v != -1)
                {
                    intArray[nn] = v;
                    count++;
                }
            }
            int[] newArray = new int[count];
            for (int nn = 0; nn < count; nn++)
            {
                newArray[nn] = intArray[nn];
            }
            return newArray;
        }

        static float[] convert(int[] texIndices, int[] faces, float[] textureCoords)
        {
            float[] resultArray = new float[textureCoords.Length];

            for (int count = 0; count < texIndices.Length; count++)
            {

                int index = texIndices[count];

                float u = textureCoords[index * 2];

                float v = textureCoords[index * 2 + 1];

                int index2 = faces[count];

                resultArray[index2 * 2] = u;

                resultArray[index2 * 2 + 1] = v;

            }

            return resultArray;

        }

        static Vector3[] getNormals(Normal normal)
        {
            string vectors = normal.Vector;

            string[] vecs = vectors.Split(fChop, StringSplitOptions.RemoveEmptyEntries);
            Vector3[] v = new Vector3[(int)Math.Floor((double)vecs.Length / 3.0)];

            int j = 0;
            int m = 0;
            while (j < vecs.Length && m < v.Length)
            {
                int k = j + 1;
                int l = j + 2;
                v[m].x = (float)Double.Parse(vecs[j]);
                v[m].y = (float)Double.Parse(vecs[k]);
                v[m].z = (float)Double.Parse(vecs[l]) * -1; //X3D has a right handed coordinate system

                j = j + 3;
                m = m + 1;
            }
            return v;
        }

        static Vector3[] getCoordinates(Coordinate coordinate)
        {
            if(coordinate==null)
            {
                return new Vector3[0];
            }
            string pts = coordinate.Point;

            string[] ptss = pts.Split(fChop, StringSplitOptions.RemoveEmptyEntries);
            Vector3[] v = new Vector3[(int)Math.Floor((double)ptss.Length / 3.0)];

            int j = 0;
            int m = 0;
            while (j < ptss.Length && m < v.Length)
            {
                int k = j + 1;
                int l = j + 2;
                v[m].x = (float)Double.Parse(ptss[j]);
                v[m].y = (float)Double.Parse(ptss[k]);
                v[m].z = (float)Double.Parse(ptss[l]) * -1; //X3D has a right handed coordinate system

                j = j + 3;
                m = m + 1;
            }
            return v;
        }

        static int[] getCoordIndex(IndexedFaceSet indexedFaceSet)
        {
            string cistr = indexedFaceSet.CoordIndex;
            if (!string.IsNullOrEmpty(indexedFaceSet.Index))
            {
                cistr = indexedFaceSet.Index;
            }
            string[] ci = cistr.Split(fChop, StringSplitOptions.RemoveEmptyEntries);
            int j = 0;
            for (int i = 0; i < ci.Length; i++) if (!ci[i].Equals("-1")) j = j + 1;

            int[] coordIndex = new int[j];
            j = 0;
            for (int i = 0; i < ci.Length; i++)
            {
                if (!ci[i].Equals("-1"))
                {
                    coordIndex[j] = Int32.Parse(ci[i]);
                    j = j + 1;
                }
            }

            j = 0;
            while (j < coordIndex.Length)
            {
                int t = coordIndex[j];
                int k = j + 2;
                coordIndex[j] = coordIndex[k];
                coordIndex[k] = t;
                j = j + 3;
            }
            return coordIndex;
        }

        #endregion PointObject

    }


}