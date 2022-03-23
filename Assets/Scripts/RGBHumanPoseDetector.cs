using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Unity.Barracuda;
using Vuforia;

public class RGBHumanPoseDetector : MonoBehaviour
{

    // Public Script parameters
    public bool RunOnGPU = true;
    public NNModel modelAsset;
    public float TrackingThreshold = 0.3f;
    public int[] m_TrackableJoints = { 7, 10, 11, 12, 13, 14, 15 };

    /*JOINTS    
    0 - r ankle   8 - upper neck 
    1 - r knee    9 - head top 
    2 - r hip     10 - r wrist
    3 - l hip     11 - r elbow
    4 - l knee    12 - r shoulder
    5 - l ankle   13 - l shoulder 
    6 - pelvis    14 - l elbow 
    7 - thorax    15 - l wrist
    */

    // Definitions
    private Camera m_cam;
    private float m_screen_width, m_screen_heigth;
    private int img_width, img_heigth;
    private IWorker m_worker;
    PixelFormat m_pixelFormat;
    List<GameObject> sphere_list;

    // training statistics for PoseNet
    private float[] mean = { 0.485f, 0.456f, 0.406f };
    private float[] std = { 0.229f, 0.224f, 0.225f };

    // Start is called before the first frame update
    void Start()
    {

        // Initialize the tracking indicators
        sphere_list = new List<GameObject>();
        for (int joint_id = 0; joint_id < m_TrackableJoints.Length; joint_id++)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            var cubeRenderer = sphere.GetComponent<Renderer>();
            cubeRenderer.material.SetColor("_Color", Color.red);

            sphere_list.Add(sphere);
        }

        // Initializations
        m_cam = Camera.main;
        m_screen_width = Screen.width;
        m_screen_heigth = Screen.height;
        img_width = 256;
        img_heigth = 256;

        // Load model and worker in Barracuda
        Model model = ModelLoader.Load(modelAsset);

        if (RunOnGPU) { m_worker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, model); }
        else { m_worker = WorkerFactory.CreateWorker(WorkerFactory.Type.CSharpBurst, model); }

        // Set PyTorch model format
        ComputeInfo.channelsOrder = ComputeInfo.ChannelsOrder.NCHW;

        // Register vuforia callbacks
        VuforiaApplication.Instance.OnVuforiaStarted += OnVuforiaStarted;
        VuforiaBehaviour.Instance.World.OnStateUpdated += OnVuforiaUpdated;

    }

    private void OnVuforiaStarted()
    {
        // Initialize the camera
        m_pixelFormat = PixelFormat.RGB888;
        bool success = VuforiaBehaviour.Instance.CameraDevice.SetFrameFormat(m_pixelFormat, true);

        if (success)
        {
            Debug.Log("Successfully registered pixel format " + m_pixelFormat.ToString());
        }
        else
        {
            Debug.Log("Failed to register pixel format " + m_pixelFormat.ToString());
        }
    }

    void OnVuforiaUpdated()
    {
        // Retrieve the camera image
        Image image = VuforiaBehaviour.Instance.CameraDevice.GetCameraImage(m_pixelFormat);

        if (image.Width != 0)
        {
            // Copy image to Texture2D and resize to model input format
            Texture2D targetTexture = new Texture2D(img_width, img_heigth, TextureFormat.RGB24, false);
            image.CopyToTexture(targetTexture, false);
            targetTexture = ScaleTexture(targetTexture, img_width, img_heigth);
            targetTexture.Apply();

            // Save texture to PNG for debugging
            // byte[] bytes = targetTexture.EncodeToPNG();
            // File.WriteAllBytes(Application.dataPath + "/../SavedScreen.png", bytes);

            // Convert texture to Barracuda tensor
            Tensor inputs = new Tensor(targetTexture);

            // Run the model
            m_worker.Execute(inputs);
            Tensor output = m_worker.PeekOutput();

            float from_val;
            int from_x;
            int from_y;

            // Get tracking results for the joints defined in m_joints
            for (int joint_id = 0; joint_id < m_TrackableJoints.Length; joint_id++)
            {
                from_val = 0f;
                from_x = 0;
                from_y = 0;

                for (int i = 0; i < 64; i++)
                {
                    for (int j = 0; j < 64; j++)
                    {
                        if (output[0, i, j, m_TrackableJoints[joint_id]] > from_val)
                        {
                            from_val = output[0, i, j, m_TrackableJoints[joint_id]];
                            from_x = j;
                            from_y = i;
                        }
                    }
                }

                // Debug.Log("Max Val: " + from_val + "\nLocation Height: " + from_y + "\nLocation Width: " + from_x);

                // Render tracking indicator at tracked position
                Vector3 point = new Vector3(m_screen_width * from_x / 64, m_screen_heigth - m_screen_heigth * from_y / 64, 40f);
                Vector3 world_point = m_cam.ScreenToWorldPoint(point);

                if (from_val > TrackingThreshold)
                {
                    var sphere_renderer = sphere_list[joint_id].GetComponent<Renderer>();
                    sphere_renderer.enabled = true;
                    sphere_list[joint_id].transform.position = world_point;
                }
                else
                {
                    var sphere_renderer = sphere_list[joint_id].GetComponent<Renderer>();
                    sphere_renderer.enabled = false;
                }
            }

            // Clean up Tensors
            inputs.Dispose();
            output.Dispose();

        }

    }

    private Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, true);
        Color[] rpixels = result.GetPixels(0);
        float incX = (1.0f / (float)targetWidth);
        float incY = (1.0f / (float)targetHeight);
        for (int px = 0; px < rpixels.Length; px++)
        {
            rpixels[px] = source.GetPixelBilinear(incX * ((float)px % targetWidth), incY * ((float)Mathf.Floor(px / targetWidth)));
        }
        result.SetPixels(rpixels, 0);
        result.Apply();
        return result;
    }

}
