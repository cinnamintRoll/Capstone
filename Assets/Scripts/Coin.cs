using BNG;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public float MinForce = 0.1f;
    public float LastRelativeVelocity = 0;
    public float LastDamageForce = 0;

    private void OnCollisionEnter(Collision collision)
    {
        if (!this.isActiveAndEnabled) return;

        OnCollisionEvent(collision);
    }

    public virtual void OnCollisionEvent(Collision collision)
    {
        LastDamageForce = collision.impulse.magnitude;
        LastRelativeVelocity = collision.relativeVelocity.magnitude;

        if (LastDamageForce >= MinForce)
        {
            // Access PointsManager via singleton
            if (PointsManager.Instance != null)
            {
                PointsManager.Instance.ModifyPoints(1); // Add 1 point
                Debug.Log("Coin collected! Points added.");
                this.gameObject.SetActive(false);
            }
        }
    }
}
