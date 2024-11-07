using System.Collections;
using UnityEngine;

public class AnimationStateController : MonoBehaviour
{
    public Animator anim;

    public GameObject expand;
    private ObjectManager modelExpander;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        modelExpander = expand.GetComponent<ObjectManager>();
        StartCoroutine(PlayAndWaitForAnim());
    }

    // Update is called once per frame
    void Update()
    {
    }

    public IEnumerator PlayAndWaitForAnim()
    {
        Animator targetAnim = anim;
        string stateName = "explode";
        //targetAnim.Play(stateName);

        /*if (!modelExpander.isExpanded)
        {
            targetAnim.SetBool("isReset", true);
        }*/

        //Wait until we enter the current state
        while (!targetAnim.GetCurrentAnimatorStateInfo(0).IsName(stateName))
        {
            yield return null;
        }
        //Now, Wait until the current state is done playing
        while ((targetAnim.GetCurrentAnimatorStateInfo(0).normalizedTime) < 1.0f)
        {
            yield return null;
        }

        //Done playing. Do something below!
        EndStepEvent();
    }

    void EndStepEvent()
    {
        Debug.Log("animate finish.");
        //if (!anim.GetBool("isReset"))
        //{
        //    modelExpander.expandedModel.SetActive(true);
        //    modelExpander.animateModel.SetActive(false);
        //}
        //else
        //{
        //    modelExpander.modelContainer.SetActive(true);
        //    modelExpander.animateModel.SetActive(false);
        //}
        //Destroy(GetComponent<AnimationStateController>());
    }

    public void startAnimation()
    {
        StartCoroutine(PlayAndWaitForAnim());
    }

}
