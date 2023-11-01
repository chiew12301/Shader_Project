using KC_Custom;
using Realms;
using Realms.Sync;
using System.Linq;
using UnityEngine;

public class RealmController : MonobehaviourSingleton<RealmController>
{
    private Realm m_realm;

    private App m_realmapp;
    private User m_realmuser;

    [SerializeField] private string m_realmAppID = "unitytest-lthmj";

    protected override void Awake()
    {
        base.Awake();
        if(this.m_realm == null)
        {
            this.LogInToMongo();
        }
    }

    protected override void OnDisable()
    {
        if (this.m_realm != null)
        {
            this.m_realm.Dispose();
        }
        base.OnDisable();
    }

    public bool IsRealmReady()
    {
        return this.m_realm != null;
    }

    private async void LogInToMongo()
    {
        this.m_realmapp = App.Create(new AppConfiguration(this.m_realmAppID));
        if(this.m_realmapp == null)
        {
            this.m_realmuser = await this.m_realmapp.LogInAsync(Credentials.Anonymous());
            this.m_realm = await Realm.GetInstanceAsync(new PartitionSyncConfiguration(this.m_realmuser.Id, this.m_realmuser));
        }
        else
        {
            this.m_realmuser = this.m_realmapp.CurrentUser;
            this.m_realm = Realm.GetInstance(new PartitionSyncConfiguration(this.m_realmuser.Id, this.m_realmuser));
        }
    }

    private GameDataModel GetOrCreateGameData()
    {
        GameDataModel gameData = this.m_realm.All<GameDataModel>().Where(d => d.UserId == this.m_realmuser.Id).FirstOrDefault();
        if(gameData == null)
        {
            this.m_realm.Write(() => {
                gameData = this.m_realm.Add(new GameDataModel()
                {
                    UserId = this.m_realmuser.Id,
                    Score = 0,
                    X = 0,
                    Y = 0
                });
            });
        }

        return gameData;
    }

    public int GetScore()
    {
        GameDataModel gameData = GetOrCreateGameData();
        return gameData.Score;
    }

    public Vector2 GetPosition()
    {
        GameDataModel gameData = GetOrCreateGameData();
        return new Vector2(gameData.X, gameData.Y);
    }

    public void AddScore(int val)
    {
        GameDataModel gameData = GetOrCreateGameData();
        this.m_realm.Write(() =>
        {
            gameData.Score += val;
        });
    }

    public void SetPosition(Vector2 pos)
    {
        GameDataModel gameData = GetOrCreateGameData();
        this.m_realm.Write(() =>
        {
            gameData.X = pos.x;
            gameData.Y = pos.y;
        });
    }
}
