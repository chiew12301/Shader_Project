using KC_Custom;
using Realms;
using Realms.Sync;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
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

        if(this.m_realmapp.CurrentUser == null)
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

    private GameDataModel GetOrCreatePlayerGameData()
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
                    Y = 0,
                    Z = 0
                });
            });
        }

        return gameData;
    }

    public int GetPlayerScore()
    {
        GameDataModel gameData = GetOrCreatePlayerGameData();
        return gameData.Score;
    }

    public Vector3 GetPlayerPosition()
    {
        GameDataModel gameData = GetOrCreatePlayerGameData();
        return new Vector3(gameData.X, gameData.Y, gameData.Z);
    }

    public List<GameDataModel> GetAllUserData()
    {
        List<GameDataModel> gameData = this.m_realm.All<GameDataModel>().ToList();
        if(gameData == null)
        {
            GameDataModel newdata = new GameDataModel();
            this.m_realm.Write(() => {
                newdata = this.m_realm.Add(new GameDataModel()
                {
                    UserId = this.m_realmuser.Id,
                    Score = 0,
                    X = 0,
                    Y = 0,
                    Z = 0
                });
            });
            gameData = this.m_realm.All<GameDataModel>().ToList();
        } //Add player data, but it shouldn't be empty coz will be created at first when first call player.

        return gameData;
    }

    public void AddPlayerScore(int val)
    {
        GameDataModel gameData = GetOrCreatePlayerGameData();
        this.m_realm.Write(() =>
        {
            gameData.Score += val;
        });
    }

    public void SetPlayerPosition(Vector3 pos)
    {
        GameDataModel gameData = GetOrCreatePlayerGameData();
        this.m_realm.Write(() =>
        {
            gameData.X = pos.x;
            gameData.Y = pos.y;
            gameData.Z = pos.z;
        });
    }
}
