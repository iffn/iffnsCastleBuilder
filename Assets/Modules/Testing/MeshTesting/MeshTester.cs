using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshTester : MonoBehaviour
{
    TriangleMeshInfo TestingMesh;

    // Start is called before the first frame update
    void Start()
    {
        TestingMesh = new TriangleMeshInfo();
        SetCubeCoat();
        

        //FinishMesh(updateColliders: false);
    }

    void SetCone()
    {
        VerticesHolder line = MeshGenerator.Lines.FullCircle(radius: 1, numberOfEdges: 24);

        line.Rotate(Quaternion.LookRotation(Vector3.up));
        
        TestingMesh.Add (MeshGenerator.MeshesFromLines.KnitLines(point: Vector3.up * 2, line: line , isClosed: true));
    }

    void SetCylinder()
    {
        VerticesHolder firstLine = MeshGenerator.Lines.FullCircle(radius: 1, numberOfEdges: 24);

        /*
        VerticesHolder secondLine = MeshGenerator.Lines.FullCircle(radius: 1, numberOfEdges: 24);

        secondLine.Move(offset: Vector3.up * 2);
        secondLine.ScaleAroundCenter(scaleFactor: new Vector3(1, 1, Mathf.Sqrt(2)));
        secondLine.RotateAroundCenter(rotationAmount: Quaternion.Euler(Vector3.right * 45));

        TestingMesh.TriangleInfo = MeshGenerator.MeshesFromLines.KnitLines(firstLine: firstLine, secondLine: secondLine, isClosed: true);

        firstLine = MeshGenerator.Lines.FullCircle(radius: 1, numberOfEdges: 24);
        secondLine = MeshGenerator.Lines.FullCircle(radius: 1, numberOfEdges: 24);

        firstLine.Scale(scaleFactor: new Vector3(1, 1, Mathf.Sqrt(2)));
        firstLine.Rotate(rotationAmount: Quaternion.Euler(Vector3.right * 45));
        firstLine.Move(offset: Vector3.up * 2);

        secondLine.Rotate(rotationAmount: Quaternion.Euler(Vector3.right * 90));
        secondLine.Move(offset: Vector3.up * 2 + Vector3.forward * 2);
        
        TestingMesh.Add (MeshGenerator.MeshesFromLines.KnitLines(firstLine: firstLine, secondLine: secondLine, isClosed: true));
        */

        TestingMesh.Add (MeshGenerator.MeshesFromLines.ExtrudeLinear(firstLine: firstLine, offset: Vector3.up, closeType: MeshGenerator.ShapeClosingType.closedWithSmoothEdge, smoothTransition: true));
    }

    void SetFlatArc()
    {
        Vector2 OuterRadii = new Vector2(2, 2);
        Vector2 InnerRadii = new Vector2(1, 1);

        VerticesHolder outerLine = new VerticesHolder();
        outerLine = MeshGenerator.Lines.ArcAroundZ(radius: 1, angleDeg: 90, numberOfEdges: 3);

        outerLine.Rotate(Quaternion.LookRotation(Vector3.up, Vector3.right));
        VerticesHolder innerLine = outerLine.Clone;

        outerLine.Scale(new Vector3(OuterRadii.x, 1, OuterRadii.y));
        innerLine.Scale(new Vector3(InnerRadii.x, 1, InnerRadii.y));

        TestingMesh = MeshGenerator.MeshesFromLines.KnitLines(firstLine: outerLine, secondLine: innerLine, closingType: MeshGenerator.ShapeClosingType.open, smoothTransition: true);
    }

    void SetTransition()
    {
        VerticesHolder firstLine = MeshGenerator.Lines.FullCircle(radius: 1.5f, numberOfEdges: 8);
        VerticesHolder secondLine = MeshGenerator.Lines.FullCircle(radius: 1f, numberOfEdges: 4);

        //firstLine.Rotate(Quaternion.LookRotation(Vector3.up));
        //secondLine.Rotate(Quaternion.LookRotation(Vector3.up));
        firstLine.Move(Vector3.forward * 2);

        TestingMesh.Add(MeshGenerator.MeshesFromLines.KnitLinesWithProximityPreference(firstLine: firstLine, secondLine: secondLine, isClosed: true));
    }

    void SetMultipleTransition()
    {
        int vertices = 24;

        VerticesHolder line1 = MeshGenerator.Lines.FullCircle(radius: 0.5f, numberOfEdges: 4);
        VerticesHolder line2 = MeshGenerator.Lines.FullCircle(radius: 1f, numberOfEdges: 6);
        VerticesHolder line3 = MeshGenerator.Lines.FullCircle(radius: 1.5f, numberOfEdges: 8);
        VerticesHolder line4 = MeshGenerator.Lines.FullCircle(radius: 2.0f, numberOfEdges: 12);
        VerticesHolder line5 = MeshGenerator.Lines.FullCircle(radius: 2.5f, numberOfEdges: 14);
        VerticesHolder line6 = MeshGenerator.Lines.FullCircle(radius: 3f, numberOfEdges: 18);
        VerticesHolder line7 = MeshGenerator.Lines.FullCircle(radius: 3.5f, numberOfEdges: vertices);

        line2.Move(Vector3.back * 2);
        line3.Move(Vector3.back * 4);
        line4.Move(Vector3.back * 6);
        line5.Move(Vector3.back * 8);
        line6.Move(Vector3.back * 10);
        line7.Move(Vector3.back * 12);

        TestingMesh.Add(MeshGenerator.MeshesFromLines.KnitLinesWithProximityPreference(sections: new List<VerticesHolder>(){line1, line2, line3, line4, line5, line6, line7}, sectionsAreClosed: true, shapeIsClosed: false));
    }

    void SetGuideTest()
    {
        
        
        VerticesHolder guideLine = new VerticesHolder(new List<Vector3>()
        {
            Vector3.back * 3,
            Vector3.zero,
            Vector3.up * 3
        });
        

        VerticesHolder section = MeshGenerator.Lines.FullCircle(radius: 0.5f, numberOfEdges: 24);

        //VerticesHolder guideLine = MeshGenerator.Lines.FullCircle(radius: 2f, numberOfEdges: 24);

        TestingMesh.Add(MeshGenerator.MeshesFromLines.ExtrudeAlong(sectionLine: section, guideLine: guideLine, sectionIsClosed: true, guideIsClosed: false, sharpGuideEdges: true));
    }

    void SetFrontArc()
    {
        TriangleMeshInfo fullInfo = new TriangleMeshInfo();

        VerticesHolder rightArc = MeshGenerator.Lines.ArcAroundZ(radius: 3, angleDeg: 90, numberOfEdges: 12);
        Vector3 rightPoint = new Vector3(3, 4, 0);

        fullInfo.Add(MeshGenerator.MeshesFromLines.KnitLines(point: rightPoint, line: rightArc, isClosed: false));

        int currentIndex = fullInfo.VerticesHolder.Count - 1;


        VerticesHolder leftArc = MeshGenerator.Lines.ArcAroundZ(radius: 3, angleDeg: 90, numberOfEdges: 12);
        leftArc.Rotate(Quaternion.Euler(0, 0, 90));
        Vector3 leftPoint = new Vector3(-3, 4, 0);
        leftArc.VerticesDirectly.RemoveAt(0);

        fullInfo.Add(MeshGenerator.MeshesFromLines.KnitLines(point: leftPoint, line: leftArc, isClosed: false));

        fullInfo.Triangles.Add(new TriangleHolder(currentIndex, currentIndex + 1, currentIndex + 2));
        fullInfo.Triangles.Add(new TriangleHolder(0, currentIndex + 1, currentIndex));

        TestingMesh.Add(fullInfo);
    }
    
    void SetInnerArc()
    {
        VerticesHolder arc = MeshGenerator.Lines.ArcAroundZ(radius: 3, angleDeg: 180, numberOfEdges: 12);

        TestingMesh.Add(MeshGenerator.MeshesFromLines.ExtrudeLinear(firstLine: arc, offset: Vector3.forward * 2, closeType: MeshGenerator.ShapeClosingType.open, smoothTransition: true));

        TestingMesh.FlipTriangles();
    }

    void SetCubeCoat()
    {
        VerticesHolder firstLine = new VerticesHolder();
        firstLine.Add(new Vector3(1, 0, 1));
        firstLine.Add(new Vector3(1, 0, -1));
        firstLine.Add(new Vector3(-1, 0, -1));
        firstLine.Add(new Vector3(-1, 0, 1));

        //VerticesHolder firstLine = MeshGenerator.Lines.FullCircle(radius: 1, numberOfEdges: 4);

        TestingMesh.Add(MeshGenerator.MeshesFromLines.ExtrudeLinear(firstLine: firstLine, offset: Vector3.up * 2, closeType: MeshGenerator.ShapeClosingType.closedWithSharpEdge, smoothTransition: false));

        /*
        List<Vector2> UVs = new List<Vector2>();

        UVs.Add(new Vector2(0, 0));
        UVs.Add(new Vector2(1, 0));
        UVs.Add(new Vector2(2, 0));
        UVs.Add(new Vector2(3, 0));
        UVs.Add(new Vector2(4, 0));

        UVs.Add(new Vector2(0, 1));
        UVs.Add(new Vector2(1, 1));
        UVs.Add(new Vector2(2, 1));
        UVs.Add(new Vector2(3, 1));
        UVs.Add(new Vector2(4, 1));
        */



        //Debug.Log(TestingMesh.TriangleInfo.AllVerticesDirectly.Count);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
