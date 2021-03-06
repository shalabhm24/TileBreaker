﻿namespace Breakout.FSharp

open Duality
open Duality.Resources
open System
open OpenTK.Input
open OpenTK
open Duality.Components.Physics    
open System.Linq

type BallState =
    | FixedToBat
    | Free

[<Serializable>]
type Ball() = 
    inherit Component()   
    member val BallState = BallState.FixedToBat with get,set
    interface ICmpUpdatable with
        member this.OnUpdate() =
            this.GameObj.RigidBody.LinearVelocity <- this.GameObj.RigidBody.LinearVelocity.Normalized * 50.0f

[<Serializable>]
type Bat() = 
    inherit Component()   
        
    let HalfWidth(body:RigidBody) = body.Shapes.First().AABB.W / 2.0f 

    let (|LeftKey|RightKey|OtherKey|) (keyboard:KeyboardInput) = 
        if keyboard.KeyPressed(Key.Left) then LeftKey
        elif keyboard.KeyPressed(Key.Right) then RightKey
        else OtherKey "Hi, you pressed a key...well that is interesting :D"
            
    
    let (|RightSoemthing|) (keyboard:KeyboardInput) = 
        keyboard.KeyPressed(Key.Right), "Test from right something" 

    let (|LeftSoemthing|) (keyboard:KeyboardInput) = 
        keyboard.KeyPressed(Key.Left)
    
    interface ICmpUpdatable with
        member this.OnUpdate()=        
            match DualityApp.Keyboard with
            //| LeftSoemthing true & RightSoemthing (wasPressed, stringy) -> () //the left and not the right
            | LeftKey  -> 
                let leftWall = Scene.Current.FindGameObject("LeftWall")
                if leftWall.Transform.Pos.X + HalfWidth leftWall.RigidBody <= this.GameObj.Transform.Pos.X - HalfWidth this.GameObj.RigidBody  then
                    this.GameObj.Transform.MoveBy(-Vector2.UnitX * 10.0f)                  
            | RightKey->
                 let rightWall = Scene.Current.FindGameObject("RightWall")
                 if (this.GameObj.Transform.Pos.X + HalfWidth this.GameObj.RigidBody <= rightWall.Transform.Pos.X - HalfWidth rightWall.RigidBody) then
               //if (rightWall.Transform.Pos.X - HalfWidth rightWall.RigidBody > this.GameObj.Transform.Pos.X + HalfWidth this.GameObj.RigidBody ) then
                    this.GameObj.Transform.MoveBy(Vector2.UnitX * 10.0f)
            | OtherKey s-> ()
            if (Scene.Current.FindComponent<Ball>().BallState = BallState.FixedToBat) then
                let position = this.GameObj.Transform.Pos
                Scene.Current.FindGameObject<Ball>().Transform.Pos <- new Vector3(position.X + 100.0f, position.Y - 50.0f, 0.0f)
                
