using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour 
{
	[SerializeField]
	private MeshRenderer m_renderer;
	private MaterialPropertyBlock m_materialPropertyBlock;

	private Action<string> m_commandCallback;

	private Vector3 m_realPosition = new Vector3();

	private Dictionary<int, Vector3> m_positionBuffer = new Dictionary<int, Vector3>();

	public void SetCommandSentCallback(Action<string> callback) 
	{
		m_commandCallback = callback;
	}

	private void Start()
	{
		m_materialPropertyBlock = new MaterialPropertyBlock();
		// Get the current value of the material properties in the renderer.
        m_renderer.GetPropertyBlock(m_materialPropertyBlock);
        // Assign our new value.
        m_materialPropertyBlock.SetColor("_Color", UnityEngine.Random.ColorHSV(0, 1, 1, 1, 0, 1, 1, 1));
        // Apply the edited values to the renderer.
        m_renderer.SetPropertyBlock(m_materialPropertyBlock);
	}

	public void SetPosition(Vector3 position, int sequence)
	{
		Vector3 bufferedPosition;
		if(!m_positionBuffer.TryGetValue(sequence, out bufferedPosition) || bufferedPosition != position) {
			Debug.Log("Corrected Position: Got sequence: " + sequence + " position: " + position + " bufferedPosition: " + bufferedPosition);
			m_realPosition = position;
		}
	}

	public void AddLocalPosition(Command command)
	{
		Vector3 localPosition;

		switch(command.command)
		{
			case "UP":
				localPosition = m_realPosition + new Vector3(0, 0, 1);
				break;
			case "DOWN":
				localPosition = m_realPosition + new Vector3(0, 0, -1);
				break;
			case "RIGHT":
				localPosition = m_realPosition + new Vector3(1, 0, 0);
				break;
			case "LEFT":
				localPosition = m_realPosition + new Vector3(-1, 0, 0);
				break;
			default: 
				return;
		}
		
		m_realPosition = localPosition;
		Debug.Log("Sequence: " + command.sequence + " localPosition: " + localPosition);
		m_positionBuffer[command.sequence] = localPosition;
	}

	private void Update() 
	{
		if((transform.position - m_realPosition).magnitude > 0.01f)
		{
			transform.position = Vector3.MoveTowards(transform.position, m_realPosition, 10 * Time.deltaTime);
		}

		if(this.m_commandCallback == null)
		{
			return;
		}

		if(Input.GetKeyDown(KeyCode.UpArrow)) 
		{
			this.m_commandCallback("UP");
			return;
		}
		
		if(Input.GetKeyDown(KeyCode.DownArrow)) 
		{
			this.m_commandCallback("DOWN");
			return;
		}

		if(Input.GetKeyDown(KeyCode.LeftArrow)) 
		{
			this.m_commandCallback("LEFT");
			return;
		}

		if(Input.GetKeyDown(KeyCode.RightArrow)) 
		{
			this.m_commandCallback("RIGHT");
			return;
		}
	}
}
