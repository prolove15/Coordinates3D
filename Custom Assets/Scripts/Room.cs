using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Coordinate3D
{

public class Room : MonoBehaviour
{
    
    //////////////////////////////////////////////////////////////////////
    // types
    //////////////////////////////////////////////////////////////////////
    #region types
    
    public enum GameState_En
    {
        Nothing, Inited, Play
    }

    #endregion

    //////////////////////////////////////////////////////////////////////
    // fields
    //////////////////////////////////////////////////////////////////////
    #region fields

    //-------------------------------------------------- Opponents
    Controller controller_Cp;

    FurnitureManager furnitureManager_Cp;

    //-------------------------------------------------- SerializeField
    [SerializeField]
    public List<Vector3> wallCoordinates = new List<Vector3>();

    [SerializeField]
    public Transform room_Tf;

    [SerializeField]
    Transform wallsHolder_Tf;

    //-------------------------------------------------- public fields
    public GameState_En gameState;

    public List<Transform> wall_Tfs = new List<Transform>();

    public List<List<Vector3>> wallsVertices = new List<List<Vector3>>();

    public float wallHeight = 5f;

    //-------------------------------------------------- private fields

    #endregion

    //////////////////////////////////////////////////////////////////////
    // properties
    //////////////////////////////////////////////////////////////////////
    #region properties

    //-------------------------------------------------- public properties

    //-------------------------------------------------- private properties

    #endregion

    //////////////////////////////////////////////////////////////////////
    // methods
    //////////////////////////////////////////////////////////////////////

    //-------------------------------------------------- Start is called before the first frame update
    void Start()
    {
        
    }

    //-------------------------------------------------- Update is called once per frame
    void Update()
    {
        
    }
    
    //////////////////////////////////////////////////////////////////////
    // Init
    //////////////////////////////////////////////////////////////////////
    #region Init

    //-------------------------------------------------- Init
    public void Init()
    {
        InitOpponents();

        SetWalls();

        InitRoomLayout();

        InitWallsVertices();

        gameState = GameState_En.Inited;
    }

    //--------------------------------------------------
    void InitOpponents()
    {
        controller_Cp = GameObject.FindWithTag("GameController").GetComponent<Controller>();

        furnitureManager_Cp = controller_Cp.furnitureManager_Cp;
    }

    //--------------------------------------------------
    void InitRoomLayout()
    {
        Vector3 wallSize_tp = FurnitureManager.GetSizeOfGameObject(wall_Tfs[0].gameObject);
        wallHeight = wallSize_tp.y;
    }

    //--------------------------------------------------
    void SetWalls()
    {
        for(int i = 0; i < wallsHolder_Tf.childCount; i++)
        {
            wall_Tfs.Add(wallsHolder_Tf.GetChild(i));
        }
    }

    //--------------------------------------------------
    public void InitWallsVertices()
    {
        // 
        wallsVertices.Clear();
        wallsVertices = new List<List<Vector3>>();

        // 
        for(int i = 0; i < wallCoordinates.Count; i++)
        {
            List<Vector3> wallVertices = new List<Vector3>();
            wallsVertices.Add(wallVertices);

            int nextWallIndex_tp = (i + wallCoordinates.Count - 1) % wallCoordinates.Count;
            wallVertices.Add(wallCoordinates[i]);
            wallVertices.Add(wallCoordinates[i] + new Vector3(0f, wallHeight, 0f));
            wallVertices.Add(wallCoordinates[nextWallIndex_tp] + new Vector3(0f, wallHeight, 0f));
            wallVertices.Add(wallCoordinates[nextWallIndex_tp]);
        }
    }

    #endregion

    //////////////////////////////////////////////////////////////////////
    // Play
    //////////////////////////////////////////////////////////////////////
    #region Play

    //--------------------------------------------------
    public void Play()
    {
        gameState = GameState_En.Play;
    }

    #endregion

    //////////////////////////////////////////////////////////////////////
    // modular method
    //////////////////////////////////////////////////////////////////////
    #region modular method

    //--------------------------------------------------
    public static bool LineLineIntersection(out Vector3 intersection,
        Vector3 linePoint1, Vector3 lineDirection1,
        Vector3 linePoint2, Vector3 lineDirection2)
    {
 
        Vector3 lineVec3 = linePoint2 - linePoint1;
        Vector3 crossVec1and2 = Vector3.Cross(lineDirection1, lineDirection2);
        Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineDirection2);
        float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);
 
        //is coplanar, and not parallel
        if (Mathf.Abs(planarFactor) < 0.0001f
                && crossVec1and2.sqrMagnitude > 0.0001f)
        {
            float s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
            intersection = linePoint1 + (lineDirection1 * s);
            return true;
        }
        else
        {
            intersection = Vector3.zero;
            return false;
        }
    }

    //--------------------------------------------------
    public List<Vector3> GetShrinkPolygon(List<Vector3> originalVertices_pr, float shrinkAmount)
    {
        List<Vector3> resultVertices = new List<Vector3>();

        Vector3 centroid = CalculateCentroid(originalVertices_pr);

        for (int i = 0; i < originalVertices_pr.Count; i++)
        {
            Vector3 vertex = originalVertices_pr[i];
            Vector3 direction = (vertex - centroid).normalized;
            Vector3 offset = direction * shrinkAmount;
            resultVertices.Insert(i, vertex - offset);
        }

        return resultVertices;
    }

    //--------------------------------------------------
    public List<Vector3> GetInnerPolygon(List<Vector3> originalVertices_pr,float reduceX_pr,
        float reduceZ_pr)
    {
        List<Vector3> resultVertices = new List<Vector3>(originalVertices_pr.ToArray());

        Vector3 centroid = CalculateCentroid(resultVertices);

        for (int i = 0; i < resultVertices.Count; i += 2)
        {
            int j = (i + 1) % resultVertices.Count;

            Vector3 direction = GetPerpendicularDirection(resultVertices[i],
                resultVertices[j], centroid).normalized;

            Vector3 offset = direction * reduceX_pr;

            resultVertices[i] += offset;
            resultVertices[j] += offset;
        }

        for (int i = 0; i < resultVertices.Count; i += 2)
        {
            int j = (i + resultVertices.Count - 1) % resultVertices.Count;

            Vector3 direction = GetPerpendicularDirection(resultVertices[i],
                resultVertices[j], centroid).normalized;

            Vector3 offset = direction * reduceZ_pr;
            
            resultVertices[i] += offset;
            resultVertices[j] += offset;
        }

        return resultVertices;
    } 

    //--------------------------------------------------
    public Vector3 GetPerpendicularDirection(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
    {
        // Calculate the direction of the line
        Vector3 lineDirection = lineEnd - lineStart;

        // Calculate the perpendicular direction to the line
        Vector3 projectPoint = Vector3.Project(point - lineStart, lineDirection) + lineStart;

        // Calculate the perpendicular direction on the line
        Vector3 perpendicularDir = point - projectPoint;
 
        return perpendicularDir;
    }

    //--------------------------------------------------
    public Vector3 CalculateCentroid(List<Vector3> vertices)
    {
        // Initialize variables to accumulate the sum of the x, y, and z coordinates
        float sumX = 0;
        float sumY = 0;
        float sumZ = 0;

        // Loop through all vertices and accumulate the x, y, and z coordinates
        for (int i = 0; i < vertices.Count; i++)
        {
            sumX += vertices[i].x;
            sumY += vertices[i].y;
            sumZ += vertices[i].z;
        }

        // Divide the accumulated x, y, and z coordinates by the number of vertices to get the center point
        int numVertices = vertices.Count;
        Vector3 center = new Vector3(sumX/numVertices, sumY/numVertices, sumZ/numVertices);

        return center;
    }

    //-------------------------------------------------- find closest point
    public Vector3 FindClosestPoint(Vector3 externalPoint, List<Vector3> vertices_pr)
    {
        // 
        Vector3 closestPoint = Vector3.zero;
        float shortestDistance = Mathf.Infinity;
        
        for (int i = 0; i < vertices_pr.Count; i++)
        {
            int j = (i + 1) % vertices_pr.Count;

            Vector3 v0 = vertices_pr[i];
            Vector3 v1 = vertices_pr[j];

            // 
            Vector3 pointOnLine = Vector3.Project(externalPoint - v0, v1 - v0) + v0;

            if(!(Mathf.Approximately(Vector3.Distance(v0, v1), Vector3.Distance(pointOnLine, v0)
                + Vector3.Distance(pointOnLine, v1) ) ) )
            {
                pointOnLine = Vector3.Distance(externalPoint, v0) < Vector3.Distance(externalPoint, v1) ?
                    v0 : v1;
            }

            // 
            float distance = Vector3.Distance(pointOnLine, externalPoint);

            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                closestPoint = pointOnLine;
            }
        }

        return closestPoint;
    }

    //--------------------------------------------------
    Vector3[] triangle_tp = new Vector3[3];
    public bool IsInsideOfPolygon(Vector3 point_pr, List<Vector3> vertices_pr)
    {
        // 
        bool isInside = false;

        // 
        for(int i = 0; i < vertices_pr.Count; i++)
        {
            int j = (i + 1) % vertices_pr.Count;
            int k = (i + 2) % vertices_pr.Count;
            triangle_tp[0] = vertices_pr[i];
            triangle_tp[1] = vertices_pr[j];
            triangle_tp[2] = vertices_pr[k];

            if(IsPointInsideTriangle(triangle_tp, point_pr))
            {
                isInside = true;
                break;
            }
        }

        return isInside;
    }

    //--------------------------------------------------
    public bool IsPointInsideTriangle(Vector3[] triangle, Vector3 point)
    {
        Vector3 v0 = triangle[2] - triangle[0];
        Vector3 v1 = triangle[1] - triangle[0];
        Vector3 v2 = point - triangle[0];

        float dot00 = Vector3.Dot(v0, v0);
        float dot01 = Vector3.Dot(v0, v1);
        float dot02 = Vector3.Dot(v0, v2);
        float dot11 = Vector3.Dot(v1, v1);
        float dot12 = Vector3.Dot(v1, v2);

        float invDenom = 1 / (dot00 * dot11 - dot01 * dot01);
        float u = (dot11 * dot02 - dot01 * dot12) * invDenom;
        float v = (dot00 * dot12 - dot01 * dot02) * invDenom;

        if ((u >= 0) && (v >= 0) && (u + v < 1))
        {
            return true;
        }

        return false;
    }

    #endregion

    //--------------------------------------------------
    void OnDrawGizmos()
    {
        
    }

}

}