using Sirenix.OdinInspector;
using UnityEngine;
public class ViewSkeleton : MonoBehaviour
{

	[OnValueChanged("PopulateChildren")]
	public Transform rootNode;
	public Transform[] childNodes;

	void OnDrawGizmos()
	{
		if (rootNode != null)
		{
			if (childNodes == null)
			{
				//get all joints to draw
				PopulateChildren();
			}


			foreach (Transform child in childNodes)
			{

				if (child == rootNode)
				{
					//list includes the root, if root then larger, green cube
					Gizmos.color = Color.green;
					Gizmos.DrawCube(child.position, new Vector3(.1f, .1f, .1f));
				}
				else
				{
					Gizmos.color = Color.blue;
					Gizmos.DrawLine(child.position, child.parent.position);
					Gizmos.DrawCube(child.position, new Vector3(.01f, .01f, .01f));
				}
			}

		}
	}

	public void PopulateChildren()
	{
		childNodes = rootNode.GetComponentsInChildren<Transform>();
	}
}