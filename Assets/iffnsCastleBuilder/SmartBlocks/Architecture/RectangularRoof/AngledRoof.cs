using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using iffnsStuff.iffnsBaseSystemForUnity;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class AngledRoof : MonoBehaviour
    {
        public Vector2 size = Vector2.one;
        public float height = 1;
        public float wallThickness = 1f / 3f;
        public float roofOvershoot = 0.3f;
        float roofThickness = 0.1f;

        public GameObject Peak;
        public GameObject RoofXPositive;
        public GameObject RoofXNegative;
        public GameObject WallZPositive;
        public GameObject WallZNegative;

        RectangularRoof.RoofTypes roofType;

        RectangularRoof linkedRoof;

        MailboxLineMaterial materialOutside;
        MailboxLineMaterial materialInside;
        MailboxLineMaterial materialWrapper;

        public void SetBuildParameters(RectangularRoof linkedRoof, Vector2 size, float height, float wallThickness, float roofOvershoot, Transform UVBaseObject, RectangularRoof.RoofTypes roofType, MailboxLineMaterial materialOutside, MailboxLineMaterial materialInside, MailboxLineMaterial materialWrapper)
        {
            this.roofType = roofType;

            this.size = size;
            this.height = height;
            this.wallThickness = wallThickness;
            this.roofOvershoot = roofOvershoot;

            this.linkedRoof = linkedRoof;

            this.materialOutside = materialOutside;
            this.materialInside = materialInside;
            this.materialWrapper = materialWrapper;

            ApplyBuildParameters(UVBaseObject);
        }



        public void ApplyBuildParameters(Transform baseObject)
        {
            switch (roofType)
            {
                case RectangularRoof.RoofTypes.FullAngledSquareRoof:
                    UpdateFullRoof(baseObject);
                    break;
                case RectangularRoof.RoofTypes.HalfAngledSquareRoof:
                    UpdateHalfRoof(baseObject);
                    break;
                default:
                    break;
            }
        }

        void UpdateHalfRoof(Transform baseObject)
        {
            TriangleMeshInfo RoofOutside = new TriangleMeshInfo();
            TriangleMeshInfo RoofInside = new TriangleMeshInfo();
            TriangleMeshInfo RoofWrapper = new TriangleMeshInfo();
            TriangleMeshInfo FrontWall = new TriangleMeshInfo();
            TriangleMeshInfo BackWall = new TriangleMeshInfo();

            void FinishMesh()
            {
                RoofOutside.MaterialReference = materialOutside;
                RoofInside.MaterialReference = materialInside;
                RoofWrapper.MaterialReference = materialWrapper;

                linkedRoof.AddStaticMesh(RoofOutside);
                linkedRoof.AddStaticMesh(RoofInside);
                linkedRoof.AddStaticMesh(RoofWrapper);
                linkedRoof.AddStaticMesh(FrontWall);
                linkedRoof.AddStaticMesh(FrontWall);
                linkedRoof.AddStaticMesh(BackWall);
            }
            //Base parameters
            float roofAngle = Mathf.Atan(height / size.x);
            float topAngle = Mathf.PI * 0.5f - roofAngle;

            float xOffset = roofThickness / Mathf.Sin(roofAngle);
            float heightOffset = roofThickness / Mathf.Sin(topAngle);

            //Roof outside
            RoofOutside = MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.forward * size.y, secondLine: new Vector3(-size.x, height, 0), UVOffset: Vector2.zero);
            RoofOutside.Move(Vector3.right * size.x);

            //Roof inside
            RoofInside = MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.back * size.y, secondLine: new Vector3(-size.x + xOffset, height - heightOffset, 0), UVOffset: Vector2.zero);
            RoofInside.Move(new Vector3(size.x - xOffset, 0, size.y));

            //Top wrapper
            TriangleMeshInfo tempWrapper = MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.back * size.y, secondLine: Vector3.up * heightOffset, UVOffset: Vector2.zero);
            tempWrapper.Move(new Vector3(0, height - heightOffset, size.y));
            RoofWrapper.Add(tempWrapper);

            //Bottom wrapper
            tempWrapper = MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.forward * size.y, secondLine: Vector3.right * xOffset, UVOffset: Vector2.zero);
            tempWrapper.Move(Vector3.right * (size.x - xOffset));
            RoofWrapper.Add(tempWrapper);

            //Side wrapper 1
            tempWrapper = MeshGenerator.FilledShapes.PointsClockwiseAroundFirstPoint(new List<Vector3>()
        {
            new Vector3(0, height - heightOffset, 0),
            new Vector3(0, height, 0),
            new Vector3(size.x, 0, 0),
            new Vector3(size.x - xOffset, 0, 0),
        }
            );
            RoofWrapper.Add(tempWrapper);

            //Sided wrapper 2
            tempWrapper = tempWrapper.CloneFlipped;
            tempWrapper.Move(Vector3.forward * size.y);
            RoofWrapper.Add(tempWrapper);

            //tempWrapper = MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: , secondLine: , UVOffset: Vector2.zero);



            FinishMesh();

            //Old code
            /*
            float h = height;
            float t = roofThickness;
            float w = size.x;

            float h2 = h * h;
            float t2 = t * t;
            float w2 = w * w;

            float rootTerm = Mathf.Sqrt(h2 * t2 * (h2 - t2 + w2));

            float xDiff = (rootTerm + t2 * w) / (h2 - t2);
            float yDiff = (h2 * t2 - w * rootTerm) / (h * t2 - h * w2);

            float innerWidth = size.x;
            float innerHeight = height - yDiff;
            float outerWidth = size.x + xDiff;
            float outerHeight = height;

            //Roof outside
            Vector3 outerRoofOffset = new Vector3(outerWidth, -outerHeight);
            outerRoofOffset += outerRoofOffset.normalized * roofOvershoot;

            RoofOutside = MeshGenerator.FilledShapes.RectangleAtCorner(Vector3.back * size.y, secondLine: outerRoofOffset, UVOffset: Vector2.zero);
            //RoofOutside.Move(Vector3.up * outerHeight + Vector3.forward * size.y * 0.5f);
            //RoofOutside.Move(new Vector3(size.x * 0.5f, outerHeight, size.y * 0.5f));
            RoofOutside.RotateAllUVsCWAroundOrigin(angleDeg: 180);
            RoofOutside.Move(new Vector3(0, outerHeight, size.y));

            Vector3 innerRoofOffset = new Vector3(innerWidth, -innerHeight);
            innerRoofOffset += innerRoofOffset.normalized * roofOvershoot;
            RoofInside = MeshGenerator.FilledShapes.RectangleAtCorner(Vector3.forward * size.y, secondLine: innerRoofOffset, UVOffset: Vector2.zero);
            //RoofInside.Move(Vector3.up * innerHeight + Vector3.back * size.y * 0.5f);
            //RoofInside.Move(new Vector3(size.x * 0.5f, innerHeight, size.y * 0.5f));
            RoofInside.RotateAllUVsCWAroundOrigin(angleDeg: 180);
            RoofInside.Move(new Vector3(0, innerHeight, 0));

            //RoofWrapper.Move(new Vector3(size.x * 0.5f, 0, size.y * 0.5f));
            //FrontWall.Move(new Vector3(size.x * 0.5f, 0, size.y * 0.5f));
            //BackWall.Move(new Vector3(size.x * 0.5f, 0, size.y * 0.5f));
            FinishMesh();
            */
        }

        void UpdateFullRoof(Transform originObject)
        {
            TriangleMeshInfo RoofOutside = new TriangleMeshInfo();
            TriangleMeshInfo RoofInside = new TriangleMeshInfo();
            TriangleMeshInfo RoofWrapper = new TriangleMeshInfo();
            TriangleMeshInfo FrontWall = new TriangleMeshInfo();
            TriangleMeshInfo BackWall = new TriangleMeshInfo();

            void FinishMesh()
            {
                RoofOutside.MaterialReference = materialOutside;
                RoofInside.MaterialReference = materialInside;
                RoofWrapper.MaterialReference = materialWrapper;

                linkedRoof.AddStaticMesh(RoofOutside);
                linkedRoof.AddStaticMesh(RoofInside);
                linkedRoof.AddStaticMesh(RoofWrapper);
                linkedRoof.AddStaticMesh(FrontWall);
                linkedRoof.AddStaticMesh(BackWall);
            }

            float halfWidth = size.x * 0.5f;
            float roofAngle = Mathf.Atan(height / size.x);
            float topAngle = Mathf.PI * 0.5f - roofAngle;

            float xOffset = roofThickness / Mathf.Sin(roofAngle);
            float heightOffset = roofThickness / Mathf.Sin(topAngle);

            TriangleMeshInfo tempShape;

            //Roof outside
            tempShape = MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.forward * size.y, secondLine: new Vector3(-halfWidth, height, 0), UVOffset: Vector2.zero);
            tempShape.Move(Vector3.right * size.x);
            RoofOutside.Add(tempShape);

            tempShape = MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.back * size.y, secondLine: new Vector3(halfWidth, height, 0), UVOffset: Vector2.zero);
            tempShape.Move(Vector3.forward * size.y);
            RoofOutside.Add(tempShape);

            //Roof inside
            tempShape = MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.back * size.y, secondLine: new Vector3(-halfWidth + xOffset, height - heightOffset, 0), UVOffset: Vector2.zero);
            tempShape.Move(new Vector3(size.x - xOffset, 0, size.y));
            RoofInside.Add(tempShape);

            tempShape = MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.forward * size.y, secondLine: new Vector3(halfWidth - xOffset, height - heightOffset, 0), UVOffset: Vector2.zero);
            tempShape.Move(Vector3.right * xOffset);
            RoofInside.Add(tempShape);

            //Side wrapper 1
            tempShape = MeshGenerator.FilledShapes.PointsClockwiseAroundFirstPoint(new List<Vector3>()
        {
            new Vector3(halfWidth, height - heightOffset, 0),
            new Vector3(xOffset, 0, 0),
            new Vector3(0, 0, 0),
            new Vector3(halfWidth, height, 0),
            new Vector3(size.x, 0, 0),
            new Vector3(size.x - xOffset, 0, 0)
        }
            );
            RoofWrapper.Add(tempShape);

            //Side wrapper 2
            tempShape = tempShape.CloneFlipped;
            tempShape.Move(Vector3.forward * size.y);
            RoofWrapper.Add(tempShape);

            /*
            RoofOutside = MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.forward * size.y, secondLine: new Vector3(-size.x, height, 0), UVOffset: Vector2.zero);
            RoofOutside.Move(Vector3.right * size.x);

            RoofInside = MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.back * size.y, secondLine: new Vector3(-size.x + xOffset, height - heightOffset, 0), UVOffset: Vector2.zero);
            RoofInside.Move(new Vector3(size.x - xOffset, 0, size.y));
            */

            FinishMesh();

            //Old code
            /*
            //Roof solved with Wolfram: https://www.wolframalpha.com/input/?i=Solve%5B%7Bcos%28a%29%3D%3Dt%2Fy%2C+sin%28a%29%3D%3Dt%2Fx%2C+tan%28a%29%3D%3Dh%2F%28w%2Bx%29%7D%2C+%7Bx%2Cy%2Ca%7D%5D

            float h = height;
            float t = roofThickness;
            float w = size.x * 0.5f;

            float h2 = h * h;
            float t2 = t * t;
            float w2 = w * w;

            float rootTerm = Mathf.Sqrt(h2*t2*(h2-t2+w2));

            float xDiff = (rootTerm + t2 * w) / (h2 - t2);
            float yDiff = (h2 * t2 - w * rootTerm) / (h * t2 - h * w2);

            float innerWidth = size.x * 0.5f;
            float innerHeight = height - yDiff;
            float outerWidth = size.x * 0.5f + xDiff;
            float outerHeight = height;

            //Roof outside
            Vector3 outerRoofOffset = new Vector3(outerWidth, -outerHeight);
            outerRoofOffset += outerRoofOffset.normalized * roofOvershoot;

            RoofOutside = MeshGenerator.FilledShapes.RectangleAtCorner(Vector3.back * size.y, secondLine: outerRoofOffset, UVOffset: Vector2.zero);
            RoofOutside.Move(Vector3.up * outerHeight + Vector3.forward * size.y * 0.5f);
            RoofOutside.RotateAllUVsCWAroundOrigin(angleDeg: 180);

            TriangleMeshInfo secondRoofSide = RoofOutside.CloneFlipped;
            secondRoofSide.Scale(new Vector3(-1, 1, 1));
            secondRoofSide.FlipAllUVsHorizontallyAroundOrigin();
            RoofOutside.Add(secondRoofSide);

            Vector3 innerRoofOffset = new Vector3(innerWidth, -innerHeight);
            innerRoofOffset += innerRoofOffset.normalized * roofOvershoot;
            RoofInside = MeshGenerator.FilledShapes.RectangleAtCorner(Vector3.forward * size.y, secondLine: innerRoofOffset, UVOffset: Vector2.zero);
            RoofInside.Move(Vector3.up * innerHeight + Vector3.back * size.y * 0.5f);
            RoofInside.RotateAllUVsCWAroundOrigin(angleDeg: 180);

            secondRoofSide = RoofInside.CloneFlipped;
            secondRoofSide.Scale(new Vector3(-1, 1, 1));
            secondRoofSide.FlipAllUVsHorizontallyAroundOrigin();
            RoofInside.Add(secondRoofSide);

            //Walls
            List<Vector3> ClockwiseWallPoints = new List<Vector3>();
            ClockwiseWallPoints.Add(Vector3.up * innerHeight);
            ClockwiseWallPoints.Add(Vector3.right * innerWidth);
            ClockwiseWallPoints.Add(Vector3.left * innerWidth);

            FrontWall = MeshGenerator.FilledShapes.PointsClockwiseAroundFirstPoint(ClockwiseWallPoints);
            FrontWall.Move(Vector3.back * (size.y * 0.5f));

            TriangleMeshInfo secondFrontWallInfo = FrontWall.Clone;
            secondFrontWallInfo.Move(Vector3.forward * (size.y - wallThickness));
            FrontWall.Add(secondFrontWallInfo);

            BackWall= FrontWall.CloneFlipped;
            BackWall.Move(Vector3.forward * wallThickness);

            FrontWall.GenerateUVMeshBasedOnCardinalDirections(meshObject: linkedRoof.transform, originObjectForUV: originObject);
            BackWall.GenerateUVMeshBasedOnCardinalDirections(meshObject: linkedRoof.transform, originObjectForUV: originObject);

            //Wrapper
            List<Vector3> ClockwiseFrontWrapperPoints = new List<Vector3>();
            ClockwiseFrontWrapperPoints.Add(Vector3.up * innerHeight);
            ClockwiseFrontWrapperPoints.Add(Vector3.left * innerWidth + new Vector3(-outerRoofOffset.normalized.x, outerRoofOffset.normalized.y, outerRoofOffset.normalized.z) * roofOvershoot);
            ClockwiseFrontWrapperPoints.Add(Vector3.left * outerWidth + new Vector3(-outerRoofOffset.normalized.x, outerRoofOffset.normalized.y, outerRoofOffset.normalized.z) * roofOvershoot);
            ClockwiseFrontWrapperPoints.Add(Vector3.up * outerHeight);
            ClockwiseFrontWrapperPoints.Add(Vector3.right * outerWidth + outerRoofOffset.normalized * roofOvershoot);
            ClockwiseFrontWrapperPoints.Add(Vector3.right * innerWidth + outerRoofOffset.normalized * roofOvershoot);

            TriangleMeshInfo frontWrapper = MeshGenerator.FilledShapes.PointsClockwiseAroundFirstPoint(ClockwiseFrontWrapperPoints);
            frontWrapper.Move(Vector3.back * (size.y * 0.5f));
            frontWrapper.GenerateUVMeshBasedOnCardinalDirections(meshObject: linkedRoof.transform, originObjectForUV: originObject);

            TriangleMeshInfo backWrapper = frontWrapper.CloneFlipped;
            backWrapper.Move(Vector3.forward * size.y);
            backWrapper.GenerateUVMeshBasedOnCardinalDirections(meshObject: linkedRoof.transform, originObjectForUV: originObject);

            TriangleMeshInfo bottomWrapper = MeshGenerator.FilledShapes.RectangleAroundCenter(baseLine: Vector3.left * xDiff, secondLine: Vector3.forward * size.y);
            bottomWrapper.Move(Vector3.right * (innerWidth + xDiff / 2));
            TriangleMeshInfo otherBottomWrapper = bottomWrapper.Clone;
            bottomWrapper.Move(outerRoofOffset.normalized * roofOvershoot);
            otherBottomWrapper.Move(Vector3.left * (innerWidth * 2 + xDiff));
            otherBottomWrapper.Move(new Vector3(-outerRoofOffset.normalized.x, outerRoofOffset.normalized.y, outerRoofOffset.normalized.z) * roofOvershoot);
            bottomWrapper.Add(otherBottomWrapper);
            bottomWrapper.GenerateUVMeshBasedOnCardinalDirections(meshObject: linkedRoof.transform, originObjectForUV: originObject);

            RoofWrapper = new TriangleMeshInfo();
            RoofWrapper.Add(frontWrapper);
            RoofWrapper.Add(backWrapper);
            RoofWrapper.Add(bottomWrapper);

            RoofOutside.Move(new Vector3(size.x * 0.5f, 0, size.y * 0.5f));
            RoofInside.Move(new Vector3(size.x * 0.5f, 0, size.y * 0.5f));
            RoofWrapper.Move(new Vector3(size.x * 0.5f, 0, size.y * 0.5f));
            FrontWall.Move(new Vector3(size.x * 0.5f, 0, size.y * 0.5f));
            BackWall.Move(new Vector3(size.x * 0.5f, 0, size.y * 0.5f));

            FinishMesh();
            */
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}