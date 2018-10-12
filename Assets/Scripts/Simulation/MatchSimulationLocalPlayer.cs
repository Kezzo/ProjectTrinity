using ProjectTrinity.Helper;
using ProjectTrinity.Root;

namespace ProjectTrinity.Simulation
{
    public class MatchSimulationLocalPlayer : MatchSimulationUnit
    {
        private class LocalPlayerFrameState
        {
            public byte Frame { get; private set; }

            public int XPositionBase { get; private set; }
            public int YPositionBase { get; private set; }

            public int XPositionDelta { get; private set; }
            public int YPositionDelta { get; private set; }

            public bool Confirmed { get; private set; }

            public bool Obsolete { get; set; }

            public void UpdateBaseValues(int xPosition, int yPosition)
            {
                XPositionBase = xPosition;
                YPositionBase = yPosition;

                XPositionDelta = 0;
                YPositionDelta = 0;
                
                Confirmed = false;
                Obsolete = false;
            }

            public void UpdatePositionBase(LocalPlayerFrameState localPlayerFrameState, bool keepDeltas)
            {
                XPositionBase = localPlayerFrameState.XPositionBase + localPlayerFrameState.XPositionDelta;
                YPositionBase = localPlayerFrameState.YPositionBase + localPlayerFrameState.YPositionDelta;

                if(!keepDeltas)
                {
                    XPositionDelta = 0;
                    YPositionDelta = 0;
                    Confirmed = false;
                }

                Obsolete = false;
            }

            public void SetDeltas(int xPositionDelta, int yPositionDelta, byte frame)
            {
                XPositionDelta = MathHelper.LimitValueDelta(XPositionBase, xPositionDelta, 24000);
                YPositionDelta = MathHelper.LimitValueDelta(YPositionBase, yPositionDelta, 24000);
                Frame = frame;
            }

            public void Confirm()
            {
                Confirmed = true;
            }

            public override string ToString()
            {
                return string.Format("Frame: {0} X-Base: {1} Y-Base: {2} X-Delta {3} Y-Delta: {4} Confirmed: {5}", 
                                     Frame, XPositionBase, YPositionBase, XPositionDelta, YPositionDelta, Confirmed);
            }
        }

        private readonly LocalPlayerFrameState[] localPlayerFrameStateBuffer = new LocalPlayerFrameState[30];

        private LocalPlayerFrameState lastLocalPlayerFrameState;
        private int nextLocalPlayerFrameIndex = 0;

        public MatchSimulationLocalPlayer(byte unitId, int xPosition, int yPosition, byte rotation, byte frame) 
            : base(unitId, xPosition, yPosition, rotation, frame)
        {
            for (int i = 0; i < localPlayerFrameStateBuffer.Length; i++)
            {
                localPlayerFrameStateBuffer[i] = new LocalPlayerFrameState();
            }

            localPlayerFrameStateBuffer[nextLocalPlayerFrameIndex].UpdateBaseValues(xPosition, yPosition);
            lastLocalPlayerFrameState = localPlayerFrameStateBuffer[nextLocalPlayerFrameIndex];

            nextLocalPlayerFrameIndex = MathHelper.Modulo((nextLocalPlayerFrameIndex + 1), localPlayerFrameStateBuffer.Length);
        }

        // should be called when a unit state message for the player was received.
        public override bool SetConfirmedState(int xPosition, int yPosition, byte rotation, byte frame)
        {
            bool validPosition = IsFrameInFuture(frame, LastConfirmedFrame) || (LastConfirmedFrame > frame ? LastConfirmedFrame - frame : frame - LastConfirmedFrame) >= 30;

            // we already received more recent frame
            if (!validPosition)
            {
                return true;
            }

            // oldest frame state
            int cursor = nextLocalPlayerFrameIndex;
            // just so it has a value, set to latest state, should never be used anyway.

            int lastestIndex = MathHelper.Modulo((nextLocalPlayerFrameIndex - 1), localPlayerFrameStateBuffer.Length);
            LocalPlayerFrameState lastUpdateFrameState = localPlayerFrameStateBuffer[lastestIndex];

            // iterate from old stored frame to local present frame
            // update confirmed frame and all following frames to correct current position
            while (true) {
                // should be oldest and first frame that is updated here.
                if (localPlayerFrameStateBuffer[cursor].Frame == frame)
                {
                    if (localPlayerFrameStateBuffer[cursor].XPositionBase + localPlayerFrameStateBuffer[cursor].XPositionDelta != xPosition ||
                       localPlayerFrameStateBuffer[cursor].YPositionBase + localPlayerFrameStateBuffer[cursor].YPositionDelta != yPosition)
                    { 
                        DIContainer.Logger.Warn(string.Format("Position inconsistency at frame: {0}. Local: X:{1}, Y:{2} Remote: X:{3} Y:{4}", 
                                                               frame, 
                                                               localPlayerFrameStateBuffer[cursor].XPositionBase + localPlayerFrameStateBuffer[cursor].XPositionDelta, 
                                                               localPlayerFrameStateBuffer[cursor].YPositionBase + localPlayerFrameStateBuffer[cursor].YPositionDelta,
                                                               xPosition,
                                                               yPosition));
                        DIContainer.Logger.Warn("Current buffer is: \n" + string.Join("\n", (object[])localPlayerFrameStateBuffer));
                    }

                    localPlayerFrameStateBuffer[cursor].UpdateBaseValues(xPosition, yPosition);
                    localPlayerFrameStateBuffer[cursor].Confirm();
                    lastUpdateFrameState = localPlayerFrameStateBuffer[cursor];
                }
                else if (IsFrameInFuture(localPlayerFrameStateBuffer[cursor].Frame, frame))
                {
                    localPlayerFrameStateBuffer[cursor].UpdatePositionBase(lastUpdateFrameState, true);
                    lastUpdateFrameState = localPlayerFrameStateBuffer[cursor];
                }

                if(localPlayerFrameStateBuffer[cursor].Frame == lastLocalPlayerFrameState.Frame) {
                    break;
                }

                cursor = MathHelper.Modulo(cursor + 1, localPlayerFrameStateBuffer.Length);
            }

            LastConfirmedFrame = frame;
            // update current position and rotation
            UpdateCurrentState(localPlayerFrameStateBuffer[cursor]);
            return true;
        }

        // has to be called every local simulation frame, even when no input was done.
        public void SetLocalFrameInput(int xPositionDelta, int yPositionDelta, byte rotation, byte frame)
        {
            localPlayerFrameStateBuffer[nextLocalPlayerFrameIndex].UpdatePositionBase(lastLocalPlayerFrameState, false);
            localPlayerFrameStateBuffer[nextLocalPlayerFrameIndex].SetDeltas(xPositionDelta, yPositionDelta, frame);

            UpdateCurrentState(localPlayerFrameStateBuffer[nextLocalPlayerFrameIndex], rotation);

            lastLocalPlayerFrameState = localPlayerFrameStateBuffer[nextLocalPlayerFrameIndex];

            // ring buffer
            nextLocalPlayerFrameIndex = MathHelper.Modulo((nextLocalPlayerFrameIndex + 1), localPlayerFrameStateBuffer.Length);
        }

        private void UpdateCurrentState(LocalPlayerFrameState localPlayerFrameState, byte rotation)
        {
            XPosition = localPlayerFrameState.XPositionBase + localPlayerFrameState.XPositionDelta;
            YPosition = localPlayerFrameState.YPositionBase + localPlayerFrameState.YPositionDelta;

            Rotation = (byte)MathHelper.Modulo(rotation, byte.MaxValue);
        }

        private void UpdateCurrentState(LocalPlayerFrameState localPlayerFrameState)
        {
            XPosition = localPlayerFrameState.XPositionBase + localPlayerFrameState.XPositionDelta;
            YPosition = localPlayerFrameState.YPositionBase + localPlayerFrameState.YPositionDelta;
        }
    }
}