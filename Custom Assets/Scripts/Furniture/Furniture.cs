using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using DG.Tweening;
using cakeslice;

namespace Coordinate3D
{

public class Furniture : MonoBehaviour
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

    //-------------------------------------------------- opponents
    [HideInInspector]
    public Controller controller_Cp;

    [HideInInspector]
    public FurnitureManager furnitureManager_Cp;

    [HideInInspector]
    public Outline[] outline_Cps;

    [HideInInspector]
    protected Room room_Cp;

    //-------------------------------------------------- SerializeField
    
    //-------------------------------------------------- public fields
    public GameState_En gameState;

    public List<Vector3> shrinkRoom = new List<Vector3>();

    //-------------------------------------------------- private fields
    protected GameObject furnitureRotPanel_GO;

    float furnitureWidth, furnitureLength, furnitureRange;

    bool m_dragging;

    bool m_focused;

    Vector3 rememberPos;

    Quaternion rememberRot;

    List<Renderer> renderer_Cps = new List<Renderer>();

    protected List<Collider> rotPanelCollider_Cps = new List<Collider>();

    protected List<Renderer> rotPanelRend_Cps = new List<Renderer>();

    public int furnitureTriggeredNum;

    #endregion

    //////////////////////////////////////////////////////////////////////
    // properties
    //////////////////////////////////////////////////////////////////////
    #region properties

    //-------------------------------------------------- public properties
    public bool lockCamRot
    {
        set { furnitureManager_Cp.lockCamRot = value; }
    }

    public WallFurniture wallFurniture_Cp
    {
        get { return gameObject.GetComponent<WallFurniture>(); }
    }

    public bool dragging
    {
        get { return m_dragging; }
        set
        {
            m_dragging = value;

            if(value)
            {
                RememberCurrentState();
            }
            else
            {
                if(!isAvailableMove)
                {
                    UndoState();
                }
            }

            // SetEnableRotPanelCollider(!value);
        }
    }

    public bool focused
    {
        get { return m_focused; }
        set
        {
            m_focused = value;

            SetEnableOutlines(value);

            SetEnableRotPanelRenderer(value);
        }
    }

    //-------------------------------------------------- private properties
    GameObject furnitureRotPanel_Pf
    {
        get { return furnitureManager_Cp.furnitureRotPanel_Pf; }
    }

    bool isAvailableMove
    {
        get { return furnitureTriggeredNum == 0 ? true : false; }
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
    public void Init()
    {
        InitOpponents();

        InitFurnitureSize();

        InitCollider(); // This must be precedd InitFurnitureRotPanel

        InitFurnitureRotPanel();

        SetShrinkRoom(room_Cp.wallCoordinates);

        focused = false;
        
        // 
        gameState = GameState_En.Inited;
    }

    //--------------------------------------------------
    protected void InitOpponents()
    {
        controller_Cp = GameObject.FindWithTag("GameController").GetComponent<Controller>();
        
        furnitureManager_Cp = controller_Cp.furnitureManager_Cp;

        foreach(Renderer rend_Cp_tp in GetComponentsInChildren<Renderer>())
        {
            rend_Cp_tp.gameObject.AddComponent<Outline>();
        }
        outline_Cps = GetComponentsInChildren<Outline>();

        room_Cp = controller_Cp.room_Cp;
    }

    //--------------------------------------------------
    void InitFurnitureSize()
    {
        Vector3 furnitureSize_tp = FurnitureManager.GetSizeOfGameObject(gameObject);

        furnitureWidth = furnitureSize_tp.x;
        furnitureLength = furnitureSize_tp.z;
        furnitureRange = Mathf.Pow((Mathf.Pow(furnitureWidth, 2f) + Mathf.Pow(furnitureLength, 2f)),
            0.5f) / 2f;
    }
    
    //--------------------------------------------------
    void InitFurnitureRotPanel()
    {
        furnitureRotPanel_GO = Instantiate(furnitureRotPanel_Pf, transform);

        Vector3 rotPanelSize_tp = FurnitureManager.GetSizeOfGameObject(furnitureRotPanel_Pf);

        furnitureRotPanel_GO.transform.localScale *= (furnitureRange / (rotPanelSize_tp.x / 2f));

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
    protected void InitCollider()
    {
        foreach(Renderer rend_Cp_tp in GetComponentsInChildren<Renderer>())
        {
            renderer_Cps.Add(rend_Cp_tp);

            MeshCollider collider_Cp_tp = rend_Cp_tp.gameObject.AddComponent<MeshCollider>();
            collider_Cp_tp.convex = true;
            collider_Cp_tp.isTrigger = true;

            // Rigidbody rigidB_Cp_tp = rend_Cp_tp.gameObject.AddComponent<Rigidbody>();
            // rigidB_Cp_tp.useGravity = false;
            // rigidB_Cp_tp.isKinematic = true;
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
        
    }

    #endregion

    //////////////////////////////////////////////////////////////////////
    // Move
    //////////////////////////////////////////////////////////////////////
    #region Move

    //--------------------------------------------------
    public void Move(Vector3 targetPos_pr)
    {
        Sequence moveSeq_tp = DOTween.Sequence();

        moveSeq_tp.Append(DOTween.To(() => transform.position, x => transform.position = x,
            targetPos_pr, 0.5f));

        moveSeq_tp.Play();
    }

    //--------------------------------------------------
    public void Rotate(Quaternion targetRot_pr)
    {
        transform.DORotateQuaternion(targetRot_pr, 0.5f);
    }

    #endregion

    //--------------------------------------------------
    public void SetEnableFurnitureRenderer(bool flag)
    {
        for(int i = 0; i < renderer_Cps.Count; i++)
        {
            renderer_Cps[i].enabled = flag;
        }
    }

    //--------------------------------------------------
    public void SetEnableOutlines(bool flag)
    {
        for(int i = 0; i < outline_Cps.Length; i++)
        {
            outline_Cps[i].enabled = flag;
        }
    }

    //--------------------------------------------------
    public void SetEnableRotPanelCollider(bool flag)
    {
        for(int i = 0; i < rotPanelCollider_Cps.Count; i++)
        {
            rotPanelCollider_Cps[i].enabled = flag;
        }
    }

    //--------------------------------------------------
    public void SetEnableRotPanelRenderer(bool flag)
    {
        for(int i = 0; i < rotPanelRend_Cps.Count; i++)
        {
            rotPanelRend_Cps[i].enabled = flag;
        }
    }

    //--------------------------------------------------
    public void SetActiveIgnoreRaycast(bool flag)
    {
        string layerName_tp = flag ? "Ignore Raycast" : "Default";
        foreach(Collider collider_Cp_tp in GetComponentsInChildren<Collider>())
        {
            collider_Cp_tp.gameObject.layer = LayerMask.NameToLayer(layerName_tp);
        }
    }

    //--------------------------------------------------
    void SetShrinkRoom(List<Vector3> originalVertices_pr)
    {
        float shrinkAmount = furnitureRange;
        shrinkRoom.Clear();
        
        shrinkRoom = room_Cp.GetInnerPolygon(originalVertices_pr, furnitureWidth / 2f,
            furnitureLength / 2f);
    }

    //////////////////////////////////////////////////////////////////////
    // manage state
    //////////////////////////////////////////////////////////////////////
    #region manage state
      
    //--------------------------------------------------
    void RememberCurrentState()
    {
        rememberPos = transform.position;
        rememberRot = transform.rotation;
    }

    //--------------------------------------------------
    void UndoState()
    {
        Move(rememberPos);
        Rotate(rememberRot);
    }

    #endregion

    //////////////////////////////////////////////////////////////////////
    // override
    //////////////////////////////////////////////////////////////////////
    #region override

    //--------------------------------------------------
    void OnTriggerEnter(Collider other)
    {
        Transform coll_Tf = other.transform;
        Transform root_Tf = coll_Tf.root;

        if(root_Tf == transform)
        {
            return;
        }

        if(transform.tag == "Furniture")
        {
            if(coll_Tf.tag == "Wall")
            {
                furnitureTriggeredNum++;
            }
        }
        else if(transform.tag == "WallFurniture")
        {
            if(coll_Tf.tag == "WallOuter" || root_Tf.tag == "WallFurniture")
            {
                furnitureTriggeredNum++;
            }
        }
    }

    //--------------------------------------------------
    void OnTriggerExit(Collider other)
    {
        Transform coll_Tf = other.transform;
        Transform root_Tf = coll_Tf.root;

        if(root_Tf == transform)
        {
            return;
        }

        if(transform.tag == "Furniture")
        {
            if(coll_Tf.tag == "Wall")
            {
                furnitureTriggeredNum--;
            }
        }
        else if(transform.tag == "WallFurniture")
        {
            if(coll_Tf.tag == "WallOuter" || root_Tf.tag == "WallFurniture")
            {
                furnitureTriggeredNum--;
            }
        }
    }

    //--------------------------------------------------
    void OnDrawGizmos()
    {
        for(int i = 0; i < shrinkRoom.Count; i++)
        {
            int j = (i + 1) % shrinkRoom.Count;
            Gizmos.color = Color.green;
            Gizmos.DrawLine(shrinkRoom[i], shrinkRoom[j]);

            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(shrinkRoom[i], 0.2f);
        }
    }

    #endregion

}

}