using UnityEngine;

namespace Game.Terrain
{
	public class HexMapCamera : MonoBehaviour
	{

		public float stickMinZoom, stickMaxZoom;

		public float swivelMinZoom, swivelMaxZoom;

		public float moveSpeedMinZoom, moveSpeedMaxZoom;

		public float rotationSpeed;

		public Camera mainCamera;

		Transform swivel, stick;

		public HexGrid grid;

		float zoom = 1f;

		float rotationAngle;

		private bool inForcedMove;
		private Vector3 forcedMovePos;

		public static HexMapCamera instance;

		public static bool Locked
		{
			set
			{
				instance.enabled = !value;
			}
		}

		public static void ValidatePosition()
		{
			instance.AdjustPosition(0f, 0f);
		}

		public static void CenterPosition()
		{
			instance.CenterCamera();
		}

		public static void PanToCell(HexCell cell)
        {
			instance.inForcedMove = true;
			instance.forcedMovePos = cell.Position;
        }

		void Awake()
		{
			swivel = transform.GetChild(0);
			stick = swivel.GetChild(0);
		}

		void OnEnable()
		{
			instance = this;
		}

		void Update()
		{
			if (!inForcedMove)
			{
				float zoomDelta = Input.GetAxis("Mouse ScrollWheel");
				if (zoomDelta != 0f)
				{
					AdjustZoom(zoomDelta);
				}

				float rotationDelta = Input.GetAxis("Rotation");
				if (rotationDelta != 0f)
				{
					AdjustRotation(rotationDelta);
				}

				float xDelta = Input.GetAxis("Horizontal");
				float zDelta = Input.GetAxis("Vertical");
				if (xDelta != 0f || zDelta != 0f)
				{
					AdjustPosition(xDelta, zDelta);
				}
			}
			else
            {
				var currentPosition = transform.localPosition;
				float xDelta = forcedMovePos.x - currentPosition.x;
				float zDelta = forcedMovePos.z - currentPosition.z;
				if(Mathf.Approximately(xDelta, 0.0f) && Mathf.Approximately(zDelta, 0.0f))
                {
					inForcedMove = false;
                }
				else
                {
					AdjustPosition(xDelta, zDelta);
                }
			}
		}

		void AdjustZoom(float delta)
		{
			zoom = Mathf.Clamp01(zoom + delta);

			float distance = Mathf.Lerp(stickMinZoom, stickMaxZoom, zoom);
			stick.localPosition = new Vector3(0f, 0f, distance);

			float angle = Mathf.Lerp(swivelMinZoom, swivelMaxZoom, zoom);
			swivel.localRotation = Quaternion.Euler(angle, 0f, 0f);
		}

		void AdjustRotation(float delta)
		{
			rotationAngle += delta * rotationSpeed * Time.deltaTime;
			if (rotationAngle < 0f)
			{
				rotationAngle += 360f;
			}
			else if (rotationAngle >= 360f)
			{
				rotationAngle -= 360f;
			}
			transform.localRotation = Quaternion.Euler(0f, rotationAngle, 0f);
		}

		void AdjustPosition(float xDelta, float zDelta)
		{
			Vector3 direction =
				transform.localRotation *
				new Vector3(xDelta, 0f, zDelta).normalized;
			float damping = Mathf.Max(Mathf.Abs(xDelta), Mathf.Abs(zDelta));
			float distance =
				Mathf.Lerp(moveSpeedMinZoom, moveSpeedMaxZoom, zoom) *
				damping * Time.deltaTime;

			Vector3 position = transform.localPosition;
			position += direction * distance;
			transform.localPosition = ClampPosition(position);
		}

		Vector3 ClampPosition(Vector3 position)
		{
			float xMax = (grid.cellCountX - 0.5f) * (2f * HexMetrics.innerRadius);
			position.x = Mathf.Clamp(position.x, 0f, xMax);

			float zMax = (grid.cellCountZ - 1) * (1.5f * HexMetrics.outerRadius);
			position.z = Mathf.Clamp(position.z, 0f, zMax);

			return position;
		}

		void CenterCamera()
		{
			var center = GetCenterPosition();
			var currentPosition = transform.localPosition;
			transform.localPosition = new Vector3(center.x - currentPosition.x, currentPosition.y, center.z - currentPosition.z);
			var zoomDelta = (stick.transform.localPosition.z - center.y);
			var modDelta = zoomDelta / (stickMaxZoom - stickMinZoom);
			AdjustZoom(-1 * modDelta);
		}

		Vector3 GetCenterPosition()
		{
			float xHalf = ((grid.cellCountX - 0.5f) * (2f * HexMetrics.innerRadius))/2;
			float zHalf = ((grid.cellCountZ - 1) * (1.5f * HexMetrics.outerRadius))/2;
			return new Vector3(xHalf, (stickMinZoom + stickMaxZoom)/2, zHalf);
		}
	}
}