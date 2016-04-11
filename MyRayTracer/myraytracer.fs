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
    abstract Intersect : Ray -> Intersection option
    abstract Normal : Vector -> Vector

let Sphere(center, radius) =
    let radius2 = radius * radius
    { new SceneObject with
        member this.Normal pos = Vector.Norm (pos - center)
        member this.Intersect (ray:Ray) =
            let eo = center - ray.Start
            let v = Vector.Dot(eo, ray.Dir)
            let dist =
                if (v < 0.0) then 0.0
                else
                    let disc = radius2 - (Vector.Dot(eo,eo) - (v*v))
                    if disc < 0.0 then 0.0
                    else v - (sqrt(disc))
            if dist = 0.0 then None
            else Some {Thing = this; Ray = ray; Dist = dist}
    }

let Plane(norm, offset) =
    { new SceneObject with
        member this.Normal pos = norm
        member this.Intersect (ray) =
            let denom = Vector.Dot(norm, ray.Dir)
            if denom > 0.0 then None 
            else let dist = (Vector.Dot(norm, ray.Start) + offset) / (-denom)
                 Some {Thing = this; Ray = ray; Dist = dist}
    }

type Camera(pos : Vector, lookAt : Vector) =
    let forward = Vector.Norm(lookAt - pos)
    let down = Vector(0.0, -1.0, 0.0)
    let right = 1.5 * Vector.Norm(Vector.Cross(forward, down))
    let up = 1.5 * Vector.Norm(Vector.Cross(forward, right))
    member c.Pos = pos
    member c.Forward = forward
    member c.Up = up
    member c.Right = right

