using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Serialization;

public class HeadMovement : MonoBehaviour
{
    bool ikActive = true;
    public float lookAtWeight = 1;

    public Transform lookObj;
    private Animator _animator;

    private GameObject _headPivot;
    public bool lookAway;
    [SerializeField] private float targetWeight = 0.1f;

    public MultiAimConstraint LeftEyeConstraint;
    public MultiAimConstraint RightEyeConstraint;

    // Start is called before the first frame update
    private void Start()
    {
        _animator = GetComponent<Animator>();

        _headPivot = new GameObject("HeadPivot")
        {
            transform =
            {
                parent = transform,
                localPosition = new Vector3(0, 1.4f, 0)
            }
        };

        InvokeRepeating(nameof(NewTarget), 0, 2);
    }

    private void NewTarget()
    {
        targetWeight = Random.Range(0.2f, 1.0f);
        // make look away more likely to be true than false
        lookAway = Random.Range(0, 10) > 6;
    }

    // Update is called once per frame
    private void Update()
    {
        _headPivot.transform.LookAt(lookObj);
        var angle = _headPivot.transform.localRotation.y;

        lookAtWeight = angle < 0.65 && angle > -0.65
            ? Mathf.Lerp(lookAtWeight, targetWeight, Time.deltaTime * 2.5f)
            : Mathf.Lerp(lookAtWeight, 0, Time.deltaTime * 2.5f);

        if (lookAway)
        {
            LeftEyeConstraint.weight = lookAtWeight;
            RightEyeConstraint.weight = lookAtWeight;
        }
        else if (!lookAway)
        {
            LeftEyeConstraint.weight = 1 - lookAtWeight;
            RightEyeConstraint.weight = 1 - lookAtWeight;
        }
    }

    public void OnAnimatorIK()
    {
        if (_animator)
        {
            if (ikActive)
            {
                if (lookObj != null)
                {
                    _animator.SetLookAtWeight(lookAtWeight, 0.2f, 0.9f, 0.5f);
                    _animator.SetLookAtPosition(lookObj.position);
                }
            }
            else
            {
                _animator.SetLookAtWeight(0);
            }
        }
    }
}
