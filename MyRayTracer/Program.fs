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
    { Things = [ Plane(Vector(0.0, 1.0, 0.0), 0.0, Color(1.0,1.0,1.0))];
      Lights = [ {Pos = Vector(-2.0, 2.5, 0.0); Color = Color(0.5, 0.45, )}]
    }   


type RayTracerForm() as this =
    inherit Form(ClientSize = new Size(width + 95, height + 59), Text = "RayTracer")
    let mutable bitmap = new Bitmap(width, height, PixelFormat.Format32bppRgb)
    let mutable pictureBox = new PictureBox()
    do pictureBox.Dock <- DockStyle.Fill
    do pictureBox.SizeMode <- PictureBoxSizeMode.StretchImage
    do pictureBox.Image <- bitmap
    do this.Controls.Add pictureBox
    do this.Load += this.RayTracerForm_Load
    do this.Show()
    
    let RayTracerForm_Load sender e =
        do this.Show()
        let raytracer = new RayTracer(width, height)
        do  raytracer.Render(baseScene)
        pictureBox.Invalidate

[<STAThread>]
do Application.EnableVisualStyles();
do Application.SetCompatibleTextRenderingDefault(false);
do Application.Run(new RayTracerForm())
