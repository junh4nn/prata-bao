using System;

[Serializable]
public class PullRequest
{
    public string userId;
}

[Serializable]
public class GachaItem
{
    public int id;
    public string name;
    public string rarity;
}

[Serializable]
public class PullResponse
{
    public string message;
    public GachaItem item;
    public int newBalance;
}