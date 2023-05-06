using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Coordinate3D
{
    
public class Controller : MonoBehaviour
{
    
    //////////////////////////////////////////////////////////////////////
    // types
    //////////////////////////////////////////////////////////////////////
    #region types
    
    public enum GameState_En
    {
        Nothing, Inited
    }

    [System.Serializable]
    public struct Opponents_St
    {
        [Header("Background")]
        public BackgroundManager bgdManager_Cp;

        [Header("UI")]
        public UIManager uiManager_Cp;

        [Header("Furniture")]
        public FurnitureManager furnitureManager_Cp;

        [Header("EazyController")]
        public EazyCamera.EazyController eazyController_Cp;

        [Header("Rooms")]
        public Room room_Cp;

        [Header("InteractWebGL")]
        public InteractWebGL interactWebGL_Cp;
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
    [SerializeField]
    public Opponents_St opponents;

    #endregion

    //////////////////////////////////////////////////////////////////////
    // properties
    //////////////////////////////////////////////////////////////////////
    #region properties

    //-------------------------------------------------- opponents properties
    BackgroundManager bgdManager_Cp
    {
        get { return opponents.bgdManager_Cp; }
    }

    public UIManager uiManager_Cp
    {
        get { return opponents.uiManager_Cp; }
    }

    public FurnitureManager furnitureManager_Cp
    {
        get { return opponents.furnitureManager_Cp; }
    }

    public EazyCamera.EazyController eazyController_Cp
    {
        get { return opponents.eazyController_Cp; }
    }

    public Room room_Cp
    {
        get { return opponents.room_Cp; }
    }

    public InteractWebGL interactWebGL_Cp
    {
        get { return opponents.interactWebGL_Cp; }
    }

    //-------------------------------------------------- private properties

    #endregion

    //////////////////////////////////////////////////////////////////////
    // methods
    //////////////////////////////////////////////////////////////////////

    //-------------------------------------------------- Start is called before the first frame update
    void Start()
    {
        Init();
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
        StartCoroutine(Corou_Init());
    }

    IEnumerator Corou_Init()
    {
        // 
        bgdManager_Cp.Init();
        yield return new WaitUntil(() => bgdManager_Cp.gameState == BackgroundManager.GameState_En.Inited);

        uiManager_Cp.Init();
        yield return new WaitUntil(() => uiManager_Cp.gameState == UIManager.GameState_En.Inited);

        furnitureManager_Cp.Init();
        yield return new WaitUntil(() => furnitureManager_Cp.gameState == FurnitureManager.GameState_En.Inited);

        room_Cp.Init();
        yield return new WaitUntil(() => room_Cp.gameState == Room.GameState_En.Inited);

        interactWebGL_Cp.Init();
        yield return new WaitUntil(() => interactWebGL_Cp.gameState == InteractWebGL.GameState_En.Inited);

        // 
        gameState = GameState_En.Inited;

        // 
        Ready();
    }

    #endregion

    //////////////////////////////////////////////////////////////////////
    // Ready
    //////////////////////////////////////////////////////////////////////
    #region Ready

    //-------------------------------------------------- 
    public void Ready()
    {
        // 
        Play();
    }

    #endregion

    //////////////////////////////////////////////////////////////////////
    // Play
    //////////////////////////////////////////////////////////////////////
    #region Play

    //--------------------------------------------------
    public void Play()
    {
        bgdManager_Cp.Play();

        uiManager_Cp.Play();

        furnitureManager_Cp.Play();

        room_Cp.Play();

        interactWebGL_Cp.Play();
    }

    #endregion
}

}
