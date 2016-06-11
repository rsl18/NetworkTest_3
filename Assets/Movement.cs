using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class Movement :NetworkBehaviour {

    bool isHost;

	Transform head;
	Transform leftHand;
	Transform rightHand;

	GameObject cameraRig;


	//Lerping
	public float headLerpRate;
	public float handLerpRate;
	public float threshold;

	Transform syncHead;
	Transform syncLeft;
	Transform syncRight;


	// Use this for initialization
	void Start () {

        isHost = GameObject.Find("NetworkManager").GetComponent<CustomManager>().isHost;

        if (isLocalPlayer)
        {
            cameraRig = GameObject.Find("[CameraRig]");
            cameraRig.transform.position = transform.position;
            cameraRig.transform.rotation = transform.rotation;
        }

		head = transform.GetChild (0);
		leftHand = transform.GetChild (1);
		rightHand = transform.GetChild (2);

		syncHead = head.transform;
		syncLeft = leftHand.transform;
		syncRight = rightHand.transform;
	}
		
	void FixedUpdate () {
		ReadCameraRig ();
		LerpTransforms ();

	}


	void ReadCameraRig()
	{
		if (isLocalPlayer) 
		{
            Debug.Log("READCAMERA");
			Transform cameraHead = cameraRig.transform.GetChild (2);
			Transform cameraLeft = cameraRig.transform.GetChild (0);
			Transform cameraRight = cameraRig.transform.GetChild (1);

			head.position = cameraHead.position;
			head.localRotation = cameraHead.localRotation;
			leftHand.position = cameraLeft.position;
			leftHand.localRotation = cameraLeft.localRotation;
			rightHand.position = cameraRight.position;
			rightHand.localRotation = cameraRight.localRotation;

            Debug.Log(cameraHead.position);

			CmdSendTransforms (head.position, head.localRotation, leftHand.position, leftHand.localRotation, rightHand.position, rightHand.localRotation);
		}
	}



	[Command]
	void CmdSendTransforms (Vector3 headpos, Quaternion headrot, Vector3 leftpos, Quaternion leftrot, Vector3 rightpos, Quaternion rightrot)
	{
		syncHead.position = headpos;
		syncHead.localRotation = headrot;
		syncLeft.position = leftpos;
		syncLeft.localRotation = leftrot;
		syncRight.position = rightpos;
		syncRight.localRotation = rightrot;
        Debug.Log("ServerTransform");

		RpcUpdateTransforms (headpos, headrot, leftpos, leftrot, rightpos, rightrot);
	}


	[ClientRpc]
	void RpcUpdateTransforms(Vector3 headpos, Quaternion headrot, Vector3 leftpos, Quaternion leftrot, Vector3 rightpos, Quaternion rightrot)
	{
		if (!isLocalPlayer && !isHost)
        {
            if (Vector3.Distance(syncHead.position, headpos) > 0)
            {
                syncHead.position = headpos;
                syncHead.localRotation = headrot;
            }

            if (Vector3.Distance(syncLeft.position, leftpos) > 0)
            {
                syncLeft.position = leftpos;
                syncLeft.localRotation = leftrot;
            }

            if (Vector3.Distance(syncRight.position, rightpos) > 0)
            {
                syncRight.position = rightpos;
                syncRight.localRotation = rightrot;
            }
		}
	}

	void LerpTransforms()
	{
		if (!isLocalPlayer && !isHost) 
		{
            Debug.Log("LerpTransforms");
            if (Vector3.Distance(syncHead.position, head.position) > threshold)
            {
                head.position = Vector3.Lerp(head.position, syncHead.position, Time.fixedDeltaTime * headLerpRate);
                head.localRotation = Quaternion.Lerp(head.localRotation, syncHead.localRotation, Time.fixedDeltaTime * headLerpRate);
            }

            if (Vector3.Distance(syncLeft.position, leftHand.position) > threshold)
            {
                Debug.Log("Lerp");
                leftHand.position = Vector3.Lerp(leftHand.position, syncLeft.position, Time.fixedDeltaTime * handLerpRate);
                leftHand.localRotation = Quaternion.Lerp(leftHand.localRotation, syncLeft.localRotation, Time.fixedDeltaTime * handLerpRate);
            }

            if (Vector3.Distance(syncRight.position, rightHand.position) > threshold)
            {
                Debug.Log("Lerp");
                rightHand.position = Vector3.Lerp(rightHand.position, syncRight.position, Time.fixedDeltaTime * handLerpRate);
                rightHand.localRotation = Quaternion.Lerp(rightHand.localRotation, syncRight.localRotation, Time.fixedDeltaTime * handLerpRate);
            }
		}
	}
}
