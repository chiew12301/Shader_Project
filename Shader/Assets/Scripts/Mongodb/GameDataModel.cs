using Realms;
using System.Collections;
using System.Collections.Generic;
using MongoDB.Bson;

public class GameDataModel : RealmObject
{
    [MapTo("_id")]
    [PrimaryKey]
    public ObjectId Id { get; set; }
    public float X { get; set; }
    public float Y { get; set; }
    [MapTo("score")]
    public int Score { get; set; }
    [MapTo("user_id")]
    public string UserId { get; set; }

}
