using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour, Destroyable {
    public float cooldown;
    public Material playerMaterial, defaultEnemyMaterial;
    public float aggroDistance;
    public GameObject markerTemplate;
    protected GameObject player;
    GameObject marker;
    public bool displayMarker = true;
    protected bool isFiring = false;
    public static bool scrambled = false;
    public delegate void OnEnemyDestroyedAction(EnemyController e);
    public static event OnEnemyDestroyedAction OnEnemyDestroyed;
    public bool isMovingVariant;
    public Rigidbody rootRB;
    public float moveSpeed = 0, moveCooldown = 2;

	// Use this for initialization
	protected void Start () {
        player = PlayerController.player;
        if (displayMarker) {
            marker = Instantiate(markerTemplate, MapGenerator.instance.radar.transform);
            MeshGenerator.markers.Add(marker.GetComponent<Image>());
            StartCoroutine(MarkerLoop());
        }
        if (isMovingVariant) {
            StartCoroutine(MoveLoop());
        }
	}

    IEnumerator MoveLoop() {
        float lastJump = Time.time;
        while (true) {
            if ( Time.time - lastJump > moveCooldown && (isFiring || Aggro())) {
                rootRB.velocity = (( player.transform.position - transform.position ).normalized * moveSpeed);
                lastJump = Time.time;
            }
            yield return null;
        }
    }

    IEnumerator MarkerLoop() {
        while (true) {
            if (!IsOnScreen() && isFiring) {
                marker.SetActive(true);
                marker.GetComponent<RectTransform>().anchoredPosition = CalculateMarkerScreenPosition();

            } else {
                marker.SetActive(false);
            }
            yield return null;
        }
    }

    public virtual void Delete() {
        Destroy(gameObject);
    }

    bool IsOnScreen() {
        Vector3 vPoint = Camera.main.WorldToViewportPoint(transform.position);
        if (vPoint.x < 0 || vPoint.y < 0 || vPoint.x > 1 || vPoint.y > 1) {
            return false;
        } else {
            return true;
        }
    }

    Vector3 screenMiddle = new Vector3(0.5f, 0.5f), lastResult = Vector3.one;

    Vector3 CalculateMarkerScreenPosition() {
        Vector3 enemyPoint = Camera.main.WorldToViewportPoint(transform.position);
        Vector3 hypotenuse;
        if (Mathf.Abs(enemyPoint.x) > Mathf.Abs(enemyPoint.y)) {
            if (enemyPoint.x > enemyPoint.y) {
                hypotenuse = DoMath(enemyPoint, Vector3.right);
            } else {
                hypotenuse = DoMath(enemyPoint, Vector3.left);
            }
        } else {
            if (enemyPoint.y > enemyPoint.x) {
                hypotenuse = DoMath(enemyPoint, Vector3.up);
            } else {
                hypotenuse = DoMath(enemyPoint, Vector3.down);
            }
        }
        marker.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, Vector2.SignedAngle(new Vector3(-1, 1).normalized, hypotenuse.normalized)));
        Vector3 result = screenMiddle + hypotenuse;
        lastResult = result;
        return Camera.main.ViewportToScreenPoint(result);
    }

    Vector3 DoMath(Vector3 enemyPoint, Vector3 comparison) {
        Vector3 difference = enemyPoint - screenMiddle;
        Vector3 direction = new Vector3(difference.x, difference.y, 0f).normalized;
        float preAngle = Vector3.SignedAngle(comparison, direction, Vector3.forward);
        float angle = (90 - preAngle) * Mathf.PI / 180;
        return direction * .5f / Mathf.Sin(angle);
    }

    public bool Aggro() {
        Vector3 viewportPosition = Camera.main.WorldToViewportPoint(transform.position);
        return ( transform.position - player.transform.position ).sqrMagnitude < aggroDistance &&
                (viewportPosition.x >= 0 && viewportPosition.x <= 1 && viewportPosition.y >= 0 && viewportPosition.y <= 1);
    }

    protected IEnumerator EnemyAILoop() {
        float lastShot = 0;
        while (true) {
            if (Time.time - lastShot > cooldown && Aggro() && !isFiring) {
                lastShot = Time.time;
                StartCoroutine(FireSequence());
            }
            yield return null;
        }
    }

    public virtual IEnumerator FireSequence() {
        yield return null;
    }

    private void OnDestroy() {
        if (marker != null) {
            MeshGenerator.markers.Remove(marker.GetComponent<Image>());
            Destroy(marker);
        }
    }

    void SetFiringStatus(bool status) {
        isFiring = status;
    }

    public void Destroy() {
        if (OnEnemyDestroyed != null) {
            OnEnemyDestroyed(this);
        }
        Delete();
    }
}
