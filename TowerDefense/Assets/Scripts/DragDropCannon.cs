using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using Assets.Scripts;

/// <summary>
/// Drag and drop mechanism
/// </summary>
public class DragDropCannon : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        mainCamera = Camera.main;
    }
   
    private Camera mainCamera;
    //type of bunnies we'll create
    public GameObject CannonPrefab;
    //the starting object for the drag
    public GameObject CannonGenerator;
    bool isDragging = false;
    //temp Cannon
    private GameObject newCannon;

    //will be colored red if we cannot place a Cannon there
    private GameObject tempBackgroundBehindPath;

    // Update is called once per frame
    void Update()
    {
        //if we have money and we can drag a new Cannon
        if (Input.GetMouseButtonDown(0) && !isDragging && GameManager.Instance.MoneyAvailable >= Constants.CannonCost)
        {
            ResetTempBackgroundColor();
            Vector3 location = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 touchPos = new Vector2(location.x, location.y);
            //if user has tapped onto the Cannon generator
            if (CannonGenerator.GetComponent<CircleCollider2D>() ==Physics2D.OverlapPoint(touchPos, 1 << LayerMask.NameToLayer("CannonGenerator")))
            {
                //initiate dragging operation and create a new Cannon for us to drag
                isDragging = true;
                //create a temp Cannon to drag around
                newCannon = Instantiate(CannonPrefab, CannonGenerator.transform.position, Quaternion.identity) as GameObject;
            }
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray.origin, ray.direction);
            if (hits.Length > 0 && hits[0].collider != null)
            {
                newCannon.transform.position = hits[0].collider.gameObject.transform.position;

                //if we're hitting a path or tower
                //or there is an existing Cannon there
                //we use > 1 since we're hovering over the newCannon gameobject 
                //(i.e. there is already a Cannon there)
                if (hits.Where(x=>x.collider.gameObject.tag == "Tower").Count() > 0
                    || hits.Where(x=>x.collider.gameObject.tag == "Cannon").Count() > 1)
                {
                    //we cannot place a Cannon there
                    GameObject backgroundBehindPath = hits.Where
                        (x => x.collider.gameObject.tag == "Background").First().collider.gameObject;
                    //make the sprite material "more red"
                    //to let the user know that we can't place a Cannon here
                    backgroundBehindPath.GetComponent<SpriteRenderer>().color = Constants.RedColor;
                    
                    if (tempBackgroundBehindPath != backgroundBehindPath)
                        ResetTempBackgroundColor();
                    //cache it to revert later
                    tempBackgroundBehindPath = backgroundBehindPath;
                }
                else //just reset the color on previously set paths
                {
                    ResetTempBackgroundColor();
                }

            }
        }
        //we're stopping dragging
        else if (Input.GetMouseButtonUp(0) && isDragging)
        {
            ResetTempBackgroundColor();
            //check if we can leave the Cannon here
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            RaycastHit[] hits = Physics.RaycastAll(ray.origin, ray.direction, Mathf.Infinity, ~(1 << LayerMask.NameToLayer("CannonGenerator")));
            //in order to place it, we must have a background and no other bunnies
            if (hits.Where(x=>x.collider.gameObject.tag == "Background").Count() > 0
                && hits.Where(x => x.collider.gameObject.tag == "Path").Count() == 0
                && hits.Where(x=>x.collider.gameObject.tag == "Cannon").Count() == 1)
            {
                //we can leave a Cannon here, so decrease money and activate it
                GameManager.Instance.AlterMoneyAvailable(-Constants.CannonCost);
                newCannon.transform.position = 
                    hits.Where(x => x.collider.gameObject.tag == "Background")
                    .First().collider.gameObject.transform.position;
                newCannon.GetComponent<Cannon>().Activate();
            }
            else
            {
                //we can't leave a Cannon here, so destroy the temp one
                Destroy(newCannon);
            }
            isDragging = false;

        }
    }

    //make background sprite appear as it is
    private void ResetTempBackgroundColor()
    {
        if (tempBackgroundBehindPath != null)
            tempBackgroundBehindPath.GetComponent<SpriteRenderer>().color = Constants.BlackColor;
    }

}
