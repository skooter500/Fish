using System.Linq;
using Gamelogic;
using Gamelogic.Colors;
using UnityEngine;

public class LightScene : GLMonoBehaviour
{
	public Light[] lights;
	public Light light1;
	public Light light2;
	public Graph graph;
	public InspectableColorNode mainNode;
	public InspectableColorNode lightNode1;
	public InspectableColorNode lightNode2;

	public void Start()
	{
		Reset();
	}

	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			Reset();
		}
	}

	public void Reset()
	{
		graph.Recompute();
		
		light1.color = lightNode1.Colors.First();
		light2.color = lightNode2.Colors.First();
		var colorGenerator = mainNode.Colors.ToPeriodicGenerator();

		foreach (var pointLight in lights)
		{
			pointLight.color = colorGenerator.Next();
		}
	}
}
