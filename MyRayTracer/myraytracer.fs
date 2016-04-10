module RayTracer_FSharp

open System
open System.Windows.Forms
open System.Drawing
open System.Threading
open System.Threading.Tasks

type Vector(x:float, y:float, z:float) =
    member this.X = x
    member this.Y = y
    member this.Z = z
    static member ( * ) (k, (v:Vector)) = Vector(k*v.X, k*v.Y, k*v.Z)
    static member ( - ) (v1:Vector, v2:Vector) = Vector(v1.X-v2.X, v1.Y-v2.Y, v1.Z-v2.Z)
    static member ( + ) (v1:Vector, v2:Vector) = Vector(v1.X+v2.X, v1.Y+v2.Y, v1.Z+v2.Z)
    static member Dot (v1:Vector, v2:Vector) = v1.X*v2.X + v1.Y*v2.Y + v1.Z*v2.Z
    static member Mag (v:Vector) = sqrt(v.X*v.X+v.Y*v.Y+v.Z*v.Z)
    static member Norm (v:Vector) =
        let mag = Vector.Mag v
        let div = if mag = 0.0 then infinity else 1.0/mag
        div * v
    static member Cross (v1:Vector, v2:Vector) = 
        Vector(v1.Y * v2.Z - v1.Z * v2.Y,
               v1.Z * v2.X - v1.X * v2.Z,
               v1.X * v2.Y - v1.Y * v2.X)

type Color(r:float, g:float, b:float) =
    static member private floatToInt c = let c' = int(255.0*c) in if c' > 255 then 255 else c'
    member this.R = r
    member this.G = g
    member this.B = b
    member this.ToDrawingColor() = System.Drawing.Color.FromArgb(this.ToInt())
    member this.ToInt() = (Color.floatToInt b) ||| (Color.floatToInt g <<< 8) ||| (Color.floatToInt r <<< 16) ||| (255 <<< 24)

type Ray = {Start: Vector; Dir:Vector}

type Intersection =
    { Thing: SceneObject;
      Ray: Ray;
      Dist: double}

and SceneObject =
    { abstract Intersect }