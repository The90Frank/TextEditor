#nowarn
#load "worldview.fsx"

open System.Windows.Forms
open System.Drawing
open System.Drawing.Drawing2D
open Worldview

type LWControl() =

    let mutable position = PointF(0.f, 0.f)
    let mutable size = SizeF(0.f, 0.f)
    let mutable parent : LWContainer = LWContainer() //perche non so come inizializzarlo a null
    let mutable matrixs = W2V()
    let mutable select = false

    let mousedownevt = new Event<MouseEventArgs>()
    let mousemoveevt = new Event<MouseEventArgs>()
    let mouseupevt = new Event<MouseEventArgs>()
    let keydownevt = new Event<KeyEventArgs>()
    let keyupevt = new Event<KeyEventArgs>()
    let keypressevt = new Event<KeyPressEventArgs>()
    let resizeevt = new Event<System.EventArgs>()

    let transformPoint (m:Drawing2D.Matrix) (p:PointF) =
        let pts = [| p |]
        m.TransformPoints(pts)
        pts.[0]
    
    member this.Matrixs 
        with get() = matrixs
        and set(v:W2V) = do
            matrixs.V2W <- v.V2W.Clone()
            matrixs.W2V <- v.W2V.Clone()
    
    member this.Size 
        with get() = size
        and set(v) = size <- v

    member this.Select
        with get() = select
        and set(v) = select <- v

    member this.Position 
        with get() = position
        and set(v:PointF) = do
            let dx, dy = (v.X - position.X), (v.Y - position.Y)
            matrixs.CTranslate(dx, dy)
            position <- v

    member this.Parent 
        with get() = parent
        and set(v:LWContainer) = parent <- v

    member this.MouseDown = mousedownevt.Publish
    member this.MouseMove = mousemoveevt.Publish
    member this.MouseUp = mouseupevt.Publish
    member this.KeyDown = keydownevt.Publish
    member this.KeyUp = keyupevt.Publish
    member this.KeyPress = keypressevt.Publish
    member this.Resize = resizeevt.Publish

    abstract OnMouseDown : MouseEventArgs -> unit
    default this.OnMouseDown e = 
                this.Select <- true
                mousedownevt.Trigger(e)

    abstract OnMouseMove : MouseEventArgs -> unit
    default this.OnMouseMove e = mousemoveevt.Trigger(e)

    abstract OnMouseUp : MouseEventArgs -> unit
    default this.OnMouseUp e = mouseupevt.Trigger(e)

    abstract OnKeyDown : KeyEventArgs -> unit
    default this.OnKeyDown e = keydownevt.Trigger(e)

    abstract OnKeyUp : KeyEventArgs -> unit
    default this.OnKeyUp e = keyupevt.Trigger(e)

    abstract OnKeyPress : KeyPressEventArgs -> unit
    default this.OnKeyPress e = keypressevt.Trigger(e)

    abstract OnPaint : PaintEventArgs -> unit
    default this.OnPaint e = ()

    abstract OnResize : System.EventArgs -> unit
    default this.OnResize e = resizeevt.Trigger(e)

    abstract Invalidate : _ -> unit
    default this.Invalidate _ = this.Parent.Invalidate()

    abstract HitTest : PointF -> bool
    default this.HitTest p =
        let aux = PointF(0.f, 0.f)
        (new RectangleF(aux, this.Size)).Contains(transformPoint this.Matrixs.V2W p)


and LWContainer() as this =
    inherit UserControl()

    let lvcontrols = ResizeArray<LWControl>()
  
    do 
        this.SetStyle(ControlStyles.DoubleBuffer, true)
        this.SetStyle(ControlStyles.AllPaintingInWmPaint, true)
    done

    member this.LWControls with get() = lvcontrols

    override this.OnResize e =
        for idx in 0 .. (lvcontrols.Count - 1) do
            let c = lvcontrols.[idx]
            c.OnResize(e)
        
    override this.OnKeyDown e =
        match (lvcontrols |> Seq.tryFind (fun c -> c.Select)) with
        | Some c -> c.OnKeyDown(e)
        | None -> ()

    override this.OnKeyUp e =
        match (lvcontrols |> Seq.tryFind (fun c -> c.Select)) with
        | Some c -> c.OnKeyUp(e)
        | None -> ()

    override this.OnKeyPress e =
        match (lvcontrols |> Seq.tryFind (fun c -> c.Select)) with
        | Some c -> c.OnKeyPress(e)
        | None -> ()
        
    override this.OnMouseDown e =
        for idx in 0 .. (lvcontrols.Count - 1) do
            let c = lvcontrols.[idx]
            c.Select <- false
        let p = PointF(single e.X, single e.Y)
        match (lvcontrols |> Seq.tryFind (fun c -> c.HitTest p)) with
        | Some c -> c.OnMouseDown(e)
        | None -> ()

    override this.OnMouseUp e =
        let p = PointF(single e.X, single e.Y)
        for idx in 0 .. (lvcontrols.Count - 1) do
            let c = lvcontrols.[idx]
            c.OnMouseUp(e)

    override this.OnMouseMove e =
        let p = PointF(single e.X, single e.Y)
        match (lvcontrols |> Seq.tryFind (fun c -> c.HitTest p)) with
        | Some c -> c.OnMouseMove(e); this.Invalidate();
        | None -> ()

    override this.OnPaint e =
        let g = e.Graphics
        g.SmoothingMode <- System.Drawing.Drawing2D.SmoothingMode.AntiAlias
        for idx in (lvcontrols.Count - 1) .. -1 .. 0 do
            let c = lvcontrols.[idx]
            g.Transform <- c.Matrixs.W2V
            c.OnPaint e
        done