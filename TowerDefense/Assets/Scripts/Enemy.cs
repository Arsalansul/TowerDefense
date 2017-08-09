using UnityEngine;
using System.Collections;
using Assets.Scripts;
using System;

public class Enemy : MonoBehaviour
{
    //death sound found here
    //https://www.freesound.org/people/psychentist/sounds/168567/

    public int Health;
    int nextWaypointIndex = 0;
    public float Speed = 2f;
    // Use this for initialization
    void Start()
    {
        Health = Constants.InitialEnemyHealth;
    }

    // Update is called once per frame
    void Update()
    {

        //calculate the distance between current position
        //and the target waypoint
        if (Vector3.Distance(transform.position, GameManager.Instance.Waypoints[nextWaypointIndex].position) < 0.01f)
        {
            //is this waypoint the last one?
            if (nextWaypointIndex == GameManager.Instance.Waypoints.Length - 1)
            {
                RemoveAndDestroy();
                GameManager.Instance.Lives--;
            }
            else
            {
                //our enemy will go to the next waypoint
                nextWaypointIndex++;
                //our simple AI, enemy is looking at the next waypoint
                transform.LookAt(GameManager.Instance.Waypoints[nextWaypointIndex].position);
                //only in the z axis
                //transform.eulerAngles = new Vector3(0, 0, 0);
            }
        }
        
        //enemy is moved towards the next waypoint
        transform.position = Vector3.MoveTowards(transform.position, GameManager.Instance.Waypoints[nextWaypointIndex].position, Time.deltaTime * Speed);
    }



    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Ball")
        {//if we're hit by an Ball
            if (Health > 0)
            {
                //decrease enemy health
                Health -= Constants.Ball;
                if (Health <= 0)
                {
                    RemoveAndDestroy();
                }
            }
            col.gameObject.GetComponent<Ball>().Disable(); //disable the Ball
        }
    }
    public event EventHandler EnemyKilled;

    void RemoveAndDestroy()
    {
        //AudioManager.Instance.PlayDeathSound();
        //remove it from the enemy list
        GameManager.Instance.Enemies.Remove(this.gameObject);
        Destroy(this.gameObject);
        //notify interested parties that we died
        if (EnemyKilled != null)
            EnemyKilled(this, EventArgs.Empty);
    }
}
