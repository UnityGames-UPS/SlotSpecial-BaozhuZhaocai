using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[System.Serializable]
public  class SpriteLists
{
    public Sprite[] spritex;
}
public class AnimationsOP : MonoBehaviour
{

    public int StartIndex;
    public int EndIndex;

    [Header("Rocket")]
    [SerializeField] private Animator ParentRocket;
    [SerializeField] private Animator RedRocket;
    [SerializeField] private Animator BlueRocket;
    [SerializeField] private Animator GreenRocket;
    [SerializeField] private ImageAnimation LastBlast;

    [Header("FireWorksBG")]
    [SerializeField] private ImageAnimation[] FireWorkOne;

    [Header("Blast Prograsive")]
   // [SerializeField] private SpriteLists[] blast;
    

    [Header("Blast Prograsive")]
   // [SerializeField] private SpriteLists[] loops;

    [Header("Craciker Image")]
    [SerializeField] private ImageAnimation GreenPataka;
    [SerializeField] private ImageAnimation RedPataka;
    [SerializeField] private ImageAnimation BluePataka;

    [Header("GeeenLadi")]
    [SerializeField] private SpriteLists[] g_EmptyLoop;
    [SerializeField] private SpriteLists[] g_FireLoop;
    [SerializeField] private SpriteLists[] g_ProgressLoop;
    [SerializeField] private SpriteLists[] g_BlastLoop;

    [Header("RedLadi")]
    [SerializeField] private SpriteLists[] r_EmptyLoop;
    [SerializeField] private SpriteLists[] r_FireLoop;
    [SerializeField] private SpriteLists[] r_ProgressLoop;
    [SerializeField] private SpriteLists[] r_BlastLoop;

    [Header("BlueLadi")]
    [SerializeField] private SpriteLists[] b_EmptyLoop;
    [SerializeField] private SpriteLists[] b_FireLoop;
    [SerializeField] private SpriteLists[] b_ProgressLoop;
    [SerializeField] private SpriteLists[] b_BlastLoop;

    private void Start()
    {
      //  DoorAnimation(Object, Startpos, EndPos.position);
      //  DoorAnimation(Object1, Startpos1, EndPos1.position);
        StartCoroutine(FireworkAnimation());

        StartCoroutine(TillWhen(GreenPataka, StartIndex, EndIndex));
        StartCoroutine(TillWhen(RedPataka, StartIndex, EndIndex));
        StartCoroutine(TillWhen(BluePataka, StartIndex, EndIndex-2));

        StartCoroutine(TestRocketAnimation());
    }


    #region Rocket animation


    IEnumerator TestRocketAnimation()
    {
        for (int i = 1; i <6; i++)
        {
            for (int j = 1; j < 4; j++)
            {
                int x = Random.Range(1, 6);
                int x1 = Random.Range(1, 6);
                int y = Random.Range(1, 4);
                int y1 = Random.Range(1, 4);
               // yield return new WaitForSeconds(2f);
                Debug.Log("Clip" + i  + j);
                ParentRocket.Play("Clip"+i.ToString()+j.ToString());
                RedRocket.Play("Clip"+x.ToString()+y.ToString());
                GreenRocket.Play("Clip"+x1.ToString()+y1.ToString());
                string stateName = "Clip" + i.ToString() + j.ToString();
                while (!ParentRocket.GetCurrentAnimatorStateInfo(0).IsName(stateName))
                {
                    yield return null;
                }

                // Wait until the animation is done playing
                while (ParentRocket.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
                {
                    yield return null;
                }
                LastBlast.StopAnimation();
                LastBlast.StartAnimation();

            }
        }
    }



    #endregion

    #region NumberBurnAnimation

    IEnumerator BurnANumber(ImageAnimation animScript, Sprite[] Ship_Sprite, bool loop, float animspeed = 12)
    {
        Debug.Log("DevTest: Animation");
        animScript.StopAnimation();
        animScript.textureArray.Clear();
        animScript.textureArray.TrimExcess();
        for (int i = 0; i < Ship_Sprite.Length; i++)
        {
            animScript.textureArray.Add(Ship_Sprite[i]);
        }
        Debug.Log("DevTest: Animation2");
        animScript.AnimationSpeed = animspeed;
        animScript.doLoopAnimation = loop;
        animScript.StartAnimation();
        yield return new WaitUntil(() => animScript.IsComplete);
    }


    #endregion


    #region Pataka_Animation

    IEnumerator TillWhen(ImageAnimation anim,int startIndex,int endIndex)
    {
        while(startIndex<endIndex)
        {
           // Debug.Log("DevTest: Whileloop");
            yield return Blastprocess(startIndex, anim, false);
            startIndex++;
        }
        yield return Blastprocess(endIndex, anim, true);
    }
    IEnumerator Blastprocess(int x,ImageAnimation anim,bool End)
    {
        SpriteLists[] blast = null;
        SpriteLists[] loops = null;
        if (anim == RedPataka)
        {
            loops = r_FireLoop;
            blast = r_ProgressLoop;

        }
        else if(anim == BluePataka)
        {

            blast = b_ProgressLoop;
            loops = b_FireLoop;
        }
        else if (anim == GreenPataka)
        {

            blast = g_ProgressLoop;
            loops = g_FireLoop;
        }
        yield return ProgressiveFireAnimation(anim, blast[x].spritex,false);
       
        if(End)
        {
            StartCoroutine(ProgressiveFireAnimation(anim, loops[x].spritex,true,10f));
        }
    }
    IEnumerator ProgressiveFireAnimation(ImageAnimation animScript,Sprite[] Ship_Sprite , bool loop,float animspeed=70f)
    {
     //   Debug.Log("DevTest: Animation");
        animScript.StopAnimation();
        animScript.textureArray.Clear();
        animScript.textureArray.TrimExcess();
        for (int i = 0; i < Ship_Sprite.Length; i++)
        {
            animScript.textureArray.Add(Ship_Sprite[i]);
        }
      //  Debug.Log("DevTest: Animation2");
        animScript.AnimationSpeed = animspeed;
        animScript.doLoopAnimation = loop;
        animScript.StartAnimation();
        yield return new WaitUntil(() => animScript.IsComplete);
    }

        internal void DoorAnimation(GameObject obj, Transform startpos,Vector3 endpos)
    {
        obj.transform.position = startpos.position;
        obj.SetActive(true);
        Tween anim = obj.transform.DOMove(endpos, 1f).SetEase(Ease.OutBounce).OnComplete(() => Debug.Log("AnimationDone"));
    }

    IEnumerator FireworkAnimation()
    {
        while (true)
        {
            int x = Random.Range(0, 6);
            FireWorkOne[x].StartAnimation();

           
            yield return new WaitForSeconds(2f);
            FireWorkOne[x].StopAnimation();
        }
    }
    #endregion
}
