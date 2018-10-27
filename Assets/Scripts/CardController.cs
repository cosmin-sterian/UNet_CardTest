using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CardController : NetworkBehaviour {
	private Vector3 _firstPos;
	private Vector3 _screenPoint;
	private NetworkIdentity netview;
	private GameObject _parentGameObject;

	public GameObject PlayerSpawn;
	public GameObject EnemySpawn;

	private static bool _cardAdded;

	// Use this for initialization
	void Start () {
		// Debug.Log(name);
		PlayerSpawn = GameObject.Find("SpawnPosPlayer");
		EnemySpawn = GameObject.Find("SpawnPosEnemy");
		_parentGameObject = transform.parent.gameObject;
		// _firstPos = transform.position;
		_screenPoint = Camera.main.WorldToScreenPoint(transform.position);
		netview = GetComponentInParent<NetworkIdentity>();

		if (_cardAdded) {
			// This is the new card
			transform.position -= new Vector3(3, 0 ,0);
			// TODO: Find out why it doesn't move :|
		}
		
		// Debug.Log(netview.isLocalPlayer);

		if (netview != null && netview.isLocalPlayer) {
			GetComponent<MeshRenderer>().material.color = Color.blue;
		}

		if (netview != null && netview.isServer) {
			GetComponent<Transform>().position = PlayerSpawn.transform.position;
		} else {
			GetComponent<Transform>().position = EnemySpawn.transform.position;
		}
		
		AddNewCard();
	}

	private void AddNewCard() {
		// Simulate adding new cards
		if (_cardAdded)
			return;
		_cardAdded = true;
		_parentGameObject.SetActive(false); // Trick required to bypass NetTransfChild AddComponent bug(null pointer)
		NetworkTransformChild newComponent = _parentGameObject.AddComponent<NetworkTransformChild>();
		GameObject newCard = (GameObject) Instantiate(gameObject, transform.parent);
		newComponent.target = newCard.transform;
		newComponent.enabled = true;
		Vector3 offset = new Vector3(-3, 0, 0);
		newCard.transform.position = newCard.transform.position + offset;
		_parentGameObject.SetActive(true);
		Debug.Log("new card coords: " + newCard.transform.position);
	}

	private void OnMouseDown() {
		Debug.Log(name);
	}

	private void OnMouseDrag() {
		// Debug.Log("You clicked on " + name);
		if (netview == null || !netview.isLocalPlayer) // Check if localPlayer in parent network identity
			return;
		
		Vector3 cursScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _screenPoint.z);
		Vector3 cursPosition = Camera.main.ScreenToWorldPoint(cursScreenPoint);
		transform.position = cursPosition;
	}
}
