using ProjectTrinity.Helper;

namespace ProjectTrinity.Simulation
{
    public class MatchSimulationLocalPlayer : MatchSimulationUnit
    {
        private struct LocalPlayerFrameState
        {
            public byte Frame { get; private set; }

            public int XPositionBase { get; private set; }
            public int YPositionBase { get; private set; }
            public byte RotationBase { get; private set; }

            public int XPositionDelta { get; private set; }
            public int YPositionDelta { get; private set; }
            public byte RotationDelta { get; private set; }

            public bool Confirmed { get; private set; }

            public bool Obsolete { get; set; }

            public void UpdatePositionBase(int xPosition, int yPosition, byte rotation)
            {
                XPositionBase = xPosition;
                YPositionBase = yPosition;
                RotationBase = rotation;

                XPositionDelta = 0;
                YPositionDelta = 0;
                RotationDelta = 0;

                Confirmed = false;
                Obsolete = false;
            }

            public void UpdatePositionBase(LocalPlayerFrameState localPlayerFrameState, bool keepDeltas)
            {
                XPositionBase = localPlayerFrameState.XPositionBase + localPlayerFrameState.XPositionDelta;
                YPositionBase = localPlayerFrameState.YPositionBase + localPlayerFrameState.YPositionDelta;
                RotationBase = (byte) MathHelper.Modulo((localPlayerFrameState.RotationBase + localPlayerFrameState.RotationDelta), byte.MaxValue);

                if(!keepDeltas)
                {
                    XPositionDelta = 0;
                    YPositionDelta = 0;
                    RotationDelta = 0;
                }

                Confirmed = false;
                Obsolete = false;
            }

            public void SetDeltas(int xPositionDelta, int yPositionDelta, byte rotationDelta, byte frame)
            {
                XPositionDelta = xPositionDelta;
                YPositionDelta = yPositionDelta;
                RotationDelta = rotationDelta;
                Frame = frame;
            }

            public void Confirm()
            {
                Confirmed = true;
            }
        }

        private readonly LocalPlayerFrameState[] localPlayerFrameStateBuffer = new LocalPlayerFrameState[30];

        private LocalPlayerFrameState lastLocalPlayerFrameState;
        private int nextLocalPlayerFrameIndex = 0;

        public MatchSimulationLocalPlayer(byte unitId, int xPosition, int yPosition, byte rotation, byte frame) 
            : base(unitId, xPosition, yPosition, rotation, frame)
        {
            localPlayerFrameStateBuffer[nextLocalPlayerFrameIndex].UpdatePositionBase(xPosition, yPosition, rotation);
            lastLocalPlayerFrameState = localPlayerFrameStateBuffer[nextLocalPlayerFrameIndex];

            nextLocalPlayerFrameIndex = MathHelper.Modulo((nextLocalPlayerFrameIndex + 1), localPlayerFrameStateBuffer.Length);
        }

        // should be called when a unit state message for the player was received.
        public override void SetConfirmedState(int xPosition, int yPosition, byte rotation, byte frame)
        {
            // oldest frame state
            int cursor = nextLocalPlayerFrameIndex;
            // just so it has a value, set to latest state, should never be used anyway.

            int lastestIndex = MathHelper.Modulo((nextLocalPlayerFrameIndex - 1), localPlayerFrameStateBuffer.Length);
            LocalPlayerFrameState lastUpdateFrameState = localPlayerFrameStateBuffer[lastestIndex];

            // iterate to local present
            while (!Equals(localPlayerFrameStateBuffer[cursor], lastLocalPlayerFrameState)) {

                LocalPlayerFrameState nextFrameState = localPlayerFrameStateBuffer[cursor];
                cursor = MathHelper.Modulo((nextLocalPlayerFrameIndex + 1), localPlayerFrameStateBuffer.Length);

                if (!IsFrameInFutureOrPresent(nextFrameState.Frame, frame) || nextFrameState.Obsolete)
                {
                    nextFrameState.Obsolete = true;
                    continue;
                }

                if (nextFrameState.Frame == frame)
                {
                    nextFrameState.UpdatePositionBase(xPosition, yPosition, rotation);
                    nextFrameState.Confirm();
                    lastUpdateFrameState = nextFrameState;
                    continue;
                }

                nextFrameState.UpdatePositionBase(lastUpdateFrameState, true);
                lastUpdateFrameState = nextFrameState;
            }

            // update current position and rotation
            UpdateCurrentState(lastUpdateFrameState);
        }

        // has to be called every local simulation frame, even when no input was done.
        public void SetLocalFrameInput(int xPositionDelta, int yPositionDelta, byte rotationDelta, byte frame)
        {
            localPlayerFrameStateBuffer[nextLocalPlayerFrameIndex].UpdatePositionBase(lastLocalPlayerFrameState, false);
            localPlayerFrameStateBuffer[nextLocalPlayerFrameIndex].SetDeltas(xPositionDelta, yPositionDelta, rotationDelta, frame);

            UpdateCurrentState(localPlayerFrameStateBuffer[nextLocalPlayerFrameIndex]);

            // ring buffer
            nextLocalPlayerFrameIndex = MathHelper.Modulo((nextLocalPlayerFrameIndex + 1), localPlayerFrameStateBuffer.Length);
            lastLocalPlayerFrameState = localPlayerFrameStateBuffer[nextLocalPlayerFrameIndex];
        }

        private void UpdateCurrentState(LocalPlayerFrameState localPlayerFrameState)
        {
            this.XPosition = localPlayerFrameState.XPositionBase + localPlayerFrameState.XPositionDelta;
            this.YPosition = localPlayerFrameState.YPositionBase + localPlayerFrameState.YPositionDelta;
            this.Rotation = (byte)MathHelper.Modulo(localPlayerFrameState.RotationBase + localPlayerFrameState.RotationDelta, byte.MaxValue);
        }
    }
}