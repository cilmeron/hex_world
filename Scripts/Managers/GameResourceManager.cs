
using System;
using System.Collections.Generic;
using UnityEngine;

public static class GameResourceManager{
   private static Dictionary<Player,Dictionary<ResourceType,int>> playerResourceAmountDictionary;

   private static void Init(){
      playerResourceAmountDictionary = new Dictionary<Player, Dictionary<ResourceType, int>>();
   }
   
   public static void AddPlayer(Player player){
      if (playerResourceAmountDictionary == null){
         Init();
      }
      Dictionary<ResourceType,int> resourceAmountDictionary = new Dictionary<ResourceType, int>();
      foreach (ResourceType resourceType in System.Enum.GetValues(typeof(ResourceType))){
         resourceAmountDictionary[resourceType] = 0;
      }
      playerResourceAmountDictionary[player] = resourceAmountDictionary;
   }
   
   public static event EventHandler OnResourceAmountChanged;

   public enum ResourceType{
      Gold,
      //Wood,
   }
   
   public static void AddResourceAmount(Player player, ResourceType type, int amount){
      playerResourceAmountDictionary[player][type] += amount;
      OnResourceAmountChanged?.Invoke(null,EventArgs.Empty);
   }
   
   public static void AddResources(Player player,Dictionary<ResourceType, int> resources){
      foreach (var entry in resources){
         playerResourceAmountDictionary[player][entry.Key] += entry.Value;
      }
      OnResourceAmountChanged?.Invoke(null,EventArgs.Empty);
   }

   public static void RemoveResourceAmount(Player player,ResourceType type, int amount){
      //remove resources from storages as well - pathfinidng so that gatheres take the resources to the building
      playerResourceAmountDictionary[player][type] -= amount;
      OnResourceAmountChanged?.Invoke(null,EventArgs.Empty);
   }
   
   public static void RemoveResources(Player player,Dictionary<ResourceType, int> resources){
      foreach (var entry in resources){
         playerResourceAmountDictionary[player][entry.Key] -= entry.Value;
      }
      OnResourceAmountChanged?.Invoke(null,EventArgs.Empty);
   }
   
   
   public static int GetResourceAmount(Player player,ResourceType type){
      return playerResourceAmountDictionary[player][type];
   }

   public static bool HasResources(Player player,Dictionary<ResourceType, int> costs){
      bool hasResources = true;
      foreach (var entry in costs){
         if (playerResourceAmountDictionary[player][entry.Key] < entry.Value){
            hasResources = false;
            break;
         }
      }

      return hasResources;
   }

   public static Dictionary<Player, Dictionary<ResourceType, int>> GetDictionary(){
      return playerResourceAmountDictionary;
   }
   
}
