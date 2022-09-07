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
        MailboxLineBool RaiseToFloorParam;

        MailboxLineMaterial OutsideMaterial;
        MailboxLineMaterial InsideMaterial;
        MailboxLineMaterial WrapperMaterial;

        NodeGridRectangleOrganizer ModificationNodeOrganizer;

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
            List<string> enumString = new();

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

        public override bool RaiseToFloor
        {
            get
            {
                return RaiseToFloorParam.Val;
            }
        }

        public override bool IsStructural
        {
            get
            {
                return true;
            }
        }

        public override float ModificationNodeHeight
        {
            get
            {
                if (RaiseToFloor)
                {
                    return Height + LinkedFloor.BottomFloorHeight;
                }
                else
                {
                    return Height;
                }
            }
        }

        public override void Setup(IBaseObject linkedFloor)
        {
            base.Setup(linkedFloor);

            CenterPositionParam = new MailboxLineVector2Int(name: "Center position", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter);
            OuterRadiiParam = new MailboxLineVector2Int(name: "Outer Radii", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter);
            EdgesBetweenParam = new MailboxLineDistinctUnnamed(name: "Edges between", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, Max: 16, Min: 0, DefaultValue: 8);
            ThicknessParam = new MailboxLineRanged(name: "Thickness [m]", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, Max: 1f / 3, Min: 0.001f, DefaultValue: 0.1f);
            HeightParam = new MailboxLineRanged(name: "Height [m]", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, Max: 10f, Min: 1f, DefaultValue: 2f);
            HeightOvershootParam = new MailboxLineRanged(name: "Height overshoot [m]", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, Max: 1, Min: 0, DefaultValue: 0.1f);
            RaiseToFloorParam = new MailboxLineBool(name: "Raise to floor", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: true);

            OutsideMaterial = new MailboxLineMaterial(name: "Outside material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultRoof);
            InsideMaterial = new MailboxLineMaterial(name: "Inside material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultCeiling);
            WrapperMaterial = new MailboxLineMaterial(name: "Wrapper material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultPlaster);

            SetupAngleTypeParam();

            NodeGridPositionModificationNode firstNode = ModificationNodeLibrary.NewNodeGridPositionModificationNode;
            firstNode.Setup(linkedObject: this, value: CenterPositionParam);
            FirstPositionNode = firstNode;

            NodeGridPositionModificationNode secondNode = ModificationNodeLibrary.NewNodeGridPositionModificationNode;
            secondNode.Setup(linkedObject: this, value: OuterRadiiParam, relativeReferenceHolder: CenterPositionParam);
            SecondPositionNode = secondNode;

            ModificationNodeOrganizer = new NodeGridRectangleOrganizer(linkedObject: this, firstNode: firstNode, secondNode: secondNode);

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
            base.ApplyBuildParameters();

            //Check validity
            if (Failed)
            {
                return;
            }

            if (ModificationNodeOrganizer.ObjectOrientationGridSize.x == 0)
            {
                Failed = true;
                return;
            }

            //Define mesh
            TriangleMeshInfo RoofOutside;
            TriangleMeshInfo RoofInside;
            TriangleMeshInfo BottomEdge;
            TriangleMeshInfo RightEdge = new(planar: true);
            TriangleMeshInfo LeftEdge = new(planar: true);

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

                StaticMeshManager.AddTriangleInfoIfValid(RoofOutside);
                StaticMeshManager.AddTriangleInfoIfValid(RoofInside);
                StaticMeshManager.AddTriangleInfoIfValid(BottomEdge);


                if (Angle != Angles.Deg360)
                {
                    RightEdge.MaterialReference = WrapperMaterial;
                    LeftEdge.MaterialReference = WrapperMaterial;

                    StaticMeshManager.AddTriangleInfoIfValid(RightEdge);
                    StaticMeshManager.AddTriangleInfoIfValid(LeftEdge);
                }

                BuildAllMeshes();
            }


            if (ModificationNodeOrganizer.ObjectOrientationGridSize.x == 0)
            {
                Failed = true;
                return;
            }

            Vector2 outerSize = ModificationNodeOrganizer.ObjectOrientationSize;

            float ratio = Height / (outerSize.x + outerSize.y) / 2;

            Vector2 OuterRadii = new(outerSize.x + HeightOvershoot / ratio, outerSize.y + HeightOvershoot / ratio);

            Vector2 InnerRadii = new(outerSize.x + HeightOvershoot / ratio - Thickness, outerSize.y + HeightOvershoot / ratio - Thickness);

            VerticesHolder outerLine = new();

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

            VerticesHolder outerLineCone = outerLine.Clone;

            if (isClosed)
            {
                outerLineCone.Add(outerLine.VerticesDirectly[0]);
            }

            //Outside with correct UV
            float outerCircumence = 0;

            List<float> uvXPoints = new();

            uvXPoints.Add(0);

            for(int i = 0; i< outerLineCone.VerticesDirectly.Count - 1; i++)
            {
                float offset = (outerLineCone.VerticesDirectly[i + 1] - outerLineCone.VerticesDirectly[i]).magnitude;
                outerCircumence += offset;
                uvXPoints.Add(outerCircumence);
            }

            //float uvCircumfence = Mathf.Round(outerCircumence);
            float uvCircumfence = outerCircumence * 0.75f; //Slight multiplication to reduce distortion

            if (uvCircumfence < 1) uvCircumfence = 1;

            float uvScaleFactor = uvCircumfence / outerCircumence;

            for(int i = 0; i< uvXPoints.Count; i++)
            {
                uvXPoints[i] *= uvScaleFactor;
            }

            int verticalSteps = 5;

            float heightRatio = 1f / verticalSteps;

            List<VerticesHolder> Circles = new();

            Circles.Add(outerLineCone);

            float uvYOffset = (Mathf.Sqrt(Height * Height + OuterRadii.x * OuterRadii.x) + Mathf.Sqrt(Height * Height + OuterRadii.y * OuterRadii.y)) * 0.5f / verticalSteps;
            List<Vector2> UVs = new();

            foreach (float x in uvXPoints)
            {
                UVs.Add(new Vector2(-x, 0));
            }

            for (int i = 1; i< verticalSteps + 1; i++)
            {
                VerticesHolder nextLine = outerLineCone.Clone;
                nextLine.Scale((1 - heightRatio * i + MathHelper.SmallFloat * 5) * Vector3.one);
                nextLine.Move(heightRatio * i * Height * Vector3.up);
                Circles.Add(nextLine);

                float heigh = uvYOffset * i;

                foreach (float x in uvXPoints)
                {
                    UVs.Add(new Vector2(-x, heigh));
                }
            }

            RoofOutside = MeshGenerator.MeshesFromLines.KnitLinesWithProximityPreference(sections: Circles, sectionsAreClosed: false, shapeIsClosed: false, planar: false);

            RoofOutside.UVs = UVs;

            //Inside
            Vector3 innderScaleFactor = new(
                x: InnerRadii.x / OuterRadii.x,
                y: (Height - Thickness * ratio) / Height,
                z: InnerRadii.y / OuterRadii.y);

            RoofInside = RoofOutside.CloneFlipped;
            RoofInside.Scale(innderScaleFactor);

            //Bottom edge
            if (isClosed)
            {
                BottomEdge = MeshGenerator.MeshesFromLines.KnitLinesSmooth(firstLine: outerLine, secondLine: innerLine, closingType: MeshGenerator.ShapeClosingType.closedWithSmoothEdge, planar: true);
            }
            else
            {
                BottomEdge = MeshGenerator.MeshesFromLines.KnitLinesSmooth(firstLine: outerLine, secondLine: innerLine, closingType: MeshGenerator.ShapeClosingType.open, planar: true);
            }
            
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