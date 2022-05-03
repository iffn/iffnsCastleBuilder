using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class BedBase : MonoBehaviour
    {
        [SerializeField] GameObject Leg1;
        [SerializeField] GameObject Leg2;
        [SerializeField] GameObject Leg3;
        [SerializeField] GameObject Leg4;
        [SerializeField] GameObject Top;

        public enum MattressSizes
        {
            Single,
            Double,
            Queen,
            King
        }

        public static List<string> MattressTypeStrings
        {
            get
            {
                List<string> enumString = new List<string>();

                int enumValues = System.Enum.GetValues(typeof(MattressSizes)).Length;

                for (int i = 0; i < enumValues; i++)
                {
                    MattressSizes type = (MattressSizes)i;

                    enumString.Add(type.ToString());
                }

                return enumString;
            }

        }

        float blockSize = 1f / 3;

        MattressSizes currentMattressSize;
        public MattressSizes CurrentMattressSize
        {
            get
            {
                return currentMattressSize;
            }
            set
            {
                currentMattressSize = value;
                ApplyBuildParameters();
            }
        }


        public void SetBuildParameters(MattressSizes mattressSize)
        {
            CurrentMattressSize = mattressSize;
            ApplyBuildParameters();
        }

        public Vector2Int GridSize
        {
            get
            {
                int gridLenght = 6;
                int gridWidth = 1;

                switch (currentMattressSize)
                {
                    case MattressSizes.Single:
                        gridWidth = 3;
                        break;
                    case MattressSizes.Double:
                        gridWidth = 4;
                        break;
                    case MattressSizes.Queen:
                        gridWidth = 5;
                        break;
                    case MattressSizes.King:
                        gridWidth = 6;
                        break;
                    default:
                        Debug.LogWarning("Error: Bed size not defined");
                        break;
                }

                return new Vector2Int(gridWidth, gridLenght);
            }
        }

        public Vector2 Size
        {
            get
            {
                return new Vector2(blockSize * GridSize.x, blockSize * GridSize.y);
            }
        }

        void ApplyBuildParameters()
        {
            float legWidth = 0.1f;
            float legHeight = 0.15f;

            float lenght = Size.y;
            float width = Size.x;

            Leg1.transform.localPosition = new Vector3(
                legWidth / 2,
                legHeight / 2,
                legWidth / 2);
            Leg2.transform.localPosition = new Vector3(
                legWidth / 2,
                legHeight / 2,
                lenght - legWidth / 2
                );
            Leg3.transform.localPosition = new Vector3(
                width - legWidth / 2,
                legHeight / 2,
                legWidth / 2
                );
            Leg4.transform.localPosition = new Vector3(
                width - legWidth / 2,
                legHeight / 2,
                lenght - legWidth / 2
                );

            Leg1.transform.localScale = Leg2.transform.localScale = Leg3.transform.localScale = Leg4.transform.localScale = new Vector3(legWidth, legHeight, legWidth);

            Top.transform.localPosition = Vector3.up * legHeight;
            Top.transform.localScale = new Vector3(width, 1, lenght);
        }
    }
}