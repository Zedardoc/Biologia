using UnityEngine;
using UnityEngine.UI;
public class menuListener : MonoBehaviour
{
    public Animator animator;
    public GameObject button1;
    public GameObject button2;
    public GameObject button3;
    public GameObject cube;
    public Slider slider;
    public GameObject menu;
    public void OnPlay()
    {
        button1.SetActive(true);
        button2.SetActive(true);
        button3.SetActive(true);
        menu.SetActive(false);
        cube.transform.localScale = new Vector3(3 +slider.value, cube.transform.localScale.y, 3+ slider.value);
        animator.SetBool("play", true);
    }
}
