
using System;
using System.Collections.Generic;
using UnityEngine;

public static class GameResourceManager{
   private static Dictionary<Player.Nations,Dictionary<ResourceType,int>> playerResourceAmountDictionary;

   private static void Init(){
      playerResourceAmountDictionary = new Dictionary<Player.Nations, Dictionary<ResourceType, int>>();
   }
   
   public static void AddPlayer(Player.Nations nation){
      if (playerResourceAmountDictionary == null){
         Init();
      }
      Dictionary<ResourceType,int> resourceAmountDictionary = new Dictionary<ResourceType, int>();
      foreach (ResourceType resourceType in System.Enum.GetValues(typeof(ResourceType))){
         resourceAmountDictionary[resourceType] = 0;
      }
      playerResourceAmountDictionary[nation] = resourceAmountDictionary;
   }
   
   public static event EventHandler OnResourceAmountChanged;

   public enum ResourceType{
      Gold,
      //Wood,
   }
   
   public static void AddResourceAmount(Player.Nations nation, ResourceType type, int amount){
      playerResourceAmountDictionary[nation][type] += amount;
      OnResourceAmountChanged?.Invoke(null,EventArgs.Empty);
   }
   
   public static void AddResources(Player.Nations nation,Dictionary<ResourceType, int> resources){
      foreach (var entry in resources){
         playerResourceAmountDictionary[nation][entry.Key] += entry.Value;
      }
      OnResourceAmountChanged?.Invoke(null,EventArgs.Empty);
   }

   public static void RemoveResourceAmount(Player.Nations nation,ResourceType type, int amount){
      //remove resources from storages as well - pathfinidng so that gatheres take the resources to the building
      playerResourceAmountDictionary[nation][type] -= amount;
      OnResourceAmountChanged?.Invoke(null,EventArgs.Empty);
   }
   
   public static void RemoveResources(Player.Nations nation,Dictionary<ResourceType, int> resources){
      foreach (var entry in resources){
         playerResourceAmountDictionary[nation][entry.Key] -= entry.Value;
      }
      OnResourceAmountChanged?.Invoke(null,EventArgs.Empty);
   }
   
   
   public static int GetResourceAmount(Player.Nations nation,ResourceType type){
      return playerResourceAmountDictionary[nation][type];
   }

   public static bool HasResources(Player.Nations nation,Dictionary<ResourceType, int> costs){
      bool hasResources = true;
      foreach (var entry in costs){
         if (playerResourceAmountDictionary[nation][entry.Key] < entry.Value){
            hasResources = false;
            break;
         }
      }

      return hasResources;
   }

   public static Dictionary<Player.Nations, Dictionary<ResourceType, int>> GetDictionary(){
      return playerResourceAmountDictionary;
   }
   
}
