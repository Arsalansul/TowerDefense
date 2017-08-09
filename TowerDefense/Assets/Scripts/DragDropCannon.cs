using UnityEngine;
using System.Collections;
using System.Linq;
using Assets.Scripts;
using UnityEngine.EventSystems;
/// <summary>
/// Drag and drop mechanism
/// </summary>
public class DragDropCannon : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{        
    // Use this for initialization
    void Start()
    {
        mainCamera = Camera.main;
        newCannon = Instantiate(CannonPrefab);
        newCannon.SetActive(false);
    }
   
    private Camera mainCamera;
    public GameObject CannonPrefab;
    private GameObject newCannon;
    void Update()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 50, Color.yellow);
    }

    //will be colored red if we cannot place a Cannon there
    private GameObject tempBackgroundBehindPath;

    private GameObject selected = null;

    public void OnBeginDrag(PointerEventData data)
    {
        selected = data.pointerPressRaycast.gameObject;
    }    

    public void OnDrag(PointerEventData data)
    {
        if (selected.gameObject.tag == "CannonGenerator" && GameManager.Instance.MoneyAvailable >= Constants.CannonCost)
        {                  
            RaycastHit[] hits;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            hits = Physics.RaycastAll(ray, 500f);

            if (hits.Length > 0 && hits != null)
            {
                int terrainCollderQuadIndex = GetTerrainColliderQuadIndex(hits);
                if (terrainCollderQuadIndex != -1)
                {
                    newCannon.transform.position = hits[terrainCollderQuadIndex].point;
                    newCannon.SetActive(true);
                }
                
                //if (hits.Where(x => x.collider.gameObject.tag == "Tower").Count() > 0
                //    || hits.Where(x => x.collider.gameObject.tag == "Cannon").Count() > 1)
                //{
                //    //we cannot place a Cannon there
                //    GameObject backgroundBehindPath = hits.Where(x => x.collider.gameObject.tag == "Background").First().collider.gameObject;
                //    //make the sprite material "more red"
                //    //to let the user know that we can't place a Cannon here
                //    backgroundBehindPath.GetComponent<MeshRenderer>().material.color = Constants.RedColor;
                //
                //    if (tempBackgroundBehindPath != backgroundBehindPath)
                //        ResetTempBackgroundColor();
                //    //cache it to revert later
                //    tempBackgroundBehindPath = backgroundBehindPath;
                //}
                //else //just reset the color on previously set paths
                //{
                //    ResetTempBackgroundColor();
                //}
                else
                    newCannon.SetActive(false);
            }
           

        }

    }

    public void OnEndDrag(PointerEventData data)
    {
        selected = null;
        //ResetTempBackgroundColor();

        // If the prefab instance is active after dragging stopped, it means
        // it's in the arena so (for now), just drop it in.
        if (newCannon.activeSelf)
        {
            Instantiate(CannonPrefab, newCannon.transform.position, Quaternion.identity);
        }

        // Then set it to inactive ready for the next drag!
        newCannon.SetActive(false);

        ////in order to place it, we must have a background and no other bunnies
        //if (hits.Where(x => x.collider.gameObject.tag == "Background").Count() > 0
        //    && hits.Where(x => x.collider.gameObject.tag == "Path").Count() == 0
        //    && hits.Where(x => x.collider.gameObject.tag == "Cannon").Count() == 1)
        //{
        //    //we can leave a Cannon here, so decrease money and activate it
        //    GameManager.Instance.AlterMoneyAvailable(-Constants.CannonCost);
        //    newCannon.transform.position = hits.Where(x => x.collider.gameObject.tag == "Background").First().collider.gameObject.transform.position;
        //    newCannon.GetComponent<Cannon>().Activate();
        //}
        //else
        //{
        //    //we can't leave a Cannon here, so destroy the temp one
        //    Destroy(newCannon);
        //}

    }
    //popov end
    int GetTerrainColliderQuadIndex(RaycastHit[] hits)
    {
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider.gameObject.tag==("Background"))
            {
                return i;
            }
        }

        return -1;
    }
    //make background sprite appear as it is
    private void ResetTempBackgroundColor()
    {
        if (tempBackgroundBehindPath != null)
            tempBackgroundBehindPath.GetComponent<MeshRenderer>().material.color = Constants.BlackColor;
    }
    
}
