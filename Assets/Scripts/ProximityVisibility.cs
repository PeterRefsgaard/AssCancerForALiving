using UnityEngine;
using Google.XR.ARCoreExtensions;
using UnityEngine.XR.ARFoundation;
using TMPro;
using UnityEngine.Android;
using System.Collections;

public class ProximityVisibility : MonoBehaviour
{
    public Unity.XR.CoreUtils.XROrigin xrOrigin;
    public AREarthManager earthManager;
    public GameObject targetObject; // fx din cube
    public double targetLatitude;
    public double targetLongitude;
    public double targetAltitude = 0;
    public float visibleDistanceMeters = 5f;
    public TextMeshProUGUI distanceText;

    void Start()
    {
        StartCoroutine(RequestPermissions());
    }

    IEnumerator RequestPermissions()
    {
        // Request CAMERA permission
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);
            yield return null; // Wait a frame before checking again
        }

        // Request FINE LOCATION permission
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
            yield return null;
        }

        // Wait until both permissions are granted
        while (!Permission.HasUserAuthorizedPermission(Permission.Camera) ||
               !Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            yield return null;
        }

        // Start location services
        Input.location.Start();

        // Optional: Wait until location is initialized
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (Input.location.status != LocationServiceStatus.Running)
        {
            Debug.LogWarning("Location services failed to start.");
        }
        else
        {
            Debug.Log("Location services started successfully.");
        }
    }

    void Update()
    {
        Debug.Log($"[GPS DEBUG] ARSession.state: {ARSession.state}");
        Debug.Log($"[GPS DEBUG] EarthTrackingState: {earthManager.EarthTrackingState}");
        Debug.Log($"[GPS DEBUG] Geospatial pose: Lat {earthManager.CameraGeospatialPose.Latitude}, Lon {earthManager.CameraGeospatialPose.Longitude}");

        if (ARSession.state != ARSessionState.SessionTracking ||
            earthManager.EarthTrackingState != UnityEngine.XR.ARSubsystems.TrackingState.Tracking)
        {
            targetObject.SetActive(false);
            if (distanceText != null) distanceText.text = "Searching for GPS...";
            return;
        }

        var camPose = earthManager.CameraGeospatialPose;

        double distance = HaversineDistance(
            camPose.Latitude, camPose.Longitude,
            targetLatitude, targetLongitude);

        float verticalDiff = Mathf.Abs((float)(camPose.Altitude - targetAltitude));

        double totalDistance = System.Math.Sqrt(distance * distance + verticalDiff * verticalDiff);

        if (distanceText != null)
            distanceText.text = $"Afstand: {totalDistance:F1} m";

        targetObject.SetActive(totalDistance <= visibleDistanceMeters);
    }

    double HaversineDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371000; // Radius of Earth in meters
        double dLat = Mathf.Deg2Rad * (float)(lat2 - lat1);
        double dLon = Mathf.Deg2Rad * (float)(lon2 - lon1);

        double a =
            System.Math.Sin(dLat / 2) * System.Math.Sin(dLat / 2) +
            System.Math.Cos(Mathf.Deg2Rad * (float)lat1) *
            System.Math.Cos(Mathf.Deg2Rad * (float)lat2) *
            System.Math.Sin(dLon / 2) * System.Math.Sin(dLon / 2);

        double c = 2 * System.Math.Atan2(System.Math.Sqrt(a), System.Math.Sqrt(1 - a));

        return R * c; // distance in meters
    }
}
