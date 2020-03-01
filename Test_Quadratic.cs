using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Quadratic : MonoBehaviour
{
    /*
    Vector3[] vert = new Vector3[40];
    Vector2[] uvs = new Vector2[40];
    int[] triag = new int[108];
    float M;
    float A;
    float B;
    Mesh m;
    // Start is called before the first frame update
    void Start()
    {
        m = new Mesh();
    }
    private Vector3 CalculateQuad(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        return ((1 - t) * (1 - t) * p0) + (2 * (1 - t) * t * p1) + ((t * t) * p2);
    }
    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y,0 );

        m.Clear();
        Ray r = new Ray(Instant_Reference.playerCamStraightRay.origin, 
            new Vector3(Instant_Reference.playerCamStraightRay.direction.x, 0, Instant_Reference.playerCamStraightRay.direction.z));
        M = Instant_Reference.playerCamStraightRay.direction.y;
        A = M != .5? Mathf.Abs(1 / (.5f - M)) : 20;
        B = M != .5 ? Mathf.Abs(1 / (.5f - M)) : 20;
        print(M);
      //  (-(A) * (S - (M / A)) * (S - (M / A))) + (M * M / A) + B
            for (int i = 0; i < 20; i++)
            {
                 float S = (float)i/7;
                 float T = (float)i / 20;
            Vector3 P = CalculateQuad(T, r.origin, new Vector3(r.GetPoint(A / 2).x, B, r.GetPoint(A / 2).z), new Vector3(r.GetPoint(A).x, 0, r.GetPoint(A).z));
                vert[i * 2] = new Vector3(P.x, P.y, P.z + 0.1f);
            
                vert[(i * 2) + 1] = new Vector3(P.x, P.y, P.z - 0.1f);
            uvs[i * 2] = new Vector2(vert[i * 2].x, vert[i * 2].z);
                uvs[(i * 2) + 1] = new Vector2(vert[(i * 2) + 1].x, vert[(i * 2) + 1].z);

            }

            for (int t = 0; t < 20; t++)
            {
                if (t * 2 < 36)
                {
                    triag[t * 6] = t * 2;
                    triag[(t * 6) + 1] = (t * 2) + 2;
                    triag[(t * 6) + 2] = triag[(t * 6) + 1] - 1;
                    triag[(t * 6) + 3] = triag[(t * 6) + 2];
                    triag[(t * 6) + 4] = triag[(t * 6) + 3] + 1;
                    triag[(t * 6) + 5] = triag[(t * 6) + 4] + 1;
                }
            }

            m.vertices = vert;
            m.triangles = triag;
          //  m.uv = uvs;



            GetComponent<MeshFilter>().mesh = m;
        
        m.RecalculateNormals();
        m.RecalculateBounds();
        m.RecalculateTangents();
    }*/
}
