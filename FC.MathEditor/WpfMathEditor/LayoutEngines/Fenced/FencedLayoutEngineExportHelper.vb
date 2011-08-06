Public Class FencedLayoutEngineExportHelper : Inherits ExportHelper

    Public Sub New(This As FencedLayoutEngine)
        MyBase.New(This)
    End Sub

    Protected Shadows ReadOnly Property This() As FencedLayoutEngine
        Get
            Return MyBase.This
        End Get
    End Property

    Public Overrides Sub AppendKeyboardInput(SB As System.Text.StringBuilder)

    End Sub

    Public Overrides Sub AppendLaTeX(SB As System.Text.StringBuilder)

    End Sub

    Public Overrides Sub AppendMathML(SB As System.Text.StringBuilder)

    End Sub

    Protected Overrides Sub Draw_Internal(DG As System.Windows.Media.DrawingContext)

    End Sub

    Public Overrides Sub GenerateLayout()

    End Sub
End Class
