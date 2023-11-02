using Cysharp.Threading.Tasks.Triggers;
using NobleConnect;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace Bonkers
{
    public class RacoonCode : MonoBehaviour
    {
        GameObject player;
        Rigidbody rb;

        [SerializeField]
        float racoonSpeed = 10f;
        float lifetime = 4;

        bool isScaling = false;

        GameObject bin;
        GameObject racoon1;
        GameObject racoon2;
        GameObject racoon3;

        private void Awake()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            rb = GetComponent<Rigidbody>();
            transform.rotation = player.transform.rotation;
            bin = this.gameObject.transform.GetChild(3).gameObject;

            StartCoroutine(scaleOverTime(bin.gameObject.transform, new Vector3(2, 2, 2), 4f));
            racoon1 = this.gameObject.transform.GetChild(0).gameObject;
            racoon2 = this.gameObject.transform.GetChild(1).gameObject;
            racoon3 = this.gameObject.transform.GetChild(2).gameObject;
            racoon1.gameObject.transform.position = this.gameObject.transform.position + ((transform.forward * -1) * 3) + ((transform.right * -1) * 2);
            racoon2.gameObject.transform.position = this.gameObject.transform.position + ((transform.forward * -1) * 3) + (transform.right * 2);
            racoon3.gameObject.transform.position = this.gameObject.transform.position + ((transform.forward * -1) * 3) + (transform.right * 1);
            //StartCoroutine(racoonsToBin());
        }
        private void Update()
        {
            if (!isScaling)
            {
                rb.velocity = transform.forward * racoonSpeed;
            }

            if (isScaling)
            {
                var step = 1.0f * Time.deltaTime;
                racoon1.transform.position = Vector3.MoveTowards(racoon1.transform.position, bin.transform.position, step);
                racoon1.transform.LookAt(bin.transform, Vector3.up);
                racoon2.transform.position = Vector3.MoveTowards(racoon2.transform.position, bin.transform.position, step);
                racoon2.transform.LookAt(bin.transform, Vector3.up);
                racoon3.transform.position = Vector3.MoveTowards(racoon3.transform.position, bin.transform.position, step);
                racoon3.transform.LookAt(bin.transform, Vector3.up);
            }

        }

        IEnumerator scaleOverTime(Transform objectToScale, Vector3 toScale, float duration)
        {
            if (isScaling)
            {
                yield break;
            }
            isScaling = true;

            float counter = 0;

            Vector3 startScaleSize = objectToScale.localScale;

            while (counter < duration)
            {
                counter += Time.deltaTime;
                objectToScale.localScale = Vector3.Lerp(startScaleSize, toScale, counter / duration);
                yield return null;
            }
            // put running trigger here
            StartCoroutine(Lifetime());
            isScaling = false;
        }

        //IEnumerator racoonsToBin()
        //{
        //    while (isScaling)
        //    {
        //        racoon1.transform.position = Vector3.MoveTowards(transform.position, bin.transform.position, 0.02f);
        //        racoon1.transform.LookAt(bin.transform, Vector3.up);
        //        racoon2.transform.position = Vector3.MoveTowards(transform.position, bin.transform.position, 0.02f);
        //        racoon2.transform.LookAt(bin.transform, Vector3.up);
        //        racoon3.transform.position = Vector3.MoveTowards(transform.position, bin.transform.position, 0.02f);
        //        racoon3.transform.LookAt(bin.transform, Vector3.up);
        //    }
        //    return null;
        //}

        IEnumerator Lifetime()
        {
            yield return new WaitForSeconds(lifetime);
            Destroy(this.gameObject);
        }
    }
}
