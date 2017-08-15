using UnityEngine;
using System.Collections;
using System.Linq;
using Assets.Scripts;

public class Cannon : MonoBehaviour
{

    //Ball sound found here
    //https://www.freesound.org/people/Erdie/sounds/65734/

    public Transform BallSpawnPosition;
    public GameObject BallPrefab;
    public float ShootWaitTime = 2f;
    private float LastShootTime = 0f;
    GameObject targetedEnemy;
    private float InitialBallForce = 100f;
    private CannonState State;

    // Use this for initialization
    void Start()
    {
        State = CannonState.Searching;  ///TODO вернуть inactive
        //find where we're shooting from
        BallSpawnPosition = transform.Find("BallSpawnPosition");
    }

    // Update is called once per frame
    void Update()
    {
        //if we're in the last round and we've killed all enemies, do nothing
        if (GameManager.Instance.FinalRoundFinished && GameManager.Instance.Enemies.Where(x => x != null).Count() == 0)
            State = CannonState.Inactive;

        //searching for an enemy
        if (State == CannonState.Searching)
        {
            if (GameManager.Instance.Enemies.Where(x => x != null).Count() == 0) return;
            
            targetedEnemy = GameManager.Instance.Enemies.Where(x => x != null)
                .Aggregate((current, next) => Vector3.Distance(current.transform.position, transform.position)
               < Vector3.Distance(next.transform.position, transform.position)
              ? current : next);

            //if there is an enemy and is close to us, target it
            if (targetedEnemy != null && targetedEnemy.activeSelf
                && Vector3.Distance(transform.position, targetedEnemy.transform.position)
                < Constants.MinDistanceForCannonToShoot)
            {
                State = CannonState.Targeting;
            }

        }
        else if (State == CannonState.Targeting)
        {
            //if the targeted enemy is still close to us, look at it and shoot!
            if (targetedEnemy != null 
                && Vector3.Distance(transform.position, targetedEnemy.transform.position)
                    < Constants.MinDistanceForCannonToShoot)
            {
                LookAndShoot();
            }
            else //enemy has left our shooting range, so look for another one
            {
                State = CannonState.Searching;
            }
        }
    }

    private void LookAndShoot()
    {
        //look at the enemy
        Quaternion diffRotation = Quaternion.LookRotation
            (-transform.position + targetedEnemy.transform.position, Vector3.forward);
        transform.rotation = Quaternion.RotateTowards
            (transform.rotation, diffRotation, Time.deltaTime * 2000);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

        //make sure we're almost looking at the enemy before start shooting
        Vector3 direction = targetedEnemy.transform.position - transform.position;
        float axisDif = Vector3.Angle(transform.forward, direction);
        //shoot only if we have 20 degrees rotation difference to the enemy   
        if (axisDif <= 30f)
        {
            if (Time.time - LastShootTime > ShootWaitTime)
            {
                Shoot(direction);
                LastShootTime = Time.time;
            }
        
        }
    }


    private void Shoot(Vector3 dir)
    {
        //if the enemy is still close to us
        if (targetedEnemy != null && targetedEnemy.activeSelf
            && Vector3.Distance(transform.position, targetedEnemy.transform.position)
                    < Constants.MinDistanceForCannonToShoot)
        {
            //create a new Ball
            GameObject go = ObjectPoolerManager.Instance.BallPooler.GetPooledObject();
            go.transform.position = BallSpawnPosition.position;
            go.transform.rotation = transform.rotation;
            go.SetActive(true);
            //SHOOT IT!
            go.GetComponent<Rigidbody>().AddForce(dir * InitialBallForce);
            //AudioManager.Instance.PlayBallSound();
        }
        else//find another enemy
        {
            State = CannonState.Searching;
        }
    }

    public void Activate()
    {
        State = CannonState.Searching;
    }
}
