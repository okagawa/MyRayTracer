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
