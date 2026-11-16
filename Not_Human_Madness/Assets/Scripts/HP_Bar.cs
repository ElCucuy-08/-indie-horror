using UnityEngine;
using UnityEngine.UI;
public class HP_Bar : MonoBehaviour
{
    public float Health = 100;
    public Slider HpProgress;

    private void Start()
    {

    }
    private void Update()
    {
        HpProgress.value = Health;
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            Health -= 10;
        }
    }
}
