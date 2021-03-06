﻿using UnityEngine;
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
            hits = Physics.RaycastAll(ray, 50f);

            if (hits.Length > 0 && hits != null)
            {
                int terrainCollderQuadIndex = GetTerrainColliderQuadIndex(hits);
                if (terrainCollderQuadIndex != -1)
                {
                    newCannon.transform.position = new Vector3(Mathf.Round(hits[terrainCollderQuadIndex].point.x),0, Mathf.Round(hits[terrainCollderQuadIndex].point.z));
                    newCannon.SetActive(true);
                    if (hits.Where(x => x.collider.gameObject.tag == "Tower").Count() > 0 || hits.Where(x => x.collider.gameObject.tag == "Cannon").Count() > 1)
                    {
                        //we cannot place a Cannon there
                        GameObject backgroundBehindPath = hits.Where(x => x.collider.gameObject.tag == "Background").First().collider.gameObject;
                        //make the sprite material "more red"
                        //to let the user know that we can't place a Cannon here
                        backgroundBehindPath.GetComponent<MeshRenderer>().material.color = Constants.RedColor;

                        if (tempBackgroundBehindPath != backgroundBehindPath)
                            ResetTempBackgroundColor();
                        //cache it to revert later
                        tempBackgroundBehindPath = backgroundBehindPath;

                        newCannon.SetActive(false);
                    }
                    else //just reset the color on previously set paths
                    {
                        ResetTempBackgroundColor();
                    }
                }
                else
                    newCannon.SetActive(false);                
            }         

        }
    }

    public void OnEndDrag(PointerEventData data)
    {
        selected = null;
        
        ResetTempBackgroundColor();

        // If the prefab instance is active after dragging stopped, it means
        // it's in the arena so (for now), just drop it in.
        if (newCannon.activeSelf)
        {
            Instantiate(CannonPrefab, newCannon.transform.position, Quaternion.identity);
            GameManager.Instance.AlterMoneyAvailable(-Constants.CannonCost);
        }       

        // Then set it to inactive ready for the next drag!
        newCannon.SetActive(false);       
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
            tempBackgroundBehindPath.GetComponent<MeshRenderer>().material.color=Constants.BackgroundColor;
    }
    
}
