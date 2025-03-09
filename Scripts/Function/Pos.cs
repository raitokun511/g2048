using UnityEngine;
using System.Collections;

public class Pos
{

	public static int OUT_ON_LEFT = 1;
	public static int OUT_ON_TOP = 2;
	public static int OUT_ON_RIGHT = 3;
	public static int OUT_ON_BOTTOM = 4;
	public static int OUT = 5;
	public static int IN_BE_BOUND = 6;
	public static int IN_BOUND = 7;
	public static int COLLIDE = 8;


	public static bool Contains(Vector3 rectPosition, Vector3 rectSize, Vector2 position)
	{
		if (position.x >= (rectPosition.x - rectSize.x / 2.0f) && position.x <= (rectPosition.x + rectSize.x / 2.0f))
			if (position.y >= (rectPosition.y - rectSize.y / 2.0f) && position.y <= (rectPosition.y + rectSize.y / 2.0f))
				return true;

		return false;

	}
	public static bool Contains(GameObject obj, Vector2 position)
	{
		if (obj.GetComponent<SpriteRenderer>())
		{
			return Contains(obj.transform.position, obj.GetComponent<SpriteRenderer>().size, position);
		}
		else if (obj.GetComponent<RectTransform>())
		{
			Vector3 realPanelPos = RectTransformUtility.CalculateRelativeRectTransformBounds(GameObject.Find("Canvas").transform, obj.transform).center;
			Vector3 panelWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(obj.transform.position.x, obj.transform.position.y, 10));
			Vector3 panelWorldSize = Camera.main.ScreenToWorldPoint(obj.GetComponent<RectTransform>().rect.size);
			Vector2 posToWorldPoint = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));

			Vector2 screenPos2D = position;
			Vector2 anchoredPos;
			Debug.Log("Inside " + RectTransformUtility.ScreenPointToLocalPointInRectangle(obj.GetComponent<RectTransform>(), screenPos2D, Camera.main, out anchoredPos));



			Debug.Log("Real " + panelWorldPos + "     " + panelWorldSize + "     " + posToWorldPoint);
			return Contains(realPanelPos, obj.GetComponent<RectTransform>().rect.size, position);
		}
		return false;
	}

	public static int Collider(Vector3 object1Pos, Vector3 object1Rect, Vector3 object2Pos, Vector3 object2Rect)
	{

		if (object1Pos.x + object1Rect.x / 2 < object2Pos.x - object2Rect.x / 2)
			return Pos.OUT_ON_LEFT;
		if (object1Pos.x - object1Rect.x / 2 > object2Pos.x + object2Rect.x / 2)
			return Pos.OUT_ON_RIGHT;
		if (object1Pos.y + object1Rect.y / 2 < object2Pos.y - object2Rect.y / 2)
			return Pos.OUT_ON_BOTTOM;
		if (object1Pos.y - object1Rect.y / 2 > object2Pos.y + object2Rect.y / 2)
			return Pos.OUT_ON_TOP;



		Vector3 TopRight1 = new Vector3(object1Pos.x + object1Rect.x / 2, object1Pos.y + object1Rect.y / 2, 0);
		Vector3 TopLeft1 = new Vector3(object1Pos.x - object1Rect.x / 2, object1Pos.y + object1Rect.y / 2, 0);
		Vector3 BottomRight1 = new Vector3(object1Pos.x + object1Rect.x / 2, object1Pos.y - object1Rect.y / 2, 0);
		Vector3 BottomLeft1 = new Vector3(object1Pos.x - object1Rect.x / 2, object1Pos.y - object1Rect.y / 2, 0);

		Vector3 TopRight2 = new Vector3(object2Pos.x + object2Rect.x / 2, object2Pos.y + object2Rect.y / 2, 0);
		Vector3 TopLeft2 = new Vector3(object2Pos.x - object2Rect.x / 2, object2Pos.y + object2Rect.y / 2, 0);
		Vector3 BottomRight2 = new Vector3(object2Pos.x + object2Rect.x / 2, object2Pos.y - object2Rect.y / 2, 0);
		Vector3 BottomLeft2 = new Vector3(object2Pos.x - object2Rect.x / 2, object2Pos.y - object2Rect.y / 2, 0);

		bool inTopRight = Pos.Contains(object2Pos, object2Rect, TopRight1);
		bool inTopLeft = Pos.Contains(object2Pos, object2Rect, TopLeft1);
		bool inBottomRight = Pos.Contains(object2Pos, object2Rect, BottomRight1);
		bool inBottomLeft = Pos.Contains(object2Pos, object2Rect, BottomLeft1);
		bool inMiddle = Pos.Contains(object2Pos, object2Rect, object1Pos);
		if (inTopRight && inTopLeft && inBottomLeft && inBottomRight && inMiddle)
			return Pos.IN_BE_BOUND;
		else
		{
			if (Pos.Contains(object1Pos, object1Rect, TopRight2) &&
				Pos.Contains(object1Pos, object1Rect, TopLeft2) &&
				Pos.Contains(object1Pos, object1Rect, BottomRight2) &&
				Pos.Contains(object1Pos, object1Rect, BottomLeft2) &&
				!inTopRight && !inTopLeft && !inBottomLeft && !inBottomRight && !inMiddle)
				return Pos.IN_BOUND;
		}
		return Pos.COLLIDE;
	}

	public static int LightCollider(Vector3 lightPos, Vector3 lightRect, Vector3 objectPos, Vector3 objectRect)
	{
		if (lightPos.x + lightRect.x / 2 >= objectPos.x - objectRect.x / 2 && lightPos.x - lightRect.x / 2 <= objectPos.x + objectRect.x / 2
			&& lightPos.y + lightRect.y / 2 >= objectPos.y - objectRect.y / 2 && lightPos.y - lightRect.y / 2 <= objectPos.y + objectRect.y / 2)
			return Pos.COLLIDE;
		return Pos.OUT;
	}



}