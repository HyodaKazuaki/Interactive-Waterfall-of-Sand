using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DepthCollision : MonoBehaviour
{

	// background image display instance
	public Image backgroundImage;

	//Physics Material
	public PhysicMaterial physicMaterial;

	// the KinectManager instance
	private KinectManager manager;

	// the foreground texture
	//private Texture2D foregroundTex;

	// rectangle taken by the foreground texture (in pixels)
	private Rect foregroundRect;
	private Vector2 foregroundOfs;

	// game objects to contain the joint colliders
	private GameObject[] jointColliders = null;

	// Use this for initialization
	void Start()
	{
		// calculate the foreground rectangle
		Rect cameraRect = Camera.main.pixelRect;
		float rectHeight = cameraRect.height;
		float rectWidth = cameraRect.width;

		if (rectWidth > rectHeight)
			rectWidth = rectHeight * KinectWrapper.Constants.DepthImageWidth / KinectWrapper.Constants.DepthImageHeight;
		else
			rectHeight = rectWidth * KinectWrapper.Constants.DepthImageHeight / KinectWrapper.Constants.DepthImageWidth;

		foregroundOfs = new Vector2((cameraRect.width - rectWidth) / 2, (cameraRect.height - rectHeight) / 2);
		foregroundRect = new Rect(foregroundOfs.x, cameraRect.height - foregroundOfs.y, rectWidth, -rectHeight);

		// create joint colliders
		int numColliders = (int)KinectWrapper.NuiSkeletonPositionIndex.Count;
		jointColliders = new GameObject[numColliders * 2];

		for (int i = 0; i < numColliders * 2; i++)
		{
			string sColObjectName = ((KinectWrapper.NuiSkeletonPositionIndex)(i % numColliders)).ToString() + "Collider";
			jointColliders[i] = new GameObject(sColObjectName);
			jointColliders[i].transform.parent = transform;

			CapsuleCollider collider = jointColliders[i].AddComponent<CapsuleCollider>();
			collider.radius = 0.85f;
			collider.direction = 2; // direction to z axis
			collider.height = 3.5f;
			collider.material = physicMaterial;
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (manager == null)
		{
			manager = KinectManager.Instance;
		}

		// get the users texture
		if (manager && manager.IsInitialized())
		{
			//foregroundTex = manager.GetUsersLblTex();

			if (backgroundImage && (backgroundImage.sprite == null))
			{
				//backgroundImage.texture = manager.GetUsersClrTex();
				backgroundImage.sprite = Sprite.Create(manager.GetUsersClrTex(), new Rect(0, 0, manager.GetUsersClrTex().width, manager.GetUsersClrTex().height), Vector2.zero);
				backgroundImage.rectTransform.sizeDelta = new Vector2(manager.GetUsersClrTex().width, manager.GetUsersClrTex().height);
			}
		}

		if (manager.IsUserDetected())
		{
			uint[] userId = new uint[2] { manager.GetPlayer1ID(), manager.GetPlayer2ID() };

			// update colliders
			int numColliders = (int)KinectWrapper.NuiSkeletonPositionIndex.Count;

			for (int j = 0; j < 2; j++)
			{
				for (int i = 0; i < numColliders; i++)
				{
					if (manager.IsJointTracked(userId[j], i))
					{
						Vector3 posJoint = manager.GetRawSkeletonJointPos(userId[j], i);

						if (posJoint != Vector3.zero)
						{
							// convert the joint 3d position to depth 2d coordinates
							Vector2 posDepth = manager.GetDepthMapPosForJointPos(posJoint);

							float scaledX = posDepth.x * foregroundRect.width / KinectWrapper.Constants.DepthImageWidth;
							float scaledY = posDepth.y * -foregroundRect.height / KinectWrapper.Constants.DepthImageHeight;

							float screenX = foregroundOfs.x + scaledX;
							float screenY = Camera.main.pixelHeight - (foregroundOfs.y + scaledY);
							float zDistance = posJoint.z - Camera.main.transform.position.z;

							Vector3 posScreen = new Vector3(screenX, screenY, zDistance);
							Vector3 posCollider = Camera.main.ScreenToWorldPoint(posScreen);

							jointColliders[i + j * numColliders].transform.position = posCollider;
						}
					}
				}
			}
		}
	}
}
