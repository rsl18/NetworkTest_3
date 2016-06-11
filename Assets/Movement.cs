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
	public float headLerpPosRate;
    public float headLerpRotRate;
    public float handLerpPosRate;
    public float handLerpRotRate;
    public float threshold;

	Vector3 syncHeadPos;
    Vector3 syncLeftPos;
    Vector3 syncRightPos;
    Quaternion syncHeadRot;
    Quaternion syncLeftRot;
    Quaternion syncRightRot;


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

        syncHeadPos = head.position;
		syncLeftPos = leftHand.position;
		syncRightPos = rightHand.position;
        syncHeadRot = head.rotation;
        syncLeftRot = leftHand.rotation;
        syncRightRot = rightHand.rotation;
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

			head.position = cameraHead.position;
			head.localRotation = cameraHead.localRotation;
			leftHand.position = cameraLeft.position;
			leftHand.localRotation = cameraLeft.localRotation;
			rightHand.position = cameraRight.position;
			rightHand.localRotation = cameraRight.localRotation;

			CmdSendTransforms (head.position, head.localRotation, leftHand.position, leftHand.localRotation, rightHand.position, rightHand.localRotation);
		}
	}



	[Command]
	void CmdSendTransforms (Vector3 headpos, Quaternion headrot, Vector3 leftpos, Quaternion leftrot, Vector3 rightpos, Quaternion rightrot)
	{
		syncHeadPos = headpos;
		syncHeadRot = headrot;
		syncLeftPos = leftpos;
		syncLeftRot = leftrot;
		syncRightPos = rightpos;
		syncRightRot = rightrot;

		RpcUpdateTransforms (headpos, headrot, leftpos, leftrot, rightpos, rightrot);
	}


	[ClientRpc]
	void RpcUpdateTransforms(Vector3 headpos, Quaternion headrot, Vector3 leftpos, Quaternion leftrot, Vector3 rightpos, Quaternion rightrot)
	{
		if (!isLocalPlayer && !isHost)
        {
            if (Vector3.Distance(syncHeadPos, headpos) > 0)
            {
                syncHeadPos= headpos;
            }

            if (Quaternion.Angle(syncHeadRot, headrot) > 0)
            {
                syncHeadRot = headrot;
            }

            if (Vector3.Distance(syncLeftPos, leftpos) > 0)
            {
                syncLeftPos = leftpos;
            }

            if (Quaternion.Angle(syncLeftRot, leftrot) > 0)
            {
                syncLeftRot = leftrot;
            }

            if (Vector3.Distance(syncRightPos, rightpos) > 0)
            {
                syncRightPos = rightpos;
            }

            if (Quaternion.Angle(syncRightRot, rightrot) > 0)
            {
                syncRightRot = rightrot;
            }
        }
	}

	void LerpTransforms()
	{
		if (!isLocalPlayer && !isHost) 
		{
            Debug.Log("LerpTransforms");
            if (Vector3.Distance(syncHeadPos, head.position) > threshold)
            {
                head.position = Vector3.Lerp(head.position, syncHeadPos, Time.fixedDeltaTime * headLerpPosRate);
            }

            if (Quaternion.Angle(syncHeadRot, head.rotation) > threshold)
            {
                head.rotation = Quaternion.Lerp(head.localRotation, syncHeadRot, Time.fixedDeltaTime * headLerpRotRate);
            }

            if (Vector3.Distance(syncLeftPos, leftHand.position) > threshold)
            {
                leftHand.position = Vector3.Lerp(leftHand.position, syncLeftPos, Time.fixedDeltaTime * handLerpPosRate);
            }

            if (Quaternion.Angle(syncLeftRot, leftHand.rotation) > threshold)
            {
                leftHand.rotation = Quaternion.Lerp(leftHand.localRotation, syncLeftRot, Time.fixedDeltaTime * handLerpRotRate);
            }

            if (Vector3.Distance(syncRightPos, rightHand.position) > threshold)
            {
                rightHand.position = Vector3.Lerp(rightHand.position, syncRightPos, Time.fixedDeltaTime * handLerpPosRate);
            }

            if (Quaternion.Angle(syncRightRot, rightHand.rotation) > threshold)
            {
                rightHand.rotation = Quaternion.Lerp(rightHand.localRotation, syncRightRot, Time.fixedDeltaTime * handLerpRotRate);
            }
        }
	}
}
