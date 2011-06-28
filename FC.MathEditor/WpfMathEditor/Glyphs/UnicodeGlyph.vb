Partial Public Class UnicodeGlyph : Inherits MathElement

    Public Sub New(C As Integer)
        Me.New(C, Nothing, Nothing)
    End Sub

    Public Sub New(C As Integer, F As Typeface)
        Me.New(C, F, Nothing)
    End Sub

    Public Sub New(C As Integer, F As Typeface, S As Double?)
        ' Field initialization
        Me.C = C

        If F IsNot Nothing Then
            Me.Font = F
        End If

        If S IsNot Nothing Then
            Me._FontSize = S : Me._IsFontSizeDefined = True
        End If

    End Sub

    Private C As Integer ' UnicodeGlyphChar
    Public ReadOnly Property DisplayCharacter() As String
        Get
            Return Char.ConvertFromUtf32(C)
        End Get
    End Property

    Public Overrides Function Clone_Internal() As MathElement
        Return New UnicodeGlyph(C)
    End Function

    Public Overrides Function ToString() As String
        Return C
    End Function

    Protected Overrides Function GetInitialChildrenHelper() As ChildrenHelper
        Return New GlyphChildrenHelper(Me)
    End Function

    Protected Overrides Function GetInitialExportHelper() As ExportHelper
        Return New UnicodeGlyphExportHelper(Me)
    End Function

    Protected Overrides Function GetInitialInputHelper() As InputHelper
        Return New UnicodeGlyphInputHelper(Me)
    End Function
End Class