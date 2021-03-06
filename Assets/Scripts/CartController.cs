﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class CartController : MonoBehaviour {

    public GameObject JumpPrefab;
    //public GameObject LootCartPrefab;
    public GameObject BrakeEffect;


    public float ConstantForce = 1.0f;

    public bool InvertSideLean = false;
    public bool InvertFwdLean = false;


    public float MaxLeanAngle = 45;
    public float MaxLeanForce = 2;
    private ForceMode LeanMode = ForceMode.Acceleration;
    public float MaxBrakeForce = 0.5f;

    public float MaxJump = 5;

    public float MaxMass = 4;

    public GameObject LeadLootCart;

    Rigidbody Rb { get; set; }

    public float MassModifier { get { return Mathf.SmoothStep(0.5f, 2.0f, Rb.mass / MaxMass); } }

    float InitialDrag { get; set; }

    public bool IsJumping { get; set; }

    public float CurrentBrake { get; protected set; }

    


    // Use this for initialization
    void Start() {
        Rb = GetComponent<Rigidbody>();
        InitialDrag = Rb.drag;
    }

    void Update() {
        // Jump

        var jump = Input.GetAxisRaw("Jump");


        if (jump >= 0.5 && !IsJumping) {

            var offset = Rb.velocity;
            
            var trigger = Instantiate(JumpPrefab, transform.position + Rb.velocity, new Quaternion());
            Destroy(trigger, 1000);
        }

        // Cull Loot Cart

        var cull = Input.GetAxisRaw("Cull");

        if (cull >= 0.5) {
            SpawnLootCart(new Vector3());
        }

        BrakeEffect.SetActive(CurrentBrake != 0);
    }

    // Update is called once per frame
    void FixedUpdate() {

        // Check Velocity > 0 or DIE
        if (Rb.velocity.sqrMagnitude < float.Epsilon)
            ; // TODO: Player Dies


        // Lean
        var leanH = -MaxLeanForce * MassModifier * Input.GetAxis("Horizontal");
        var leanV = -MaxLeanForce * MassModifier * Input.GetAxis("Vertical");

        if (InvertSideLean) leanH *= -1;
        if (InvertFwdLean) leanV *= -1;

        var forcePosn = transform.TransformPoint(new Vector3(0, 1, 0));
        var forceDirn = transform.TransformDirection(new Vector3(0, 0, leanH));

        var tilt = Mathf.Acos(Vector3.Dot(transform.TransformDirection(new Vector3(0, 0, 1)), Vector3.up)) - (Mathf.PI / 2.0f);

        

        if (false && Mathf.Abs(tilt) > (MaxLeanAngle * Mathf.PI / 180)) {
            Debug.Log("Tilt!!!" + (tilt * 180.0 / Mathf.PI).ToString("N3") + " > " + (MaxLeanAngle * Mathf.PI / 180));
            Rb.AddRelativeTorque(new Vector3(-leanH, 0, leanV), LeanMode); // Lean Left/Right, Forward/Back
        } else
            Rb.AddRelativeTorque(new Vector3(leanH, 0, leanV), LeanMode); // Lean Left/Right, Forward/Back
        


        
        Rb.AddRelativeForce(new Vector3(-leanV, 0, leanH)); // Move Left/Right

        if (leanH != 0 || leanV != 0)
            Debug.Log("Lean H: " + leanH.ToString("N3") + " , V: " + leanV.ToString("N3"));



        // Brake

        CurrentBrake = MaxBrakeForce * Input.GetAxis("Brake");

        Rb.drag = Mathf.Max(InitialDrag, CurrentBrake);

        if (CurrentBrake != 0)
            Debug.Log("Brake: " + CurrentBrake.ToString("N3"));

        



    }

    void OnCollisionEnter(Collision col) {


    }


    void OnCollisionStay(Collision col) {
    }

    void OnCollisionExit(Collision col) {
    }

    void OnTriggerExit(Collider col) {
        if ( col.gameObject == LeadLootCart) {
            // Loot Cart cleared space, Spawn Cart
            var offset = transform.position - LeadLootCart.transform.position;
            offset.Scale(new Vector3(0.5f, 0.5f, 0.5f));

            var posn = transform.position - offset;
            var cart = Instantiate(JumpPrefab);

            cart.GetComponent<SpringJoint>().connectedBody = Rb;
            LeadLootCart.GetComponent<SpringJoint>().connectedBody = cart.GetComponent<Rigidbody>();
        }
    }

    public void SpawnLootCart(Vector3 posn) {
        //if (LeadLootCart != null) // Detach cart to allow room
        //{
        //    var hitch = LeadLootCart.GetComponent<SpringJoint>();
        //    if (hitch != null)
        //        Destroy(hitch);

        //} else {

        //    var cart = Instantiate(LootCartPrefab, posn, transform.rotation);

        //    var hitch = LeadLootCart.AddComponent<SpringJoint>();
        //    hitch.anchor = new Vector3(0, 0, -1);
        //    hitch.connectedAnchor = new Vector3(0, 0, 1);
        //    hitch.connectedBody = cart.GetComponent<Rigidbody>();
        //}
    }
}
