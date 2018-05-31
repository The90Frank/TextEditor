#nowarn
#load "worldview.fsx"
#load "lvc.fsx"

open System.Windows.Forms
open System.Drawing
open System.Drawing.Drawing2D
open Worldview
open Lvc
open System.Drawing.Text
open System

let mutable lato = 20.f

type MyButton() as this =
    inherit LWControl()

    let mutable char = null
    let mutable charcolor = Brushes.Black
    let mutable backcolor = Brushes.Gray
    let mutable presscolor = Brushes.Gray
    let mutable press = false
    let mutable font = new Font("Arial Black", 12.f)

    member this.Size
        with get() = base.Size
        and set(v) = do
            base.Size <- v
            font <- new Font("Arial Black", (max base.Size.Height base.Size.Width)/3.f)

    member this.Press 
        with get() = press
        and set(v) = press <- v

    member this.Char 
        with get() = char
        and set(v) = char <- v

    member this.CharColor 
        with get() = charcolor
        and set(v) = charcolor <- v

    member this.BackColor
        with get() = backcolor
        and set(v) = backcolor <- v

    member this.PressColor
        with get() = presscolor
        and set(v) = presscolor <- v

    override this.OnPaint e = 
        let g = e.Graphics
        let rect = RectangleF (0.f, 0.f, base.Size.Width, base.Size.Height)
        let noobrect = Rectangle(0, 0, int base.Size.Width, int base.Size.Height)
        g.FillRectangle(backcolor, rect)
        g.DrawRectangle(Pens.Black, noobrect)
        if press then
            g.FillEllipse(presscolor, rect)
        if (char <> null) then
            let sz = g.MeasureString(char, this.Parent.Font)
            g.DrawString(char, font, charcolor, PointF((this.Size.Width - sz.Width) / 2.f, (this.Size.Height - sz.Height) / 2.f))

and CUse = Nope|Up|Down|Right|Left|ZoomUp|ZoomDown|RotateR|RotateL|Font

and Controlli() as this =
    inherit LWControl()

    let mutable controlled: Editor = Editor()
    let mutable winx, winy = 0.f, 0.f
    let initialize = PointF(0.f, 0.f)
    let brush = Brushes.Gray
    let brushs = Brushes.WhiteSmoke
    let timer = new Timer(Interval=100)
    let mutable conuse = Nope 

    let up = new MyButton()
    let down = new MyButton()
    let left = new MyButton()
    let right = new MyButton()
    let zoomup = new MyButton()
    let zoomdown = new MyButton()
    let rotatel = new MyButton()
    let rotater = new MyButton()

    do
        timer.Tick.Add(fun _ -> controlled.Move conuse)
        timer.Start()

        up.Parent <- this.Parent
        up.Size <- SizeF(lato,lato)
        up.Char <- "▲"
        up.Position <- initialize
        up.BackColor <- brush
        up.PressColor <- brushs
        up.MouseDown.Add(fun _ -> conuse <- Up; up.Press <- true)
        up.MouseUp.Add(fun _ -> conuse <- Nope; up.Press <- false)
  
        down.Parent <- this.Parent
        down.Size <- SizeF(lato,lato)
        down.Char <- "▼"
        down.Position <- initialize
        down.BackColor <- brush
        down.PressColor <- brushs
        down.MouseDown.Add(fun _ -> conuse <- Down; down.Press <- true)
        down.MouseUp.Add(fun _ -> conuse <- Nope; down.Press <- false)
        
        left.Parent <- this.Parent
        left.Size <- SizeF(lato,lato)
        left.Char <- "◄"
        left.Position <- initialize
        left.BackColor <- brush
        left.PressColor <- brushs
        left.MouseDown.Add(fun _ -> conuse <- Left; left.Press <- true)
        left.MouseUp.Add(fun _ -> conuse <- Nope; left.Press <- false)

        right.Parent <- this.Parent
        right.Size <- SizeF(lato,lato)
        right.Char <- "►"
        right.Position <- initialize
        right.BackColor <- brush
        right.PressColor <- brushs
        right.MouseDown.Add(fun _ -> conuse <- Right; right.Press <- true)
        right.MouseUp.Add(fun _ -> conuse <- Nope; right.Press <- false)
        
        zoomup.Parent <- this.Parent
        zoomup.Size <- SizeF(lato,lato)
        zoomup.Char <- "+"
        zoomup.Position <- initialize
        zoomup.BackColor <- brush
        zoomup.PressColor <- brushs
        zoomup.MouseDown.Add(fun _ -> conuse <- ZoomUp; zoomup.Press <- true)
        zoomup.MouseUp.Add(fun _ -> conuse <- Nope; zoomup.Press <- false)
        
        zoomdown.Parent <- this.Parent
        zoomdown.Size <- SizeF(lato,lato)
        zoomdown.Char <- "-"
        zoomdown.Position <- initialize
        zoomdown.BackColor <- brush
        zoomdown.PressColor <- brushs
        zoomdown.MouseDown.Add(fun _ -> conuse <- ZoomDown; zoomdown.Press <- true)
        zoomdown.MouseUp.Add(fun _ -> conuse <- Nope; zoomdown.Press <- false)
        
        rotatel.Parent <- this.Parent
        rotatel.Size <- SizeF(lato,lato)
        rotatel.Char <- "L"
        rotatel.Position <- initialize
        rotatel.BackColor <- brush
        rotatel.PressColor <- brushs
        rotatel.MouseDown.Add(fun _ -> conuse <- RotateL; rotatel.Press <- true)
        rotatel.MouseUp.Add(fun _ -> conuse <- Nope; rotatel.Press <- false)
        
        rotater.Parent <- this.Parent
        rotater.Size <- SizeF(lato,lato)
        rotater.Char <- "R"
        rotater.Position <- initialize
        rotater.BackColor <- brush
        rotater.PressColor <- brushs
        rotater.MouseDown.Add(fun _ -> conuse <- RotateR; rotater.Press <- true)
        rotater.MouseUp.Add(fun _ -> conuse <- Nope; rotater.Press <- false)
    done

    member this.Controlled
        with get() = controlled
        and set(v) = controlled <- v

    member this.Parent
        with get() = base.Parent
        and set(v:LWContainer) = do
            base.Parent <- v
            v.LWControls.Add(up)
            v.LWControls.Add(down)
            v.LWControls.Add(left)
            v.LWControls.Add(right)
            v.LWControls.Add(zoomup)
            v.LWControls.Add(zoomdown)
            v.LWControls.Add(rotater)
            v.LWControls.Add(rotatel)

    override this.OnResize _ =
        winx <- single this.Parent.ClientSize.Width
        winy <- single this.Parent.ClientSize.Height
        up.Position <- PointF(winx-lato,lato)
        down.Position <- PointF(winx-lato,winy-2.f*lato)
        left.Position <- PointF(0.f,winy-lato)
        right.Position <- PointF(winx-2.f*lato,winy-lato)
        zoomup.Position <- PointF((winx/2.f)-(2.f*lato),winy-2.f*lato)
        zoomdown.Position <- PointF((winx/2.f)-lato,winy-2.f*lato)
        rotatel.Position <- PointF((winx/2.f),winy-2.f*lato)
        rotater.Position <- PointF((winx/2.f)+lato,winy-2.f*lato)
        this.Invalidate()

    override this.OnPaint e =
        let g = e.Graphics
        g.FillRectangle(brush, winx-lato, winy-lato, lato, lato)

and Carattere() as this =
    inherit LWControl()

    let mutable fsize = 8.f
    let mutable fonts = [|new Font("Arial", fsize); new Font("Times New Roman", fsize); new Font("Arial Black", fsize); new Font("Segoe Script", fsize); new Font("Comic Sans MS", fsize); new Font("Cambria", fsize)|]

    let mutable char = null
    let mutable fontstate = 0
    let mutable font = fonts.[0]
    let mutable brush = Brushes.Black
    let mutable bbrush = Brushes.LightBlue
    let mutable press = None

    member this.Char 
        with get() = char
        and set(v) = char <- v

    member this.Font 
        with get() = font
        and set(v) = 
            font <- v
            fontstate <- -1

    member this.FontState 
        with get() = fontstate
        and set(v) = 
            fontstate <- v
            font <- fonts.[v]
    
    member this.Brush 
        with get() = brush
        and set(v) = brush <- v
    
    member this.Size with get() = base.Size

    override this.OnPaint e =
        let g = e.Graphics
        base.Size <- g.MeasureString(char, font) //non mi vengono in mente altre idee per mantenere consistente la size
        g.Transform <- this.Matrixs.W2V
        if this.Select then
            g.FillRectangle(bbrush, RectangleF(PointF(0.f,0.f), this.Size))
        g.DrawString (char, font, brush, PointF(0.f,0.f))

    override this.OnMouseDown e =
        this.Select <- true
        let p = [| PointF(single e.X,single e.Y) |]
        this.Matrixs.V2W.TransformPoints p
        press <- Some (p.[0])

    override this.OnMouseUp e =
        press <- None

    override this.OnMouseMove e =
        if (press <> None) then
            let p = [| PointF(single e.X, single e.Y) |]
            this.Matrixs.V2W.TransformPoints p
            let x = [|PointF(this.Position.X + (single p.[0].X - press.Value.X) , this.Position.Y + (single p.[0].Y - press.Value.Y))|]
            this.Position <- x.[0]

    member this.NextFont =
        fontstate <- fontstate + 1
        if (fontstate = fonts.Length) then
            fontstate <- 0
        font <- fonts.[fontstate] 

    member this.Zoom c =
        match c with
        | ZoomUp -> fsize <- fsize+1.f
        | ZoomDown -> if fsize > 1.f then fsize <- fsize-1.f
        | _ -> ()
        fonts <- [|new Font("Arial", fsize); new Font("Times New Roman", fsize); new Font("Arial Black", fsize); new Font("Segoe Script", fsize); new Font("Comic Sans MS", fsize); new Font("Cambria", fsize)|]
        font <- fonts.[fontstate]

and ControlliEditor() as this =
    inherit LWControl()

    let mutable controlled = ResizeArray<Carattere>()
    let mutable parent : LWContainer = LWContainer()
    let brush = Brushes.Gray
    let brushs = Brushes.WhiteSmoke
    let timer = new Timer(Interval=100)
    let timeranimation = new Timer(Interval=100)
    let mutable conuse = Nope 
    let mutable slowerfont = 10
    let mutable animationstate = false
    let mutable ctrl = false

    let rotatel = new MyButton()
    let rotater = new MyButton()
    let zoomup = new MyButton()
    let zoomdown = new MyButton()
    let fontchange = new MyButton()
    let animation = new MyButton()

    let aplay x =
        let mutable b = false
        
        if (slowerfont = 10) then
            b <- true
            slowerfont <- 0
        else
            slowerfont <- slowerfont + 1
        
        for idx in 0 .. (controlled.Count - 1) do
            let c = controlled.[idx]
            let p = PointF(c.Size.Width/2.f, c.Size.Height/2.f)
            match x with
            | RotateL -> c.Matrixs.XRotate(-1.f, p)
            | RotateR -> c.Matrixs.XRotate(1.f, p)
            | ZoomUp -> c.Zoom ZoomUp
            | ZoomDown -> c.Zoom ZoomDown
            | Font -> if b then c.NextFont
            | _ -> ()

    do
        timer.Tick.Add(fun _ -> do
            match conuse with
            | RotateL -> aplay RotateL; this.Invalidate()
            | RotateR -> aplay RotateR; this.Invalidate()
            | ZoomUp -> aplay ZoomUp; this.Invalidate()
            | ZoomDown -> aplay ZoomDown; this.Invalidate()
            | Font -> aplay Font; this.Invalidate()
            | _ -> ()
            )
        timer.Start()

        timeranimation.Tick.Add(fun _ -> do
            for idx in 0 .. (controlled.Count - 1) do
                let c = controlled.[idx]
                let p = PointF(c.Size.Width/2.f, c.Size.Height/2.f)
                c.Matrixs.XRotate(1.f, p)
            )

        base.Size <- SizeF(single base.Parent.ClientSize.Width, lato)

        rotatel.Parent <- this.Parent
        rotatel.Size <- SizeF(lato,lato)
        rotatel.Char <- "L"
        rotatel.Position <- PointF(0.f,0.f)
        rotatel.BackColor <- brush
        rotatel.PressColor <- brushs
        rotatel.MouseDown.Add(fun _ -> conuse <- RotateL; rotatel.Press <- true)
        rotatel.MouseUp.Add(fun _ -> conuse <- Nope; rotatel.Press <- false)
        
        rotater.Parent <- this.Parent
        rotater.Size <- SizeF(lato,lato)
        rotater.Char <- "R"
        rotater.Position <- PointF(lato,0.f)
        rotater.BackColor <- brush
        rotater.PressColor <- brushs
        rotater.MouseDown.Add(fun _ -> conuse <- RotateR; rotater.Press <- true)
        rotater.MouseUp.Add(fun _ -> conuse <- Nope; rotater.Press <- false)

        zoomup.Parent <- this.Parent
        zoomup.Size <- SizeF(lato,lato)
        zoomup.Char <- "+"
        zoomup.Position <- PointF(3.f*lato,0.f)
        zoomup.BackColor <- brush
        zoomup.PressColor <- brushs
        zoomup.MouseDown.Add(fun _ -> conuse <- ZoomUp; zoomup.Press <- true)
        zoomup.MouseUp.Add(fun _ -> conuse <- Nope; zoomup.Press <- false)
        
        zoomdown.Parent <- this.Parent
        zoomdown.Size <- SizeF(lato,lato)
        zoomdown.Char <- "-"
        zoomdown.Position <- PointF(4.f*lato,0.f)
        zoomdown.BackColor <- brush
        zoomdown.PressColor <- brushs
        zoomdown.MouseDown.Add(fun _ -> conuse <- ZoomDown; zoomdown.Press <- true)
        zoomdown.MouseUp.Add(fun _ -> conuse <- Nope; zoomdown.Press <- false)

        fontchange.Parent <- this.Parent
        fontchange.Size <- SizeF(lato,lato)
        fontchange.Char <- "F"
        fontchange.Position <- PointF(6.f*lato,0.f)
        fontchange.BackColor <- brush
        fontchange.PressColor <- brushs
        fontchange.MouseDown.Add(fun _ -> conuse <- Font; fontchange.Press <- true)
        fontchange.MouseUp.Add(fun _ -> conuse <- Nope; slowerfont <- 10; fontchange.Press <- false)

        animation.Parent <- this.Parent
        animation.Size <- SizeF(lato,lato)
        animation.Char <- "»"
        animation.Position <- PointF(7.f*lato,0.f)
        animation.BackColor <- brush
        animation.PressColor <- brushs
        animation.MouseDown.Add(fun _ ->    animation.Press <- not(animationstate); 
                                            animationstate <- not(animationstate); 
                                            if animationstate then timeranimation.Start()
                                                else timeranimation.Stop()
                                            this.Invalidate()
                                            )
    done

    member this.Controlled with get() = controlled

    member this.Size with get() = base.Size

    member this.Ctrl 
        with get() = ctrl
        and set(v) = ctrl <- v

    override this.OnPaint e =
        let g = e.Graphics
        let rect = RectangleF(PointF(0.f,0.f), this.Size)
        g.FillRectangle(brush, rect)
        let char = "CTRL"
        let mutable charcolor = Brushes.White
        if ctrl then
            charcolor <- Brushes.LightBlue
        let mutable font = new Font("Arial Black", 10.f)
        let sz = g.MeasureString(char, this.Parent.Font)
        g.DrawString(char, font, charcolor, 9.f*lato, 0.f)

    override this.OnResize e =
        base.Size <- SizeF(single base.Parent.ClientSize.Width, lato)
        this.Invalidate()
        
    member this.Parent
        with get() = base.Parent
        and set(v:LWContainer) = do
            base.Parent <- v
            parent <- v
            parent.LWControls.Add(rotater)
            parent.LWControls.Add(rotatel)
            parent.LWControls.Add(zoomup)
            parent.LWControls.Add(zoomdown)
            parent.LWControls.Add(fontchange)
            parent.LWControls.Add(animation)

and Editor() as this =
    inherit LWControl()
    
    let mutable caratteri = ResizeArray<Carattere>()
    let mutable interline = 1.1f
    let mutable tick = 500
    let mutable tickstate = true
    let mutable edit = ControlliEditor()
    let mutable next = PointF(0.f,0.f)
    let mutable y = 20.f
    let mutable x = 20.f

    let ETranslate x y =
        this.Matrixs.CTranslate(x,y)
        for idx in 0 .. (caratteri.Count - 1) do
            let c = caratteri.[idx]
            c.Matrixs.CTranslate(x,y)

    let ERotate a b =
        this.Matrixs.CRotate(a, b)
        for idx in 0 .. (caratteri.Count - 1) do
            let c = caratteri.[idx]
            c.Matrixs.CRotate(a, b)

    let EScale x y b =
        this.Matrixs.CScale(x, y, b)
        for idx in 0 .. (caratteri.Count - 1) do
            let c = caratteri.[idx]
            c.Matrixs.CScale(x, y, b)

    let timer = new Timer()
    do
        timer.Interval <- tick
        timer.Start()
        timer.Tick.Add(fun _ -> tickstate <- not(tickstate); this.Invalidate())
    done

    member this.Edit
        with get() = edit
        and set(v) = edit <- v

    member this.Tick
        with get() = tick
        and set(v) = tick <- v

    override this.OnResize _ =
        this.Invalidate()

    override this.OnKeyDown e =
        match e.KeyCode with
        | Keys.ControlKey -> edit.Ctrl <- not(edit.Ctrl)
        | Keys.Back -> do if (caratteri.Count > 0) then caratteri.RemoveAt (caratteri.Count-1)
        | Keys.Delete -> for idx in (caratteri.Count - 1) .. -1 .. 0 do
                            if caratteri.[idx].Select then
                                caratteri.RemoveAt(idx)
        | Keys.F1 -> this.Move Up
        | Keys.F2 -> this.Move Down
        | Keys.F3 -> this.Move Right
        | Keys.F4 -> this.Move Left
        | Keys.F5 -> this.Move ZoomUp
        | Keys.F6 -> this.Move ZoomDown
        | Keys.F7 -> this.Move RotateL
        | Keys.F8 -> this.Move RotateR
        | _ -> do match (caratteri |> Seq.tryFindIndex (fun c -> c.Select)) with
                                | Some i -> if  ((65 <= e.KeyValue) && (e.KeyValue <= 90)) then
                                                for idx in i .. (caratteri.Count - 1) do
                                                    if caratteri.[idx].Select then
                                                        caratteri.[idx].Char <- e.KeyCode.ToString()
                                | None -> do
                                           if  (((65 <= e.KeyValue) && (e.KeyValue <= 90)) || e.KeyValue=32 || e.KeyValue=13) then
                                                match e.KeyValue with
                                                | 13 -> do
                                                            next.Y <- next.Y + y * interline
                                                            next.X <- 0.f
                                                | _ -> do
                                                            if (e.KeyValue <> 32) then
                                                                let c = new Carattere()
                                                                c.Char <- e.KeyCode.ToString()
                                                                let t = this.Matrixs
                                                                c.Matrixs <- t
                                                                c.Matrixs.NTranslate(next.X, next.Y)
                                                                caratteri.Add c
                                                            let aux = next.X + 2.f*x
                                                            if (aux >= this.Size.Width) then
                                                                next.X <- 0.f
                                                                next.Y <- next.Y + y * interline
                                                            else
                                                                next.X <- aux-x
                                                if (next.Y >= (this.Size.Height-y)) then
                                                        next.Y <- 0.f
                                           done
        this.Invalidate()

    override this.OnPaint e = 
        let g = e.Graphics
        g.Transform <- this.Matrixs.W2V
        let t = g.Transform
        let rect = RectangleF(PointF(0.f,0.f), this.Size)
        g.FillRectangle(Brushes.White, rect)
        y <- g.MeasureString("M", this.Parent.Font).Height*interline
        x <- g.MeasureString("M", this.Parent.Font).Width
        if caratteri.Count <> 0 then
            for i in 0 .. caratteri.Count-1 do
                let csize = caratteri.[i].Size
                let points = [|PointF(0.f,0.f);PointF(0.f,csize.Height);PointF(csize.Width,0.f);PointF(csize.Width,csize.Height)|]
                caratteri.[i].Matrixs.W2V.TransformPoints points
                this.Matrixs.V2W.TransformPoints points
                let mutable b = true
                for i in 0 .. 3 do
                    b <- (b && rect.Contains(points.[i]))
                if b then
                    caratteri.[i].OnPaint e
        if (tickstate && this.Select) then
            t.Translate(next.X,next.Y)
            g.Transform <- t
            g.DrawLine(Pens.Black, PointF(0.f,0.f), PointF(0.f, y))    
    
    override this.OnMouseDown e =
        this.Select <- true
        let p = PointF(single e.X, single e.Y)
        if not(edit.Ctrl) then
            for idx in 0 .. (caratteri.Count - 1) do
                let c = caratteri.[idx]
                c.Select <- false
            edit.Controlled.Clear()
        match (caratteri |> Seq.tryFind (fun c -> c.HitTest p)) with
        | Some c -> do
            if not(c.Select) then
                edit.Controlled.Add(c)
                c.Select <- true
            for idx in 0 .. (caratteri.Count - 1) do
                if caratteri.[idx].Select then
                    caratteri.[idx].OnMouseDown(e)
        | None -> do
            let aux = [|p|]
            this.Matrixs.V2W.TransformPoints aux
            if (aux.[0].X<=(this.Size.Width-x) && (aux.[0].Y-y)>=0.f) then
                next.X <- aux.[0].X
                next.Y <- aux.[0].Y - y
                this.Invalidate()

    override this.OnMouseMove e =
        match (caratteri |> Seq.tryFindIndex (fun c -> c.Select)) with
        | Some i -> do
            for idx in i .. (caratteri.Count - 1) do
                if caratteri.[idx].Select then
                    caratteri.[idx].OnMouseMove(e)
        | None -> ()

    override this.OnMouseUp e = 
        match (caratteri |> Seq.tryFindIndex (fun c -> c.Select)) with
        | Some i -> do
            for idx in i .. (caratteri.Count - 1) do
                if caratteri.[idx].Select then
                    caratteri.[idx].OnMouseUp(e)
        | None -> ()
    
    member this.Move e =
        let p = SizeF(single this.Parent.ClientSize.Width, single this.Parent.ClientSize.Height)
        match e with
        | Up -> ETranslate 0.f -1.f
        | Down -> ETranslate 0.f 1.f
        | Left -> ETranslate -1.f 0.f
        | Right -> ETranslate 1.f 0.f
        | ZoomUp -> EScale 1.01f 1.01f p
        | ZoomDown -> EScale 0.99f 0.99f p
        | RotateL -> ERotate -1.f p
        | RotateR -> ERotate 1.f p
        | _ -> ()
        

let f = new Form(Text = "Mid Edit")
f.Show()
let lwc = new LWContainer(Dock=DockStyle.Fill)

let contr = new Controlli()
contr.Parent <- lwc
lwc.LWControls.Add(contr)

let contredit = new ControlliEditor()
contredit.Parent <- lwc
contredit.Position <- PointF(0.f, 0.f)
lwc.LWControls.Add(contredit)

let str = new Editor()
str.Edit <- contredit
contr.Controlled <- str

lwc.LWControls.Add(str)

str.Parent <- lwc
str.Size <- SizeF(300.f,600.f)
str.Position <- PointF(lato,3.f*lato)

lwc.BackColor <- Color.LightGray

f.Controls.Add(lwc)
lwc.Select()

// welcome in Windows 3.1 ♥

#if COMPILED
module BoilerPlateForForm = 
    [<System.STAThread>]
    do ()
    do System.Windows.Forms.Application.Run()
#endif
