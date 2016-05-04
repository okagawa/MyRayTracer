open System
open System.Windows.Forms
open System.Drawing
open System.Drawing.Imaging
open System.Diagnostics
open System.Runtime.InteropServices
open System.Threading
open System.Threading.Tasks
open System.Collections.Concurrent
open Raytracer_FSharp

let mutable width = 800
let mutable height = 600

module Surfaces = 
    let Shiny = 
        { new Surface with 
            member s.Diffuse pos = Color(1.0,1.0,1.0) 
            member s.Specular pos = Color(0.5,0.5,0.5)
            member s.Reflect pos = 0.7
            member s.Roughness = 250.0 }
    let MatteShiny = 
        { new Surface with
            member s.Diffuse pos = Color(1.0,1.0,1.0)
            member s.Specular pos = Color(0.25,0.25,0.25)
            member s.Reflect pos = 0.7
            member s.Roughness = 250.0 }
    let Checkerboard = 
        { new Surface with 
            member s.Diffuse pos =
                if (int (System.Math.Floor(pos.Z) + System.Math.Floor(pos.X))) % 2 <> 0
                then Color(1.0,1.0,1.0)
                else Color(0.0,0.0,0.0);
            member s.Specular pos = Color(1.0,1.0,1.0);
            member s.Reflect pos =
                if (int (System.Math.Floor(pos.Z) + System.Math.Floor(pos.X))) % 2 <> 0
                then 0.1
                else 0.7;
            member s.Roughness = 150.0 }

let baseScene = 
    { Things = [ Plane( Vector(0.0,1.0,0.0), 0.0, Surfaces.Checkerboard);
                 Sphere( Vector(0.0,1.0,-0.25), 1.0, Surfaces.Shiny) ];
      Lights = [ { Pos = Vector(-2.0,2.5,0.0); Color = Color(0.5,0.45,0.41) };
                 { Pos = Vector(2.0,4.5,2.0); Color = Color(0.99,0.95,0.8) } ];
      Camera = Camera(Vector(2.75, 2.0, 3.75), Vector(-0.6, 0.5, 0.0)) }

type ObjectPool<'a>(valueSelector : unit -> 'a) = 
    let objects = new ConcurrentQueue<'a>() :> IProducerConsumerCollection<'a>
    member pool.GetObject () = 
        let b, item = objects.TryTake()
        if b then item else valueSelector ()
    member pool.PutObject o = 
        objects.TryAdd(o) |> ignore

(*
type RayTracerForm() as this =
    inherit Form(ClientSize = new Size(width + 95, height + 59), Text = "RayTracer")
    let mutable bitmap = new Bitmap(width, height, PixelFormat.Format32bppRgb)
    let mutable buffers = ObjectPool(fun () -> Array.create (width * height) 0)
    let mutable pictureBox = new PictureBox()
    do pictureBox.Dock <- DockStyle.Fill
    do pictureBox.SizeMode <- PictureBoxSizeMode.StretchImage
    do pictureBox.Image <- bitmap
    do this.Controls.Add pictureBox
    do this.Text <- "Ray Tracer"
    do this.Load.Add( this.RayTracerForm_Load )
    do this.Show()

    member this.RayTracerForm_Load (e:EventArgs):unit =
        let rgb = buffers.GetObject()
        let raytracer = new RayTracer(width, height)
        do this.Show()
        do Console.WriteLine "Hello2"
        do raytracer.Render(baseScene, rgb)
        do pictureBox.Invalidate()
 *)

 type RayTracerForm() as this =
    inherit Form(ClientSize = new Size(width+95, height+59), Text="RayTracer")
    let mutable bitmap = new Bitmap(width, height, PixelFormat.Format32bppRgb)
    let mutable buffers = ObjectPool(fun () -> Array.create (width * height) 0)
    let mutable pictureBox = new PictureBox()
    let Beep e = System.Console.Beep()
    do pictureBox.Dock <- DockStyle.Fill
    do pictureBox.SizeMode <- PictureBoxSizeMode.StretchImage
    do pictureBox.Image <- bitmap
    do this.Text <- "Ray Tracer"
    do this.Click.Add ( Beep )
    do this.Load.Add (fun e -> Console.WriteLine("Hi"))
    do this.Show()
    

[<STAThread>]
do Application.EnableVisualStyles();
do Application.SetCompatibleTextRenderingDefault(false);
do Application.Run(new RayTracerForm())
