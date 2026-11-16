using UnityEngine;

public class Death : MonoBehaviour
{
    public GameObject cam1;
    public HP_Bar hp;

    private void Start()
    {

    }
    private void Update()
    {
        if (hp.Health <= 0)
        {
            cam1.SetActive(false);
        }
    }
}
