using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class Movement :NetworkBehaviour {

    public bool isHost;
    public bool isLocal;

	Transform head;
	Transform leftHand;
	Transform rightHand;

	GameObject cameraRig;


	//Lerping
	public float headLerpPosRate;
    public float headLerpRotRate;
    public float handLerpPosRate;
    public float handLerpRotRate;

    public float lerpThresholdPos;
    public float lerpThresholdRot;

    public float sendThresholdPos;
    public float sendThresholdRot;

	Vector3 syncHeadPos;
    Vector3 syncLeftPos;
    Vector3 syncRightPos;
    Quaternion syncHeadRot;
    Quaternion syncLeftRot;
    Quaternion syncRightRot;


    // Use this for initialization
    void Start () {

        isHost = GameObject.Find("NetworkManager").GetComponent<CustomManager>().isHost;
        isLocal = isLocalPlayer;

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

    void LerpTransforms()
    {
        if (!isLocalPlayer)
        {
            Debug.Log("LerpTransforms");
            if (Vector3.Distance(syncHeadPos, head.position) > lerpThresholdPos)
            {
                head.position = Vector3.Lerp(head.position, syncHeadPos, Time.fixedDeltaTime * headLerpPosRate);
            }

            if (Quaternion.Angle(syncHeadRot, head.rotation) > lerpThresholdRot)
            {
                head.rotation = Quaternion.Lerp(head.localRotation, syncHeadRot, Time.fixedDeltaTime * headLerpRotRate);
            }

            if (Vector3.Distance(syncLeftPos, leftHand.position) > lerpThresholdPos)
            {
                leftHand.position = Vector3.Lerp(leftHand.position, syncLeftPos, Time.fixedDeltaTime * handLerpPosRate);
            }

            if (Quaternion.Angle(syncLeftRot, leftHand.rotation) > lerpThresholdRot)
            {
                leftHand.rotation = Quaternion.Lerp(leftHand.localRotation, syncLeftRot, Time.fixedDeltaTime * handLerpRotRate);
            }

            if (Vector3.Distance(syncRightPos, rightHand.position) > lerpThresholdPos)
            {
                rightHand.position = Vector3.Lerp(rightHand.position, syncRightPos, Time.fixedDeltaTime * handLerpPosRate);
            }

            if (Quaternion.Angle(syncRightRot, rightHand.rotation) > lerpThresholdRot)
            {
                rightHand.rotation = Quaternion.Lerp(rightHand.localRotation, syncRightRot, Time.fixedDeltaTime * handLerpRotRate);
            }
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
		if (!isLocalPlayer)
        {
            if (Vector3.Distance(syncHeadPos, headpos) > sendThresholdPos)
            {
                syncHeadPos= headpos;
            }

            if (Quaternion.Angle(syncHeadRot, headrot) > sendThresholdRot)
            {
                syncHeadRot = headrot;
            }

            if (Vector3.Distance(syncLeftPos, leftpos) > sendThresholdPos)
            {
                syncLeftPos = leftpos;
            }

            if (Quaternion.Angle(syncLeftRot, leftrot) > sendThresholdRot)
            {
                syncLeftRot = leftrot;
            }

            if (Vector3.Distance(syncRightPos, rightpos) > sendThresholdPos)
            {
                syncRightPos = rightpos;
            }

            if (Quaternion.Angle(syncRightRot, rightrot) > sendThresholdRot)
            {
                syncRightRot = rightrot;
            }
        }
	}


}
