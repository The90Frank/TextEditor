#nowarn
open System.Windows.Forms
open System.Drawing
open System.Drawing.Drawing2D

type W2V() as this =
    let mutable w2v = new Drawing2D.Matrix()
    let mutable v2w = new Drawing2D.Matrix()

    member this.NTranslate(tx, ty) =
        w2v.Translate(tx, ty)
        v2w.Translate(-tx, -ty, Drawing2D.MatrixOrder.Append)

    member this.NRotate(a) =
        w2v.Rotate(a)
        v2w.Rotate(-a, Drawing2D.MatrixOrder.Append)

    member this.NScale(sx, sy) =
        w2v.Scale(sx, sy)
        v2w.Scale(1.f/sx, 1.f/sy, Drawing2D.MatrixOrder.Append)
  
    member this.CTranslate(tx, ty) =
        w2v.Translate(tx, ty, Drawing2D.MatrixOrder.Append)
        v2w.Translate(-tx, -ty)

    member this.CRotate(a, cSize:SizeF) =
        let center = [|PointF(single cSize.Width/2.f,single cSize.Height/2.f)|]
        v2w.TransformPoints(center)
        let xc, yc = center.[0].X, center.[0].Y
            
        w2v.Translate(xc,yc)
        v2w.Translate(-xc,-yc,MatrixOrder.Append)

        w2v.Rotate(a)
        v2w.Rotate(-a,MatrixOrder.Append)
            
        w2v.Translate(-xc,-yc)
        v2w.Translate(xc,yc,MatrixOrder.Append)

    member this.CScale(sx, sy, cSize:SizeF) =
        let center = [|PointF(single cSize.Width/2.f,single cSize.Height/2.f)|]
        v2w.TransformPoints(center)
        let xc, yc = center.[0].X, center.[0].Y
            
        w2v.Translate(xc,yc)
        v2w.Translate(-xc,-yc,MatrixOrder.Append)

        w2v.Scale(sx, sy)
        v2w.Scale(1.f/sx, 1.f/sy, Drawing2D.MatrixOrder.Append)
            
        w2v.Translate(-xc,-yc)
        v2w.Translate(xc,yc,MatrixOrder.Append)   
        
    member this.XRotate(a, p:PointF) =
        w2v.RotateAt(a, p)
        v2w.RotateAt(-a, p, MatrixOrder.Append)

    member this.XScale(sx, sy, p:PointF) = 
        let xc, yc = p.X, p.Y
            
        w2v.Translate(xc,yc)
        v2w.Translate(-xc,-yc,MatrixOrder.Append)

        w2v.Scale(sx, sy)
        v2w.Scale(1.f/sx, 1.f/sy, Drawing2D.MatrixOrder.Append)
            
        w2v.Translate(-xc,-yc)
        v2w.Translate(xc,yc,MatrixOrder.Append) 

    member this.W2V 
        with get() = w2v 
        and set(v:Matrix) = w2v <- v.Clone()

    member this.V2W 
        with get() = v2w 
        and set(v:Matrix) = v2w <- v.Clone()