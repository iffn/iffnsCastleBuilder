using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class GridOrientation
    {
        GridForwardOrientations currentForwardOrientation;

        public enum GridForwardOrientations
        {
            ZPositive,
            XPositive,
            ZNegative,
            XNegative
        }
        public enum GridQuarterOrientations
        {
            XPosZPos,
            XPosZNeg,
            XNegZNeg,
            XNegZPos,
        }

        public GridOrientation(GridForwardOrientations forwardOrientation)
        {
            ForwardOrientation = forwardOrientation;
        }

        public GridOrientation(GridQuarterOrientations quarterOrientation)
        {
            QuarterOrientation = quarterOrientation;
        }

        public GridForwardOrientations ForwardOrientation
        {
            get
            {
                return currentForwardOrientation;
            }
            set
            {
                currentForwardOrientation = value;
            }
        }

        public GridQuarterOrientations QuarterOrientation
        {
            get
            {
                switch (currentForwardOrientation)
                {
                    case GridForwardOrientations.XPositive:
                        return GridQuarterOrientations.XPosZNeg;

                    case GridForwardOrientations.XNegative:
                        return GridQuarterOrientations.XNegZPos;

                    case GridForwardOrientations.ZPositive:
                        return GridQuarterOrientations.XPosZPos;

                    case GridForwardOrientations.ZNegative:
                        return GridQuarterOrientations.XNegZNeg;

                    default:
                        Debug.LogWarning("Error: Orientation not defined");
                        return GridQuarterOrientations.XPosZPos;
                }
            }
            set
            {
                switch (value)
                {
                    case GridQuarterOrientations.XPosZPos:
                        currentForwardOrientation = GridForwardOrientations.ZPositive;
                        break;

                    case GridQuarterOrientations.XPosZNeg:
                        currentForwardOrientation = GridForwardOrientations.XPositive;
                        break;

                    case GridQuarterOrientations.XNegZNeg:
                        currentForwardOrientation = GridForwardOrientations.ZNegative;
                        break;

                    case GridQuarterOrientations.XNegZPos:
                        currentForwardOrientation = GridForwardOrientations.XNegative;
                        break;

                    default:
                        Debug.LogWarning("Error: Orientation not defined");
                        currentForwardOrientation = GridForwardOrientations.ZPositive;
                        break;
                }
            }
        }

    }
}