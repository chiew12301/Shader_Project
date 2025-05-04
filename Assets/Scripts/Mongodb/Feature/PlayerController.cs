using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KC_Custom;

public class PlayerController : MonobehaviourSingleton<PlayerController>
{
    [Header("Component")]
    [SerializeField] private Camera m_playerCam = null;
    [SerializeField] private MeshRenderer m_playerMeshRenderer = null;
    [SerializeField] private Rigidbody m_rigidbody = null;

    [Header("Settings")]
    [SerializeField] private float m_speed = 1.0f;

    private Vector3 m_inputPosition = Vector3.zero;


    //==========================================

    protected override void Start()
    {
        base.Start();
        if(RealmController.GetInstance() != null && RealmController.GetInstance().IsRealmReady())
        {
            this.transform.position = RealmController.GetInstance().GetPlayerPosition();
        }
        InvokeRepeating("PostDataToDatabase", 1.0f, 2.0f);
    }

    protected override void Update() 
    {
        base.Update();    
        this.m_inputPosition = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
    }

    protected override void LateUpdate() 
    {
        base.LateUpdate();
        this.MovePlayerPosition();
        //this.PostDataToDatabase();
    }

    //==========================================

    private void MovePlayerPosition()
    {
        this.m_rigidbody.linearVelocity = this.m_inputPosition * this.m_speed * Time.deltaTime;
    }

    private void PostDataToDatabase()
    {
        if(RealmController.GetInstance() != null && RealmController.GetInstance().IsRealmReady())
        {
            RealmController.GetInstance().SetPlayerPosition(this.transform.position);
            RealmController.GetInstance().AddPlayerScore(1);
        }
    }

    //==========================================
}
