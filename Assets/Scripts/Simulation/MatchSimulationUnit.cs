using ProjectTrinity.Helper;
using UnityEngine;

namespace ProjectTrinity.Simulation
{
    public class MatchSimulationUnit
    {
        public byte UnitId { get; private set; }
        public int XPosition { get; protected set; }
        public int YPosition { get; protected set; }
        public byte Rotation { get; protected set; }

        private byte LastConfirmedFrame;

        public MatchSimulationUnit(byte unitId, int xPosition, int yPosition, byte rotation, byte frame)
        {
            UnitId = unitId;
            SetConfirmedState(xPosition, yPosition, rotation, frame);
        }

        public virtual void SetConfirmedState(int xPosition, int yPosition, byte rotation, byte frame) 
        {
            // don't update to old state. || account for frame wrap around
            if(IsFrameInFutureOrPresent(frame, LastConfirmedFrame))
            {
                XPosition = xPosition;
                YPosition = yPosition;
                Rotation = rotation;
                LastConfirmedFrame = frame;
            }
        }

        public Vector3 GetUnityPosition()
        {
            return new Vector3(UnitValueConverter.ToUnityPosition(XPosition), 0f, UnitValueConverter.ToUnityPosition(YPosition));
        }

        public Vector3 GetUnityRotation()
        {
            return new Vector3(0f, UnitValueConverter.ToUnityRotation(Rotation), 0f);
        }

        protected bool IsFrameInFutureOrPresent(byte frame, byte presentFrame)
        {
            return true; //frame >= presentFrame || (frame > 0 && frame < 30 && (byte.MaxValue - presentFrame) < 30);
        }
    }
}