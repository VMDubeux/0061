using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private int stepsInGrass;
    [SerializeField] private int minStepsToEncounter;
    [SerializeField] private int maxStepsToEncounter;

    private PlayerControls playerControls;
    private Rigidbody rb;
    private Vector3 movement;
    private bool movingInGrass;
    private float stepTimer;
    private int stepsToEncouter;
    private PartyManager partyManager;

    private const float TIME_PER_STEP = 0.5f;
    private const string IS_WALK_PARAM = "isWalk";
    private const string BATTLE_SCENE = "BattleScene";

    private void Awake()
    {
        playerControls = new PlayerControls();
        CalculateStepsToNextEncouter();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        partyManager = GameObject.FindFirstObjectByType<PartyManager>();
        if (partyManager.GetPosition() != Vector3.zero)
        {
            transform.position = partyManager.GetPosition();
        }
    }

    void Update()
    {
        float x = playerControls.Player.Move.ReadValue<Vector2>().x;
        float z = playerControls.Player.Move.ReadValue<Vector2>().y;
        //print($"{x} , {z}");
        movement = new Vector3(x, 0, z).normalized;
        anim.SetBool(IS_WALK_PARAM, movement != Vector3.zero);

        switch (x)
        {
            case < 0:
                spriteRenderer.flipX = true;
                break;

            case > 0:
                spriteRenderer.flipX = false;
                break;
        }
    }

    private void FixedUpdate()
    {
        rb.MovePosition(transform.position + speed * Time.deltaTime * movement);

        Collider[] collider = Physics.OverlapSphere(transform.position, 1, layerMask);
        movingInGrass = collider.Length != 0 && movement != Vector3.zero;

        if (movingInGrass)
        {
            stepTimer += Time.fixedDeltaTime;

            if (stepTimer > TIME_PER_STEP)
            {
                stepsInGrass++;
                stepTimer = 0;

                if (stepsInGrass >= stepsToEncouter)
                {
                    partyManager.SetPosition(transform.position);
                    SceneManager.LoadScene(BATTLE_SCENE);
                    //print("Change scene!");
                }
            }
        }
    }

    private void CalculateStepsToNextEncouter()
    {
        stepsToEncouter = Random.Range(minStepsToEncounter, maxStepsToEncounter);
    }
}