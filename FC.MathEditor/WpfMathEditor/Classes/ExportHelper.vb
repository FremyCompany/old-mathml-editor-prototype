Public MustInherit Class ExportHelper

    Protected WithEvents This As MathElement
    Public Sub New(ByVal This As MathElement)
        Me.This = This
    End Sub

    Public Function ToKeyboardInput() As String
        Dim SB As New System.Text.StringBuilder()
        AppendKeyboardInput(SB)
        Return SB.ToString()
    End Function

    Public Function ToLaTeX() As String
        Dim SB As New System.Text.StringBuilder()
        AppendLaTeX(SB)
        Return SB.ToString()
    End Function

    Public Function ToMathML() As String
        Dim SB As New System.Text.StringBuilder()
        AppendMathML(SB)
        Return SB.ToString()
    End Function

    Public Function ToBitmap() As DrawingImage
        Dim DV As New DrawingVisual()
        Dim DG = DV.RenderOpen()
        Draw(DG)
        Return New DrawingImage(DV.Drawing)
    End Function

    Public MustOverride Sub AppendKeyboardInput(ByVal SB As System.Text.StringBuilder)
    Public MustOverride Sub AppendLaTeX(ByVal SB As System.Text.StringBuilder)
    Public MustOverride Sub AppendMathML(ByVal SB As System.Text.StringBuilder)

    Public MustOverride Sub Draw(ByVal DG As DrawingContext)

    Private IsLayoutToDate As Boolean = False
    Public Sub InvalidateLayout()
        IsLayoutToDate = False
    End Sub

    Public Property LayoutOptions As LayoutOptions

    Private Sub PerformLayout()
        If IsLayoutToDate Then
            GenerateLayout()
        End If
    End Sub

    ' Fields to be filled by GenerateLayout !
    Protected W As Double
    Protected H As Double

    Public ReadOnly Property Width As Double
        Get
            Return W
        End Get
    End Property

    Public ReadOnly Property Height As Double
        Get
            Return H
        End Get
    End Property

    Public MustOverride Function GetChildLocation(ByVal El As MathElement) As Rect
    Private Shared Function FitRect(ByVal ChildRect As Rect, ByVal InitalParentRect As Rect, ByVal FinalParentRect As Rect) As Rect
        ChildRect.Offset(-CType(InitalParentRect.Location, Vector))
        ChildRect.Scale(FinalParentRect.Width / InitalParentRect.Width, FinalParentRect.Height / InitalParentRect.Height)
        ChildRect.Offset(FinalParentRect.Location)
        Return ChildRect
    End Function

    Public ReadOnly Property Size As Size
        Get
            Return New Size(W, H)
        End Get
    End Property

    Public ReadOnly Property SizeRect As Rect
        Get
            Return New Rect(Size)
        End Get
    End Property

    Public ReadOnly Property LocationInParent As Rect
        Get
            If This.Parent Is Nothing Then
                Return SizeRect
            Else
                Return This.Parent.Export.GetChildLocation(This)
            End If
        End Get
    End Property

    Public ReadOnly Property LocationInRoot As Rect
        Get
            If This.Parent Is Nothing Then Return LocationInParent

            Dim L = LocationInParent

        End Get
    End Property

    Public MustOverride Sub GenerateLayout()


End Class

Public Enum LayoutOptions

    Inline = 2 ^ 0          ' Force inline if not  specified otherwhise
    InlineBlock = 2 ^ 1     ' Use inline if it's computed as beautiful
    Block = 2 ^ 2           ' Use block if not specified otherwhise

End Enum