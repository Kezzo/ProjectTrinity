using ProjectTrinity.Helper;
using UniRx;
using UnityEngine;

namespace ProjectTrinity.Simulation
{
    public class MatchSimulationUnit
    {
        public byte UnitId { get; private set; }
        public ReactiveProperty<int> XPosition { get; protected set; }
        public ReactiveProperty<int> YPosition { get; protected set; }
        public ReactiveProperty<byte> Rotation { get; protected set; }
        public ReactiveProperty<byte> HealthPercent { get; set; }

        protected byte LastConfirmedFrame;

        public MatchSimulationUnit(byte unitId, int xPosition, int yPosition, byte rotation, byte healthPercent, byte frame)
        {
            UnitId = unitId;
            XPosition = new ReactiveProperty<int>(xPosition);
            YPosition = new ReactiveProperty<int>(yPosition);
            Rotation = new ReactiveProperty<byte>(rotation);
            HealthPercent = new ReactiveProperty<byte>(healthPercent);
            LastConfirmedFrame = frame;
        }

        public virtual bool SetConfirmedState(int xPosition, int yPosition, byte rotation, byte healthPercent, byte frame) 
        {
            // don't update to old state. || account for frame wrap around || just accept bigger differences
            if(IsFrameInFuture(frame, LastConfirmedFrame) || (LastConfirmedFrame > frame ? LastConfirmedFrame - frame : frame - LastConfirmedFrame) >= 30)
            {
                XPosition.Value = xPosition;
                YPosition.Value = yPosition;
                Rotation.Value = rotation;
                HealthPercent.Value = healthPercent;

                LastConfirmedFrame = frame;
                return true;
            }

            return false;
        }

        public Vector3 GetUnityPosition()
        {
            return new Vector3(UnitValueConverter.ToUnityPosition(XPosition.Value), 0f, UnitValueConverter.ToUnityPosition(YPosition.Value));
        }

        public Quaternion GetUnityRotation()
        {
            return Quaternion.Euler(0f, UnitValueConverter.ToUnityRotation(Rotation.Value), 0f);
        }

        public static bool IsFrameInFuture(byte frame, byte presentFrame)
        {
            return frame > presentFrame || (frame >= 0 && frame < 30 && (byte.MaxValue - presentFrame) < 30); // cut off after 30 frames
        }
    }
}