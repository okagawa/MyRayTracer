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


[<STAThread>]
do Application.EnableVisualStyles();
do Application.SetCompatibleTextRenderingDefault(false);
do Application.Run(new RayTracerForm())
