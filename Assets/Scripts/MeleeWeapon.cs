using redd096;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    [Header("Melee Points")]
    [SerializeField] Transform topMeleePoint = default;
    [SerializeField] Transform bottomMeleePoint = default;
    [Header("Traces")]
    [Tooltip("Press it to update melee points")]
    [SerializeField] bool update = false;
    [Tooltip("Number of traces for every swing. Without top and bottom")]
    [SerializeField] int tracesPerSwing = 1;
    [SerializeField] bool useAlsoTopMelee = true;
    [SerializeField] bool useAlsoBottomMelee = true;

    [Header("Debug")]
    [SerializeField] float durationDebugTraces = 5;

    List<Transform> meleePoints = new List<Transform>();
    Vector3[] meleePreviousPositions;

    Coroutine changeParent_Coroutine;

    void Start()
    {
        //create melee points
        SetMeleePoints();
    }

    private void OnValidate()
    {
        //update melee points
        if(update)
        {
            update = false;
            SetMeleePoints();
        }
    }

    #region private API

    #region melee points

    void SetMeleePoints()
    {
        //reset 
        ResetMeleePoints();

        //get offset between melee points 
        //(example: top is [0,100,0], bottom is [0,0,0], tracesPerSwing is 1 -> 100 / 2 = 50, the center between them)
        Vector3 length = topMeleePoint.position - bottomMeleePoint.position;
        float offsetBetweenMeleePoints = length.magnitude / (tracesPerSwing + 1);

        //instantiate every melee point
        InstantiateEveryMeleePoint(offsetBetweenMeleePoints, length.normalized);

        //create previous position for every melee point
        CreateMeleePreviousPositions();
    }

    void ResetMeleePoints()
    {
        if (meleePoints != null)
        {
            //remove any older melee point
            foreach (Transform melee in meleePoints)
            {
                //if not null, not top melee, not bottom melee
                if (melee != null && melee != topMeleePoint && melee != bottomMeleePoint)
                    Destroy(melee.gameObject);
            }
        }

        //clear list
        meleePoints = new List<Transform>();
    }

    void InstantiateEveryMeleePoint(float offsetBetweenMeleePoints, Vector3 direction)
    {
        //add bottom melee point
        if (useAlsoBottomMelee)
            meleePoints.Add(bottomMeleePoint);

        //for every trace
        for(int i = 0; i < tracesPerSwing; i++)
        {
            //get offset and position starting from bottom melee point
            float offsetTrace = (i + 1) * offsetBetweenMeleePoints;
            Vector3 position = bottomMeleePoint.position + (direction * offsetTrace);

            //instantiate and set transform position and parent
            GameObject meleePoint = new GameObject("Trace " + (i + 1));
            meleePoint.transform.position = position;
            meleePoint.transform.SetParent(transform);

            //add melee point
            meleePoints.Add(meleePoint.transform);
        }

        //add top melee point
        if (useAlsoTopMelee)
            meleePoints.Add(topMeleePoint);
    }

    #endregion

    #region melee previous positions

    void CreateMeleePreviousPositions()
    {
        //reset array
        meleePreviousPositions = new Vector3[meleePoints.Count];

        //and update previous positions
        UpdateMeleePreviousPositions();
    }

    void UpdateMeleePreviousPositions()
    {
        //foreach melee point, update previous position
        for (int i = 0; i < meleePoints.Count; i++)
        {
            meleePreviousPositions[i] = meleePoints[i].position;
        }
    }

    #endregion

    #region general

    IEnumerator ChangeParent_Coroutine(Transform newParent, float timeToWait, float durationLerpWeaponPosition)
    {
        //wait
        yield return new WaitForSeconds(timeToWait);

        //set parent
        transform.SetParent(newParent);

        //start
        float delta = 0;
        Vector3 startPosition = transform.localPosition;
        Quaternion startRotation = transform.localRotation;

        //lerp position
        while(delta < 1)
        {
            delta += Time.deltaTime / durationLerpWeaponPosition;

            transform.localPosition = Vector3.Lerp(startPosition, Vector3.zero, delta);
            transform.localRotation = Quaternion.Lerp(startRotation, Quaternion.identity, delta);

            yield return null;
        }

        //set final
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    #endregion

    #endregion

    #region public API

    public void UpdatePosition(Vector3 position, Quaternion rotation)
    {
        //used before instead of Change Parent - you can call it in update if you don't want to change weapon parent
        transform.position = position;
        transform.rotation = rotation;
    }

    public RaycastHit[] Attack(bool onlyResetPositions = false)
    {
        RaycastHit[] hits = new RaycastHit[meleePoints.Count];

        //if only reset positions, don't get hits
        if (onlyResetPositions == false)
        {
            //else foreach melee point, get hits
            for (int i = 0; i < meleePoints.Count; i++)
            {
                Physics.Linecast(meleePreviousPositions[i], meleePoints[i].position, out hits[i], CreateLayer.LayerOnly("Enemy"), QueryTriggerInteraction.Ignore);

                //draw debug
                if(durationDebugTraces > 0)
                    Debug.DrawLine(meleePreviousPositions[i], meleePoints[i].position, Color.red, durationDebugTraces);
            }
        }

        //update melee positions and return every hit
        UpdateMeleePreviousPositions();
        return hits;
    }

    public void ChangeParent(Transform newParent, float timeToWait, float durationLerpWeaponPosition)
    {
        if (changeParent_Coroutine != null)
            StopCoroutine(changeParent_Coroutine);

        //start change parent coroutine
        changeParent_Coroutine = StartCoroutine(ChangeParent_Coroutine(newParent, timeToWait, durationLerpWeaponPosition));
    }

    #endregion
}
