using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static System.Collections.Specialized.BitVector32;


public static class MeshGenerator
{
    public static class Lines
    {
        public static VerticesHolder RectangleAroundCenterXY(float xSize, float ySize)
        {
            VerticesHolder returnValue = new();

            float halfXSize = 0.5f * xSize;
            float halfYSize = 0.5f * ySize;

            returnValue.Add(new Vector3(-halfXSize, -halfYSize));
            returnValue.Add(new Vector3(-halfXSize, halfYSize));
            returnValue.Add(new Vector3(halfXSize, halfYSize));
            returnValue.Add(new Vector3(halfXSize, -halfYSize));

            return returnValue;
        }

        public static VerticesHolder RectangleAtCornerXY(float xSize, float ySize)
        {
            VerticesHolder returnValue = new();

            returnValue.Add(new Vector3(0, 0, 0));
            returnValue.Add(new Vector3(0, ySize, 0));
            returnValue.Add(new Vector3(xSize, ySize, 0));
            returnValue.Add(new Vector3(xSize, 0, 0));

            return returnValue;
        }

        public static VerticesHolder RectangleAtCornerZY(float zSize, float ySize)
        {
            VerticesHolder returnValue = new();

            returnValue.Add(new Vector3(0, 0, zSize));
            returnValue.Add(new Vector3(0, ySize, zSize));
            returnValue.Add(new Vector3(0, ySize, 0));
            returnValue.Add(new Vector3(0, 0, 0));

            return returnValue;
        }

        public static VerticesHolder FullCircle(float radius, int numberOfEdges)
        {
            VerticesHolder returnValue = new();

            float angleIncrement = 360 / numberOfEdges * Mathf.Deg2Rad;

            for (int edge = 0; edge < numberOfEdges; edge++)
            {
                float angle = edge * angleIncrement;

                returnValue.Add(new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0));
            }

            return returnValue;
        }

        public static VerticesHolder ArcAroundZ(float radius, float angleDeg, int numberOfEdges)
        {
            VerticesHolder returnValue = new();

            float angleIncrement = angleDeg / numberOfEdges * Mathf.Deg2Rad;

            for (int edge = 0; edge < numberOfEdges + 1; edge++)
            {
                float currentAngle = edge * angleIncrement;

                returnValue.Add(new Vector3(Mathf.Cos(currentAngle) * radius, Mathf.Sin(currentAngle) * radius, 0));
            }

            return returnValue;
        }

        public static VerticesHolder ArcAroundY(float radius, float angleDeg, int numberOfEdges)
        {
            VerticesHolder returnValue = new();

            float angleIncrement = angleDeg / numberOfEdges * Mathf.Deg2Rad;

            for (int edge = 0; edge < numberOfEdges + 1; edge++)
            {
                float currentAngle = edge * angleIncrement;

                returnValue.Add(new Vector3(Mathf.Cos(currentAngle) * radius, 0 , Mathf.Sin(currentAngle) * radius));
            }

            return returnValue;
        }

        public static VerticesHolder ArcAroundX(float radius, float angleDeg, int numberOfEdges)
        {
            VerticesHolder returnValue = new();

            float angleIncrement = angleDeg / numberOfEdges * Mathf.Deg2Rad;

            for (int edge = 0; edge < numberOfEdges + 1; edge++)
            {
                float currentAngle = edge * angleIncrement;

                returnValue.Add(new Vector3(0, Mathf.Sin(currentAngle) * radius, Mathf.Cos(currentAngle) * radius));
            }

            return returnValue;
        }
    }

    public static class FilledShapes
    {
        public static TriangleMeshInfo RectangleAroundCenter(Vector3 baseLineFull, Vector3 secondLineFull)
        {
            TriangleMeshInfo returnValue = new(planar: true);

            returnValue.VerticesHolder.Add(0.5f * (baseLineFull + secondLineFull));
            returnValue.VerticesHolder.Add(0.5f * (baseLineFull - secondLineFull));
            returnValue.VerticesHolder.Add(0.5f * (-baseLineFull - secondLineFull));
            returnValue.VerticesHolder.Add(0.5f * (-baseLineFull + secondLineFull));

            returnValue.Triangles.Add(new TriangleHolder(0, 1, 2));
            returnValue.Triangles.Add(new TriangleHolder(0, 2, 3));

            returnValue.UVs.Add(new Vector2(baseLineFull.magnitude, secondLineFull.magnitude) * 0.5f);
            returnValue.UVs.Add(new Vector2(baseLineFull.magnitude, -secondLineFull.magnitude) * 0.5f);
            returnValue.UVs.Add(new Vector2(-baseLineFull.magnitude, -secondLineFull.magnitude) * 0.5f);
            returnValue.UVs.Add(new Vector2(-baseLineFull.magnitude, secondLineFull.magnitude) * 0.5f);

            return returnValue;
        }

        public static TriangleMeshInfo RectangleAtCorner(Vector3 baseLine, Vector3 secondLine, Vector2 uvOffset)
        {
            TriangleMeshInfo returnValue = new(planar: true);

            returnValue.VerticesHolder.Add(Vector3.zero);
            returnValue.VerticesHolder.Add(secondLine);
            returnValue.VerticesHolder.Add(baseLine + secondLine);
            returnValue.VerticesHolder.Add(baseLine);

            returnValue.Triangles.Add(new TriangleHolder(0, 1, 2));
            returnValue.Triangles.Add(new TriangleHolder(0, 2, 3));

            returnValue.UVs.Add(new Vector2(0, 0) + uvOffset);
            returnValue.UVs.Add(new Vector2(0, secondLine.magnitude) + uvOffset);
            returnValue.UVs.Add(new Vector2(baseLine.magnitude, secondLine.magnitude) + uvOffset);
            returnValue.UVs.Add(new Vector2(baseLine.magnitude, 0) + uvOffset);

            return returnValue;
        }

        public static List<TriangleMeshInfo> BoxAroundCenter(Vector3 size)
        {
            List<TriangleMeshInfo> returnValue = new();

            //up down
            TriangleMeshInfo face1 = RectangleAroundCenter(baseLineFull: Vector3.right * size.x, Vector3.forward * size.z);
            face1.Move(offset: 0.5f * size.y * Vector3.up);

            TriangleMeshInfo face2 = RectangleAroundCenter(baseLineFull: Vector3.forward * size.z, Vector3.right * size.x);
            face2.Move(offset: 0.5f * size.y * Vector3.down);

            //left right
            TriangleMeshInfo face3 = RectangleAroundCenter(baseLineFull: Vector3.forward * size.z, Vector3.up * size.y);
            face3.Move(offset: 0.5f * size.x * Vector3.right);

            TriangleMeshInfo face4 = RectangleAroundCenter(baseLineFull: Vector3.up * size.y, Vector3.forward * size.z);
            face4.Move(offset: 0.5f * size.x * Vector3.left);

            //forward back
            TriangleMeshInfo face5 = RectangleAroundCenter(baseLineFull: Vector3.up * size.y, Vector3.right * size.x);
            face5.Move(offset: 0.5f * size.z * Vector3.forward);

            TriangleMeshInfo face6 = RectangleAroundCenter(baseLineFull: Vector3.right * size.x, Vector3.up * size.y);
            face6.Move(offset: 0.5f * size.z * Vector3.back);

            returnValue.Add(face1);
            returnValue.Add(face2);
            returnValue.Add(face3);
            returnValue.Add(face4);
            returnValue.Add(face5);
            returnValue.Add(face6);

            return returnValue;
        }

        public static TriangleMeshInfo CylinderAroundCenterWithoutCap(float radius, float length, Vector3 direction, int numberOfEdges)
        {
            VerticesHolder circle = Lines.FullCircle(radius: radius, numberOfEdges: numberOfEdges);

            TriangleMeshInfo returnValue = MeshesFromLines.ExtrudeLinearWithSmoothCorners(firstLine: circle, offset: Vector3.forward * length, closeType: ShapeClosingType.closedWithSmoothEdge, planar: false);

            returnValue.Move(length * 0.5f * Vector3.back);

            returnValue.Rotate(Quaternion.LookRotation(direction));

            return returnValue;
        }

        public static List<TriangleMeshInfo> CylinderCaps(float radius, float length, Vector3 direction, int numberOfEdges)
        {
            VerticesHolder circleLine = Lines.FullCircle(radius: radius, numberOfEdges: numberOfEdges);

            List<TriangleMeshInfo> returnValue = new();

            TriangleMeshInfo circle = MeshesFromLines.KnitLinesSmooth(point: Vector3.zero, line: circleLine, isClosed: true, planar: true);

            circle.Move(length * 0.5f * Vector3.back);

            returnValue.Add(circle.Clone);

            circle.FlipTriangles();

            circle.Move(length * Vector3.forward);
            
            returnValue.Add(circle);

            foreach(TriangleMeshInfo currentCircle in returnValue)
            {
                currentCircle.Rotate(Quaternion.LookRotation(direction));
            }

            return returnValue;
        }

        public static TriangleMeshInfo PointsClockwiseAroundStartPoint(Vector3 startPoint, List<Vector3> points, bool planar)
        {
            List<Vector3> usedPoints = new(points);
            usedPoints.Insert(0, startPoint);

            return PointsClockwiseAroundFirstPoint(points: usedPoints, planar: planar);
        }

        public static TriangleMeshInfo PointsClockwiseAroundFirstPoint(List<Vector3> points, bool planar)
        {
            TriangleMeshInfo returnValue = new(planar: planar);

            if (points == null) return returnValue;
            if (points.Count < 3) return returnValue;

            returnValue.VerticesHolder.Add(points);

            for (int i = 0; i < points.Count - 2; i++)
            {
                returnValue.Triangles.Add(new TriangleHolder(baseOffset: 0, t1: 0, t2: i + 1, t3: i + 2));
            }

            //UVs
            returnValue.UVs.Add(Vector2.zero);
            
            Vector3 vecA = points[1] - points[0];
            returnValue.UVs.Add(Vector2.up * vecA.magnitude);

            float angleC = 0;

            for (int i = 2; i < points.Count; i++)
            {
                float a = (points[i - 1] - points[0]).magnitude;
                float b = (points[i] - points[0]).magnitude;
                float c = (points[i] - points[i - 1]).magnitude;

                angleC += Mathf.Acos((a * a + b * b - c * c) / (2 * a * b)); //Law of cosines

                returnValue.UVs.Add(new Vector2(Mathf.Sin(angleC) * b, Mathf.Cos(angleC) * b));
            }

            return returnValue;
        }
    }

    public enum ShapeClosingType
    {
        open,
        closedWithSharpEdge,
        closedWithSmoothEdge
    }

    public static class MeshesFromLines
    {
        public static List<TriangleMeshInfo> KnitLinesWithSharpEdges(VerticesHolder firstLine, VerticesHolder secondLine, bool closed)
        {
            List<TriangleMeshInfo> returnValue = new();

            for (int i = 0; i < firstLine.Count - 1; i++)
            {
                List<Vector3> line = new()
                {
                    secondLine.VerticesDirectly[i + 1],
                    firstLine.VerticesDirectly[i + 1],
                    firstLine.VerticesDirectly[i]
                };

                TriangleMeshInfo addition = MeshGenerator.MeshesFromLines.KnitLinesSmooth(point: secondLine.VerticesDirectly[i], line: line, isClosed: false, planar: true);

                returnValue.Add(addition);
            }

            if (closed)
            {
                List<Vector3> line = new()
                {
                    secondLine.VerticesDirectly[0],
                    firstLine.VerticesDirectly[0],
                    firstLine.VerticesDirectly[^1]
                };

                TriangleMeshInfo addition = MeshGenerator.MeshesFromLines.KnitLinesSmooth(point: secondLine.VerticesDirectly[^1], line: line, isClosed: false, planar: true);

                returnValue.Add(addition);
            }

            return returnValue;
        }

        public static TriangleMeshInfo KnitLinesSmooth(VerticesHolder firstLine, VerticesHolder secondLine, ShapeClosingType closingType, bool planar)
        {
            // IsClosed = First and last point should be closed -> True if closed shape
            // IsSealed = Smooth transition between first first and last point
            // SmoothTransition = 

            TriangleMeshInfo returnValue = new(planar: planar);

            if (firstLine == null || secondLine == null)
            {
                Debug.LogWarning("Error with MeshGenerator: At least one line is null");

                return returnValue;
            }

            if (firstLine.Count != secondLine.Count)
            {
                Debug.LogWarning("Error with MeshGenerator: Both lines do not have the same amount of vertices");

                return returnValue;
            }

            if (firstLine.Count == 0)
            {
                Debug.LogWarning("Error with MeshGenerator: Both elements have a length of 0");

                return returnValue;
            }

            if (closingType == ShapeClosingType.closedWithSharpEdge)
            {
                returnValue.VerticesHolder.Add(firstLine.VerticesDirectly);
                returnValue.VerticesHolder.Add(firstLine.VerticesDirectly[0]);

                returnValue.VerticesHolder.Add(secondLine.VerticesDirectly);
                returnValue.VerticesHolder.Add(secondLine.VerticesDirectly[0]);

                for (int i = 0; i < firstLine.Count; i++)
                {
                    returnValue.Triangles.Add(new TriangleHolder(baseOffset: i, t1: 0, t2: 1, t3: firstLine.Count + 1));
                    returnValue.Triangles.Add(new TriangleHolder(baseOffset: i, t1: 1, t2: firstLine.Count + 2, t3: firstLine.Count + 1));
                }
            }
            else
            {
                returnValue.VerticesHolder.Add(firstLine.VerticesDirectly);
                returnValue.VerticesHolder.Add(secondLine.VerticesDirectly);

                for (int i = 0; i < firstLine.Count - 1; i++)
                {
                    returnValue.Triangles.Add(new TriangleHolder(baseOffset: i, t1: 0, t2: 1, t3: firstLine.Count));
                    returnValue.Triangles.Add(new TriangleHolder(baseOffset: i, t1: 1, t2: firstLine.Count + 1, t3: firstLine.Count));
                }

                if (closingType != ShapeClosingType.open)
                {
                    returnValue.Triangles.Add(new TriangleHolder(firstLine.Count - 1, 0, firstLine.Count * 2 - 1));
                    returnValue.Triangles.Add(new TriangleHolder(0, firstLine.Count, firstLine.Count * 2 - 1));
                }
            }

            //Generate UVs
            int firstLineCount = returnValue.VerticesHolder.VerticesDirectly.Count / 2;

            //First UV line
            float firstOffset = 0;
            returnValue.UVs.Add(Vector2.zero);

            for (int i = 1; i < firstLineCount; i++)
            {
                firstOffset += (returnValue.VerticesHolder.VerticesDirectly[i] - firstLine.VerticesDirectly[i - 1]).magnitude;
                returnValue.UVs.Add(new Vector2(firstOffset, 0));
            }

            //Second UV line
            float secondOffset = 0;
            float verticalOffset = (secondLine.VerticesDirectly[0] - firstLine.VerticesDirectly[0]).magnitude;

            returnValue.UVs.Add(new Vector2(0, verticalOffset));

            for (int i = firstLineCount + 1; i < returnValue.VerticesHolder.VerticesDirectly.Count; i++)
            {
                secondOffset += (returnValue.VerticesHolder.VerticesDirectly[i] - returnValue.VerticesHolder.VerticesDirectly[i - 1]).magnitude;
                returnValue.UVs.Add(new Vector2(secondOffset, verticalOffset));
            }

            //Move second UV line
            float secondLineMove = (firstOffset - secondOffset) / 2;

            for (int i = firstLineCount; i < returnValue.VerticesHolder.VerticesDirectly.Count; i++)
            {
                returnValue.UVs[i] += Vector2.right * secondLineMove;
            }

            return returnValue;
        }

        public static List<TriangleMeshInfo> KnitLinesWithSharpEdges(Vector3 point, VerticesHolder line, bool isClosed)
        {
            List<TriangleMeshInfo> returnValue = new();

            if (line == null || line.Count < 2)
            {
                Debug.LogWarning("Error with MeshGenerator: Not enough information to build mesh");
                return returnValue;
            }

            List<Vector3> lineVertices = line.VerticesDirectly;

            for (int i = 1; i < line.Count; i++)
            {
                returnValue.Add(MeshesFromPoints.MeshFrom3Points(point, lineVertices[i], lineVertices[i-1]));
            }

            if (isClosed)
            {
                returnValue.Add(MeshesFromPoints.MeshFrom3Points(point, lineVertices[0], lineVertices[line.Count - 1]));
            }

            return returnValue;
        }

        public static TriangleMeshInfo KnitLinesSmooth(Vector3 point, VerticesHolder line, bool isClosed, bool planar)
        {
            TriangleMeshInfo returnValue = new(planar: planar);

            if (line == null || line.Count < 2)
            {
                Debug.LogWarning("Error with MeshGenerator: Not enough information to build mesh");
                return returnValue;
            }

            returnValue.VerticesHolder.Add(point);

            returnValue.VerticesHolder.Add(line);

            for (int i = 1; i < line.Count; i++)
            {
                returnValue.Triangles.Add(new TriangleHolder(0, i + 1, i));
            }

            if (isClosed)
            {
                returnValue.Triangles.Add(new TriangleHolder(0, 1, line.Count));
            }

            //Add zero vectors as UVs
            for(int i = 0; i<returnValue.VerticesHolder.Count; i++)
            {
                returnValue.UVs.Add(Vector2.zero);
            }

            return returnValue;
        }

        public static TriangleMeshInfo KnitLinesSmooth(Vector3 point, List<Vector3> line, bool isClosed, bool planar)
        {
            TriangleMeshInfo returnValue = new(planar: planar);

            if (line == null || line.Count < 2)
            {
                Debug.LogWarning("Error with MeshGenerator: Not enough information to build mesh");
                return returnValue;
            }

            returnValue.VerticesHolder.Add(point);

            returnValue.VerticesHolder.Add(line);

            for (int i = 1; i < line.Count; i++)
            {
                returnValue.Triangles.Add(new TriangleHolder(0, i + 1, i));
            }

            if (isClosed)
            {
                returnValue.Triangles.Add(new TriangleHolder(0, 1, line.Count));
            }


            //UV guessing
            returnValue.UVs.Add(Vector2.zero);

            float angle = Mathf.PI * 0.5f / line.Count;

            for(int i = 0; i<line.Count; i++)
            {
                returnValue.UVs.Add(new Vector2(Mathf.Sin(angle * i), Mathf.Cos(angle * i)));
            }

            return returnValue;
        }

        public static TriangleMeshInfo KnitLinesSmooth(List<VerticesHolder> sections, bool sectionsAreClosed, bool shapeIsClosed, bool planar)
        {
            TriangleMeshInfo returnValue = new(planar: planar);

            //Check validity
            if (sections == null || sections.Count < 2)
            {
                Debug.LogWarning("Error with MeshGenerator: Not enough information to build mesh");
                return returnValue;
            }

            int pointCount = sections[0].Count;

            foreach (VerticesHolder holder in sections)
            {
                if (holder.Count != pointCount)
                {
                    Debug.LogWarning($"Mesh generation error: KnitLinesSmooth was attempted with lines that don't have the same length");
                    return returnValue;
                }
            }

            //Add each vertex
            foreach (VerticesHolder holder in sections)
            {
                returnValue.VerticesHolder.Add(holder);
            }

            int firstLineStartIndex = 0;
            int secondLineStartIndex = pointCount;

            for (int i = 1; i < sections.Count; i++)
            {
                merge2Lines(firstLineStartIndex: firstLineStartIndex, secondLineStartIndex: secondLineStartIndex);

                firstLineStartIndex = secondLineStartIndex;
                secondLineStartIndex += pointCount;
            }

            firstLineStartIndex = secondLineStartIndex;
            secondLineStartIndex = 0;

            if (shapeIsClosed)
            {
                merge2Lines(firstLineStartIndex: firstLineStartIndex, secondLineStartIndex: secondLineStartIndex);
            }

            return returnValue;

            void merge2Lines(int firstLineStartIndex, int secondLineStartIndex)
            {
                for (int i = 0; i < pointCount - 1; i++)
                {
                    returnValue.Triangles.Add(new TriangleHolder(
                        secondLineStartIndex + i,
                        firstLineStartIndex + i,
                        secondLineStartIndex + 1 + i
                        ));

                    returnValue.Triangles.Add(new TriangleHolder(
                        firstLineStartIndex + i,
                        firstLineStartIndex + 1 + i,
                        secondLineStartIndex + 1 + i
                        ));
                }

                if (sectionsAreClosed)
                {
                    returnValue.Triangles.Add(new TriangleHolder(
                        secondLineStartIndex + pointCount - 1,
                        firstLineStartIndex + pointCount - 1,
                        secondLineStartIndex + 0
                        ));

                    returnValue.Triangles.Add(new TriangleHolder(
                        firstLineStartIndex + pointCount - 1,
                        firstLineStartIndex + 0,
                        secondLineStartIndex + 0
                        ));
                }
            }
        }

        public static TriangleMeshInfo KnitLinesWithProximityPreference(List<VerticesHolder> sections, bool sectionsAreClosed, bool shapeIsClosed, bool planar) //ToDo: Improve, since at the moment, the proximity preference is ignored at the ends
        {
            TriangleMeshInfo returnValue = new(planar: planar);

            if (sections == null || sections.Count < 2)
            {
                Debug.LogWarning("Error with MeshGenerator: Not enough information to build mesh");
                return returnValue;
            }

            returnValue.VerticesHolder.Add(sections[0]);

            for (int i = 0; i < sections.Count - 1; i++)
            {
                int offset1 = returnValue.VerticesHolder.Count - sections[i].Count;
                int offset2 = returnValue.VerticesHolder.Count;

                returnValue.VerticesHolder.Add(sections[i + 1]);

                int max1 = returnValue.VerticesHolder.Count - sections[i + 1].Count - 1;
                int max2 = returnValue.VerticesHolder.Count - 1;

                if (sectionsAreClosed)
                {
                    float distance12 = (returnValue.VerticesHolder.VerticesDirectly[max1] - returnValue.VerticesHolder.VerticesDirectly[offset2]).magnitude;
                    float distance21 = (returnValue.VerticesHolder.VerticesDirectly[max2] - returnValue.VerticesHolder.VerticesDirectly[offset1]).magnitude;

                    if (distance12 < distance21)
                    {
                        returnValue.Triangles.Add(new TriangleHolder(max1, offset2, max2));
                        returnValue.Triangles.Add(new TriangleHolder(max1, offset1, offset2));
                    }
                    else
                    {
                        returnValue.Triangles.Add(new TriangleHolder(max1, offset1, max2));
                        returnValue.Triangles.Add(new TriangleHolder(offset1, offset2, max2));
                    }
                }

                bool end1 = false;
                bool end2 = false;

                //int areas = sections[i].Count + sections[i + 1].Count - 2;

                void CreateTriangleWithBaseOnLine2()
                {
                    returnValue.Triangles.Add(new TriangleHolder(offset1, offset2 + 1, offset2));

                    offset2++;

                    if (offset2 == max2) end2 = true;
                }

                void CreateTriangleWithBaseOnLine1()
                {
                    returnValue.Triangles.Add(new TriangleHolder(offset1, offset1 + 1, offset2));

                    offset1++;

                    if (offset1 == max1) end1 = true;
                }

                //for(int area = 0; area<areas; area++)
                while (true)
                {
                    if (!end1 && !end2)
                    {
                        float distance12 = (returnValue.VerticesHolder.VerticesDirectly[offset2 + 1] - returnValue.VerticesHolder.VerticesDirectly[offset1]).magnitude;
                        float distance21 = (returnValue.VerticesHolder.VerticesDirectly[offset1 + 1] - returnValue.VerticesHolder.VerticesDirectly[offset2]).magnitude;

                        if (distance12 < distance21)
                        {
                            CreateTriangleWithBaseOnLine2();
                        }
                        else
                        {
                            CreateTriangleWithBaseOnLine1();
                        }
                    }
                    else if (end1 && !end2)
                    {
                        CreateTriangleWithBaseOnLine2();
                    }
                    else if (!end1 && end2)
                    {
                        CreateTriangleWithBaseOnLine1();
                    }
                    else
                    {
                        break;
                    }
                }

                if (!end1 || !end2) Debug.Log("not complete");
            }

            if (shapeIsClosed)
            {
                int offset1 = returnValue.VerticesHolder.Count - sections[^1].Count;
                int offset2 = 0;

                int max1 = returnValue.VerticesHolder.Count - 1;
                int max2 = sections[0].Count;

                if (sectionsAreClosed)
                {
                    float distance12 = (returnValue.VerticesHolder.VerticesDirectly[max1] - returnValue.VerticesHolder.VerticesDirectly[offset2]).magnitude;
                    float distance21 = (returnValue.VerticesHolder.VerticesDirectly[max2] - returnValue.VerticesHolder.VerticesDirectly[offset1]).magnitude;

                    if (distance12 < distance21)
                    {
                        returnValue.Triangles.Add(new TriangleHolder(max1, offset2, max2));

                        returnValue.Triangles.Add(new TriangleHolder(max1, offset1, offset2));
                    }
                    else
                    {
                        returnValue.Triangles.Add(new TriangleHolder(max1, offset1, max2));

                        returnValue.Triangles.Add(new TriangleHolder(offset1, offset2, max2));
                    }
                }

                bool end1 = false;
                bool end2 = false;

                //int areas = sections[i].Count + sections[i + 1].Count - 2;

                void CreateTriangleWithBaseOnLine2()
                {
                    returnValue.Triangles.Add(new TriangleHolder(offset1, offset2 + 1, offset2));

                    offset2++;

                    if (offset2 == max2) end2 = true;
                }

                void CreateTriangleWithBaseOnLine1()
                {
                    returnValue.Triangles.Add(new TriangleHolder(offset1, offset1 + 1, offset2));

                    offset1++;

                    if (offset1 == max1) end1 = true;
                }

                //for(int area = 0; area<areas; area++)
                while (true)
                {
                    if (!end1 && !end2)
                    {
                        float distance12 = (returnValue.VerticesHolder.VerticesDirectly[offset2 + 1] - returnValue.VerticesHolder.VerticesDirectly[offset1]).magnitude;
                        float distance21 = (returnValue.VerticesHolder.VerticesDirectly[offset1 + 1] - returnValue.VerticesHolder.VerticesDirectly[offset2]).magnitude;

                        if (distance12 < distance21)
                        {
                            CreateTriangleWithBaseOnLine2();
                        }
                        else
                        {
                            CreateTriangleWithBaseOnLine1();
                        }
                    }
                    else if (end1 && !end2)
                    {
                        CreateTriangleWithBaseOnLine2();
                    }
                    else if (!end1 && end2)
                    {
                        CreateTriangleWithBaseOnLine1();
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return returnValue;
        }

        public static TriangleMeshInfo KnitLinesWithProximityPreference(VerticesHolder firstLine, VerticesHolder secondLine, bool isClosed, bool planar)
        {
            //ToDo: UVs

            TriangleMeshInfo returnValue = new(planar: planar);

            if (firstLine == null || secondLine == null)
            {
                Debug.LogWarning("Error with MeshGenerator: At least one line is null");

                return new TriangleMeshInfo(planar: planar);
            }

            if (firstLine.Count == 0 || secondLine.Count == 0)
            {
                Debug.LogWarning("Error with MeshGenerator: At least one line has a length of 0");

                return new TriangleMeshInfo(planar: planar);
            }

            if(firstLine.Count == 1)
            {
                if (secondLine.Count == 1)
                {
                    Debug.LogWarning("Error with MeshGenerator: Both lines have a length of 1");

                    return new TriangleMeshInfo(planar: planar);
                }

                return KnitLinesSmooth(point: firstLine.VerticesDirectly[0], line: secondLine, isClosed: isClosed, planar: planar);
            }
            else if(secondLine.Count == 1)
            {
                return KnitLinesSmooth(point: secondLine.VerticesDirectly[0], line: firstLine, isClosed: isClosed, planar: planar);
            }

            int offset1 = 0;
            int offset2 = firstLine.Count;

            returnValue.VerticesHolder.Add(firstLine);
            returnValue.VerticesHolder.Add(secondLine);

            int max1 = firstLine.Count - 1;
            int max2 = returnValue.VerticesHolder.Count - 1;

            if (isClosed)
            {
                float distance12 = (returnValue.VerticesHolder.VerticesDirectly[max1] - returnValue.VerticesHolder.VerticesDirectly[offset2]).magnitude;
                float distance21 = (returnValue.VerticesHolder.VerticesDirectly[max2] - returnValue.VerticesHolder.VerticesDirectly[offset1]).magnitude;

                if (distance12 < distance21)
                {
                    returnValue.Triangles.Add(new TriangleHolder(max1, offset2, max2));
                    returnValue.Triangles.Add(new TriangleHolder(max1, offset1, offset2));
                }
                else
                {
                    returnValue.Triangles.Add(new TriangleHolder(max1, offset1, max2));
                    returnValue.Triangles.Add(new TriangleHolder(offset1, offset2, max2));
                }
            }

            bool end1 = false;
            bool end2 = false;

            //int areas = firstLine.Count + secondLine.Count - 2;

            void CreateTriangleWithBaseOnLine2()
            {
                returnValue.Triangles.Add(new TriangleHolder(offset1, offset2 + 1, offset2));

                offset2++;

                if (offset2 == max2) end2 = true;
            }

            void CreateTriangleWithBaseOnLine1()
            {
                returnValue.Triangles.Add(new TriangleHolder(offset1, offset1 + 1, offset2));

                offset1++;

                if (offset1 == max1) end1 = true;
            }

            while (true)
            {
                if (!end1 && !end2)
                {
                    float distance12 = (returnValue.VerticesHolder.VerticesDirectly[offset2 + 1] - returnValue.VerticesHolder.VerticesDirectly[offset1]).magnitude;
                    float distance21 = (returnValue.VerticesHolder.VerticesDirectly[offset1 + 1] - returnValue.VerticesHolder.VerticesDirectly[offset2]).magnitude;

                    if (distance12 < distance21)
                    {
                        CreateTriangleWithBaseOnLine2();
                    }
                    else
                    {
                        CreateTriangleWithBaseOnLine1();
                    }
                }
                else if (end1 && !end2)
                {
                    CreateTriangleWithBaseOnLine2();
                }
                else if (!end1 && end2)
                {
                    CreateTriangleWithBaseOnLine1();
                }
                else
                {
                    break;
                }
            }

            return returnValue;
        }

        public static List<TriangleMeshInfo> ExtrudeLinearWithSharpCorners(VerticesHolder firstLine, Vector3 offset, bool closed)
        {
            List<TriangleMeshInfo> returnValue = new();

            List<Vector3> baseLine = firstLine.VerticesDirectly;

            for(int i = 0; i< baseLine.Count - 1; i++)
            {
                returnValue.Add(MeshesFromPoints.MeshFrom4Points(baseLine[i], baseLine[i + 1], baseLine[i + 1] + offset, baseLine[i] + offset));
            }

            if (closed)
            {
                returnValue.Add(MeshesFromPoints.MeshFrom4Points(baseLine[^1], baseLine[0], baseLine[0] + offset, baseLine[^1] + offset));
            }

            return returnValue;
        }

        public static TriangleMeshInfo ExtrudeLinearWithSmoothCorners(VerticesHolder firstLine, Vector3 offset, ShapeClosingType closeType, bool planar)
        {
            VerticesHolder secondLine = new(firstLine);

            secondLine.Move(offset);

            TriangleMeshInfo returnValue = KnitLinesSmooth(firstLine: firstLine, secondLine: secondLine, closingType: closeType, planar: planar);

            if(closeType == ShapeClosingType.closedWithSharpEdge)
            {

            }

            return returnValue;
        }

        public static List<TriangleMeshInfo> ExtrudeAlong(VerticesHolder sectionLine, VerticesHolder guideLine, bool sectionIsClosed, bool guideIsClosed, bool sharpGuideEdges)
        {
            List<TriangleMeshInfo> returnValue = new();

            List<VerticesHolder> sections = new();

            sections.Add(new VerticesHolder(sectionLine));

            Vector3 lookDirection = guideLine.VerticesDirectly[1] - guideLine.VerticesDirectly[0];

            if (lookDirection.magnitude != 0)
            {
                sections[^1].Rotate(Quaternion.LookRotation(lookDirection, Vector3.up)); //ToDo: Improve if closed
            }
            else
            {
                Debug.LogWarning("Error: Distance between points is 0");
            }
                
            sections[^1].Move(guideLine.VerticesDirectly[0]);

            for (int i = 1; i < guideLine.Count - 1; i++)
            {
                sections.Add(new VerticesHolder(sectionLine));

                Vector3 vecAhead = guideLine.VerticesDirectly[i + 1] - guideLine.VerticesDirectly[i];
                Vector3 vecBehind = guideLine.VerticesDirectly[i] - guideLine.VerticesDirectly[i - 1];

                Vector3 tanDirection = (vecBehind).normalized + (vecAhead).normalized;
                //Vector3 normDirection = -tanDirection / 2 + vecBehind;

                if(tanDirection.magnitude != 0)
                {
                    sections[^1].Rotate(Quaternion.LookRotation(forward: tanDirection));
                }
                else
                {
                    Debug.LogWarning("Error: Distance between points is 0");
                }

                sections[^1].Move(guideLine.VerticesDirectly[i]);
            }

            sections.Add(new VerticesHolder(sectionLine));
            sections[^1].Rotate(Quaternion.LookRotation(guideLine.VerticesDirectly[^1] - guideLine.VerticesDirectly[^2])); //ToDo: Improve if closed
            sections[^1].Move(guideLine.VerticesDirectly[^1]);

            if (sharpGuideEdges)
            {
                List<VerticesHolder> guideElements = new();

                for(int i = 0; i < sectionLine.Count; i++)
                {
                    guideElements.Add(new VerticesHolder());
                }

                for (int guideIndex = 0; guideIndex < guideElements.Count; guideIndex++)
                {
                    for (int sectionIndex = 0; sectionIndex < sections.Count; sectionIndex++)
                    {
                        guideElements[guideIndex].Add(sections[sectionIndex].VerticesDirectly[guideIndex]);
                    }
                }

                ShapeClosingType closingType;
                if (guideIsClosed)
                {
                    closingType = ShapeClosingType.closedWithSmoothEdge;
                }
                else
                {
                    closingType = ShapeClosingType.open;
                }

                for (int i = 0; i < guideElements.Count - 1; i++)
                {
                    returnValue.Add(KnitLinesSmooth(firstLine: guideElements[i], secondLine: guideElements[i + 1], closingType: closingType, planar: false));
                }

                if (sectionIsClosed)
                {
                    returnValue.Add(KnitLinesSmooth(firstLine: guideElements[^1], secondLine: guideElements[0], closingType: closingType, planar: false));
                }

            }
            else
            {
                TriangleMeshInfo smoothShape = KnitLinesWithProximityPreference(sections: sections, sectionsAreClosed: sectionIsClosed, shapeIsClosed: guideIsClosed, planar: false);
                
                smoothShape.FlipTriangles();

                returnValue.Add(smoothShape);

                //ToDo: add caps if closed
            }

            return returnValue;
        }

        public static TriangleMeshInfo AddWallBetween2Points(Vector3 firstClockwiseFloorPoint, Vector3 secondClockwiseFloorPoint, float wallHeight, Vector3 uvOffset)
        {
            float uvXOffset = Mathf.Sqrt(uvOffset.x * uvOffset.x + uvOffset.z * uvOffset.z);

            TriangleMeshInfo returnValue = MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: secondClockwiseFloorPoint - firstClockwiseFloorPoint, secondLine: Vector3.up * wallHeight, uvOffset: new Vector2(uvXOffset, uvOffset.y));

            returnValue.Move(firstClockwiseFloorPoint);

            //returnValue.FlipTriangles();

            /*
            TriangleMeshInfo returnValue = new TriangleMeshInfo();

            returnValue.VerticesHolder.Add(firstClockwiseFloorPoint);
            returnValue.VerticesHolder.Add(secondClockwiseFloorPoint);
            returnValue.VerticesHolder.Add(secondClockwiseFloorPoint + Vector3.up * wallHeight);
            returnValue.VerticesHolder.Add(firstClockwiseFloorPoint + Vector3.up * wallHeight);

            returnValue.Triangles.Add(new TriangleHolder(0, 1, 2));
            returnValue.Triangles.Add(new TriangleHolder(0, 2, 3));
            */

            return returnValue;
        }
        public static List<TriangleMeshInfo> AddVerticalWallsBetweenMultiplePoints(List<Vector3> floorPointsInClockwiseOrder, float height, bool closed, Vector3 uvOffset)
        {
            List<TriangleMeshInfo> returnValue = new();

            for (int i = 0; i < floorPointsInClockwiseOrder.Count - 1; i++)
            {
                returnValue.Add(AddWallBetween2Points(floorPointsInClockwiseOrder[i], floorPointsInClockwiseOrder[i + 1], height, uvOffset));
            }

            if (closed)
            {
                returnValue.Add(AddWallBetween2Points(floorPointsInClockwiseOrder[^1], floorPointsInClockwiseOrder[0], height, uvOffset));
            }

            return returnValue;
        }

        /*
        public static List<TriangleMeshInfo> AddVerticalWallsBetweenMultiplePointsAsList(List<Vector3> floorPointsInClockwiseOrder, float height, bool closed, Vector3 uvOffset)
        {
            List<TriangleMeshInfo> returnList = new();

            for (int i = 0; i < floorPointsInClockwiseOrder.Count - 1; i++)
            {
                returnList.Add(AddWallBetween2Points(floorPointsInClockwiseOrder[i], floorPointsInClockwiseOrder[i + 1], height, uvOffset));
            }

            if (closed)
            {
                returnList.Add(AddWallBetween2Points(floorPointsInClockwiseOrder[^1], floorPointsInClockwiseOrder[0], height, uvOffset));
            }

            return returnList;
        }
        */
    }

    public static class MeshesFromPoints
    {
        public static TriangleMeshInfo MeshFrom3Points(Vector3 p0, Vector3 p1, Vector3 p2)
        {
            TriangleMeshInfo returnValue = new(planar: true);

            returnValue.VerticesHolder.Add(p0);
            returnValue.VerticesHolder.Add(p1);
            returnValue.VerticesHolder.Add(p2);

            returnValue.Triangles.Add(new TriangleHolder(0, 1, 2));

            //UV guesstimation
            Vector2 firstOffset = Vector2.right * (p1 - p0).magnitude;
            Vector2 secondOffset = Vector2.up * (p2 - p0).magnitude;

            returnValue.UVs.Add(Vector2.zero);
            returnValue.UVs.Add(firstOffset);
            returnValue.UVs.Add(secondOffset);

            return returnValue;
        }

        public static TriangleMeshInfo MeshFrom4Points(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            TriangleMeshInfo returnValue = new(planar: true);

            returnValue.VerticesHolder.Add(p0);
            returnValue.VerticesHolder.Add(p1);
            returnValue.VerticesHolder.Add(p2);
            returnValue.VerticesHolder.Add(p3);

            returnValue.Triangles.Add(new TriangleHolder(0, 1, 2));
            returnValue.Triangles.Add(new TriangleHolder(0, 2, 3));

            //UV guesstimation
            Vector2 firstOffset = Vector2.right * (p1 - p0).magnitude;
            Vector2 secondOffset = Vector2.up * (p3 - p0).magnitude;
            
            returnValue.UVs.Add(Vector2.zero);
            returnValue.UVs.Add(firstOffset);
            returnValue.UVs.Add(firstOffset + secondOffset);
            returnValue.UVs.Add(secondOffset);

            return returnValue;
        }
    }

    
}


