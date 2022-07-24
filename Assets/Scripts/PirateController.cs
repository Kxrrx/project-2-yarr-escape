using System.Collections;
using UnityEngine;

public class PirateController : MonoBehaviour
{
    public int pirateLives = 3;

    public bool isDead = false;
    public float runSpeed = 15f;
    public float walkSpeed = 7.5f;
    public float carryingSpeed = 5f;
    public float jumpHeight = 1.0f;
    public float gravityValue = -9.81f;
    public float pickUpRange = 2;
    public Transform playerPickUpPos;

    public GameObject nearestPickupItem;
    private float pointerDownTimer = 0f;
    private float requiredHoldTime = 0.2f;

    public GameObject[] pirateMeshList;

    //Private:
    private CharacterController controller;
    public Animator animator;
    private Vector3 playerVelocity;
    private bool isGrounded = false;
    private bool isHoldingItem = false;
    private bool isPickingOrPlacingBarrel = false;
    private bool isCarrying = false;
    private Transform barrelOriginalParent;

    public float pirateControllerRadius = 0.25f;
    public Vector3 pirateControllerCenter = new Vector3(0,1,0);
    public float pirateControllerCarryingRadius = 0.5f;
    public Vector3 pirateControllerCarryingCenter = new Vector3(0, 1, 0.3f);

    private bool blockUpdate = false;
    private Vector3 victoryPosition = new Vector3(0, -0.42f, 16);


    private void Awake()
    {
        if(GameData.singleton != null)
        {
            foreach(GameObject mesh in pirateMeshList)
            {
                mesh.SetActive(false);
            }
            pirateMeshList[GameData.singleton.selectedPirateIndex].SetActive(true);
        }
        else
        {
            pirateMeshList[0].SetActive(true);
        }
    }
    private void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
        animator = transform.GetChild(0).GetComponent<Animator>();
    }

    void Update()
    {
        if (blockUpdate)
        {
            return;
        }
        if (isDead)
        {
            animator.Play("Death");
            if(AudioManager.singleton != null)
            {
                AudioManager.singleton.Stop("Gameplay");
                AudioManager.singleton.Play("GameOver");
            }

            controller.enabled = false;

            pirateLives--;

            if (pirateLives < 1)
            {
                LevelManager.singleton.LoseGame();
            }
            else
            {
                LevelManager.singleton.LoseOneLife();
            }
            enabled = false;
            return;
        }
        if (controller.transform.localPosition.y < -14.7f)
        {
            isDead = true;
            Debug.Log("You died by falling!");
            return;
        }

        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));


        //Movement without carrying:
        if (!isPickingOrPlacingBarrel)
        {
            if (!isHoldingItem && move != Vector3.zero)
            {
                gameObject.transform.forward = move;
                isCarrying = false;
                animator.SetBool("CarryingItem", false);
           
                if (Input.GetButton("Walk")) //Walk:
                {
                    animator.SetBool("Walk", true);
                    animator.SetBool("Running", false);
                    controller.Move(walkSpeed * Time.deltaTime * move);
                }
                else //Run:
                {
                    animator.SetBool("Walk", false);
                    animator.SetBool("Running", true);
                    controller.Move(runSpeed * Time.deltaTime * move);
                }
            }
            else if (isHoldingItem && move != Vector3.zero) //Carrying barrel:
            {
                gameObject.transform.forward = move;
                isCarrying = true;
                LevelManager.singleton.hintText.gameObject.SetActive(true);
                LevelManager.singleton.hintText.text = "Release [space] to place the barrel down.";
                animator.SetBool("Running", false);
                animator.SetBool("CarryingItem", true);
                controller.Move(carryingSpeed * Time.deltaTime * move);
            }
            else //Idle:
            {
                animator.SetBool("Walk", false);
                animator.SetBool("Running", false);
                animator.SetBool("CarryingItem", false);
            }

            playerVelocity.y += gravityValue * Time.deltaTime;
            controller.Move(playerVelocity * Time.deltaTime);
        }    

        if (Input.GetButton("Action") && !isHoldingItem && !isPickingOrPlacingBarrel && nearestPickupItem != null)
        {
            pointerDownTimer += Time.deltaTime;

            if(pointerDownTimer >= requiredHoldTime)
            {
                LevelManager.singleton.hintText.gameObject.SetActive(false);
                Vector3 itemToLookAt = new Vector3(nearestPickupItem.transform.position.x, 0, nearestPickupItem.transform.position.z);
                transform.LookAt(itemToLookAt);
                StartCoroutine(PickUpBarrel(nearestPickupItem));
            }
        }
        else
        {
            pointerDownTimer = 0f;
        }

        if (Input.GetButton("Action") == false && isHoldingItem)
        {
            animator.SetBool("CarryingItem", false);
            Debug.Log("Put barrel down");
            LevelManager.singleton.hintText.gameObject.SetActive(false);
            StartCoroutine(PutBarrelDown());
        }
    }

    private void FixedUpdate()
    {
        if (isDead || controller.transform.position.y < -10)
        {
            return;
        }

        isGrounded = controller.isGrounded;
        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        // Bit shift the index of the layer (8) to get a bit mask
        int layerMask = 1 << 8;
        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        layerMask = ~layerMask;

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, layerMask))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * hit.distance, Color.yellow);
            if (isHoldingItem)
            {
                controller.radius = pirateControllerCarryingRadius;
                controller.center = pirateControllerCarryingCenter;
            }
            else
            {
                controller.radius = pirateControllerRadius;
                controller.center = pirateControllerCenter;
            }
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * 1000, Color.white);
            controller.radius = 0.05f; //Make it super thin to drop the pirate.
            Debug.Log("Player is not standing on the ground! Player is falling!");
        }
    }

    private IEnumerator PickUpBarrel(GameObject barrel)
    {
        isPickingOrPlacingBarrel = true; // safety check

        animator.SetBool("HoldingItem", true);
        yield return new WaitForSeconds(0.2f);
        barrelOriginalParent = barrel.transform.parent;
        barrel.transform.SetParent(playerPickUpPos);
        barrel.transform.localPosition = Vector3.zero;
        barrel.transform.localRotation = Quaternion.identity;

        controller.radius = pirateControllerCarryingRadius;
        controller.center = pirateControllerCarryingCenter;

        yield return new WaitForSeconds(0.8f);

        isHoldingItem = true;
        isPickingOrPlacingBarrel = false; // safety check
        yield return null;
    }

    private IEnumerator PutBarrelDown()
    {
        isPickingOrPlacingBarrel = true; // safety check

        isHoldingItem = false;
        animator.SetBool("HoldingItem", false);
        yield return new WaitForSeconds(0.2f);

        GameObject barrel = playerPickUpPos.GetChild(0).gameObject;
        barrel.transform.SetParent(barrelOriginalParent);
        barrel.transform.localPosition = new Vector3(barrel.transform.localPosition.x, 0, barrel.transform.localPosition.z) + transform.forward * 0.35f;
        barrel.transform.rotation = Quaternion.identity;

        controller.radius = pirateControllerRadius;
        controller.center = pirateControllerCenter;

        yield return new WaitForSeconds(0.8f);

        isPickingOrPlacingBarrel = false; // safety check
        yield return null;
    }

    public void Revive()
    {
        if(playerPickUpPos.childCount > 0)
        {
            Destroy(playerPickUpPos.GetChild(0).gameObject);
        }
        isDead = false;
        enabled = true;
        isHoldingItem = false;
        isCarrying = false;
        animator.SetBool("HoldingItem", false);
        animator.SetBool("CarryingItem", false);
        animator.SetTrigger("Revive");

        if(AudioManager.singleton != null)
        {
            AudioManager.singleton.Play("Gameplay");
        }
        controller.enabled = true;
        Camera.main.GetComponent<CameraFollow>().enabled = true;
    }
    public void WalkToTheVictoryPosition()
    {
        StartCoroutine(AnimatePlayerToVictoryPosition());
    }
    private IEnumerator AnimatePlayerToVictoryPosition()
    {
        blockUpdate = true;

        animator.SetBool("Running", true);
        Camera.main.GetComponent<CameraFollow>().zoomToGetawaySloop = true;

        while (transform.position != victoryPosition)
        {
            transform.LookAt(victoryPosition);
            transform.position = Vector3.MoveTowards(transform.position, victoryPosition, runSpeed * Time.deltaTime);
            yield return null;
        }
        
        animator.SetBool("Running", false);
        animator.Play("Cheer");
        transform.eulerAngles = new Vector3(0, 180, 0);

        LevelManager.singleton.continueButton.SetActive(true);
        LevelManager.singleton.finishedMenu.SetActive(true);

        yield return null;
    }
}