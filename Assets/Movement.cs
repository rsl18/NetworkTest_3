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
			Transform cameraHead = cameraRig.transform.GetChild (2);
			Transform cameraLeft = cameraRig.transform.GetChild (0);
			Transform cameraRight = cameraRig.transform.GetChild (1);

			head.localPosition = cameraHead.localPosition;
			head.localRotation = cameraHead.localRotation;
			leftHand.localPosition = cameraLeft.localPosition;
			leftHand.localRotation = cameraLeft.localRotation;
			rightHand.localPosition = cameraRight.localPosition;
			rightHand.localRotation = cameraRight.localRotation;

			CmdSendTransforms (head.localPosition, head.localRotation, leftHand.localPosition, leftHand.localRotation, rightHand.localPosition, rightHand.localRotation);
		}
	}



	[Command]
	void CmdSendTransforms (Vector3 headpos, Quaternion headrot, Vector3 leftpos, Quaternion leftrot, Vector3 rightpos, Quaternion rightrot)
	{
		syncHead.localPosition = headpos;
		syncHead.localRotation = headrot;
		syncLeft.localPosition = leftpos;
		syncLeft.localRotation = leftrot;
		syncRight.localPosition = rightpos;
		syncRight.localRotation = rightrot;
        Debug.Log("ServerTransform");

		RpcUpdateTransforms (headpos, headrot, leftpos, leftrot, rightpos, rightrot);
	}


	[ClientRpc]
	void RpcUpdateTransforms(Vector3 headpos, Quaternion headrot, Vector3 leftpos, Quaternion leftrot, Vector3 rightpos, Quaternion rightrot)
	{
		if (!isLocalPlayer && !isHost)
        {
            if (Vector3.Distance(syncHead.localPosition, headpos) > threshold)
            {
                syncHead.localPosition = headpos;
                syncHead.localRotation = headrot;
            }

            if (Vector3.Distance(syncLeft.localPosition, leftpos) > threshold)
            {
                syncLeft.localPosition = leftpos;
                syncLeft.localRotation = leftrot;
            }

            if (Vector3.Distance(syncRight.localPosition, rightpos) > threshold)
            {
                syncRight.localPosition = rightpos;
                syncRight.localRotation = rightrot;
            }
		}
	}

	void LerpTransforms()
	{
		if (!isLocalPlayer && !isHost) 
		{
            if (Vector3.Distance(syncHead.localPosition, head.localPosition) > threshold)
            {
                head.localPosition = Vector3.Lerp(head.localPosition, syncHead.localPosition, Time.fixedDeltaTime * headLerpRate);
                head.localRotation = Quaternion.Lerp(head.localRotation, syncHead.localRotation, Time.fixedDeltaTime * headLerpRate);
            }

            if (Vector3.Distance(syncLeft.localPosition, leftHand.localPosition) > threshold)
            {
                leftHand.localPosition = Vector3.Lerp(leftHand.localPosition, syncLeft.localPosition, Time.fixedDeltaTime * handLerpRate);
                leftHand.localRotation = Quaternion.Lerp(leftHand.localRotation, syncLeft.localRotation, Time.fixedDeltaTime * handLerpRate);
            }

            if (Vector3.Distance(syncRight.localPosition, rightHand.localPosition) > threshold)
            {
                rightHand.localPosition = Vector3.Lerp(rightHand.localPosition, syncRight.localPosition, Time.fixedDeltaTime * handLerpRate);
                rightHand.localRotation = Quaternion.Lerp(rightHand.localRotation, syncRight.localRotation, Time.fixedDeltaTime * handLerpRate);
            }
		}
	}
}
