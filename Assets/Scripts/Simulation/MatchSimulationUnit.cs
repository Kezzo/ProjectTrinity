using ProjectTrinity.Helper;
using ProjectTrinity.Networking.Messages;
using UniRx;
using UnityEngine;

namespace ProjectTrinity.Simulation
{
    public class MatchSimulationUnit
    {
        public byte UnitId { get; private set; }

        public class MovementProperties
        {
            public int XPosition;
            public int YPosition;
            public byte Rotation;
            public byte Frame;

            public Vector3 GetUnityPosition()
            {
                return new Vector3(UnitValueConverter.ToUnityPosition(XPosition), 0f, UnitValueConverter.ToUnityPosition(YPosition));
            }

            public Quaternion GetUnityRotation()
            {
                return Quaternion.Euler(0f, UnitValueConverter.ToUnityRotation(Rotation), 0f);
            }

        }

        protected MovementProperties movementState;
        public ReactiveProperty<MovementProperties> MovementState { get; private set; }

        public ReactiveProperty<byte> HealthPercent { get; set; }

        public Subject<MatchSimulation.AbilityActivation> AbilityActivationSubject;

        protected byte LastConfirmedFrame;

        public MatchSimulationUnit(byte unitId, int xPosition, int yPosition, byte rotation, byte healthPercent, byte frame)
        {
            UnitId = unitId;
            movementState = new MovementProperties {
                XPosition = xPosition,
                YPosition = yPosition,
                Rotation = rotation,
                Frame = frame
            };
            MovementState = new ReactiveProperty<MovementProperties>(movementState);

            HealthPercent = new ReactiveProperty<byte>(healthPercent);

            AbilityActivationSubject = new Subject<MatchSimulation.AbilityActivation>();

            LastConfirmedFrame = frame;
        }

        public virtual bool SetConfirmedState(int xPosition, int yPosition, byte rotation, byte healthPercent, byte frame) 
        {
            // don't update to old state. || account for frame wrap around
            if(IsFrameInFuture(frame, LastConfirmedFrame) || (LastConfirmedFrame > frame ? LastConfirmedFrame - frame : frame - LastConfirmedFrame) >= 30)
            {
                movementState.XPosition = xPosition;
                movementState.YPosition = yPosition;
                movementState.Rotation = rotation;
                movementState.Frame = frame;

                MovementState.SetValueAndForceNotify(movementState);

                HealthPercent.Value = healthPercent;

                LastConfirmedFrame = frame;
                return true;
            }

            return false;
        }

        public static bool IsFrameInFuture(byte frame, byte presentFrame)
        {
            return frame > presentFrame || (frame >= 0 && frame < 30 && (byte.MaxValue - presentFrame) < 30); // cut off after 30 frames
        }
    }
}