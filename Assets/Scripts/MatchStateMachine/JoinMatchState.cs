using ProjectTrinity.Helper;
using ProjectTrinity.MatchStateMachine;
using ProjectTrinity.Networking.HTTP;
using ProjectTrinity.Root;
using UnityEngine.Networking;

public class JoinMatchState : IMatchState
{
    MatchStateMachine matchStateMachine;
    UnityWebRequest findMatchRequest;
    private int playerCount = 0;

    public JoinMatchState(int playerCount)
    {
        this.playerCount = playerCount;
    }

    public void OnActivate(MatchStateMachine matchStateMachine)
    {
        if (EnvironmentHelper.StaticEnvironment == EnvironmentHelper.Environment.LOCAL)
        {
            matchStateMachine.InitializeUdpClient("127.0.0.1", 2448);
            matchStateMachine.ChangeMatchState(new TimeSyncMatchState());
            return;
        }

        this.matchStateMachine = matchStateMachine;
        string joinMatchPath = EnvironmentHelper.HTTPServerUrl + "/joinmatch/" + playerCount;
        findMatchRequest = UnityWebRequest.Get(joinMatchPath);
        findMatchRequest.SendWebRequest();
        DIContainer.Logger.Debug("Started to join match with url: " + joinMatchPath);
    }

    public void OnDeactivate()
    {
        if (findMatchRequest != null)
        { 
            findMatchRequest.Dispose();
        }
    }

    public void OnFixedUpdateTick()
    {
        if (findMatchRequest == null || !findMatchRequest.isDone)
        {
            return;
        }

        if (findMatchRequest.isNetworkError || findMatchRequest.isHttpError)
        {
            DIContainer.Logger.Error(findMatchRequest.error);
            matchStateMachine.ChangeMatchState(new IdleMatchState());
        }
        else
        {
            DIContainer.Logger.Debug("WebServer Response " + findMatchRequest.downloadHandler.text);
            JoinMatchResponse joinMatchResponse = DIContainer.Serializer.Deserialize<JoinMatchResponse>(findMatchRequest.downloadHandler.text);

            if (joinMatchResponse == null)
            {
                DIContainer.Logger.Error("JoinMatchResponse is null!");
                return;
            }

            joinMatchResponse.IP = joinMatchResponse.IP == "::1" ? "localhost" : joinMatchResponse.IP;

            matchStateMachine.InitializeUdpClient(joinMatchResponse.IP, joinMatchResponse.Port);
            DIContainer.Logger.Debug(string.Format("Joined match with IP: {0} and Port: {1}", joinMatchResponse.IP, joinMatchResponse.Port));
            matchStateMachine.ChangeMatchState(new TimeSyncMatchState());
        }
    }
}
