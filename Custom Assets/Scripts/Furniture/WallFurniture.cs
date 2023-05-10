using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Coordinate3D
{

public class WallFurniture : Furniture
{
    
    //////////////////////////////////////////////////////////////////////
    // types
    //////////////////////////////////////////////////////////////////////
    #region types
    
    #endregion

    //////////////////////////////////////////////////////////////////////
    // fields
    //////////////////////////////////////////////////////////////////////
    #region fields

    //-------------------------------------------------- Opponents

    //-------------------------------------------------- SerializeField

    //-------------------------------------------------- public fields
    public float wallFurnitureWidth, wallFurnitureHeight;

    public List<Vector3> shrinkPolygon = new List<Vector3>();

    //-------------------------------------------------- private fields
    Vector3 wallFurnitureSize;

    public int m_wallID;

    #endregion

    //////////////////////////////////////////////////////////////////////
    // properties
    //////////////////////////////////////////////////////////////////////
    #region properties

    //-------------------------------------------------- public properties
    public int wallID
    {
        get { return m_wallID; }
        set
        {
            m_wallID = value;
        
            SetShrinkPolygon(room_Cp.wallsVertices[value]);
        }
    }

    //-------------------------------------------------- private properties
    GameObject furnitureRotPanel_Pf
    {
        get { return furnitureManager_Cp.wallFurnitureRotPanel_Pf; }
    }

    float maxSizeSide
    {
        get
        {
            return Mathf.Pow(Mathf.Pow(wallFurnitureWidth, 2f)
                + Mathf.Pow(wallFurnitureHeight, 2f), 0.5f);
        }
    }

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
    public new void Init()
    {
        InitOpponents();

        InitCollider(); // This must be precedd InstantWallFurnitureRotPanel

        InstantWallFurnitureRotPanel();
        
        SetEnableOutlines(false);

        SetEnableRotPanelRenderer(false);

        gameState = GameState_En.Inited;
    }

    //-------------------------------------------------- 
    new void InitOpponents()
    {
        base.InitOpponents();
    }

    //--------------------------------------------------
    public void SetWallFurnitureSize(Vector3 size_pr)
    {
        wallFurnitureSize = size_pr;
        wallFurnitureWidth = size_pr.x;
        wallFurnitureHeight = size_pr.z;
    }

    #endregion

    //--------------------------------------------------
    void InstantWallFurnitureRotPanel()
    {
        // 
        Vector3 size_tp = FurnitureManager.GetSizeOfGameObject(furnitureRotPanel_Pf);
        float rotPanelRadius = size_tp.x;
        float scale = maxSizeSide / rotPanelRadius;

        // 
        furnitureRotPanel_GO = Instantiate(furnitureRotPanel_Pf, transform);
        furnitureRotPanel_GO.transform.localScale *= scale;

        // 
        foreach(Collider coll_Cp_tp in furnitureRotPanel_GO.GetComponentsInChildren<Collider>())
        {
            rotPanelCollider_Cps.Add(coll_Cp_tp);
        }

        foreach(Renderer rend_Cp_tp in furnitureRotPanel_GO.GetComponentsInChildren<Renderer>())
        {
            rotPanelRend_Cps.Add(rend_Cp_tp);
        }
    }

    //--------------------------------------------------
    void SetShrinkPolygon(List<Vector3> originalVertices_pr)
    {
        float shrinkAmount = maxSizeSide / 2f;
        shrinkPolygon.Clear();
        
        shrinkPolygon = room_Cp.GetInnerPolygon(originalVertices_pr, wallFurnitureWidth / 2f,
            wallFurnitureHeight / 2f);
    }

    //--------------------------------------------------
    void OnDrawGizmos()
    {
        Gizmos.color= Color.red;
        for(int i = 0; i < shrinkPolygon.Count; i++)
        {
            Gizmos.DrawSphere(shrinkPolygon[i], 0.2f);
        }

        Gizmos.color= Color.blue;
        for(int i = 0; i < shrinkPolygon.Count; i++)
        {
            int j = (i + 1) % shrinkPolygon.Count;
            Gizmos.DrawLine(shrinkPolygon[i], shrinkPolygon[j]);
        }
    }

}

}