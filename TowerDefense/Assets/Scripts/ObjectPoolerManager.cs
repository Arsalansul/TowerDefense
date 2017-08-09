using UnityEngine;
using System.Collections;
using System;

public class ObjectPoolerManager : MonoBehaviour {

    //we'll need pools for Balls and audio objects
    public ObjectPooler BallPooler;
    public ObjectPooler AudioPooler;

    public GameObject BallPrefab;


    //basic singleton implementation
    public static ObjectPoolerManager Instance {get;private set;}
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        //just instantiate the pools
        if (BallPooler == null)
        {
            GameObject go = new GameObject("BallPooler");
            BallPooler = go.AddComponent<ObjectPooler>();
            BallPooler.PooledObject = BallPrefab;
            go.transform.parent = this.gameObject.transform;
            BallPooler.Initialize();
        }

        if (AudioPooler == null)
        {
            GameObject go = new GameObject("AudioPooler");
            AudioPooler = go.AddComponent<ObjectPooler>();
            go.transform.parent = this.gameObject.transform;
            AudioPooler.Initialize(typeof(AudioSource));
        }

        
    }

}
