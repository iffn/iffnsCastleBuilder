using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using iffnsStuff.iffnsBaseSystemForUnity;

public class TriangleMeshInfo
{
    public VerticesHolder VerticesHolder;
    public List<TriangleHolder> Triangles;
    //public MeshManager LinkedManager;
    public List<Vector2> UVs;

    public bool planar = true;

    public MailboxLineMaterial MaterialReference;
    public Material AlternativeMaterial;
    public Material MaterialToBeUsed
    {
        get
        {
            if(MaterialReference != null)
            {
                return MaterialReference.Val.LinkedMaterial;
            }
            return AlternativeMaterial;
        }
    }

    public enum ColliderStates
    {
        VisibleCollider,
        SeeThroughCollider,
        VisbleWithoutCollider
    }

    public ColliderStates ActiveCollider = ColliderStates.VisibleCollider;
    //public bool ActiveCollider = true;

    public List<Vector3> AllVerticesDirectly
    {
        get
        {
            return VerticesHolder.VerticesDirectly;
        }
        set
        {
            VerticesHolder.VerticesDirectly = value;
        }
    }

    public List<int> AllTrianglesDirectly
    {
        get
        {
            List<int> returnList = new List<int>();

            foreach (TriangleHolder triangle in Triangles)
            {
                returnList.AddRange(triangle.AsList);
            }

            return returnList;
        }
        set
        {
            Triangles.Clear();

            if(value.Count % 3 != 0)
            {
                Debug.LogWarning("Error, Triangle count not divisible by 3");
                return;
            }

            for(int i = 0; i<value.Count; i += 3)
            {
                Triangles.Add(new TriangleHolder(value[i], value[i+1], value [i+2]));
            }
        }
    }
    
    public bool IsValid
    {
        get
        {
            if (Triangles.Count == 0 || VerticesHolder.Count == 0)
            {
                //Debug.LogWarning("Error: Mesh is empty");
                return false; //Warning: Returning true causes nasty crash with empty meshes in MultyMeshManager which hard crashes visual studio when trying to catch in debugger while exporting
            }

            //Check if all triangles accessible
            if (AllTrianglesDirectly.Max() > VerticesHolder.Count - 1)
            {
                Debug.LogWarning("Error with mesh: triangles reference vertex that doesn't exist");
                return false;
            }

            //Check unique vertices
            List<int> errorLines = new List<int>();

            List<int> allTriangles = AllTrianglesDirectly;
            List<Vector3> allVertices = AllVerticesDirectly;

            for (int i = 0; i < allTriangles.Count; i += 3)
            {


                if (allVertices[allTriangles[i]] == allVertices[allTriangles[i + 1]]
                   || allVertices[allTriangles[i]] == allVertices[allTriangles[i + 2]]
                   || allVertices[allTriangles[i + 1]] == allVertices[allTriangles[i + 2]])
                {
                    errorLines.Add(i);
                }
                if(float.IsInfinity(allVertices[allTriangles[i]].x) || float.IsInfinity(allVertices[allTriangles[i]].y) || float.IsInfinity(allVertices[allTriangles[i]].z))
                {
                    errorLines.Add(i);
                }
            }

            if (errorLines.Count > 0)
            {
                Debug.LogWarning("Error: triangle pair at vertex " + errorLines[0] + " does not have unique vertecies. Number of broken vertecies = " + errorLines.Count);
                return false;
            }

            return true;
        }
    }

    void Setup(bool planar)
    {
        VerticesHolder = new VerticesHolder();
        Triangles = new List<TriangleHolder>();
        AllTrianglesDirectly = new List<int>();
        UVs = new List<Vector2>();
        this.planar = planar;
    }

    public TriangleMeshInfo(bool planar)
    {
        Setup(planar);
        
    }

    public TriangleMeshInfo(List<Vector3> vertices, List<int> triangles, bool planar)
    {
        Setup(planar);

        this.AllVerticesDirectly = vertices;
        this.AllTrianglesDirectly = triangles;
    }

    public TriangleMeshInfo(List<Vector3> vertices, List<int> triangles, List<Vector2> UVs, bool planar)
    {
        Setup(planar);

        this.AllVerticesDirectly = vertices;
        this.AllTrianglesDirectly = triangles;
        this.UVs = UVs;
    }

    public TriangleMeshInfo(TriangleMeshInfo template)
    {
        planar = template.planar;

        VerticesHolder = template.VerticesHolder.Clone;
        Triangles = new List<TriangleHolder>();
        foreach(TriangleHolder holder in template.Triangles)
        {
            Triangles.Add(holder.Clone);
        }
        UVs = new List<Vector2>(template.UVs);
        
        MaterialReference = template.MaterialReference;
        AlternativeMaterial = template.AlternativeMaterial;

        ActiveCollider = template.ActiveCollider;
    }

    public void FixUVCount()
    {
        int VertexUVDifference = VerticesHolder.Count - UVs.Count;

        if (VertexUVDifference == 0) return;
        else if(VertexUVDifference > 0)
        {
            for (int i = 0; i < VertexUVDifference; i++)
            {
                UVs.Add(Vector2.zero);
            }
        }
        else
        {
            for (int i = 0; i < -VertexUVDifference; i++)
            {
                UVs.RemoveAt(UVs.Count - 1);
            }
        }
    }

    public void Add(TriangleMeshInfo newInfo)
    {
        int vertexOffset = VerticesHolder.Count;

        foreach(TriangleHolder triangle in newInfo.Triangles)
        {
            Triangles.Add(new TriangleHolder(template: triangle, offset: vertexOffset));
        }

        VerticesHolder.Add(newInfo.VerticesHolder);

        UVs.AddRange(newInfo.UVs);

        int VertexUVDifference = newInfo.VerticesHolder.Count - newInfo.UVs.Count;

        if (VertexUVDifference > 0)
        {
            Debug.LogWarning("Warning: Triangle info to be added has " + VertexUVDifference + " Verticies more than UVs");

            FixUVCount();
        }
        else if (VertexUVDifference < 0)
        {
            Debug.LogWarning("Error: Triangle info to be added has " + -VertexUVDifference + "Verticies less than UVs");

            FixUVCount();
        }
    }

    public void Add(TriangleMeshInfo newInfo, Transform originalTransform, Transform thisTransform)
    {
        int vertexOffset = VerticesHolder.Count;

        foreach (TriangleHolder triangle in newInfo.Triangles)
        {
            Triangles.Add(new TriangleHolder(template: triangle, offset: vertexOffset));
        }

        VerticesHolder.Add(holder: newInfo.VerticesHolder, originalTransform: originalTransform, thisTransform: thisTransform);

        UVs.AddRange(newInfo.UVs);

        int VertexUVDifference = newInfo.VerticesHolder.Count - newInfo.UVs.Count;

        if (VertexUVDifference > 0)
        {
            Debug.LogWarning("Warning: Triangle info to be added has " + VertexUVDifference + " Verticies more than UVs");

            for (int i = 0; i < VertexUVDifference; i++)
            {
                newInfo.UVs.Add(Vector2.zero);
            }
        }
        else if (VertexUVDifference < 0)
        {
            Debug.LogWarning("Error: Triangle info to be added has " + -VertexUVDifference + "Verticies less than UVs");

            for (int i = 0; i < -VertexUVDifference; i++)
            {
                newInfo.UVs.RemoveAt(newInfo.UVs.Count - 1);
            }
        }
    }

    public void RemoveVertexIncludingUV(int index)
    {
        VerticesHolder.Remove(index);

        if(UVs.Count > index)
        {
            UVs.RemoveAt(index);
        }

        for(int i = 0; i< Triangles.Count; i++)
        {
            bool remove = Triangles[i].IsAffectedOtherwiseReduce(index);

            if (remove) Triangles.RemoveAt(i);
        }
    }

    public TriangleMeshInfo Clone
    {
        get
        {
            return new TriangleMeshInfo(this);
        }
    }

    public TriangleMeshInfo CloneFlipped
    {
        get
        {
            TriangleMeshInfo returnValue = new TriangleMeshInfo(this);
            returnValue.FlipTriangles();
            return returnValue;
        }
    }

    public void Move(Vector3 offset)
    {
        for (int i = 0; i < VerticesHolder.Count; i++)
        {
            AllVerticesDirectly[i] += offset;
        }
    }

    public void Rotate(Quaternion rotationAmount)
    {
        for (int i = 0; i < VerticesHolder.Count; i++)
        {
            AllVerticesDirectly[i] = rotationAmount * AllVerticesDirectly[i];
        }
    }

    public void Scale(float scale)
    {
        VerticesHolder.Scale(Vector3.one * scale);
    }

    public void Scale(Vector3 scale)
    {
        VerticesHolder.Scale(scale);
    }

    public void Transorm(Transform origin, Transform target)
    {
        VerticesHolder.Transorm(origin: origin, target: target);
    }

    public void FlipTriangles()
    {
        if (Triangles.Count == 0) return;

        foreach(TriangleHolder triangle in Triangles)
        {
            triangle.Flip();
        }
    }

    public void MergeVertex(int oldIndex, int newIndex)
    {
        foreach (TriangleHolder triangle in Triangles)
        {
            triangle.ReplaceTriangleAndMoveElements(oldIndex: oldIndex, newIndex: newIndex);
        }

        AllVerticesDirectly.RemoveAt(oldIndex);
    }

    public void MergeOverlappingVertices(float threshold)
    {
        int tried = 0;
        int merged = 0;

        Debug.Log("Vertices before = " + AllVerticesDirectly.Count);

        System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
        watch.Start();

        for (int firstIndex = 0; firstIndex < AllVerticesDirectly.Count; firstIndex++)
        {
            Vector3 firstVertex = AllVerticesDirectly[firstIndex];

            for (int secondIndex = firstIndex + 1; secondIndex < AllVerticesDirectly.Count; secondIndex++)
            {
                Vector3 secondVertex = AllVerticesDirectly[secondIndex];

                tried++;

                float difference = (firstVertex - secondVertex).magnitude;

                if (difference < threshold)
                {
                    //Merge
                    MergeVertex(oldIndex: secondIndex, newIndex: firstIndex);
                    merged++;
                }
            }
        }

        watch.Stop();
        Debug.Log("Time taken = " + watch.ElapsedMilliseconds + "ms");

        Debug.Log("Vertices after = " + AllVerticesDirectly.Count);

        Debug.Log("Tried = " + tried);
        Debug.Log("Meged = " + merged);
    }

    public void UpdateLinkedInfo()
    {
        foreach(TriangleHolder holder in Triangles)
        {
            holder.LinkedInfo = this;
        }
    }

    enum Direction
    {
        yz0,
        xz0,
        xy0,
        Tilted
    };

    public void MoveAllUVs(Vector2 offset)
    {
        for(int i = 0; i< UVs.Count; i++)
        {
            UVs[i] = UVs[i] + offset;
        }
    }

    public void RotateAllUVsCWAroundOrigin(float angleDeg)
    {
        for (int i = 0; i < UVs.Count; i++)
        {
            UVs[i] = MathHelper.RotateVector2CW(vector: UVs[i], angleDeg: angleDeg);
        }
    }

    public void FlipAllUVsHorizontallyAroundOrigin()
    {
        for (int i = 0; i < UVs.Count; i++)
        {
            UVs[i] = new Vector2(-UVs[i].x, UVs[i].y);
        }
    }

    public void FlipAllUVsVerticallyAroundOrigin()
    {
        for (int i = 0; i < UVs.Count; i++)
        {
            UVs[i] = new Vector2(UVs[i].x, -UVs[i].y);
        }
    }

    public void GenerateUVMapIfPlanar(Transform meshTransform, Transform refernceTransform)
    {
        if (!planar) return;

        if (AllVerticesDirectly.Count < 3) return;

        UpdateLinkedInfo();

        TriangleHolder currentTriangle = Triangles[0];

        Vector3 normal = currentTriangle.NormalVector;

        UVs.Clear();

        Direction direction;

        if (MathHelper.FloatIsZero(normal.y) && MathHelper.FloatIsZero(normal.z))
        {
            direction = Direction.yz0;
        }
        else if (MathHelper.FloatIsZero(normal.x) && MathHelper.FloatIsZero(normal.z))
        {
            direction = Direction.xz0;
        }
        else if (MathHelper.FloatIsZero(normal.x) && MathHelper.FloatIsZero(normal.y))
        {
            direction = Direction.xy0;
        }
        else
        {
            direction = Direction.Tilted;
        }

        switch (direction)
        {
            case Direction.yz0:
                UVs.Clear();
                //meshObject.transform.name += " x0";
                foreach (Vector3 baseVector in AllVerticesDirectly)
                {
                    //UVs.Add(new Vector2(originVector.z + localOffset.z, originVector.y + localOffset.y));
                    UVs.Add(new Vector2(baseVector.z, baseVector.y));
                }
                break;
            case Direction.xz0:
                UVs.Clear();
                //meshObject.transform.name += " y0";
                foreach (Vector3 baseVector in AllVerticesDirectly)
                {
                    //UVs.Add(new Vector2(originVector.x + localOffset.x, originVector.z + localOffset.z));
                    UVs.Add(new Vector2(baseVector.x, baseVector.z));
                }
                break;
            case Direction.xy0:
                UVs.Clear();
                //meshObject.transform.name += " z0";
                foreach (Vector3 baseVector in AllVerticesDirectly)
                {
                    //UVs.Add(new Vector2(originVector.x + localOffset.x, originVector.y + localOffset.y));
                    UVs.Add(new Vector2(baseVector.x, baseVector.y));
                }
                break;
            case Direction.Tilted:
                GeneratePlanarUVMeshIfTilted(normal: normal, meshTransform: meshTransform, refernceTransform: refernceTransform);
                break;
            default:
                Debug.LogWarning("Error: UV direction not defined");
                break;
        }

    }

    void GeneratePlanarUVMeshIfTilted(Vector3 normal, Transform meshTransform, Transform refernceTransform)
    {
        //ToDo: Rewrite code to do it without the Helper transform. Then check if faster than cardinal direction if statement

        Transform helperObject = new GameObject().transform;

        helperObject.transform.parent = refernceTransform;
        //Debug.Log("1: " + watch.Elapsed.TotalSeconds);

        //If looking directly down or up:
        //Quaternion rotation;

        if (MathHelper.FloatIsZero(normal.x) && MathHelper.FloatIsZero(normal.z))
        {
            helperObject.transform.LookAt(worldPosition: -refernceTransform.up, worldUp: refernceTransform.forward);

            //rotation = Quaternion.LookRotation(forward: -refernceTransform.up, upwards: refernceTransform.forward);
        }
        else
        {
            //Project 3D
            Vector3 worldPosition = meshTransform.TransformDirection(normal);

            helperObject.transform.LookAt(worldPosition: meshTransform.TransformDirection(normal), worldUp: refernceTransform.up);

            //rotation = Quaternion.LookRotation(forward: meshTransform.TransformDirection(normal), upwards: refernceTransform.up);
        }
        //Debug.Log("2: " + watch.Elapsed.TotalSeconds);

        foreach (Vector3 vertex in AllVerticesDirectly)
        {
            Vector3 transformedPoint = helperObject.InverseTransformPoint(meshTransform.TransformPoint(vertex));

            UVs.Add(new Vector2(transformedPoint.x, transformedPoint.y));
        }
        //Debug.Log("3: " + watch.Elapsed.TotalSeconds);

        GameObject.Destroy(helperObject.gameObject);
    }

    public static TriangleMeshInfo CompbineMultipleIntoSingleInfo(List<TriangleMeshInfo> infos)
    {
        TriangleMeshInfo returnValue = new TriangleMeshInfo(planar: false);

        foreach(TriangleMeshInfo info in infos)
        {
            returnValue.Add(info);
        }

        return returnValue;
    }

    public static List<TriangleMeshInfo> GetCloneOfInfoList(List<TriangleMeshInfo> infos, bool flip)
    {
        List<TriangleMeshInfo> returnValue = new List<TriangleMeshInfo>();

        foreach(TriangleMeshInfo info in infos)
        {
            if (flip)
            {
                returnValue.Add(info.CloneFlipped);
            }
            else
            {
                returnValue.Add(info.Clone);
            }
            
        }

        return returnValue;
    }
}

public class VerticesHolder
{
    List<Vector3> vertices;

    public List<Vector3> VerticesDirectly
    {
        get
        {
            return vertices;
        }
        set
        {
            vertices = value;
        }
    }

    public int Count
    {
        get
        {
            return vertices.Count;
        }
    }

    public VerticesHolder()
    {
        vertices = new List<Vector3>();
    }

    public VerticesHolder(List<Vector3> vertices)
    {
        this.vertices = new List<Vector3>(vertices);
    }

    public VerticesHolder(VerticesHolder holder)
    {
        this.vertices = new List<Vector3>(holder.vertices);
    }

    public void Reverse()
    {
        vertices.Reverse();
    }

    public VerticesHolder Clone
    {
        get
        {
            return new VerticesHolder(this);
        }
    }

    public VerticesHolder CloneReversed
    {
        get
        {
            VerticesHolder returnValue = new VerticesHolder(this);
            returnValue.Reverse();
            return returnValue;
        }
    }

    public void Add(Vector3 element)
    {
        vertices.Add(element);
    }

    public void Add(List<Vector3> elements)
    {
        vertices.AddRange(elements);
    }

    public void Add(VerticesHolder holder)
    {
        vertices.AddRange(holder.vertices);
    }

    public void Add(VerticesHolder holder, Transform originalTransform, Transform thisTransform)
    {
        foreach(Vector3 vertex in holder.vertices)
        {
            Vector3 worldVector = originalTransform.TransformPoint(vertex);
            Vector3 newLocalVector = thisTransform.InverseTransformDirection(worldVector);
            vertices.Add(newLocalVector);
        }
    }

    public void Insert(int index, Vector3 element)
    {
        vertices.Insert(index, element);
    }

    public void Remove(int index)
    {
        vertices.RemoveAt(index);
    }

    public void Move(Vector3 offset)
    {
        for (int i = 0; i < vertices.Count; i++)
        {
            vertices[i] += offset;
        }
    }

    public void Rotate(Quaternion rotationAmount)
    {
        for (int i = 0; i < vertices.Count; i++)
        {
            vertices[i] = rotationAmount * vertices[i];
        }
    }

    public void RotateAroundCenterOfMass(Quaternion rotationAmount)
    {
        Vector3 offset = CenterOfMass;

        Move(-offset);

        Rotate(rotationAmount);

        Move(offset);
    }

    public void Scale(Vector3 scaleFactor)
    {
        for (int i = 0; i < vertices.Count; i++)
        {
            vertices[i] = MathHelper.MultiplyVectorElemetns(vertices[i], scaleFactor);
        }
    }

    public void ScaleAroundPoint(Vector3 point, Vector3 scaleFactor)
    {
        Move(-point);
        Scale(scaleFactor);
        Move(point);
    }

    public void ScaleAroundCenterOfMass(Vector3 scaleFactor)
    {
        Vector3 offset = CenterOfMass;

        ScaleAroundPoint(point: offset, scaleFactor: scaleFactor);
    }

    public void Transorm(Transform origin, Transform target)
    {
        for (int i = 0; i < vertices.Count; i++)
        {
            vertices[i] = target.InverseTransformPoint(origin.TransformPoint(vertices[i]));
        }
    }

    public Vector3 CenterOfMass
    {
        get
        {
            Vector3 returnVector = Vector3.zero;

            foreach (Vector3 vertex in vertices)
            {
                returnVector += vertex;
            }

            returnVector /= vertices.Count;

            return returnVector;
        }
    }
    
}

public class TriangleHolder
{
    public TriangleMeshInfo LinkedInfo;

    public int Index1 { private set; get; }
    public int Index2 { private set; get; }
    public int Index3 { private set; get; }

    public bool IsAffectedOtherwiseReduce(int index)
    {
        if (Index1 == index) return true;
        if (Index2 == index) return true;
        if (Index3 == index) return true;

        if (Index1 > index) Index1--;
        if (Index2 > index) Index2--;
        if (Index3 > index) Index3--;

        return false;
    }

    //Constructors
    public TriangleHolder(int triangle1, int triangle2, int triangle3)
    {
        this.Index1 = triangle1;
        this.Index2 = triangle2;
        this.Index3 = triangle3;
    }

    public TriangleHolder(int baseOffset, int t1, int t2, int t3)
    {
        this.Index1 = t1 + baseOffset;
        this.Index2 = t2 + baseOffset;
        this.Index3 = t3 + baseOffset;
    }

    public TriangleHolder(TriangleHolder template, int offset)
    {
        List<int> triangles = template.AsList;

        Index1 = triangles[0] + offset;
        Index2 = triangles[1] + offset;
        Index3 = triangles[2] + offset;
    }

    public TriangleHolder(TriangleHolder template)
    {
        List<int> triangles = template.AsList;

        Index1 = triangles[0];
        Index2 = triangles[1];
        Index3 = triangles[2];
    }

    //Duplicate
    public TriangleHolder Clone
    {
        get
        {
            return new TriangleHolder(this);
        }
    }

    public TriangleHolder CloneFlipped
    {
        get
        {
            return new TriangleHolder(Index2, Index1, Index3);
        }
    }

    //Modifications
    public void  Flip()
    {
        int temp = Index1;
        Index1 = Index2;
        Index2 = temp;
    }

    public void ReplaceTriangleAndMoveElements(int oldIndex, int newIndex)
    {
        if(Index1 == oldIndex)
        {
            Index1 = newIndex;
        }
        else if(Index1 > oldIndex)
        {
            Index1--;
        }

        if (Index2 == oldIndex)
        {
            Index2 = newIndex;
        }
        else if (Index2 > oldIndex)
        {
            Index2--;
        }

        if (Index3 == oldIndex)
        {
            Index3 = newIndex;
        }
        else if (Index3 > oldIndex)
        {
            Index3--;
        }
    }

    //Info
    public List<int> AsList
    {
        get
        {
            return new List<int>() { Index1, Index2, Index3 };
        }
    }

    public Vector3 NormalVector
    {
        get
        {
            List<Vector3> vertices = Vertices;

            Vector3 side1 = vertices[1] - vertices[0];
            Vector3 side2 = vertices[2] - vertices[0];

            Vector3 returnVector = Vector3.Cross(side1, side2);

            return returnVector.normalized;
        }
    }

    public List<Vector3> Vertices
    {
        get
        {
            if(Index1 > LinkedInfo.AllTrianglesDirectly.Count - 1
                || Index2 > LinkedInfo.AllTrianglesDirectly.Count - 1
                || Index3 > LinkedInfo.AllTrianglesDirectly.Count - 1)
            {
                Debug.Log("Error");
            }

            List<Vector3> returnList = new List<Vector3>
            {
                LinkedInfo.AllVerticesDirectly[Index1],
                LinkedInfo.AllVerticesDirectly[Index2],
                LinkedInfo.AllVerticesDirectly[Index3]
            };

            return returnList;
        }
    }
}