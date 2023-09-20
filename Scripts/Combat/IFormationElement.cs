using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public interface IFormationElement{

   bool IsLeader(){
      return GetFormation().GetLeader() == this;
   }
   bool IsInFormation();
   Formation GetFormation();

   Vector3 GetRelativePosition();
   
   void RemoveEntityFromFormation();
   void AddEntityToFormation(Formation f, Vector3 relativePos);
   Renderer GetRenderer();
   GameObject GetGameObject();
   float GetMaxHP();
   float GetCurrentHP();
   Sprite GetSprite();
   bool IsSelectable(){
      return typeof(ISelectable).IsInstanceOfType(this);
   }
   ISelectable GetSelectable();
   Player GetPlayer();

   bool IsMoveable(){
      return this is IMoveable;
   }

   [CanBeNull]
   IMoveable GetMoveable(){
      return GetGameObject().GetComponent<IMoveable>();
   }
 
   

   
}
