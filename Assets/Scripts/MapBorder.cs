using UnityEngine;
using System.Collections;

public class MapBorder : MonoBehaviour {

	void OnTriggerExit2D(Collider2D other) {
		if (other.tag == "bullet"){
            GameObject.Find(other.name).GetComponent<Bullet>().borderSwaps++;
			if ((other.transform.position.y > (GetComponent<BoxCollider2D>().transform.position.y + (GetComponent<BoxCollider2D>().size.y / 2)))){
				other.transform.position = new Vector3(other.transform.position.x, ((GetComponent<BoxCollider2D>().offset.y - (GetComponent<BoxCollider2D>().size.y / 2) + 0.1F)), 0);
			}
			else if ((other.transform.position.y < ((GetComponent<BoxCollider2D>().offset.y - GetComponent<BoxCollider2D>().size.y / 2)))){
				other.transform.position = new Vector3(other.transform.position.x, ((GetComponent<BoxCollider2D>().offset.y + (GetComponent<BoxCollider2D>().size.y / 2) - 0.1F)), 0);
			}
			else if ((other.transform.position.x > (GetComponent<BoxCollider2D>().transform.position.x + (GetComponent<BoxCollider2D>().size.x / 2)))){
				other.transform.position = new Vector3(((GetComponent<BoxCollider2D>().offset.x - (GetComponent<BoxCollider2D>().size.x / 2) + 0.1F)), other.transform.position.y, 0);
			}
			else if ((other.transform.position.x < ((GetComponent<BoxCollider2D>().offset.x - GetComponent<BoxCollider2D>().size.x / 2)))){
				other.transform.position = new Vector3(((GetComponent<BoxCollider2D>().offset.x + (GetComponent<BoxCollider2D>().size.x / 2) - 0.1F)),other.transform.position.y, 0);
			}
		}
		if (other.tag == "player_child"){
			if ((other.transform.parent.transform.position.y > (GetComponent<BoxCollider2D>().transform.position.y + (GetComponent<BoxCollider2D>().size.y / 2)))){
				other.transform.parent.transform.position = new Vector3(other.transform.transform.parent.position.x, ((GetComponent<BoxCollider2D>().offset.y - (GetComponent<BoxCollider2D>().size.y / 2) + 0.1F)), 0);
			}
			else if ((other.transform.parent.transform.position.y < ((GetComponent<BoxCollider2D>().offset.y - GetComponent<BoxCollider2D>().size.y / 2)))){
				other.transform.parent.transform.position = new Vector3(other.transform.transform.parent.position.x, ((GetComponent<BoxCollider2D>().offset.y + (GetComponent<BoxCollider2D>().size.y / 2) - 0.1F)), 0);
			}
			else if ((other.transform.parent.transform.position.x > (GetComponent<BoxCollider2D>().transform.position.x + (GetComponent<BoxCollider2D>().size.x / 2)))){
				other.transform.parent.transform.position = new Vector3(((GetComponent<BoxCollider2D>().offset.x - (GetComponent<BoxCollider2D>().size.x / 2) + 0.1F)), other.transform.parent.transform.position.y, 0);
			}
			else if ((other.transform.parent.transform.position.x < ((GetComponent<BoxCollider2D>().offset.x - GetComponent<BoxCollider2D>().size.x / 2)))){
				other.transform.parent.transform.position = new Vector3(((GetComponent<BoxCollider2D>().offset.x + (GetComponent<BoxCollider2D>().size.x / 2) - 0.1F)),other.transform.parent.transform.position.y, 0);
			}

            GameObject go = GameObject.Find(other.transform.parent.name);
            player playerGO = go.GetComponent<player>();
            playerGO.borderSwaps++;
		}
	}
}