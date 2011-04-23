Public Class TextEditExportHelper : Inherits RowLayoutEngineExportHelper

    Protected Shadows ReadOnly Property This As TextEdit
        Get
            Return MyBase.This
        End Get
    End Property

    Public Sub New(ByVal T As TextEdit)
        MyBase.New(T)
    End Sub

    Public Overrides ReadOnly Property InitialAboveBaseLineHeight As Double
        Get
            Try
                Dim GT As New GlyphTypeface
                This.Font.TryGetGlyphTypeface(GT)
                Return This.FontSize * GT.Baseline
            Catch ex As Exception
                Return 0
            End Try
        End Get
    End Property

    Public Overrides ReadOnly Property InitialBelowBaseLineHeight As Double
        Get
            Try
                Dim GT As New GlyphTypeface
                This.Font.TryGetGlyphTypeface(GT)
                Return This.FontSize * (GT.Height - GT.Baseline)
            Catch ex As Exception
                Return 0
            End Try
        End Get
    End Property

End Class
