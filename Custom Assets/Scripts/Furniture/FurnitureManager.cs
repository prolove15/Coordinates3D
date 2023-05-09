using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Coordinate3D
{

public class FurnitureManager : MonoBehaviour
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

    EazyCamera.EazyController eazyController_Cp;

    Room room_Cp;

    //-------------------------------------------------- SerializeField
    [SerializeField]
    public GameObject furnitureRotPanel_Pf;

    [SerializeField]
    public GameObject wallFurnitureRotPanel_Pf;

    [SerializeField]
    public float furnitureRotCoef = 10f;

    [SerializeField]
    public List<GameObject> furniture_Pfs = new List<GameObject>();

    [SerializeField]
    public List<GameObject> wallFurniture_Pfs = new List<GameObject>();

    //-------------------------------------------------- public fields
    public GameState_En gameState;

    public List<Furniture> furniture_Cps = new List<Furniture>();

    public List<WallFurniture> wallFurniture_Cps = new List<WallFurniture>();

    public Furniture holdedFurniture_Cp;

    public Furniture focusedFurniture_Cp;

    public FurnitureControlPoint holdedFurnitureCP_Cp;

    //-------------------------------------------------- private fields
    bool m_lockCamRot;

    bool m_isHoldingFurniture;

    Vector3 holdOffset;

    bool outedHolding;

    #endregion

    //////////////////////////////////////////////////////////////////////
    // properties
    //////////////////////////////////////////////////////////////////////
    #region properties

    //-------------------------------------------------- public properties
    public bool lockCamRot
    {
        get { return m_lockCamRot; }
        set
        {
            m_lockCamRot = value;

            SetLockCamRot();
        }
    }

    public Transform holdedFurniture_Tf
    {
        get { return holdedFurniture_Cp.transform; }
    }

    //-------------------------------------------------- private properties
    Transform room_Tf
    {
        get { return controller_Cp.room_Cp.room_Tf; }
    }

    List<Transform> wall_Tfs
    {
        get { return room_Cp.wall_Tfs; }
    }

    bool isHoldingFurniture
    {
        get
        {
            return m_isHoldingFurniture;
        }
        set
        {
            m_isHoldingFurniture = value;

            SetLockCamRot();
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
        if(gameState == GameState_En.Play)
        {
            DetectHoldedFurniture();

            DragFurniture();
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            InstantFurniture();
        }
        if(Input.GetKeyDown(KeyCode.A))
        {
            InstantWallFurniture();
        }
    }

    //////////////////////////////////////////////////////////////////////
    // Init
    //////////////////////////////////////////////////////////////////////
    #region Init

    //-------------------------------------------------- Init
    public void Init()
    {
        // 
        InitOpponents();

        // 
        ignoreLayer = ~(1 << LayerMask.NameToLayer("Ignore Raycast"));

        // 
        gameState = GameState_En.Inited;
    }

    //--------------------------------------------------
    void InitOpponents()
    {
        controller_Cp = GameObject.FindWithTag("GameController").GetComponent<Controller>();
        eazyController_Cp = controller_Cp.eazyController_Cp;
        room_Cp = controller_Cp.room_Cp;
    }

    #endregion

    //////////////////////////////////////////////////////////////////////
    // instant furniture
    //////////////////////////////////////////////////////////////////////
    #region instant furniture

    //--------------------------------------------------
    public void InstantFurniture(int furnitureIndex_pr)
    {
        GameObject furniture_Pf_pr = furniture_Pfs[furnitureIndex_pr];

        GameObject furniture_GO_tp = Instantiate(furniture_Pf_pr, room_Tf.position,
            furniture_Pf_pr.transform.rotation);

        Furniture furniture_Cp_tp = furniture_GO_tp.GetComponent<Furniture>();
        furniture_Cp_tp.Init();

        furniture_Cps.Add(furniture_Cp_tp);
    }

    //--------------------------------------------------
    public void InstantFurniture()
    {
        int furnitureIndex_tp = Random.Range(0, furniture_Pfs.Count);
        InstantFurniture(furnitureIndex_tp);
    }

    //--------------------------------------------------
    public void InstantWallFurniture(int index)
    {
        GameObject wallFurniture_Pf_pr = wallFurniture_Pfs[index];
        int wallID_tp = 0;

        // 
        Vector3 wallFurnitureSize_tp = FurnitureManager.GetSizeOfGameObject(wallFurniture_Pf_pr);
        GameObject wallFurniture_GO_tp = Instantiate(wallFurniture_Pf_pr, wall_Tfs[wallID_tp].position,
            wall_Tfs[wallID_tp].rotation);

        // 
        WallFurniture wallFurniture_Cp_tp = wallFurniture_GO_tp.GetComponent<WallFurniture>();

        wallFurniture_Cp_tp.SetWallFurnitureSize(wallFurnitureSize_tp);
        wallFurniture_Cp_tp.Init();
        wallFurniture_Cp_tp.wallID = wallID_tp;

        wallFurniture_Cps.Add(wallFurniture_Cp_tp);
        
        if(!room_Cp.wallEventHandlers[wallID_tp].isVisible)
        {
            wallFurniture_Cp_tp.gameObject.SetActive(false);
        }
    }

    //--------------------------------------------------
    public void InstantWallFurniture()
    {
        int index_tp = Random.Range(0, wallFurniture_Pfs.Count);
        InstantWallFurniture(index_tp);
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

    //--------------------------------------------------
    void DetectHoldedFurniture()
    {
        // 
        if(Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            if(focusedFurniture_Cp)
            {
                bool existFocusedFurniture_tp = false;

                RaycastHit rayCH_tp2;
                if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out rayCH_tp2))
                {
                    Transform rayCH_Tf_tp = rayCH_tp2.transform.root;
                    if(rayCH_Tf_tp.tag == "Furniture" || rayCH_Tf_tp.tag == "WallFurniture")
                    {
                        if(rayCH_Tf_tp.GetComponent<Furniture>() == focusedFurniture_Cp)
                        {
                            existFocusedFurniture_tp = true;
                        }
                    }
                }

                if(!existFocusedFurniture_tp)
                {
                    focusedFurniture_Cp.focused = false;
                    focusedFurniture_Cp = null;
                }
            }
        }

        if(!Input.GetMouseButton(0) && !Input.GetMouseButton(1))
        {
            if(isHoldingFurniture)
            {
                holdedFurniture_Cp.dragging = false;
                holdedFurniture_Cp = null;
                isHoldingFurniture = false;
            }
            return;
        }

        if(isHoldingFurniture)
        {
            return;
        }

        if(lockCamRot)
        {
            return;
        }

        // 
        RaycastHit rayCH_tp;
        if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out rayCH_tp))
        {
            if(rayCH_tp.transform.tag == "FurnitureCP")
            {
                return;
            }
            
            Transform hitedTarget_tp = rayCH_tp.transform.root;
            if(hitedTarget_tp.tag == "Furniture" || hitedTarget_tp.tag == "WallFurniture")
            {
                holdedFurniture_Cp = hitedTarget_tp.GetComponent<Furniture>();
                holdedFurniture_Cp.dragging = true;

                holdOffset = rayCH_tp.point - hitedTarget_tp.position;
                isHoldingFurniture = true;

                focusedFurniture_Cp = holdedFurniture_Cp;
                focusedFurniture_Cp.focused = true;
            }
        }
    }

    //--------------------------------------------------
    int ignoreLayer;
    void DragFurniture()
    {
        if(!isHoldingFurniture)
        {
            return;
        }

        // 
        RaycastHit rayCH;
        if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out rayCH,
            Mathf.Infinity, ignoreLayer))
        {
            Vector3 targetPos = rayCH.point - holdOffset;

            // 
            List<Vector3> vertices_tp = holdedFurniture_Cp.shrinkRoom;
            int wallIndex_tp = -1;
            if(holdedFurniture_Cp.gameObject.tag == "WallFurniture")
            {
                if(rayCH.transform.tag == "Wall")
                {
                    for(int i = 0; i < room_Cp.wall_Tfs.Count; i++)
                    {
                        if(rayCH.transform == room_Cp.wall_Tfs[i])
                        {
                            wallIndex_tp = i;
                            break;
                        }
                    }

                    holdedFurniture_Cp.wallFurniture_Cp.wallID = wallIndex_tp;
                    vertices_tp = holdedFurniture_Cp.wallFurniture_Cp.shrinkPolygon;
                    holdedFurniture_Tf.rotation = room_Cp.wall_Tfs[wallIndex_tp].rotation;
                }
                else if(rayCH.transform.root.tag == "WallFurniture")
                {
                    wallIndex_tp = rayCH.transform.root.GetComponent<WallFurniture>().wallID;

                    holdedFurniture_Cp.wallFurniture_Cp.wallID = wallIndex_tp;
                    vertices_tp = holdedFurniture_Cp.wallFurniture_Cp.shrinkPolygon;
                    holdedFurniture_Tf.rotation = room_Cp.wall_Tfs[wallIndex_tp].rotation;
                }
                else
                {
                    return;
                }
            }

            // 
            if(!room_Cp.IsInsideOfPolygon(targetPos, vertices_tp))
            {
                targetPos = room_Cp.FindClosestPoint(targetPos, vertices_tp);
            }

            if(holdedFurniture_Cp.gameObject.tag == "Furniture")
            {
                targetPos.y = room_Tf.position.y;
            }
            else if(holdedFurniture_Cp.gameObject.tag == "WallFurniture")
            {
                Transform wall_Tf_tp = room_Cp.wall_Tfs[wallIndex_tp];
                targetPos = Vector3.ProjectOnPlane(targetPos - wall_Tf_tp.position,
                    wall_Tf_tp.up) + wall_Tf_tp.position;
            }

            holdedFurniture_Cp.Move(targetPos);
        }
    }

    //--------------------------------------------------
    void SetLockCamRot()
    {
        if(lockCamRot || isHoldingFurniture)
        {
            eazyController_Cp.lockCamRot = true;
        }
        else
        {
            eazyController_Cp.lockCamRot = false;
        }
    }

    //--------------------------------------------------
    public void SetWallFurnitureActiveState(int index, bool state)
    {
        for(int i = 0; i < wallFurniture_Cps.Count; i++)
        {
            if(wallFurniture_Cps[i].wallID == index)
            {
                wallFurniture_Cps[i].gameObject.SetActive(state);
                break;
            }
        }
    }

    //--------------------------------------------------
    public static Vector3 GetSizeOfGameObject(GameObject object_pr)
    {
        Vector3 size_tp = new Vector3();

        foreach(Renderer rend_Cp_tp in object_pr.GetComponentsInChildren<Renderer>())
        {
            Vector3 furnitureSize = rend_Cp_tp.bounds.size;
            size_tp.x = size_tp.x < furnitureSize.x ? furnitureSize.x : size_tp.x;
            size_tp.y = size_tp.y < furnitureSize.y ? furnitureSize.y : size_tp.y;
            size_tp.z = size_tp.z < furnitureSize.z ? furnitureSize.z : size_tp.z;
        }

        return size_tp;
    }

}

}