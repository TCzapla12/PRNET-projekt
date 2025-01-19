﻿using Grpc.Core;

namespace PetKeeperMobileApp.Utils;

public class Storage
{
    public static async Task<Metadata> GetMetadata()
    {
        return new Metadata
        {
            { "Authorization", $"Bearer { await Storage.GetToken() }" }
        };
    }

    public static async Task SaveCredentials(string token)
    {
        //await SecureStorage.SetAsync("id", id);
        await SecureStorage.SetAsync("token", token);
    }

    public static void RemoveCredentials()
    {
        //SecureStorage.Remove("id");
        SecureStorage.Remove("token");
    }

    public static async Task<string> GetToken()
    {
        return await SecureStorage.GetAsync("token") ?? string.Empty;
    }

    //public static async Task<string> GetUserId()
    //{
    //    return await SecureStorage.GetAsync("id") ?? string.Empty;
    //}
}
