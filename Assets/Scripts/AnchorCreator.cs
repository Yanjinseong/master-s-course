using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.Barracuda;
using System.Linq;
using System;
using UnityEngine.UI;

public class AnchorCreator : MonoBehaviour
{

    [SerializeField]
    public NNModel modelFile_Cup_seramic;
    static public Model _runtimeModel_Cup_ceramic;
    static public IWorker _engine_Cup_ceramic;

    public NNModel modelFile_Cup_paper;
    static public Model _runtimeModel_Cup_paper;
    static public IWorker _engine_Cup_paper;

    public NNModel modelFile_Ball_soccer;
    static public Model _runtimeModel_Ball_soccer;
    static public IWorker _engine_Ball_soccer;

    public NNModel modelFile_Can;
    static public Model _runtimeModel_Can;
    static public IWorker _engine_Can;

    public NNModel modelFile_Box;
    static public Model _runtimeModel_Box;
    static public IWorker _engine_Box;

    public NNModel modelFile_Bottle_plastic;
    static public Model _runtimeModel_Bottle_plastic;
    static public IWorker _engine_Bottle_plastic;

    public NNModel modelFile_Bottle_glass;
    static public Model _runtimeModel_Bottle_glass;
    static public IWorker _engine_Bottle_glass;

    public NNModel modelFile_Waste_container;
    static public Model _runtimeModel_Waste_container;
    static public IWorker _engine_Waste_container;

    public NNModel modelFile_Mouse;
    static public Model _runtimeModel_Mouse;
    static public IWorker _engine_Mouse;

    public NNModel modelFile_Phone;
    static public Model _runtimeModel_Phone;
    static public IWorker _engine_Phone;

    public float depth = 0f;
    public float frustumHeight = 0f;
    public float frustumWidth = 0f;
    public float object_width = 0f;
    public float object_height = 0f;
    public double w = 0f;
    public double h = 0f;



    public void RemoveAllAnchors()
    {
        Debug.Log($"DEBUG: Removing all anchors ({anchorDic.Count})");
        foreach (var anchor in anchorDic)
        {
            Destroy(anchor.Key.gameObject);
        }
        s_Hits.Clear();
        anchorDic.Clear();
    }

    void Awake()
    {
        m_RaycastManager = GetComponent<ARRaycastManager>();
        m_AnchorManager = GetComponent<ARAnchorManager>();
        GameObject cameraImage = GameObject.Find("Camera Image");
        phoneARCamera = cameraImage.GetComponent<PhoneARCamera>();
    }

    private void Start()
    {
        _runtimeModel_Cup_ceramic = ModelLoader.Load(modelFile_Cup_seramic);
        _runtimeModel_Cup_paper = ModelLoader.Load(modelFile_Cup_paper);
        _runtimeModel_Ball_soccer = ModelLoader.Load(modelFile_Ball_soccer);
        _runtimeModel_Can = ModelLoader.Load(modelFile_Can);
        _runtimeModel_Box = ModelLoader.Load(modelFile_Box);
        _runtimeModel_Bottle_plastic = ModelLoader.Load(modelFile_Bottle_plastic);
        _runtimeModel_Bottle_glass = ModelLoader.Load(modelFile_Bottle_glass);
        _runtimeModel_Waste_container = ModelLoader.Load(modelFile_Waste_container);
        _runtimeModel_Mouse = ModelLoader.Load(modelFile_Mouse);
        _runtimeModel_Phone = ModelLoader.Load(modelFile_Phone);
    }

    ARAnchor CreateAnchor(in ARRaycastHit hit)
    {
        // TODO: create plane anchor

        // create a regular anchor at the hit pose
        Debug.Log($"DEBUG: Creating regular anchor. distance: {hit.distance}. session distance: {hit.sessionRelativeDistance} type: {hit.hitType}.");
        return m_AnchorManager.AddAnchor(hit.pose);
    }

    private bool Pos2Anchor(float x, float y, BoundingBox outline)
    {
        
        if (m_RaycastManager.Raycast(new Vector2(x, y), s_Hits, trackableTypes))
        {
            // Raycast hits are sorted by distance, so the first one will be the closest hit.
            var hit = s_Hits[0];
            depth = hit.distance * 100;
            //TextMesh anchorObj = GameObject.Find("New Text").GetComponent<TextMesh>();
            // Create a new anchor
            Debug.Log("Creating Anchor");

            var width = outline.Dimensions.Width * this.scaleFactor;
            var height = outline.Dimensions.Height * this.scaleFactor;

            frustumHeight = 2.0f * depth * Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad);
            frustumWidth = frustumHeight * Camera.main.aspect;
            object_height = ((height * frustumHeight) / Screen.height);
            object_width = ((width * frustumWidth) / Screen.width);

            w = Math.Truncate(object_width * 100) / 100;
            h = Math.Truncate(object_height * 100) / 100;

            if (outline.Label == "Cup(paper)")
            {
                _engine_Cup_paper = WorkerFactory.CreateWorker(WorkerFactory.Type.CSharpBurst, _runtimeModel_Cup_paper);

                var strideArray = new float[2] { object_width, object_height };

                var tensor = new Tensor(1, 2, strideArray);

                Tensor output = _engine_Cup_paper.Execute(tensor).PeekOutput();

                //anchorObj_mesh.text = $"w:{w} h:{h}\n{outline.Label}: {(int)(outline.Confidence * 100)}% weight: {output[0]}";
                anchorObj_mesh.text = $"w:{w}cm h:{h}cm\n weight: {output[0]}g";

                _engine_Cup_paper.Dispose();
                output.Dispose();
            }
            else if (outline.Label == "Ball(soccer)")
            {
                //_runtimeModel_Ball_soccer = ModelLoader.Load(modelFile_Ball_soccer);

                _engine_Ball_soccer = WorkerFactory.CreateWorker(WorkerFactory.Type.CSharpBurst, _runtimeModel_Ball_soccer);

                var strideArray = new float[2] { object_width, object_height };

                var tensor = new Tensor(1, 2, strideArray);

                Tensor output = _engine_Ball_soccer.Execute(tensor).PeekOutput();

                //anchorObj_mesh.text = $"w:{w} h:{h}\n{outline.Label}: {(int)(outline.Confidence * 100)}% weight: {output[0]}";
                //anchorObj_mesh.text = $"w:{w}cm h:{h}cm\n weight: {output[0]}g";
                anchorObj_mesh.text = $"w:{w}cm h:{h}cm\n";

                _engine_Ball_soccer.Dispose();
                output.Dispose();
            }
            else if (outline.Label == "Cup(ceramic)")
            {

                _engine_Cup_ceramic = WorkerFactory.CreateWorker(WorkerFactory.Type.CSharpBurst, _runtimeModel_Cup_ceramic);

                var strideArray = new float[2] { object_width, object_height };

                var tensor = new Tensor(1, 2, strideArray);

                Tensor output = _engine_Cup_ceramic.Execute(tensor).PeekOutput();

                //anchorObj_mesh.text = $"w:{w} h:{h}\n{outline.Label}: {(int)(outline.Confidence * 100)}% weight: {output[0]}";
                anchorObj_mesh.text = $"w:{w}cm h:{h}cm\n weight: {output[0]}g";

                _engine_Cup_ceramic.Dispose();
                output.Dispose();
            }
            else if (outline.Label == "Ball(golf)")
            {
                //anchorObj_mesh.text = $"w:{w} h:{h}\n{outline.Label}: {(int)(outline.Confidence * 100)}% weight: 45.93";
                anchorObj_mesh.text = $"w:{w}cm h:{h}cm weight: 45.93g";
            }
            else if (outline.Label == "Ball(tennis)")
            {
                //anchorObj_mesh.text = $"w:{w} h:{h}\n{outline.Label}: {(int)(outline.Confidence * 100)}% weight: 54.66";
                anchorObj_mesh.text = $"w:{w}cm h:{h}cm\n weight: 54.66g";
            }
            else if (outline.Label == "Can")
            {
                _engine_Can = WorkerFactory.CreateWorker(WorkerFactory.Type.CSharpBurst, _runtimeModel_Can);

                var strideArray = new float[2] { object_width, object_height };

                var tensor = new Tensor(1, 2, strideArray);

                Tensor output = _engine_Can.Execute(tensor).PeekOutput();

                //anchorObj_mesh.text = $"w:{w} h:{h}\n{outline.Label}: {(int)(outline.Confidence * 100)}% weight: {output[0]}";
                anchorObj_mesh.text = $"w:{w}cm h:{h}cm\n weight: {output[0]}g";

                _engine_Can.Dispose();
                output.Dispose();
            }
            else if (outline.Label == "Box")
            {
                _engine_Box = WorkerFactory.CreateWorker(WorkerFactory.Type.CSharpBurst, _runtimeModel_Box);

                var strideArray = new float[2] { object_width, object_height };

                var tensor = new Tensor(1, 2, strideArray);

                Tensor output = _engine_Box.Execute(tensor).PeekOutput();

                //anchorObj_mesh.text = $"w:{w} h:{h}\n{outline.Label}: {(int)(outline.Confidence * 100)}% weight: {output[0]}";
                anchorObj_mesh.text = $"w:{w}cm h:{h}cm\n weight: {output[0]}g";

                _engine_Box.Dispose();
                output.Dispose();
            }
            else if (outline.Label == "Bottle(plastic)")
            {
                _engine_Bottle_plastic = WorkerFactory.CreateWorker(WorkerFactory.Type.CSharpBurst, _runtimeModel_Bottle_plastic);

                var strideArray = new float[2] { object_width, object_height };

                var tensor = new Tensor(1, 2, strideArray);

                Tensor output = _engine_Bottle_plastic.Execute(tensor).PeekOutput();

                //anchorObj_mesh.text = $"w:{w} h:{h}\n{outline.Label}: {(int)(outline.Confidence * 100)}% weight: {output[0]}";
                anchorObj_mesh.text = $"w:{w}cm h:{h}cm\n weight: {output[0]}g";

                _engine_Bottle_plastic.Dispose();
                output.Dispose();
            }
            else if (outline.Label == "Bottle(glass)")
            {
                _engine_Bottle_glass = WorkerFactory.CreateWorker(WorkerFactory.Type.CSharpBurst, _runtimeModel_Bottle_glass);

                var strideArray = new float[2] { object_width, object_height };

                var tensor = new Tensor(1, 2, strideArray);

                Tensor output = _engine_Bottle_glass.Execute(tensor).PeekOutput();

                //anchorObj_mesh.text = $"w:{w} h:{h}\n{outline.Label}: {(int)(outline.Confidence * 100)}% weight: {output[0]}";
                anchorObj_mesh.text = $"w:{w}cm h:{h}cm\n weight: {output[0]}g";

                _engine_Bottle_glass.Dispose();
                output.Dispose();
            }
            else if (outline.Label == "Waste(container)")
            {
                _engine_Waste_container = WorkerFactory.CreateWorker(WorkerFactory.Type.CSharpBurst, _runtimeModel_Waste_container);

                var strideArray = new float[2] { object_width, object_height };

                var tensor = new Tensor(1, 2, strideArray);

                Tensor output = _engine_Waste_container.Execute(tensor).PeekOutput();

                //anchorObj_mesh.text = $"w:{w} h:{h}\n{outline.Label}: {(int)(outline.Confidence * 100)}% weight: {output[0]}";
                anchorObj_mesh.text = $"w:{w}cm h:{h}cm\n weight: {output[0]}g";

                _engine_Waste_container.Dispose();
                output.Dispose();
            }
            else if (outline.Label == "Computer mouse")
            {
                _engine_Mouse = WorkerFactory.CreateWorker(WorkerFactory.Type.CSharpBurst, _runtimeModel_Mouse);

                var strideArray = new float[2] { object_width, object_height };

                var tensor = new Tensor(1, 2, strideArray);

                Tensor output = _engine_Mouse.Execute(tensor).PeekOutput();

                //anchorObj_mesh.text = $"w:{w} h:{h}\n{outline.Label}: {(int)(outline.Confidence * 100)}% weight: {output[0]}";
                anchorObj_mesh.text = $"w:{w}cm h:{h}cm\n weight: {output[0]}g";

                _engine_Mouse.Dispose();
                output.Dispose();
            }
            else if (outline.Label == "Mobile phone")
            {
                _engine_Phone = WorkerFactory.CreateWorker(WorkerFactory.Type.CSharpBurst, _runtimeModel_Phone);

                var strideArray = new float[2] { object_width, object_height };

                var tensor = new Tensor(1, 2, strideArray);

                Tensor output = _engine_Phone.Execute(tensor).PeekOutput();

                //anchorObj_mesh.text = $"w:{w} h:{h}\n{outline.Label}: {(int)(outline.Confidence * 100)}% weight: {output[0]}";
                anchorObj_mesh.text = $"w:{w}cm h:{h}cm\n weight: {output[0]}g";

                _engine_Phone.Dispose();
                output.Dispose();
            }
            else
            {
                //anchorObj_mesh.text = $"{outline.Label}: {(int)(outline.Confidence * 100)}%";
                anchorObj_mesh.text = $"w:{w}cm h:{h}cm";
            }

            var anchor = CreateAnchor(hit);
            if (anchor)
            {
                
                Debug.Log($"DEBUG: creating anchor. {outline}");
                // Remember the anchor so we can remove it later.
                anchorDic.Add(anchor, outline);
                Debug.Log($"DEBUG: Current number of anchors {anchorDic.Count}.");
                //PhoneARCamera.depth = hit.distance*100+2f;
                
                return true;
            }
            else
            {
                Debug.Log("DEBUG: Error creating anchor");
                return false;
            }

        }
        else
        {
            //Debug.Log("Couldn't raycast");
        }
        return false;
    }

    void Update()
    {
        // If bounding boxes are not stable, return directly without raycast
        if (!phoneARCamera.localization)
        {
            return;
        }

        boxSavedOutlines = phoneARCamera.boxSavedOutlines;
        shiftX = phoneARCamera.shiftX;
        shiftY = phoneARCamera.shiftY;
        scaleFactor = phoneARCamera.scaleFactor;
        // Remove outdated anchor that is not in boxSavedOutlines
        // Currently not using. Can be removed.
        if (anchorDic.Count != 0)
        {
            List<ARAnchor> itemsToRemove = new List<ARAnchor>();
            foreach (KeyValuePair<ARAnchor, BoundingBox> pair in anchorDic)
            {
                if (!boxSavedOutlines.Contains(pair.Value))
                {
                    Debug.Log($"DEBUG: anchor removed. {pair.Value.Label}: {(int)(pair.Value.Confidence * 100)}%");

                    itemsToRemove.Add(pair.Key);
                    m_AnchorManager.RemoveAnchor(pair.Key);
                    s_Hits.Clear();
                }
            }
            foreach (var item in itemsToRemove) {
                anchorDic.Remove(item);
            }
        }

        // return if no bounding boxes
        if (boxSavedOutlines.Count == 0)
        {
            return;
        }
        // create anchor for new bounding boxes
        foreach (var outline in boxSavedOutlines)
        {
            if (outline.Used)
            {
                continue;
            }

            // Note: rect bounding box coordinates starts from top left corner.
            // AR camera starts from borrom left corner.
            // Need to flip Y axis coordinate of the anchor 2D position when raycast
            var xMin = outline.Dimensions.X * this.scaleFactor + this.shiftX;
            var width = outline.Dimensions.Width * this.scaleFactor;
            var yMin = outline.Dimensions.Y * this.scaleFactor + this.shiftY;
            yMin = Screen.height - yMin;
            var height = outline.Dimensions.Height * this.scaleFactor;

            float center_x = xMin + width / 2f;
            float center_y = yMin - height / 2f;

            
            if (Pos2Anchor(center_x, center_y, outline))
            {
                Debug.Log("Outline used is true");
                outline.Used = true;  
            }
            else
            { 
                //Debug.Log("Outline used is false");
            }

            
        }

    }

    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    IDictionary<ARAnchor, BoundingBox> anchorDic = new Dictionary<ARAnchor, BoundingBox>();

    // from PhoneARCamera
    private List<BoundingBox> boxSavedOutlines;
    private float shiftX;
    private float shiftY;
    private float scaleFactor;
    //private float depth; // ****************************************

    public PhoneARCamera phoneARCamera;
    public ARRaycastManager m_RaycastManager;
    public TextMesh anchorObj_mesh;
    public ARAnchorManager m_AnchorManager;

    // Raycast against planes and feature points
    //const TrackableType trackableTypes = TrackableType.Planes;//FeaturePoint;
    const TrackableType trackableTypes = TrackableType.Planes | TrackableType.FeaturePoint;
}
