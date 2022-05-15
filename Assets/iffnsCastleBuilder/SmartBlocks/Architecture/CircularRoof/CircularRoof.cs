using iffnsStuff.iffnsBaseSystemForUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class CircularRoof : OnFloorObject
    {
        //Assignments

        //Build parameters
        MailboxLineVector2Int CenterPositionParam;
        MailboxLineVector2Int OuterRadiiParam;
        MailboxLineDistinctUnnamed EdgesBetweenParam;
        MailboxLineDistinctNamed AngleParam;
        MailboxLineRanged ThicknessParam;
        MailboxLineRanged HeightParam;
        MailboxLineRanged HeightOvershootParam;

        MailboxLineMaterial OutsideMaterial;
        MailboxLineMaterial InsideMaterial;
        MailboxLineMaterial WrapperMaterial;

        BlockGridRectangleOrganizer ModificationNodeOrganizer;

        public override ModificationOrganizer Organizer
        {
            get
            {
                return ModificationNodeOrganizer;
            }
        }

        public Vector2Int CenterPosition
        {
            get
            {
                return CenterPositionParam.Val;
            }
            set
            {
                CenterPositionParam.Val = value;
                ApplyBuildParameters();
            }
        }

        public Vector2Int Radii
        {
            get
            {
                return OuterRadiiParam.Val;
            }
            set
            {
                OuterRadiiParam.Val = value;
                ApplyBuildParameters();
            }
        }

        public int EdgesBetween
        {
            get
            {
                return EdgesBetweenParam.Val;
            }
            set
            {
                EdgesBetweenParam.Val = value;
                ApplyBuildParameters();
            }
        }

        public float Thickness
        {
            get
            {
                return ThicknessParam.Val;
            }
            set
            {
                ThicknessParam.Val = value;
                ApplyBuildParameters();
            }
        }

        public float Height
        {
            get
            {
                return HeightParam.Val;
            }
            set
            {
                HeightParam.Val = value;
                ApplyBuildParameters();
            }
        }

        public float HeightOvershoot
        {
            get
            {
                return HeightOvershootParam.Val;
            }
            set
            {
                HeightOvershootParam.Val = value;
                ApplyBuildParameters();
            }
        }

        Vector2Int OuterRadiiAbsolute
        {
            get
            {
                return new Vector2Int(Mathf.Abs(Radii.x), Mathf.Abs(Radii.y));
            }
        }

        public enum Angles
        {
            Deg90,
            Deg180,
            Deg270,
            Deg360
        }

        void SetupAngleTypeParam()
        {
            List<string> enumString = new List<string>();

            int enumValues = System.Enum.GetValues(typeof(Angles)).Length;

            for (int i = 0; i < enumValues; i++)
            {
                Angles type = (Angles)i;

                enumString.Add(type.ToString().Replace("Deg", "") + "°");
            }

            AngleParam = new MailboxLineDistinctNamed(
                "Roof angle",
                CurrentMailbox,
                Mailbox.ValueType.buildParameter,
                enumString,
                0);
        }

        public Angles Angle
        {
            get
            {
                Angles returnValue = (Angles)AngleParam.Val;

                return returnValue;
            }
            set
            {
                AngleParam.Val = (int)value;
                ApplyBuildParameters();
            }
        }

        public override void Setup(IBaseObject linkedFloor)
        {
            base.Setup(linkedFloor);

            CenterPositionParam = new MailboxLineVector2Int(name: "Center position", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter);
            OuterRadiiParam = new MailboxLineVector2Int(name: "Outer Radii", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter);
            EdgesBetweenParam = new MailboxLineDistinctUnnamed(name: "Edges between", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, Max: 16, Min: 0, DefaultValue: 8);
            ThicknessParam = new MailboxLineRanged(name: "Thickness", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, Max: 1f / 3, Min: 0.001f, DefaultValue: 0.1f);
            HeightParam = new MailboxLineRanged(name: "Height [m]", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, Max: 10f, Min: 1f, DefaultValue: 3f);
            HeightOvershootParam = new MailboxLineRanged(name: "Height overshoot [m]", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, Max: 1, Min: 0, DefaultValue: 0.1f);

            OutsideMaterial = new MailboxLineMaterial(name: "Outside material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultRoof);
            InsideMaterial = new MailboxLineMaterial(name: "Inside material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultCeiling);
            WrapperMaterial = new MailboxLineMaterial(name: "Wrapper material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultPlaster);

            SetupAngleTypeParam();

            BlockGridPositionModificationNode firstNode = ModificationNodeLibrary.NewBlockGridPositionModificationNode;
            firstNode.Setup(linkedObject: this, value: CenterPositionParam);
            FirstPositionNode = firstNode;

            BlockGridPositionModificationNode secondNode = ModificationNodeLibrary.NewBlockGridPositionModificationNode;
            secondNode.Setup(linkedObject: this, value: OuterRadiiParam, relativeReferenceHolder: CenterPositionParam);
            SecondPositionNode = secondNode;

            ModificationNodeOrganizer = new BlockGridRectangleOrganizer(linkedObject: this, firstNode: firstNode, secondNode: secondNode);

            SetupEditButtons();
        }

        public void CompleteSetupWithBuildParameters(IBaseObject linkedFloor, Vector2Int centerPosition, Vector2Int outerRadii, int edgesBetween, float thickness, float height, float heightOvershoot)
        {
            Setup(linkedFloor);

            CenterPositionParam.Val = centerPosition;
            OuterRadiiParam.Val = outerRadii;
            EdgesBetweenParam.Val = edgesBetween;
            ThicknessParam.Val = thickness;
            HeightParam.Val = height;
            HeightOvershootParam.Val = heightOvershoot;
        }



        public override void ResetObject()
        {
            baseReset();

            ResetEditButtons();
        }

        public override void InternalUpdate()
        {
            NonOrderedInternalUpdate();
        }

        public override void PlaytimeUpdate()
        {
            NonOrderedPlaytimeUpdate();
        }

        public override void ApplyBuildParameters()
        {
            failed = false;

            TriangleMeshInfo RoofOutside;
            TriangleMeshInfo RoofInside;
            TriangleMeshInfo BottomEdge;
            TriangleMeshInfo RightEdge = new TriangleMeshInfo();
            TriangleMeshInfo LeftEdge = new TriangleMeshInfo();

            void FinishMeshes()
            {
                /*
                TriangleMeshInfo testInfo = MeshGenerator.FilledShapes.RectangleAtCorner(Vector3.right, Vector3.up, Vector2.zero);

                testInfo.MaterialReference = OutsideMaterial;

                StaticMeshManager.AddTriangleInfo(testInfo);
                */
                //StaticMeshManager.AddTriangleInfo(MeshGenerator.FilledShapes.BoxAroundCenter(Vector3.one));

                RoofOutside.MaterialReference = OutsideMaterial;
                RoofInside.MaterialReference = InsideMaterial;
                BottomEdge.MaterialReference = WrapperMaterial;

                StaticMeshManager.AddTriangleInfo(RoofOutside);
                StaticMeshManager.AddTriangleInfo(RoofInside);
                StaticMeshManager.AddTriangleInfo(BottomEdge);


                if (Angle != Angles.Deg360)
                {
                    RightEdge.MaterialReference = WrapperMaterial;
                    LeftEdge.MaterialReference = WrapperMaterial;

                    StaticMeshManager.AddTriangleInfo(RightEdge);
                    StaticMeshManager.AddTriangleInfo(LeftEdge);
                }


                BuildAllMeshes();
            }

            ModificationNodeOrganizer.SetLinkedObjectPositionAndOrientation(raiseToFloor: false);

            Vector2 outerSize = ModificationNodeOrganizer.ObjectOrientationSize;
            Vector2 innerSize = outerSize - Vector2.one * Thickness;

            float ratio = Height / (outerSize.x + outerSize.y) / 2;

            Vector2 OuterRadii = new Vector2(outerSize.x + HeightOvershoot / ratio, outerSize.y + HeightOvershoot / ratio);

            Vector2 InnerRadii = new Vector2(outerSize.x + HeightOvershoot / ratio - Thickness, outerSize.y + HeightOvershoot / ratio - Thickness);

            VerticesHolder outerLine = new VerticesHolder();

            bool isClosed = true;

            switch (Angle)
            {
                case Angles.Deg90:
                    outerLine = MeshGenerator.Lines.ArcAroundZ(radius: 1, angleDeg: 90, numberOfEdges: EdgesBetween + 2);
                    isClosed = false;
                    break;
                case Angles.Deg180:
                    outerLine = MeshGenerator.Lines.ArcAroundZ(radius: 1, angleDeg: 180, numberOfEdges: EdgesBetween * 2 + 3);
                    isClosed = false;
                    break;
                case Angles.Deg270:
                    outerLine = MeshGenerator.Lines.ArcAroundZ(radius: 1, angleDeg: 270, numberOfEdges: EdgesBetween * 3 + 4);
                    isClosed = false;
                    break;
                case Angles.Deg360:
                    outerLine = MeshGenerator.Lines.FullCircle(radius: 1, numberOfEdges: EdgesBetween * 4 + 4);
                    isClosed = true;
                    break;
                default:
                    Debug.Log("Angle type not defined");
                    break;
            }

            outerLine.Rotate(Quaternion.LookRotation(Vector3.up, Vector3.right));
            outerLine.Move(Vector3.down * HeightOvershoot);
            VerticesHolder innerLine = outerLine.Clone;

            outerLine.Scale(new Vector3(OuterRadii.x, 1, OuterRadii.y));
            innerLine.Scale(new Vector3(InnerRadii.x, 1, InnerRadii.y));

            RoofOutside = MeshGenerator.MeshesFromLines.KnitLines(point: Vector3.up * Height, line: outerLine, isClosed: isClosed);
            RoofOutside.FlipTriangles();

            /*
            List<Vector3> points = new List<Vector3>();
            points.Add(Vector3.up * Height);
            points.AddRange(outerLine.Vertices);
            points.Add(outerLine.Vertices[0]);
            RoofOutside = MeshGenerator.FilledShapes.PointsClockwiseAroundFirstPoint(points: points);
            */

            RoofInside = MeshGenerator.MeshesFromLines.KnitLines(point: Vector3.up * (Height - Thickness * ratio), line: innerLine, isClosed: isClosed);

            BottomEdge = MeshGenerator.MeshesFromLines.KnitLines(firstLine: outerLine, secondLine: innerLine, closingType: MeshGenerator.ShapeClosingType.closedWithSmoothEdge, smoothTransition: true);
            BottomEdge.FlipTriangles();

            //Side edges



            if (Angle != Angles.Deg360)
            {
                RightEdge = MeshGenerator.MeshesFromPoints.MeshFrom4Points(
                    Vector3.up * Height,
                    Vector3.up * (Height - Thickness * ratio),
                    Vector3.down * HeightOvershoot + Vector3.forward * (outerSize.y + HeightOvershoot / ratio - Thickness),
                    Vector3.down * HeightOvershoot + Vector3.forward * (outerSize.y + HeightOvershoot / ratio)
                    );

                switch (Angle)
                {
                    case Angles.Deg90:
                        LeftEdge = MeshGenerator.MeshesFromPoints.MeshFrom4Points(
                            Vector3.up * Height,
                            Vector3.up * (Height - Thickness * ratio),
                            Vector3.down * HeightOvershoot + Vector3.forward * (outerSize.x + HeightOvershoot / ratio - Thickness),
                            Vector3.down * HeightOvershoot + Vector3.forward * (outerSize.x + HeightOvershoot / ratio)
                            );
                        LeftEdge.FlipTriangles();
                        LeftEdge.Rotate(Quaternion.Euler(Vector3.up * 90));
                        break;
                    case Angles.Deg180:
                        LeftEdge = RightEdge.CloneFlipped;
                        LeftEdge.Rotate(Quaternion.Euler(Vector3.up * 180));
                        break;
                    case Angles.Deg270:
                        LeftEdge = MeshGenerator.MeshesFromPoints.MeshFrom4Points(
                            Vector3.up * Height,
                            Vector3.up * (Height - Thickness * ratio),
                            Vector3.down * HeightOvershoot + Vector3.forward * (outerSize.x + HeightOvershoot / ratio - Thickness),
                            Vector3.down * HeightOvershoot + Vector3.forward * (outerSize.x + HeightOvershoot / ratio)
                            );
                        LeftEdge.FlipTriangles();
                        LeftEdge.Rotate(Quaternion.Euler(Vector3.up * 270));
                        break;
                    case Angles.Deg360:
                        break;
                    default:
                        break;
                }
            }

            FinishMeshes();
        }

        void SetupEditButtons()
        {
            AddEditButtonFunctionToBeginning(function: new SingleButtonBaseEditFunction(buttonName: "Copy Flip Diagonally", delegate { CopyFlipDiagonally(); }));
            AddEditButtonFunctionToBeginning(function: new SingleButtonBaseEditFunction(buttonName: "Copy Flip Vertical", delegate { CopyFlipY(); }));
            AddEditButtonFunctionToBeginning(function: new SingleButtonBaseEditFunction(buttonName: "Copy Flip Horizontal", delegate { CopyFlipX(); }));
            AddEditButtonFunctionToBeginning(function: new SingleButtonBaseEditFunction(buttonName: "Flip Vertical", delegate { ModificationNodeOrganizer.FlipVertical(); }));
            AddEditButtonFunctionToBeginning(function: new SingleButtonBaseEditFunction(buttonName: "Flip Horizontal", delegate { ModificationNodeOrganizer.FlipHorizontal(); }));
            AddEditButtonFunctionToBeginning(function: new SingleButtonBaseEditFunction(buttonName: "Rotate Counter-Clockwise", delegate { ModificationNodeOrganizer.RotateCounterClockwise(); }));
            AddEditButtonFunctionToBeginning(function: new SingleButtonBaseEditFunction(buttonName: "Rotate Clockwise", delegate { ModificationNodeOrganizer.RotateClockwise(); }));
        }

        void CopyFlipX()
        {
            CircularRoof newArc = CreateNewHorizontalArc();

            newArc.CompleteSetupWithBuildParameters(
                linkedFloor: LinkedFloor,
                centerPosition: new Vector2Int(CenterPosition.x - System.Math.Sign(Radii.x), CenterPosition.y),
                outerRadii: new Vector2Int(-Radii.x, Radii.y),
                edgesBetween: EdgesBetween,
                thickness: Thickness,
                height: Height,
                heightOvershoot: HeightOvershoot);

            newArc.ApplyBuildParameters();
        }

        void CopyFlipY()
        {
            CircularRoof newArc = CreateNewHorizontalArc();

            newArc.CompleteSetupWithBuildParameters(
                linkedFloor: LinkedFloor,
                centerPosition: new Vector2Int(CenterPosition.x, CenterPosition.y - System.Math.Sign(Radii.y)),
                outerRadii: new Vector2Int(Radii.x, -Radii.y),
                edgesBetween: EdgesBetween,
                thickness: Thickness,
                height: Height,
                heightOvershoot: HeightOvershoot);

            newArc.ApplyBuildParameters();
        }

        void CopyFlipDiagonally()
        {
            CircularRoof newArc = CreateNewHorizontalArc();

            newArc.CompleteSetupWithBuildParameters(
                linkedFloor: LinkedFloor,
                centerPosition: new Vector2Int(CenterPosition.x - System.Math.Sign(Radii.x), CenterPosition.y - System.Math.Sign(Radii.y)),
                outerRadii: new Vector2Int(-Radii.x, -Radii.y),
                edgesBetween: EdgesBetween,
                thickness: Thickness,
                height: Height,
                heightOvershoot: HeightOvershoot);

            newArc.ApplyBuildParameters();
        }

        public override void MoveOnGrid(Vector2Int offset)
        {
            ModificationNodeOrganizer.MoveOnGrid(offset: offset);
        }

        CircularRoof CreateNewHorizontalArc()
        {
            CircularRoof returnValue = Instantiate(ResourceLibrary.TryGetTemplateFromStringIdentifier(IdentifierString) as CircularRoof);

            return returnValue;
        }
    }
}