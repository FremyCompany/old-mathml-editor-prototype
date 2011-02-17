Partial Public Class UnicodeGlyph : Inherits MathElement

    Public Sub New(ByVal C As Integer)
        Me.New(C, Nothing, Nothing)
    End Sub

    Public Sub New(ByVal C As Integer, ByVal F As Typeface)
        Me.New(C, F, Nothing)
    End Sub

    Public Sub New(ByVal C As Integer, ByVal F As Typeface, ByVal S As Double?)
        ' Field initialization
        Me.C = C

        ' MathElement properties
        Export = New UnicodeGlyphExportHelper(Me)
        _Children = New GlyphChildrenHelper(Me)

        ' End init
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

End Class