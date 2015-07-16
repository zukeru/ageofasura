using UnityEngine;
using System.Collections;

public enum AnchorPoint {
	TopLeft,
	TopRight,
	BottomLeft,
	BottomRight,
	Top,
	Bottom,
	Left,
	Right
}



public abstract class AtavismWindowTemplate : MonoBehaviour {
	
	public GUISkin skin;
	public AnchorPoint anchor;
	public Vector2 anchorOffset;
	public float width;
	public float height;
	public string frameName;
	public int uiLayer = 3;
	protected bool open = false;

	protected Rect uiRect;

	// Use this for initialization
	void Start () {
		
	}
	
	/// <summary>
	/// Setups the ui rect used for the class that inherits from this class.
	/// </summary>
	protected void SetupRect() {
		uiRect = SetupRect(anchor, anchorOffset.x, anchorOffset.y, width, height);
	}
	
	/// <summary>
	/// Creates and returns a Rect based on the anchor and sizes specified
	/// </summary>
	/// <returns>The rect.</returns>
	/// <param name="anchor">Anchor.</param>
	public static Rect SetupRect(AnchorPoint anchor, float offsetX, float offsetY, float width, float height) {
		Rect rect = new Rect();
		if (anchor == AnchorPoint.TopLeft) {
			rect = new Rect(offsetX, offsetY, width, height);
		} else if (anchor == AnchorPoint.TopRight) {
			rect = new Rect(Screen.width - width - offsetX, offsetY, width, height);
		} else if (anchor == AnchorPoint.BottomLeft) {
			rect = new Rect(offsetX, Screen.height - height - offsetY, width, height);
		} else if (anchor == AnchorPoint.BottomRight) {
			rect = new Rect(Screen.width - width - offsetX, Screen.height - height - offsetY, width, height);
		} else if (anchor == AnchorPoint.Top) {
			rect = new Rect(((Screen.width - width) / 2) + offsetX, offsetY, width, height);
		} else if (anchor == AnchorPoint.Bottom) {
			rect = new Rect(((Screen.width - width) / 2) + offsetX, Screen.height - height - offsetY, width, height);
		} else if (anchor == AnchorPoint.Left) {
			rect = new Rect(offsetX, ((Screen.height - height) / 2) + offsetY, width, height);
		} else if (anchor == AnchorPoint.Right) {
			rect = new Rect(Screen.width - width - offsetX, ((Screen.height - height) / 2) + offsetY, width, height);
		}
		return rect;
	}

	public void ToggleOpen() {
		open = !open;
		if (open) {
			AtavismUiSystem.AddFrame(frameName, uiRect);
		} else {
			AtavismUiSystem.RemoveFrame(frameName, new Rect(0, 0, 0, 0));
		}
	}
}

