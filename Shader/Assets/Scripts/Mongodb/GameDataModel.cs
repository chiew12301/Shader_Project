using System;
using System.Collections.Generic;
using Realms;
using MongoDB.Bson;

public partial class GameDataModel : IRealmObject
{
    [MapTo("_id")]
    [PrimaryKey]
    public ObjectId Id { get; set; }

    public float X { get; set; }

    public float Y { get; set; }

    public float Z { get; set; }

    [MapTo("score")]
    public int Score { get; set; }

    [MapTo("user_id")]
    [Required]
    public string UserId { get; set; }
}
