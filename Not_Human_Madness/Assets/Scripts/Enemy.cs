using UnityEngine;
using UnityEngine.AI;
public class Enemy : MonoBehaviour
{
    NavMeshAgent agent;
    Transform player;
    Vector3 playerCorrectPosition = Vector3.zero;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    private void Update()
    {
        playerCorrectPosition = player.position;
        playerCorrectPosition.y = transform.position.y;
        transform.LookAt(playerCorrectPosition);

        if (Vector3.Distance(playerCorrectPosition, transform.position) > 1.5)
        {
            transform.position = Vector3.MoveTowards(transform.position, playerCorrectPosition, Time.deltaTime * 8);
        } 
    }
    private void FixedUpdate()
    {
       
        float distance = Vector3.Distance(player.position, transform.position);
        if (distance > 6)
        {
            GetComponent<Animator>().SetBool("Attack", false);
        }
        else
        {
            
            transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
            GetComponent<Animator>().SetBool("Attack", true);
        }
    }
    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        Destroy(other.gameObject);
    //    }
    //}
}
