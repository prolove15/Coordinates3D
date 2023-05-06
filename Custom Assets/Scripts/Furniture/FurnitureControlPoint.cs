using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Coordinate3D
{

public class FurnitureControlPoint : MonoBehaviour
{
    
    //////////////////////////////////////////////////////////////////////
    // types
    //////////////////////////////////////////////////////////////////////
    #region types
    
    public enum GameState_En
    {
        Nothing, Inited
    }

    #endregion

    //////////////////////////////////////////////////////////////////////
    // fields
    //////////////////////////////////////////////////////////////////////
    #region fields

    //-------------------------------------------------- SerializeField

    //-------------------------------------------------- public fields
    public GameState_En gameState;

    //-------------------------------------------------- private fields
    bool m_lockCamRot;

    #endregion

    //////////////////////////////////////////////////////////////////////
    // properties
    //////////////////////////////////////////////////////////////////////
    #region properties

    //-------------------------------------------------- public properties

    //-------------------------------------------------- private properties
    Furniture furniture_Cp
    {
        get { return transform.root.gameObject.GetComponent<Furniture>(); }
    }

    public bool lockCamRot
    {
        get { return m_lockCamRot; }
        set
        {
            furniture_Cp.lockCamRot = value;
            m_lockCamRot = value;    
        }
    }

    float furnitureRotCoef
    {
        get { return furniture_Cp.furnitureManager_Cp.furnitureRotCoef; }
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
        if(lockCamRot)
        {
            RotateFurniture();
        }
    }
    
    //////////////////////////////////////////////////////////////////////
    // Init
    //////////////////////////////////////////////////////////////////////
    #region Init

    //-------------------------------------------------- Init
    public void Init()
    {

        gameState = GameState_En.Inited;
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
    // Rotate furniture
    //////////////////////////////////////////////////////////////////////
    #region Rotate furniture

    //--------------------------------------------------
    public void RotateFurniture()
    {
        float mouseXDelta = Input.GetAxis("Mouse X");

        furniture_Cp.transform.Rotate(Vector3.up * -mouseXDelta * furnitureRotCoef);
    }

    #endregion

    //////////////////////////////////////////////////////////////////////
    // override method
    //////////////////////////////////////////////////////////////////////
    #region override method

    //--------------------------------------------------
    void OnMouseUp()
    {
        // print("OnMouseUp");
        lockCamRot = false;
    }

    //--------------------------------------------------
    void OnMouseDown()
    {
        // print("OnMouseDown");
        lockCamRot = true;
    }

    #endregion

}

}