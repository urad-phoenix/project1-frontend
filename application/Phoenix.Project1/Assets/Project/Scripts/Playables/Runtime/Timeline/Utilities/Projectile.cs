using UnityEngine;
namespace Phoenix.Playables.Utilities
{
    [ExecuteInEditMode]
    public class Projectile : MonoBehaviour
    {
        public GameObject bullet;
        public Transform start;   //要到达的目标
        public Transform target;   //要到达的目标  
        public AnimationCurve Mycurve;

        public float heightscale = 1.84f;
        public bool localheight;
        public float duration = 0.4f;
        public float axis_offset = 0;
        Vector3 distanceToTarget;
        Vector3 defaultposition;
        Quaternion defaultrotation;
        Vector3 pre_temp_position;
        Vector3 original_position;
        Quaternion original_rotation;
        public float mytime = 0;
        public bool isTimeline;
        private bool move = true;
        float progess;
        [HideInInspector]
        public bool isEnd;
        [HideInInspector]
        public bool isStart;

        private void OnEnable()
        {
            if(!isTimeline)
            {
                Initializ();
            }

            // StartCoroutine(StartShoot());
        }

        public void Initializ()
        {
            if(bullet == null)
            {
                bullet = gameObject;
            }

            original_rotation = bullet.transform.rotation;
            original_position = bullet.transform.position;

            if(start != null)
            {
                bullet.transform.position = start.position;
            }

            if(localheight == true)
            {
                bullet.transform.LookAt(target.transform.position);
            }
            bullet.transform.localEulerAngles += new Vector3(0, 0, axis_offset);
            defaultposition = bullet.transform.position;
            defaultrotation = bullet.transform.rotation;
            //计算两者之间的距离  
            distanceToTarget = target == null ? Vector3.zero : target.transform.position - bullet.transform.position;
            isEnd = false;
            isStart = true;
            move = true;
        }

        private void OnDisable()
        {
            bullet.transform.position = original_position;
            bullet.transform.rotation = original_rotation;
            mytime = 0;
            move = true;
        }
        void Update()
        {
            if(!isTimeline)
            {
                if(move)
                    Shoot(mytime);
            }
        }

        public void Shoot(float my_time)
        {
            mytime = my_time;
            pre_temp_position = bullet.transform.position;

            mytime += Time.deltaTime;

            progess = (mytime / duration);
            if(progess > 0.98f)
            {
                move = false;
            }
            Vector3 temp_position = defaultposition;
            temp_position += (progess) * distanceToTarget;
            bullet.transform.position = temp_position;
            bullet.transform.rotation = defaultrotation;
            temp_position.y = (Mycurve.Evaluate(progess)) * heightscale;
            transform.Translate(Vector3.up * (Mycurve.Evaluate(progess)) * heightscale, Space.Self);
            transform.forward = (bullet.transform.position - pre_temp_position).normalized;
            isEnd = true;
            isStart = false;
        }
    }
}